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

namespace wpfcm1.FolderTypes
{
    public class InboxFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageValidate>, IHandle<MessageAck>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;

        public InboxFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
            var state = Deserialize();
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new InboxDocumentModel(new FileInfo(f))));
            
            InitWatcher(FolderPath);

            if (Documents.Count == 0) return;
            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.FirstOrDefault(d => d.DocumentPath == state.DocumentPath);
                if (found == null) continue;
                var old = found as InboxDocumentModel;
                old.IsChecked = state.IsChecked;
                old.IsValid = state.IsValid;
                old.Processed = state.Processed;
                old.IsAcknowledged = state.IsAcknowledged;
            }
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new InboxDocumentModel(new FileInfo(filePath)));
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

        public void Handle(CertificateModel message)
        {
            _certificate = message;
        }

        public void Handle(MessageSign message)
        {
            if (IsActive)
            {
                var certificateOk = _certificate != null && _certificate.IsQualified;
                if (!certificateOk) return;
                var validDocuments = GetDocumentsForSigning();
                if (!validDocuments.Any()) return;

                var result = _windowManager.ShowDialog(new DialogSignViewModel(_certificate, this));
            }
        }

        public async void Handle(MessageValidate message)
        {
            var documents = Documents.Where(d => !d.Processed).Cast<InboxDocumentModel>();
            foreach (var document in documents)
            {
                var isValid = await PdfHelpers.ValidatePdfCertificatesAsync(document.DocumentPath);
                document.IsValid = isValid;
                document.Processed = true;
            }
        }

        public void Handle(MessageAck message)
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).ToList();
            var destinationDir = SignedTransferRules.Map[FolderPath];
            foreach (var document in validDocuments)
            {
                var fileName = Path.GetFileName(document.DocumentPath);
                var destinationFilePath = Path.Combine(destinationDir, fileName + ".ack");
                File.Create(destinationFilePath).Dispose();
                document.IsAcknowledged = true;
            }
        }

        public IList<DocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault() && !d.IsAcknowledged).Cast<DocumentModel>().ToList();
            return validDocuments;
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
    }
}
