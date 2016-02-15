using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text;
using wpfcm1.Certificates;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.PDF
{
    public class PdfHelpers
    {
        //public static Tuple<MatchCollection, MatchCollection> ExtractText(string path)
        //{
        //    using (var reader = new PdfReader(path))
        //    {
        //        var parser = new PdfReaderContentParser(reader);

        //        ITextExtractionStrategy strategy = parser.ProcessContent(1, new LocationTextExtractionStrategy());
        //        var text = strategy.GetResultantText();

        //        var regexPib = new Regex(@"\b\d{9}\b", RegexOptions.Multiline);
        //        var matchesPib = regexPib.Matches(text);
        //        var regexNo = new Regex(@"\b(\w*(-\w*)*(\/\d+))\b", RegexOptions.Multiline);
        //        var matchesNo = regexNo.Matches(text);

        //        return Tuple.Create(matchesPib, matchesNo);
        //    }
        //}

        //public static Task<Tuple<MatchCollection, MatchCollection>> ExtractTextAsync(string path)
        //{
        //    return Task.Run(() => ExtractText(path));
        //}

        public static Tuple<string, string> ExtractText(string path)
        {
            using (var reader = new PdfReader(path))
            {
                Rectangle rectPib = new Rectangle(240, 615, 295, 630)
                {
                    Border = Rectangle.BOX,
                    BorderColor = BaseColor.RED,
                    BorderWidth = 1
                };

                //Rectangle rectNo = new Rectangle(476, 680, 570, 735)
                Rectangle rectNo = new Rectangle(526, 680, 570, 705)
                {
                    Border = Rectangle.BOX,
                    BorderColor = BaseColor.RED,
                    BorderWidth = 1
                };
                RenderFilter filterPib = new RegionTextRenderFilter(rectPib);
                ITextExtractionStrategy strategyPib = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterPib);

                RenderFilter filterNo = new RegionTextRenderFilter(rectNo);
                ITextExtractionStrategy strategyNo = new FilteredTextRenderListener(new LocationTextExtractionStrategy(), filterNo);

                var textPib = PdfTextExtractor.GetTextFromPage(reader, 1, strategyPib);
                var textNo = PdfTextExtractor.GetTextFromPage(reader, 1, strategyNo);

                return Tuple.Create(textPib, textNo);
            }
        }

        public static Task<Tuple<string, string>> ExtractTextAsync(string path)
        {
            return Task.Run(() => ExtractText(path));
        }

        public static bool ValidatePdfCertificates(string path)
        {
            bool isValid = true;
            using (var reader = new PdfReader(path))
            {
                AcroFields fields = reader.AcroFields;
                try
                {
                    CheckIntegrity(fields);
                    CheckSignatures(fields);
                }
                catch
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public static Task<bool> ValidatePdfCertificatesAsync(string path)
        {
            return Task.Run(() => ValidatePdfCertificates(path));
        }

        private static void CheckIntegrity(AcroFields fields)
        {
            bool coversDoc = false;
            bool integrityOk = false;
            foreach (var sigName in fields.GetSignatureNames())
            {
                coversDoc = fields.SignatureCoversWholeDocument(sigName);
                PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
                integrityOk = pkcs7.Verify();
            }
            if (!(coversDoc && integrityOk))
                throw new Exception();
        }

        private static void CheckSignatures(AcroFields fields)
        {
            foreach (var sigName in fields.GetSignatureNames())
            {
                PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
                var tsOk = pkcs7.VerifyTimestampImprint();
                if (!tsOk) throw new Exception();

                var c2 = new X509Certificate2();
                c2.Import(pkcs7.SigningCertificate.GetEncoded());
                var c2chain = CertificateHelpers.GetChain(c2);
                var errors = CertificateHelpers.CheckCertificate(c2, c2chain);
                if (errors.Count > 0) throw new Exception();

                var cert = pkcs7.SigningCertificate;
                DateTime signDate = pkcs7.SignDate;
                cert.CheckValidity(signDate);
                cert.CheckValidity();

                X509Certificate[] certs = pkcs7.SignCertificateChain;
                X509Certificate signCert = certs[0];
                X509Certificate issuerCert = (certs.Length > 1 ? certs[1] : null);

                CheckRevocation(pkcs7, signCert, issuerCert, signDate);
                CheckRevocation(pkcs7, signCert, issuerCert, DateTime.Now);
            }
        }

        private static void CheckRevocation(PdfPKCS7 pkcs7, X509Certificate signCert, X509Certificate issuerCert, DateTime date)
        {
            var ocsps = new List<BasicOcspResp>();
            if (pkcs7.Ocsp != null)
                ocsps.Add(pkcs7.Ocsp);
            var ocspVerifier = new OcspVerifier(null, ocsps);
            List<VerificationOK> verification = ocspVerifier.Verify(signCert, issuerCert, date);
            if (verification.Count == 0)
            {
                var crls = new List<X509Crl>();
                if (pkcs7.CRLs != null)
                    crls.AddRange(pkcs7.CRLs);
                var crlVerifier = new CrlVerifier(null, crls);
                verification = crlVerifier.Verify(signCert, issuerCert, date);
                verification.AddRange(verification);
            }
            if (verification.Count == 0)
                throw  new Exception();
        }
    }
}
