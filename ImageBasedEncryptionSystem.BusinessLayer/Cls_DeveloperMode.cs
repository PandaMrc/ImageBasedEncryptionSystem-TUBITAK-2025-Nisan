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

                // Geliştirici bilgilerini al
                var developers = Cls_Config.GetDeveloperID();
                if (developers == null || !developers.Any())
                {
                    Console.WriteLine(Errors.ERROR_NO_DEVELOPERS_FOUND);
                    return string.Format(Errors.ERROR_LOGIN_FAILED, "Geliştirici bilgileri bulunamadı");
                }

                Console.WriteLine(Debug.DEBUG_DEV_MODE_LOGIN_PROCESSING);
                // Kimlik doğrulama
                var developer = developers.FirstOrDefault(d => d.DevID == devId && d.Password == password);
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
    

