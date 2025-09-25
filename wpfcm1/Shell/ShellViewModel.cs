using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Deployment.Application;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Certificates;
using wpfcm1.DataAccess;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.FolderGroups;
using wpfcm1.OlympusApi;
using wpfcm1.Preview;
using wpfcm1.Settings;
using wpfcm1.Toolbar;

namespace wpfcm1.Shell
{
    public interface IShell { }

    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell, IHandle<MessageShowHome>, IHandle<MessageSync>, IHandle<MessagePickCert>
    {
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, ToolBarViewModel toolBar, CertificatesViewModel certs)
        {
            DisplayName = string.Empty;
            _events = events;
            _events.SubscribeOnUIThread(this);
            _windowManager = windowManager;

            ToolBar = toolBar;
            CertVM = certs;

            HomeVM = new HomeViewModel(this);

            //OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Izlazne Fakture", events, _windowManager);
            //InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Ulazne Fakture", events, _windowManager);
            //IosOutboundVM = new FolderGroupViewModel(FolderManager.IosOutboundFolders, "Izlazni IOS", events, _windowManager);
            //IosInboundVM = new FolderGroupViewModel(FolderManager.IosInboundFolders, "Ulazni IOS", events, _windowManager);
            //OtpadOutboundVM = new FolderGroupViewModel(FolderManager.OtpadOutboundFolders, "Izl. Kret.otp", events, _windowManager);
            //OtpadInboundVM = new FolderGroupViewModel(FolderManager.OtpadInboundFolders, "Ul. Kret.otp.", events, _windowManager);
            //OtpremnicaOutboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaOutboundFolders, "Izl. Otpremnica", events, _windowManager);
            //OtpremnicaInboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaInboundFolders, "Ul. Otpremnica", events, _windowManager);
            //KpOutboundVM = new FolderGroupViewModel(FolderManager.KpOutboundFolders, "Izlazna KP", events, _windowManager);
            //KpInboundVM = new FolderGroupViewModel(FolderManager.KpInboundFolders, "Ulazna KP", events, _windowManager);
            //PovratiOutboundVM = new FolderGroupViewModel(FolderManager.PovratiOutboundFolders, "Izlazni Povrati", events, _windowManager);
            //PovratiInboundVM = new FolderGroupViewModel(FolderManager.PovratiInboundFolders, "Ulazni Povrati", events, _windowManager);
            //OtherOutboundVM = new FolderGroupViewModel(FolderManager.OtherOutboundFolders, "Ostali Izlazni", events, _windowManager);
            //OtherInboundVM = new FolderGroupViewModel(FolderManager.OtherInboundFolders, "Ostali Ulazni", events, _windowManager);

            LoginVM = new DialogLoginViewModel(_windowManager);

            ShowLogin();
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellation = default)
        {
            InboundVM?.Dispose();
            OutboundVM?.Dispose();
            IosInboundVM?.Dispose();
            IosOutboundVM?.Dispose();
            OtpadInboundVM?.Dispose();
            OtpadOutboundVM?.Dispose();
            OtpremnicaInboundVM?.Dispose();
            OtpremnicaOutboundVM?.Dispose();
            KpInboundVM?.Dispose();
            KpOutboundVM?.Dispose();
            PovratiInboundVM?.Dispose();
            PovratiOutboundVM?.Dispose();
            OtherInboundVM?.Dispose();
            OtherOutboundVM?.Dispose();
            LoginVM?.Dispose();

            return Task.CompletedTask;
        }

        //Imported components:
        public ToolBarViewModel ToolBar { get; set; }
        public CertificatesViewModel CertVM { get; private set; }
        //Screens:
        public HomeViewModel HomeVM { get; set; }
        public FolderGroupViewModel OutboundVM { get; set; }
        public FolderGroupViewModel InboundVM { get; set; }
        public FolderGroupViewModel IosOutboundVM { get; set; }
        public FolderGroupViewModel IosInboundVM { get; set; }
        public FolderGroupViewModel OtpadOutboundVM { get; set; }
        public FolderGroupViewModel OtpadInboundVM { get; set; }
        public FolderGroupViewModel OtpremnicaOutboundVM { get; set; }
        public FolderGroupViewModel OtpremnicaInboundVM { get; set; }
        public FolderGroupViewModel KpOutboundVM { get; set; }
        public FolderGroupViewModel KpInboundVM { get; set; }
        public FolderGroupViewModel PovratiOutboundVM { get; set; }
        public FolderGroupViewModel PovratiInboundVM { get; set; }
        public FolderGroupViewModel OtherOutboundVM { get; set; }
        public FolderGroupViewModel OtherInboundVM { get; set; }
        public DialogLoginViewModel LoginVM { get; set; }

