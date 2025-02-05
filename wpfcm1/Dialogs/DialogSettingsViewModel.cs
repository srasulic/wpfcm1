using System.ComponentModel;
using Caliburn.Micro;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class UserModel : PropertyChangedBase, IDataErrorInfo
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserModel()
        {
            Folders folders = Folders.Default;
            User user = User.Default;

            RootFolder = folders.RootFolder;
            ArchiveFolder = folders.ArchiveFolder;
            ArchivePolicy = user.ArchivePolicy != "" ? user.ArchivePolicy : @"BASIC";

            UserName = user.UserName;
            PIB = user.PIB;
            ApiURL = user.ApiURL;
            AppTitle = user.AppTitle;
            Variation = user.Variation;
            FtpServer = user.FtpServer;
            //Token = user.Token;
            TimestampServer = user.TimestampServer;
            TimestampUserName = user.TimestampUserName;
            TimestampPassword = user.TimestampPassword;
            XSigShift = user.XSigShift;
            YSigShift = user.YSigShift;
            XSigShiftRight = user.XSigShiftRight;
            YSigShiftRight = user.YSigShiftRight;

            /* boja hedera je definisana prema Variation u App.xaml.cs */
            /* Naziv u zaglavlju je definisan prema Variation u AppBootstrapper.cs */
        }

        public string AppTitle { get; set; }

        private string _rootFolder;
        public string RootFolder
        {
            get { return _rootFolder; }
            set { _rootFolder = value; NotifyOfPropertyChange(() => RootFolder); }
        }

        private string _archiveFolder;
        public string ArchiveFolder
        {
            get { return _archiveFolder; }
            set { _archiveFolder = value; NotifyOfPropertyChange(() => ArchiveFolder); }
        }

        private string _archivePolicy;
        public string ArchivePolicy
        {
            get { return _archivePolicy; }
            set { _archivePolicy = value; NotifyOfPropertyChange(() => ArchivePolicy); }
        }

        public string Variation { get; set; }

        public string UserName { get; set; }

        public string PIB { get; set; }

        public string Token { get; set; }

        private string _timestampServer;
        public string TimestampServer
        {
            get { return _timestampServer; }
            set { _timestampServer = value; NotifyOfPropertyChange(() => TimestampServer); }
        }

        private string _timestampUserName;
        public string TimestampUserName
        {
            get { return _timestampUserName; }
            set { _timestampUserName = value; NotifyOfPropertyChange(() => TimestampUserName); }
        }

        private string _timestampPassword;
        public string TimestampPassword
        {
            get { return _timestampPassword; }
            set { _timestampPassword = value; NotifyOfPropertyChange(() => TimestampPassword); }
        }

        public string ApiURL { get; set; }

        public string FtpServer { get; set; }

        private float _xSigShift;
        public float XSigShift
        {
            get
            {
                if (_xSigShift < 0) return 0;
                return _xSigShift < 590 ? _xSigShift : 590;
            }
            set { _xSigShift = value; NotifyOfPropertyChange(() => XSigShift); }
        }

        private float _ySigShift;
        public float YSigShift
        {
            get
            {
                if (_ySigShift < 0) return 0;
                return _ySigShift < 840 ? _ySigShift : 840;
            }
            set { _ySigShift = value ; NotifyOfPropertyChange(() => YSigShift); }
        }

        private float _xSigShiftRight;
        public float XSigShiftRight
        {
            get
            {
                if (_xSigShiftRight < 0) return 0;
                return _xSigShiftRight < 190 ? _xSigShiftRight : 190;
            }
            set { _xSigShiftRight = value; NotifyOfPropertyChange(() => XSigShiftRight); }
        }

        private float _ySigShiftRight;
        public float YSigShiftRight
        {
            get
            {
                if (_ySigShiftRight < 0) return 0;
                return _ySigShiftRight < 190 ? _ySigShiftRight : 190;
            }
            set { _ySigShiftRight = value; NotifyOfPropertyChange(() => YSigShiftRight); }
        }

        public string Error { get; set; }

        public string this[string columnName]
        { 
            get
            {
                return null;
            }
        }
    }

    public class DialogSettingsViewModel : Screen
    {
        public UserModel UserTemp { get; set; }

        public DialogSettingsViewModel()
        {
            DisplayName = "";
            UserTemp = new UserModel();
        }

        public void OnClose()
        {
            SaveUser();
            TryClose(true);
        }

        public void OnCancel()
        {
            TryClose(true);
        }

        private void SaveUser()
        {
            Folders.Default.RootFolder = UserTemp.RootFolder;
            Folders.Default.ArchiveFolder = UserTemp.ArchiveFolder;
            User.Default.ArchivePolicy = UserTemp.ArchivePolicy;

            //User.Default.UserName = UserTemp.UserName;
            //User.Default.PIB = UserTemp.PIB;
            //User.Default.ApiURL = UserTemp.ApiURL;
            //User.Default.AppTitle = UserTemp.AppTitle;
            //User.Default.Variation = UserTemp.Variation;
            //User.Default.FtpServer = UserTemp.FtpServer;
            //User.Default.Token = UserTemp.Token;
            User.Default.TimestampServer = UserTemp.TimestampServer;
            User.Default.TimestampUserName = UserTemp.TimestampUserName;
            User.Default.TimestampPassword = UserTemp.TimestampPassword;
            //User.Default.XSigShift = UserTemp.XSigShift;
            //User.Default.YSigShift = UserTemp.YSigShift;
            //User.Default.XSigShiftRight = UserTemp.XSigShiftRight;
            //User.Default.YSigShiftRight = UserTemp.YSigShiftRight;

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
