using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace wpfcm1.FTP
{
    public class FtpClient
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FtpClient(string uri, string user, string pass)
        {
            Uri = uri;
            User = user;
            Pass = pass;
            // Da bi nam prolazila kontrola sertifikata bezuslovno (samo želimo kriptovan saobraćaj)
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(myValidateServerCertificate);
        }

        public string Uri { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }

        private FtpWebRequest PrepareFtpRequest (string uri) {

            var req = (FtpWebRequest)WebRequest.Create(uri);
            req.Proxy = null;
            req.Credentials = new NetworkCredential(User, Pass);

            req.EnableSsl = true;
            // ovo nam ne treba jer nam ftp server ne zahteva klijentski cert. Možda nekad u budućnosti...
     //       X509Certificate cert = X509Certificate.CreateFromCertFile(@"D:\temp\edokument.crt");
     //       X509CertificateCollection certCollection = new X509CertificateCollection();
     //       certCollection.Add(cert);
     //       req.ClientCertificates = certCollection;

            return req;
        }

        /// <summary>
        /// List FTP directory files
        /// </summary>
        /// <param name="ftpDir">FTP directory to list (e.g. edokument/faktura/outbound/outbox/)</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListDirectoryAsync(string ftpDir = "")
        {
            var uri = string.Format("{0}/{1}", Uri, ftpDir);

            var req = PrepareFtpRequest(uri);
            var result = new List<string>();
            req.Method = WebRequestMethods.Ftp.ListDirectory;

            using (var resp = req.GetResponse())
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                while (reader.Peek() >= 0)
                {
                    var l = await reader.ReadLineAsync();
                    //TODO: ubaci filter za tip fajlova?
                    result.Add(l);
                }
            }
            return result;
        }

        /// <summary>
        /// List FTP directory details
        /// </summary>
        /// <param name="ftpDir">FTP directory to list (e.g. edokument/faktura/outbound/outbox/)</param>
        /// <returns></returns>
        /// NAPOMENA: odustali smo od asinhrone funkcije jer nije pouzdano vraćala listu fajlova. Sekla je rezultate.
        ///           Kroz debugger je radila ispravno... 
        public IEnumerable<string> ListDirectoryDetails(string ftpDir = "")
        {
            var uri = string.Format("{0}/{1}", Uri, ftpDir);
            var req = PrepareFtpRequest(uri);
            var result = new List<string>();
            
            req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            try
            {
                string lin;
                using (var resp = req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {

                    while ((lin = reader.ReadLine()) != null)
                    //while (reader.Peek() >= 0)
                    {
//                        var l = await reader.ReadLineAsync();
                        result.Add(lin);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("ERR: ListDirectoryDetails", ex);
            }
            return result;
        }

        /// <summary>
        /// Download file from FTP
        /// </summary>
        /// <param name="dir">FTP directory to download from</param>
        /// <param name="fileName">Name of file to download</param>
        /// <param name="destFilePath">Destination file path</param>
        /// <returns></returns>
        public async Task DownloadFileAsync(string dir, string fileName, string destFilePath)
        {
            var wc = new WebClient {Proxy = null, Credentials = new NetworkCredential(User, Pass)};
            var uri = string.Format(@"{0}{1}{2}", Uri, dir, fileName);
            await wc.DownloadFileTaskAsync(uri, destFilePath);
        }

        /// <summary>
        /// Upload file to FTP, eventually provide new file name
        /// </summary>
        /// <param name="sourceFilePath">Source file path</param>
        /// <param name="destinationDir">FTP directory upload destination</param>
        /// <param name="newFileName">Optional new file name</param>
        /// <returns></returns>
        public void UploadFile(string sourceFilePath, string destinationDir, string newFileName = "")
        {
            var ftpFileName = string.IsNullOrEmpty(newFileName) ? Path.GetFileName(sourceFilePath) : newFileName;
            var relUri = string.Format(@"{0}{1}/{2}", Uri, destinationDir, ftpFileName);
            var req = PrepareFtpRequest(relUri);

            //req.KeepAlive = false;
            //req.ReadWriteTimeout = 10000;
            req.Method = WebRequestMethods.Ftp.UploadFile;

            using (var reqStream = req.GetRequestStream())
            {
                var bytes = File.ReadAllBytes(sourceFilePath);
                reqStream.Write(bytes, 0, bytes.Length);
            }

            using (var resp = (FtpWebResponse) req.GetResponse())
            {
                //Log.Info(resp.StatusDescription);
            }
        }

        /// <summary>
        /// Async wrapper for UploadFile
        /// </summary>
        public Task UploadFileAsync(string sourceFilePath, string destinationDir, string newFileName = "")
        {
            return Task.Run(() => UploadFile(sourceFilePath, destinationDir, newFileName));
        }

        /// <summary>
        /// Rename file on FTP
        /// </summary>
        /// <param name="ftpFileUri">FTP uri of file to be renamed</param>
        /// <param name="newFileName">New file name for <paramref name="ftpFileUri"/></param>
        public void RenameFile(string ftpFileUri, string newFileName)
        {
            var req = PrepareFtpRequest(ftpFileUri);
            req.Method = WebRequestMethods.Ftp.Rename;
            req.RenameTo = newFileName;

            using (var resp = (FtpWebResponse) req.GetResponse())
            {
                //Log.Info(resp.StatusDescription);
            }
        }


        public static bool myValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
                return true;
            
        }

    }
}
