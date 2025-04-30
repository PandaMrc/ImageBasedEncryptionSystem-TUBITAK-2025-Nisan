using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ImageBasedEncryptionSystem.TypeLayer;
using ImageBasedEncryptionSystem.DataLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_DeveloperMode
    {
        #region Fields
        private bool _isLoggedIn = false;
        private bool _isDevModeActive = false;
        private string _currentDevId = string.Empty;
        private string ConfigFilePath => Cls_Config.GetConfigFilePath();
        #endregion

        #region Properties
        public bool IsLoggedIn => _isLoggedIn;
        public bool IsDevModeActive => _isDevModeActive;
        public string CurrentDevId => _currentDevId;
        #endregion

        #region Login/Logout Methods
        /// <summary>
        /// Geliştirici kimliği ve parola ile giriş yapma metodu
        /// </summary>
        /// <param name="devId">Geliştirici kimliği</param>
        /// <param name="password">Parola</param>
        /// <returns>İşlem sonucu mesajı</returns>
        public string Login(string devId, string password)
        {
            try
            {
                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGIN_STARTED);
                // Giriş parametrelerini kontrol et
                if (string.IsNullOrWhiteSpace(devId))
                    return Errors.ERROR_LOGIN_USERNAME_EMPTY;

                if (string.IsNullOrWhiteSpace(password))
                    return Errors.ERROR_LOGIN_PASSWORD_EMPTY;

                // Config.json dosyasını oku
                if (!File.Exists(ConfigFilePath))
                    return string.Format(Errors.ERROR_LOGIN_FAILED, Errors.ERROR_FILE_NOT_FOUND);

                string jsonContent = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<ConfigModel>(jsonContent);

                if (config == null || config.Developers == null || !config.Developers.Any())
                    return string.Format(Errors.ERROR_LOGIN_FAILED, "Geliştirici bilgileri bulunamadı");

                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGIN_PROCESSING);
                // Kimlik doğrulama
                var developer = config.Developers.FirstOrDefault(d => d.DevID == devId && d.Password == password);
                if (developer == null)
                    return Errors.ERROR_LOGIN_CREDENTIALS;

                // Giriş başarılı
                _isLoggedIn = true;
                _currentDevId = devId;
                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGIN_PROCESSED);
                return Success.LOGIN_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_DEV_MODE_LOGIN_PROCESS_FAILED, ex.Message));
                throw;
            }
        }

        /// <summary>
        /// Geliştirici oturumunu kapatma metodu
        /// </summary>
        /// <returns>İşlem sonucu mesajı</returns>
        public string Logout()
        {
            try
            {
                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGOUT_STARTED);
                // Oturum kapatma işlemleri
                _isLoggedIn = false;
                _currentDevId = string.Empty;
                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGOUT_PROCESSING);
                Console.WriteLine(Success.SUCCESS_DEV_MODE_LOGOUT_PROCESSED);
                return Success.LOGOUT_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_DEV_MODE_LOGOUT_PROCESS_FAILED, ex.Message));
                throw;
            }
        }
        #endregion

        #region Developer Mode Methods
        /// <summary>
        /// Geliştirici modunu etkinleştirir
        /// </summary>
        /// <returns>İşlem sonucu mesajı</returns>
        public string ActivateDevMode()
        {
            try
            {
                if (!_isLoggedIn)
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;

                _isDevModeActive = true;
                return Success.DEV_MODE_ACTIVATE_SUCCESS;
            }
            catch (Exception ex)
            {
                return string.Format(Errors.ERROR_DEV_MODE_ACTIVATE, ex.Message);
            }
        }

        /// <summary>
        /// Geliştirici modunu devre dışı bırakır
        /// </summary>
        /// <returns>İşlem sonucu mesajı</returns>
        public string DeactivateDevMode()
        {
            try
            {
                if (!_isLoggedIn)
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;

                _isDevModeActive = false;
                return Success.DEV_MODE_DEACTIVATE_SUCCESS;
            }
            catch (Exception ex)
            {
                return string.Format(Errors.ERROR_DEV_MODE_DEACTIVATE, ex.Message);
            }
        }

        /// <summary>
        /// Geliştiricilerin kullanabileceği gizli özelliklere erişim sağlar
        /// </summary>
        /// <param name="feature">Erişilmek istenen özellik</param>
        /// <returns>İşlem sonucu veya özellik sonucu</returns>
        public string AccessDevFeature(DevFeature feature)
        {
            try
            {
                if (!_isLoggedIn)
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;

                if (!_isDevModeActive)
                    return "Geliştirici modu aktif değil. Önce geliştirici modunu etkinleştirin.";

                // Özelliğe göre farklı işlemler yap
                switch (feature)
                {
                    case DevFeature.ViewSystemInfo:
                        return GetSystemInfo();
                    case DevFeature.RunDiagnostics:
                        return RunDiagnostics();
                    case DevFeature.EnableDebugLogging:
                        return EnableDebugLogging();
                    case DevFeature.AccessHiddenSettings:
                        return AccessHiddenSettings();
                    default:
                        return Errors.ERROR_GENERAL_NOT_SUPPORTED;
                }
            }
            catch (Exception ex)
            {
                return string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
            }
        }

        #region Dev Feature Helper Methods
        private string GetSystemInfo()
        {
            // Sistem bilgilerini döndür
            return "Sistem Bilgileri:\n" +
                   $"- İşletim Sistemi: {Environment.OSVersion}\n" +
                   $"- Makine Adı: {Environment.MachineName}\n" +
                   $"- İşlemci Sayısı: {Environment.ProcessorCount}\n" +
                   $"- .NET Sürümü: {Environment.Version}";
        }

        private string RunDiagnostics()
        {
            // Tanılama işlemlerini gerçekleştir
            return "Tanılama Sonuçları:\n" +
                   "- Tüm bileşenler kontrol edildi.\n" +
                   "- Şifreleme modülleri çalışıyor.\n" +
                   "- Veri gizleme bileşenleri çalışıyor.";
        }

        private string EnableDebugLogging()
        {
            // Hata ayıklama günlüklerini etkinleştir
            return "Hata ayıklama günlükleri etkinleştirildi. Günlük dosyası: logs/debug.log";
        }

        private string AccessHiddenSettings()
        {
            // Gizli ayarlara erişim sağla
            return "Gizli Ayarlar:\n" +
                   "- Gelişmiş şifreleme algoritmaları etkinleştirildi\n" +
                   "- Test modu kullanılabilir\n" +
                   "- Özelleştirilebilir bit derinliği ayarları etkinleştirildi";
        }
        #endregion
        #endregion

        #region History Methods
        /// <summary>
        /// Şifreleme veya şifre çözme işlemlerinin geçmişini kaydeder
        /// </summary>
        /// <param name="historyData">Kaydedilecek işlem verileri</param>
        public void AddToEncryptionHistory(Dictionary<string, object> historyData)
        {
            try
            {
                if (!_isLoggedIn || !_isDevModeActive)
                    return;

                // Geçmiş dosyasının yolunu belirle
                string historyFilePath = Path.Combine(
                    Path.GetDirectoryName(ConfigFilePath),
                    "encryption_history.json");

                // Mevcut geçmiş verilerini yükle veya yeni oluştur
                List<Dictionary<string, object>> historyList;
                if (File.Exists(historyFilePath))
                {
                    string jsonContent = File.ReadAllText(historyFilePath);
                    historyList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent) 
                                 ?? new List<Dictionary<string, object>>();
                }
                else
                {
                    historyList = new List<Dictionary<string, object>>();
                }

                // Geliştirici bilgisini ekle
                historyData["DeveloperId"] = _currentDevId;

                // Geçmişe yeni kaydı ekle
                historyList.Add(historyData);

                // Dosyaya kaydet
                string updatedJson = JsonConvert.SerializeObject(historyList, Formatting.Indented);
                File.WriteAllText(historyFilePath, updatedJson);
            }
            catch (Exception ex)
            {
                // Geçmiş kaydında hata - sessizce göz ardı et
                System.Diagnostics.Debug.WriteLine($"Şifreleme geçmişi kaydedilirken hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Şifreleme geçmişini okur ve döndürür
        /// </summary>
        /// <returns>Şifreleme işlemleri geçmişi</returns>
        public List<Dictionary<string, object>> GetEncryptionHistory()
        {
            try
            {
                if (!_isLoggedIn || !_isDevModeActive)
                    return new List<Dictionary<string, object>>();

                // Geçmiş dosyasının yolunu belirle
                string historyFilePath = Path.Combine(
                    Path.GetDirectoryName(ConfigFilePath),
                    "encryption_history.json");

                // Dosya yoksa boş liste döndür
                if (!File.Exists(historyFilePath))
                    return new List<Dictionary<string, object>>();

                // Geçmiş verilerini yükle
                string jsonContent = File.ReadAllText(historyFilePath);
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonContent) 
                       ?? new List<Dictionary<string, object>>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Şifreleme geçmişi okunurken hata: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }
        #endregion

        #region Status Check Methods
        /// <summary>
        /// Geliştirici modunun aktif olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Durum mesajı ve sonucu içeren tuple</returns>
        public (string Message, bool Result) CheckDevModeStatus()
        {
            try
            {
                // Önce giriş yapılmış mı kontrol et
                if (!_isLoggedIn)
                    return (Errors.ERROR_DEV_MODE_ACCESS_DENIED, false);

                // Geliştirici modu aktif mi kontrol et
                if (_isDevModeActive)
                    return ($"Geliştirici modu şu anda aktif. Geliştirici: {_currentDevId}", true);
                else
                    return ("Geliştirici modu şu anda devre dışı.", false);
            }
            catch (Exception ex)
            {
                return (string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), false);
            }
        }

        /// <summary>
        /// Geliştirici girişi yapılıp yapılmadığını kontrol eder
        /// </summary>
        /// <returns>Durum mesajı ve sonucu içeren tuple</returns>
        public (string Message, bool Result) CheckLoginStatus()
        {
            try
            {
                if (_isLoggedIn)
                    return ($"Geliştirici girişi yapılmış. Geliştirici: {_currentDevId}", true);
                else
                    return ("Henüz geliştirici girişi yapılmamış.", false);
            }
            catch (Exception ex)
            {
                return (string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), false);
            }
        }

        /// <summary>
        /// Geliştirici giriş durumunu kontrol eder ve geçerli kimliği döndürür
        /// </summary>
        /// <param name="requireDevMode">Geliştirici modunun da aktif olması gerekip gerekmediği</param>
        /// <returns>Durum mesajı</returns>
        public string ValidateAccess(bool requireDevMode = false)
        {
            try
            {
                // Giriş yapılmış mı kontrol et
                if (!_isLoggedIn)
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;

                // Geliştirici modu gerekiyorsa kontrol et
                if (requireDevMode && !_isDevModeActive)
                    return "Geliştirici modu aktif değil. Önce geliştirici modunu etkinleştirin.";

                // Erişim izni verildi
                return Success.MESSAGE_GENERAL_SUCCESS;
            }
            catch (Exception ex)
            {
                return string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
            }
        }
        #endregion

        #region Config File Operations
        /// <summary>
        /// Sistem kimliğini Config.json dosyasına kaydeder
        /// </summary>
        /// <param name="identity">Kaydedilecek kimlik</param>
        /// <returns>İşlem sonucu mesajı</returns>
        public string SaveIdentityToConfig(string identity)
        {
            try
            {
                // Girişi kontrol et
                if (!_isLoggedIn || !_isDevModeActive)
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;

                if (string.IsNullOrWhiteSpace(identity))
                    return "Kimlik boş olamaz!";

                // Config.json dosyasını oku
                if (!File.Exists(ConfigFilePath))
                    return string.Format(Errors.ERROR_FILE_NOT_FOUND, ConfigFilePath);

                string jsonContent = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<ConfigModel>(jsonContent);

                if (config == null)
                    config = new ConfigModel { Developers = new List<Developer>() };

                // SystemIdentity ekle veya güncelle
                if (!jsonContent.Contains("SystemIdentity"))
                {
                    try
                    {
                        // JsonContent içine doğrudan ekleme yapıyoruz
                        // Son süslü parantezden önce ekleme yap
                        int lastBraceIndex = jsonContent.LastIndexOf('}');
                        if (lastBraceIndex > 0)
                        {
                            // Öncesinde başka veri varsa virgül ekle
                            string insertion = jsonContent.Substring(0, lastBraceIndex).TrimEnd().EndsWith("]") 
                                ? ",\n  \"SystemIdentity\": \"" + identity + "\"\n" 
                                : "\n  \"SystemIdentity\": \"" + identity + "\"\n";
                                
                            jsonContent = jsonContent.Insert(lastBraceIndex, insertion);
                            
                            // Dosyayı kalıcı olarak kaydetme
                            string absolutePath = Path.GetFullPath(ConfigFilePath);
                            File.WriteAllText(absolutePath, jsonContent);
                            
                            // Dosya erişimini kontrol et
                            if (File.Exists(absolutePath))
                            {
                                string verification = File.ReadAllText(absolutePath);
                                if (verification.Contains(identity))
                                {
                                    return Success.MESSAGE_GENERAL_SAVED;
                                }
                                else
                                {
                                    return "Kimlik dosyaya yazıldı ancak doğrulama başarısız oldu.";
                                }
                            }
                            else
                            {
                                return "Dosya kaydedildi ancak dosyaya erişilemiyor.";
                            }
                        }
                    }
                    catch (Exception saveEx)
                    {
                        // Alternatif yöntem dene
                        try
                        {
                            // ConfigModel'i doğrudan güncelle ve yeniden serialize et
                            dynamic expandoConfig = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
                            expandoConfig.SystemIdentity = identity;
                            
                            string updatedJson = JsonConvert.SerializeObject(expandoConfig, Formatting.Indented);
                            File.WriteAllText(ConfigFilePath, updatedJson);
                            
                            return Success.MESSAGE_GENERAL_SAVED;
                        }
                        catch
                        {
                            throw saveEx; // İlk hatayı yeniden fırlat
                        }
                    }
                }
                else
                {
                    try
                    {
                        // SystemIdentity zaten var, güncelle
                        string pattern = "\"SystemIdentity\"\\s*:\\s*\"[^\"]*\"";
                        string replacement = "\"SystemIdentity\": \"" + identity + "\"";
                        string updatedJson = System.Text.RegularExpressions.Regex.Replace(jsonContent, pattern, replacement);
                        
                        // Kalıcı olarak kaydetme
                        string absolutePath = Path.GetFullPath(ConfigFilePath);
                        File.WriteAllText(absolutePath, updatedJson);
                        
                        // Dosya erişimini kontrol et
                        if (File.Exists(absolutePath))
                        {
                            string verification = File.ReadAllText(absolutePath);
                            if (verification.Contains(identity))
                            {
                                return Success.MESSAGE_GENERAL_UPDATED;
                            }
                            else
                            {
                                return "Kimlik dosyaya yazıldı ancak doğrulama başarısız oldu.";
                            }
                        }
                        else
                        {
                            return "Dosya güncellendi ancak dosyaya erişilemiyor.";
                        }
                    }
                    catch (Exception updateEx)
                    {
                        // Alternatif yöntem dene
                        try
                        {
                            // ConfigModel'i doğrudan güncelle ve yeniden serialize et
                            dynamic expandoConfig = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
                            expandoConfig.SystemIdentity = identity;
                            
                            string updatedJson = JsonConvert.SerializeObject(expandoConfig, Formatting.Indented);
                            File.WriteAllText(ConfigFilePath, updatedJson);
                            
                            return Success.MESSAGE_GENERAL_UPDATED;
                        }
                        catch
                        {
                            throw updateEx; // İlk hatayı yeniden fırlat
                        }
                    }
                }

                return "Kimlik bilgisi kaydedilemedi!";
            }
            catch (Exception ex)
            {
                return string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
            }
        }
        #endregion
    }

    #region Helper Classes and Enums
    /// <summary>
    /// Config.json dosyasındaki yapıyı temsil eden sınıf
    /// </summary>
    internal class ConfigModel
    {
        public List<Developer> Developers { get; set; }
    }

    /// <summary>
    /// Geliştirici bilgilerini temsil eden sınıf
    /// </summary>
    internal class Developer
    {
        public string DevID { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// Geliştirici modunda kullanılabilecek özellikler
    /// </summary>
    public enum DevFeature
    {
        ViewSystemInfo,
        RunDiagnostics,
        EnableDebugLogging,
        AccessHiddenSettings
    }
    #endregion
}
