using Caliburn.Micro;
using wpfcm1.Shell;

namespace wpfcm1.FolderGroups
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
