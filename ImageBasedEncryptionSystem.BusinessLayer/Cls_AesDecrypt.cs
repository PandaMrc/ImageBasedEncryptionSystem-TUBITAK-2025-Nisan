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
    public class Cls_AesDecrypt
    {
        // AES şifreleme için anahtar ve IV boyutları
        private const int KeySize = 256;
        private const int BlockSize = 128;
        
        // Varsayılan salt değeri (sabit)
        private static readonly byte[] DefaultSalt = new byte[16] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76, 0x20, 0x32, 0x33 };

        /// <summary>
        /// Verilen paroladan AES anahtarı ve IV oluşturur
        /// </summary>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <param name="keyBytes">Oluşturulan anahtar</param>
        /// <param name="ivBytes">Oluşturulan IV</param>
        private void GenerateKeyFromPassword(string password, out byte[] keyBytes, out byte[] ivBytes)
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
        private void GenerateKeyFromPassword(string password, byte[] salt, out byte[] keyBytes, out byte[] ivBytes)
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
            }
            catch (Exception)
            {
                throw new CryptographicException(Errors.ERROR_AES_KEY_GENERATION);
            }
        }

        /// <summary>
        /// AES ile şifrelenmiş metni çözer
        /// </summary>
        /// <param name="encryptedText">Base64 formatında şifrelenmiş metin</param>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <returns>Çözülmüş metin</returns>
        public string DecryptText(string encryptedText, string password)
        {
            // Şifreli metin kontrolü
            if (string.IsNullOrEmpty(encryptedText))
                throw new ArgumentException(Errors.ERROR_DATA_NO_HIDDEN);

            // Parola kontrolü
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(Errors.ERROR_PASSWORD_EMPTY);
                
            if (password.StartsWith(" "))
                throw new ArgumentException(Errors.ERROR_PASSWORD_SPACE);
                
            if (password.Length < 3 || password.Length > 11)
                throw new ArgumentException(Errors.ERROR_PASSWORD_LENGTH_RANGE);

            try
            {
                // Paroladan anahtar ve IV oluştur
                byte[] keyBytes, ivBytes;
                GenerateKeyFromPassword(password, out keyBytes, out ivBytes);

                // Base64 formatındaki metni byte dizisine çevir
                byte[] cipherBytes = Convert.FromBase64String(encryptedText);

                string plaintext = null;

                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    
                    try
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (CryptographicException)
                    {
                        throw new CryptographicException(Errors.ERROR_PASSWORD_WRONG);
                    }
                }

                return plaintext;
            }
            catch (FormatException)
            {
                throw new CryptographicException(Errors.ERROR_DATA_CORRUPTED);
            }
            catch (CryptographicException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CryptographicException(Errors.ERROR_AES_DECRYPT);
            }
        }

        /// <summary>
        /// Özel anahtar ve IV ile şifrelenmiş metni çözer
        /// </summary>
        /// <param name="encryptedTextBytes">Şifrelenmiş metin byte dizisi</param>
        /// <param name="aesKey">AES anahtarı ve IV'nin birleştirilmiş hali</param>
        /// <returns>Çözülmüş metin</returns>
        public string DecryptText(byte[] encryptedTextBytes, byte[] aesKey)
        {
            if (encryptedTextBytes == null || encryptedTextBytes.Length == 0)
                throw new ArgumentException(Errors.ERROR_DATA_NO_HIDDEN);
                
            if (aesKey == null || aesKey.Length == 0)
                throw new ArgumentException(Errors.ERROR_AES_KEY_GENERATION);

            try
            {
                // Birleştirilmiş anahtarı ayır (ilk 32 bayt anahtar, sonraki 16 bayt IV)
                byte[] keyBytes = new byte[KeySize / 8]; // 32 bayt
                byte[] ivBytes = new byte[BlockSize / 8]; // 16 bayt
                
                if (aesKey.Length != keyBytes.Length + ivBytes.Length)
                    throw new ArgumentException(Errors.ERROR_AES_IV_INVALID);
                    
                Buffer.BlockCopy(aesKey, 0, keyBytes, 0, keyBytes.Length);
                Buffer.BlockCopy(aesKey, keyBytes.Length, ivBytes, 0, ivBytes.Length);

                string plaintext = null;

                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    
                    try
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(encryptedTextBytes))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (CryptographicException)
                    {
                        throw new CryptographicException(Errors.ERROR_PASSWORD_WRONG);
                    }
                }

                return plaintext;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is CryptographicException)
                    throw;
                throw new CryptographicException(Errors.ERROR_AES_DECRYPT);
            }
        }
        
        /// <summary>
        /// Şifreli veriyi çözer ve byte dizisi olarak döndürür (ExtractAndDecryptText için gerekli)
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veriler</param>
        /// <param name="aesKey">AES anahtarı</param>
        /// <param name="errorMessage">Hata mesajı</param>
        /// <returns>Çözülen veri</returns>
        public byte[] DecryptData(byte[] encryptedData, byte[] aesKey, out string errorMessage)
        {
            errorMessage = string.Empty;
            
            if (encryptedData == null || encryptedData.Length == 0)
            {
                errorMessage = Errors.ERROR_DATA_NO_HIDDEN;
                return null;
            }
            
            if (aesKey == null || aesKey.Length == 0)
            {
                errorMessage = Errors.ERROR_AES_KEY_GENERATION;
                return null;
            }
            
            try
            {
                // Birleştirilmiş anahtarı ayır (ilk 32 bayt anahtar, sonraki 16 bayt IV)
                byte[] keyBytes = new byte[KeySize / 8]; // 32 bayt
                byte[] ivBytes = new byte[BlockSize / 8]; // 16 bayt
                
                if (aesKey.Length != keyBytes.Length + ivBytes.Length)
                {
                    errorMessage = Errors.ERROR_AES_IV_INVALID;
                    return null;
                }
                    
                Buffer.BlockCopy(aesKey, 0, keyBytes, 0, keyBytes.Length);
                Buffer.BlockCopy(aesKey, keyBytes.Length, ivBytes, 0, ivBytes.Length);
                
                byte[] decryptedData;
                
                using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    
                    ICryptoTransform decryptor = aes.CreateDecryptor();
                    
                    try
                    {
                        using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                        {
                            using (MemoryStream msPlain = new MemoryStream())
                            {
                                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    // Veriyi oku ve belleğe aktar
                                    byte[] buffer = new byte[1024];
                                    int bytesRead;
                                    
                                    while ((bytesRead = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        msPlain.Write(buffer, 0, bytesRead);
                                    }
                                    
                                    decryptedData = msPlain.ToArray();
                                }
                            }
                        }
                        
                        return decryptedData;
                    }
                    catch (CryptographicException)
                    {
                        errorMessage = Errors.ERROR_PASSWORD_WRONG;
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Şifreli metni çözmeyi dener
        /// </summary>
        /// <param name="encryptedText">Base64 formatında şifrelenmiş metin</param>
        /// <param name="password">Kullanıcının girdiği parola</param>
        /// <param name="decryptedText">Çözülen metin</param>
        /// <param name="errorMessage">Hata mesajı</param>
        /// <returns>İşlem başarılı ise true</returns>
        public bool TryDecryptText(string encryptedText, string password, out string decryptedText, out string errorMessage)
        {
            decryptedText = string.Empty;
            errorMessage = string.Empty;
            
            try
            {
                decryptedText = DecryptText(encryptedText, password);
                return true;
            }
            catch (CryptographicException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            catch (Exception)
            {
                errorMessage = Errors.ERROR_AES_DECRYPT;
                return false;
            }
        }
    }
} 