using System.Windows.Controls;

namespace wpfcm1.FolderTypes
{
    public partial class GeneratedFolderView : UserControl
    {

        public GeneratedFolderView()
        {
            InitializeComponent();

            if (System.Text.RegularExpressions.Regex.IsMatch(Settings.User.Default.PIB, "99997"))
                Izdavalac.Visibility = System.Windows.Visibility.Visible ;
            else
                Izdavalac.Visibility = System.Windows.Visibility.Collapsed; 
        }
    }
}
