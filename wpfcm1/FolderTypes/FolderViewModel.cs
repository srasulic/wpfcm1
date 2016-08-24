using Caliburn.Micro;
using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;
using wpfcm1.Events;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Preview;

namespace wpfcm1.FolderTypes
{
    public class FolderViewModel : Screen, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // protected string[] Extensions = { ".pdf", ".ack" };
        protected string[] Extensions = { ".pdf" };
        protected FileSystemWatcher _watcher;
        private readonly Dispatcher _dispatcher;
        protected readonly IEventAggregator _events;
        private string _expList;

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

        protected void PsKillPdfHandlers()
        {   // Srdjan - da pustimo u pozadini neke alatke da pobiju eventualne procese koji drze sapu na PDF fajlovima
            //          Zbog brzine aplikaciji hadle.exe prosledjujemo deo naziva file handlera (bez ovoga traje 10-ak sekundi)
            //          Podrzavamo (ocekujemo) Foxit Reader ili Adobe Acrobat Reader
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";

                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Fox ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                startInfo.Arguments = @" /C for /f ""tokens=3"" %G IN ('c:\edokument\bin\handle.exe /accepteula eDokument\ -p Acro ^| findstr /i /r /c:"".*pid:.*pdf$""') DO c:\edokument\bin\pskill.exe /accepteula %G";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

            }
            catch (Exception ex)
            {
                Log.Error("Error while running PSTOOLS...", ex);
            }
        }
        
        protected void RejectDocument ()
        {
            var checkedDocuments = Documents.Where(d => d.IsChecked);
            var destinationDir = SigningTransferRules.ProcessedMap[FolderPath];
            foreach (var document in checkedDocuments)
            {
                var sourceFilePath = document.DocumentPath;
                var fileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = string.Format("X_{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), fileName);
                var destinationFilePath = Path.Combine(destinationDir, destinationFileName);
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        protected void XlsExport()
        {
            try
            {
                var documents = Documents.Cast<DocumentModel>();
                _expList = "\"Mark\",\"Pib izdavalac\",\"Pib primalac\",\"Fajl\",\"KB\",\"Br Dok\"\r\n";
                foreach (var document in documents)
                {
                    string[] fileNameParts = document.DocumentPath.Split('\\');
                    string[] parts = fileNameParts.Last().Split('_');
                    _expList = string.Concat(_expList, "\"", document.IsChecked.ToString(), "\",\"", parts[0], "\",\"", parts[1], "\",\"", fileNameParts.Last(), "\",\"", document.LengthKB, "\",\"", parts[2], "\"\r\n");
                }

                string filename = string.Concat(Guid.NewGuid().ToString(), @".csv");
                filename = string.Concat(Path.GetTempPath(), filename);
                try
                {
                    System.Text.Encoding utf16 = System.Text.Encoding.GetEncoding(1254);
                    byte[] output = utf16.GetBytes(_expList);
                    FileStream fs = new FileStream(filename, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(output, 0, output.Length); //write the encoded file
                    bw.Flush();
                    bw.Close();
                    fs.Close();
                }
                catch
                {

                }

                System.Diagnostics.Process.Start(filename);
            }
            catch
            {

            }
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

        public virtual void OnCheck(object e)
        {

        }
    }
}
