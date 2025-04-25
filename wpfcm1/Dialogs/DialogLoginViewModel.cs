using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using wpfcm1.OlympusApi;
using wpfcm1.Settings;

namespace wpfcm1.Dialogs
{
    public class Variation
    {
        public string Name { get; set; }
        public string FtpServer { get; set; }
        public string AppTitle { get; set; }
        public string ApiUrl { get; set; }
    }

    public class LoginOkCache
    {
        public Variation Variation { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public Token Token { get; set; }
        public Tenant Tenant { get; set; }
        public Profile Profile { get; set; }
        public Mappings Mappings { get; set; }
    }

    public class DialogLoginViewModel : Screen, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private LoginOkCache _loginOkCache = new LoginOkCache();

        public DialogLoginViewModel(IWindowManager windowManager)
        {
            DisplayName = "Polisign Login";
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            int index = Variations.FindIndex(p => p.Name == User.Default.Variation);
            if (index != -1)
            {
                SelectedVariation = Variations[index];
            }
            else
            {
                SelectedVariation = Variations[0];
            }

            UserName = User.Default.UserName;
            return Task.CompletedTask;
        }

        private List<Variation> _variations = new List<Variation>
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
        public List<Variation> Variations
        {
            get => _variations;
            set
            {
                _variations = value;
            }
        }

        private Variation _variation;
        public Variation SelectedVariation
        {
            get => _variation;
            set
            {
                _variation = value;
                NotifyOfPropertyChange(() => SelectedVariation);
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        private Token _token;
        public Token Token
        {
            get => _token;
            set
            {
                _token = value;
                NotifyOfPropertyChange(() => Token);
                NotifyOfPropertyChange(() => CanSaveAndClose);
            }
        }

        private BindableCollection<Tenant> _tenants;
        public BindableCollection<Tenant> Tenants
        {
            get => _tenants;
            set
            {
                _tenants = value;
                NotifyOfPropertyChange(() => Tenants);
                NotifyOfPropertyChange(() => CanSaveAndClose);
            }
        }

        private Tenant _tenant;
        public Tenant SelectedTenant
        {
            get => _tenant;
            set
            {
                _tenant = value;
                NotifyOfPropertyChange(() => SelectedTenant);
                NotifyOfPropertyChange(() => CanSaveAndClose);
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
                NotifyOfPropertyChange(() => CanSaveAndClose);
            }
        }

        public async void Login()
        {
            var svc = new OlympusService(SelectedVariation.ApiUrl);
            Tenants = null;

            var token = await svc.PostUsersLogin(UserName, Password);
            if (token == null)
            {
                Log.Error($"FAILED LOGIN: [UserName = {UserName}]");
                Tenants = null;
                //SelectedTenant = null;
                return;
            }
            Log.Info($"SUCCESSFUL LOGIN: [UserName = {UserName}]");

            Token = token;

            var userTenants = await svc.GetUsersTenants(token);
            if (userTenants == null || userTenants.result.code != 0)
            {
                Log.Error($"FAILED Obtaining Olympus Tenants: [UserName = {UserName}]");
                Log.Error($"ERROR GetUsersTenants {userTenants.result.userMessage}");
                return;
            }
            Log.Info($"SUCCESSFUL Obtaining Olympus Tenants: [UserName = {UserName}]");

            Tenants = new BindableCollection<Tenant>(userTenants.tenants);
            SelectedTenant = Tenants[0];

            _loginOkCache.Variation = OlympusService.DeepCopy(SelectedVariation);
            _loginOkCache.User = UserName;
            _loginOkCache.Pass = Password;
            _loginOkCache.Token = OlympusService.DeepCopy(Token);
        }

        public async void OnSelectedTenant()
        {
            var svc = new OlympusService(SelectedVariation.ApiUrl);

            var putres = await svc.PutUsersSetTenant(Token, SelectedTenant);
            if (putres == null || putres.code != 0)
            {
                Log.Error($"FAILED PUT set tenant: [UserName = {UserName}] [Tenant = {SelectedTenant?.tenant}]");
                Log.Error($"ERROR PutUsersSetTenant {putres.userMessage}");
                Tenants = null;
                //SelectedTenant = null;
                return;
            }
            Log.Info($"SUCCESSFUL PUT set tenant: [UserName = {UserName}] [Tenant = {SelectedTenant?.tenant}]");

            ProfileResult res = await svc.GetConfigPolisign(Token, SelectedTenant);
            if (res == null || res.result.code != 0)
            {
                Log.Error($"FAILED Obtaining Olympus Profile: [UserName = {UserName}] [Tenant = {SelectedTenant?.tenant}]");
                Log.Error($"ERROR GetConfigPolisign {res.result.userMessage}");
                return;
            }
            Log.Info($"SUCCESSFUL Obtaining Olympus Profile: [UserName = {UserName}] [Tenant = {SelectedTenant?.tenant}]");

            SelectedProfile = res.profile;

            var mapres = await svc.GetConfigDocumentTypeMappings(Token);
            if (mapres == null || mapres.result.code != 0)
            {
                Log.Error($"ERROR GetConfigDocumentTypeMappings: {mapres.result.userMessage}");
                return;
            }

            _loginOkCache.Mappings = OlympusService.DeepCopy(mapres.mappings);
            _loginOkCache.Tenant = OlympusService.DeepCopy(SelectedTenant);
            _loginOkCache.Profile = OlympusService.DeepCopy(SelectedProfile);
        }

        public void SaveAndClose()
        {
            User.Default.UserName = _loginOkCache.User;
            User.Default.ApiURL = _loginOkCache.Variation.ApiUrl;
            User.Default.AppTitle = _loginOkCache.Variation.AppTitle;
            User.Default.Variation = _loginOkCache.Variation.Name;
            User.Default.FtpServer = _loginOkCache.Variation.FtpServer;
            User.Default.PIB = _loginOkCache.Profile.tenant_info.ib;
            User.Default.TimestampServer = _loginOkCache.Profile.tenant_info.ts_url;
            User.Default.TimestampUserName = _loginOkCache.Profile.tenant_info.ts_username;
            User.Default.TimestampPassword = _loginOkCache.Profile.tenant_info.ts_pass;
            User.Default.XSigShift = _loginOkCache.Profile.tenant_info.X_SIG_SHIFT_LEVI;
            User.Default.YSigShift = _loginOkCache.Profile.tenant_info.Y_SIG_SHIFT_LEVI;
            User.Default.XSigShiftRight = _loginOkCache.Profile.tenant_info.X_SIG_SHIFT_DESNI;
            User.Default.YSigShiftRight = _loginOkCache.Profile.tenant_info.Y_SIG_SHIFT_DESNI;

            User.Default.JsonMappings = OlympusService.SerializeToJson(_loginOkCache.Mappings);
            User.Default.JsonToken = OlympusService.SerializeToJson(_loginOkCache.Token);
            User.Default.JsonProfile = OlympusService.SerializeToJson(_loginOkCache.Profile);

            User.Default.Save();
            //TODO: treba mi DI, servis za api, login, setingse i foldere

            (GetView() as Window).DialogResult = true;
        }

        public bool CanSaveAndClose => Token != null && SelectedTenant != null && SelectedProfile != null;

        public void Cancel()
        {
            (GetView() as Window).DialogResult = false;
        }

        public void Dispose()
        {

        }
    }
}
