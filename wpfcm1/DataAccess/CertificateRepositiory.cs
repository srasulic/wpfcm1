using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var certificateItems = (from X509Certificate2 certificate in store.Certificates select new CertificateModel(certificate)).ToList();
            return certificateItems;
        }
    }
}
