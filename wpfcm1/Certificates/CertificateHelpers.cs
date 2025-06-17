using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Certificates
{
    public static class CertificateHelpers
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Tuple<X509Chain, bool> GetChain(X509Certificate2 cert)
        {
            var chain = new X509Chain();
            var isValid = chain.Build(cert);
            return new Tuple<X509Chain, bool>(chain, isValid);
        }

        public static List<X509Certificate> GetChainElements(X509Chain chain)
        {
            return (
                from X509ChainElement x509ChainElement
                in chain.ChainElements
                select DotNetUtilities.FromX509Certificate(x509ChainElement.Certificate)
            ).ToList();
        }

        public static IList<ICrlClient> GetCrlClients(List<X509Certificate> chain)
        {
            IList<ICrlClient> crlList = new List<ICrlClient>();
            crlList.Add(new CrlClientOnline(chain));
            return crlList;
        }

        public static IList<ICrlClient> GetCrlClentsOffline(List<X509Certificate> chainElements)
        {
            var crlUrls = new List<string>();
            foreach (var cert in chainElements)
            {
                var crlUrl = CertificateUtil.GetCRLURL(cert);
                if (string.IsNullOrEmpty(crlUrl)) continue;

                var crlUri = new Uri(crlUrl);
                crlUrl = Uri.UnescapeDataString(crlUri.GetLeftPart(UriPartial.Path));
                crlUrl = crlUrl.Substring(0, 4).ToUpper() + crlUrl.Substring(4, crlUrl.Length - 4);
                if (!crlUrls.Contains(crlUrl))
                    crlUrls.Add(crlUrl);
            }

            var crlList = new List<ICrlClient>();
            foreach (var crlUrl in crlUrls)
            {
                if (Regex.IsMatch(crlUrl, @"http.+", RegexOptions.IgnoreCase))
                {
                    // za NBGP BIH (Delta Planet) workaround ... Tamo cert telo nije umelo da zanovi SSL sertifikat na adresi koja vraća CRL
                    if (Settings.User.Default.Variation == "BIH")
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    }

                    System.Net.WebRequest req = System.Net.HttpWebRequest.Create(crlUrl);
                    System.IO.Stream ins = req.GetResponse().GetResponseStream();
                    var baos = new System.IO.MemoryStream();
                    byte[] buf = new byte[1024];
                    int readedBytes;
                    while ((readedBytes = ins.Read(buf, 0, 1024)) > 0) baos.Write(buf, 0, readedBytes);
                    ins.Close();
                    ICrlClient ccoff = new CrlClientOffline(baos.ToArray());
                    crlList.Add(ccoff);
                }
                else
                {
                    using (var de = new DirectoryEntry(crlUrl) { AuthenticationType = AuthenticationTypes.Anonymous })
                    {
                        try { 
                            var crlBytes = de.Properties["certificateRevocationList;binary"].Value as byte[];

                            // HALCOM podrška - zbog nove LDAP specifikacije, vrednost se vraća za atribut koji nema "binary" u nazivu:
                            if (crlBytes == null || crlBytes.Length == 0)
                            {
                                crlBytes = de.Properties["certificateRevocationList"].Value as byte[];
                            }

                            var ccoff = new CrlClientOffline(crlBytes);
                            crlList.Add(ccoff);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.Message + crlUrl);
                            // TODO: da li nam treba crl lista?
                            //throw new Exception(@"Server sertifikacionog tela nije dostupan: " + crlUrl, ex);
                        }
                    }
                }
            }

            return crlList;
        }

        public static Task<IList<ICrlClient>> GetCrlClentsOfflineAsync(List<X509Certificate> chainElements)
        {
            return Task.Run(() =>
                GetCrlClentsOffline(chainElements)
            );
        }

        public static ITSAClient GetTsaClient(List<X509Certificate> chain)
        {
            return (
                from crt
                in chain
                select CertificateUtil.GetTSAURL(crt)
                    into tsaUrl
                    where tsaUrl != null
                    select new TSAClientBouncyCastle(tsaUrl)
            ).FirstOrDefault();
        }


    }
}
