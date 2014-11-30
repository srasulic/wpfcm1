using Caliburn.Micro;
using System.ComponentModel.Composition;
using wpfcm1.DataAccess;
using wpfcm1.ViewModels;

namespace wpfcm1
{
    [Export(typeof(IShell))]
    public sealed class ShellViewModel : Conductor<object>, IShell
    {
        public ShellViewModel()
        {
            CertVM = new CertificatesViewModel();
            HomeVM = new HomeViewModel(this);
            OutboundVM = new FolderGroupViewModel(FolderManager.InvoicesOutboundFolders, "Outbound");
            InboundVM = new FolderGroupViewModel(FolderManager.InvoicesInboundFolders, "Inbound");
            DisplayName = "Invoices";

            ShowHome();
        }

        public CertificatesViewModel CertVM { get; private set; }
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
