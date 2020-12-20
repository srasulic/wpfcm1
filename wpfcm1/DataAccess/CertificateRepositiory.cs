using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using wpfcm1.Model;


namespace wpfcm1.DataAccess
{
    public class CertificateRepositiory
    {
        public CertificateRepositiory()
        {
            CertificateItems = LoadCertificateItems();
        }

        public List<CertificateModel> CertificateItems { get; private set; }

        private static List<CertificateModel> LoadCertificateItems()
        {
            /*
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var certificateItems = (from X509Certificate2 certificate in store.Certificates select new CertificateModel(certificate)).ToList();
            return certificateItems;
            */

            certFoundInSmartCard = false;
            var myStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            myStore.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            List<CertificateModel> list = new List<CertificateModel>();
            List<CertificateModel> allQualifiedCerts = new List<CertificateModel>();
            foreach (X509Certificate2 cert in myStore.Certificates)
                {
                try {
                    if (!cert.HasPrivateKey) continue;
                    if (!CertHasNonRepudiation(cert)) continue;
                    // privremeno komentar dok ne nadjemo bolji nacin (podiže se Insert smart card za neke sertifikate iz liste, što nije exception)
                    /*
                    var rsa = cert.PrivateKey as RSACryptoServiceProvider;
                    if (rsa == null) continue;
                    if (rsa.CspKeyContainerInfo.HardwareDevice)
                    {
                        list.Add(new CertificateModel(cert));
                        certFoundInSmartCard= true;                    
                    }
                    */
                    allQualifiedCerts.Add(new CertificateModel(cert));
                    
                }catch (System.Security.Cryptography.CryptographicException ex) { 
                    allQualifiedCerts.Add(new CertificateModel(cert)); 
                }
            }

            if (certFoundInSmartCard)
            {
                return list;
            }
            else
            {
                return allQualifiedCerts;
            }
            
        }

        public static bool certFoundInSmartCard { get; set; }

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

    };

}
