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
        private float _xSigShift, _ySigShift, _xSigShiftRight, _ySigShiftRight;
        private float _llxPib, _llyPib, _urxPib, _uryPib;
        private float _llxNo, _llyNo, _urxNo, _uryNo;
        private bool _invoicesInbound, _invoicesOutbound, _iosInbound, _iosOutbound, _kpInbound, _kpOutbound, _povratiInbound, _povratiOutbound, _otherInbound, _otherOutbound;

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
            InvoicesInbound = user.InvoicesInbound;
            InvoicesOutbound = user.InvoicesOutbound;
            IosInbound = user.IosInbound;
            IosOutbound = user.IosOutbound;
            KpInbound = user.KpInbound;
            KpOutbound = user.KpOutbound;
            PovratiInbound = user.PovratiInbound;
            PovratiOutbound = user.PovratiOutbound;
            OtherInbound = user.OtherInbound;
            OtherOutbound = user.OtherOutbound;
            XSigShift = user.XSigShift;
            YSigShift = user.YSigShift;
            XSigShiftRight = user.XSigShiftRight;
            YSigShiftRight = user.YSigShiftRight;

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

        //    private bool , _InvoicesOutbound, _IosInbound, _IosOutbound, _KpInbound, _KpOutbound, _OtherInbound, _OtherOutbound;
        public bool InvoicesInbound
        {
            get { return _invoicesInbound; }
            set { _invoicesInbound = value; NotifyOfPropertyChange(() => _invoicesInbound); }
        }

        public bool InvoicesOutbound
        {
            get { return _invoicesOutbound; }
            set { _invoicesOutbound = value; NotifyOfPropertyChange(() => _invoicesOutbound); }
        }

        public bool IosInbound
        {
            get { return _iosInbound; }
            set { _iosInbound = value; NotifyOfPropertyChange(() => _iosInbound); }
        }

        public bool IosOutbound
        {
            get { return _iosOutbound; }
            set { _iosOutbound = value; NotifyOfPropertyChange(() => _iosOutbound); }
        }

        public bool KpInbound
        {
            get { return _kpInbound; }
            set { _kpInbound = value; NotifyOfPropertyChange(() => _kpInbound); }
        }

        public bool KpOutbound
        {
            get { return _kpOutbound; }
            set { _kpOutbound = value; NotifyOfPropertyChange(() => _kpOutbound); }
        }


        public bool PovratiInbound
        {
            get { return _povratiInbound; }
            set { _povratiInbound = value; NotifyOfPropertyChange(() => _povratiInbound); }
        }

        public bool PovratiOutbound
        {
            get { return _povratiOutbound; }
            set { _povratiOutbound = value; NotifyOfPropertyChange(() => _povratiOutbound); }
        }


        public bool OtherInbound
        {
            get { return _otherInbound; }
            set { _otherInbound = value; NotifyOfPropertyChange(() => _otherInbound); }
        }

        public bool OtherOutbound
        {
            get { return _otherOutbound; }
            set { _otherOutbound = value; NotifyOfPropertyChange(() => _otherOutbound); }
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

        public float XSigShift
        {
            get
            {
                if (_xSigShift < 0) return 0;
                return _xSigShift < 190 ? _xSigShift : 190 ;
            }
            set { _xSigShift = value; NotifyOfPropertyChange(() => XSigShift); }
        }

        public float YSigShift
        {
            get
            {
                if (_ySigShift < 0) return 0;
                return _ySigShift < 190 ? _ySigShift : 190;
            }
            set { _ySigShift = value ; NotifyOfPropertyChange(() => YSigShift); }
        }

        public float XSigShiftRight
        {
            get
            {
                if (_xSigShiftRight < 0) return 0;
                return _xSigShiftRight < 190 ? _xSigShiftRight : 190;
            }
            set { _xSigShiftRight = value; NotifyOfPropertyChange(() => XSigShiftRight); }
        }

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
            User.Default.InvoicesInbound = UserTemp.InvoicesInbound;
            User.Default.InvoicesOutbound = UserTemp.InvoicesOutbound;
            User.Default.IosInbound = UserTemp.IosInbound;
            User.Default.IosOutbound = UserTemp.IosOutbound;
            User.Default.KpInbound = UserTemp.KpInbound;
            User.Default.KpOutbound = UserTemp.KpOutbound;
            User.Default.PovratiInbound = UserTemp.PovratiInbound;
            User.Default.PovratiOutbound = UserTemp.PovratiOutbound;
            User.Default.OtherInbound = UserTemp.OtherInbound;
            User.Default.OtherOutbound = UserTemp.OtherOutbound;
            User.Default.XSigShift = UserTemp.XSigShift;
            User.Default.YSigShift = UserTemp.YSigShift;
            User.Default.XSigShiftRight = UserTemp.XSigShiftRight;
            User.Default.YSigShiftRight = UserTemp.YSigShiftRight;

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
