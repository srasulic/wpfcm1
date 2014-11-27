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
            OutboundVM = new WorkspaceViewModel(FolderManager.InvoicesOutboundFolders, "Outbound");
            InboundVM = new WorkspaceViewModel(FolderManager.InvoicesInboundFolders, "Inbound");
            HomeVM = new HomeViewModel(this);
            DisplayName = "Shell";

            ShowHome();
        }

        public WorkspaceViewModel OutboundVM { get; private set; }
        public WorkspaceViewModel InboundVM { get; private set; }
        public HomeViewModel HomeVM { get; private set; }

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
