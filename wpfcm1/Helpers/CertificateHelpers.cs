using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Helpers
{
    public static class CertificateHelpers
    {
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
                using (var de = new DirectoryEntry(crlUrl) { AuthenticationType = AuthenticationTypes.Anonymous })
                {
                    var crlBytes = de.Properties["certificateRevocationList;binary"].Value as byte[];
                    var ccoff = new CrlClientOffline(crlBytes);
                    crlList.Add(ccoff);
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
