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

        #region Geliştirici Modu İşlemleri Başarı Mesajları
        public const string SUCCESS_DEV_MODE_LOGIN_STARTED = "Login: Başlatılıyor...";
        public const string SUCCESS_DEV_MODE_LOGIN_PROCESSING = "Login: Kimlik doğrulama yapılıyor...";
        public const string SUCCESS_DEV_MODE_LOGIN_PROCESSED = "Login: Giriş başarıyla yapıldı.";
        public const string SUCCESS_DEV_MODE_LOGOUT_STARTED = "Logout: Başlatılıyor...";
        public const string SUCCESS_DEV_MODE_LOGOUT_PROCESSING = "Logout: Oturum kapatılıyor...";
        public const string SUCCESS_DEV_MODE_LOGOUT_PROCESSED = "Logout: Oturum başarıyla kapatıldı.";
        #endregion

        #region Görsel İşleme Başarı Mesajları
        public const string IMAGE_LOAD_SUCCESS = "Görüntü başarıyla yüklendi.";
        public const string IMAGE_SAVE_SUCCESS = "Görüntü başarıyla kaydedildi.";
        public const string IMAGE_PROCESS_SUCCESS = "Görüntü işleme başarıyla tamamlandı.";
        public const string IMAGE_ENCRYPTION_SUCCESS = "Görsel başarıyla şifrelendi.";
        #endregion

        #region Kimlik İşlemleri Başarı Mesajları
        public const string IDENTITY_CREATE_SUCCESS = "Kimlik başarıyla oluşturuldu.";
        public const string IDENTITY_UPDATE_SUCCESS = "Kimlik başarıyla güncellendi.";
        public const string IDENTITY_SAVE_SUCCESS = "Kimlik bilgileri başarıyla kaydedildi.";
        public const string IDENTITY_GENERATE_SUCCESS = "Rastgele kimlik başarıyla oluşturuldu.";
        #endregion

        #region Kimlik Oluşturma İşlemleri Başarı Mesajları
        public const string SUCCESS_IDENTITY_CREATE_STARTED = "CreateRandomIdentity: Başlatılıyor...";
        public const string SUCCESS_IDENTITY_CREATE_PROCESSING = "CreateRandomIdentity: Kimlik oluşturuluyor...";
        public const string SUCCESS_IDENTITY_CREATE_PROCESSED = "CreateRandomIdentity: Kimlik başarıyla oluşturuldu.";
        #endregion

        #region Sistem İşlemleri Başarı Mesajları
        public const string SYSTEM_INIT_SUCCESS = "Sistem başlatma işlemi başarılı.";
        public const string SYSTEM_CONFIG_SAVE_SUCCESS = "Sistem yapılandırması başarıyla kaydedildi.";
        public const string SYSTEM_CONFIG_LOAD_SUCCESS = "Sistem yapılandırması başarıyla yüklendi.";
        #endregion

        #region AES İşlemleri Başarı Mesajları
        public const string SUCCESS_AES_GENERATE_KEY_STARTED = "GenerateAESKey: Başlatılıyor...";
        public const string SUCCESS_AES_GENERATE_KEY_COMPLETED = "GenerateAESKey: Anahtar başarıyla oluşturuldu.";
        public const string SUCCESS_AES_ENCRYPT_STARTED = "Encrypt: Başlatılıyor...";
        public const string SUCCESS_AES_ENCRYPT_ENGINE_INIT = "Encrypt: Şifreleme motoru başlatılıyor...";
        public const string SUCCESS_AES_ENCRYPT_PROCESSING = "Encrypt: Metin şifreleniyor...";
        public const string SUCCESS_AES_ENCRYPT_PROCESSED = "Encrypt: Metin başarıyla şifrelendi.";
        public const string SUCCESS_AES_DECRYPT_STARTED = "Decrypt: Başlatılıyor...";
        public const string SUCCESS_AES_DECRYPT_ENGINE_INIT = "Decrypt: Şifre çözme motoru başlatılıyor...";
        public const string SUCCESS_AES_DECRYPT_PROCESSING = "Decrypt: Metin çözülüyor...";
        public const string SUCCESS_AES_DECRYPT_PROCESSED = "Decrypt: Metin başarıyla çözüldü.";
        #endregion

        #region RSA İşlemleri Başarı Mesajları
        public const string SUCCESS_RSA_GET_SYSTEM_IDENTITY_STARTED = "GetSystemIdentity: Başlatılıyor...";
        public const string SUCCESS_RSA_GET_SYSTEM_IDENTITY_DEFAULT = "GetSystemIdentity: Config dosyası bulunamadı, varsayılan kimlik kullanılacak.";
        public const string SUCCESS_RSA_GET_SYSTEM_IDENTITY_INVALID = "GetSystemIdentity: Geçersiz kimlik, varsayılan kimlik kullanılacak.";
        public const string SUCCESS_RSA_GET_SYSTEM_IDENTITY_LOADED = "GetSystemIdentity: Kimlik başarıyla yüklendi.";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_STARTED = "EnsureKeyPair: Başlatılıyor...";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_EXISTING = "EnsureKeyPair: Mevcut anahtar çifti kullanılacak.";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_GENERATING = "EnsureKeyPair: Yeni anahtar çifti oluşturuluyor...";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_GENERATED = "EnsureKeyPair: Anahtar çifti başarıyla oluşturuldu.";
        public const string SUCCESS_RSA_ENCRYPT_STARTED = "Encrypt: Başlatılıyor...";
        public const string SUCCESS_RSA_ENCRYPT_ENGINE_INIT = "Encrypt: Şifreleme motoru başlatılıyor...";
        public const string SUCCESS_RSA_ENCRYPT_PROCESSING = "Encrypt: Metin şifreleniyor...";
        public const string SUCCESS_RSA_ENCRYPT_PROCESSED = "Encrypt: Metin başarıyla şifrelendi.";
        public const string SUCCESS_RSA_DECRYPT_STARTED = "Decrypt: Başlatılıyor...";
        public const string SUCCESS_RSA_DECRYPT_ENGINE_INIT = "Decrypt: Şifre çözme motoru başlatılıyor...";
        public const string SUCCESS_RSA_DECRYPT_PROCESSING = "Decrypt: Metin çözülüyor...";
        public const string SUCCESS_RSA_DECRYPT_PROCESSED = "Decrypt: Metin başarıyla çözüldü.";
        public const string SUCCESS_RSA_GET_PUBLIC_KEY_PEM_STARTED = "GetPublicKeyPem: Başlatılıyor...";
        public const string SUCCESS_RSA_GET_PUBLIC_KEY_PEM_EXPORTING = "GetPublicKeyPem: Public key dışa aktarılıyor...";
        public const string SUCCESS_RSA_GET_PUBLIC_KEY_PEM_EXPORTED = "GetPublicKeyPem: Public key başarıyla dışa aktarıldı.";
        public const string SUCCESS_RSA_GET_PRIVATE_KEY_PEM_STARTED = "GetPrivateKeyPem: Başlatılıyor...";
        public const string SUCCESS_RSA_GET_PRIVATE_KEY_PEM_EXPORTING = "GetPrivateKeyPem: Private key dışa aktarılıyor...";
        public const string SUCCESS_RSA_GET_PRIVATE_KEY_PEM_EXPORTED = "GetPrivateKeyPem: Private key başarıyla dışa aktarıldı.";
        #endregion

        #region Wavelet İşlemleri Başarı Mesajları
        public const string SUCCESS_WAVELET_EMBED_TEXT_STARTED = "EmbedTextInImage: Başlatılıyor...";
        public const string SUCCESS_WAVELET_EMBED_TEXT_PROCESSING = "EmbedTextInImage: Metin gömülüyor...";
        public const string SUCCESS_WAVELET_EMBED_TEXT_PROCESSED = "EmbedTextInImage: Metin başarıyla gömüldü.";
        public const string SUCCESS_WAVELET_EXTRACT_TEXT_STARTED = "ExtractTextFromImage: Başlatılıyor...";
        public const string SUCCESS_WAVELET_EXTRACT_TEXT_PROCESSING = "ExtractTextFromImage: Metin çıkarılıyor...";
        public const string SUCCESS_WAVELET_EXTRACT_TEXT_PROCESSED = "ExtractTextFromImage: Metin başarıyla çıkarıldı.";
        public const string SUCCESS_WAVELET_TRANSPARENCY_STARTED = "EnsureTransparency: Başlatılıyor...";
        public const string SUCCESS_WAVELET_TRANSPARENCY_PROCESSING = "EnsureTransparency: Transparanlık sağlanıyor...";
        public const string SUCCESS_WAVELET_TRANSPARENCY_PROCESSED = "EnsureTransparency: Transparanlık başarıyla sağlandı.";
        #endregion

        #region Kimlik ve RSA İşlemleri Başarı Mesajları
        public const string SUCCESS_IDENTITY_SAVED = "Kimlik başarıyla kaydedildi.";
        public const string SUCCESS_RSA_KEY_GENERATED = "RSA anahtar çifti başarıyla oluşturuldu.";
        #endregion

        #region RSA Anahtar Üretimi Başarı Mesajları
        public const string DEBUG_SYSTEM_IDENTITY_RECEIVED = "[DEBUG] Alınan SystemIdentity: {0}";
        public const string DEBUG_SYSTEM_IDENTITY_BYTE_ARRAY = "[DEBUG] SystemIdentity byte dizisi: {0}";
        #endregion
    }
}