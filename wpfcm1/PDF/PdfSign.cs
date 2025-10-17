using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using wpfcm1.Certificates;
using wpfcm1.Settings;
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
            SignatureRules.SignatureLocation sigLocation,
            string reason = "")
        {
            PdfReader.unethicalreading = true;
            using (var reader = new PdfReader(src))
            using (var fs = new FileStream(dst, FileMode.Create))

                switch (sigLocation)
                {
                    case SignatureRules.SignatureLocation.UpperLeft:
                        using (var stamper = PdfStamper.CreateSignature(reader, fs, '\0'))
                        {
                            CreateSignature(cert, chain, crlList, ocspClient, tsaClient, stamper, reader, sigLocation, true, reason);
                        }
                        break;
                    case SignatureRules.SignatureLocation.UpperRight:
                        using (var stamper = PdfStamper.CreateSignature(reader, fs, '\0', null, true))
                        {
                            CreateSignature(cert, chain, crlList, ocspClient, tsaClient, stamper, reader, sigLocation, false, reason);
                        }
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
            bool provideCertificationLevel,
            string reason)
        {
            PdfSignatureAppearance appearance = stamper.SignatureAppearance;
            appearance.Reason = reason;
            //appearance.Location = location;
            //appearance.Contact = contact;
            if (provideCertificationLevel)
                appearance.CertificationLevel = PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS;
            var rect = GetSignatureRect(reader, sigLocation);
            var sigName = SignatureRules.SignatureName[sigLocation];
            appearance.SetVisibleSignature(rect, 1, sigName);

            // podrška za YU i Cir slova
            string path = System.Environment.GetEnvironmentVariable("SystemRoot") + @"\fonts\Arial.ttf";
            Font font = FontFactory.GetFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            appearance.Layer2Font = new Font(font);

            IExternalSignature pks;
            if (cert.SignatureAlgorithm.FriendlyName == "sha1RSA" || cert.IssuerName.Name == "C=BA, S=Republika Srpska, O=Poreska uprava, CN=PURS CA 1")
            {
                pks = new X509Certificate2Signature(cert, DigestAlgorithms.SHA1);
            }
            else
            {
                //pks = new X509Certificate2Signature(cert, DigestAlgorithms.SHA256);
                pks = new CngExternalSignature(cert, "SHA-256");
            }

            MakeSignature.SignDetached(appearance, pks, chain, crlList, ocspClient, tsaClient, 0, CryptoStandard.CADES);
        }

        private static Rectangle GetSignatureRect(
            PdfReader reader,
            SignatureRules.SignatureLocation sigLocation)
        {
            var pageRect = reader.GetPageSize(1);
            var pageRotation = reader.GetPageRotation(1);

            var sigHeight = 50;
            var sigWidth = 190;

            // TODO: staviti provere da ne izađemo iz vidljivog dela strane

            // ako je portrait:
            if (pageRotation == 0)
            {
                if (sigLocation == SignatureRules.SignatureLocation.UpperLeft)
                {
                    //var signatureRect = new Rectangle(10, pageRect.Height - 60, 200, pageRect.Height - 10);
                    var x = User.Default.XSigShift + 10;
                    var y = pageRect.Height - User.Default.YSigShift - 60;
                    var xx = x + sigWidth;
                    var yy = y + sigHeight;

                    var signatureRect = new Rectangle(x, y, xx, yy);
                    // Za Korporion urađena dorada, spušten potpis na dno strane: var signatureRect = new Rectangle(360, 60, 550, 110);
                    return signatureRect;
                }
                else
                {
                    //var signatureRect = new Rectangle(pageRect.Width - 200, pageRect.Height - 60, pageRect.Width - 10, pageRect.Height - 10);
                    var x = pageRect.Width - User.Default.XSigShiftRight - 200;
                    var y = pageRect.Height - User.Default.YSigShiftRight - 60;
                    var xx = x + sigWidth;
                    var yy = y + sigHeight;
                    var signatureRect = new Rectangle(x, y, xx, yy);
                    return signatureRect;
                }
            }
            // ako je landscape:
            else if (pageRotation == 90)
            {
                if (sigLocation == SignatureRules.SignatureLocation.UpperLeft)
                {
                    //var signatureRect = new Rectangle(10, pageRect.Width - 60, 200, pageRect.Width - 10);
                    var x = User.Default.XSigShift + 10;
                    var y = pageRect.Width - User.Default.YSigShift - 60;
                    var xx = x + sigWidth;
                    var yy = y + sigHeight;

                    var signatureRect = new Rectangle(x, y, xx, yy);
                    return signatureRect;
                }
                else
                {
                    //var signatureRect = new Rectangle(pageRect.Height - 200, pageRect.Width - 60, pageRect.Height - 10, pageRect.Width - 10);
                    var x = pageRect.Height - User.Default.XSigShiftRight - 200;
                    var y = pageRect.Width - User.Default.YSigShiftRight - 60;
                    var xx = x + sigWidth;
                    var yy = y + sigHeight;
                    var signatureRect = new Rectangle(x, y, xx, yy);
                    return signatureRect;
                }
            }
            // ako nije ni landscape ni portrait, stavi potpise bilo gde na strani gde ce se uvek videti:
            else
            {
                if (sigLocation == SignatureRules.SignatureLocation.UpperLeft)
                {
                    var signatureRect = new Rectangle(10, 10, 200, 200);
                    return signatureRect;
                }
                else
                {
                    var signatureRect = new Rectangle(10, 300, 200, 500);
                    return signatureRect;
                }
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
            SignatureRules.SignatureLocation sigLocation,
            string reason = "")
        {
            return Task.Run(() =>
                SignPdf(src, dst, cert, chain, crlList, ocspClient, tsaClient, sigLocation, reason)
            );
        }
    }
}
