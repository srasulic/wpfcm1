using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Extensions;
using wpfcm1.FolderTypes;
using wpfcm1.FTP;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSyncViewModel : Screen
    {
        private readonly Dictionary<string, FolderViewModel> _folders;
        private Progress<string> _reporter;
        private CancellationTokenSource _cancellation;

        public DialogSyncViewModel(Dictionary<string, FolderViewModel> folders)
        {
            DisplayName = "";
            _folders = folders;
            _reporter = new Progress<string>();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            Reports = new BindableCollection<string>();
        }

        public BindableCollection<string> Reports { get; set; }

        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                NotifyOfPropertyChange(() => InProgress);
                NotifyOfPropertyChange(() => CanOnClose);
                NotifyOfPropertyChange(() => CanOnStart);
            }
        }

        void _reporter_ProgressChanged(object sender, string e)
        {
            Reports.Add(e);
        }

        public void OnClose()
        {
            TryClose(true);
        }

        public bool CanOnClose { get { return !InProgress; } }

        public void OnCancel()
        {
            if (_cancellation != null) _cancellation.Cancel();
        }

        public bool CanOnStart { get { return !InProgress; } }

        public async void OnStart()
        {
            try
            {
                _cancellation = new CancellationTokenSource();
                InProgress = true;
                await SyncAllAsync(_reporter, _cancellation.Token).WithCancellation(_cancellation.Token);
            }
            catch (OperationCanceledException ex)
            {
                (_reporter as IProgress<string>).Report("Operation cancelled");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (WebException ex)
            {
                (_reporter as IProgress<string>).Report("Network error");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (UriFormatException ex)
            {
                (_reporter as IProgress<string>).Report("Cryptographic error.");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (Exception ex)
            {
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            finally
            {
                InProgress = false;
            }
        }

        public async Task SyncAllAsync(IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            var ftpServer = User.Default.FtpServer;
            if (string.IsNullOrEmpty(ftpServer)) throw new ApplicationException("FTP server nije unet!");
            var ftpUsername = User.Default.FtpUserName;
            if (string.IsNullOrEmpty(ftpUsername)) throw new ApplicationException("Username korisnika nije unet!");
            var ftpPassword = User.Default.FtpPassword;
            if (string.IsNullOrEmpty(ftpPassword)) throw new ApplicationException("Password korisnika nije unet!");

            var ftpClient = new FtpClient(ftpServer, ftpUsername, ftpPassword);

            foreach (var folder in _folders)
            {
                var sourceDir = folder.Key;
                var destinationDir = FtpTransferRules.FtpMap[sourceDir];
                var documents = new List<DocumentModel>(folder.Value.Documents); //shallow copy, cannot iterate collection that is going to be modified
                var ftpAction = FtpTransferRules.Action[sourceDir];
                switch (ftpAction)
                {
                    case FtpTransferRules.TransferAction.Upload:
                        if (reporter != null) reporter.Report(string.Format("Upload:\t{0} -> {1}", sourceDir, destinationDir));
                        await Upload(ftpClient, documents, sourceDir, destinationDir, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                    case FtpTransferRules.TransferAction.Sync:
                        if (reporter != null) reporter.Report(string.Format("Sync:\t{0} <-> {1}", sourceDir, destinationDir));
                        await Sync(ftpClient, documents, sourceDir, destinationDir, true, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                    case FtpTransferRules.TransferAction.Download:
                        if (reporter != null) reporter.Report(string.Format("Sync:\t{0} <-> {1}", sourceDir, destinationDir));
                        await Sync(ftpClient, documents, sourceDir, destinationDir, false, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                }
            }
        }

        private async Task Upload(FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir, 
                                  IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            foreach (var document in documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var tempFileName = sourceFileName + ".tmp";

                token.ThrowIfCancellationRequested();
                if (reporter != null) reporter.Report(string.Format("Uploading: {0}", sourceFileName));

                await ftpClient.UploadFileAsync(sourceFilePath, destinationDir, tempFileName);

                token.ThrowIfCancellationRequested();

                var destinationFtpUri = string.Format("{0}{1}{2}", ftpClient.Uri, destinationDir, tempFileName);
                ftpClient.RenameFile(destinationFtpUri, sourceFileName);

                var destinationFilePath = Path.Combine(FtpTransferRules.LocalMap[sourceDir], Path.GetFileName(sourceFileName));
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        private async Task Sync(FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir, bool deleteLocal,
                                IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            var localFileNames = documents.Select(di => di.DocumentInfo.Name);
            var remoteFileNames = await ftpClient.ListDirectoryAsync(destinationDir);

            var local = new SortedSet<string>(localFileNames);
            var remote = new SortedSet<string>(remoteFileNames);
            var diffLocal = local.Except(remote);
            var diffRemote = remote.Except(local);

            foreach (var fileName in diffRemote)
            {
                var filePath = Path.Combine(sourceDir, fileName);
                if (reporter != null) reporter.Report(string.Format("Download: {0}", fileName));

                token.ThrowIfCancellationRequested();
                await ftpClient.DownloadFileAsync(destinationDir, fileName, filePath);
            }

            if (deleteLocal)
            {
                foreach (var fileName in diffLocal)
                {
                    var filePath = Path.Combine(sourceDir, fileName);
                    if (reporter != null) reporter.Report(string.Format("Delete: {0}", fileName));
                    File.Delete(filePath);
                }
            }
        }
    }
}
