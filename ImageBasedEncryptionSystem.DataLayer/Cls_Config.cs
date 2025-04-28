using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
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
                // Geliştirme ortamındaki göreceli yol
                string relativePath = "../../../ImageBasedEncryptionSystem.DataLayer/Config.json";
                
                // Önce göreceli yolu dene
                if (File.Exists(relativePath))
                {
                    return relativePath;
                }
                
                // Göreceli yol bulunamazsa, uygulamanın çalıştığı dizinde ara
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDir = Path.GetDirectoryName(exePath);
                string localConfigPath = Path.Combine(exeDir, "Config.json");
                
                if (File.Exists(localConfigPath))
                {
                    return localConfigPath;
                }
                
                // Hala bulunamazsa AppData klasörüne bak
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appDataConfigDir = Path.Combine(appDataPath, "ImageBasedEncryptionSystem");
                string appDataConfigPath = Path.Combine(appDataConfigDir, "Config.json");
                
                if (File.Exists(appDataConfigPath))
                {
                    return appDataConfigPath;
                }
                
                // Şifrelenmiş dosya kontrolü
                string encryptedConfigPath = appDataConfigPath + ENCRYPTED_EXTENSION;
                if (File.Exists(encryptedConfigPath))
                {
                    // Şifrelenmiş dosyayı çöz ve orijinal adla kaydet
                    try
                    {
                        DecryptConfigFile(encryptedConfigPath, appDataConfigPath);
                        if (File.Exists(appDataConfigPath))
                        {
                            return appDataConfigPath;
                        }
                    }
                    catch
                    {
                        // Şifre çözme başarısız olursa sessizce devam et
                    }
                }
                
                // Hiçbir yerde bulunamazsa, AppData klasörüne varsayılan config oluştur
                try
                {
                    if (!Directory.Exists(appDataConfigDir))
                    {
                        Directory.CreateDirectory(appDataConfigDir);
                    }
                    
                    CreateDefaultConfigFile(appDataConfigPath);
                    
                    if (File.Exists(appDataConfigPath))
                    {
                        // Oluşturulan dosyayı şifrele
                        EncryptConfigFile(appDataConfigPath);
                        return appDataConfigPath;
                    }
                }
                catch
                {
                    // AppData'ya yazılamazsa exe konumunda dene
                    try
                    {
                        CreateDefaultConfigFile(localConfigPath);
                        if (File.Exists(localConfigPath))
                        {
                            // Oluşturulan dosyayı şifrele
                            EncryptConfigFile(localConfigPath);
                            return localConfigPath;
                        }
                    }
                    catch
                    {
                        // İki lokasyona da yazılamazsa varsayılan yolu döndür
                    }
                }
                
                // Hiçbir yerde bulunamazsa varsayılan göreceli yolu dön
                return relativePath;
            }
            catch
            {
                // Hata durumunda varsayılan göreceli yolu dön
                return "../../../ImageBasedEncryptionSystem.DataLayer/Config.json";
            }
        }
        
        /// <summary>
        /// Varsayılan Config.json dosyasını oluşturur.
        /// </summary>
        /// <param name="configPath">Oluşturulacak dosyanın tam yolu</param>
        public static void CreateDefaultConfigFile(string configPath)
        {
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
      ""SystemIdentity"": ""pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4"",
      ""DefaultSystemIdentity"": ""VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4""
    }
  ]
}";
            
            // Dosyayı oluştur
            File.WriteAllText(configPath, defaultConfig);
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
                if (!File.Exists(configPath))
                    return false;
                
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
                                }
                            }
                        }
                    }
                }
                
                return true;
            }
            catch
            {
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
                if (!File.Exists(encryptedPath))
                    return false;
                
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
                                }
                            }
                        }
                    }
                }
                
                return true;
            }
            catch
            {
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
                string configPath = GetConfigFilePath();
                if (!File.Exists(configPath))
                    return string.Empty;
                
                string jsonContent = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(jsonContent);
                
                if (config[section] != null && config[section].Count > 0)
                {
                    foreach (var item in config[section])
                    {
                        if (item[key] != null)
                        {
                            return item[key].ToString();
                        }
                    }
                }
                
                return string.Empty;
            }
            catch
            {
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
                string configPath = GetConfigFilePath();
                if (!File.Exists(configPath))
                    return false;
                
                string jsonContent = File.ReadAllText(configPath);
                dynamic config = JsonConvert.DeserializeObject(jsonContent);
                
                if (config[section] != null && config[section].Count > 0)
                {
                    config[section][0][key] = value;
                    
                    // JSON'u formatlayarak kaydet
                    string updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);
                    File.WriteAllText(configPath, updatedJson);
                    
                    // Şifreli versiyonu güncelle
                    EncryptConfigFile(configPath);
                    
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetSystemIdentity()
        {
            string configPath = GetConfigFilePath();
            if (!File.Exists(configPath))
                return null;
            string json = File.ReadAllText(configPath);
            var match = System.Text.RegularExpressions.Regex.Match(json, "\"SystemIdentity\"\\s*:\\s*\"([^\"]+)\"");
            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value;
            return null;
        }
    }
}
