using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Invoices.FTP
{
    public class FtpClient
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Uri { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }

        public FtpClient(string uri, string user, string pass)
        {
            Uri = uri;
            User = user;
            Pass = pass;
        }

        /// <summary>
        /// List FTP directory files
        /// </summary>
        /// <param name="ftpDir">FTP directory to list (e.g. edokument/faktura/outbound/outbox/)</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListDirectoryAsync(string ftpDir = "")
        {
            var uri = string.Format("{0}/{1}", Uri, ftpDir);
            Log.Info(string.Format("Fetching file list from {0}", uri));

            var result = new List<string>();
            try
            {
                var ftpReq = (FtpWebRequest)WebRequest.Create(uri);
                ftpReq.Proxy = null;
                ftpReq.Credentials = new NetworkCredential(User, Pass);
                ftpReq.Method = WebRequestMethods.Ftp.ListDirectory;

                using (var resp = ftpReq.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    while (reader.Peek() >= 0)
                    {
                        var l = await reader.ReadLineAsync();
                        //TODO: ubaci filter za tip fajlova?
                        result.Add(l);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("ListDirectoryAsync failed: {0}", ftpDir), e);
                throw;
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
            try
            {
                await wc.DownloadFileTaskAsync(uri, destFilePath);
            }
            catch (Exception e)
            {
                Log.Error(string.Format("DownloadFileAsync error {0}", destFilePath), e);
                throw;
            } 
        }

        /// <summary>
        /// Upload file to FTP, eventually provide new file name
        /// </summary>
        /// <param name="dir">FTP directory upload destination</param>
        /// <param name="filePath">Source file path</param>
        /// <param name="newFileName">Optional new file name</param>
        /// <returns></returns>
        public void UploadFile(string dir, string filePath, string newFileName="")
        {
            var ftpFileName = string.IsNullOrEmpty(newFileName) ? Path.GetFileName(filePath) : newFileName;
            var relUri = string.Format(@"{0}{1}/{2}", Uri, dir, ftpFileName);
            try
            {
                var req = (FtpWebRequest)WebRequest.Create(relUri);
                req.Proxy = null;
                req.Credentials = new NetworkCredential(User, Pass);
                //req.KeepAlive = false;
                //req.ReadWriteTimeout = 10000;
                req.Method = WebRequestMethods.Ftp.UploadFile;

                using (var reqStream = req.GetRequestStream())
                {
                    var bytes = File.ReadAllBytes(filePath);
                    reqStream.Write(bytes, 0, bytes.Length);
                }

                using (var resp = (FtpWebResponse) req.GetResponse())
                {
                    //Log.Info(resp.StatusDescription);
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("UploadFile error {0}", filePath), e);
                throw;
            }
        }

        /// <summary>
        /// Async wrapper for UploadFile
        /// </summary>
        public Task UploadFileAsync(string dir, string filePath, string newFileName = "")
        {
            return Task.Run(() => UploadFile(dir, filePath, newFileName));
        }

        /// <summary>
        /// Rename file on FTP
        /// </summary>
        /// <param name="ftpFileUri">FTP uri of file to be renamed</param>
        /// <param name="newFileName">New file name for <paramref name="ftpFileUri"/></param>
        public void RenameFile(string ftpFileUri, string newFileName)
        {
            try
            {
                var req = (FtpWebRequest)WebRequest.Create(ftpFileUri);
                req.Proxy = null;
                req.Credentials = new NetworkCredential(User, Pass);
                req.Method = WebRequestMethods.Ftp.Rename;
                req.RenameTo = newFileName;

                using (var resp = (FtpWebResponse) req.GetResponse())
                {
                    //Log.Info(resp.StatusDescription);
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("RenameFile error"), e);
                throw;
            }
        }
    }
}
