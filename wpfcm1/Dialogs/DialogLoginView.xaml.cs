using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace wpfcm1.Dialogs
{
    public partial class DialogLoginView : MetroWindow
    {
        public DialogLoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic)DataContext).Password = (sender as PasswordBox)?.Password;
        }

        private void DialogLoginView_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserName.Text))
            {
                Keyboard.Focus(UserName);
            }
            else
            {
                Keyboard.Focus(Password);
            }
        }
    }
}
