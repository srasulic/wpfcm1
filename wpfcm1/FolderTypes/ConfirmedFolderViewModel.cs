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
    public class ConfirmedFolderViewModel : FolderViewModel, IHandle<MessageXls>, IHandle<MessageValidate>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;
        

        public ConfirmedFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new ConfirmedDocumentModel(new FileInfo(f))));

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
                    document.Processed = false;
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
                old.isValidated2 = state.isValidated2;
                old.IsSignedAgain = state.IsSignedAgain;
                old.isApprovedForProcessing = state.isApprovedForProcessing;
                old.IsAcknowledged = state.IsAcknowledged;
                old.isRejected = state.isRejected;
                old.sigValidationInfo = state.sigValidationInfo;
                old.archiveReady = state.archiveReady;

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
        

            for (int i = Documents.Count - 1; i >= 0; i-- )
            {
                if (Documents[i].IsSignedAgain) Documents.RemoveAt(i);
            }

            foreach (var document in Documents)
            {
                InternalMessengerGetStates(document);
                document.sigAdditionalInfo = "refresh";
            }

            //var states = Deserialize();
            //foreach (var state in states)
            //{
            //    var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
            //    if (found == null) continue;
            //    var old = found as DocumentModel;
            //    old.Processed = state.Processed;
            //    old.IsSignedAgain = state.IsSignedAgain;
            //}
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
                var newDoc = new ConfirmedDocumentModel(new FileInfo(filePath));
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
                Documents.Add(new ConfirmedDocumentModel(new FileInfo(filePath)));
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

        public override void Dispose()
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<ConfirmedDocumentModel> items = Documents.Cast<ConfirmedDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<ConfirmedDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<ConfirmedDocumentModel> Deserialize()
        {
            var oldList = new List<ConfirmedDocumentModel>();
            var xs = new XmlSerializer(typeof(List<ConfirmedDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<ConfirmedDocumentModel>)xs.Deserialize(s);
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

            var view = ec.View as ConfirmedFolderView;
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
