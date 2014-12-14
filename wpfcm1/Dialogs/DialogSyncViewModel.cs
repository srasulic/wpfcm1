using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Extensions;
using wpfcm1.FolderTypes;
using wpfcm1.FTP;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Processing;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSyncViewModel : Screen
    {
        private readonly Dictionary<string, FolderViewModel> _folders;
        private readonly Progress<string> _reporter;
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
                var syncMgr = new SyncManager();
                switch (ftpAction)
                {
                    case FtpTransferRules.TransferAction.Upload:
                        if (reporter != null) reporter.Report(string.Format("Upload:\t{0} -> {1}", sourceDir, destinationDir));
                        await syncMgr.Upload(ftpClient, documents, sourceDir, destinationDir, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                    case FtpTransferRules.TransferAction.Sync:
                        if (reporter != null) reporter.Report(string.Format("Sync:\t{0} <-> {1}", sourceDir, destinationDir));
                        await syncMgr.Sync(ftpClient, documents, sourceDir, destinationDir, true, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                    case FtpTransferRules.TransferAction.Download:
                        if (reporter != null) reporter.Report(string.Format("Sync:\t{0} <-> {1}", sourceDir, destinationDir));
                        await syncMgr.Sync(ftpClient, documents, sourceDir, destinationDir, false, reporter, token);
                        if (reporter != null) reporter.Report("OK");
                        break;
                }
            }
        }
    }
}
