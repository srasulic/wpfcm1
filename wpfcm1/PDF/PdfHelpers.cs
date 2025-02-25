using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using wpfcm1.Certificates;
using wpfcm1.Model;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace wpfcm1.PDF
{
    public class PdfHelpers
    {
        //
        // 1.0 - Manipulacija tekstom
        //
        public static Tuple<string, string> ExtractText(string path, RecognitionPattern.Coordinates pibAtt, RecognitionPattern.Coordinates docAtt)
        {
            using (var reader = new PdfReader(path))
            {
                //  Rectangle rectPib = new Rectangle(User.Default.LlxPib, User.Default.LlyPib, User.Default.UrxPib, User.Default.UryPib)
                Rectangle rectPib = new Rectangle(pibAtt.x, pibAtt.y, pibAtt.xx, pibAtt.yy)
                {
                    Border = Rectangle.BOX,
                    BorderColor = BaseColor.RED,
                    BorderWidth = 1
                };

                Rectangle rectNo = new Rectangle(docAtt.x, docAtt.y, docAtt.xx, docAtt.yy)
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

        //
        // 1.1 - Utvrdi orjentaciju i rotaciju strane
        //
        public static Tuple<bool, int> ExtractOrientationRotation(string path)
        {
            bool isPortrait;
            int pageRotation;
            using (var reader = new PdfReader(path))
            {
                //  Rectangle rectPib = new Rectangle(User.Default.LlxPib, User.Default.LlyPib, User.Default.UrxPib, User.Default.UryPib)
                pageRotation = reader.GetPageRotation(1);


                var pageRect = reader.GetPageSize(1);
                if (pageRect.Height > pageRect.Width)
                {
                    isPortrait = true;
                }
                else
                {
                    isPortrait = false;
                }

                return Tuple.Create(isPortrait, pageRotation);
            }
        }

        public static Task<Tuple<bool, int>> ExtractOrientationRotationAsync(string path)
        {
            return Task.Run(() => ExtractOrientationRotation(path));
        }

        public static Task<Tuple<string, string>> ExtractTextAsync(string path, RecognitionPattern.Coordinates pibAtt, RecognitionPattern.Coordinates docAtt)
        {
            return Task.Run(() => ExtractText(path, pibAtt, docAtt));
        }

        //
        // 2.0 - validacija sertifikata
        //
        public static Tuple<bool, string> ValidatePdfCertificatesWithInfo(string path)
        {
            bool isValid = true;
            string validationInfo = "";
            try
            {
                using (var reader = new PdfReader(path))
                {
                    AcroFields fields = reader.AcroFields;
                    try
                    {
                        CheckIntegrity(fields);
                        validationInfo = validationInfo + "OK: Integritet potpisa proveren. " + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        isValid = false;
                        validationInfo = validationInfo + "GREŠKA: Greška prilikom provere integriteta potpisa: " + e.Message + System.Environment.NewLine;
                    }

                    try
                    {
                        CheckTimestamp(fields);
                        validationInfo = validationInfo + "OK: Dokument potpisan Vremenskim žigom. " + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        validationInfo = validationInfo + "NAPOMENA: Vremenski žig ne postoji ili ga nije bilo moguće proveriti (" + e.Message + ")" + System.Environment.NewLine;
                    }

                    try
                    {
                        CheckSignatures(fields);
                        validationInfo = validationInfo + "OK: Svi potpisi na dokumentu su ispravni. " + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        isValid = false;
                        validationInfo = validationInfo + "GREŠKA: Greška prilikom provere integriteta potpisa: " + e.Message + System.Environment.NewLine;
                    }
                    // Dodoati LTV proveru!!!

                    //   PdfLtvVerifier verifier = new PdfLtvVerifier(reader);
                    //   List<VerificationOK> xxx = new List<VerificationOK>();
                    //   verifier.Verify(xxx);
                    //   if (xxx == null)
                    //   {
                    //       validationInfo = validationInfo + "Ovo je test ";
                    //   }

                }
            }
            catch (Exception e)
            {
                isValid = false;
                validationInfo = validationInfo + "GREŠKA: Neobradjena greška " + e.Message + System.Environment.NewLine;
            }
            return Tuple.Create(isValid, validationInfo);
        }

        public static PdfPKCS7 GetPcks7(string path, int position)
        {
            try
            {
                var reader = new PdfReader(path);
                AcroFields fields = reader.AcroFields;
                var i = 1;
                foreach (var sigName in fields.GetSignatureNames())
                {
                    if (i++ == position)
                    {
                        PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
                        return pkcs7;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // dodajemo Validate Pdf Cert funkciju koja će umeti da vrati <Tuple<bool, string>> gde će string biti dodatna informacija na temu sertifikata
        public static Task<Tuple<bool, string>> ValidatePdfCertificatesWithInfoAsync(string path)
        {
            return Task.Run(() => ValidatePdfCertificatesWithInfo(path));
        }

        public static Task<PdfPKCS7> GetPcks7Async(string path, int position)
        {
            return Task.Run(() => GetPcks7(path, position));
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

        private static void CheckTimestamp(AcroFields fields)
        {
            foreach (var sigName in fields.GetSignatureNames())
            {
                PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
                var tsOk = pkcs7.VerifyTimestampImprint();
                if (!tsOk) throw new Exception("Greška prilikom provere ugrađenog vremenskog žiga / Verification error - VerifyTimestampImprint check failed.");
            }
        }

        private static void CheckSignatures(AcroFields fields)
        {
            foreach (var sigName in fields.GetSignatureNames())
            {
                PdfPKCS7 pkcs7 = fields.VerifySignature(sigName);
                var c2 = new X509Certificate2();
                c2.Import(pkcs7.SigningCertificate.GetEncoded());
                var c2chain = CertificateHelpers.GetChain(c2);

                // Isključeno jer proverava da li su sertifikati validni u ovom trenutku
                // TODO: dodati validaciju na ispravan način!!!!!

                // var errors = CertificateHelpers.CheckCertificate(c2, c2chain);
                // if (errors.Count > 0) throw new Exception("Greška u lancu sertifikata. Proverite da li je instaliran root sertifikat u lancu / Certificate chain error. Check if You trust all certificates in chain.");

                // Exceptions:
                //   Org.BouncyCastle.Security.Certificates.CertificateExpiredException
                //   Org.BouncyCastle.Security.Certificates.CertificateNotYetValidException
                var cert = pkcs7.SigningCertificate;
                DateTime signDate = pkcs7.SignDate;
                cert.CheckValidity(signDate);


                // zasto nismo uzeli SigningCertificate iz prethodnog koraka?
                X509Certificate[] certs = pkcs7.SignCertificateChain;
                X509Certificate signCert = certs[0];
                X509Certificate issuerCert = (certs.Length > 1 ? certs[1] : null);

                // Od funkcije očekujemo da ako nije u redu da Exception("Nije bilo moguće proveriti da li je sertifikat opozvan / Certificate revocation check failed.");
                CheckRevocation(pkcs7, signCert, issuerCert, signDate);
            }
        }

        private static void CheckRevocation(PdfPKCS7 pkcs7, X509Certificate signCert, X509Certificate issuerCert, DateTime date)
        {
            List<BasicOcspResp> ocsps = new List<BasicOcspResp>();
            if (pkcs7.Ocsp != null)
                ocsps.Add(pkcs7.Ocsp);
            PdfOcspVerifier ocspVerifier = new PdfOcspVerifier(null, ocsps);
            // TODO: napraviti posebnu granu koja može da proveri i online ako nema embedovanih 
            //       ocsp i crl... U z napomenu da nije LTV potpis...
            ocspVerifier.OnlineCheckingAllowed = false;
            List<VerificationOK> verification = ocspVerifier.Verify(signCert, issuerCert, date.ToUniversalTime());
            if (verification.Count == 0)
            {
                var crls = new List<X509Crl>();
                if (pkcs7.CRLs != null)
                    crls.AddRange(pkcs7.CRLs);
                PdfCrlVerifier crlVerifier = new PdfCrlVerifier(null, crls);
                // TODO: napraviti posebnu granu koja može da proveri i online ako nema embedovanih 
                //       ocsp i crl... U z napomenu da nije LTV potpis...
                crlVerifier.OnlineCheckingAllowed = false;
                verification = crlVerifier.Verify(signCert, issuerCert, date.ToUniversalTime());
                verification.AddRange(verification);
            }
            if (verification.Count == 0)
                throw new Exception("Nije bilo moguće proveriti da li je sertifikat opozvan / Certificate revocation check failed.");
        }
    }
}
