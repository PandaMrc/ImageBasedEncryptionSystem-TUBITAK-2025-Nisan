using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ImageBasedEncryptionSystem.BusinessLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// RSA ile şifreleme işlemleri yapan sınıf
    /// </summary>
    public class Cls_RsaEncrypt
    {
        private readonly Cls_RsaKey _rsaKey;

        public string PublicKey => _rsaKey.PublicKey;
        public string PrivateKey => _rsaKey.PrivateKey;

        public Cls_RsaEncrypt()
        {
            _rsaKey = new Cls_RsaKey();
        }

        /// <summary>
        /// RSA algoritması ile veriyi şifreler
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <returns>Şifrelenmiş metin (Base64 formatında)</returns>
        public string Encrypt(string plainText)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    return string.Empty;

                // Metni byte dizisine çevir
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);

                // RSA şifreleme için public key'i kullan
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072))
                {
                    // Public key'i içe aktar
                    rsa.FromXmlString(PublicKey);

                    // Şifreleme işlemi
                    byte[] encryptedData = rsa.Encrypt(dataToEncrypt, true);

                    // Şifrelenmiş veriyi Base64 olarak döndür
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (CryptographicException ce)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Şifreleme Hatası (Kriptografik): {ce.Message}");
                throw new CryptographicException($"RSA şifreleme işlemi sırasında kriptografik bir hata oluştu: {ce.Message}", ce);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Şifreleme Hatası (Genel): {ex.Message}");
                throw new Exception($"RSA şifreleme işlemi sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Public anahtarı dışa aktarır
        /// </summary>
        /// <returns>Public anahtar XML formatında</returns>
        public string ExportPublicKey()
        {
            return PublicKey;
        }

        /// <summary>
        /// Private anahtarı dışa aktarır
        /// </summary>
        /// <returns>Private anahtar XML formatında</returns>
        public string ExportPrivateKey()
        {
            return PrivateKey;
        }
    }
}
