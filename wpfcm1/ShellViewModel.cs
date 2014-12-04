using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.DataAccess;
using wpfcm1.Toolbar;
using wpfcm1.ViewModels;

namespace wpfcm1
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell
    {
        [ImportingConstructor]
        public ShellViewModel(ToolBarViewModel toolBar)
        {
            ToolBar = toolBar;
            CertVM = new CertificatesViewModel();
            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Outbound");
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Inbound");
            DisplayName = "Invoices";

            ShowHome();
        }

        public ToolBarViewModel ToolBar { get; set; }
        public CertificatesViewModel CertVM { get; private set; }
        //Screens:
        public HomeViewModel HomeVM { get; private set; }
        public FolderGroupViewModel OutboundVM { get; private set; }
        public FolderGroupViewModel InboundVM { get; private set; }

        public void ShowHome()
        {
            ActivateItem(HomeVM);
        }

        public void ShowOutbound()
        {
            ActivateItem(OutboundVM);
        }

        public void ShowInbound()
        {
            ActivateItem(InboundVM);
        }
    }
}
