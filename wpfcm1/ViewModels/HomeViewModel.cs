using Caliburn.Micro;

namespace wpfcm1.ViewModels
{
    public class HomeViewModel : Screen
    {
        public HomeViewModel(ShellViewModel svm)
        {
            Svm = svm;
        }

        public ShellViewModel Svm { get; private set; }
    }
}
