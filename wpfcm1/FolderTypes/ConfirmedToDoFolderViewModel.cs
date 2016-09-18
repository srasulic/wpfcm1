using Caliburn.Micro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;
using System;
using System.Text.RegularExpressions;

namespace wpfcm1.FolderTypes
{
    public class ConfirmedToDoFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageXls>, IHandle<MessageReject>, IHandle<MessageValidate>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;
        
        public ConfirmedToDoFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new ConfirmedToDoDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            if (Documents.Count == 0) return;
            foreach (var document in Documents)
            {
                if (!Regex.IsMatch(document.DocumentPath, @".+_s.pdf$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                if (Regex.IsMatch(document.DocumentPath, @".+_s_s.pdf$", RegexOptions.IgnoreCase))
                {
                    document.HasSecondSignature = true;
                    document.IsSignedAgain = false;
                    document.Processed = true;
                    continue;
                }
                var found = Documents.FirstOrDefault(d => d.DocumentPath == Regex.Replace(document.DocumentPath, @"_s.pdf", @"_s_s.pdf", RegexOptions.IgnoreCase));
                if ( found == null ) {
                    document.HasSecondSignature = false;
                    document.IsSignedAgain = false;
                    document.Processed = false;
                } else {
                    document.HasSecondSignature = false;
                    document.IsSignedAgain = true;
                    document.Processed = true;
                }
            }

            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as DocumentModel;
                old.Processed = state.Processed;
                old.IsValid = state.IsValid;
                old.isValidated = state.isValidated;
                old.IsSignedAgain = state.IsSignedAgain; 
                old.isApprovedForProcessing = state.isApprovedForProcessing;
                old.IsAcknowledged = state.IsAcknowledged;
                old.isRejected = state.isRejected;
                old.sigValidationInfo = state.sigValidationInfo;

                old.sigReason = state.sigReason;
                old.sigTS = state.sigTS;
                old.sigDateSigned = state.sigDateSigned;
                old.sigSignerName = state.sigSignerName;
                old.sigOrg = state.sigOrg;

                old.sigReason2 = state.sigReason2;
                old.sigTS2 = state.sigTS2;
                old.sigDateSigned2 = state.sigDateSigned2;
                old.sigSignerName2 = state.sigSignerName2;
                old.sigOrg2 = state.sigOrg2;
            }
        
            for (int i = Documents.Count - 1; i >= 0; i--)
            {
                if (Documents[i].IsSignedAgain || Documents[i].Processed || Documents[i].HasSecondSignature) Documents.RemoveAt(i);
            }

            foreach (var document in Documents)
            {
                InternalMessengerGetStates(document);
                SetSigAdditionalInfo(document);
            }
        }
    

