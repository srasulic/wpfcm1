using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.FolderTypes
{
    public class GeneratedFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>, IHandle<MessageExtractData>
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

                var result = _windowManager.ShowDialog(new DialogSignGeneratedViewModel(_certificate, validDocuments));
            }
        }

        public async void Handle(MessageExtractData message)
        {
            var documents = Documents.Where(d => !d.Processed).Cast<GeneratedDocumentModel>();
            //var documents = Documents.Cast<GeneratedDocumentModel>();
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

        private IList<GeneratedDocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<GeneratedDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault()).ToList();
            return validDocuments;
        }
    }
}
