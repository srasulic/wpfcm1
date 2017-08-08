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
using iTextSharp.text.pdf.security;

namespace wpfcm1.FolderTypes
{
    public class InboxFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageValidate>, IHandle<MessageAck>, IHandle<MessageXls>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;

        public InboxFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new InboxDocumentModel(new FileInfo(f))));
            
            InitWatcher(FolderPath);

            DocumentsCV = CollectionViewSource.GetDefaultView(Documents) as ListCollectionView;
            DocumentsCV.Filter = new Predicate<object>(FilterDocument);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as InboxDocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.isValidated = state.isValidated;
                old.Processed = state.Processed;
                old.IsAcknowledged = state.IsAcknowledged;
                old.IsSignedAgain = state.IsSignedAgain;
                old.HasSecondSignature = state.HasSecondSignature;
                old.isApprovedForProcessing = state.isApprovedForProcessing;
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

                old.namePib1Name = state.namePib1Name;
                old.namePib2Name = state.namePib2Name;

                old.WaitForServerProcessing = state.WaitForServerProcessing;

            }
            foreach (var document in Documents)
            {
                document.sigAdditionalInfo = "refresh";
            }
        }

        protected override void AddFile(string filePath)
        {
            if (Regex.IsMatch(filePath, @".+syncstamp$", RegexOptions.IgnoreCase))
            {
                InternalMessengerGetStates();
                // ovo nam nije baš tačno, pravi okidač je synchstamp u OUTBOX-u ...
                SetWaitForServerProcessing();
            }
            // necemo u listu dodavati one koji nisu validno nazvan pdf
            else if (!Regex.IsMatch(filePath, @".+_s.pdf$", RegexOptions.IgnoreCase))
            {

            }
            else
            {
                Documents.Add(new InboxDocumentModel(new FileInfo(filePath)));
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
                //PsKillPdfHandlers(); // workaround - pskill ubija sve procese koji rade nad PDF-ovima u eDokument
                var certificateOk = _certificate != null && _certificate.IsQualified;
                if (!certificateOk) return;
                var validDocuments = GetDocumentsForSigning();
                if (!validDocuments.Any()) return;

                //TODO: ovo mora drugacije
                //_events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
                var result = _windowManager.ShowDialog(new DialogSignViewModel(_certificate, this));
            }
        }

        public async void Handle(MessageValidate message)
        {
            if (!IsActive) return;
            await ValidateDocSignaturesAsync();
            // kada završimo validaciju okinućemo i slanje ack fajlova
            var documents = Documents.Where(d => d.isValidated  && !d.IsAcknowledged).Cast<DocumentModel>();
            var destinationDir = SigningTransferRules.LocalMap[FolderPath];
            foreach (var document in documents)
            {
                var fileName = Path.GetFileName(document.DocumentPath);
                var destinationFilePath = Path.Combine(destinationDir, fileName + ".ack");
                File.Create(destinationFilePath).Dispose();
                document.IsAcknowledged = true;
            }
        }

        public void Handle(MessageAck message)
        {
            if (!IsActive) return;
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).ToList();
            //// ako nije bilo cekiranih dokumenata
            //// dodajemo ih iz liste selektovanih
            //// NAPOMENA: izbaceno jer moramo proveriti da li ce ih zbuniti dvojako ponasanje programa!
            //if (!checkedDocuments.Any())
            //{
            //    var v = GetView() as UserControl;
            //    var dg = v.FindName("Documents") as DataGrid;
            //    validDocuments = dg.SelectedItems.Cast<InboxDocumentModel>().ToList().Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).ToList();
            //}
            var destinationDir = SigningTransferRules.LocalMap[FolderPath];
            foreach (var document in validDocuments)
            {
                var fileName = Path.GetFileName(document.DocumentPath);
                var destinationFilePath = Path.Combine(destinationDir, fileName + ".ack");
                File.Create(destinationFilePath).Dispose();
                document.IsAcknowledged = true;
            }
        }

        //public void Handle(MessageToDo message)
        //{
        //    if (!IsActive) return;
        //    var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
        //    var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).ToList();
        //    var destinationDir = SigningTransferRules.LocalMap[FolderPath];
        //    foreach (var document in validDocuments)
        //    {
        //        var fileName = Path.GetFileName(document.DocumentPath);
        //        var destinationFilePath = Path.Combine(destinationDir, fileName + ".ack");
        //        File.Create(destinationFilePath).Dispose();
        //        document.IsAcknowledged = true;
        //    }
        //}

        public IList<DocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
            //var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).Cast<DocumentModel>().ToList();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && d.IsAcknowledged && !d.HasSecondSignature && !d.IsSignedAgain).Cast<DocumentModel>().ToList();
            return validDocuments;
        }

        public void SetApproved(bool approved)
        {
            if (!IsActive) return;
            
            var documents = GetDocumentsForSigning();
            foreach (var document in documents)
            {
                document.isApprovedForProcessing = approved;
                document.isRejected = !approved;
                InternalMessageModel message = new InternalMessageModel(document);
                message.Processed = null; // ne zelimo da saljemo info na temu procesirano jer je poruka upucena drugom folderu
                SerializeMessage(message);
            }
        }

        public override void Dispose()
        {
            Serialize();
        }

        private void Serialize()
        {
            var filePath = Path.Combine(FolderPath, "state.xml");
            var file = File.Create(filePath);
            List<InboxDocumentModel> items = Documents.Cast<InboxDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<InboxDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<InboxDocumentModel> Deserialize()
        {
            var oldList = new List<InboxDocumentModel>();
            var xs = new XmlSerializer(typeof(List<InboxDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            try
            {
                using (Stream s = File.OpenRead(file))
                    oldList = (List<InboxDocumentModel>) xs.Deserialize(s);
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

            var view = ec.View as InboxFolderView;
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
