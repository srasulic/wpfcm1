using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using wpfcm1.Settings;
using static wpfcm1.Common.PoliSignTypes;

namespace wpfcm1.DataAccess
{
    public class APIManager
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static HttpClient httpClient = new HttpClient();

        public static string GetCustomerNameByPIB(string pib)
        {
            try
            {
                string urlAddress = string.Format("{0}/index/api?pib={1}", User.Default.ApiURL, pib);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                    string data = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();

                    if (string.IsNullOrEmpty(data))
                        return "";

                    string[] arrayData = data.Split(',');
                    string[] arrayName = arrayData[0].Split(':');
                    return arrayName[1].Replace("\"", string.Empty);
                }

                return "";
            }
            catch (Exception ex)
            {
                Log.Info("ERR: GetCustomerNameByPIB" + ex);
                return "";
            }
        }

        public static async Task<string> GetCustomerNameByPIBAsync(string pib)
        {
            try
            {

                string urlAddress = string.Format("{0}/index/api?pib={1}", User.Default.ApiURL, pib);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader readStream = null;

                        readStream = new StreamReader(responseStream);

                        string data = await readStream.ReadToEndAsync();

                        readStream.Close();


                        if (string.IsNullOrEmpty(data))
                            return "";

                        string[] arrayData = data.Split(',');
                        string[] arrayName = arrayData[0].Split(':');
                        return arrayName[1].Replace("\"", string.Empty);
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        public static string GetArchivePolicy()
        {
            // TODO: privremeno je u Settingsu, treba dodati Web Servis i izmestiti ovo podešavanje na serversku stranu
            if (!String.IsNullOrEmpty(User.Default.ArchivePolicy))
                return User.Default.ArchivePolicy;
            return "BASIC";

        }
        /**
         * PortalAPIRequest implementira prozivanje apija na Portalu sa autentifikacijom putem tokena.
         * 
         */

        public static async Task<string> TestRequestAsync()
        {
            var data = await PortalAPIRequestAsync("testRequest", null);

            System.Windows.MessageBox.Show(data);

            return data;
        }

        public static async Task<List<Linija>> GetSveLinijePublicAsync(string pib)
        {
            var pairs = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("pib", pib) };
            var content = new FormUrlEncodedContent(pairs);
            var response = await PortalPublicAPIRequestAsync("getSveLinije", content);
            List<Linija> resultSet = JsonConvert.DeserializeObject<List<Linija>>(response);
            return resultSet;
        }

        public static async Task<List<FileToDownload>> GetFilesToDownloadAsync()
        {
            var pairs = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("sf_voz", Common.UserDetails.RegBrVozila) };
            var content = new FormUrlEncodedContent(pairs);
            var response = await PortalAPIRequestAsync("getFilesToDownload", content);
            List<FileToDownload> resultSet = JsonConvert.DeserializeObject<List<FileToDownload>>(response);
            return resultSet;
        }
        public static async Task<string> GetDeliveryPointAsync(string f_naziv)
        {
            var pairs = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("f_naziv", f_naziv) };
            var content = new FormUrlEncodedContent(pairs);
            var response = await PortalAPIRequestAsync("getDeliveryPoint", content);
            //string resultSet = JsonConvert.DeserializeObject<string>(response);
            if (Regex.IsMatch(response, "ERROR:", RegexOptions.IgnoreCase))  return ""; 
            else return response;
        }

        public static async Task<string> SetRazvozStavkaStatus(string f_naziv)
        {
            var pairs = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("sf_voz", Common.UserDetails.RegBrVozila), new KeyValuePair<string, string>("f_naziv", f_naziv) };
            var content = new FormUrlEncodedContent(pairs);
            var response = await PortalAPIRequestAsync("setRazvozStavkaStatus", content);
            return response;
        }
        private static async Task<string> PortalAPIRequestAsync(string remoteMethod, FormUrlEncodedContent postPayload)
        {
           try
            {
                string urlAddress = string.Format("{0}/API/{1}", User.Default.ApiURL, remoteMethod);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(urlAddress),
                    Headers = {
                        { HttpRequestHeader.Authorization.ToString(), "Bearer " + Common.UserDetails.CurrentToken },
                        { HttpRequestHeader.Accept.ToString(), "application/json" },
                        { "X-Version", "1" }
                    },
                    Content = postPayload
                };

                var result = await httpClient.SendAsync(httpRequestMessage);

                string data = "";
                if (result.IsSuccessStatusCode)
                {
                    data = await result.Content.ReadAsStringAsync();
                }

                return data;
            }
            catch (Exception ex)
            {
                Log.Info("ERR: PortalAPIRequest" + ex);
                return ex.Message;

            }
        }


        private static async Task<string> PortalPublicAPIRequestAsync(string remoteMethod, FormUrlEncodedContent postPayload)
        {
            try
            {
                string urlAddress = string.Format("{0}/index/{1}", User.Default.ApiURL, remoteMethod);

                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(urlAddress),
                    Headers = {
                    //    { HttpRequestHeader.Authorization.ToString(), "Bearer " + Common.UserDetails.CurrentToken },
                        { HttpRequestHeader.Accept.ToString(), "application/json" },
                        { "X-Version", "1" }
                    },
                    Content = postPayload
                };

                var result = await httpClient.SendAsync(httpRequestMessage);

                string data = "";
                if (result.IsSuccessStatusCode)
                {
                    data = await result.Content.ReadAsStringAsync();
                }

                return data;
            }
            catch (Exception ex)
            {
                Log.Info("ERR: PortalPublicAPIRequest" + ex);
                return ex.Message;

            }
        }


    }
}
