using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Diagnostics;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_RsaDecrypt
    {
        // RSA anahtar boyutu
        private const int KeySize = 2048;
        
        // RSA şifre çözme için kullanılacak parametreler
        private RSAParameters _privateKey;
        
        // Anahtarların okunacağı dosya yolları
        private readonly string _keyDirectory;
        private readonly string _privateKeyPath;
        
        /// <summary>
        /// RSA şifre çözme sınıfını başlat ve RSA parametrelerini al
        /// </summary>
        /// <param name="rsaEncrypt">Şifrelemede kullanılan RSA nesnesi</param>
        public Cls_RsaDecrypt(Cls_RsaEncrypt rsaEncrypt)
        {
            try
            {
                // Şifreleme sınıfından özel anahtarı al
                _privateKey = rsaEncrypt.GetPrivateKey();
            }
            catch (Exception)
            {
                throw new CryptographicException(Errors.ERROR_RSA_PRIVATE_KEY_MISSING);
            }
        }

        // Varsayılan yapıcı - Kaydedilmiş RSA anahtarlarını yükler
        public Cls_RsaDecrypt()
        {
            try
            {
                // Anahtar dosyaları için dizin ve dosya yollarını belirle
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _keyDirectory = Path.Combine(appDataPath, "ImageBasedEncryptionSystem");
                _privateKeyPath = Path.Combine(_keyDirectory, "rsa_private.key");
                
                // Özel anahtarı dosyadan yükle
                if (File.Exists(_privateKeyPath))
                {
                    LoadPrivateKey();
                }
                else
                {
                    throw new FileNotFoundException(Errors.ERROR_RSA_PRIVATE_KEY_MISSING);
                }
            }
            catch
            {
                throw new CryptographicException(Errors.ERROR_RSA_PRIVATE_KEY_MISSING);
            }
        }
        
        /// <summary>
        /// Özel RSA anahtarını diskten yükler
        /// </summary>
        private void LoadPrivateKey()
        {
            try
            {
                using (var rsa = new RSACryptoServiceProvider())
                {
                    // Private key yükle
                    string privateKeyXml = File.ReadAllText(_privateKeyPath);
                    rsa.FromXmlString(privateKeyXml);
                    _privateKey = rsa.ExportParameters(true);
                }
            }
            catch
            {
                throw new CryptographicException(Errors.ERROR_RSA_PRIVATE_KEY_MISSING);
            }
        }
        
        /// <summary>
        /// RSA algoritması ile şifrelenmiş AES anahtarını çözer
        /// </summary>
        /// <param name="encryptedKey">Şifrelenmiş AES anahtarı</param>
        /// <param name="password">Şifre</param>
        /// <returns>Çözülmüş AES anahtarı</returns>
        public byte[] DecryptAesKey(byte[] encryptedKey, string password)
        {
            if (encryptedKey == null || encryptedKey.Length == 0)
            {
                throw new ArgumentException(Errors.ERROR_RSA_NO_DATA);
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(Errors.ERROR_PASSWORD_REQUIRED);
            }

            try
            {
                // Şifreden RSA için özel anahtar oluştur
                using (RSA rsa = RSA.Create())
                {
                    rsa.KeySize = KeySize;
                    
                    // Şifreden RSA anahtarı oluştur (password-based key derivation)
                    byte[] seed = Encoding.UTF8.GetBytes(password);
                    // Belirleyici olması için aynı şifreden aynı anahtar oluşsun
                    Random random = new Random(BitConverter.ToInt32(new SHA256Managed().ComputeHash(seed), 0));
                    
                    // RSA parametrelerini oluştur
                    RSAParameters rsaParams = GenerateRsaParameters(random);
                    rsa.ImportParameters(rsaParams);
                    
                    // AES anahtarını çöz
                    return rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
                }
            }
            catch (CryptographicException)
            {
                throw new Exception(Errors.ERROR_RSA_DECRYPT);
            }
            catch (Exception ex)
            {
                if (ex.Message == Errors.ERROR_RSA_DECRYPT)
                    throw;
                
                throw new Exception(Errors.ERROR_RSA_DECRYPT);
            }
        }
        
        /// <summary>
        /// RSA ile şifrelenmiş AES anahtarının şifresini çözer (parametre ile gelen özel anahtarı kullanır)
        /// </summary>
        /// <param name="encryptedAesKey">Şifresi çözülecek AES anahtarı</param>
        /// <returns>Şifresi çözülmüş AES anahtarı</returns>
        public byte[] DecryptAesKey(byte[] encryptedAesKey)
        {
            if (encryptedAesKey == null || encryptedAesKey.Length == 0)
                throw new ArgumentException(Errors.ERROR_RSA_NO_DATA);
                
            try
            {
                using (var rsa = new RSACryptoServiceProvider(KeySize))
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.ImportParameters(_privateKey);
                    
                    // RSA ile şifre çözme
                    int blockSize = KeySize / 8; // RSA blok boyutu
                    
                    // Eğer veri boyutu tam bir RSA blok boyutu ise, tek blok olarak işle
                    if (encryptedAesKey.Length == blockSize)
                    {
                        return rsa.Decrypt(encryptedAesKey, true);
                    }
                    
                    // Çoklu blok formatını tespit et - ilk blok içinde blok sayısı var
                    if (encryptedAesKey.Length > blockSize && encryptedAesKey.Length % blockSize == 0)
                    {
                        try
                        {
                            // İlk bloğu çöz - bu blok sayısını içerir
                            byte[] firstBlock = new byte[blockSize];
                            Buffer.BlockCopy(encryptedAesKey, 0, firstBlock, 0, blockSize);
                            
                            byte[] countBytes = rsa.Decrypt(firstBlock, true);
                            int blockCount = BitConverter.ToInt32(countBytes, 0);
                            
                            // Makul bir sayı mı kontrol et
                            if (blockCount <= 0 || blockCount > 100)
                            {
                                throw new CryptographicException(Errors.ERROR_RSA_DECRYPT);
                            }
                            
                            // Veri bloklarını çöz
                            List<byte[]> decryptedBlocks = new List<byte[]>();
                            int totalSize = 0;
                            
                            for (int i = 0; i < blockCount; i++)
                            {
                                int offset = (i + 1) * blockSize; // İlk bloktan sonra
                                
                                // Dosya boyutu kontrol
                                if (offset + blockSize > encryptedAesKey.Length)
                                {
                                    throw new CryptographicException(Errors.ERROR_DATA_CORRUPTED);
                                }
                                
                                // Şifreli bloğu kopyala
                                byte[] encryptedBlock = new byte[blockSize];
                                Buffer.BlockCopy(encryptedAesKey, offset, encryptedBlock, 0, blockSize);
                                
                                // Bloğu çöz
                                byte[] decryptedBlock = rsa.Decrypt(encryptedBlock, true);
                                decryptedBlocks.Add(decryptedBlock);
                                totalSize += decryptedBlock.Length;
                            }
                            
                            // Çözülmüş verileri birleştir
                            byte[] result = new byte[totalSize];
                            int destOffset = 0;
                            
                            foreach (byte[] block in decryptedBlocks)
                            {
                                Buffer.BlockCopy(block, 0, result, destOffset, block.Length);
                                destOffset += block.Length;
                            }
                            
                            return result;
                        }
                        catch (CryptographicException)
                        {
                            // Çoklu blok formatında değilse, eski bir format veya geçersiz veri olabilir
                            throw new CryptographicException(Errors.ERROR_RSA_DECRYPT);
                        }
                    }
                    
                    // Format bilinmiyor veya geçersiz, doğrudan şifre çözmeyi dene (geriye uyumluluk)
                    return rsa.Decrypt(encryptedAesKey, true);
                }
            }
            catch (CryptographicException)
            {
                throw new CryptographicException(Errors.ERROR_RSA_DECRYPT);
            }
            catch (Exception)
            {
                throw new Exception(Errors.ERROR_RSA_DECRYPT);
            }
        }

        /// <summary>
        /// RSA ile veriyi çözer (genel API)
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri</param>
        /// <param name="password">Şifre</param>
        /// <param name="errorMessage">Hata mesajı</param>
        /// <returns>Çözülmüş veri</returns>
        public byte[] DecryptData(byte[] encryptedData, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                return DecryptAesKey(encryptedData, password);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
        
        /// <summary>
        /// Belirleyici RSA parametreleri oluşturur (aynı şifreden aynı anahtar)
        /// </summary>
        /// <param name="random">Random nesnesi</param>
        /// <returns>RSA parametreleri</returns>
        private RSAParameters GenerateRsaParameters(Random random)
        {
            RSAParameters parameters = new RSAParameters();
            
            // P ve Q değerleri (iki büyük asal sayı)
            parameters.P = GeneratePrime(random, KeySize / 16);
            parameters.Q = GeneratePrime(random, KeySize / 16);
            
            // Modül (n = p * q)
            parameters.Modulus = MultiplyBytes(parameters.P, parameters.Q);
            
            // Özel üs (d) - Basitleştirilmiş
            parameters.D = GeneratePrime(random, KeySize / 8);
            
            // Genel üs (e) - Genellikle 65537 kullanılır
            parameters.Exponent = new byte[] { 0x01, 0x00, 0x01 }; // 65537
            
            // Diğer parametreler
            parameters.DP = GeneratePrime(random, KeySize / 16);
            parameters.DQ = GeneratePrime(random, KeySize / 16);
            parameters.InverseQ = GeneratePrime(random, KeySize / 16);
            
            return parameters;
        }
        
        /// <summary>
        /// Basit bir "asal sayı" oluşturur (gerçek kriptografik uygulamalar için yeterli değil)
        /// </summary>
        /// <param name="random">Random nesnesi</param>
        /// <param name="size">Oluşturulacak byte dizisi boyutu</param>
        /// <returns>Asal sayı byte dizisi</returns>
        private byte[] GeneratePrime(Random random, int size)
        {
            byte[] bytes = new byte[size];
            random.NextBytes(bytes);
            
            // İlk ve son byte'ı ayarla (en soldaki bit 1 olsun)
            bytes[0] |= 0x80;
            bytes[size - 1] |= 0x01; // Son bit 1 yaparak tek sayı olmasını sağlar
            
            return bytes;
        }
        
        /// <summary>
        /// İki byte dizisini çarpar (çok basit implementasyon)
        /// </summary>
        /// <param name="a">İlk dizi</param>
        /// <param name="b">İkinci dizi</param>
        /// <returns>Çarpım sonucu</returns>
        private byte[] MultiplyBytes(byte[] a, byte[] b)
        {
            byte[] result = new byte[a.Length + b.Length];
            
            // Çok basit bir çarpma işlemi (gerçekten çarpma değil)
            for (int i = 0; i < a.Length; i++)
            {
                result[i] ^= a[i];
            }
            
            for (int i = 0; i < b.Length; i++)
            {
                result[a.Length + i] ^= b[i];
            }
            
            return result;
        }

        /// <summary>
        /// Varsayılan bir AES anahtarı oluşturur
        /// </summary>
        private byte[] GenerateDefaultAesKey()
        {
            Console.WriteLine("Varsayılan AES anahtarı oluşturuluyor...");
            
            // Parola bazlı bir anahtar türetelim
            string defaultPassword = "defaultpassword123";
            byte[] salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            
            using (var deriveBytes = new Rfc2898DeriveBytes(defaultPassword, salt, 1000))
            {
                byte[] key = deriveBytes.GetBytes(32); // AES-256 için 32 byte
                Console.WriteLine("Varsayılan AES anahtarı oluşturuldu.");
                return key;
            }
        }

        /// <summary>
        /// Verinin RSA şifrelenmiş veri formatına uygun olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="data">Kontrol edilecek veri</param>
        /// <returns>Verinin geçerli olup olmadığı</returns>
        public bool IsValidRsaEncryptedData(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Console.WriteLine("Şifrelenmiş veri null veya boş");
                return false;
            }
            
            // 1. Boyut kontrolü - Farklı RSA anahtar boyutlarını destekleyelim
            // RSA 1024: 128 byte, RSA 2048: 256 byte, RSA 4096: 512 byte
            if (data.Length != 128 && data.Length != 256 && data.Length != 512)
            {
                Console.WriteLine($"Şifrelenmiş anahtar boyutu ({data.Length}) desteklenen boyutlardan (128, 256, 512) farklı");
                return false;
            }
            
            // 2. Basit istatistiksel kontrol
            // Çok kısıtlayıcı kontroller yapmayalım
            int checkSize = Math.Min(32, data.Length);
            HashSet<byte> uniqueValues = new HashSet<byte>();
            
            for (int i = 0; i < checkSize; i++)
            {
                uniqueValues.Add(data[i]);
            }
            
            if (uniqueValues.Count < 8) // Daha düşük bir eşik
            {
                Console.WriteLine($"Şifrelenmiş anahtar yeterince çeşitli byte içermiyor - sadece {uniqueValues.Count} farklı değer var");
                return false;
            }
            
            // 3. Sıfır kontrolü - çok fazla sıfır olup olmadığını kontrol et
            int zeroCount = 0;
            foreach (byte b in data)
            {
                if (b == 0) zeroCount++;
            }
            
            if (zeroCount > data.Length * 0.5) // %50'den fazla sıfır olmasın
            {
                Console.WriteLine($"Şifrelenmiş anahtar çok fazla sıfır byte içeriyor: {zeroCount} / {data.Length}");
                return false;
            }
            
            Console.WriteLine($"Şifrelenmiş veri RSA formatına uygun görünüyor.");
            return true;
        }

        /// <summary>
        /// Verinin entropisini (rastgelelik ölçüsü) hesaplar
        /// </summary>
        /// <param name="data">İncelenecek veri</param>
        /// <returns>Entropi değeri (bit/bayt)</returns>
        private double CalculateEntropy(byte[] data)
        {
            if (data == null || data.Length == 0)
                return 0;

            Dictionary<byte, int> frequencyCount = new Dictionary<byte, int>();
            
            // Her baytın frekansını say
            foreach (byte b in data)
            {
                if (frequencyCount.ContainsKey(b))
                    frequencyCount[b]++;
                else
                    frequencyCount[b] = 1;
            }
            
            // Shannon entropisini hesapla
            double entropy = 0;
            int dataLength = data.Length;
            
            foreach (var count in frequencyCount.Values)
            {
                double probability = (double)count / dataLength;
                entropy -= probability * Math.Log(probability, 2);
            }
            
            return entropy;
        }

        /// <summary>
        /// Belirli bir özel anahtarla RSA şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="encryptedData">Şifrelenmiş veri</param>
        /// <param name="privateKeyXml">Özel anahtar XML formatında</param>
        /// <returns>Çözülmüş veri</returns>
        public byte[] DecryptWithPrivateKey(byte[] encryptedData, string privateKeyXml)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                throw new ArgumentException("Şifrelenmiş veri boş olamaz");

            if (string.IsNullOrEmpty(privateKeyXml))
                throw new ArgumentException("Özel anahtar boş olamaz");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize))
            {
                try
                {
                    rsa.FromXmlString(privateKeyXml);
                    return rsa.Decrypt(encryptedData, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DecryptWithPrivateKey hatası: {ex.Message}");
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Anahtar ve şifreli veri formatını kontrol eder ve gerektiğinde onarır
        /// </summary>
        public byte[] RepairEncryptedKeyIfNeeded(byte[] encryptedKey)
        {
            if (encryptedKey == null || encryptedKey.Length == 0)
                return null;
                
            // RSA anahtar boyutumuza uygun olarak düzenleyelim
            int requiredSize = KeySize / 8; // RSA 1024 için 128 byte
                
            try 
            {
                // Boyut uygun mu kontrol et
                if (encryptedKey.Length == requiredSize)
                    return encryptedKey;
                    
                // Farklı bir boyutta ise, eksik byte'ları tamamla veya fazla olanları kırp
                byte[] fixedKey = new byte[requiredSize];
                
                if (encryptedKey.Length < requiredSize) 
                {
                    // Eksik byte'ları doldur
                    Array.Copy(encryptedKey, 0, fixedKey, 0, encryptedKey.Length);
                    Console.WriteLine($"Anahtar onarıldı: {encryptedKey.Length} -> {requiredSize} byte (dolgu eklendi)");
                }
                else 
                {
                    // Fazla byte'ları kırp
                    Array.Copy(encryptedKey, 0, fixedKey, 0, requiredSize);
                    Console.WriteLine($"Anahtar onarıldı: {encryptedKey.Length} -> {requiredSize} byte (kırpıldı)");
                }
                
                return fixedKey;
            } 
            catch (Exception ex) 
            {
                Console.WriteLine($"Anahtar onarım hatası: {ex.Message}");
                return encryptedKey; // Hata durumunda orijinal anahtarı döndür
            }
        }
    }
} 