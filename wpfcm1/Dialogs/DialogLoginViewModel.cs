using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using wpfcm1.Settings;
using System.Windows;
using System.Net;
using System.IO;
using wpfcm1.DataAccess;
using wpfcm1.FTP;
using Newtonsoft.Json;

namespace wpfcm1.Dialogs
{
    public class LoginModel : PropertyChangedBase
    {
        
        private string _userName;
        private string _password;
        private string _message;
        private string _token;
        private string _pib;

        public LoginModel()
        {
            User user = User.Default;

            UserName = user.UserName;
            Password = "";            
            Token = "";
            Message = "";
            _pib = user.PIB;
            
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

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }        

        public string Token
        {
            get { return _token; }
            set { _token = value; NotifyOfPropertyChange(() => Token); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; NotifyOfPropertyChange(() => Message); }
        }

        public string PIB
        {
            get { return _pib; }
            set { _pib = value; NotifyOfPropertyChange(() => PIB); }
        }


        public string Error { get; set; }
    }






    public class DialogLoginViewModel : Screen, IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LoginModel LoginTemp { get; set; }
        private IWindowManager _windowManager;

        public DialogLoginViewModel(IWindowManager windowManager)
        {
            DisplayName = "Polisign Login";
            LoginTemp = new LoginModel();
            _windowManager = windowManager;
        }

        public void OnClose()
        {
            Login();
            User.Default.UserName = LoginTemp.UserName;
            User.Default.Save();
            //(GetView() as Window).Hide();
        }

        public void OnCancel()
        {
            TryClose(true);
        }

        public void Dispose()
        {
            try
            {
                (GetView() as Window).Close();
            } catch
            {
                // ok, ovde upadamo ako je izabrana opcija bez logina
            }
        }

        public void OpenSettings()
        {
            _windowManager.ShowDialog(new DialogSettingsViewModel());
            LoginTemp.PIB = User.Default.PIB;
        }

        private void Login()
        {
            // OpenSettings();
            if (LoginTemp.UserName == "Settings!")
            {
                _windowManager.ShowDialog(new DialogSettingsViewModel());
            }

            if (Settings.User.Default.Variation == "MTEL")
            {
                LoginMTEL();
            }
            else
            {
                LoginLegacy();
            }
        }
        private void LoginLegacy()
        {
            // OpenSettings();
            //if (LoginTemp.UserName == "Settings!")
            //{
            //    _windowManager.ShowDialog(new DialogSettingsViewModel());
            //}
            if (string.IsNullOrEmpty(LoginTemp.UserName) || string.IsNullOrEmpty(LoginTemp.Password))
            {
                if (string.IsNullOrEmpty(LoginTemp.UserName)) LoginTemp.Message = "Nije uneto korisničko ime!";
                else LoginTemp.Message = "Nije uneta šifra!";
                return;
            }
            if (string.IsNullOrEmpty(LoginTemp.PIB))
            {
                _windowManager.ShowDialog(new DialogSettingsViewModel());
                LoginTemp.PIB = User.Default.PIB;
            }


            string reqUrl = User.Default.ApiURL + @"/login/remoteLogin";
            var request = WebRequest.Create(reqUrl);

            var postData = "user_name=" + LoginTemp.UserName;
            postData += "&user_password=" + LoginTemp.Password;
            postData += "&pib=" + LoginTemp.PIB;
            var data = Encoding.ASCII.GetBytes(postData);


            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                LoginTemp.Message = ex.Message;
            }
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();

                    AuthenticationResponse json = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticationResponse>(responseFromServer);

                    string message = json.message;
                    string token = json.token;
                    string ftpPass = json.ftpPass;
                      

                    if (!String.IsNullOrEmpty(token))
                    {
                        Log.Info("SUCCESSFUL LOGIN (UserName = " + LoginTemp.UserName + ") - " + message);
                        LoginTemp.Message = message;
                        User.Default.Token = token; 
                        User.Default.FtpPassword = "WEB ftpPass used";
                        FtpClient.WebFtpPass = ftpPass;
                        User.Default.FtpUserName = LoginTemp.PIB;

                        (GetView() as Window).Hide();
                        
                    }
                    else
                    {
                        Log.Error("FAILED LOGIN: (UserName = " + LoginTemp.UserName + ") - " + message[0]);
                        LoginTemp.Message = message;
                    }

