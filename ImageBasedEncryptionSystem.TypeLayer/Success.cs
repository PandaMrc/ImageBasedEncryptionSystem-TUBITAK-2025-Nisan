using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    /// <summary>
    /// Uygulama genelinde kullanılan başarı mesajlarını içeren statik sınıf
    /// </summary>
    public static class Success
    {
        #region Genel Başarı Mesajları
        public const string MESSAGE_GENERAL_SUCCESS = "İşlem başarıyla tamamlandı.";
        public const string MESSAGE_GENERAL_SAVED = "Bilgiler başarıyla kaydedildi.";
        public const string MESSAGE_GENERAL_UPDATED = "Bilgiler başarıyla güncellendi.";
        public const string MESSAGE_GENERAL_DELETED = "Bilgiler başarıyla silindi.";
        public const string MESSAGE_GENERAL_LOADED = "Bilgiler başarıyla yüklendi.";
        #endregion

        #region Şifreleme/Çözme İşlemleri Başarı Mesajları
        public const string ENCRYPT_SUCCESS_GENERAL = "Veri başarıyla şifrelendi ve görüntüye gizlendi.";
        public const string DECRYPT_SUCCESS_GENERAL = "Şifreli metin başarıyla çözüldü.";
        public const string ENCRYPT_SUCCESS_DETAILED = "Metin AES ile şifrelendi, RSA ile anahtar korundu ve görüntüye gizlendi.";
        public const string DECRYPT_SUCCESS_DETAILED = "Görüntüden çıkarılan şifreli metin başarıyla çözüldü.";
        public const string ENCRYPT_SUCCESS_AES = "Metin AES algoritması ile başarıyla şifrelendi.";
        public const string DECRYPT_SUCCESS_AES = "Metin AES algoritması ile başarıyla çözüldü.";
        public const string ENCRYPT_SUCCESS_RSA = "Metin RSA algoritması ile başarıyla şifrelendi.";
        public const string DECRYPT_SUCCESS_RSA = "Metin RSA algoritması ile başarıyla çözüldü.";
        public const string ENCRYPT_DATA_HIDDEN = "Veri görüntüye başarıyla gizlendi.";
        public const string DECRYPT_DATA_EXTRACTED = "Veri görüntüden başarıyla çıkarıldı.";
        #endregion

        #region Anahtar İşlemleri Başarı Mesajları
        public const string KEY_GENERATION_SUCCESS = "Anahtar başarıyla oluşturuldu.";
        public const string KEY_SAVE_SUCCESS = "Anahtar başarıyla kaydedildi.";
        #endregion

        #region Dosya İşlemleri Başarı Mesajları
        public const string FILE_SAVE_SUCCESS = "Dosya başarıyla kaydedildi.";
        public const string FILE_OPEN_SUCCESS = "Dosya başarıyla açıldı.";
        #endregion

        #region Geliştirici Modu Başarı Mesajları
        public const string DEV_MODE_ACTIVATED = "Geliştirici modu başarıyla etkinleştirildi.";
        public const string DEV_MODE_DEACTIVATED = "Geliştirici modu başarıyla devre dışı bırakıldı.";
        public const string DEV_MODE_LOGIN_SUCCESS = "Geliştirici girişi başarıyla yapıldı.";
        public const string DEV_MODE_LOGOUT_SUCCESS = "Geliştirici oturumu başarıyla kapatıldı.";
        public const string LOGIN_SUCCESS = "Giriş başarıyla yapıldı.";
        public const string LOGOUT_SUCCESS = "Oturum başarıyla kapatıldı.";
        public const string DEV_MODE_ACTIVATE_SUCCESS = "Geliştirici modu başarıyla etkinleştirildi.";
        public const string DEV_MODE_DEACTIVATE_SUCCESS = "Geliştirici modu başarıyla devre dışı bırakıldı.";
        #endregion

        #region Görsel İşleme Başarı Mesajları
        public const string IMAGE_LOAD_SUCCESS = "Görüntü başarıyla yüklendi.";
        public const string IMAGE_SAVE_SUCCESS = "Görüntü başarıyla kaydedildi.";
        public const string IMAGE_PROCESS_SUCCESS = "Görüntü işleme başarıyla tamamlandı.";
        #endregion

        #region Kimlik İşlemleri Başarı Mesajları
        public const string IDENTITY_CREATE_SUCCESS = "Kimlik başarıyla oluşturuldu.";
        public const string IDENTITY_UPDATE_SUCCESS = "Kimlik başarıyla güncellendi.";
        public const string IDENTITY_SAVE_SUCCESS = "Kimlik bilgileri başarıyla kaydedildi.";
        public const string IDENTITY_GENERATE_SUCCESS = "Rastgele kimlik başarıyla oluşturuldu.";
        #endregion

        #region Sistem İşlemleri Başarı Mesajları
        public const string SYSTEM_INIT_SUCCESS = "Sistem başlatma işlemi başarılı.";
        public const string SYSTEM_CONFIG_SAVE_SUCCESS = "Sistem yapılandırması başarıyla kaydedildi.";
        public const string SYSTEM_CONFIG_LOAD_SUCCESS = "Sistem yapılandırması başarıyla yüklendi.";
        #endregion
    }
}