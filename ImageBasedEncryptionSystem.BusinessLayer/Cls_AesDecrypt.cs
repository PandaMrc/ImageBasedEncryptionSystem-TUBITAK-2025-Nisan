using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_AesDecrypt
    {
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int Iterations = 10000;
        private string _lastErrorMessage = string.Empty;
        private string _lastSuccessMessage = string.Empty;

        /// <summary>
        /// Son hata mesajını döndürür
        /// </summary>
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
        }

        /// <summary>
        /// Son başarı mesajını döndürür
        /// </summary>
        public string LastSuccessMessage
        {
            get { return _lastSuccessMessage; }
        }

        /// <summary>
        /// AES algoritması ile şifrelenmiş metni çözer
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri (Salt + IV + Şifreli Metin)</param>
        /// <param name="password">Şifre çözme için kullanılacak parola</param>
        /// <returns>Çözülmüş metin</returns>
        public string DecryptText(byte[] encryptedData, string password)
        {
            try
            {
                // Parametre kontrolü
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return null;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Minimum gerekli uzunluk kontrolü (Salt + IV)
                int minLength = 16 + (BlockSize / 8);
                if (encryptedData.Length < minLength)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return null;
                }

                // Salt değerini çıkar
                byte[] salt = new byte[16];
                Buffer.BlockCopy(encryptedData, 0, salt, 0, 16);

                // IV değerini çıkar
                byte[] iv = new byte[BlockSize / 8];
                Buffer.BlockCopy(encryptedData, 16, iv, 0, BlockSize / 8);

                // Parola ve salt değerden anahtar türet
                byte[] key;
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    key = deriveBytes.GetBytes(KeySize / 8);
                }

                // Şifreli metni çıkar
                int encryptedTextLength = encryptedData.Length - 16 - (BlockSize / 8);
                byte[] encryptedText = new byte[encryptedTextLength];
                Buffer.BlockCopy(encryptedData, 16 + (BlockSize / 8), encryptedText, 0, encryptedTextLength);

                // AES algoritması ile çöz
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    try
                    {
                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        using (var ms = new MemoryStream(encryptedText))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs))
                        {
                            string decryptedText = sr.ReadToEnd();
                            _lastSuccessMessage = Success.DECRYPT_SUCCESS_AES;
                            return decryptedText;
                        }
                    }
                    catch (CryptographicException)
                    {
                        _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                        return null;
                    }
                }
            }
            catch (CryptographicException)
            {
                _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                return null;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES algoritması ile şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri (Salt + IV + Şifreli Veri)</param>
        /// <param name="password">Şifre çözme için kullanılacak parola</param>
        /// <returns>Çözülmüş veri</returns>
        public byte[] DecryptData(byte[] encryptedData, string password)
        {
            try
            {
                // Parametre kontrolü
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return null;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Minimum gerekli uzunluk kontrolü (Salt + IV)
                int minLength = 16 + (BlockSize / 8);
                if (encryptedData.Length < minLength)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return null;
                }

                // Salt değerini çıkar
                byte[] salt = new byte[16];
                Buffer.BlockCopy(encryptedData, 0, salt, 0, 16);

                // IV değerini çıkar
                byte[] iv = new byte[BlockSize / 8];
                Buffer.BlockCopy(encryptedData, 16, iv, 0, BlockSize / 8);

                // Parola ve salt değerden anahtar türet
                byte[] key;
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    key = deriveBytes.GetBytes(KeySize / 8);
                }

                // Şifreli veriyi çıkar
                int encryptedDataLength = encryptedData.Length - 16 - (BlockSize / 8);
                byte[] cipherBytes = new byte[encryptedDataLength];
                Buffer.BlockCopy(encryptedData, 16 + (BlockSize / 8), cipherBytes, 0, encryptedDataLength);

                return DecryptDataWithKeyIV(cipherBytes, key, iv);
            }
            catch (CryptographicException)
            {
                _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                return null;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES algoritması ile şifrelenmiş veriyi çözer, özel anahtar ve IV kullanır
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri</param>
        /// <param name="key">AES anahtarı</param>
        /// <param name="iv">Başlangıç vektörü</param>
        /// <returns>Çözülmüş veri</returns>
        public byte[] DecryptDataWithKeyIV(byte[] encryptedData, byte[] key, byte[] iv)
        {
            try
            {
                // Parametre kontrolü
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return null;
                }

                if (key == null || key.Length != KeySize / 8)
                {
                    _lastErrorMessage = Errors.ERROR_AES_KEY_GENERATION;
                    return null;
                }

                if (iv == null || iv.Length != BlockSize / 8)
                {
                    _lastErrorMessage = Errors.ERROR_AES_IV_INVALID;
                    return null;
                }

                // AES algoritması ile çöz
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    try
                    {
                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(new MemoryStream(encryptedData), decryptor, CryptoStreamMode.Read))
                            {
                                byte[] buffer = new byte[1024];
                                int bytesRead;
                                while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, bytesRead);
                                }
                            }

                            _lastSuccessMessage = Success.DECRYPT_SUCCESS_AES;
                            return ms.ToArray();
                        }
                    }
                    catch (CryptographicException)
                    {
                        _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                        return null;
                    }
                }
            }
            catch (CryptographicException)
            {
                _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                return null;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Şifrelenmiş bir dosyayı yükler
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Dosya içeriği (şifreli veri)</returns>
        public byte[] LoadEncryptedFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    _lastErrorMessage = Errors.ERROR_FILE_INVALID_PATH;
                    return null;
                }

                if (!File.Exists(filePath))
                {
                    _lastErrorMessage = Errors.ERROR_FILE_NOT_FOUND;
                    return null;
                }

                // Dosyayı oku
                byte[] encryptedData = File.ReadAllBytes(filePath);
                
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return null;
                }
                
                _lastSuccessMessage = Success.FILE_OPEN_SUCCESS;
                return encryptedData;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_FILE_OPEN, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Şifrelenmiş dosyayı yükler ve şifresini çözer
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>Çözülmüş dosya içeriği</returns>
        public byte[] LoadAndDecryptFile(string filePath, string password)
        {
            try
            {
                byte[] encryptedData = LoadEncryptedFile(filePath);
                if (encryptedData == null)
                {
                    // Hata mesajı zaten LoadEncryptedFile içinde ayarlandı
                    return null;
                }

                byte[] decryptedData = DecryptData(encryptedData, password);
                if (decryptedData == null)
                {
                    // Hata mesajı zaten DecryptData içinde ayarlandı
                    return null;
                }

                _lastSuccessMessage = Success.DECRYPT_SUCCESS_AES;
                return decryptedData;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Şifrelenmiş dosyadaki metni yükler ve şifresini çözer
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>Çözülmüş metin</returns>
        public string LoadAndDecryptTextFile(string filePath, string password)
        {
            try
            {
                byte[] encryptedData = LoadEncryptedFile(filePath);
                if (encryptedData == null)
                {
                    // Hata mesajı zaten LoadEncryptedFile içinde ayarlandı
                    return null;
                }

                string decryptedText = DecryptText(encryptedData, password);
                if (decryptedText == null)
                {
                    // Hata mesajı zaten DecryptText içinde ayarlandı
                    return null;
                }

                _lastSuccessMessage = Success.DECRYPT_SUCCESS_AES;
                return decryptedText;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Parolayı doğrular (şifrelenmiş veriyi çözmeyi deneyerek)
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri</param>
        /// <param name="password">Doğrulanacak parola</param>
        /// <returns>Parola doğru ise true, değilse false</returns>
        public bool ValidatePassword(byte[] encryptedData, string password)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return false;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return false;
                }

                // Minimum gerekli uzunluk kontrolü (Salt + IV)
                int minLength = 16 + (BlockSize / 8);
                if (encryptedData.Length < minLength)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return false;
                }

                // Salt değerini çıkar
                byte[] salt = new byte[16];
                Buffer.BlockCopy(encryptedData, 0, salt, 0, 16);

                // IV değerini çıkar
                byte[] iv = new byte[BlockSize / 8];
                Buffer.BlockCopy(encryptedData, 16, iv, 0, BlockSize / 8);

                // Parola ve salt değerden anahtar türet
                byte[] key;
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    key = deriveBytes.GetBytes(KeySize / 8);
                }

                // Şifreli veriyi çıkar
                int encryptedDataLength = encryptedData.Length - 16 - (BlockSize / 8);
                byte[] cipherBytes = new byte[encryptedDataLength];
                Buffer.BlockCopy(encryptedData, 16 + (BlockSize / 8), cipherBytes, 0, encryptedDataLength);

                // AES algoritması ile çözmeyi dene
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    try
                    {
                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        using (var ms = new MemoryStream(cipherBytes))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            // Sadece doğrulama amaçlı olduğu için bir miktar veri oku
                            byte[] buffer = new byte[1024];
                            cs.Read(buffer, 0, buffer.Length);

                            _lastSuccessMessage = Success.PASSWORD_VALIDATION_SUCCESS;
                            return true;
                        }
                    }
                    catch (CryptographicException)
                    {
                        _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return false;
            }
        }
    }
}
