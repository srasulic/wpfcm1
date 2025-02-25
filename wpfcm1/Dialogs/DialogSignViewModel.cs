using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Certificates;
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
        private bool _buttonApproveVisible; 

        public DialogSignViewModel(CertificateModel certificate, FolderViewModel folder)
        {
            DisplayName = "PoliSign - potpisivanje dokumenata";
            _certificate = certificate;
            _folder = folder;
            _reporter = new Progress<string>();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            Reports = new BindableCollection<string>();
            
            if (Regex.IsMatch(certificate.Certificate.Issuer, @"CN=MUPCA") )
            {
                Reports.Add("! ! ! >>UPOZORENJE<< ! ! !");
                Reports.Add("MUP sertifikat nije pogodan za poslovnu upotrebu i neće biti podržan od 01.11.2019.");
                Reports.Add("! ! ! >>UPOZORENJE<< ! ! !");
                Reports.Add("Kako bi izbegli zastoj u radu, molimo da što pre uputite zahtev za PTT CA, PKS CA ili Halcom sertifikata");
                Reports.Add("OBJAŠNJENJE:");
                Reports.Add("  MUP CA jeste kvalifikovani sertifikat, ali nije pogodan za potpisivanje velikog broja dokumenata. ");
                Reports.Add("  Naime, kvalifikovanim potpisivanjem dokumenta ovim sertifikatom veličina potpisanog ");
                Reports.Add("  dokumenta prelazi 5MB. Ovo je 10 puta više od prihvatljive veličine. To znači da bi ");
                Reports.Add("  izdatak pravnog lica za čuvanje elektronski potpisanih dokumenata koji ");
                Reports.Add("  je projektovan na iznos od 100 EUR mesečno, narastao na 1000 EUR mesečno ");
                Reports.Add("  ukoliko se koristi MUP sertifikat.");
                Reports.Add("  Kako bi rast arhive podataka i budućih troškova svih korisnika sistema stavili pod kontrolu, ");
                Reports.Add("  primorani smo da ukinemo podršku za MUP sertifikate.");
                Reports.Add("  --------------");
            }
            Reports.Add("Dokumenti obeleženi za potpisivanje:");
            var Documents = GetDocumentsForSigning(_folder);
            foreach (var document in Documents)
            {
                Reports.Add(document.DocumentInfo.Name);
            }

            if (_folder is InboxFolderViewModel)
            {
                _buttonApproveVisible = true;
            }
            else
            {
                _buttonApproveVisible = false;
            }

        }

        public bool buttonApproveVisible { 
            get { return _buttonApproveVisible; } 
            set { }  
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

        public async Task OnClose()
        {
            await TryCloseAsync(true);
        }

        public bool CanOnClose { get { return !InProgress; } }

        public void OnCancel()
        {
            if (_cancellation != null) _cancellation.Cancel();
        }

        private bool _canOnStart = true;
        public bool CanOnStart
        {
            set { _canOnStart = value; }
            get { return _canOnStart && !InProgress; } 
        }

        public async void OnStart()
        {
            FolderViewModel.PsKillPdfHandlers();
            Reports.Clear();
            Reports.Add("Priprema za potpisivanje...");
            CanOnStart = false;
            try
            {
                _cancellation = new CancellationTokenSource();
                InProgress = true;
                // shallow documents copy, always updated, even if cancel was pressed
                var documents = GetDocumentsForSigning(_folder);
                var sourceDir = _folder.FolderPath; 
                var signingMgr = new SigningManager(_certificate, documents, sourceDir, _folder);
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

        public void OnApprove()
        {
            if (_folder is InboxFolderViewModel)
            {
                (_folder as InboxFolderViewModel).SetApproved(true);
                Reports.Clear();
                Reports.Add("Dokumenti su označeni kao ispravni i odobreni za dalju obradu i potpisivanje...");
            }
        }

        public void OnApproveNot()
        {
            if (_folder is InboxFolderViewModel)
            {
                (_folder as InboxFolderViewModel).SetApproved(false);
                Reports.Clear();
                Reports.Add("Dokumenti su označeni kao nevalidni za dalju obradu i potpisivanje...");
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
