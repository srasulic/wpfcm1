using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.DataAccess;
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

        void _reporter_ProgressChanged(object sender, string e)
        {
            Reports.Add(e);
        }

        public async void OnStart()
        {
            try
            {
                _cancellation = new CancellationTokenSource();
                var token = _cancellation.Token;
                await SyncAllAsync(_reporter, token).WithCancellation(_cancellation.Token);
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

            }
        }

        public void OnClose()
        {
            TryClose(true);
        }

        public void OnCancel()
        {
            if (_cancellation != null) _cancellation.Cancel();
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
                var destinationDir = FtpTransferRules.Map[sourceDir];
                var ftpAction = FtpTransferRules.Action[sourceDir];
                var documents = new List<DocumentModel>(folder.Value.Documents); //shallow copy, cannot iterate collection that is going to be modified
                switch (ftpAction)
                {
                    case FtpTransferRules.TransferAction.Upload:
                        reporter.Report(string.Format("Upload:\t{0} -> {1}", sourceDir, destinationDir));
                        await Upload(ftpClient, documents, sourceDir, destinationDir, reporter, token);
                        reporter.Report("OK");
                        break;
                    case FtpTransferRules.TransferAction.Sync:
                        reporter.Report(string.Format("Sync:\t{0} <-> {1}", sourceDir, destinationDir));
                        var deleteLocal = folder.Key == FolderManager.InboundInboxFolder || folder.Key == FolderManager.OutboundPendFolder;
                        await Sync(ftpClient, documents, sourceDir, destinationDir, deleteLocal, reporter, token);
                        reporter.Report("OK");
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
                reporter.Report(string.Format("Uploading: {0}", sourceFileName));

                await ftpClient.UploadFileAsync(sourceFilePath, destinationDir, tempFileName);

                var destinationFtpUri = string.Format("{0}{1}{2}", ftpClient.Uri, destinationDir, tempFileName);

                ftpClient.RenameFile(destinationFtpUri, sourceFileName);

                var destinationFilePath = Path.Combine(FtpSucessTransferRules.Map[sourceDir], Path.GetFileName(sourceFileName));
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
                reporter.Report(string.Format("Download: {0}", fileName));

                await ftpClient.DownloadFileAsync(destinationDir, fileName, filePath);
            }

            if (deleteLocal)
            {
                foreach (var fileName in diffLocal)
                {
                    var filePath = Path.Combine(sourceDir, fileName);
                    reporter.Report(string.Format("Delete: {0}", fileName));
                    File.Delete(filePath);
                }
            }
        }
    }
}
