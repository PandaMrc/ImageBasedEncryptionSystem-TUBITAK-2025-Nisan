using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_RsaEncrypt
    {
        // RSA anahtar boyutu
        private const int KeySize = 2048;
        
        // Oluşturulan RSA public/private anahtar çifti
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;
        
        // Anahtarların kaydedileceği dosya yolları
        private readonly string _keyDirectory;
        private readonly string _publicKeyPath;
        private readonly string _privateKeyPath;
        
        // Sabit anahtar olduğunu belirten dosya
        private readonly string _fixedKeyFlag;

        /// <summary>
        /// RSA şifreleme için anahtar çifti oluşturur
        /// </summary>
        public Cls_RsaEncrypt()
        {
            try
            {
                // Anahtar dosyaları için dizin ve dosya yollarını belirle
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _keyDirectory = Path.Combine(appDataPath, "ImageBasedEncryptionSystem");
                _publicKeyPath = Path.Combine(_keyDirectory, "rsa_public.key");
                _privateKeyPath = Path.Combine(_keyDirectory, "rsa_private.key");
                _fixedKeyFlag = Path.Combine(_keyDirectory, "fixed_key.flag");

                // Dizin yoksa oluştur
                if (!Directory.Exists(_keyDirectory))
                {
                    Directory.CreateDirectory(_keyDirectory);
                }

                // Daha önce kayıtlı anahtarlar varsa yükle
                if (File.Exists(_publicKeyPath) && File.Exists(_privateKeyPath))
                {
                    bool loaded = LoadKeys();
                    if (loaded)
                    {
                        // Anahtarları başarıyla yükledik, sabit anahtar bayrağını oluştur
                        if (!File.Exists(_fixedKeyFlag))
                        {
                            File.WriteAllText(_fixedKeyFlag, DateTime.Now.ToString());
                        }
                        return; // Başarıyla yüklendiyse, yeni anahtar oluşturmaya gerek yok
                    }
                }

                // Yeni RSA anahtar çifti oluştur
                using (var rsa = new RSACryptoServiceProvider(KeySize))
                {
                    rsa.PersistKeyInCsp = false;
                    _publicKey = rsa.ExportParameters(false);
                    _privateKey = rsa.ExportParameters(true);
                    
                    // Anahtarları disk üzerine kaydet
                    SaveKeys();
                    
                    // Sabit anahtar bayrağını oluştur
                    File.WriteAllText(_fixedKeyFlag, DateTime.Now.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RSA anahtar oluşturma hatası: {ex.Message}");
                throw new CryptographicException(Errors.ERROR_RSA_KEY_GENERATION);
            }
        }
        
        /// <summary>
        /// RSA şifreleme için anahtar çifti oluşturur veya yeniden oluşturur
        /// </summary>
        /// <param name="regenerateKeys">Anahtarları yeniden oluşturmak için true, var olan anahtarları kullanmak için false</param>
        public Cls_RsaEncrypt(bool regenerateKeys)
        {
            try
            {
                // Anahtar dosyaları için dizin ve dosya yollarını belirle
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _keyDirectory = Path.Combine(appDataPath, "ImageBasedEncryptionSystem");
                _publicKeyPath = Path.Combine(_keyDirectory, "rsa_public.key");
                _privateKeyPath = Path.Combine(_keyDirectory, "rsa_private.key");
                _fixedKeyFlag = Path.Combine(_keyDirectory, "fixed_key.flag");

                // Dizin yoksa oluştur
                if (!Directory.Exists(_keyDirectory))
                {
                    Directory.CreateDirectory(_keyDirectory);
                }

                // Yeniden oluşturma istenmiyorsa ve dosyalar varsa, anahtarları yükle
                if (!regenerateKeys && File.Exists(_publicKeyPath) && File.Exists(_privateKeyPath))
                {
                    bool loaded = LoadKeys();
                    if (loaded)
                    {
                        return; // Başarıyla yüklendiyse, yeni anahtar oluşturmaya gerek yok
                    }
                }

                // Yeni RSA anahtar çifti oluştur
                using (var rsa = new RSACryptoServiceProvider(KeySize))
                {
                    rsa.PersistKeyInCsp = false;
                    _publicKey = rsa.ExportParameters(false);
                    _privateKey = rsa.ExportParameters(true);
                    
                    // Anahtarları disk üzerine kaydet
                    SaveKeys();
                    
                    // Kullanıcı tarafından zorla değiştirilmişse, fixed key bayrağını kaldır
                    if (regenerateKeys && File.Exists(_fixedKeyFlag))
                    {
                        try
                        {
                            File.Delete(_fixedKeyFlag);
                        }
                        catch
                        {
                            // Silme hatası önemli değil
                        }
                    }
                    else
                    {
                        // Sabit anahtar bayrağını oluştur
                        File.WriteAllText(_fixedKeyFlag, DateTime.Now.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RSA anahtar oluşturma hatası: {ex.Message}");
                throw new CryptographicException(Errors.ERROR_RSA_KEY_GENERATION);
            }
        }
        
        /// <summary>
        /// RSA anahtarlarını disk üzerine kaydeder
        /// </summary>
        private bool SaveKeys()
        {
            try
            {
                // RSA parametrelerini XML formatına dönüştürmek için
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(_publicKey);
                    string publicKeyXml = rsa.ToXmlString(false);
                    File.WriteAllText(_publicKeyPath, publicKeyXml);

                    rsa.ImportParameters(_privateKey);
                    string privateKeyXml = rsa.ToXmlString(true);
                    File.WriteAllText(_privateKeyPath, privateKeyXml);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Hatayı kaydet
                Debug.WriteLine($"RSA anahtarları kaydedilemedi: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// RSA anahtarlarını diskten yükler
        /// </summary>
        private bool LoadKeys()
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    // Public key yükle
                    string publicKeyXml = File.ReadAllText(_publicKeyPath);
                    rsa.FromXmlString(publicKeyXml);
                    _publicKey = rsa.ExportParameters(false);
                    
                    // Private key yükle
                    string privateKeyXml = File.ReadAllText(_privateKeyPath);
                    rsa.FromXmlString(privateKeyXml);
                    _privateKey = rsa.ExportParameters(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Hatayı kaydet
                Debug.WriteLine($"RSA anahtarları yüklenemedi: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Mevcut anahtarların sabit (değiştirilmemiş) olup olmadığını kontrol eder
        /// </summary>
        public bool IsUsingFixedKey()
        {
            return File.Exists(_fixedKeyFlag);
        }
        
        /// <summary>
        /// AES anahtarını RSA ile şifreler
        /// </summary>
        /// <param name="aesKey">Şifrelenecek AES anahtarı</param>
        /// <returns>RSA ile şifrelenmiş AES anahtarı</returns>
        public byte[] EncryptAesKey(byte[] aesKey)
        {
            if (aesKey == null || aesKey.Length == 0)
                throw new ArgumentException(Errors.ERROR_AES_KEY_GENERATION);
                
            try
            {
                using (var rsa = new RSACryptoServiceProvider(KeySize))
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.ImportParameters(_publicKey);
                    
                    // RSA ile şifreleme yapılırken, veri boyutu kısıtlaması var
                    // Verinin maksimum boyutu, anahtar boyutunun (bayt cinsinden) - 42'dir
                    int maxDataLength = (KeySize / 8) - 42;
                    
                    if (aesKey.Length > maxDataLength)
                    {
                        // AES anahtarı çok büyük, bölümlere ayırarak şifrele
                        Debug.WriteLine($"Uzun AES anahtarı ({aesKey.Length} bayt) bölümlere ayrılarak şifreleniyor");
                        
                        // Önce veri bloklarının sayısını hesapla
                        int blockCount = (aesKey.Length + maxDataLength - 1) / maxDataLength;
                        
                        // Blok sayısını kaydedecek öncü bayt dizisi
                        byte[] countBytes = BitConverter.GetBytes(blockCount);
                        
                        // Her blok için şifrelenmiş verileri tutacak liste
                        List<byte[]> encryptedBlocks = new List<byte[]>();
                        
                        // Blok sayısını şifrele ve listeye ekle
                        encryptedBlocks.Add(rsa.Encrypt(countBytes, true));
                        
                        // Her bloğu şifrele
                        for (int i = 0; i < blockCount; i++)
                        {
                            // Mevcut bloğun boyutunu hesapla
                            int blockSize = Math.Min(maxDataLength, aesKey.Length - (i * maxDataLength));
                            byte[] block = new byte[blockSize];
                            
                            // Bloğu orijinal anahtardan kopyala
                            Buffer.BlockCopy(aesKey, i * maxDataLength, block, 0, blockSize);
                            
                            // Bloğu şifrele ve listeye ekle
                            byte[] encryptedBlock = rsa.Encrypt(block, true);
                            encryptedBlocks.Add(encryptedBlock);
                        }
                        
                        // Şifreli blokların toplam boyutunu hesapla
                        int totalLength = 0;
                        foreach (byte[] block in encryptedBlocks)
                        {
                            totalLength += block.Length;
                        }
                        
                        // Sonuç dizisini oluştur
                        byte[] result = new byte[totalLength];
                        int offset = 0;
                        
                        // Tüm blokları birleştir
                        foreach (byte[] block in encryptedBlocks)
                        {
                            Buffer.BlockCopy(block, 0, result, offset, block.Length);
                            offset += block.Length;
                        }
                        
                        return result;
                    }
                    else
                    {
                        // Normal şifreleme (tek blok)
                        return rsa.Encrypt(aesKey, true);
                    }
                }
            }
            catch (CryptographicException ex)
            {
                Debug.WriteLine($"RSA şifreleme hatası (kriptografi): {ex.Message}");
                throw new CryptographicException(Errors.ERROR_RSA_ENCRYPT);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RSA şifreleme hatası (genel): {ex.Message}");
                throw new Exception(Errors.ERROR_RSA_ENCRYPT);
            }
        }
        
        /// <summary>
        /// RSA private key'i döndürür
        /// </summary>
        /// <returns>RSA özel anahtarı</returns>
        public RSAParameters GetPrivateKey()
        {
            return _privateKey;
        }
        
        /// <summary>
        /// RSA sağlayıcısını oluşturup döndürür
        /// </summary>
        /// <returns>RSA sağlayıcısı</returns>
        public RSACryptoServiceProvider GetRsaProvider()
        {
            try
            {
                var rsa = new RSACryptoServiceProvider(KeySize);
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(_privateKey);
                return rsa;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RSA sağlayıcısı oluşturma hatası: {ex.Message}");
                throw new CryptographicException(Errors.ERROR_RSA_KEY_GENERATION);
            }
        }
        
        /// <summary>
        /// Verilen parola ile AES anahtarını RSA kullarak şifreler
        /// </summary>
        /// <param name="password">AES anahtarı oluşturmak için kullanılacak parola</param>
        /// <returns>RSA ile şifrelenmiş AES anahtarı</returns>
        public byte[] EncryptAesKeyWithPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(Errors.ERROR_PASSWORD_EMPTY);
                
            if (password.StartsWith(" "))
                throw new ArgumentException(Errors.ERROR_PASSWORD_SPACE);
                
            if (password.Length < 3 || password.Length > 11)
                throw new ArgumentException(Errors.ERROR_PASSWORD_LENGTH_RANGE);
                
            try
            {
                // Paroladan AES anahtarı oluştur
                Cls_AesEncrypt aesEncrypt = new Cls_AesEncrypt();
                byte[] aesKey = aesEncrypt.GetKey(password);
                
                // AES anahtarını RSA ile şifrele
                return EncryptAesKey(aesKey);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Parola ile RSA şifreleme hatası: {ex.Message}");
                throw new Exception(Errors.ERROR_RSA_ENCRYPT);
            }
        }
    }
} 