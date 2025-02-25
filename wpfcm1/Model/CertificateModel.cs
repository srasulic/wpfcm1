using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using wpfcm1.Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Model
{
    public class CertificateModel
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public X509Certificate2 Certificate { get; private set; }
        public string CertificateSimpleName { get; private set; }
        public string CertificateDisplayName { get; private set; }
        public List<X509Certificate> ChainElements { get; private set; }
        public List<string> Errors { get; private set; }
        public string CertDisplayWithErrors { get; private set; }
        public bool IsQualified { get; private set; }
        public bool HasWarnings { get; private set; }

        public CertificateModel(X509Certificate2 certificate)
        {
            Certificate = certificate;
            CertificateSimpleName = Certificate.GetNameInfo(X509NameType.SimpleName, false);
            CertificateDisplayName = CertificateSimpleName + @" - " + Regex.Match(certificate.IssuerName.Name, @"(CN=)(.[A-Za-z]+)(.*)").Groups[2];

            // ako nema private key, nije kvalifikovan. Ubrzava se start aplikacije jer se ne bilduju bespotrebni sertifikati
            if (!Certificate.HasPrivateKey)
            {
                IsQualified = false;
                HasWarnings = false;
                return;
            }

            if (Regex.IsMatch(certificate.Issuer, @"CN=MUPCA"))
            {
                HasWarnings = true;
            }
            else
            {
                HasWarnings = false;
            }

            var chainBuildInfo = CertificateHelpers.GetChain(Certificate);
            var chain = chainBuildInfo.Item1;
            ChainElements = CertificateHelpers.GetChainElements(chain);


            // podrška za nekvalifikovane sertifikate koje koristi Republika Srpska
            if (certificate.IssuerName.Name == "C=BA, S=Republika Srpska, O=Poreska uprava, CN=PURS CA 1")
            {
                Errors = new List<string>();
            }
            else
            {
                Errors = CertificateHelpers.CheckCertificate(Certificate, chainBuildInfo);
            }

            var bccert = DotNetUtilities.FromX509Certificate(Certificate);
            var crlUrl = CertificateUtil.GetCRLURL(bccert);
            var ocspUrl = CertificateUtil.GetOCSPURL(bccert);
            var hasCrl = crlUrl != null;
            var hasOcsp = ocspUrl != null;
            if (!(hasOcsp || hasCrl))
            {
                Errors.Add("Cannot check revocation (no ocsp and crl).");
            }

            //////////
            // privremeno za TEST
            //
            //   IsQualified = true;
            //   return;
            //
            //
            /////////
            IsQualified = Errors.Count == 0 && (hasOcsp || hasCrl);
            // linija za prikaz u listi
            CertDisplayWithErrors = CertificateDisplayName;
            if (IsQualified)
            {
                CertDisplayWithErrors = "* " + CertDisplayWithErrors;
            }
            else
            {
                foreach (var error in Errors)
                {
                    Log.Info("CERT ERR: " + certificate.Subject);
                    Log.Info("          " + error);
                    CertDisplayWithErrors = "* " + CertDisplayWithErrors + System.Environment.NewLine + "      err: " + error;
                }
            }
        }
    }
}