        protected override void AddFile(string filePath)
        {
            // ako je zavrsio sinh, azuriraj statuse poruka iz xml fajlova
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
            }
            // ako je stigao xml pronadji njegov pdf u listi i setuj mu status da ima eksternu poruku
            // (ovde je problem ako nam xml stigne u istoj sinh pre pdf fajla) ?!
            if (Regex.IsMatch(filePath, @".+.pdf.xml$", RegexOptions.IgnoreCase))
            {
                var docName = Regex.Replace(filePath, @".xml", "");
                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
                if (!(found == null)) found.hasExternalMessage = true;  // ftp ih još ne spusti do kraja kada probamo da ih pročitamo... inače zbog sorta xml stiže nakon pdf-a, tako da ovo radi...  
            }
            // ack fajl samo ignorisemo
            else if (Regex.IsMatch(filePath, @".+.pdf.ack$", RegexOptions.IgnoreCase))
            {
                // ovo ne bi smelo da se desi... ove ack-ove nemamo kako da tretiramo, ignorisemo ih
            }
            // ako je stigao obostrano potpisan, dodajemo ga u listu a njegovom _s fajlu promenimo status
            else if (Regex.IsMatch(filePath, @".+_s_s.pdf$", RegexOptions.IgnoreCase))
            {
                var docName = Regex.Replace(filePath, @"_s_s", "_s");
                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
                var newDoc = new ConfirmedToDoDocumentModel(new FileInfo(filePath));
                newDoc.HasSecondSignature = true;
                newDoc.IsSignedAgain = false;
                newDoc.Processed = true;
                Documents.Add(newDoc);
                if (!(found == null))
                {
                    found.IsSignedAgain = true;
                    found.Processed = true;
                }
            }
            // ostale samo dodamo u listu
            else
            {
                Documents.Add(new ConfirmedToDoDocumentModel(new FileInfo(filePath)));
            }
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //TODO: hack: checkbox checkmark moze da se izgubi prilikom promene taba, ako promena nije komitovana
            var v = GetView() as UserControl;
            var dg = v.FindName("Documents") as DataGrid;
            dg.CommitEdit(DataGridEditingUnit.Row, true);
        }

        public async void Handle(MessageValidate message)
        {
            if (!IsActive) return;
            await ValidateDocSignaturesAsync();
        }

        public void Handle(CertificateModel message)
        {
            _certificate = message;
        }

        public void Handle(MessageXls message)
        {
            if (!IsActive) return;
            XlsExport();

        }

        public void Handle(MessageSign message)
        {
            if (IsActive)
            {
                // izmesteno na OnStart dijaloga
                //PsKillPdfHandlers(); // workaround - pskill ubija sve procese koji rade nad PDF-ovima u eDokument 
                var certificateOk = _certificate != null && _certificate.IsQualified;
                if (!certificateOk) return;
                var validDocuments = GetDocumentsForSigning();
                if (!validDocuments.Any()) return;

                //TODO: ovo mora drugacije
               // _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty)); // prebaceno u dijalog na OnStart; tamo se koristi veći čekić (pskill)
                var result = _windowManager.ShowDialog(new DialogSignViewModel(_certificate, this));
            }
        }

        public void Handle(MessageReject message)
        {
            if (!IsActive) return;
            SetRejected();
        }

        public void SetApproved(bool approved)
        {
            if (!IsActive) return;

            var documents = GetDocumentsForSigning();
            foreach (var document in documents)
            {
                document.isApprovedForProcessing = approved;
                document.isRejected = !approved;
                //if (document.Processed) { document.Processed = false; };
                SerializeMessage(document);
                //document.Processed = true;
            }
        }

        public IList<DocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<DocumentModel>();
            var validDocuments = checkedDocuments.Where(d => !d.Processed && !d.HasSecondSignature && !d.IsSignedAgain).Cast<DocumentModel>().ToList();
            return validDocuments;
        }

        public override void Dispose()
        {
            Serialize();
        }

        private void Serialize()
        {
            // serijalizujemo ceo folder
            var AllDocuments = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new ConfirmedToDoDocumentModel(new FileInfo(f))));


            foreach (var document in AllDocuments)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == document.DocumentPath);
                if (found == null)
                {
                    document.Processed = true;
                }
                else
                {
                    var docForSerialization = found as DocumentModel;
                    document.Processed = docForSerialization.Processed ;
                    document.IsSignedAgain = docForSerialization.IsSignedAgain;
                    document.HasSecondSignature= docForSerialization.HasSecondSignature;

                    document.IsValid = docForSerialization.IsValid;
                    document.isValidated = docForSerialization.isValidated;

                    document.isApprovedForProcessing = docForSerialization.isApprovedForProcessing;
                    document.IsAcknowledged = docForSerialization.IsAcknowledged;

                    document.isRejected = docForSerialization.isRejected;
                    document.sigValidationInfo = docForSerialization.sigValidationInfo;

                    document.sigReason = docForSerialization.sigReason;
                    document.sigTS = docForSerialization.sigTS;
                    document.sigDateSigned = docForSerialization.sigDateSigned;
                    document.sigSignerName = docForSerialization.sigSignerName;
                    document.sigOrg = docForSerialization.sigOrg;

                    document.sigReason2 = docForSerialization.sigReason2;
                    document.sigTS2 = docForSerialization.sigTS2;
                    document.sigDateSigned2 = docForSerialization.sigDateSigned2;
                    document.sigSignerName2 = docForSerialization.sigSignerName2;
                    document.sigOrg2 = docForSerialization.sigOrg2;
                }
            }
            
            // sada su svi koji su bili i ToDo listi zapamceni a svo koji nisu bili tu su obelezeni kao procesirani
            var filePath = Path.Combine(FolderPath, "stateToDo.xml");
            var file = File.Create(filePath);
            List<ConfirmedToDoDocumentModel> items = AllDocuments.Cast<ConfirmedToDoDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<ConfirmedToDoDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }



        private List<ConfirmedToDoDocumentModel> Deserialize()
        {
            var oldList = new List<ConfirmedToDoDocumentModel>();
            var xs = new XmlSerializer(typeof(List<ConfirmedToDoDocumentModel>));
            var file = Path.Combine(FolderPath, "stateToDo.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<ConfirmedToDoDocumentModel>)xs.Deserialize(s);
            }
            catch
            {
                
            }
            return oldList;
        }

        public override void OnCheck(object e)
        {
            var ec = e as ActionExecutionContext;
            var cb = ec.Source as CheckBox;

            var view = ec.View as ConfirmedToDoFolderView;
            var dg = view.Documents;
            var items = dg.SelectedItems;
            foreach (var item in items)
            {
                var doc = item as DocumentModel;
                doc.IsChecked = cb.IsChecked.GetValueOrDefault();
            }
        }
    }
}
