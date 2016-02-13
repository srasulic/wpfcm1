using System.Windows;
using MahApps.Metro.Controls;

namespace wpfcm1.Dialogs
{
    public partial class DialogSettingsView : MetroWindow
    {
        public DialogSettingsView()
        {
            InitializeComponent();
        }

        //HACK: following two event handlers deliberately break MVVM pattern,
        // since PasswordBox does not support WPF data binding.
        private void DialogSettingsView_OnLoaded(object sender, RoutedEventArgs e)
        {
            //passwdBox.Password = ((DialogSettingsViewModel)DataContext).User.FtpPassword;
            passwdBox.Password = ((dynamic)DataContext).UserTemp.FtpPassword;
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic)DataContext).UserTemp.FtpPassword = passwdBox.Password;
        }
    }
}
