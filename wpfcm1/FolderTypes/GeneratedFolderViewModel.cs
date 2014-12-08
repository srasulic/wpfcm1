using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Serialization;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.FolderTypes
{
    public class GeneratedFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageExtractData>, IHandle<MessageReject>
    {
        private readonly IWindowManager _windowManager;
        private CertificateModel _certificate;

        public GeneratedFolderViewModel(string path, string name, IEventAggregator events, IWindowManager winMgr) : base(path, name, events)
        {
            _windowManager = winMgr;
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                Directory.EnumerateFiles(FolderPath)
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new GeneratedDocumentModel(new FileInfo(f))));

            InitWatcher(FolderPath);

            var states = Deserialize();
            foreach (var state in states)
            {
                var found = Documents.First(d => d.DocumentPath == state.DocumentPath) as GeneratedDocumentModel;
                if (found == null) continue;
                found.IsChecked = state.IsChecked;
                found.IsValid = state.IsValid;
                found.InvoiceNo = state.InvoiceNo;
                found.Pib = state.Pib;
                found.Processed = state.Processed;
            }
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new GeneratedDocumentModel(new FileInfo(filePath)));
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new ViewModelActivatedMessage(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
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

        public async void Handle(MessageExtractData message)
        {
            var documents = Documents.Where(d => !d.Processed).Cast<GeneratedDocumentModel>();
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib))
                throw new ApplicationException("PIB korisnika nije unet!");

            foreach (var document in documents)
            {
                var matchResults = await PdfHelpers.ExtractTextAsync(document.DocumentPath);
                string pibMatch = "";
                foreach (var match in matchResults.Item1)
                {
                    if (match.ToString() == pib)
                        continue;
                    pibMatch = match.ToString();
                    break;
                }
                document.Pib = pibMatch;
                document.InvoiceNo = matchResults.Item2.Count > 0 ? matchResults.Item2[0].Value : "";
                document.InvoiceNo = document.InvoiceNo.Replace('/', '-');
                document.Processed = true;
            }
        }

        public void Handle(MessageReject message)
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked);
            var destinationDir = ProcessedTransferRules.Map[FolderPath];
            foreach (var document in checkedDocuments)
            {
                var sourceFilePath = document.DocumentPath;
                var fileName = Path.GetFileName(sourceFilePath);
                var destinationFilePath = Path.Combine(destinationDir, fileName);
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        public IList<DocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<GeneratedDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault()).Cast<DocumentModel>().ToList();
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
            List<GeneratedDocumentModel> items = Documents.Cast<GeneratedDocumentModel>().ToList();
            var xs = new XmlSerializer(typeof(List<GeneratedDocumentModel>));
            using (Stream s = file)
                xs.Serialize(s, items);
        }

        private List<GeneratedDocumentModel> Deserialize()
        {
            var oldList = new List<GeneratedDocumentModel>();
            var xs = new XmlSerializer(typeof(List<GeneratedDocumentModel>));
            var file = Path.Combine(FolderPath, "state.xml");
            if (!File.Exists(file)) return oldList;
            using (Stream s = File.OpenRead(file))
                oldList = (List<GeneratedDocumentModel>) xs.Deserialize(s);
            return oldList;
        }
    }
}
