using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// RSA algoritması ile şifre çözme işlemleri yapan sınıf
    /// </summary>
    public class Cls_RsaDecrypt
    {
        private string _privateKey;

        /// <summary>
        /// RSA ile şifre çözme sınıfı varsayılan oluşturucu
        /// </summary>
        public Cls_RsaDecrypt()
        {
            // Boş constructor
        }

        /// <summary>
        /// RSA şifre çözme sınıfını bir RSA şifreleme sınıfıyla ilişkilendirerek oluşturur
        /// </summary>
        /// <param name="rsaEncrypt">İlişkilendirilecek RSA şifreleme sınıfı</param>
        public Cls_RsaDecrypt(Cls_RsaEncrypt rsaEncrypt)
        {
            if (rsaEncrypt != null && !string.IsNullOrEmpty(rsaEncrypt.PrivateKey))
            {
                _privateKey = rsaEncrypt.PrivateKey;
            }
        }

        /// <summary>
        /// Private anahtarı ayarlar
        /// </summary>
        /// <param name="privateKey">XML formatında RSA private anahtarı</param>
        public void SetPrivateKey(string privateKey)
        {
            if (!string.IsNullOrEmpty(privateKey))
            {
                _privateKey = privateKey;
            }
        }

        /// <summary>
        /// RSA ile şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="cipherText">Şifrelenmiş metin (Base64 formatında)</param>
        /// <returns>Orijinal metin</returns>
        public string Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText))
                    return string.Empty;

                if (string.IsNullOrEmpty(_privateKey))
                    throw new CryptographicException("RSA private anahtarı ayarlanmamış. Lütfen SetPrivateKey metodunu kullanın veya yapılandırıcıda bir Cls_RsaEncrypt nesnesi sağlayın.");

                // Şifreli metni byte dizisine çevir
                byte[] dataToDecrypt = Convert.FromBase64String(cipherText);

                // RSA şifre çözme için private key'i kullan
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // Private key'i içe aktar
                    rsa.FromXmlString(_privateKey);

                    // Şifre çözme işlemi
                    byte[] decryptedData = rsa.Decrypt(dataToDecrypt, true);

                    // Çözülmüş veriyi metin olarak döndür
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (CryptographicException ce)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Şifre Çözme Hatası (Kriptografik): {ce.Message}");
                throw new CryptographicException($"RSA şifre çözme işlemi sırasında kriptografik bir hata oluştu: {ce.Message}", ce);
            }
            catch (FormatException fe)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Şifre Çözme Hatası (Format): {fe.Message}");
                throw new FormatException("Şifreli metin geçerli bir Base64 formatında değil", fe);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Şifre Çözme Hatası (Genel): {ex.Message}");
                throw new Exception($"RSA şifre çözme işlemi sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }
    }
}
