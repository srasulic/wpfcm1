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
using wpfcm1.FolderTypes;
using System.Text.RegularExpressions;

namespace wpfcm1.Processing
{
    public class SigningManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SigningManager(CertificateModel certificate, IList<DocumentModel> documents, string sourceDir, FolderViewModel folder)
        {
            Certificate = certificate;
            Documents = documents;
            SourceDir = sourceDir;
            Folder = folder;
        }

        public IList<DocumentModel> Documents { get; private set; }
        public string SourceDir { get; set; }
        public CertificateModel Certificate { get; private set; }
        public FolderViewModel Folder { get; set; }

        public async Task SignAsync(string reason = "", IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            // preuzmemo vrednosti - stringove iz podešavanja aplikacije:
            string tsServer = User.Default.TimestampServer;
            if (string.IsNullOrEmpty(tsServer)) throw new ApplicationException("Timestamp server korisnika nije unet!");
            var tsUser = User.Default.TimestampUserName;
            var tsPass = User.Default.TimestampPassword;
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib)) throw new ApplicationException("PIB korisnika nije unet!");

            var crlList = await CertificateHelpers.GetCrlClentsOfflineAsync(Certificate.ChainElements);
            var ocspClient = new OcspClientBouncyCastle();
            // posebna podrska za definisanje željenog SHA algoritma
            // default će biti SHA1
            TSAClientBouncyCastle tsaClient = null;
            if (tsServer.IndexOf( @":SHA1") > 0){
                tsServer = tsServer.Replace(@":SHA1", "");
                tsaClient = new TSAClientBouncyCastle(tsServer, tsUser, tsPass, 0, DigestAlgorithms.SHA1);
            }
            else if (tsServer.IndexOf(@":SHA256") > 0)
            {
                tsServer = tsServer.Replace(@":SHA256", "");
                tsaClient = new TSAClientBouncyCastle(tsServer, tsUser, tsPass, 0, DigestAlgorithms.SHA256);
            }
            else if (tsServer.IndexOf(@":SHA384") > 0)
            {
                tsServer = tsServer.Replace(@":SHA384", "");
                tsaClient = new TSAClientBouncyCastle(tsServer, tsUser, tsPass, 0, DigestAlgorithms.SHA384);
            }
            else if (tsServer.IndexOf(@":SHA512") > 0)
            {
                tsServer = tsServer.Replace(@":SHA512", "");
                tsaClient = new TSAClientBouncyCastle(tsServer, tsUser, tsPass, 0, DigestAlgorithms.SHA512);
            }
            else if (tsServer != @"N" )
            {
                tsaClient = new TSAClientBouncyCastle(tsServer, tsUser, tsPass, 0, DigestAlgorithms.SHA1);
            };

            var destinationDir = SigningTransferRules.LocalMap[SourceDir];
            var signatureLocation = SignatureRules.Map[SourceDir];

            token.ThrowIfCancellationRequested();

            foreach (var document in Documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = CreateSignedPdfFileName(document);
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
                catch (Exception)
                {
                    if (File.Exists(destinationFilePath))
                        File.Delete(destinationFilePath); // dest file already created
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
                        document.IsAcknowledged = true;
                        break;
                    case SigningTransferRules.FinalAction.SecondSignatureMark:
                        //document.HasSecondSignature = true;
                        //document.IsAcknowledged = false;
                        document.IsSignedAgain = true;
                        document.Processed = true;
                        document.archiveReady = true;

                        var message = new InternalMessageModel(document);
                        Folder.SerializeMessage(message);
                        
                        break;
                    case SigningTransferRules.FinalAction.Store:
                        var processedDir = SigningTransferRules.ProcessedMap[SourceDir];
                        // ovako cemo obezbediti da se ne desi da je u Obradjeno vec postoji takav fajl 
                        var processedFileName = string.Format("{0}_{1}", DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"), sourceFileName);
                        var processedFilePath = Path.Combine(processedDir, processedFileName);
                        Log.Info(string.Format("Copying: {0}", processedFilePath));
                        File.Copy(sourceFilePath, processedFilePath);
                        Log.Info(string.Format("Deleting: {0}", sourceFilePath));
                        File.Delete(sourceFilePath);
                        //document.IsSigned = true;
                        break;
                }

                if (reporter != null) reporter.Report(string.Format("Potpisan kao:  {0}", destinationFileName));
            }
            if (reporter != null) reporter.Report("OK");
        }

        private string CreateSignedPdfFileName(DocumentModel document)
        {
            if (document is GeneratedDocumentModel)
            {
                var gdoc = document as GeneratedDocumentModel;

                if (String.IsNullOrEmpty(gdoc.PibIssuer))
                {
                    gdoc.PibIssuer = User.Default.PIB;
                }

                string destinationFileName;

                // za izvršitelje dokumenti Others neće biti rename-ovani
                if (User.Default.Variation == "RS-IZVRSITELJ" && Regex.IsMatch(document.DocumentInfo.FullName, @"ostali", RegexOptions.IgnoreCase) )
                {
                    destinationFileName = document.DocumentInfo.Name;

                }
                else
                {
                    destinationFileName = string.Format("{0}_{1}_{2}_{3}_s.pdf", gdoc.PibIssuer, gdoc.PibReciever, gdoc.InvoiceNo, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
                }
                return destinationFileName;
            }
            if (document is InboxDocumentModel)
            {
                var destinationFileName = string.Format("{0}_s{1}", Path.GetFileNameWithoutExtension(document.DocumentInfo.Name), Path.GetExtension(document.DocumentInfo.Name));
                return destinationFileName;
            }
            throw new ArgumentException("CreateSignedPdfFileName - error.");
        }
    }
}
