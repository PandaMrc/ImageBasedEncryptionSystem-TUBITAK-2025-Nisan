using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_AesEncrypt
    {
        // AES şifreleme için anahtar ve IV boyutları
        private const int KeySize = 256;
        private const int BlockSize = 128;

        // Varsayılan salt değeri (sabit)
        private static readonly byte[] DefaultSalt = new byte[16] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x20, 0x32, 0x33 };

        // En son oluşturulan AES anahtarını sakla
        private byte[] _lastAesKey = null;
        private byte[] _lastIvBytes = null;

        /// <summary>
        /// Verilen paroladan AES anahtarı ve IV oluşturur
        /// </summary>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <param name="keyBytes">Oluşturulan anahtar</param>
        /// <param name="ivBytes">Oluşturulan IV</param>
        public void GenerateKeyFromPassword(string password, out byte[] keyBytes, out byte[] ivBytes)
        {
            GenerateKeyFromPassword(password, DefaultSalt, out keyBytes, out ivBytes);
        }
        
        /// <summary>
        /// Verilen paroladan ve salt değerinden AES anahtarı ve IV oluşturur
        /// </summary>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <param name="salt">Tuzlama değeri</param>
        /// <param name="keyBytes">Oluşturulan anahtar</param>
        /// <param name="ivBytes">Oluşturulan IV</param>
        public void GenerateKeyFromPassword(string password, byte[] salt, out byte[] keyBytes, out byte[] ivBytes)
        {
            // Parola kontrolü
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(Errors.ERROR_PASSWORD_EMPTY);
                
            if (password.StartsWith(" "))
                throw new ArgumentException(Errors.ERROR_PASSWORD_SPACE);
                
            if (password.Length < 3 || password.Length > 11)
                throw new ArgumentException(Errors.ERROR_PASSWORD_LENGTH_RANGE);

            // Salt kontrolü
            if (salt == null || salt.Length < 8)
                salt = DefaultSalt;

            try
            {
                using (var derivation = new Rfc2898DeriveBytes(password, salt, 1000))
                {
                    keyBytes = derivation.GetBytes(KeySize / 8); // 256 bit = 32 byte
                    ivBytes = derivation.GetBytes(BlockSize / 8); // 128 bit = 16 byte
                }

                // Son oluşturulan anahtarı sakla
                _lastAesKey = new byte[keyBytes.Length];
                _lastIvBytes = new byte[ivBytes.Length];
                Buffer.BlockCopy(keyBytes, 0, _lastAesKey, 0, keyBytes.Length);
                Buffer.BlockCopy(ivBytes, 0, _lastIvBytes, 0, ivBytes.Length);
            }
            catch (Exception)
            {
                throw new CryptographicException(Errors.ERROR_AES_KEY_GENERATION);
            }
        }

        /// <summary>
        /// Metni AES ile şifreler
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <returns>Şifrelenmiş metin byte dizisi</returns>
        public byte[] EncryptText(string plainText, string password)
        {
            // Metin kontrolü
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException(Errors.ERROR_TEXT_EMPTY);
                
            if (plainText.StartsWith(" "))
                throw new ArgumentException(Errors.ERROR_TEXT_SPACE);
                
            if (plainText.Length < 3)
                throw new ArgumentException(Errors.ERROR_TEXT_LENGTH_MIN);
                
            if (plainText.Length > 10000)
                throw new ArgumentException(Errors.ERROR_TEXT_LENGTH_MAX);

            // Parola kontrolü
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(Errors.ERROR_PASSWORD_EMPTY);
                
            if (password.StartsWith(" "))
                throw new ArgumentException(Errors.ERROR_PASSWORD_SPACE);
                
            if (password.Length < 3 || password.Length > 11)
                throw new ArgumentException(Errors.ERROR_PASSWORD_LENGTH_RANGE);

            byte[] keyBytes, ivBytes;
            GenerateKeyFromPassword(password, out keyBytes, out ivBytes);

            byte[] encrypted;

            try
            {
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;

                    ICryptoTransform encryptor = aes.CreateEncryptor();

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                return encrypted;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                    throw;
                throw new CryptographicException(Errors.ERROR_AES_ENCRYPT);
            }
        }

        /// <summary>
        /// Metni AES ile şifreler ve Base64 formatında döndürür
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <returns>Şifrelenmiş metin (Base64 formatında)</returns>
        public string EncryptTextToBase64(string plainText, string password)
        {
            byte[] encryptedBytes = EncryptText(plainText, password);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// En son oluşturulan AES anahtarını döndürür
        /// </summary>
        /// <returns>Son kullanılan AES anahtarı ve IV'nin birleşimi</returns>
        public byte[] GetLastAesKey()
        {
            if (_lastAesKey == null || _lastIvBytes == null)
            {
                throw new InvalidOperationException(Errors.ERROR_AES_KEY_GENERATION);
            }

            // Anahtar ve IV'yi birleştir
            byte[] combinedKey = new byte[_lastAesKey.Length + _lastIvBytes.Length];
            Buffer.BlockCopy(_lastAesKey, 0, combinedKey, 0, _lastAesKey.Length);
            Buffer.BlockCopy(_lastIvBytes, 0, combinedKey, _lastAesKey.Length, _lastIvBytes.Length);

            return combinedKey;
        }

        /// <summary>
        /// AES anahtarı ve IV'yi birleştirerek döndürür
        /// </summary>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <returns>Anahtar ve IV'nin birleştirilmiş hali</returns>
        public byte[] GetKey(string password)
        {
            byte[] keyBytes, ivBytes;
            GenerateKeyFromPassword(password, out keyBytes, out ivBytes);

            // Anahtar ve IV'yi birleştir
            byte[] combinedKey = new byte[keyBytes.Length + ivBytes.Length];
            Buffer.BlockCopy(keyBytes, 0, combinedKey, 0, keyBytes.Length);
            Buffer.BlockCopy(ivBytes, 0, combinedKey, keyBytes.Length, ivBytes.Length);

            return combinedKey;
        }
    }
} 