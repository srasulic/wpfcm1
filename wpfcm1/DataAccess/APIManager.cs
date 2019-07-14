using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using wpfcm1.Settings;

namespace wpfcm1.DataAccess
{
    public class APIManager
    {
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
            catch
            {
                return "";
            }
        }

        public static string GetArchivePolicy()
        {
            var pib = User.Default.PIB;

            if (pib == "105480755")
                return "NBGP";
            else
                return "BASIC";

        }


    }
}
