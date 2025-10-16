using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf.security;

namespace wpfcm1.Certificates
{
    internal class CngExternalSignature : IExternalSignature
    {
        private readonly X509Certificate2 _certificate;
        private readonly string _hashAlgorithm;
        private readonly string _encryptionAlgorithm;

        public CngExternalSignature(X509Certificate2 certificate, string hashAlgorithm)
        {
            _certificate = certificate;
            _hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));
            _encryptionAlgorithm = "RSA";
        }

        public string GetEncryptionAlgorithm() => _encryptionAlgorithm;

        public string GetHashAlgorithm() => _hashAlgorithm;

        public byte[] Sign(byte[] message)
        {
            if (_certificate.HasPrivateKey)
            {
                RSA rsaKey = _certificate.GetRSAPrivateKey();
                if (rsaKey != null)
                {
                    var hashName = MapToHashAlgorithmName(_hashAlgorithm);

                    byte[] hash;
                    using (var hasher = HashAlgorithm.Create(hashName.Name))
                    {
                        if (hasher == null)
                            throw new InvalidOperationException($"Unsupported hash algorithm: {hashName.Name}");
                        hash = hasher.ComputeHash(message);
                    }

                    // Sign the hash using the RSA key
                    //return rsaKey.SignHash(message, hashName, RSASignaturePadding.Pkcs1);
                    return rsaKey.SignHash(hash, hashName, RSASignaturePadding.Pkcs1);
                }

                // --- DSA/ECDSA KEY HANDLING (for completeness, though less common for PDF) ---
                ECDsa ecdsaKey = _certificate.GetECDsaPrivateKey();
                if (ecdsaKey != null)
                {
                    // iTextSharp's PrivateKeySignature handles ECC/DSA keys differently.
                    // For robust signing, you might need to use BouncyCastle's ECDsaSigner 
                    // implementation or ensure the key is exported/imported correctly, 
                    // but for RSA (the most common) the above should work.
                    throw new NotSupportedException("ECDsa/DSA keys require a different iTextSharp signature implementation.");
                }

                throw new CryptographicException("The private key could not be extracted as RSA or ECDsa.");
            }

            throw new CryptographicException("Certificate does not contain a private key.");
        }

        private static HashAlgorithmName MapToHashAlgorithmName(string digest)
        {
            switch (digest.ToUpperInvariant())
            {
                case "SHA1": return HashAlgorithmName.SHA1;
                case "SHA-1": return HashAlgorithmName.SHA1;
                case "SHA256": return HashAlgorithmName.SHA256;
                case "SHA-256": return HashAlgorithmName.SHA256;
                case "SHA384": return HashAlgorithmName.SHA384;
                case "SHA-384": return HashAlgorithmName.SHA384;
                case "SHA512": return HashAlgorithmName.SHA512;
                case "SHA-512": return HashAlgorithmName.SHA512;
                default:
                    throw new ArgumentException($"Unsupported digest algorithm: {digest}");
            }
        }
    }
}
