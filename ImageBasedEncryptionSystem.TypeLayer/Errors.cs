using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    public static class Errors
    {
        #region Genel Hatalar
        public const string ERROR_GENERAL_CANCELED = "[ERROR] İşlem kullanıcı tarafından iptal edildi.";
        public const string ERROR_GENERAL_UNEXPECTED = "[ERROR] Beklenmeyen bir hata oluştu: {0}";
        #endregion

        #region Geliştirici Modu Hataları
        public const string ERROR_LOGIN_USERNAME_EMPTY = "[ERROR] Giriş kullanıcı adı boş.";
        public const string ERROR_LOGIN_PASSWORD_EMPTY = "[ERROR] Giriş parolası boş.";
        public const string ERROR_FILE_NOT_FOUND = "[ERROR] Dosya bulunamadı.";
        public const string ERROR_NO_DEVELOPERS_FOUND = "[ERROR] Geliştirici bilgileri bulunamadı.";
        public const string ERROR_LOGIN_CREDENTIALS = "[ERROR] Giriş kimlik bilgileri hatalı.";
        public const string ERROR_DEV_MODE_ACCESS_DENIED = "[ERROR] Geliştirici modu erişim reddedildi.";
        public const string ERROR_IDENTITY_EMPTY = "[ERROR] Kimlik boş olamaz.";
        public const string ERROR_IDENTITY_VERIFICATION_FAILED = "[ERROR] Kimlik doğrulama başarısız.";
        public const string ERROR_FILE_ACCESS = "[ERROR] Dosya erişimi başarısız.";
        public const string ERROR_IDENTITY_SAVE_FAILED = "[ERROR] Kimlik kaydedilemedi.";
        public const string ERROR_LOGIN_FAILED = "[ERROR] Giriş başarısız: {0}";
        public const string ERROR_DEV_MODE_ACTIVATE = "[ERROR] Geliştirici modu etkinleştirme başarısız oldu: {0}";
        public const string ERROR_DEV_MODE_DEACTIVATE = "[ERROR] Geliştirici modu devre dışı bırakma başarısız oldu.";
        public const string ERROR_STATUS_CHECK_FAILED = "[ERROR] Durum kontrolü başarısız oldu.";
        public const string ERROR_LOGIN_STATUS_CHECK_FAILED = "[ERROR] Giriş durumu kontrolü başarısız oldu.";
        public const string ERROR_DEV_MODE_INACTIVE = "[ERROR] Geliştirici modu aktif değil.";
        public const string ERROR_VALIDATE_ACCESS_FAILED = "[ERROR] Erişim doğrulama başarısız oldu.";
        public const string ERROR_SAVE_ID_CONFIG_FAILED = "[ERROR] Kimlik yapılandırma kaydetme başarısız oldu.";
        public const string ERROR_DEV_MODE_REQUIRED = "[ERROR] Bu özelliği kullanabilmek için önce geliştirici modunu etkinleştirmelisiniz.";
        public const string ERROR_LOGIN_BACKGROUND = "[ERROR] Login formu arka planı oluşturulurken hata: {0}";
        public const string ERROR_LOGIN_CANCEL = "[ERROR] Giriş işlemi iptal edildi.";
        public const string ERROR_LOGIN_FORM_CLOSING = "[ERROR] Login formu kapatılırken hata: {0}";
        #endregion

        #region Admin Form Hataları
        public const string ERROR_ADMIN_FORM_LOAD = "[ERROR] Admin formu yüklenirken hata oluştu: {0}";
        public const string ERROR_ADMIN_BACKGROUND = "[ERROR] Admin formu arka planı oluşturulurken hata: {0}";
        public const string ERROR_ADMIN_INITIAL_DATA_LOAD = "[ERROR] Admin formu ilk veri yüklenirken hata: {0}";
        public const string ERROR_ADMIN_CONFIG_FILE_READ = "[ERROR] Config dosyası okunurken hata oluştu: {0}";
        public const string ERROR_ADMIN_FORM_CLOSING = "[ERROR] Admin formu kapatılırken hata: {0}";
        public const string ERROR_ADMIN_RANDOM_IDENTITY = "[ERROR] Rastgele kimlik oluşturulurken hata: {0}";
        public const string ERROR_ADMIN_IDENTITY_EMPTY = "[ERROR] Lütfen önce yeni bir kimlik girin veya oluşturun.";
        public const string ERROR_ADMIN_IDENTITY_SAVE = "[ERROR] Kimlik kaydedilirken hata oluştu: {0}";
        public const string ERROR_ADMIN_IDENTITY_RESET_CONFIRM = "[ERROR] Kimliği varsayılan değere sıfırlamak istediğinize emin misiniz?";
        public const string ERROR_ADMIN_IDENTITY_RESET = "[ERROR] Kimlik sıfırlanırken hata oluştu: {0}";
        public const string ERROR_ADMIN_DEFAULT_IDENTITY_READ = "[ERROR] Varsayılan kimlik okunurken hata oluştu: {0}";
        #endregion

        #region Resim İşleme Hataları
        public const string ERROR_IMAGE_TOO_SMALL = "[ERROR] Seçilen resim çok küçük. En az 256x256 piksel boyutunda olmalıdır.";
        public const string ERROR_IMAGE_NOT_SELECTED = "[ERROR] Lütfen bir resim seçin.";
        public const string ERROR_PASSWORD_INVALID = "[ERROR] Parola boş olamaz ve boşluk karakteri içeremez.";
        public const string ERROR_TEXT_EMPTY = "[ERROR] Şifrelenecek metin boş olamaz.";
        public const string ERROR_NO_HIDDEN_DATA = "[ERROR] Resimde gizlenmiş veri bulunamadı.";
        public const string ERROR_NOT_ENCRYPTED_WITH_APP = "[ERROR] Bu resim bu uygulama ile şifrelenmemiş.";
        public const string ERROR_INVALID_DATA_FORMAT = "[ERROR] Geçersiz veri formatı.";
        public const string ERROR_KEYS_MISMATCH = "[ERROR] Anahtarlar uyuşmuyor.";
        public const string ERROR_DEV_MODE_OBJECT_NULL = "[ERROR] Geliştirici modu nesnesi boş!";
        public const string ERROR_HELP_FORM = "[ERROR] Yardım formu açılırken bir hata oluştu: {0}";
        public const string ERROR_LOGOUT_CONFIRMATION = "[ERROR] Geliştirici hesabından çıkış yaparsanız Geliştirici Modu kapatılacaktır. Yine de devam etmek istiyor musunuz?";
        public const string ERROR_IMAGE_CANCELED = "[ERROR] Resim seçme işlemi iptal edildi.";
        #endregion
    }
} 