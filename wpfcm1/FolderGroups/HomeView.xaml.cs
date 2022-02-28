using System.Windows.Controls;

namespace wpfcm1.FolderGroups
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            if (!Settings.User.Default.InvoicesOutbound) TileOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileOutboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.InvoicesInbound) TileInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.IosInbound) TileIosInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileIosInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.IosOutbound) TileIosOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileIosOutboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.KpInbound) TileKpInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileKpInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.OtherOutbound) TileOtherOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileOtherOutboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.PovratiInbound) TilePovratiInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TilePovratiInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.KpOutbound) TileKpOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileKpOutboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.OtherInbound) TileOtherInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileOtherInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.OtpadInbound) TileOtpadInboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileOtpadInboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.OtpadOutbound) TileOtpadOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TileOtpadOutboundVM.Visibility = System.Windows.Visibility.Visible;
            if (!Settings.User.Default.PovratiOutbound) TilePovratiOutboundVM.Visibility = System.Windows.Visibility.Collapsed; else TilePovratiOutboundVM.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
