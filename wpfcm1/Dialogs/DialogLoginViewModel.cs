using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;
using wpfcm1.OlympusApi;

namespace wpfcm1.Dialogs
{
    public class Variation
    {
        public string Name { get; set; }
        public string FtpServer { get; set; }
        public string AppTitle { get; set; }
        public string ApiUrl { get; set; }
    }

    public class DialogLoginViewModel : Screen, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DialogLoginViewModel(IWindowManager windowManager)
        {
            DisplayName = "Polisign Login";
            SelectedVariation = Variations[0];
        }

        public ObservableCollection<Variation> Variations { get; } = new ObservableCollection<Variation>
        {
            new Variation { Name = "RS", FtpServer = @"ftp://ftp.aserta.rs/", AppTitle = @"https://edokument.rs", ApiUrl = @"https://edokument.rs"},
            new Variation { Name = "RS-LOGIN", FtpServer = @"ftp://ftp.aserta.rs/", AppTitle = @"https://edokument.rs", ApiUrl = @"https://edokument.rs"},
            new Variation { Name = "RS-IZVRSITELJ", FtpServer = @"ftp://ftp.aserta.rs/", AppTitle = @"https://edokument.rs", ApiUrl = @"https://edokument.rs"},
            new Variation { Name = "MTEL", FtpServer = @"", AppTitle = @"http://10.10.8.5", ApiUrl = @"http://10.10.8.5:8000"},
            new Variation { Name = "MTELDEV", FtpServer = @"", AppTitle = @"http://10.10.8.5", ApiUrl = @"http://10.10.8.5:8000"},
            new Variation { Name = "BIH", FtpServer = @"ftp://ftp.polisign.net/", AppTitle = @"https://polisign.net", ApiUrl = @"https://polisign.net"},
            new Variation { Name = "RSDEV", FtpServer = @"ftp://edokument.dev.aserta.rs/", AppTitle = @"https://edokument.dev.aserta.rs", ApiUrl = @"https://edokument.dev.aserta.rs"},
            new Variation { Name = "BIHDEV", FtpServer = @"ftp://116.203.101.59/", AppTitle = @"https://116.203.101.59", ApiUrl = @"https://116.203.101.59"}
        };

        private Variation _variation;
        public Variation SelectedVariation
        {
            get => _variation;
            set { _variation = value; NotifyOfPropertyChange(() => SelectedVariation); }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set { _userName = value; NotifyOfPropertyChange(() => UserName); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; NotifyOfPropertyChange(() => Password); }
        }

        private Token _token;
        public Token Token
        {
            get => _token;
            set
            {
                _token = value;
                NotifyOfPropertyChange(() => Token);
                NotifyOfPropertyChange(() => CanOnClose);
            }
        }

        private List<Tenant> _tenants;
        public List<Tenant> Tenants
        {
            get => _tenants;
            set { _tenants = value; NotifyOfPropertyChange(() => Tenants); }
        }

        private Tenant _tenant;
        public Tenant SelectedTenant
        {
            get => _tenant;
            set
            {
                _tenant = value;
                NotifyOfPropertyChange(() => SelectedTenant);
                NotifyOfPropertyChange(() => CanOnClose);
                OnSelectedTenant();
            }
        }

        private Profile _profile;
        public Profile SelectedProfile 
        {
            get => _profile;
            set
            {
                _profile = value;
                NotifyOfPropertyChange(() => SelectedProfile);
                NotifyOfPropertyChange(() => CanOnClose);
            }
        }

        public async void OnSelectedTenant()
        {
            var svc = new OlympusService(SelectedVariation.ApiUrl);
            
            ProfileResult res = await svc.GetConfigPolisign(Token, SelectedTenant);
            if (res == null)
            {
                Log.Error($"FAILED Obtaining Olympus Profile: [UserName = {UserName}] [Tenant = {SelectedTenant.tenant}]");
                return;
            }

            Log.Info($"Obtained Olympus Profile [UserName = {UserName}] [Tenant = {SelectedTenant.tenant}]");

            SelectedProfile = res.profile;
        }

        public async void OnLogin()
        {
            var svc = new OlympusService(SelectedVariation.ApiUrl);

            var token = await svc.PostUsersLogin(UserName, Password);
            if (token == null)
            {
                Log.Error($"FAILED LOGIN: [UserName = {UserName}]");
                return;
            }

            Log.Info($"SUCCESSFUL LOGIN: [UserName = {UserName}]");

            Token = token;

            var userTenants = await svc.GetUsersTenants(token);
            if (userTenants == null)
            {
                Log.Error($"FAILED Obtaining Olympus Tenants: [UserName = {UserName}]");
                return;
            }

            Log.Info($"Obtained Olympus Tenants: [UserName = {UserName}]");

            Tenants = userTenants.tenants;
            SelectedTenant = Tenants[0];
        }

        public void OnClose()
        {
            // TODO: sve proslo ok, ovde sacuvaj varijaciju, user, pass, token?, tenant, profil
            // sacuvani podaci treba da se povezu na settings dijalog
            (GetView() as Window).Hide();
        }

        public bool CanOnClose => Token != null && SelectedTenant != null && SelectedProfile != null;

        public void OnCancel()
        {
            TryClose(true);
        }

        public void Dispose()
        {

        }
    }
}
