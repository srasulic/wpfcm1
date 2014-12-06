using Caliburn.Micro;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.Model;

namespace wpfcm1.FolderTypes
{
    public class InboxFolderViewModel : FolderViewModel, IHandle<CertificateModel>, IHandle<MessageSign>
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
                .Where(f => Extensions.Contains(Path.GetExtension(f)))
                .Select(f => new InboxDocumentModel(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new InboxDocumentModel(new FileInfo(filePath)));
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

                var result = _windowManager.ShowDialog(new DialogSignViewModel());
            }
        }

        private IEnumerable<InboxDocumentModel> GetDocumentsForSigning()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked).Cast<InboxDocumentModel>();
            var validDocuments = checkedDocuments.Where(d => d.IsValid.GetValueOrDefault()).ToList(); // izbaco ACKovane fajlove
            return validDocuments;
        }
    }
}
