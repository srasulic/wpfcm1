using Caliburn.Micro;
using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using wpfcm1.Certificates;
using wpfcm1.DataAccess;
using wpfcm1.Extensions;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSignGeneratedViewModel : Screen
    {
        private readonly CertificateModel _certificate;
        private readonly IList<GeneratedDocumentModel> _documents;
        private Progress<string> _reporter;
        private CancellationTokenSource _cancellation;
 
        public DialogSignGeneratedViewModel(CertificateModel certificate, IList<GeneratedDocumentModel> documents)
        {
            DisplayName = "";
            _certificate = certificate;
            _documents = documents;
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
                await SignAsync(_reporter, token).WithCancellation(token);
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
            catch (CryptographicException ex)
            {
                (_reporter as IProgress<string>).Report("Cryptographic error.");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (COMException ex)
            {
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

        private async Task SignAsync(IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
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

            token.ThrowIfCancellationRequested();

            foreach (var document in _documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = string.Format("{0}_{1}_{2}_{3}_s.pdf", User.Default.PIB, document.Pib, document.InvoiceNo, DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
                var detinationFilePath = Path.Combine(destinationDir, destinationFileName);

                if (reporter != null) reporter.Report(string.Format("Signing: {0}", sourceFileName));

                try
                {
                    token.ThrowIfCancellationRequested();
                    await PdfSign.SignPdfAsync(
                        sourceFilePath, detinationFilePath,
                        _certificate.Certificate,
                        _certificate.ChainElements,
                        crlList,
                        ocspClient,
                        tsaClient,
                        signatureLocation
                        );
                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    if (File.Exists(detinationFilePath))
                        File.Delete(detinationFilePath); //itext maybe created dest file
                    throw;
                }
                catch (CryptographicException)
                {
                    if (File.Exists(detinationFilePath))
                        File.Delete(detinationFilePath); //itext already created dest file
                    throw;
                }

                var processedFilePath = Path.Combine(processedDir, sourceFileName);
                File.Move(sourceFilePath, processedFilePath);

                if (reporter != null) reporter.Report(string.Format("Signed:  {0}", destinationFileName));
            }
        }
    }
}
