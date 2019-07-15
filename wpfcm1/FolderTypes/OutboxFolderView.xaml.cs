using System.Windows.Controls;

namespace wpfcm1.FolderTypes
{
    public partial class OutboxFolderView : UserControl
    {
        public OutboxFolderView()
        {
            InitializeComponent();
            if (System.Text.RegularExpressions.Regex.IsMatch(Settings.User.Default.PIB, "99997"))
            {
                Izdavalac.Visibility = System.Windows.Visibility.Visible;
                PibIzdavalac.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Izdavalac.Visibility = System.Windows.Visibility.Collapsed;
                PibIzdavalac.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