                    reader.Close();
                }
            }

        }
        private void LoginMTEL()
        {
            // OpenSettings();
            //if (LoginTemp.UserName == "Settings!")
            //{
            //    _windowManager.ShowDialog(new DialogSettingsViewModel());
            //}
            if (string.IsNullOrEmpty(Common.UserDetails.RegBrVozila))
            {
                // LoginTemp.Message = "Nije izabrano vozilo!";
                // return;
                Common.UserDetails.RegBrVozila = "999999";
            }
            if (string.IsNullOrEmpty(LoginTemp.UserName) || string.IsNullOrEmpty(LoginTemp.Password))
            {
                if (string.IsNullOrEmpty(LoginTemp.UserName)) LoginTemp.Message = "Nije uneto korisničko ime!";
                else LoginTemp.Message = "Nije uneta šifra!";
                return;
            }
            if (string.IsNullOrEmpty(LoginTemp.PIB))
            {
                _windowManager.ShowDialog(new DialogSettingsViewModel());
                LoginTemp.PIB = User.Default.PIB;
            }

            string reqUrl;
            string postData;
            reqUrl = User.Default.ApiURL + @"/olympus/v1/users/login";
            postData = "grant_type=password";
            //postData += "&username=" + LoginTemp.PIB + "%23%23%23" + LoginTemp.UserName;
            postData += "&username=" + LoginTemp.UserName;
            postData += "&password=" + LoginTemp.Password;
            postData += "&scope=&client_id=string&client_secret=string";


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqUrl);

            var data = Encoding.UTF8.GetBytes(postData);

            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json"; // dodato za Olympus
            request.ContentLength = data.Length;

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                LoginTemp.Message = ex.Message;
            }


            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();

                    AuthenticationResponseMTEL json = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticationResponseMTEL>(responseFromServer);

                    string token = json.access_token;

                    if (!String.IsNullOrEmpty(token))
                    {
                        Log.Info("SUCCESSFUL LOGIN (UserName = " + LoginTemp.UserName + ") ");
                        Common.UserDetails.CurrentToken = token;
                        User.Default.FtpPassword = "";
                        FtpClient.WebFtpPass = "";
                        User.Default.FtpUserName = "";

                        Log.Info("Obtaining Olympus tenant profile (UserName = " + LoginTemp.UserName);
                        //var alfrescoClient = new AlfrescoClient(User.Default.FtpServer, LoginTemp.UserName, LoginTemp.Password);
                        string jsonResponse = @"{ ";
                        jsonResponse += @"    ""tenant_info"": {";
                        jsonResponse += @"        ""tenant"": ""108572864"", ";
                        jsonResponse += @"        ""ib"": ""108572864"", ";
                        jsonResponse += @"        ""naziv"": ""Test Aserta doo"", ";
                        jsonResponse += @"        ""ext_tenant_id"": ""108572864"", ";
                        jsonResponse += @"        ""inbox_node_id"": null, ";
                        jsonResponse += @"        ""ts_url"": ""N"", ";
                        jsonResponse += @"        ""ts_username"": null, ";
                        jsonResponse += @"        ""ts_pass"": null, ";
                        jsonResponse += @"        ""arch_polisa"": null, ";
                        jsonResponse += @"        ""X_SIG_SHIFT_LEVI"": 590, ";
                        jsonResponse += @"        ""Y_SIG_SHIFT_LEVI"": 840, ";
                        jsonResponse += @"        ""X_SIG_SHIFT_DESNI"": 190, ";
                        jsonResponse += @"        ""Y_SIG_SHIFT_DESNI"": 190 ";
                        jsonResponse += @"    }, ";
                        jsonResponse += @"    ""tip_dok_info"": [ ";
                        jsonResponse += @"        {";
                        jsonResponse += @"            ""tip_dok"": ""faktura"",";
                        jsonResponse += @"            ""smer"": ""outbound""";
                        jsonResponse += @"        },";
                        jsonResponse += @"        {";
                        jsonResponse += @"            ""tip_dok"": ""ios"",";
                        jsonResponse += @"            ""smer"": ""outbound""";
                        jsonResponse += @"        },";
                        jsonResponse += @"        {";
                        jsonResponse += @"            ""tip_dok"": ""kp"",";
                        jsonResponse += @"            ""smer"": ""outbound""";
                        jsonResponse += @"        }";
                        jsonResponse += @"    ]";
                        jsonResponse += @"}";


                        ApiTenantProfileResponse tpResponse = JsonConvert.DeserializeObject<ApiTenantProfileResponse>(jsonResponse);
                        Console.WriteLine("Naziv: " + tpResponse.tenant_info.naziv);

                        User.Default.PIB = tpResponse.tenant_info.ib;
                        User.Default.XSigShift = tpResponse.tenant_info.X_SIG_SHIFT_LEVI;
                        User.Default.YSigShift = tpResponse.tenant_info.Y_SIG_SHIFT_LEVI;
                        User.Default.XSigShiftRight = tpResponse.tenant_info.X_SIG_SHIFT_DESNI;
                        User.Default.YSigShiftRight = tpResponse.tenant_info.Y_SIG_SHIFT_DESNI;

                        User.Default.TimestampServer = tpResponse.tenant_info.ts_url;
                        User.Default.TimestampUserName = tpResponse.tenant_info.ts_username;
                        User.Default.TimestampPassword = tpResponse.tenant_info.ts_pass;


                        // TESTIRAMO 
                        // da li bi podešavanje User. odavde umesto iz settings radilo
                        // 
                        User.Default.IosInbound = false;
                        User.Default.IosOutbound = false;
                        User.Default.KpInbound = false;
                        User.Default.KpOutbound = false;
                        User.Default.OtherInbound = false;
                        User.Default.OtherOutbound = false;

                        foreach (var tipDok in tpResponse.tip_dok_info)
                        {
                            Console.WriteLine($"Tip dokumenta: {tipDok.tip_dok}, Smer: {tipDok.smer}");
                            if (tipDok.smer == "inbound")
                            {
                                switch (tipDok.tip_dok)
                                {
                                    case "izdavanje":
                                        User.Default.IzdavanjeInbound = true;
                                        break;
                                    case "razvoz":
                                        User.Default.RazvozInbound = true;
                                        break;
                                    //case "utovar":
                                    //    User.Default.UtovarInbound = true;
                                    //    break;
                                    case "ios":
                                        User.Default.IosInbound = true;
                                        break;
                                    //case "faktura":
                                    //    break;
                                    case "kp":
                                        User.Default.KpInbound = true;
                                        break;
                                    //case "otpad":
                                    //    break;
                                    case "ostali":
                                        User.Default.OtherInbound = true;
                                        break;
                                    //case "plata":
                                    //    break;
                                    default:
                                        break;
                                }
                            }

                            if (tipDok.smer == "outbound")
                            {
                                switch (tipDok.tip_dok)
                                {
                                    case "izdavanje":
                                        User.Default.IzdavanjeOutbound = true;
                                        break;
                                    case "razvoz":
                                        User.Default.RazvozOutbound = true;
                                        break;
                                    //case "utovar":
                                    //    User.Default.UtovarOutbound = true;
                                    //    break;
                                    case "ios":
                                        User.Default.IosOutbound = true;
                                        break;
                                    //case "faktura":
                                    //    break;
                                    case "kp":
                                        User.Default.KpOutbound = true;
                                        break;
                                    //case "otpad":
                                    //    break;
                                    case "ostali":
                                        User.Default.OtherOutbound = true;
                                        break;
                                    //case "plata":
                                    //    break;
                                    default:
                                        break;
                                }
                            }



                        }

                    }
                    else
                    {
                        Log.Error("FAILED LOGIN: (UserName = " + LoginTemp.UserName + ") - ");
                        LoginTemp.Message = "FAILED LOGIN";
                    }



                    reader.Close();
                }
            }

        }
    }

    public class AuthenticationResponse
    {
        public string message { get; set; }
        public string token { get; set; }
        public string ftpPass { get; set; }
    }

    public class AuthenticationResponseMTEL
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class TenantInfo
    {
        public string tenant { get; set; }
        public string ib { get; set; }
        public string naziv { get; set; }
        public string ext_tenant_id { get; set; }
        public string inbox_node_id { get; set; }
        public string ts_url { get; set; }
        public string ts_username { get; set; }
        public string ts_pass { get; set; }
        public string arch_polisa { get; set; }
        public int X_SIG_SHIFT_LEVI { get; set; }
        public int Y_SIG_SHIFT_LEVI { get; set; }
        public int X_SIG_SHIFT_DESNI { get; set; }
        public int Y_SIG_SHIFT_DESNI { get; set; }
    }



    public class TipDokInfo
    {
        public string tip_dok { get; set; }
        public string smer { get; set; }
    }

    public class ApiTenantProfileResponse
    {
        public TenantInfo tenant_info { get; set; }
        public List<TipDokInfo> tip_dok_info { get; set; }
    }


}
