using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Caliburn.Micro;
using wpfcm1.DataAccess;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class UserModel : PropertyChangedBase, IDataErrorInfo
    {
        private bool _initInProgress;
        private string _rootFolder;
        private string _archiveFolder;
        private string _archivePolicy;
        private string _userName;
        private string _pib;
        private string _ftpServer;
        private string _ftpUserName;
        private string _ftpPassword;
        private string _timestampServer;
        private string _timestampUserName;
        private string _timestampPassword;
        private string _token;
        private string _apiURL;
        private string _variation;
        private float _xSigShift, _ySigShift, _xSigShiftRight, _ySigShiftRight;
        private float _llxPib, _llyPib, _urxPib, _uryPib;
        private float _llxNo, _llyNo, _urxNo, _uryNo;
        private bool _invoicesInbound, _invoicesOutbound, _iosInbound, _iosOutbound, _otpadInbound, _otpadOutbound, _otpremnicaInbound, _otpremnicaOutbound, _kpInbound, _kpOutbound, _povratiInbound, _povratiOutbound, _otherInbound, _otherOutbound;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _appTitle;
        private bool _mandatoryLogin;

        public UserModel()
        {
            InitInProgress = true;

            Folders folders = Folders.Default;
            User user = User.Default;

            RootFolder = folders.RootFolder;
            ArchiveFolder = folders.ArchiveFolder;
            UserName = user.UserName;
            PIB = user.PIB;
     //       FtpServer = user.FtpServer != "" ? user.FtpServer : @"ftp://ftp.polisign.net/";
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
            OtpadInbound = user.OtpadInbound;
            OtpadOutbound = user.OtpadOutbound;
            OtpremnicaInbound = user.OtpremnicaInbound;
            OtpremnicaOutbound = user.OtpremnicaOutbound;
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
            Variation = user.Variation != "" ? user.Variation : @"RS";
            ArchivePolicy = user.ArchivePolicy != "" ? user.ArchivePolicy : @"BASIC";
            MandatoryLogin = user.MandatoryLogin ;

            //            ApiURL = user.ApiURL;
            //          AppTitle = user.AppTitle;

            InitInProgress = false;

            /* boja hedera je definisana prema Variation u App.xaml.cs */
            /* Naziv u zaglavlju je definisan prema Variation u AppBootstrapper.cs */

        }

        private bool InitInProgress
        {
            get
            {
                return _initInProgress;
            }
            set
            {
                _initInProgress = value;
            }
        }

        public string AppTitle
        {
            get
            {
                return _appTitle;
            }
            set
            {
                if (value == _appTitle) return;
                LogChanges("_appTitle", _appTitle, value);
                _appTitle = value;
                NotifyOfPropertyChange(() => AppTitle);
            }
        }

        public bool MandatoryLogin
        {
            get
            {
                return _mandatoryLogin;
            }
            set
            {
                if (value == _mandatoryLogin) return;
                LogChanges("_mandatoryLogin", _mandatoryLogin.ToString(), value.ToString());
                _mandatoryLogin = value;
                NotifyOfPropertyChange(() => MandatoryLogin);
            }
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (value == _rootFolder) return;
                LogChanges("_rootFolder", _rootFolder, value);
                _rootFolder = value;
                NotifyOfPropertyChange(() => RootFolder);
            }
        }

        public string ArchiveFolder
        {
            get { return _archiveFolder; }
            set
            {
                if (value == _archiveFolder) return;
                LogChanges("_archiveFolder", _archiveFolder, value);
                _archiveFolder = value;
                NotifyOfPropertyChange(() => ArchiveFolder);
            }
        }

        public string ArchivePolicy
        {
            get { return _archivePolicy; }
            set
            {
                if (value == _archivePolicy) return;
                LogChanges("_archivePolicy", _archivePolicy, value);
                _archivePolicy = value;
                NotifyOfPropertyChange(() => ArchivePolicy);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                LogChanges("_userName", _userName, value);
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
                LogChanges("_pib", _pib, value);
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
                LogChanges("_ftpServer", _ftpServer, value);
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
                LogChanges("_ftpUserName", _ftpUserName, value);
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
                LogChanges("_ftpPassword", _ftpPassword, value);
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
                LogChanges("_timestampServer", _timestampServer, value);
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
                LogChanges("_timestampUserName", _timestampUserName, value);
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
                LogChanges("_timestampPassword", _timestampPassword, value);
                _timestampPassword = value;
                NotifyOfPropertyChange(() => TimestampPassword);
            }
        }


        public string ApiURL
        {
            get {
                return _apiURL; }
            set
            {
                if (value == _apiURL) return;
                LogChanges("_apiURL", _apiURL, value);
                _apiURL = value;
                NotifyOfPropertyChange(() => ApiURL);
            }
        }

        public string Variation
        {
            get { return _variation; }
            set
            {
                if (value == _variation) return;
                LogChanges("_variation", _variation, value);
                _variation = value;

                if (_variation == "RS")
                {
                    FtpServer = @"ftp://ftp.aserta.rs/";
                    AppTitle = @"https://edokument.rs";
                    ApiURL = @"https://edokument.rs";
                    MandatoryLogin = true;
                }
                if (_variation == "RS-LOGIN")
                {
                    FtpServer = @"ftp://ftp.aserta.rs/";
                    AppTitle = @"https://edokument.rs";
                    ApiURL = @"https://edokument.rs";
                    MandatoryLogin = true;
                }
                if (_variation == "RS-IZVRSITELJ")
                {
                    FtpServer = @"ftp://ftp.aserta.rs/";
                    AppTitle = @"https://edokument.rs";
                    ApiURL = @"https://edokument.rs";
                    MandatoryLogin = true;
                }
                else if (_variation == "BIH") {
                    FtpServer = @"ftp://ftp.polisign.net/";
                    AppTitle = @"https://polisign.net";
                    ApiURL = @"https://polisign.net";
                    MandatoryLogin = false;
                }
                else if (_variation == "RSDEV") {
                    FtpServer = @"ftp://edokument.dev.aserta.rs/";
                    AppTitle = @"https://edokument.dev.aserta.rs";
                    ApiURL = @"https://edokument.dev.aserta.rs";
                    MandatoryLogin = false;
                }
                else if (_variation == "BIHDEV") {
                    FtpServer = @"ftp://116.203.101.59/";
                    AppTitle = @"https://116.203.101.59";
                    ApiURL = @"https://116.203.101.59";
                    MandatoryLogin = false;
                }

                NotifyOfPropertyChange(() => Variation);
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
            set {
                if (_invoicesInbound == value) return;
                LogChanges("_invoicesInbound", _invoicesInbound.ToString(), value.ToString());
                _invoicesInbound = value;
                NotifyOfPropertyChange(() => _invoicesInbound);
                }
        }

        public bool InvoicesOutbound
        {
            get { return _invoicesOutbound; }
            set
            {
                if (_invoicesOutbound == value) return;
                LogChanges("_invoicesOutbound", _invoicesOutbound.ToString(), value.ToString());
                _invoicesOutbound = value;
                NotifyOfPropertyChange(() => _invoicesOutbound);
            }
        }

        public bool IosInbound
        {
            get { return _iosInbound; }
            set
            {
                if (_iosInbound == value) return;
                LogChanges("_iosInbound", _iosInbound.ToString(), value.ToString());
                _iosInbound = value;
                NotifyOfPropertyChange(() => _iosInbound);
            }
        }


        public bool IosOutbound
        {
            get { return _iosOutbound; }
            set {
                if (_iosOutbound == value) return;
                LogChanges("_iosOutbound", _iosOutbound.ToString(), value.ToString());
                _iosOutbound = value;
                NotifyOfPropertyChange(() => _iosOutbound);
            }
        }

        public bool OtpadInbound
        {
            get { return _otpadInbound; }
            set
            {
                if (_otpadInbound == value) return;
                LogChanges("_otpadInbound", _otpadInbound.ToString(), value.ToString());
                _otpadInbound = value;
                NotifyOfPropertyChange(() => _otpadInbound);
            }
        }

        public bool OtpremnicaInbound
        {
            get { return _otpremnicaInbound; }
            set
            {
                if (_otpremnicaInbound == value) return;
                LogChanges("_otpadInbound", _otpremnicaInbound.ToString(), value.ToString());
                _otpremnicaInbound = value;
                NotifyOfPropertyChange(() => _otpremnicaInbound);
            }
        }

        public bool OtpadOutbound
        {
            get { return _otpadOutbound; }
            set
            {
                if (_otpadOutbound == value) return;
                LogChanges("_otpadOutbound", _otpadOutbound.ToString(), value.ToString());
                _otpadOutbound = value;
                NotifyOfPropertyChange(() => _otpadOutbound);
            }
        }


        public bool OtpremnicaOutbound
        {
            get { return _otpremnicaOutbound; }
            set
            {
                if (_otpremnicaOutbound == value) return;
                LogChanges("_otpadOutbound", _otpremnicaOutbound.ToString(), value.ToString());
                _otpremnicaOutbound = value;
                NotifyOfPropertyChange(() => _otpremnicaOutbound);
            }
        }

        public bool KpInbound
        {
            get { return _kpInbound; }
            set {
                if (_kpInbound == value) return;
                LogChanges("_kpInbound", _kpInbound.ToString(), value.ToString());
                _kpInbound = value;
                NotifyOfPropertyChange(() => _kpInbound);
            }
        }

        public bool KpOutbound
        {
            get { return _kpOutbound; }
            set
            {
                if (_kpOutbound == value) return;
                LogChanges("_kpOutbound", _kpOutbound.ToString(), value.ToString());
                _kpOutbound = value;
                NotifyOfPropertyChange(() => _kpOutbound);
            }
        }


        public bool PovratiInbound
        {
            get { return _povratiInbound; }
            set {
                if (_povratiInbound == value) return;
                LogChanges("_povratiInbound", _povratiInbound.ToString(), value.ToString());
                _povratiInbound = value;
                NotifyOfPropertyChange(() => _povratiInbound); }
        }

        public bool PovratiOutbound
        {
            get { return _povratiOutbound; }
            set {
                if (_povratiOutbound == value) return;
                LogChanges("_povratiOutbound", _povratiOutbound.ToString(), value.ToString());
                _povratiOutbound = value;
                NotifyOfPropertyChange(() => _povratiOutbound); }
        }


        public bool OtherInbound
        {
            get { return _otherInbound; }
            set {
                if (_otherInbound == value) return;
                LogChanges("_otherInbound", _otherInbound.ToString(), value.ToString());
                _otherInbound = value;
                NotifyOfPropertyChange(() => _otherInbound);
                Log.Info("Change in settings - OtherInbound");
            }
        }

        public bool OtherOutbound
        {
            get { return _otherOutbound; }
            set {
                if (_otherOutbound == value) return;
                LogChanges("_otherOutbound", _otherOutbound.ToString(), value.ToString());
                _otherOutbound = value;
                NotifyOfPropertyChange(() => _otherOutbound);
                Log.Info("Change in settings - OtherOutbound");
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "PIB":
                        if (string.IsNullOrWhiteSpace(PIB))
                            return "Validna vrednost je 9 ili 13 cifara";
                        // var regexPib = new Regex(@"\b\d{9}\b");
                        // var regexJib = new Regex(@"\b\d{13}\b");
                        
                        // if (!regexPib.IsMatch(PIB))
                        if (!FolderTypes.GeneratedFolderViewModel.IsPibOk(PIB, false))
                        {
                            return "Unesite validan pib / jib";
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
//                return _xSigShift < 190 ? _xSigShift : 190;
                return _xSigShift < 590 ? _xSigShift : 590;
            }
            set { _xSigShift = value; NotifyOfPropertyChange(() => XSigShift); }
        }

        public float YSigShift
        {
            get
            {
                if (_ySigShift < 0) return 0;
//                return _ySigShift < 190 ? _ySigShift : 190;
                return _ySigShift < 840 ? _ySigShift : 840;
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

        private void LogChanges(string fieldDisplayName, string oldValue, string newValue)
        {
            if (InitInProgress) Log.Info("Settings value - " + fieldDisplayName + ":" + newValue);
            else Log.Info("Change in settings - " + fieldDisplayName + " OLD:" + oldValue + " NEW:" + newValue);

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
            User.Default.OtpadInbound = UserTemp.OtpadInbound;
            User.Default.OtpadOutbound = UserTemp.OtpadOutbound;
            User.Default.OtpremnicaInbound = UserTemp.OtpremnicaInbound;
            User.Default.OtpremnicaOutbound = UserTemp.OtpremnicaOutbound;
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
            User.Default.ApiURL = UserTemp.ApiURL;
            User.Default.Variation = UserTemp.Variation;
            User.Default.AppTitle = UserTemp.AppTitle;
            User.Default.ArchivePolicy = UserTemp.ArchivePolicy;
            User.Default.MandatoryLogin = UserTemp.MandatoryLogin;

            //encrypted FTP Password
            //User.Default.FtpPassword = EncryptionHelper.Encrypt(User.Default.FtpPassword);

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