        public async void ShowLogin()
        {
            //*****************************************************************
            // dok ne nađemo način da sprečimo iskakanje Please insert card, ovo će biti privremenio isključeno.
            // CertVM.RefreshCertificateList();
            //*****************************************************************
            await ActivateItemAsync(HomeVM);
            await _events.PublishOnUIThreadAsync(new MessageViewModelActivated(ActiveItem.GetType().Name));

            var settings = new Dictionary<string, object>
            {
                { "Topmost", false },
            };
            var result = await _windowManager.ShowDialogAsync(LoginVM, null, settings);
            
            if (result == false)
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Izlazne Fakture", _events, _windowManager);
                InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Ulazne Fakture", _events, _windowManager);
                IosOutboundVM = new FolderGroupViewModel(FolderManager.IosOutboundFolders, "Izlazni IOS", _events, _windowManager);
                IosInboundVM = new FolderGroupViewModel(FolderManager.IosInboundFolders, "Ulazni IOS", _events, _windowManager);
                OtpadOutboundVM = new FolderGroupViewModel(FolderManager.OtpadOutboundFolders, "Izl. Kret.otp", _events, _windowManager);
                OtpadInboundVM = new FolderGroupViewModel(FolderManager.OtpadInboundFolders, "Ul. Kret.otp.", _events, _windowManager);
                OtpremnicaOutboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaOutboundFolders, "Izl. Otpremnica", _events, _windowManager);
                OtpremnicaInboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaInboundFolders, "Ul. Otpremnica", _events, _windowManager);
                KpOutboundVM = new FolderGroupViewModel(FolderManager.KpOutboundFolders, "Izlazna KP", _events, _windowManager);
                KpInboundVM = new FolderGroupViewModel(FolderManager.KpInboundFolders, "Ulazna KP", _events, _windowManager);
                PovratiOutboundVM = new FolderGroupViewModel(FolderManager.PovratiOutboundFolders, "Izlazni Povrati", _events, _windowManager);
                PovratiInboundVM = new FolderGroupViewModel(FolderManager.PovratiInboundFolders, "Ulazni Povrati", _events, _windowManager);
                OtherOutboundVM = new FolderGroupViewModel(FolderManager.OtherOutboundFolders, "Ostali Izlazni", _events, _windowManager);
                OtherInboundVM = new FolderGroupViewModel(FolderManager.OtherInboundFolders, "Ostali Ulazni", _events, _windowManager);

                SetFolderGroupVisibility();

                // brisanje starih fajlova u obradjeno i loca_sent
                Profile profile = OlympusService.DeserializeFromJson<Profile>(User.Default.JsonProfile);
                int br_dana_obradjeno = profile.tenant_info.br_dana_obradjeno;
                int br_dana_local_sent = profile.tenant_info.br_dana_local_sent;
                string rootDir = Path.Combine(Folders.Default.RootFolder, profile.tenant_info.tenant);
                DeleteOldFilesIn(rootDir, "obradjeno", br_dana_obradjeno);
                DeleteOldFilesIn(rootDir, "local_sent", br_dana_local_sent);

                HomeVM = new HomeViewModel(this);
                await ActivateItemAsync(HomeVM);
                await _events.PublishOnUIThreadAsync(new MessageViewModelActivated(ActiveItem.GetType().Name));

                // mora ovako jer je izgubljen publish sertifikata na UI thread
                // (CertVM raspalio poruku pre nego sto je formiran HomeVM)
                CertVM.OnSelectedCertificate();

                DisplayName = User.Default.AppTitle;
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    DisplayName += " - " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }

                //var version = Assembly.GetExecutingAssembly().GetName().Version;
                //DisplayName += " " + version;

                AppBootstrapper.appVersion = DisplayName;
            }
        }

        private void SetFolderGroupVisibility()
        {
            Profile profile = OlympusService.DeserializeFromJson<Profile>(User.Default.JsonProfile);
            foreach (var item in profile.tip_dok_pristup)
            {
                if (item.tip_dok == "faktura" && item.smer == "outbound")
                {
                    OutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "faktura" && item.smer == "inbound")
                {
                    InboundVM.IsVisible = true;
                }

                if (item.tip_dok == "ios" && item.smer == "outbound")
                {
                    IosOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "ios" && item.smer == "inbound")
                {
                    IosInboundVM.IsVisible = true;
                }

                if (item.tip_dok == "otpad" && item.smer == "outbound")
                {
                    OtpadOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "otpad" && item.smer == "inbound")
                {
                    OtpadInboundVM.IsVisible = true;
                }

                if (item.tip_dok == "otpremnica" && item.smer == "outbound")
                {
                    OtpremnicaOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "otpremnica" && item.smer == "inbound")
                {
                    OtpremnicaInboundVM.IsVisible = true;
                }

                if (item.tip_dok == "kp" && item.smer == "outbound")
                {
                    KpOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "kp" && item.smer == "inbound")
                {
                    KpInboundVM.IsVisible = true;
                }

                if (item.tip_dok == "povrati" && item.smer == "outbound")
                {
                    PovratiOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "povrati" && item.smer == "inbound")
                {
                    PovratiInboundVM.IsVisible = true;
                }

                if (item.tip_dok == "ostali" && item.smer == "outbound")
                {
                    OtherOutboundVM.IsVisible = true;
                }
                if (item.tip_dok == "ostali" && item.smer == "inbound")
                {
                    OtherInboundVM.IsVisible = true;
                }
            }
        }

        private void DeleteOldFilesIn(string rootDir, string dirName, int br_dana)
        {
            foreach (string dir in Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories))
            {
                string folderName = Path.GetFileName(dir);

                if (folderName.Equals(dirName, StringComparison.OrdinalIgnoreCase))
                {
                    if (br_dana == -1)
                    {
                        // nema brisanja
                    }
                    else if (br_dana == 0)
                    {
                        Log.Info($"Cleaning target folder: {dir}");

                        // brisemo sve
                        foreach (string file in Directory.GetFiles(dir))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error deleting {file}: {ex.Message}");
                            }
                        }
                    }
                    else if (br_dana > 0)
                    {
                        Log.Info($"Cleaning target folder: {dir}");

                        DateTime threshold = DateTime.Now.AddDays(-br_dana);

                        foreach (string file in Directory.GetFiles(dir))
                        {
                            try
                            {
                                DateTime lastWriteTime = File.GetLastWriteTime(file);

                                if (lastWriteTime.Date < threshold.Date)
                                {
                                    File.Delete(file);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error deleting {file}: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        public void ShowHome()
        {
            ActivateItemAsync(HomeVM);
            _events.PublishOnUIThreadAsync(new MessageViewModelActivated(ActiveItem.GetType().Name));
        }

        public void ShowOutbound()
        {
            ActivateItemAsync(OutboundVM);
        }

        public void ShowInbound()
        {
            ActivateItemAsync(InboundVM);
        }

        public void ShowIosOutbound()
        {
            ActivateItemAsync(IosOutboundVM);
        }

        public void ShowIosInbound()
        {
            ActivateItemAsync(IosInboundVM);
        }
        public void ShowOtpadOutbound()
        {
            ActivateItemAsync(OtpadOutboundVM);
        }

        public void ShowOtpremnicaOutbound()
        {
            ActivateItemAsync(OtpremnicaOutboundVM);
        }

        public void ShowOtpadInbound()
        {
            ActivateItemAsync(OtpadInboundVM);
        }

        public void ShowOtpremnicaInbound()
        {
            ActivateItemAsync(OtpremnicaInboundVM);
        }

        public void ShowKpOutbound()
        {
            ActivateItemAsync(KpOutboundVM);
        }

        public void ShowKpInbound()
        {
            ActivateItemAsync(KpInboundVM);
        }

        public void ShowPovratiOutbound()
        {
            ActivateItemAsync(PovratiOutboundVM);
        }

        public void ShowPovratiInbound()
        {
            ActivateItemAsync(PovratiInboundVM);
        }

        public void ShowOtherOutbound()
        {
            ActivateItemAsync(OtherOutboundVM);
        }

        public void ShowOtherInbound()
        {
            ActivateItemAsync(OtherInboundVM);
        }

        public void ShowSettings()
        {
            var result = _windowManager.ShowDialogAsync(new DialogSettingsViewModel());
        }

        public void ShowAbout()
        {
            var result = _windowManager.ShowDialogAsync(new DialogAboutViewModel());
        }

        public void Handle(MessageShowHome message)
        {
            ShowHome();
        }

        public void Handle(MessageSync message)
        {
            //TODO: ovo mora drugacije
            _events.PublishOnUIThreadAsync(new MessageShowPdf(PreviewViewModel.Empty));

            var settings = new Dictionary<string, object>
            {
                { "Topmost", false },
            };
            var result = _windowManager.ShowDialogAsync(new DialogSyncViewModel(), null, settings);
        }

        public void Handle(MessagePickCert message)
        {
            bool pickCertificate = true;
            CertVM.RefreshCertificateList(pickCertificate);
        }

        public Task HandleAsync(MessageShowHome message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }

        public Task HandleAsync(MessageSync message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }

        public Task HandleAsync(MessagePickCert message, CancellationToken cancellationToken)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}
