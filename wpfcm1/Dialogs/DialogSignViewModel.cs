using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using wpfcm1.Extensions;
using wpfcm1.FolderTypes;
using wpfcm1.Model;
using wpfcm1.Processing;

namespace wpfcm1.Dialogs
{
    public class DialogSignViewModel : Screen
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CertificateModel _certificate;
        private readonly FolderViewModel _folder;
        private readonly Progress<string> _reporter;
        private CancellationTokenSource _cancellation;

        public DialogSignViewModel(CertificateModel certificate, FolderViewModel folder)
        {
            DisplayName = "PoliSign - potpisivanje dokumenata";
            _certificate = certificate;
            _folder = folder;
            _reporter = new Progress<string>();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            Reports = new BindableCollection<string>();
        }

        public BindableCollection<string> Reports { get; set; }

        private string _reason;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; NotifyOfPropertyChange(() => InProgress); }
        }

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
                var signingMgr = new SigningManager(_certificate, documents, sourceDir);
                Log.Info("Signing started...");
                await signingMgr.SignAsync(Reason, _reporter, _cancellation.Token).WithCancellation(_cancellation.Token);
                Log.Info("Signing finished...");
            }
            catch (OperationCanceledException ex)
            {
                Log.Error("Signing cancelled...", ex);
                (_reporter as IProgress<string>).Report("Operation cancelled");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (WebException ex)
            {
                Log.Error("Network error..", ex); 
                (_reporter as IProgress<string>).Report("Network error");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (CryptographicException ex)
            {
                Log.Error("Cryptographic error...", ex);
                (_reporter as IProgress<string>).Report("Cryptographic error");
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            catch (COMException ex)
            {
                Log.Error("Error while signing...", ex);
                (_reporter as IProgress<string>).Report(ex.Message); 
            }
            catch (Exception ex)
            {
                Log.Error("Error while signing...", ex);
                (_reporter as IProgress<string>).Report(ex.Message);
            }
            finally
            {
                InProgress = false;
            }
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
