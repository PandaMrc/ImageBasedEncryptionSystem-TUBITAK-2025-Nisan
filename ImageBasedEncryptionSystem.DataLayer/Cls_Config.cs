using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImageBasedEncryptionSystem.DataLayer
{
    public class Cls_Config
    {
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
      ""DefeultSystemIdentity"": ""VARSAYILAN_KIMLIK_TUBITAK_KSSAL_2025_pVi4-IFdJkbp_-ETi_6x-RYOd-qD_4""
    }
  ]
}";
            
            // Dosyayı oluştur
            File.WriteAllText(configPath, defaultConfig);
        }
    }
}
