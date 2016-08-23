using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using wpfcm1.Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Model
{
    public class CertificateModel
    {
        public X509Certificate2 Certificate { get; private set; }
        public string CertificateSimpleName { get; private set; }
        public List<X509Certificate> ChainElements { get; private set; }
        public List<string> Errors { get; private set; }
        public bool IsQualified { get; private set; }

        public CertificateModel(X509Certificate2 certificate)
        {
            Certificate = certificate;
            CertificateSimpleName = Certificate.GetNameInfo(X509NameType.SimpleName, false);

            // ako nema private key, nije kvalifikovan. Ubrzava se start aplikacije jer se ne bilduju bespotrebni sertifikati
            if (!Certificate.HasPrivateKey)
            {
                IsQualified = false;
                return;
            }

            var chainBuildInfo = CertificateHelpers.GetChain(Certificate);
            var chain = chainBuildInfo.Item1;
            ChainElements = CertificateHelpers.GetChainElements(chain);

            Errors = CertificateHelpers.CheckCertificate(Certificate, chainBuildInfo);

            var bccert = DotNetUtilities.FromX509Certificate(Certificate);
            var crlUrl = CertificateUtil.GetCRLURL(bccert);
            var ocspUrl = CertificateUtil.GetOCSPURL(bccert);
            var hasCrl = crlUrl != null;
            var hasOcsp = ocspUrl != null;
            if (!(hasOcsp || hasCrl))
                Errors.Add("Cannot check revocation (no ocsp and crl).");

            IsQualified = Errors.Count == 0 && (hasOcsp || hasCrl);
        }
    }
}
