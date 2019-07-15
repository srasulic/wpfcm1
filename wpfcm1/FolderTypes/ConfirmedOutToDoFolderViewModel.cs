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
using System.Windows.Data;

namespace wpfcm1.FolderTypes
{
    public class ConfirmedOutToDoFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageXls>, IHandle<MessageReject>, IHandle<MessageArchive>, IHandle<MessageValidate>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;
        
        public ConfirmedOutToDoFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath) 
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new ConfirmedOutToDoDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            foreach (var document in Documents)
            {
                // necemo u listu dodavati one koji nisu validno nazvan pdf
                if (!Regex.IsMatch(document.DocumentPath, @".+_s.pdf$", RegexOptions.IgnoreCase))
                {
                    continue;
                }
                // setujemo osobine za _s_s
                if (Regex.IsMatch(document.DocumentPath, @".+_s_s.pdf$", RegexOptions.IgnoreCase))
                {
                    document.HasSecondSignature = true;
                    document.IsSignedAgain = false;
                    document.Processed = false;
                    continue;
                }
                // za dokument _s, pokusamo da pronadjemo pripadajuci _s_s i setujemo osobine (necemo prikazivati oba)
                var found = Documents.FirstOrDefault(d => d.DocumentPath == Regex.Replace(document.DocumentPath, @"_s.pdf", @"_s_s.pdf", RegexOptions.IgnoreCase));
                if ( found == null ) {
                    document.HasSecondSignature = false;
                    document.IsSignedAgain = false;
                    document.Processed = false;
                } else {
                    document.HasSecondSignature = false;
                    document.IsSignedAgain = true;
                    document.Processed = false;
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

                old.namePib1Name = state.namePib1Name;
                old.namePib2Name = state.namePib2Name;

            }
        
            for (int i = Documents.Count - 1; i >= 0; i--)
            {
                if (Documents[i].IsSignedAgain || Documents[i].Processed )
                {
                    Documents.RemoveAt(i);
                }
            }

            foreach (var document in Documents)
            {
                InternalMessengerGetStates(document);
                document.sigAdditionalInfo = "refresh";
            }
        }


        protected override void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
            } 
            else if (Regex.IsMatch(filePath, @".+.pdf.xml$", RegexOptions.IgnoreCase))
            {
                var docName = Regex.Replace(filePath, @".xml", "");
                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
                if (!(found == null)) found.hasExternalMessage = true;  //  InternalMessengerGetStates(found);
            }
            else if (Regex.IsMatch(filePath, @".+.pdf.ack$", RegexOptions.IgnoreCase))
            {
                // ovo ne bi smelo da se desi... ove ack-ove nemamo kako da tretiramo, ignorisemo ih
            }
            else if (Regex.IsMatch(filePath, @".+.pdf.jpg$",  RegexOptions.IgnoreCase))
            {
                // ovo ne bi smelo da se desi... ove ack-ove nemamo kako da tretiramo, ignorisemo ih
            }
            else if (Regex.IsMatch(filePath, @".+_s_s.pdf$", RegexOptions.IgnoreCase))
            {
                var docName = Regex.Replace(filePath, @"_s_s", "_s");
                var found = Documents.Where(d => d.DocumentPath == docName).FirstOrDefault();
                found.HasSecondSignature = false;
                found.IsSignedAgain = true;
                found.Processed = true;

                var newDoc = new ConfirmedOutToDoDocumentModel(new FileInfo(filePath));
                newDoc.HasSecondSignature = true;
                newDoc.IsSignedAgain = false;
                newDoc.Processed = false;
                Documents.Add(newDoc);

            }
            else
            {
                Documents.Add(new ConfirmedOutToDoDocumentModel(new FileInfo(filePath)));
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
            var dg = v.FindName("DocumentsCV") as DataGrid;
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


        public void Handle(MessageReject message)
        {
            if (!IsActive) return;
            SetRejected();
            SetArchived();
        }

        public void Handle(MessageArchive message)
        {
            if (!IsActive) return;
            SetArchived();
        }



        //public IList<DocumentModel> GetDocumentsForSigning()
        //{
        //    var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<DocumentModel>();
        //    var validDocuments = checkedDocuments.Where(d => !d.Processed && !d.HasSecondSignature && !d.IsSignedAgain).Cast<DocumentModel>().ToList();
        //    return validDocuments;
        //}

        public override void Dispose(bool disposing)
        {
            Serialize();
        }

        private void Serialize()
        {
            // serijalizujemo ceo folder
            var AllDocuments = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new ConfirmedOutToDoDocumentModel(new FileInfo(f))));


            foreach (var document in AllDocuments)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == document.DocumentPath);
                if (found == null)
                {
                    document.Processed = true;
                    // FIX - prilikom serijalizacije ToDo foldera većini dokumenata (koji su arhivirani) nije u ovom trenutku
                    //    dostupan naziv komitenata. Zato su svi prolazili prilikom serijalizacije sa upitom ka serveru 
                    //    jer se takav poziv nalazi u get metodi ...
                    //    Ovom ispravkom se smanjilo vreme zatvaranja aplikacije 
                    document.namePib1Name = "x";
                    document.namePib2Name = "x";
                }
                else
                {
                    var docForSerialization = found as DocumentModel;
                    document.Processed = docForSerialization.Processed ;
                    document.IsSignedAgain = docForSerialization.IsSignedAgain;
                    document.HasSecondSignature= docForSerialization.HasSecondSignature;

                    document.IsValid = docForSerialization.IsValid;
                    document.isValidated = docForSerialization.isValidated;
                    document.isValidated2 = docForSerialization.isValidated2;
                    document.sigValidationInfo = docForSerialization.sigValidationInfo;

                    document.isApprovedForProcessing = docForSerialization.isApprovedForProcessing;
                    document.IsAcknowledged = docForSerialization.IsAcknowledged;
                    document.isRejected = docForSerialization.isRejected;

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

                    document.namePib1Name = docForSerialization.namePib1Name;
                    document.namePib2Name = docForSerialization.namePib2Name;
                }
            }
            
            // sada su svi koji su bili i ToDo listi zapamceni a svo koji nisu bili tu su obelezeni kao procesirani
            var filePath = Path.Combine(FolderPath, "stateToDo.xml");
            var file = File.Create(filePath);
            List<ConfirmedOutToDoDocumentModel> items = AllDocuments.Cast<ConfirmedOutToDoDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<ConfirmedOutToDoDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }



        //public void InternalMessengerGetStates(DocumentModel document)
        //{
        //    var extDocState = new DocumentModel();
        //    var xs = new XmlSerializer(typeof(ConfirmedOutToDoDocumentModel));

        //    var fileName = Path.GetFileName(document.DocumentPath);
        //    var file = Path.Combine(FolderPath, fileName + ".xml");
        //    if (!File.Exists(file)) return;
        //    try
        //    {
        //        using (Stream s = File.OpenRead(file))
        //            extDocState = (DocumentModel)xs.Deserialize(s);
        //        document.isApprovedForProcessing = extDocState.isApprovedForProcessing;
        //        document.IsAcknowledged = true;
        //        if (extDocState.isRejected == true) { document.isRejected = true; document.Processed = true; }
        //        // ne sme jer ga koristimo za izbacivanje iz ToDo liste
        //        //document.Processed = true;
        //    }
        //    catch
        //    {

        //    }
        //}

        private List<ConfirmedOutToDoDocumentModel> Deserialize()
        {
            var oldList = new List<ConfirmedOutToDoDocumentModel>();
            var xs = new XmlSerializer(typeof(List<ConfirmedOutToDoDocumentModel>));
            var file = Path.Combine(FolderPath, "stateToDo.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<ConfirmedOutToDoDocumentModel>)xs.Deserialize(s);
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

            var view = ec.View as ConfirmedOutToDoFolderView;
            var dg = view.DocumentsCV;
            var items = dg.SelectedItems;
            if (items.Count > 1)
            {
                foreach (var item in items)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }
            }
            else
            {
                foreach (var item in DocumentsCV)
                {
                    var doc = item as DocumentModel;
                    doc.IsChecked = cb.IsChecked.GetValueOrDefault();
                }
            }
        } 
    }
}
