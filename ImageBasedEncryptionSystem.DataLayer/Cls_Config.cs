using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using ImageBasedEncryptionSystem.TypeLayer;
using Newtonsoft.Json;

namespace ImageBasedEncryptionSystem.DataLayer
{
    public class Cls_Config
    {
        // Sabit AES anahtarı - Config.json şifrelemek için kullanılacak
        private static readonly byte[] AES_KEY = {
            0x56, 0x72, 0xA7, 0x12, 0xD9, 0xE3, 0x43, 0xF1, 
            0x89, 0x36, 0xC4, 0x2B, 0x5E, 0x7D, 0x91, 0x08,
            0x14, 0xB5, 0x3C, 0x6F, 0xE2, 0x47, 0x9A, 0xD0,
            0x62, 0x1F, 0x8D, 0xA5, 0x30, 0xC8, 0x59, 0xE7
        }; // 256-bit (32 byte) anahtar
        
        private static readonly byte[] AES_IV = {
            0x32, 0x87, 0xF5, 0x01, 0xD4, 0xB9, 0x6C, 0xE8, 
            0x23, 0x9A, 0x4F, 0x7B, 0x10, 0xC5, 0x3E, 0x8D
        }; // 128-bit (16 byte) IV
        
        // Config dosyasının şifreli uzantısı
        private const string ENCRYPTED_EXTENSION = ".secure";
        
        /// <summary>
        /// Config.json dosyasının yolunu döndürür.
        /// Uygulama geliştirme sırasında göreceli yol, dağıtım sırasında uygulama klasörü içindeki yol kullanılır.
        /// </summary>
        /// <returns>Config.json dosyasının yolu</returns>
        public static string GetConfigFilePath()
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_GET_CONFIG_FILE_PATH_STARTED);
                // Uygulama dizininde şifreli dosyayı ara
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDir = Path.GetDirectoryName(exePath);
                string encryptedConfigPath = Path.Combine(exeDir, "Config.json" + ENCRYPTED_EXTENSION);

