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
                        
            UserName = "";
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






    public class DialogLoginViewModel : Screen
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
            //(GetView() as Window).Hide();
        }

        public void OnCancel()
        {
            TryClose(true);
        }

        private void Login()
        {
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

            var request = WebRequest.Create("http://edev.office.aserta.rs/login/remoteLogin");

            var postData = "user_name=" + LoginTemp.UserName;
            postData += "&user_password=" + LoginTemp.Password;
            postData += "&pib=" + LoginTemp.PIB;
            var data = Encoding.ASCII.GetBytes(postData);


            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    string responseFromServer = reader.ReadToEnd();

                    string[] requestArguments = responseFromServer.Split('}');
                    string[] message = requestArguments[1].Split('*');


                    if (message.Length > 1)
                    {
                        Log.Info("SUCCESSFUL LOGIN (UserName = " + LoginTemp.UserName + ") - " + message[0]);
                        LoginTemp.Message = message[0];
                        User.Default.Token = message[1];
                        (GetView() as Window).Hide();
                    }
                    else
                    {
                        Log.Error("FAILED LOGIN: (UserName = " + LoginTemp.UserName + ") - " + message[0]);
                        LoginTemp.Message = message[0];
                    }

                    reader.Close();
                }
            }

        }

    }



}
