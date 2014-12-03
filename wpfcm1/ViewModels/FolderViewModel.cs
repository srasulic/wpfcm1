using Caliburn.Micro;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using wpfcm1.Model;

namespace wpfcm1.ViewModels
{
    public class FolderViewModel : Screen
    {
        protected string[] Extensions = { ".pdf", ".ack" };
        protected FileSystemWatcher _watcher;
        private readonly Dispatcher _dispatcher;

        public FolderViewModel(string path, string name)
        {
            FolderPath = path;
            DisplayName = name;
            _dispatcher = Dispatcher.CurrentDispatcher;

            InitDocuments();
        }

        public string FolderPath { get; private set; }
        public int Count { get { return Documents.Count; } }
        public virtual BindableCollection<DocumentItem> Documents { get; set; }
        
        private bool _isChanged;
        public bool IsChanged
        {
            get { return _isChanged; }
            set { _isChanged = value; NotifyOfPropertyChange(() => IsChanged); }
        }

        protected virtual void InitDocuments()
        {
            Documents = new BindableCollection<DocumentItem>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f)))
                 .Select(f => new DocumentItem(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        protected void InitWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path) { NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName };
            _watcher.Changed += Watcher_Changed; 
            _watcher.Deleted += Watcher_Deleted;
            _watcher.Created += Watcher_Created;
            _watcher.Renamed += Watcher_Renamed;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {

        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            IsChanged = true;
            var action = new System.Action(() => AddFile(e.FullPath));
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }

        protected virtual void AddFile(string filePath)
        {
            Documents.Add(new DocumentItem(new FileInfo(filePath)));
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            IsChanged = true;
            var action = new System.Action(() =>
            {
                var docToRemove = Documents.FirstOrDefault(f => f.DocumentPath == e.FullPath);
                if (docToRemove != null) Documents.Remove(docToRemove);
            });
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {

        }

        protected override void OnViewAttached(object view, object context)
        {
            Documents.Refresh(); // nestaje error notification template kada se promeni tab
        }
    }
}
