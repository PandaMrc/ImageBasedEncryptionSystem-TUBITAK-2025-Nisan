using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    public static class Success
    {
        #region Genel Başarı Mesajları     
        // Yeni sabitler
        public const string MESSAGE_GENERAL_SUCCESS = "İşlem Başarılı!";
        public const string MESSAGE_GENERAL_COMPLETE = "İşlem tamamlandı!";
        public const string MESSAGE_GENERAL_SAVED = "Veriler başarıyla kaydedildi.";
        public const string MESSAGE_GENERAL_UPDATED = "Veriler başarıyla güncellendi.";
        public const string MESSAGE_GENERAL_OPTIMIZED = "İşlem optimize edildi ve başarıyla tamamlandı.";
        public const string MESSAGE_GENERAL_VALIDATED = "Veriler doğrulandı ve geçerli.";
        public const string MESSAGE_GENERAL_INITIALIZED = "Sistem başarıyla başlatıldı.";
        #endregion

        #region Şifreleme/Şifre Çözme Başarı Mesajları
        public const string ENCRYPT_SUCCESS_GENERAL = "Şifreleme İşlemi Başarılı!";
        public const string DECRYPT_SUCCESS_GENERAL = "Şifre Çözme İşlemi Başarılı!";
        public const string ENCRYPT_SUCCESS_AES = "AES şifreleme işlemi başarıyla tamamlandı.";
        public const string DECRYPT_SUCCESS_AES = "AES şifre çözme işlemi başarıyla tamamlandı.";
        public const string ENCRYPT_SUCCESS_RSA = "RSA şifreleme işlemi başarıyla tamamlandı.";
        public const string DECRYPT_SUCCESS_RSA = "RSA şifre çözme işlemi başarıyla tamamlandı.";
        public const string ENCRYPT_SUCCESS_AES_RSA = "AES-RSA hibrit şifreleme başarıyla tamamlandı.";
        public const string DECRYPT_SUCCESS_AES_RSA = "AES-RSA hibrit şifre çözme başarıyla tamamlandı.";
        public const string KEY_GENERATION_SUCCESS = "Şifreleme anahtarları başarıyla oluşturuldu.";
        public const string KEY_VALIDATION_SUCCESS = "Anahtar doğrulama başarılı.";
        public const string PASSWORD_VALIDATION_SUCCESS = "Şifre doğrulama başarılı.";
        #endregion

        #region Görsel İşlemleri Başarı Mesajları
        public const string IMAGE_HIDE_SUCCESS = "Veri görsele başarıyla gizlendi.";
        public const string IMAGE_EXTRACT_SUCCESS = "Veri görselden başarıyla çıkarıldı.";
        public const string IMAGE_SAVE_SUCCESS = "Görsel başarıyla kaydedildi.";
        public const string IMAGE_LOAD_SUCCESS = "Görsel başarıyla yüklendi.";
        public const string IMAGE_PROCESS_SUCCESS = "Görsel işleme başarıyla tamamlandı.";
        #endregion

        #region LSB İşlemleri Başarı Mesajları
        public const string LSB_HIDE_SUCCESS = "LSB yöntemiyle veri başarıyla gizlendi.";
        public const string LSB_EXTRACT_SUCCESS = "LSB yöntemiyle veri başarıyla çıkarıldı.";
        public const string LSB_CAPACITY_SUCCESS = "LSB kapasitesi yeterli bulundu.";
        #endregion

        #region Wavelet İşlemleri Başarı Mesajları
        public const string WAVELET_TRANSFORM_SUCCESS = "Dalgacık (Wavelet) dönüşümü başarıyla uygulandı.";
        public const string WAVELET_INVERSE_SUCCESS = "Ters dalgacık (Wavelet) dönüşümü başarıyla uygulandı.";
        public const string WAVELET_ENCODE_SUCCESS = "Dalgacık tabanlı veri gizleme işlemi başarıyla tamamlandı.";
        public const string WAVELET_DECODE_SUCCESS = "Dalgacık tabanlı veri çıkarma işlemi başarıyla tamamlandı.";
        public const string WAVELET_COEFFICIENT_SUCCESS = "Dalgacık katsayıları başarıyla hesaplandı.";
        public const string WAVELET_ENCRYPT_SUCCESS = "Dalgacık tabanlı şifreleme işlemi başarıyla tamamlandı.";
        public const string WAVELET_DECRYPT_SUCCESS = "Dalgacık tabanlı şifre çözme işlemi başarıyla tamamlandı.";
        #endregion

        #region Dosya İşlemleri Başarı Mesajları
        public const string FILE_OPEN_SUCCESS = "Dosya başarıyla açıldı.";
        public const string FILE_SAVE_SUCCESS = "Dosya başarıyla kaydedildi.";
        #endregion

        #region Kullanıcı İşlemleri Başarı Mesajları
        public const string LOGIN_SUCCESS = "Giriş başarılı!";
        public const string LOGOUT_SUCCESS = "Çıkış başarılı!";
        #endregion

        #region Geliştirici Modu Başarı Mesajları
        public const string DEV_MODE_ACTIVATE_SUCCESS = "Geliştirici modu başarıyla etkinleştirildi.";
        public const string DEV_MODE_DEACTIVATE_SUCCESS = "Geliştirici modu başarıyla devre dışı bırakıldı.";
        public const string DEV_MODE_LOGIN_SUCCESS = "Geliştirici girişi başarılı.";
        public const string DEV_MODE_LOGOUT_SUCCESS = "Geliştirici oturumu başarıyla kapatıldı.";
        public const string DEV_MODE_FEATURE_ACTIVATED = "Geliştirici özelliği başarıyla etkinleştirildi: {0}";
        #endregion

        #region Analiz Başarı Mesajları
        public const string ANALYSIS_PROCESS_SUCCESS = "Analiz işlemi başarıyla tamamlandı.";
        public const string ANALYSIS_COMPARE_SUCCESS = "Karşılaştırma analizi başarılı.";
        #endregion

        #region İşlem Sonuçları
        public const string RESULT_SUCCESS_WITH_COUNT = "{0} adet öğe başarıyla işlendi.";
        public const string RESULT_SUCCESS_RATIO = "İşlem %{0} başarı oranıyla tamamlandı.";
        public const string RESULT_SUCCESS_TIME = "İşlem {0} sürede tamamlandı.";
        #endregion
    }
} 