                if (File.Exists(encryptedConfigPath))
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_ENCRYPTED_CONFIG_FILE_FOUND);
                    return encryptedConfigPath;
                }

                // Şifreli dosya bulunamazsa, yeni bir config oluştur
                string configPath = Path.Combine(exeDir, "Config.json");
                CreateDefaultConfigFile(configPath);
                EncryptConfigFile(configPath);
                File.Delete(configPath);
                Console.WriteLine(TypeLayer.Debug.DEBUG_DEFAULT_CONFIG_FILE_CREATED);
                return encryptedConfigPath;
            }
            catch
            {
                Console.WriteLine(TypeLayer.Debug.ERROR_GET_CONFIG_FILE_PATH_FAILED);
                return null;
            }
        }
        
        /// <summary>
        /// Varsayılan Config.json dosyasını oluşturur.
        /// </summary>
        /// <param name="configPath">Oluşturulacak dosyanın tam yolu</param>
        public static void CreateDefaultConfigFile(string configPath)
        {
            Console.WriteLine(TypeLayer.Debug.DEBUG_CREATE_DEFAULT_CONFIG_FILE_STARTED);
            // Varsayılan config içeriği
            string defaultConfig = @"{
  ""Developers"": [
    {
      ""DevID"": ""admin"",
      ""Password"": ""12345""
    },
    {
      ""DevID"": ""developer"",
      ""Password"": ""dev123""
    }
  ],

  ""Identity"": [
    {
      ""SystemIdentity"": ""VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4"",
      ""DefaultSystemIdentity"": ""VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4""
    }
  ]
}";
            
            // Dosyayı oluştur
            File.WriteAllText(configPath, defaultConfig);
            Console.WriteLine(TypeLayer.Debug.DEBUG_CREATE_DEFAULT_CONFIG_FILE_COMPLETED);
        }
        
        /// <summary>
        /// Config.json dosyasını şifreler
        /// </summary>
        /// <param name="configPath">Şifrelenecek dosyanın yolu</param>
        /// <returns>Şifreleme başarılı oldu mu?</returns>
        public static bool EncryptConfigFile(string configPath)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_ENCRYPT_CONFIG_FILE_STARTED);
                if (!File.Exists(configPath))
                {
                    Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_FILE_NOT_FOUND);
                    return false;
                }
                
                // Orijinal config içeriğini oku
                string configContent = File.ReadAllText(configPath);
                
                // Şifrelenmiş dosya yolu
                string encryptedPath = configPath + ENCRYPTED_EXTENSION;
                
                // AES ile şifrele
                using (Aes aes = Aes.Create())
                {
                    aes.Key = AES_KEY;
                    aes.IV = AES_IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    // Şifreleme işlemini gerçekleştir
                    using (FileStream fsOutput = new FileStream(encryptedPath, FileMode.Create))
                    {
                        using (ICryptoTransform encryptor = aes.CreateEncryptor())
                        {
                            using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter sw = new StreamWriter(cs))
                                {
                                    sw.Write(configContent);
                                    Console.WriteLine(TypeLayer.Debug.DEBUG_ENCRYPT_CONFIG_FILE_COMPLETED);
                                }
                            }
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.ERROR_ENCRYPT_CONFIG_FILE_FAILED, ex.Message));
                return false;
            }
        }
        
        /// <summary>
        /// Şifrelenmiş Config.json dosyasının şifresini çözer
        /// </summary>
        /// <param name="encryptedPath">Şifrelenmiş dosyanın yolu</param>
        /// <param name="outputPath">Çözülmüş dosyanın kaydedileceği yol</param>
        /// <returns>Şifre çözme başarılı oldu mu?</returns>
        public static bool DecryptConfigFile(string encryptedPath, string outputPath)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_DECRYPT_CONFIG_FILE_STARTED);
                if (!File.Exists(encryptedPath))
                {
                    Console.WriteLine(TypeLayer.Debug.ERROR_ENCRYPTED_CONFIG_FILE_NOT_FOUND);
                    return false;
                }
                
                // AES ile şifre çöz
                using (Aes aes = Aes.Create())
                {
                    aes.Key = AES_KEY;
                    aes.IV = AES_IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    // Şifre çözme işlemini gerçekleştir
                    using (FileStream fsInput = new FileStream(encryptedPath, FileMode.Open))
                    {
                        using (ICryptoTransform decryptor = aes.CreateDecryptor())
                        {
                            using (CryptoStream cs = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader sr = new StreamReader(cs))
                                {
                                    // Çözülmüş içeriği oku
                                    string decryptedContent = sr.ReadToEnd();
                                    
                                    // Hedef dosyaya kaydet
                                    File.WriteAllText(outputPath, decryptedContent);
                                    Console.WriteLine(TypeLayer.Debug.DEBUG_DECRYPT_CONFIG_FILE_COMPLETED);
                                }
                            }
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.ERROR_DECRYPT_CONFIG_FILE_FAILED, ex.Message));
                return false;
            }
        }
        
        /// <summary>
        /// Config.json dosyasından belirli bir değeri okur
        /// </summary>
        /// <param name="section">Ana bölüm (örn: "Identity")</param>
        /// <param name="key">Anahtar (örn: "SystemIdentity")</param>
        /// <returns>Okunan değer veya boş string</returns>
        public static string ReadConfigValue(string section, string key)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_READ_CONFIG_VALUE_STARTED);
                string encryptedConfigPath = GetConfigFilePath();
                if (encryptedConfigPath == null || !File.Exists(encryptedConfigPath))
                {
                    Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_FILE_NOT_FOUND);
                    return string.Empty;
                }

                string configPath = encryptedConfigPath.Replace(ENCRYPTED_EXTENSION, "");
                DecryptConfigFile(encryptedConfigPath, configPath);

                string jsonContent = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(jsonContent);
                File.Delete(configPath);

                if (config[section] != null && config[section].Count > 0)
                {
                    foreach (var item in config[section])
                    {
                        if (item[key] != null)
                        {
                            Console.WriteLine(TypeLayer.Debug.DEBUG_READ_CONFIG_VALUE_COMPLETED);
                            return item[key].ToString();
                        }
                    }
                }

                Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_VALUE_NOT_FOUND);
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.ERROR_READ_CONFIG_VALUE_FAILED, ex.Message));
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Config.json dosyasına bir değer yazar
        /// </summary>
        /// <param name="section">Ana bölüm (örn: "Identity")</param>
        /// <param name="key">Anahtar (örn: "SystemIdentity")</param>
        /// <param name="value">Yazılacak değer</param>
        /// <returns>İşlem başarılı oldu mu?</returns>
        public static bool WriteConfigValue(string section, string key, string value)
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_WRITE_CONFIG_VALUE_STARTED);
                string encryptedConfigPath = GetConfigFilePath();
                if (encryptedConfigPath == null || !File.Exists(encryptedConfigPath))
                {
                    Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_FILE_NOT_FOUND);
                    return false;
                }

                string configPath = encryptedConfigPath.Replace(ENCRYPTED_EXTENSION, "");
                DecryptConfigFile(encryptedConfigPath, configPath);

                string jsonContent = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(jsonContent);

                if (config[section] != null && config[section].Count > 0)
                {
                    config[section][0][key] = value;

                    string updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);
                    File.WriteAllText(configPath, updatedJson);

                    EncryptConfigFile(configPath);
                    File.Delete(configPath);
                    Console.WriteLine(TypeLayer.Debug.DEBUG_WRITE_CONFIG_VALUE_COMPLETED);
                    return true;
                }

                Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_SECTION_NOT_FOUND);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.ERROR_WRITE_CONFIG_VALUE_FAILED, ex.Message));
                return false;
            }
        }

        public static string GetSystemIdentity()
        {
            return ReadConfigValue("Identity", "SystemIdentity");
        }

        public static bool UpdateSystemIdentity(string newIdentity)
        {
            return WriteConfigValue("Identity", "SystemIdentity", newIdentity);
        }

        /// <summary>
        /// Varsayılan SystemIdentity değerini döndürür.
        /// </summary>
        /// <returns>Varsayılan SystemIdentity</returns>
        public static string GetDefaultSystemIdentity()
        {
            return ReadConfigValue("Identity", "DefaultSystemIdentity");
        }

        public static List<dynamic> GetDeveloperID()
        {
            try
            {
                Console.WriteLine(TypeLayer.Debug.DEBUG_GET_DEVELOPER_INFO_STARTED);
                string encryptedConfigPath = GetConfigFilePath();
                if (encryptedConfigPath == null || !File.Exists(encryptedConfigPath))
                {
                    Console.WriteLine(TypeLayer.Debug.ERROR_CONFIG_FILE_NOT_FOUND);
                    return null;
                }

                string configPath = encryptedConfigPath.Replace(ENCRYPTED_EXTENSION, "");
                DecryptConfigFile(encryptedConfigPath, configPath);

                string jsonContent = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(jsonContent);
                File.Delete(configPath);

                if (config.Developers != null && config.Developers.Count > 0)
                {
                    Console.WriteLine(TypeLayer.Debug.DEBUG_GET_DEVELOPER_INFO_COMPLETED);
                    return config.Developers.ToObject<List<dynamic>>();
                }

                Console.WriteLine(TypeLayer.Debug.ERROR_NO_DEVELOPERS_FOUND);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(TypeLayer.Debug.ERROR_GET_DEVELOPER_INFO_FAILED, ex.Message));
                return null;
            }
        }
    }
}
