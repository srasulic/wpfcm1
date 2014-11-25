using Caliburn.Micro;
using System.Windows;
using wpfcm1.ViewModels;

namespace wpfcm1
{
    public class AppBootstrapper : BootstrapperBase 
    {
        public AppBootstrapper()
        {
            Initialize();
        }
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
