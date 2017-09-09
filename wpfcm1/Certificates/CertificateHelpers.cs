using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Certificates
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
                if (Regex.IsMatch(crlUrl, @"http.+", RegexOptions.IgnoreCase)) 
                {

                    System.Net.WebRequest req = System.Net.HttpWebRequest.Create(crlUrl);
                    System.IO.Stream ins = req.GetResponse().GetResponseStream();
                    var baos = new System.IO.MemoryStream();
                    byte[] buf = new byte[1024];
                    int readedBytes;
                    while ((readedBytes = ins.Read(buf, 0, 1024)) > 0) baos.Write(buf, 0, readedBytes);
                    ins.Close();
                    ICrlClient ccoff = new CrlClientOffline(baos.ToArray());
                    crlList.Add(ccoff);

                } else {
                    using (var de = new DirectoryEntry(crlUrl) { AuthenticationType = AuthenticationTypes.Anonymous })
                    {
                        var crlBytes = de.Properties["certificateRevocationList;binary"].Value as byte[];
                        var ccoff = new CrlClientOffline(crlBytes);
                        crlList.Add(ccoff);
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

        public static List<string> CheckCertificate(X509Certificate2 certificate, Tuple<X509Chain, bool> chainBuildInfo)
        {
            var errors = new List<string>();

            var certificateSimpleName = certificate.GetNameInfo(X509NameType.SimpleName, false);
            var match = Regex.Match(certificateSimpleName, @"\b(\d{6,9})(-(\d{13}))?\b");
            var jmbg = match.Groups[3].Value;
            // TODO: Zasto nam je ovo uopste vazno? Da li smo imali neku ideju sta sa JMBG ili nismo znali da postoje specijlani slucajevi kada se sert moze izdati bez jmbg...
            if (!string.IsNullOrEmpty(jmbg))
            {
                bool jmbgOk = CheckJmbg(jmbg);
                if (!jmbgOk) errors.Add("Bad JMBG.");
            }

            var chain = chainBuildInfo.Item1;
            var isChainValid = chainBuildInfo.Item2;
            if (!isChainValid)
            {
                errors.Add(string.Format("Certificate not valid - {0}", chain.ChainStatus[0].Status));
            }

            foreach (var extension in certificate.Extensions)
            {
                var isCritical = extension.Critical;
                var fn = extension.Oid.FriendlyName;
                var v = extension.Oid.Value;
                var fs = extension.Format(true);
                if (extension is X509KeyUsageExtension)
                {
                    var keyUsageExtension = extension as X509KeyUsageExtension;
                    if (!((keyUsageExtension.KeyUsages & X509KeyUsageFlags.NonRepudiation) != 0 ||
                          (keyUsageExtension.KeyUsages & (X509KeyUsageFlags.NonRepudiation & X509KeyUsageFlags.DigitalSignature)) !=
                          0)
                        )
                    {
                        errors.Add(string.Format("Bad key usage - {0}", keyUsageExtension.KeyUsages));

                    }
                }
                //if (v == "1.3.6.1.5.5.7.1.3") //QcStatements
                //{
                //    Asn1OctetString octetString = bccert.GetExtensionValue(new DerObjectIdentifier(v));
                //    byte[] der = octetString.GetOctets();
                //    Asn1Object asn1obj = Asn1Object.FromByteArray(der);
                //}
                //if (v == "2.5.29.32") //policy ids
                //{
                //    Asn1OctetString octetString = bccert.GetExtensionValue(new DerObjectIdentifier(v));
                //    byte[] der = octetString.GetOctets();
                //    Asn1Object asn1obj = Asn1Object.FromByteArray(der);
                //}
            }

            return errors;
        }

        private static bool CheckJmbg(string jmbg)
        {
            if (string.IsNullOrEmpty(jmbg))
                return false;

            var j = new int[jmbg.Length];
            for (var i = 0; i < jmbg.Length; i++)
            {
                j[i] = int.Parse(jmbg.ElementAt(i).ToString());
            }

            //A.B.V.G.D.Đ.E.Ž.Z.I.J.K.L
            //0.1.2.3.4.5.6.7.8.9.10.11.12
            //L = 11 - (( 7*(A+E) + 6*(B+Ž) + 5*(V+Z) + 4*(G+I) + 3*(D+J) + 2*(Đ+K) ) % 11)
            var l = 11 - ((7 * (j[0] + j[6]) + 6 * (j[1] + j[7]) + 5 * (j[2] + j[8]) + 4 * (j[3] + j[9]) + 3 * (j[4] + j[10]) + 2 * (j[5] + j[11])) % 11);
            l = l > 10 ? 0 : l;

            return l == j[12];
        }
    }
}
