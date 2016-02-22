using System.ComponentModel;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class UserModel : PropertyChangedBase, IDataErrorInfo
    {
        private string _rootFolder;
        private string _userName;
        private string _pib;
        private string _ftpServer;
        private string _ftpUserName;
        private string _ftpPassword;
        private string _timestampServer;
        private string _timestampUserName;
        private string _timestampPassword;
        private float _llxPib, _llyPib, _urxPib, _uryPib;
        private float _llxNo, _llyNo, _urxNo, _uryNo;

        public UserModel()
        {
            Folders folders = Folders.Default;
            User user = User.Default;

            RootFolder = folders.RootFolder;
            UserName = user.UserName;
            PIB = user.PIB;
            FtpServer = user.FtpServer;
            FtpUserName = user.FtpUserName;
            FtpPassword = user.FtpPassword;
            TimestampServer = user.TimestampServer;
            TimestampUserName = user.TimestampUserName;
            TimestampPassword = user.TimestampPassword;
            LlxPib = user.LlxPib;
            LlyPib = user.LlyPib;
            UrxPib = user.UrxPib;
            UryPib = user.UryPib;
            LlxNo = user.LlxNo;
            LlyNo = user.LlyNo;
            UrxNo = user.UrxNo;
            UryNo = user.UryNo;
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (value == _rootFolder) return;
                _rootFolder = value;
                NotifyOfPropertyChange(() => RootFolder);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string PIB
        {
            get { return _pib; }
            set
            {
                if (value == _pib) return;
                _pib = value;
                NotifyOfPropertyChange(() => PIB);
            }
        }

        public string FtpServer
        {
            get { return _ftpServer; }
            set
            {
                if (value == _ftpServer) return;
                _ftpServer = value;
                NotifyOfPropertyChange(() => FtpServer);
            }
        }

        public string FtpUserName
        {
            get { return _ftpUserName; }
            set
            {
                if (value == _ftpUserName) return;
                _ftpUserName = value;
                NotifyOfPropertyChange(() => FtpUserName);
            }
        }

        public string FtpPassword
        {
            get { return _ftpPassword; }
            set
            {
                if (value == _ftpPassword) return;
                _ftpPassword = value;
                NotifyOfPropertyChange(() => FtpPassword);
            }
        }

        public string TimestampServer
        {
            get { return _timestampServer; }
            set
            {
                if (value == _timestampServer) return;
                _timestampServer = value;
                NotifyOfPropertyChange(() => TimestampServer);
            }
        }

        public string TimestampUserName
        {
            get { return _timestampUserName; }
            set
            {
                if (value == _timestampUserName) return;
                _timestampUserName = value;
                NotifyOfPropertyChange(() => TimestampUserName);
            }
        }

        public string TimestampPassword
        {
            get { return _timestampPassword; }
            set
            {
                if (value == _timestampPassword) return;
                _timestampPassword = value;
                NotifyOfPropertyChange(() => TimestampPassword);
            }
        }

        public float LlxPib
        {
            get { return _llxPib; }
            set { _llxPib = value; NotifyOfPropertyChange(() => LlxPib); }
        }

        public float LlyPib
        {
            get { return _llyPib; }
            set { _llyPib = value; NotifyOfPropertyChange(() => LlyPib); }
        }

        public float UrxPib
        {
            get { return _urxPib; }
            set { _urxPib = value; NotifyOfPropertyChange(() => UrxPib); }
        }

        public float UryPib
        {
            get { return _uryPib; }
            set { _uryPib = value; NotifyOfPropertyChange(() => UryPib); }
        }

        public float LlxNo
        {
            get { return _llxNo; }
            set { _llxNo = value; NotifyOfPropertyChange(() => LlxNo); }
        }

        public float LlyNo
        {
            get { return _llyNo; }
            set { _llyNo = value; NotifyOfPropertyChange(() => LlyNo); }
        }

        public float UrxNo
        {
            get { return _urxNo; }
            set { _urxNo = value; NotifyOfPropertyChange(() => UrxNo); }
        }

        public float UryNo
        {
            get { return _uryNo; }
            set { _uryNo = value; NotifyOfPropertyChange(() => UryNo); }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "PIB":
                        if (string.IsNullOrWhiteSpace(PIB))
                            return "Pib must have 9 numbers";
                        var regexPib = new Regex(@"\b\d{9}\b");
                        if (!regexPib.IsMatch(PIB))
                        {
                            return "Pib must have 9 numbers";
                        }
                        break;
                    default:
                        return null;
                }
                return null;
            }
        }

        public string Error { get; set; }
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
            User.Default.UserName = UserTemp.UserName;
            User.Default.PIB = UserTemp.PIB;
            User.Default.FtpServer = UserTemp.FtpServer;
            User.Default.FtpUserName = UserTemp.FtpUserName;
            User.Default.FtpPassword = UserTemp.FtpPassword;
            User.Default.TimestampServer = UserTemp.TimestampServer;
            User.Default.TimestampUserName = UserTemp.TimestampUserName;
            User.Default.TimestampPassword = UserTemp.TimestampPassword;
            User.Default.LlxPib = UserTemp.LlxPib;
            User.Default.LlyPib = UserTemp.LlyPib;
            User.Default.UrxPib = UserTemp.UrxPib;
            User.Default.UryPib = UserTemp.UryPib;
            User.Default.LlxNo = UserTemp.LlxNo;
            User.Default.LlyNo = UserTemp.LlyNo;
            User.Default.UrxNo = UserTemp.UrxNo;
            User.Default.UryNo = UserTemp.UryNo;

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
