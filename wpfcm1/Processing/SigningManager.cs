using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Certificates;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.Processing
{
    public class SigningManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SigningManager(CertificateModel certificate, IList<DocumentModel> documents, string sourceDir)
        {
            Certificate = certificate;
            Documents = documents;
            SourceDir = sourceDir;
        }

        public IList<DocumentModel> Documents { get; private set; }
        public string SourceDir { get; set; }
        public CertificateModel Certificate { get; private set; }

        public async Task SignAsync(string reason = "", IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            var tsServer = User.Default.TimestampServer;
            if (string.IsNullOrEmpty(tsServer)) throw new ApplicationException("Timestamp server korisnika nije unet!");
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib)) throw new ApplicationException("PIB korisnika nije unet!");

            var crlList = await CertificateHelpers.GetCrlClentsOfflineAsync(Certificate.ChainElements);
            var ocspClient = new OcspClientBouncyCastle();
            var tsaClient = new TSAClientBouncyCastle(tsServer, "", "", 0, DigestAlgorithms.SHA1);

            var destinationDir = SigningTransferRules.LocalMap[SourceDir];
            var signatureLocation = SignatureRules.Map[SourceDir];

            token.ThrowIfCancellationRequested();

            foreach (var document in Documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = CreateSignedPdfFileName(document, User.Default.PIB);
                var destinationFilePath = Path.Combine(destinationDir, destinationFileName);

                if (reporter != null) reporter.Report(string.Format("Potpisivanje: {0}", sourceFileName));
                Log.Info(string.Format("Signing src file: {0}", sourceFilePath));

                try
                {
                    token.ThrowIfCancellationRequested();
                    await PdfSign.SignPdfAsync(
                        sourceFilePath, destinationFilePath,
                        Certificate.Certificate,
                        Certificate.ChainElements,
                        crlList,
                        ocspClient,
                        tsaClient,
                        signatureLocation,
                        reason
                    );
                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    if (File.Exists(destinationFilePath))
                        File.Delete(destinationFilePath); //itext maybe created dest file
                    throw;
                }
                catch (CryptographicException)
                {
                    if (File.Exists(destinationFilePath))
                        File.Delete(destinationFilePath); //itext already created dest file
                    throw;
                }

                var finalAction = SigningTransferRules.OnFinished[SourceDir];
                switch (finalAction)
                {
                    case SigningTransferRules.FinalAction.Acknowledge:
                        var destinationAckFilePath = Path.Combine(destinationDir, sourceFileName + ".ack");
                        if (reporter != null) reporter.Report(string.Format("Potvrdjen kao: {0}", destinationAckFilePath));
                        Log.Info(string.Format("Acknowledged: {0}", destinationAckFilePath));
                        File.Create(destinationAckFilePath).Dispose();
                        break;
                    case SigningTransferRules.FinalAction.Store:
                        var processedDir = SigningTransferRules.ProcessedMap[SourceDir];
                        var processedFilePath = Path.Combine(processedDir, sourceFileName);
                        Log.Info(string.Format("Copying: {0}", processedFilePath));
                        File.Copy(sourceFilePath, processedFilePath);
                        break;
                }

                Log.Info(string.Format("Deleting: {0}", sourceFilePath));
                File.Delete(sourceFilePath);

                if (reporter != null) reporter.Report(string.Format("Potpisan kao:  {0}", destinationFileName));
            }
            if (reporter != null) reporter.Report("OK");
        }

        private string CreateSignedPdfFileName(DocumentModel document, string userPib = "")
        {
            if (document is GeneratedDocumentModel)
            {
                var gdoc = document as GeneratedDocumentModel;
                var destinationFileName = string.Format("{0}_{1}_{2}_{3}_s.pdf", userPib, gdoc.Pib, gdoc.InvoiceNo, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
                return destinationFileName;
            }
            if (document is InboxDocumentModel)
            {
                var destinationFileName = string.Format("{0}_s{1}", Path.GetFileNameWithoutExtension(document.DocumentInfo.Name), Path.GetExtension(document.DocumentInfo.Name));
                return destinationFileName;
            }
            throw new ArgumentException("document");
        }
    }
}
