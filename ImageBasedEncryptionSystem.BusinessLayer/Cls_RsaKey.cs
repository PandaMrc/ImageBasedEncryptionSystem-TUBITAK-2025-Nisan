using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ImageBasedEncryptionSystem.DataLayer;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// RSA anahtarlarını yöneten ve oluşturan sınıf
    /// </summary>
    public class Cls_RsaKey : IDisposable
    {
        private string _currentSystemIdentity;
        private string _publicKey;
        private string _privateKey;
        private Timer _identityCheckTimer;
        private readonly Cls_Config _config;
        private readonly object _lockObject = new object();
        private bool _disposed = false;

        public string PublicKey => _publicKey;
        public string PrivateKey => _privateKey;

        /// <summary>
        /// Cls_RsaKey sınıfının yapıcı metodu
        /// </summary>
        public Cls_RsaKey()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("RSA Anahtar Yöneticisi başlatılıyor...");
                _config = new Cls_Config();
                InitializeKeys();
                StartIdentityCheckTimer();
                System.Diagnostics.Debug.WriteLine(Success.SYSTEM_INIT_SUCCESS);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Anahtar Yöneticisi başlatma hatası: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), ex);
            }
        }

        /// <summary>
        /// RSA anahtarlarını başlatır
        /// </summary>
        private void InitializeKeys()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SystemIdentity değeri alınıyor...");
                string systemIdentity = _config.GetSystemIdentity();

                if (string.IsNullOrEmpty(systemIdentity))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_GENERAL_UNEXPECTED);
                    throw new ArgumentException(Errors.ERROR_GENERAL_UNEXPECTED);
                }

                if (systemIdentity.Length < 10 || systemIdentity.Length > 50)
                {
                    System.Diagnostics.Debug.WriteLine($"SystemIdentity uzunluk hatası: {systemIdentity.Length} karakter");
                    throw new ArgumentException("SystemIdentity değeri geçersiz. Minimum 10, maksimum 50 karakter olmalıdır.");
                }

                System.Diagnostics.Debug.WriteLine($"SystemIdentity değeri alındı: {systemIdentity}");
                _currentSystemIdentity = systemIdentity;
                GenerateKeys(systemIdentity);
                System.Diagnostics.Debug.WriteLine(Success.KEY_GENERATION_SUCCESS);
            }
            catch (ArgumentException ae)
            {
                System.Diagnostics.Debug.WriteLine($"SystemIdentity doğrulama hatası: {ae.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Anahtar Başlatma Hatası: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), ex);
            }
        }

        /// <summary>
        /// SystemIdentity değişikliklerini kontrol eden zamanlayıcıyı başlatır
        /// </summary>
        private void StartIdentityCheckTimer()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SystemIdentity kontrol zamanlayıcısı başlatılıyor...");
                _identityCheckTimer = new Timer(CheckIdentityChange, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                System.Diagnostics.Debug.WriteLine("SystemIdentity kontrol zamanlayıcısı başlatıldı.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Zamanlayıcı başlatma hatası: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), ex);
            }
        }

        /// <summary>
        /// SystemIdentity değişikliğini kontrol eder
        /// </summary>
        private void CheckIdentityChange(object state)
        {
            try
            {
                string newIdentity = _config.GetSystemIdentity();
                
                if (newIdentity != _currentSystemIdentity)
                {
                    lock (_lockObject)
                    {
                        if (newIdentity != _currentSystemIdentity)
                        {
                            System.Diagnostics.Debug.WriteLine($"SystemIdentity değişikliği tespit edildi: {_currentSystemIdentity} -> {newIdentity}");
                            
                            if (string.IsNullOrEmpty(newIdentity) || newIdentity.Length < 10 || newIdentity.Length > 50)
                            {
                                System.Diagnostics.Debug.WriteLine($"Yeni SystemIdentity değeri geçersiz: {newIdentity}");
                                return;
                            }

                            _currentSystemIdentity = newIdentity;
                            GenerateKeys(newIdentity);
                            System.Diagnostics.Debug.WriteLine(Success.KEY_GENERATION_SUCCESS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SystemIdentity Kontrol Hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Verilen SystemIdentity değerine göre RSA anahtarları oluşturur
        /// </summary>
        /// <param name="systemIdentity">Kimlik değeri</param>
        private void GenerateKeys(string systemIdentity)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"RSA anahtarları oluşturuluyor (SystemIdentity: {systemIdentity})...");

                // Kimlik değerinden SHA256 hash oluştur
                byte[] seedBytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    seedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(systemIdentity));
                }

                // Hash değerini kullanarak rastgele sayı üreteci için tohum oluştur
                int seed = BitConverter.ToInt32(seedBytes, 0);
                using (var rng = new RNGCryptoServiceProvider())
                {
                    // RSA anahtarlarını oluştur
                    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072))
                    {
                        // Anahtarları XML formatında al
                        _publicKey = rsa.ToXmlString(false); // Sadece public key
                        _privateKey = rsa.ToXmlString(true); // Private ve public key
                    }
                }

                System.Diagnostics.Debug.WriteLine(Success.KEY_GENERATION_SUCCESS);
            }
            catch (CryptographicException ce)
            {
                System.Diagnostics.Debug.WriteLine($"Kriptografik hata: {ce.Message}");
                throw new CryptographicException(string.Format(Errors.ERROR_RSA_KEY_GENERATION, ce.Message), ce);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RSA Anahtar Oluşturma Hatası: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message), ex);
            }
        }

        /// <summary>
        /// Kaynakları temizler
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Kaynakları temizler
        /// </summary>
        /// <param name="disposing">Yönetilen kaynakların temizlenip temizlenmeyeceği</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("RSA Anahtar Yöneticisi kaynakları temizleniyor...");
                        _identityCheckTimer?.Dispose();
                        System.Diagnostics.Debug.WriteLine("RSA Anahtar Yöneticisi kaynakları temizlendi.");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Kaynak temizleme hatası: {ex.Message}");
                    }
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Yıkıcı metot
        /// </summary>
        ~Cls_RsaKey()
        {
            Dispose(false);
        }
    }
} 