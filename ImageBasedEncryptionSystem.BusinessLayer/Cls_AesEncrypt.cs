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
    public class Cls_AesEncrypt
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
        /// Paroladan AES anahtarı oluşturur
        /// </summary>
        /// <param name="password">Anahtar oluşturmak için kullanılacak parola</param>
        /// <returns>AES anahtarı</returns>
        public byte[] GetKey(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Salt değeri oluştur
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Parola ve salt değerden anahtar türet
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] key = deriveBytes.GetBytes(KeySize / 8);
                    _lastSuccessMessage = Success.KEY_GENERATION_SUCCESS;
                    return key;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_KEY_GENERATION, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Paroladan AES anahtarı ve IV değeri oluşturur
        /// </summary>
        /// <param name="password">Anahtar oluşturmak için kullanılacak parola</param>
        /// <param name="salt">Salt değeri (sağlanmazsa rastgele oluşturulur)</param>
        /// <param name="iv">Oluşturulan IV değeri</param>
        /// <returns>AES anahtarı</returns>
        public byte[] GetKeyAndIV(string password, byte[] salt, out byte[] iv)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    iv = null;
                    return null;
                }

                // Salt değeri sağlanmamışsa rastgele oluştur
                if (salt == null || salt.Length < 8)
                {
                    salt = new byte[16];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(salt);
                    }
                }

                // Parola ve salt değerden anahtar ve IV türet
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] key = deriveBytes.GetBytes(KeySize / 8);
                    iv = deriveBytes.GetBytes(BlockSize / 8);
                    
                    _lastSuccessMessage = Success.KEY_GENERATION_SUCCESS;
                    return key;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_KEY_GENERATION, ex.Message);
                iv = null;
                return null;
            }
        }
        
        /// <summary>
        /// AES algoritması ile metni şifreler
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <param name="password">Şifreleme için kullanılacak parola</param>
        /// <returns>Şifrelenmiş veri (IV + Şifreli Metin)</returns>
        public byte[] EncryptText(string plainText, string password)
        {
            try
            {
                // Parametre kontrolü
                if (string.IsNullOrEmpty(plainText))
                {
                    _lastErrorMessage = Errors.ERROR_TEXT_EMPTY;
                    return null;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Salt değeri oluştur
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Anahtar ve IV'yi parola ve salt değerden türet
                byte[] iv;
                byte[] key = GetKeyAndIV(password, salt, out iv);

                if (key == null || iv == null)
                {
                    return null; // Hata mesajı zaten GetKeyAndIV içinde ayarlandı
                }

                // AES algoritması ile şifrele
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // Salt değerini önce yaz
                        ms.Write(salt, 0, salt.Length);
                        
                        // IV değerini salt'dan sonra yaz
                        ms.Write(iv, 0, iv.Length);

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }

                        _lastSuccessMessage = Success.ENCRYPT_SUCCESS_AES;
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES algoritması ile veriyi şifreler
        /// </summary>
        /// <param name="data">Şifrelenecek veri</param>
        /// <param name="password">Şifreleme için kullanılacak parola</param>
        /// <returns>Şifrelenmiş veri (Salt + IV + Şifreli Veri)</returns>
        public byte[] EncryptData(byte[] data, string password)
        {
            try
            {
                // Parametre kontrolü
                if (data == null || data.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return null;
                }

                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Salt değeri oluştur
                byte[] salt = new byte[16];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);
                }

                // Anahtar ve IV'yi parola ve salt değerden türet
                byte[] iv;
                byte[] key = GetKeyAndIV(password, salt, out iv);

                if (key == null || iv == null)
                {
                    return null; // Hata mesajı zaten GetKeyAndIV içinde ayarlandı
                }

                // AES algoritması ile şifrele
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // Salt değerini önce yaz
                        ms.Write(salt, 0, salt.Length);
                        
                        // IV değerini salt'dan sonra yaz
                        ms.Write(iv, 0, iv.Length);

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        _lastSuccessMessage = Success.ENCRYPT_SUCCESS_AES;
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES algoritması ile veriyi şifreler, özel anahtar ve IV kullanır
        /// </summary>
        /// <param name="data">Şifrelenecek veri</param>
        /// <param name="key">AES anahtarı</param>
        /// <param name="iv">Başlangıç vektörü</param>
        /// <returns>Şifrelenmiş veri</returns>
        public byte[] EncryptDataWithKeyIV(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                // Parametre kontrolü
                if (data == null || data.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
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

                // AES algoritması ile şifrele
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = key;
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        _lastSuccessMessage = Success.ENCRYPT_SUCCESS_AES;
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Şifrelenmiş bir dosyayı belirtilen konuma kaydeder
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri</param>
        /// <param name="filePath">Kaydedilecek dosya yolu</param>
        /// <returns>İşlem başarılı ise true, değilse false</returns>
        public bool SaveEncryptedFile(byte[] encryptedData, string filePath)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return false;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    _lastErrorMessage = Errors.ERROR_FILE_INVALID_PATH;
                    return false;
                }

                // Dosyayı yaz
                File.WriteAllBytes(filePath, encryptedData);
                
                _lastSuccessMessage = Success.FILE_SAVE_SUCCESS;
                return true;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_FILE_SAVE, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Rastgele bir anahtar oluşturur
        /// </summary>
        /// <returns>Rastgele AES anahtarı</returns>
        public byte[] GenerateRandomKey()
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.GenerateKey();
                    
                    _lastSuccessMessage = Success.KEY_GENERATION_SUCCESS;
                    return aes.Key;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_KEY_GENERATION, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Rastgele bir IV değeri oluşturur
        /// </summary>
        /// <returns>Rastgele IV</returns>
        public byte[] GenerateRandomIV()
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.BlockSize = BlockSize;
                    aes.GenerateIV();
                    
                    _lastSuccessMessage = "Başlangıç vektörü başarıyla oluşturuldu.";
                    return aes.IV;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_IV_INVALID, ex.Message);
                return null;
            }
        }
    }
}
