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
        #region Geliştirici Modu Başarı Mesajları
        public const string LOGIN_SUCCESS = "[SUCCESS] Giriş başarılı.";
        public const string LOGOUT_SUCCESS = "[SUCCESS] Çıkış başarılı.";
        public const string DEV_MODE_ACTIVATE_SUCCESS = "[SUCCESS] Geliştirici modu başarıyla etkinleştirildi.";
        public const string DEV_MODE_DEACTIVATE_SUCCESS = "[SUCCESS] Geliştirici modu başarıyla devre dışı bırakıldı.";
        public const string MESSAGE_GENERAL_SUCCESS = "[SUCCESS] İşlem başarıyla tamamlandı.";
        public const string MESSAGE_GENERAL_SAVED = "[SUCCESS] Bilgiler başarıyla kaydedildi.";
        public const string MESSAGE_GENERAL_UPDATED = "[SUCCESS] Bilgiler başarıyla güncellendi.";
        public const string DEV_MODE_LOGIN_SUCCESS = "[SUCCESS] Geliştirici girişi başarılı. Geliştirici modu etkinleştirildi.";
        public const string DEVELOPER_LOGOUT_SUCCESS = "[SUCCESS] Geliştirici Hesabından Çıkış Yaptınız";
        #endregion

        #region Görsel İşlemler Başarı Mesajları
        public const string IMAGE_LOAD_SUCCESS = "[SUCCESS] Resim başarıyla yüklendi.";
        #endregion

        #region Şifreleme İşlemleri Başarı Mesajları
        public const string ENCRYPTION_SUCCESS = "[SUCCESS] Şifreleme işlemi başarıyla tamamlandı.";
        public const string DECRYPTION_SUCCESS = "[SUCCESS] Şifre çözme işlemi başarıyla tamamlandı.";
        #endregion

        #region Diğer Başarı Mesajları
        public const string LOGIN_FORM_SUCCESS = "[SUCCESS] Giriş formu başarıyla yüklendi.";
        public const string LOGIN_CANCEL_SUCCESS = "[SUCCESS] Giriş işlemi başarıyla iptal edildi.";
        #endregion

        #region Admin Form Başarı Mesajları
        public const string ADMIN_FORM_SUCCESS = "[SUCCESS] Admin formu başarıyla yüklendi.";
        public const string ADMIN_RANDOM_IDENTITY_SUCCESS = "[SUCCESS] Rastgele kimlik başarıyla oluşturuldu.";
        public const string ADMIN_IDENTITY_SAVED_SUCCESS = "[SUCCESS] Kimlik ve RSA anahtarları başarıyla güncellendi.";
        public const string ADMIN_IDENTITY_RESET_SUCCESS = "[SUCCESS] Kimlik varsayılan değere sıfırlandı.";
        public const string ADMIN_RSA_KEYS_UPDATE_SUCCESS = "[SUCCESS] RSA anahtarları başarıyla güncellendi.";
        #endregion
    }
}