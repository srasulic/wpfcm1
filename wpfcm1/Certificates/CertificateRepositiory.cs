using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace wpfcm1.Certificates
{
    public class CertificateRepositiory
    {
        public CertificateRepositiory() : this(false) { }
        public CertificateRepositiory(bool pickCertificate)
        {
            CertificateItems = LoadCertificateItems(pickCertificate);
        }
        public List<CertificateModel> CertificateItems { get; private set; }

        private static bool certFoundInSmartCard { get; set; }

        private static List<CertificateModel> LoadCertificateItems(bool pickCertificate)
        {
            certFoundInSmartCard = false;

            var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            List<CertificateModel> list = new List<CertificateModel>();
            List<CertificateModel> allQualifiedCerts = new List<CertificateModel>();

            foreach (X509Certificate2 cert in myStore.Certificates)
            {
                try
                {
                    if (!cert.HasPrivateKey) continue;
                    if (!(CertHasNonRepudiation(cert) || CertHasDigitalSignature(cert))) continue;

                    var cm = new CertificateModel(cert);

                    if (!cm.IsQualified && cm.Errors.Any(e => e == "Bad key usage - KeyEncipherment, DigitalSignature"))
                    {
                        continue;
                    }

                    // privremeno uslovno, dok ne nadjemo bolji nacin (podiže se Insert smart card za neke sertifikate iz liste, što nije exception)
                    if (pickCertificate)
                    {
                        var rsa = cert.PrivateKey as RSACryptoServiceProvider;
                        if (rsa == null) continue;
                        if (rsa.CspKeyContainerInfo.HardwareDevice)
                        {
                            list.Add(cm);
                            certFoundInSmartCard = true;
                        }
                    }

                    allQualifiedCerts.Add(cm);
                }
                catch (CryptographicException)
                { 
                    allQualifiedCerts.Add(new CertificateModel(cert)); 
                }
            }

            if (certFoundInSmartCard)
            {
                return list;
                //return list.Where(c => c.Errors.Count == 0).ToList();
            }
            else
            {
                return allQualifiedCerts;
                //return allQualifiedCerts.Where(c => c.Errors.Count == 0).ToList();
            }
        }

        static public bool CertHasNonRepudiation(X509Certificate2 cert)
        {
            foreach (X509KeyUsageExtension usage_extension in cert.Extensions.OfType<X509KeyUsageExtension>())
            {
                if ((usage_extension.KeyUsages & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation)
                {
                    return true;
                }
            }

            return false;
        }

        static public bool CertHasDigitalSignature(X509Certificate2 cert)
        {
            foreach (X509KeyUsageExtension usage_extension in cert.Extensions.OfType<X509KeyUsageExtension>())
            {
                if ((usage_extension.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature)
                {
                    return true;
                }
            }

            return false;
        }
    };
}
