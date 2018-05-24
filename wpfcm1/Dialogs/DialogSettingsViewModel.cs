using System.ComponentModel;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using wpfcm1.DataAccess;
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
        private string _token;
        private float _llxPib, _llyPib, _urxPib, _uryPib;
        private float _llxNo, _llyNo, _urxNo, _uryNo;
        private bool _invoicesInbound, _invoicesOutbound, _iosInbound, _iosOutbound, _kpInbound, _kpOutbound, _otherInbound, _otherOutbound;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserModel()
        {
            Folders folders = Folders.Default;
            User user = User.Default;

            RootFolder = folders.RootFolder;
            UserName = user.UserName;
            PIB = user.PIB;
            FtpServer = user.FtpServer != "" ? user.FtpServer : @"ftp://ftp.aserta.rs/";
            FtpUserName = user.FtpUserName;
            FtpPassword = user.FtpPassword;
            TimestampServer = user.TimestampServer != "" ? user.TimestampServer : @"http://test-tsa.ca.posta.rs/timestamp1";
            TimestampUserName = user.TimestampUserName != "" ? user.TimestampUserName : @"Test.Korisnik";
            TimestampPassword = user.TimestampPassword != "" ? user.TimestampPassword : @"123456"; 
            LlxPib = user.LlxPib;
            LlyPib = user.LlyPib;
            UrxPib = user.UrxPib;
            UryPib = user.UryPib;
            LlxNo = user.LlxNo;
            LlyNo = user.LlyNo;
            UrxNo = user.UrxNo;
            UryNo = user.UryNo;
            Token = user.Token;
            InvoicesInbound = user.InvoicesInbound;
            InvoicesOutbound = user.InvoicesOutbound;
            IosInbound = user.IosInbound;
            IosOutbound = user.IosOutbound;
            KpInbound = user.KpInbound;
            KpOutbound = user.KpOutbound;
            OtherInbound = user.OtherInbound;
            OtherOutbound = user.OtherOutbound;
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (value == _rootFolder) return;
                _rootFolder = value;
                Log.Info("Change in settings - RootFolder");
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
                Log.Info("Change in settings - UserName");
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
                Log.Info("Change in settings - PIB");
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
                Log.Info("Change in settings - FtpServer");
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
                Log.Info("Change in settings - FtpUserName");
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
                Log.Info("Change in settings - FtpPassword");
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
                Log.Info("Change in settings - TimestampServer");
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
                Log.Info("Change in settings - TimestampUserName");
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
                Log.Info("Change in settings - TimestampPassword");
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

        public string Token
        {
            get { return _token; }
            set { _token = value; NotifyOfPropertyChange(() => Token); }
        }

        //    private bool , _InvoicesOutbound, _IosInbound, _IosOutbound, _KpInbound, _KpOutbound, _OtherInbound, _OtherOutbound;
        public bool InvoicesInbound
        {
            get { return _invoicesInbound; }
            set { _invoicesInbound = value; NotifyOfPropertyChange(() => _invoicesInbound); Log.Info("Change in settings - InvoicesInbound"); }
        }

        public bool InvoicesOutbound
        {
            get { return _invoicesOutbound; }
            set { _invoicesOutbound = value; NotifyOfPropertyChange(() => _invoicesOutbound); Log.Info("Change in settings - InvoicesOutbound"); }
        }

        public bool IosInbound
        {
            get { return _iosInbound; }
            set { _iosInbound = value; NotifyOfPropertyChange(() => _iosInbound); Log.Info("Change in settings - IosInbound"); }
        }

        public bool IosOutbound
        {
            get { return _iosOutbound; }
            set { _iosOutbound = value; NotifyOfPropertyChange(() => _iosOutbound); Log.Info("Change in settings - IosOutbound"); }
        }

        public bool KpInbound
        {
            get { return _kpInbound; }
            set { _kpInbound = value; NotifyOfPropertyChange(() => _kpInbound); Log.Info("Change in settings - KpInbound"); }
        }

        public bool KpOutbound
        {
            get { return _kpOutbound; }
            set { _kpOutbound = value; NotifyOfPropertyChange(() => _kpOutbound); Log.Info("Change in settings - KpOutbound"); }
        }

        public bool OtherInbound
        {
            get { return _otherInbound; }
            set { _otherInbound = value; NotifyOfPropertyChange(() => _otherInbound); Log.Info("Change in settings - OtherInbound"); }
        }

        public bool OtherOutbound
        {
            get { return _otherOutbound; }
            set { _otherOutbound = value; NotifyOfPropertyChange(() => _otherOutbound); Log.Info("Change in settings - OtherOutbound"); }
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
            User.Default.Token = UserTemp.Token;
            User.Default.InvoicesInbound = UserTemp.InvoicesInbound;
            User.Default.InvoicesOutbound = UserTemp.InvoicesOutbound;
            User.Default.IosInbound = UserTemp.IosInbound;
            User.Default.IosOutbound = UserTemp.IosOutbound;
            User.Default.KpInbound = UserTemp.KpInbound;
            User.Default.KpOutbound = UserTemp.KpOutbound;
            User.Default.OtherInbound = UserTemp.OtherInbound;
            User.Default.OtherOutbound = UserTemp.OtherOutbound;

            //encrypted FTP Password
            User.Default.FtpPassword = EncryptionHelper.Encrypt(User.Default.FtpPassword);

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
