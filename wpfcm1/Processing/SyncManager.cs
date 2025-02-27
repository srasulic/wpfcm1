using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.OlympusApi;
using wpfcm1.PDF;

namespace wpfcm1.Processing
{
    public class SyncManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task Upload(
            OlympusService client, Token authToken, TipDokPristup tdp, string tenant,
            string folder,
            IProgress<string> reporter = null,
            CancellationToken token = default)
        {
            var documents = Directory.EnumerateFiles(folder, "*.pdf");
            foreach (var srcFilePath in documents)
            {
                token.ThrowIfCancellationRequested();

                reporter?.Report($"Uploading: {Path.GetFileName(srcFilePath)}");
                Log.Info($"Uploading {srcFilePath}");

                var result = await client.PostDocumentsUpload(tdp, authToken, tenant, srcFilePath);

                if (result != null && result.code == 0)
                {
                    var dstFilePath = Path.Combine(SyncTransferRules.LocalMap[Path.GetDirectoryName(srcFilePath)], Path.GetFileName(srcFilePath));
                    Log.Info(string.Format("Moving to {0}", dstFilePath));
                    File.Move(srcFilePath, dstFilePath);
                }
                else
                {
                    Log.Error($"ERROR Uploading {srcFilePath}");
                    Log.Error(result?.userMessage);
                    reporter?.Report($"ERROR Uploading: {srcFilePath}");
                }
            }
        }

        public async Task Sync(
            OlympusService client, Token authToken, TipDokPristup tdp, string tenant, string sinceDate,
            string folder, bool deleteLocal,
            IProgress<string> reporter = null,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var since = DateTime.Parse(sinceDate).ToString("yyyy-MM-dd");
            var docs = await client.GetDocuments(tdp, authToken, tenant, since);
            if ((docs == null) || (docs != null && docs.result.code != 0))
            {
                Log.Error($"ERROR GetDocuments: {folder}");
                Log.Error($"ERROR GetDocuments: {docs?.result.userMessage}");
                return;
            }

            Dictionary<string, string> remoteFilesMap = new Dictionary<string, string>();
            foreach (var d in docs.collection)
            {
                remoteFilesMap.Add(d.teh_naziv, d.id_fajl);
            }

            List<string> remoteFileNames = remoteFilesMap.Keys.ToList();
            List<string> localFileNames = Directory.EnumerateFiles(folder, "*.pdf").Select(f => Path.GetFileName(f)).ToList();

            var local = new SortedSet<string>(localFileNames);
            var remote = new SortedSet<string>(remoteFileNames);
            var diffLocal = local.Except(remote);
            var diffRemote = remote.Except(local);

            foreach (var fileName in diffRemote)
            {
                token.ThrowIfCancellationRequested();

                var filePath = Path.Combine(folder, fileName);

                reporter?.Report($"Download: {fileName}");
                Log.Info($"Downloading {filePath}");

                var downloadResult = await client.PostFilesDownload(authToken, tenant, remoteFilesMap[fileName]);
                if (downloadResult == null || (downloadResult != null && downloadResult.result.result.code != 0))
                {
                    Log.Error($"ERROR PostFilesDownload: {fileName}");
                    Log.Error($"ERROR PostFilesDownload: {downloadResult.result.result.userMessage}");
                    reporter?.Report($"ERROR Downloading: {fileName}");
                }
                else
                {
                    File.WriteAllBytes(filePath, downloadResult.bytes);
                }
            }

            if (deleteLocal)
            {
                foreach (var fileName in diffLocal)
                {
                    var filePath = Path.Combine(folder, fileName);

                    reporter?.Report($"Delete: {fileName}");
                    Log.Info($"Deleting {filePath}");

                    File.Delete(filePath);
                }
            }
        }
    }
}
