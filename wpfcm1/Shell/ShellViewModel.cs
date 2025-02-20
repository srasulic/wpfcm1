using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using wpfcm1.Certificates;
using wpfcm1.DataAccess;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.FolderGroups;
using wpfcm1.FolderTypes;
using wpfcm1.Preview;
using wpfcm1.Toolbar;

namespace wpfcm1.Shell
{
    public interface IShell { }

    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell, IHandle<MessageShowHome>, IHandle<MessageShowWeb>, IHandle<MessageSync>, IHandle<MessagePickCert>
    {
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, ToolBarViewModel toolBar, CertificatesViewModel certs)
        {
            DisplayName = "eDokument PoliSign     " + AppBootstrapper.appVersion;
            _events = events;
            _events.SubscribeOnUIThread(this);
            _windowManager = windowManager;

            ToolBar = toolBar;
            CertVM = certs;

            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Izlazne Fakture", events, _windowManager);
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Ulazne Fakture", events, _windowManager);
            IosOutboundVM = new FolderGroupViewModel(FolderManager.IosOutboundFolders, "Izlazni IOS", events, _windowManager);
            IosInboundVM = new FolderGroupViewModel(FolderManager.IosInboundFolders, "Ulazni IOS", events, _windowManager);
            OtpadOutboundVM = new FolderGroupViewModel(FolderManager.OtpadOutboundFolders, "Izl. Kret.otp", events, _windowManager);
            OtpadInboundVM = new FolderGroupViewModel(FolderManager.OtpadInboundFolders, "Ul. Kret.otp.", events, _windowManager);
            OtpremnicaOutboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaOutboundFolders, "Izl. Otpremnica", events, _windowManager);
            OtpremnicaInboundVM = new FolderGroupViewModel(FolderManager.OtpremnicaInboundFolders, "Ul. Otpremnica", events, _windowManager);
            KpOutboundVM = new FolderGroupViewModel(FolderManager.KpOutboundFolders, "Izlazna KP", events, _windowManager);
            KpInboundVM = new FolderGroupViewModel(FolderManager.KpInboundFolders, "Ulazna KP", events, _windowManager);
            PovratiOutboundVM = new FolderGroupViewModel(FolderManager.PovratiOutboundFolders, "Izlazni Povrati", events, _windowManager);
            PovratiInboundVM = new FolderGroupViewModel(FolderManager.PovratiInboundFolders, "Ulazni Povrati", events, _windowManager);
            OtherOutboundVM = new FolderGroupViewModel(FolderManager.OtherOutboundFolders, "Ostali Izlazni", events, _windowManager);
            OtherInboundVM = new FolderGroupViewModel(FolderManager.OtherInboundFolders, "Ostali Ulazni", events, _windowManager);
            WebVM = new WebViewModel("Online report");
            LoginVM = new DialogLoginViewModel(_windowManager);

            ShowLogin();
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellation = default)
        {
            InboundVM.Dispose();
            OutboundVM.Dispose();
            IosInboundVM.Dispose();
            IosOutboundVM.Dispose();
            OtpadInboundVM.Dispose();
            OtpadOutboundVM.Dispose();
            OtpremnicaInboundVM.Dispose();
            OtpremnicaOutboundVM.Dispose();
            KpInboundVM.Dispose();
            KpOutboundVM.Dispose();
            PovratiInboundVM.Dispose();
            PovratiOutboundVM.Dispose();
            OtherInboundVM.Dispose();
            OtherOutboundVM.Dispose();
            WebVM.Dispose();
            LoginVM.Dispose();

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
        public WebViewModel WebVM { get; set; }
        public DialogLoginViewModel LoginVM { get; set; }

        public void ShowLogin()
        {
            //*****************************************************************
            // dok ne nađemo način da sprečimo iskakanje Please insert card, ovo će biti privremenio isključeno.
            // CertVM.RefreshCertificateList();
            //*****************************************************************
            ActivateItemAsync(HomeVM);
            _events.PublishOnUIThreadAsync(new MessageViewModelActivated(ActiveItem.GetType().Name));

            var result = _windowManager.ShowDialogAsync(LoginVM);
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

        public void ShowWeb()
        {
            ActivateItemAsync(WebVM);
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

        public void Handle(MessageShowWeb message)
        {
            ShowWeb();
        }

        public void Handle(MessageSync message)
        {
            var foldersToSync = new Dictionary<string, FolderViewModel>()
            {
                {FolderManager.InvoicesOutboundOutboxFolder, OutboundVM.FolderVMs[1]},
                {FolderManager.InvoicesInboundInboxFolder, InboundVM.FolderVMs[0]},
                {FolderManager.InvoicesInboundOutboxFolder, InboundVM.FolderVMs[1]},
                {FolderManager.IosOutboundOutboxFolder, IosOutboundVM.FolderVMs[1]},
                {FolderManager.IosInboundInboxFolder, IosInboundVM.FolderVMs[0]},
                {FolderManager.IosInboundOutboxFolder, IosInboundVM.FolderVMs[1]},
                {FolderManager.OtpadOutboundOutboxFolder, OtpadOutboundVM.FolderVMs[1]},
                {FolderManager.OtpadInboundInboxFolder, OtpadInboundVM.FolderVMs[0]},
                {FolderManager.OtpadInboundOutboxFolder, OtpadInboundVM.FolderVMs[1]},
                {FolderManager.OtpremnicaOutboundOutboxFolder, OtpremnicaOutboundVM.FolderVMs[1]},
                {FolderManager.OtpremnicaInboundInboxFolder, OtpremnicaInboundVM.FolderVMs[0]},
                {FolderManager.OtpremnicaInboundOutboxFolder, OtpremnicaInboundVM.FolderVMs[1]},
                {FolderManager.KpOutboundOutboxFolder, KpOutboundVM.FolderVMs[1]},
                {FolderManager.KpInboundInboxFolder, KpInboundVM.FolderVMs[0]},
                {FolderManager.KpInboundOutboxFolder, KpInboundVM.FolderVMs[1]},
                {FolderManager.PovratiOutboundOutboxFolder, PovratiOutboundVM.FolderVMs[1]},
                {FolderManager.PovratiInboundInboxFolder, PovratiInboundVM.FolderVMs[0]},
                {FolderManager.PovratiInboundOutboxFolder, PovratiInboundVM.FolderVMs[1]},
                {FolderManager.OtherOutboundOutboxFolder, OtherOutboundVM.FolderVMs[1]},
                {FolderManager.OtherInboundInboxFolder, OtherInboundVM.FolderVMs[0]},
                {FolderManager.OtherInboundOutboxFolder, OtherInboundVM.FolderVMs[1]},
            };

            //TODO: ovo mora drugacije
            _events.PublishOnUIThreadAsync(new MessageShowPdf(PreviewViewModel.Empty));
            var result = _windowManager.ShowDialogAsync(new DialogSyncViewModel(foldersToSync));
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

        public Task HandleAsync(MessageShowWeb message, CancellationToken cancellationToken)
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
