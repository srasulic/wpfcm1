using System;
using System.Collections.Generic;
using System.IO;
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

                if (result)
                {
                    var dstFilePath = Path.Combine(SyncTransferRules.LocalMap[Path.GetDirectoryName(srcFilePath)], Path.GetFileName(srcFilePath));
                    Log.Info(string.Format("Moving to {0}", dstFilePath));
                    File.Move(srcFilePath, dstFilePath);
                }
                else
                {
                    Log.Error($"ERROR Uploading {srcFilePath}");
                }
            }
        }

        public async Task Sync(
            OlympusService client, Token authToken, string tipDok,
            IEnumerable<string> documents,
            string sourceDir, bool deleteLocal,
            IProgress<string> reporter = null,
            CancellationToken token = default)
        {
            //var docs = await client.GetDocumentsOutbound(authToken);
            //List<string> names = docs.collection.Select(s => s.id_dokument).ToList();

        }
    }
}
