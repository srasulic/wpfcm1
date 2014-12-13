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
using wpfcm1.FolderTypes;
using wpfcm1.Model;
using wpfcm1.PDF;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSignViewModel : Screen
    {
        private readonly CertificateModel _certificate;
        private readonly FolderViewModel _folder;
        private readonly Progress<string> _reporter;
        private CancellationTokenSource _cancellation;

        public DialogSignViewModel(CertificateModel certificate, FolderViewModel folder)
        {
            DisplayName = "";
            _certificate = certificate;
            _folder = folder;
            _reporter = new Progress<string>();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            Reports = new BindableCollection<string>();
        }

        public BindableCollection<string> Reports { get; set; }
        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set { 
                _inProgress = value; 
                NotifyOfPropertyChange(()=>InProgress);
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
                // shallow documents copy, always updated, even if cancel was pressed
                var documents = GetDocumentsForSigning(_folder);
                var sourceDir = _folder.FolderPath;
                await SignAsync(documents, sourceDir, _reporter, _cancellation.Token).WithCancellation(_cancellation.Token);
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
                InProgress = false;
            }
        }

        private async Task SignAsync(IList<DocumentModel> documents, string sourceDir, IProgress<string> reporter = null, CancellationToken token = default(CancellationToken))
        {
            var tsServer = User.Default.TimestampServer;
            if (string.IsNullOrEmpty(tsServer)) throw new ApplicationException("Timestamp server korisnika nije unet!");
            var pib = User.Default.PIB;
            if (string.IsNullOrEmpty(pib)) throw new ApplicationException("PIB korisnika nije unet!");

            var crlList = await CertificateHelpers.GetCrlClentsOfflineAsync(_certificate.ChainElements);
            var ocspClient = new OcspClientBouncyCastle();
            var tsaClient = new TSAClientBouncyCastle(tsServer, "", "", 0, DigestAlgorithms.SHA1);

            var destinationDir = SignedTransferRules.Map[sourceDir];
            var signatureLocation = SignatureRules.Map[sourceDir];

            token.ThrowIfCancellationRequested();

            foreach (var document in documents)
            {
                var sourceFilePath = document.DocumentPath;
                var sourceFileName = Path.GetFileName(sourceFilePath);
                var destinationFileName = CreateSignedPdfFileName(document, User.Default.PIB);
                var destinationFilePath = Path.Combine(destinationDir, destinationFileName);

                if (reporter != null) reporter.Report(string.Format("Signing: {0}", sourceFileName));

                try
                {
                    token.ThrowIfCancellationRequested();
                    await PdfSign.SignPdfAsync(
                        sourceFilePath, destinationFilePath,
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

                if (sourceDir == FolderManager.InvoicesInboundInboxFolder || sourceDir == FolderManager.IosInboundInboxFolder)
                {
                    var destinationAckFilePath = Path.Combine(destinationDir, sourceFileName + ".ack");
                    File.Create(destinationAckFilePath).Dispose();
                }

                if (sourceDir == FolderManager.InvoicesOutboundErpIfaceFolder || sourceDir == FolderManager.IosOutboundErpIfaceFolder)
                {
                    var processedDir = ProcessedTransferRules.Map[sourceDir];
                    var processedFilePath = Path.Combine(processedDir, sourceFileName);
                    File.Copy(sourceFilePath, processedFilePath);
                }

                File.Delete(sourceFilePath);

                if (reporter != null) reporter.Report(string.Format("Signed:  {0}", destinationFileName));
            }
            if (reporter != null) reporter.Report("OK");
        }

        private string CreateSignedPdfFileName(DocumentModel document, string userPib="")
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

        private IList<DocumentModel> GetDocumentsForSigning(FolderViewModel folder)
        {
            if (folder is GeneratedFolderViewModel)
            {
                return (folder as GeneratedFolderViewModel).GetDocumentsForSigning();
            }
            if (folder is InboxFolderViewModel)
            {
                return (folder as InboxFolderViewModel).GetDocumentsForSigning(); 
            }
            throw new ArgumentException("folder)");
        }
    }
}
