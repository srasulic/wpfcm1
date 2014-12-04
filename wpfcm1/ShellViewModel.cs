using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.DataAccess;
using wpfcm1.Events;
using wpfcm1.Toolbar;
using wpfcm1.ViewModels;

namespace wpfcm1
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell, IHandle<MessageShowHome>
    {
        private readonly IEventAggregator _events;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events, ToolBarViewModel toolBar, CertificatesViewModel certs)
        {
            DisplayName = "Invoices";
            _events = events;
            _events.Subscribe(this);

            ToolBar = toolBar;
            CertVM = certs;

            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Outbound", events);
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Inbound", events);

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
            _events.PublishOnUIThread(new ViewModelActivatedMessage(ActiveItem.GetType().Name));
        }

        public void ShowOutbound()
        {
            ActivateItem(OutboundVM);
        }

        public void ShowInbound()
        {
            ActivateItem(InboundVM);
        }

        public void Handle(MessageShowHome message)
        {
            ShowHome();
        }
    }
}
