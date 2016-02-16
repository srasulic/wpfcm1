using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.FTP;
using wpfcm1.Model;
using wpfcm1.PDF;

namespace wpfcm1.Processing
{
    public class SyncManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task Upload(
            FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir,
            IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            foreach (var document in documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var tempFileName = sourceFileName + ".tmp";

                token.ThrowIfCancellationRequested();
                if (reporter != null) reporter.Report(string.Format("Uploading: {0}", sourceFileName));
                Log.Info(string.Format("Uploading {0}", sourceFilePath));

                await ftpClient.UploadFileAsync(sourceFilePath, destinationDir, tempFileName);

                token.ThrowIfCancellationRequested();

                var destinationFtpUri = string.Format("{0}{1}{2}", ftpClient.Uri, destinationDir, tempFileName);
                ftpClient.RenameFile(destinationFtpUri, sourceFileName);

                var destinationFilePath = Path.Combine(FtpTransferRules.LocalMap[sourceDir], Path.GetFileName(sourceFileName));
                Log.Info(string.Format("Moving to {0}", destinationFilePath));
                File.Move(sourceFilePath, destinationFilePath);
            }
        }

        public async Task Sync(
            FtpClient ftpClient, IEnumerable<DocumentModel> documents, string sourceDir, string destinationDir, bool deleteLocal,
            IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            Log.Info(string.Format("Listing directory {0}", destinationDir));
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
                Log.Info(string.Format("Downloading {0}", filePath));

                token.ThrowIfCancellationRequested();
                await ftpClient.DownloadFileAsync(destinationDir, fileName, filePath);
            }

            if (deleteLocal)
            {
                foreach (var fileName in diffLocal)
                {
                    var filePath = Path.Combine(sourceDir, fileName);
                    if (reporter != null) reporter.Report(string.Format("Delete: {0}", fileName));
                    Log.Info(string.Format("Deleting {0}", filePath));
                    File.Delete(filePath);
                }
            }
        }
    }
}
