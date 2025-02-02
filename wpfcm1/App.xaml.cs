using System.Windows;
using System.Windows.Media;

namespace wpfcm1
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();

            if (Settings.User.Default.Variation == "BIH")
            {
                Resources["WinTitleColor"] = Color.FromRgb(0, 102, 102);
            }
            else
            {
                Resources["WinTitleColor"] = Color.FromRgb(90, 169, 209);
            }
        }


    }
}
