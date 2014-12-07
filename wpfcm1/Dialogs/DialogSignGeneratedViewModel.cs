using Caliburn.Micro;
using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Certificates;
using wpfcm1.DataAccess;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSignGeneratedViewModel : Screen
    {
        private readonly CertificateModel _certificate;
        private readonly IList<GeneratedDocumentModel> _documents;
        private readonly Progress<string> _reporter = new Progress<string>();
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
 
        public DialogSignGeneratedViewModel(CertificateModel certificate, IList<GeneratedDocumentModel> documents)
        {
            DisplayName = "";
            _certificate = certificate;
            _documents = documents;
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
            await Sign(_reporter, _cancellation.Token);
        }

        public void OnClose()
        {
            TryClose(true);
        }

        public void OnCancel()
        {

        }

        private async Task Sign(IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            var tsServer = User.Default.TimestampServer;
            if (string.IsNullOrEmpty(tsServer)) throw new ApplicationException("Timestamp server korisnika nije unet!");
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib)) throw new ApplicationException("PIB korisnika nije unet!");

            var crlList = await CertificateHelpers.GetCrlClentsOfflineAsync(_certificate.ChainElements);
            var ocspClient = new OcspClientBouncyCastle();
            var tsaClient = new TSAClientBouncyCastle(tsServer, "", "", 0, DigestAlgorithms.SHA1);

            var sourceDir = FolderManager.OutboundErpIfaceFolder;
            var destinationDir = SignedTransferRules.Map[sourceDir];
            var processedDir = ProcessedTransferRules.Map[sourceDir];

            var signatureLocation = SignatureRules.Map[sourceDir];

            foreach (var document in _documents)
            {
                var sourceFilePath = document.DocumentPath;
                var destinationFileName = string.Format("{0}_{1}_{2}_{3}_s.pdf", User.Default.PIB, document.Pib, document.InvoiceNo, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
                var detinationFilePath = Path.Combine(destinationDir, destinationFileName);
                reporter.Report(sourceFilePath);

                //sign...
                await PdfSign.SignPdfAsync(
                    sourceFilePath, detinationFilePath,
                    _certificate.Certificate,
                    _certificate.ChainElements,
                    crlList,
                    ocspClient,
                    tsaClient,
                    signatureLocation
                );

                var sourceFileName = Path.GetFileName(sourceFilePath);
                var processedFilePath = Path.Combine(processedDir, sourceFileName);
                File.Move(sourceFilePath, processedFilePath);
            }
        }
    }
}
