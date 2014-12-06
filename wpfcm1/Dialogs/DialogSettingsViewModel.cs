using Caliburn.Micro;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class DialogSettingsViewModel : Screen
    {
        public DialogSettingsViewModel()
        {
            DisplayName = "";
        }

        public string RootFolder
        {
            get { return Folders.Default.RootFolder; }
            set { Folders.Default.RootFolder = value; }
        }

        public string Name
        {
            get { return User.Default.Name; }
            set { User.Default.Name = value; }
        }

        public string PIB
        {
            get { return User.Default.PIB; }
            set { User.Default.PIB = value; }
        }

        public string FtpServer
        {
            get { return User.Default.FtpServer; }
            set { User.Default.FtpServer = value; }
        }

        public string FtpUserName
        {
            get { return User.Default.FtpUserName; }
            set { User.Default.FtpUserName = value; }
        }

        public string FtpPassword
        {
            get { return User.Default.FtpPassword; }
            set { User.Default.FtpPassword = value; }
        }
        public string TimestampServer
        {
            get { return User.Default.TimestampServer; }
            set { User.Default.TimestampServer = value; }
        }


        public void OnClose()
        {
            User.Default.Save();
            Folders.Default.Save();
            TryClose(true);
        }
    }
}
