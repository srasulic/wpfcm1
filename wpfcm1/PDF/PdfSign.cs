using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.PDF
{
    public static class PdfSign
    {
        private static void SignPdf(
            string src,
            string dst,
            X509Certificate2 cert,
            List<X509Certificate> chain,
            ICollection<ICrlClient> crlList,
            IOcspClient ocspClient,
            ITSAClient tsaClient,
            SignatureRules.SignatureLocation sigLocation)
        {
            using (var reader = new PdfReader(src))
            using (var fs = new FileStream(dst, FileMode.Create))
            switch (sigLocation)
            {
                case SignatureRules.SignatureLocation.UpperLeft:
                    using (var stamper = PdfStamper.CreateSignature(reader, fs, '\0'))
                        CreateSignature(cert, chain, crlList, ocspClient, tsaClient, stamper, reader, sigLocation, true);
                    break;
                case SignatureRules.SignatureLocation.UpperRight:
                    using (var stamper = PdfStamper.CreateSignature(reader, fs, '\0', null, true))
                        CreateSignature(cert, chain, crlList, ocspClient, tsaClient, stamper, reader, sigLocation, false);
                    break;
            }
        }

        private static void CreateSignature(
            X509Certificate2 cert, 
            List<X509Certificate> chain, 
            ICollection<ICrlClient> crlList, 
            IOcspClient ocspClient,
            ITSAClient tsaClient, 
            PdfStamper stamper,
            PdfReader reader,
            SignatureRules.SignatureLocation sigLocation,
            bool provideCertificationLevel)
        {
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = "REASON";
            appearance.Location = "LOCATION";
            appearance.Contact = "CONTACT";
            if (provideCertificationLevel)
                appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS;
            var rect = GetSignatureRect(reader, sigLocation);
            var sigName = SignatureRules.SignatureName[sigLocation]; 
            appearance.SetVisibleSignature(rect, 1, sigName);

            IExternalSignature pks = new X509Certificate2Signature(cert, DigestAlgorithms.SHA256);
            MakeSignature.SignDetached(appearance, pks, chain, crlList, ocspClient, tsaClient, 0, CryptoStandard.CMS);
        }

        private static Rectangle GetSignatureRect(
            PdfReader reader,
            SignatureRules.SignatureLocation sigLocation)
        {
            var pageRect = reader.GetPageSize(1);
            if (sigLocation == SignatureRules.SignatureLocation.UpperLeft)
            {
                var signatureRect = new Rectangle(10, pageRect.Height - 60, 200, pageRect.Height - 10);
                return signatureRect;
            }
            else
            {
                var signatureRect = new Rectangle(pageRect.Width - 200, pageRect.Height - 60, pageRect.Width - 10, pageRect.Height - 10);
                return signatureRect;
            }
        }

        public static Task SignPdfAsync(
            string src,
            string dst,
            X509Certificate2 cert,
            List<X509Certificate> chain,
            ICollection<ICrlClient> crlList,
            IOcspClient ocspClient,
            ITSAClient tsaClient,
            SignatureRules.SignatureLocation sigLocation)
        {
            return Task.Run(() => 
                SignPdf(src, dst, cert, chain, crlList, ocspClient, tsaClient, sigLocation)
            );
        }
    }
}
