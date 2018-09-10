using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpfcm1.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogLoginView.xaml
    /// </summary>
    public partial class DialogLoginView : MetroWindow
    {
        public DialogLoginView()
        {
            InitializeComponent();
        }

        //HACK: following two event handlers deliberately break MVVM pattern,
        // since PasswordBox does not support WPF data binding.
        private void DialogLoginView_OnLoaded(object sender, RoutedEventArgs e)
        {
            //passwdBox.Password = ((DialogSettingsViewModel)DataContext).User.FtpPassword;
            passwdBox.Password = ((dynamic)DataContext).LoginTemp.Password;
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ((dynamic)DataContext).LoginTemp.Password = passwdBox.Password;
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
