using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using wpfcm1.Certificates;
using wpfcm1.DataAccess;
using wpfcm1.Dialogs;
using wpfcm1.Events;
using wpfcm1.FolderGroups;
using wpfcm1.FolderTypes;
using wpfcm1.Toolbar;

namespace wpfcm1.Shell
{
    public interface IShell { }

    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell, IHandle<MessageShowHome>, IHandle<MessageSync>
    {
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events, IWindowManager windowManager, ToolBarViewModel toolBar, CertificatesViewModel certs)
        {
            DisplayName = "Invoices";
            _events = events;
            _events.Subscribe(this);
            _windowManager = windowManager;

            ToolBar = toolBar;
            CertVM = certs;

            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Outbound", events, _windowManager);
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Inbound", events, _windowManager);

            ShowHome();
        }

        //Imported components:
        public ToolBarViewModel ToolBar { get; set; }
        public CertificatesViewModel CertVM { get; private set; }
        //Screens:
        public HomeViewModel HomeVM { get; private set; }
        public FolderGroupViewModel OutboundVM { get; private set; }
        public FolderGroupViewModel InboundVM { get; private set; }

        public void ShowHome()
        {
            ActivateItem(HomeVM);
            _events.PublishOnUIThread(new MessageViewModelActivated(ActiveItem.GetType().Name));
        }

        public void ShowOutbound()
        {
            ActivateItem(OutboundVM);
        }

        public void ShowInbound()
        {
            ActivateItem(InboundVM);
        }

        public void ShowSettings()
        {
            var result = _windowManager.ShowDialog(new DialogSettingsViewModel());
        }

        public void Handle(MessageShowHome message)
        {
            ShowHome();
        }

        public void Handle(MessageSync message)
        {
            var foldersToSync = new Dictionary<string, FolderViewModel>()
            {
                {FolderManager.OutboundOutboxFolder, OutboundVM.FolderVMs[1]},
                {FolderManager.OutboundPendFolder, OutboundVM.FolderVMs[3]},
                {FolderManager.OutboundConfirmedFolder, OutboundVM.FolderVMs[4]},
                {FolderManager.InboundInboxFolder, InboundVM.FolderVMs[0]},
                {FolderManager.InboundOutboxFolder, InboundVM.FolderVMs[1]},
                {FolderManager.InboundConfirmedFolder, InboundVM.FolderVMs[3]}
            };
            var result = _windowManager.ShowDialog(new DialogSyncViewModel(foldersToSync));
        }

        protected override void OnDeactivate(bool close)
        {
            InboundVM.Dispose();
            OutboundVM.Dispose();
        }
    }
}
