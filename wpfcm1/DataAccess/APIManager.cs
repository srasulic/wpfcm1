using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using wpfcm1.Settings;

namespace wpfcm1.DataAccess
{
    public class APIManager
    {
        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                        StreamReader readStream = new StreamReader(responseStream);
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
    }
}
