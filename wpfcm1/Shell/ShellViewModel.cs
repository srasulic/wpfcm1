using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    public sealed class ShellViewModel : Conductor<object>, IShell, IHandle<MessageShowHome>, IHandle<MessageShowWeb>, IHandle<MessageSync>
    {
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, ToolBarViewModel toolBar, CertificatesViewModel certs)
        {
            DisplayName = "eDokument PoliSign";
            _events = events;
            _events.Subscribe(this);
            _windowManager = windowManager;

            ToolBar = toolBar;
            CertVM = certs;           
            

            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Izlazne Fakture", events, _windowManager);
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Ulazne Fakture", events, _windowManager);
            IosOutboundVM = new FolderGroupViewModel(FolderManager.IosOutboundFolders, "Izlazni IOS", events, _windowManager);
            IosInboundVM = new FolderGroupViewModel(FolderManager.IosInboundFolders, "Ulazni IOS", events, _windowManager);
            KpOutboundVM = new FolderGroupViewModel(FolderManager.KpOutboundFolders, "Izlazna KP", events, _windowManager);
            KpInboundVM = new FolderGroupViewModel(FolderManager.KpInboundFolders, "Ulazna KP", events, _windowManager);
            OtherOutboundVM = new FolderGroupViewModel(FolderManager.OtherOutboundFolders, "Ostali Izlazni", events, _windowManager);
            OtherInboundVM = new FolderGroupViewModel(FolderManager.OtherInboundFolders, "Ostali Ulazni", events, _windowManager);
            WebVM = new WebViewModel("Online report");
            LoginVM = new DialogLoginViewModel(_windowManager);
            //var result = _windowManager.ShowDialog(new DialogLoginViewModel());


            ShowHome();
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
        public FolderGroupViewModel KpOutboundVM { get; set; }
        public FolderGroupViewModel KpInboundVM { get; set; }
        public FolderGroupViewModel OtherOutboundVM { get; set; }
        public FolderGroupViewModel OtherInboundVM { get; set; }
        public WebViewModel WebVM { get; set; }
        public DialogLoginViewModel LoginVM { get; set; }

        //For login screen 
        private static bool first = true;


        public void ShowHome()
        {   //*****************************************************************
            // dok ne nađemo način da sprečimo iskakanje Please insert card, ovo će biti privremenio isključeno.
            //CertVM.RefreshCertificateList();
            //*****************************************************************
            ActivateItem(HomeVM);
            _events.PublishOnUIThread(new MessageViewModelActivated(ActiveItem.GetType().Name));

            if (first)
            {
                first = false;
                _windowManager.ShowDialog(new DialogLoginViewModel(_windowManager));
            }
        }

        public void ShowOutbound()
        {
            ActivateItem(OutboundVM);
        }

        public void ShowInbound()
        {
            ActivateItem(InboundVM);
        }

        public void ShowIosOutbound()
        {
            ActivateItem(IosOutboundVM);
        }

        public void ShowIosInbound()
        {
            ActivateItem(IosInboundVM);
        }

        public void ShowKpOutbound()
        {
            ActivateItem(KpOutboundVM);
        }

        public void ShowKpInbound()
        {
            ActivateItem(KpInboundVM);
        }

        public void ShowOtherOutbound()
        {
            ActivateItem(OtherOutboundVM);
        }

        public void ShowOtherInbound()
        {
            ActivateItem(OtherInboundVM);
        }

        public void ShowWeb()
        {
            ActivateItem(WebVM);
        }


        public void ShowSettings()
        {
            var result = _windowManager.ShowDialog(new DialogSettingsViewModel());
        }

        public void ShowAbout()
        {
            var result = _windowManager.ShowDialog(new DialogAboutViewModel());
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
                {FolderManager.InvoicesOutboundPendFolder, OutboundVM.FolderVMs[3]},
                {FolderManager.InvoicesOutboundConfirmedFolder, OutboundVM.FolderVMs[4]},
                {FolderManager.InvoicesInboundInboxFolder, InboundVM.FolderVMs[0]},
                {FolderManager.InvoicesInboundOutboxFolder, InboundVM.FolderVMs[1]},
                {FolderManager.InvoicesInboundConfirmedFolder, InboundVM.FolderVMs[3]},
                {FolderManager.IosOutboundOutboxFolder, IosOutboundVM.FolderVMs[1]},
                {FolderManager.IosOutboundPendFolder, IosOutboundVM.FolderVMs[3]},
                {FolderManager.IosOutboundConfirmedFolder, IosOutboundVM.FolderVMs[4]},
                {FolderManager.IosInboundInboxFolder, IosInboundVM.FolderVMs[0]},
                {FolderManager.IosInboundOutboxFolder, IosInboundVM.FolderVMs[1]},
                {FolderManager.IosInboundConfirmedFolder, IosInboundVM.FolderVMs[3]},
                {FolderManager.KpOutboundOutboxFolder, KpOutboundVM.FolderVMs[1]},
                {FolderManager.KpOutboundPendFolder, KpOutboundVM.FolderVMs[3]},
                {FolderManager.KpOutboundConfirmedFolder, KpOutboundVM.FolderVMs[4]},
                {FolderManager.KpInboundInboxFolder, KpInboundVM.FolderVMs[0]},
                {FolderManager.KpInboundOutboxFolder, KpInboundVM.FolderVMs[1]},
                {FolderManager.KpInboundConfirmedFolder, KpInboundVM.FolderVMs[3]},
                {FolderManager.OtherOutboundOutboxFolder, OtherOutboundVM.FolderVMs[1]},
                {FolderManager.OtherOutboundPendFolder, OtherOutboundVM.FolderVMs[3]},
                {FolderManager.OtherOutboundConfirmedFolder, OtherOutboundVM.FolderVMs[4]},
                {FolderManager.OtherInboundInboxFolder, OtherInboundVM.FolderVMs[0]},
                {FolderManager.OtherInboundOutboxFolder, OtherInboundVM.FolderVMs[1]},
                {FolderManager.OtherInboundConfirmedFolder, OtherInboundVM.FolderVMs[3]}
            };

            //TODO: ovo mora drugacije
            _events.PublishOnUIThread(new MessageShowPdf(PreviewViewModel.Empty));
            var result = _windowManager.ShowDialog(new DialogSyncViewModel(foldersToSync));
        }

        protected override void OnDeactivate(bool close)
        {
            InboundVM.Dispose();
            OutboundVM.Dispose();
            IosInboundVM.Dispose();
            IosOutboundVM.Dispose();
            KpInboundVM.Dispose();
            KpOutboundVM.Dispose();
            OtherInboundVM.Dispose();
            OtherOutboundVM.Dispose();
            WebVM.Dispose();

        }
    }
}
