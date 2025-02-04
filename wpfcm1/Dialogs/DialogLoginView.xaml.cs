using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace wpfcm1.Dialogs
{
    public partial class DialogLoginView : MetroWindow
    {
        public DialogLoginView()
        {
            InitializeComponent();
        }

        private void DialogLoginView_OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic)DataContext).Password = (sender as PasswordBox)?.Password;
        }
    }
}
