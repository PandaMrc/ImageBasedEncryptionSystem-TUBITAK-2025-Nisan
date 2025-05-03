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
                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGIN_STARTED);
                // Giriş parametrelerini kontrol et
                if (string.IsNullOrWhiteSpace(devId))
                {
                    Console.WriteLine(Errors.ERROR_LOGIN_USERNAME_EMPTY);
                    return Errors.ERROR_LOGIN_USERNAME_EMPTY;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine(Errors.ERROR_LOGIN_PASSWORD_EMPTY);
                    return Errors.ERROR_LOGIN_PASSWORD_EMPTY;
                }

                // Config.json dosyasını oku
                if (!File.Exists(ConfigFilePath))
                {
                    Console.WriteLine(Errors.ERROR_FILE_NOT_FOUND);
                    return string.Format(Errors.ERROR_LOGIN_FAILED, Errors.ERROR_FILE_NOT_FOUND);
                }

                string jsonContent = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<ConfigModel>(jsonContent);

                if (config == null || config.Developers == null || !config.Developers.Any())
                {
                    Console.WriteLine(Errors.ERROR_NO_DEVELOPERS_FOUND);
                    return string.Format(Errors.ERROR_LOGIN_FAILED, "Geliştirici bilgileri bulunamadı");
                }

                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGIN_PROCESSING);
                // Kimlik doğrulama
                var developer = config.Developers.FirstOrDefault(d => d.DevID == devId && d.Password == password);
                if (developer == null)
                {
                    Console.WriteLine(Errors.ERROR_LOGIN_CREDENTIALS);
                    return Errors.ERROR_LOGIN_CREDENTIALS;
                }

                // Giriş başarılı
                _isLoggedIn = true;
                _currentDevId = devId;
                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGIN_PROCESSED);
                return Success.LOGIN_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.ERROR_DEV_MODE_LOGIN_PROCESS_FAILED, ex.Message));
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
                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGOUT_STARTED);
                // Oturum kapatma işlemleri
                _isLoggedIn = false;
                _currentDevId = string.Empty;
                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGOUT_PROCESSING);
                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGOUT_PROCESSED);
                return Success.LOGOUT_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.ERROR_DEV_MODE_LOGOUT_PROCESS_FAILED, ex.Message));
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
                Console.WriteLine(Debug.DEBUG_DEV_MODE_ACTIVATE_STARTED);
                if (!_isLoggedIn)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;
                }

                _isDevModeActive = true;
                Console.WriteLine(Debug.DEBUG_DEV_MODE_ACTIVATE_COMPLETED);
                return Success.DEV_MODE_ACTIVATE_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.ERROR_DEV_MODE_ACTIVATE, ex.Message));
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
                Console.WriteLine(Debug.DEBUG_DEV_MODE_DEACTIVATE_STARTED);
                if (!_isLoggedIn)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;
                }

                _isDevModeActive = false;
                Console.WriteLine(Debug.DEBUG_DEV_MODE_DEACTIVATE_COMPLETED);
                return Success.DEV_MODE_DEACTIVATE_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.ERROR_DEV_MODE_DEACTIVATE, ex.Message));
                return string.Format(Errors.ERROR_DEV_MODE_DEACTIVATE, ex.Message);
            }
        }

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
                Console.WriteLine(Debug.DEBUG_ENCRYPTION_HISTORY_ADD_STARTED);
                if (!_isLoggedIn || !_isDevModeActive)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return;
                }

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
                Console.WriteLine(Debug.DEBUG_ENCRYPTION_HISTORY_ADD_COMPLETED);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Debug.ERROR_ENCRYPTION_HISTORY_ADD_FAILED, ex.Message));
                System.Diagnostics.Debug.WriteLine($"Şifreleme geçmişi kaydedilirken hata: {ex.Message}");
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
                Console.WriteLine(Debug.DEBUG_DEV_MODE_STATUS_CHECK_STARTED);
                // Önce giriş yapılmış mı kontrol et
                if (!_isLoggedIn)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return (Errors.ERROR_DEV_MODE_ACCESS_DENIED, false);
                }

                // Geliştirici modu aktif mi kontrol et
                if (_isDevModeActive)
                {
                    Console.WriteLine(Debug.DEBUG_DEV_MODE_ACTIVE);
                    return ($"Geliştirici modu şu anda aktif. Geliştirici: {_currentDevId}", true);
                }
                else
                {
                    Console.WriteLine(Debug.DEBUG_DEV_MODE_INACTIVE);
                    return ("Geliştirici modu şu anda devre dışı.", false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_STATUS_CHECK_FAILED, ex.Message));
                return (string.Format(Errors.ERROR_STATUS_CHECK_FAILED, ex.Message), false);
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
                Console.WriteLine(Debug.DEBUG_LOGIN_STATUS_CHECK_STARTED);
                if (_isLoggedIn)
                {
                    Console.WriteLine(Debug.DEBUG_LOGIN_ACTIVE);
                    return ($"Geliştirici girişi yapılmış. Geliştirici: {_currentDevId}", true);
                }
                else
                {
                    Console.WriteLine(Debug.DEBUG_LOGIN_INACTIVE);
                    return ("Henüz geliştirici girişi yapılmamış.", false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_LOGIN_STATUS_CHECK_FAILED, ex.Message));
                return (string.Format(Errors.ERROR_LOGIN_STATUS_CHECK_FAILED, ex.Message), false);
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
                Console.WriteLine(Debug.DEBUG_ACCESS_VALIDATION_STARTED);
                // Giriş yapılmış mı kontrol et
                if (!_isLoggedIn)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;
                }

                // Geliştirici modu gerekiyorsa kontrol et
                if (requireDevMode && !_isDevModeActive)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_INACTIVE);
                    return "Geliştirici modu aktif değil. Önce geliştirici modunu etkinleştirin.";
                }

                // Erişim izni verildi
                Console.WriteLine(Debug.DEBUG_ACCESS_VALIDATION_COMPLETED);
                return Success.MESSAGE_GENERAL_SUCCESS;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_VALIDATE_ACCESS_FAILED, ex.Message));
                return string.Format(Errors.ERROR_VALIDATE_ACCESS_FAILED, ex.Message);
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
                Console.WriteLine(Debug.DEBUG_SAVE_IDENTITY_STARTED);
                // Girişi kontrol et
                if (!_isLoggedIn || !_isDevModeActive)
                {
                    Console.WriteLine(Errors.ERROR_DEV_MODE_ACCESS_DENIED);
                    return Errors.ERROR_DEV_MODE_ACCESS_DENIED;
                }

                if (string.IsNullOrWhiteSpace(identity))
                {
                    Console.WriteLine(Errors.ERROR_IDENTITY_EMPTY);
                    return "Kimlik boş olamaz!";
                }

                // Config.json dosyasını oku
                if (!File.Exists(ConfigFilePath))
                {
                    Console.WriteLine(Errors.ERROR_FILE_NOT_FOUND);
                    return string.Format(Errors.ERROR_FILE_NOT_FOUND, ConfigFilePath);
                }

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
                                    Console.WriteLine(Debug.DEBUG_SAVE_IDENTITY_COMPLETED);
                                    return Success.MESSAGE_GENERAL_SAVED;
                                }
                                else
                                {
                                    Console.WriteLine(Errors.ERROR_IDENTITY_VERIFICATION_FAILED);
                                    return "Kimlik dosyaya yazıldı ancak doğrulama başarısız oldu.";
                                }
                            }
                            else
                            {
                                Console.WriteLine(Errors.ERROR_FILE_ACCESS);
                                return "Dosya kaydedildi ancak dosyaya erişilemiyor.";
                            }
                        }
                    }
                    catch (Exception saveEx)
                    {
                        Console.WriteLine(string.Format(Debug.ERROR_SAVE_IDENTITY_FAILED, saveEx.Message));
                        // Alternatif yöntem dene
                        try
                        {
                            // ConfigModel'i doğrudan güncelle ve yeniden serialize et
                            dynamic expandoConfig = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
                            expandoConfig.SystemIdentity = identity;

                            string updatedJson = JsonConvert.SerializeObject(expandoConfig, Formatting.Indented);
                            File.WriteAllText(ConfigFilePath, updatedJson);

                            Console.WriteLine(Debug.DEBUG_SAVE_IDENTITY_COMPLETED);
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
                                Console.WriteLine(Debug.DEBUG_SAVE_IDENTITY_COMPLETED);
                                return Success.MESSAGE_GENERAL_UPDATED;
                            }
                            else
                            {
                                Console.WriteLine(Errors.ERROR_IDENTITY_VERIFICATION_FAILED);
                                return "Kimlik dosyaya yazıldı ancak doğrulama başarısız oldu.";
                            }
                        }
                        else
                        {
                            Console.WriteLine(Errors.ERROR_FILE_ACCESS);
                            return "Dosya güncellendi ancak dosyaya erişilemiyor.";
                        }
                    }
                    catch (Exception updateEx)
                    {
                        Console.WriteLine(string.Format(Debug.ERROR_SAVE_IDENTITY_FAILED, updateEx.Message));
                        // Alternatif yöntem dene
                        try
                        {
                            // ConfigModel'i doğrudan güncelle ve yeniden serialize et
                            dynamic expandoConfig = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
                            expandoConfig.SystemIdentity = identity;

                            string updatedJson = JsonConvert.SerializeObject(expandoConfig, Formatting.Indented);
                            File.WriteAllText(ConfigFilePath, updatedJson);

                            Console.WriteLine(Debug.DEBUG_SAVE_IDENTITY_COMPLETED);
                            return Success.MESSAGE_GENERAL_UPDATED;
                        }
                        catch
                        {
                            throw updateEx; // İlk hatayı yeniden fırlat
                        }
                    }
                }

                Console.WriteLine(Errors.ERROR_IDENTITY_SAVE_FAILED);
                return "Kimlik bilgisi kaydedilemedi!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_SAVE_ID_CONFIG_FAILED, ex.Message));
                return string.Format(Errors.ERROR_SAVE_ID_CONFIG_FAILED, ex.Message);
            }
        }
        #endregion
    }
}
    
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
    

