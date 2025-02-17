using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Model;
using wpfcm1.OlympusApi;
using wpfcm1.PDF;

namespace wpfcm1.Processing
{
    public class SyncManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task Upload(
            OlympusService client, Token authToken, string tipDok,
            IEnumerable<string> documents,
            IProgress<string> reporter = null,
            CancellationToken token = default)
        {
            foreach (var srcFilePath in documents)
            {
                token.ThrowIfCancellationRequested();

                reporter?.Report($"Uploading: {Path.GetFileName(srcFilePath)}");
                Log.Info($"Uploading {srcFilePath}");

                var result = await client.PostDocumentsUploadOutbound(authToken, tipDok, srcFilePath);
                if (!result)
                {
                    Log.Info($"ERROR Uploading {srcFilePath}");
                }

                var dstFilePath = Path.Combine(SyncTransferRules.LocalMap[Path.GetDirectoryName(srcFilePath)], Path.GetFileName(srcFilePath));
                Log.Info(string.Format("Moving to {0}", dstFilePath));
                File.Move(srcFilePath, dstFilePath);
            }
        }

        public async Task Sync(
            OlympusService client,
            IEnumerable<DocumentModel> documents,
            string sourceDir, bool deleteLocal,
            IProgress<string> reporter = null,
            CancellationToken token = default)
        {

        }
    }
}
