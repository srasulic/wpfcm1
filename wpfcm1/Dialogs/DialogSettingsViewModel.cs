using System.Collections.Generic;
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

        protected override void OnActivate()
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

        private string _appTitle;
        public string AppTitle
        {
            get { return _appTitle; }
            set { _appTitle = value; NotifyOfPropertyChange(() => AppTitle); }
        }

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

        private List<string> _archivePolicies = new List<string> { "BASIC", "GROUP_RENAME" };
        public List<string> ArchivePolicies
        {
            get { return _archivePolicies; }
            set { _archivePolicies = value; NotifyOfPropertyChange(() => ArchivePolicies); }
        }

        private string _archivePolicy;
        public string ArchivePolicy
        {
            get { return _archivePolicy; }
            set { _archivePolicy = value; NotifyOfPropertyChange(() => ArchivePolicy); }
        }

        private string _variation;
        public string Variation
        {
            get { return _variation; }
            set { _variation = value; NotifyOfPropertyChange(() => Variation); }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; NotifyOfPropertyChange(() => UserName); }
        }

        private string _pib;
        public string PIB
        {
            get { return _pib; }
            set { _pib = value; NotifyOfPropertyChange(() => PIB); }
        }

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

        private string _apiUrl;
        public string ApiURL
        {
            get { return _apiUrl; }
            set { _apiUrl = value; NotifyOfPropertyChange(() => ApiURL); }
        }

        private string _ftpServer;
        public string FtpServer
        {
            get { return _ftpServer; }
            set { _ftpServer = value; NotifyOfPropertyChange(() => FtpServer); }
        }

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
            set { _ySigShift = value; NotifyOfPropertyChange(() => YSigShift); }
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
            Folders.Default.RootFolder = RootFolder;
            Folders.Default.ArchiveFolder = ArchiveFolder;
            User.Default.ArchivePolicy = ArchivePolicy;

            User.Default.Save();
            Folders.Default.Save();
        }
    }
}
