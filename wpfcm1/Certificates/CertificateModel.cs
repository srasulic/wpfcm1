using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.Certificates
{
    public class CertificateModel
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public X509Certificate2 Certificate { get; private set; }
        public List<X509Certificate> ChainElements { get; private set; }

        public string CertificateSimpleName { get; private set; }
        public string CertificateDisplayName { get; private set; }

        public List<string> Errors { get; private set; }
        public string CertDisplayWithErrors { get; private set; }

        public bool IsQualified { get; private set; }
        public bool IsDisplayed { get; private set; }

        public bool HasDigitalSignature { get; private set; }
        public bool HasNonRepudiation { get; private set; }
        public bool HasExtendedDigitalSigning { get; private set; }
        public bool HasEnhancedProperties { get; private set; }

        public CertificateModel(X509Certificate2 certificate)
        {
            Certificate = certificate;
            CertificateSimpleName = Certificate.GetNameInfo(X509NameType.SimpleName, false);
            CertificateDisplayName = CertificateSimpleName + @" - " + Regex.Match(certificate.IssuerName.Name, @"(CN=)(.[A-Za-z]+)(.*)").Groups[2];

            if (!Certificate.HasPrivateKey)
            {
                IsQualified = false;
                return;
            }

            var chainBuildInfo = CertificateHelpers.GetChain(Certificate);
            var chain = chainBuildInfo.Item1;
            ChainElements = CertificateHelpers.GetChainElements(chain);

            Errors = CheckCertificate(Certificate, chainBuildInfo);

            var bccert = DotNetUtilities.FromX509Certificate(Certificate);
            var crlUrl = CertificateUtil.GetCRLURL(bccert);
            var ocspUrl = CertificateUtil.GetOCSPURL(bccert);
            var hasCrl = crlUrl != null;
            var hasOcsp = ocspUrl != null;
            if (!(hasOcsp || hasCrl))
            {
                Errors.Add("Cannot check revocation (no ocsp and crl).");
            }

            IsQualified = Errors.Count == 0 && (hasOcsp || hasCrl);

            if (certificate.Issuer == "C=BA, S=Republika Srpska, O=Poreska uprava, CN=PURS CA 1")
            {
                IsQualified = Errors.Count == 1 && (hasOcsp || hasCrl);
            }

            //IsQualified = HasDigitalSignature && (HasExtendedDigitalSigning || HasNonRepudiation) && (hasOcsp || hasCrl);
            //IsQualified = HasDigitalSignature && (HasExtendedDigitalSigning || !HasEnhancedProperties) && (hasOcsp || hasCrl);
            IsDisplayed = HasDigitalSignature && (HasExtendedDigitalSigning || !HasEnhancedProperties);

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

        private List<string> CheckCertificate(X509Certificate2 certificate, Tuple<X509Chain, bool> chainBuildInfo)
        {
            var errors = new List<string>();

            var chain = chainBuildInfo.Item1;
            var isChainValid = chainBuildInfo.Item2;
            if (!isChainValid)
            {
                errors.Add(string.Format("Certificate not valid - {0}", chain.ChainStatus[0].Status));
            }

            foreach (var extension in certificate.Extensions)
            {
                if (extension is X509KeyUsageExtension)
                {
                    var keyUsageExtension = extension as X509KeyUsageExtension;

                    HasNonRepudiation = (keyUsageExtension.KeyUsages & X509KeyUsageFlags.NonRepudiation) != 0;
                    HasDigitalSignature = (keyUsageExtension.KeyUsages & X509KeyUsageFlags.DigitalSignature) != 0;

                    //if (!(HasNonRepudiation && HasDigitalSignature))
                    if (!HasDigitalSignature)
                    {
                        errors.Add(string.Format("Bad key usage - {0}", keyUsageExtension.KeyUsages));
                    }
                }

                if (extension is X509EnhancedKeyUsageExtension)
                {
                    var enhancedKeyUsageExtension = extension as X509EnhancedKeyUsageExtension;

                    var oids = enhancedKeyUsageExtension.EnhancedKeyUsages.Cast<Oid>();
                    var result = oids.Where(u => u.FriendlyName.Contains("Document Signing")).ToList();

                    HasExtendedDigitalSigning = result.Count() > 0;
                   
                    //Hardkodirano prema dokumentu Politike certifikacije za izdavanje i upravljanje elektronskim certifikatima
                    //Poreske uprave Republike Srpske, prema izmenama 2025. godine.
                    if (certificate.Issuer == "C=BA, S=Republika Srpska, O=Poreska uprava, CN=PURS CA 1")
                    {
                        HasExtendedDigitalSigning = true;
                    }

                    HasEnhancedProperties = true;
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
    }
}
