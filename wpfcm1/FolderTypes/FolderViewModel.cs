using Caliburn.Micro;
using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.Preview;

namespace wpfcm1.FolderTypes
{
    public class FolderViewModel : Screen, IDisposable
    {
        protected string[] Extensions = { ".pdf", ".ack" };
        protected FileSystemWatcher _watcher;
        private readonly Dispatcher _dispatcher;
        protected readonly IEventAggregator _events;

        public FolderViewModel(string path, string name, IEventAggregator events)
        {
            FolderPath = path;
            DisplayName = name;
            _dispatcher = Dispatcher.CurrentDispatcher;
            _events = events;
            _events.Subscribe(this);

            InitDocuments();
        }

        public string FolderPath { get; private set; }
        public int Count { get { return Documents.Count; } }
        public virtual BindableCollection<DocumentModel> Documents { get; set; }
        private bool _isChanged;
        public bool IsChanged { get { return _isChanged; } set { _isChanged = value; NotifyOfPropertyChange(() => IsChanged); } }

        protected virtual void InitDocuments()
        {
            Documents = new BindableCollection<DocumentModel>(
                 Directory.EnumerateFiles(FolderPath)
                 .Where(f => Extensions.Contains(Path.GetExtension(f)))
                 .Select(f => new DocumentModel(new FileInfo(f))));
            InitWatcher(FolderPath);
        }

        protected override void OnActivate()
        {
            _events.PublishOnUIThread(new MessageViewModelActivated(GetType().Name));
        }

        protected override void OnDeactivate(bool close)
        {
            _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
        }

        protected override void OnViewAttached(object view, object context)
        {
            Documents.Refresh(); //TODO: hack: nestajao error notification template kada se promeni tab
        }

        protected virtual void AddFile(string filePath)
        {
            Documents.Add(new DocumentModel(new FileInfo(filePath)));
        }

        public virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var message = PreviewViewModel.Empty;
            if (e.AddedItems.Count == 1)
            {
                var document = e.AddedItems[0] as DocumentModel;
                var path = document.DocumentPath;
                if (path.EndsWith(".pdf"))
                    message = path;
            }
            _events.PublishOnUIThread(new MessageShowPdf(message)); 
        }

        #region FileSystemwatcher
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
        #endregion

        public virtual void Dispose()
        {

        }
    }
}
