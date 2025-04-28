using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// AES ile şifreleme işlemleri yapan sınıf
    /// </summary>
    public class Cls_AesEncrypt
    {
        public string Key { get; private set; }
        public string IV { get; private set; }

        /// <summary>
        /// AES algoritması ile veriyi şifreler
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <param name="password">Şifreleme için kullanılacak parola</param>
        /// <returns>Şifrelenmiş metin (Base64 formatında)</returns>
        public string Encrypt(string plainText, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    return string.Empty;

                if (string.IsNullOrEmpty(password))
                    return string.Empty;

                // Parola kullanarak rastgele 256 bitlik anahtar üret
                byte[] keyBytes;
                byte[] ivBytes;
                GenerateKeyFromPassword(password, out keyBytes, out ivBytes);

                // Key ve IV'yi Base64 string olarak sakla
                Key = Convert.ToBase64String(keyBytes);
                IV = Convert.ToBase64String(ivBytes);

                // AES şifreleme işlemi
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = ivBytes;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Şifreleme dönüştürücüsü oluştur
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Şifreleme işlemi için bellek akışı kullan
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Metni yaz
                                swEncrypt.Write(plainText);
                            }
                            // Şifrelenmiş veriyi Base64 olarak döndür
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AES Şifreleme Hatası: {ex.Message}");
                throw new CryptographicException($"AES şifreleme işlemi sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Paroladan 256 bitlik AES anahtarı ve 128 bitlik IV oluşturur
        /// </summary>
        /// <param name="password">Kullanıcı parolası</param>
        /// <param name="key">Üretilen anahtar</param>
        /// <param name="iv">Üretilen IV</param>
        private void GenerateKeyFromPassword(string password, out byte[] key, out byte[] iv)
        {
            try
            {
                // Sabit tuz değeri - gerçek uygulamalarda güvenli bir şekilde saklanmalıdır
                byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

                // Rfc2898DeriveBytes ile parola tabanlı anahtar türetme
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    key = deriveBytes.GetBytes(32); // 256 bit
                    iv = deriveBytes.GetBytes(16);  // 128 bit
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Anahtar Türetme Hatası: {ex.Message}");
                throw new CryptographicException($"Anahtar türetme işlemi sırasında bir hata oluştu: {ex.Message}", ex);
            }
        }
    }
}
