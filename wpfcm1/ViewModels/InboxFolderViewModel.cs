using Caliburn.Micro;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using wpfcm1.Events;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class InboxFolderViewModel : FolderViewModel
    {
        public InboxFolderViewModel(string path, string name, IEventAggregator events) : base(path, name, events)
        {
        }

        protected override void InitDocuments()
        {
            Documents = new BindableCollection<DocumentItem>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f)))
                 .Select(f => new InboxDocumentItem(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        protected override void AddFile(string filePath)
        {
            Documents.Add(new InboxDocumentItem(new FileInfo(filePath)));
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
    }
}
