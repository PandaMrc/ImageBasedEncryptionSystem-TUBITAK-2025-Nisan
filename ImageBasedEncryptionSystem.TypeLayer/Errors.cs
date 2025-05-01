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
        public const string ERROR_GENERAL_FAILED = "İşlem Başarısız!";
        public const string ERROR_GENERAL_UNEXPECTED = "Beklenmeyen bir hata oluştu: {0}";
        public const string ERROR_GENERAL_TIMEOUT = "İşlem zaman aşımına uğradı!";
        public const string ERROR_GENERAL_CANCELED = "İşlem kullanıcı tarafından iptal edildi!";
        public const string ERROR_GENERAL_NOT_SUPPORTED = "Bu işlem desteklenmiyor!";
        #endregion

        #region Giriş/Kullanıcı Hataları
        public const string ERROR_LOGIN_USERNAME_EMPTY = "Kullanıcı adı boş olamaz!";
        public const string ERROR_LOGIN_PASSWORD_EMPTY = "Şifre boş olamaz!";
        public const string ERROR_LOGIN_CREDENTIALS = "Kullanıcı adı veya şifre hatalı!";
        public const string ERROR_LOGIN_FAILED = "Giriş işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_LOGIN_BACKGROUND = "Arka plan oluşturulurken bir hata oluştu: {0}";
        #endregion

        #region Geliştirici Modu Hataları
        public const string ERROR_DEV_MODE_ACTIVATE = "Geliştirici modu etkinleştirilirken bir hata oluştu: {0}";
        public const string ERROR_DEV_MODE_DEACTIVATE = "Geliştirici modu kapatılırken bir hata oluştu: {0}";
        public const string ERROR_DEV_MODE_ACCESS_DENIED = "Geliştirici moduna erişim izniniz yok!";
        public const string ERROR_DEV_MODE_ALREADY_ACTIVE = "Geliştirici modu zaten etkin durumda.";
        public const string ERROR_DEV_MODE_ALREADY_DEACTIVE = "Geliştirici modu zaten devre dışı.";
        public const string ERROR_DEV_MODE_REQUIRED = "Bu özelliği kullanabilmek için geliştirici modunu etkinleştirmeniz gerekiyor.";

        #endregion

        #region Geliştirici Modu İşlemleri Hataları
        public const string ERROR_DEV_MODE_LOGIN_INIT_FAILED = "Login: Başlatılamadı - {0}";
        public const string ERROR_DEV_MODE_LOGIN_PROCESS_FAILED = "Login: İşlem sırasında hata oluştu - {0}";
        public const string ERROR_DEV_MODE_LOGOUT_INIT_FAILED = "Logout: Başlatılamadı - {0}";
        public const string ERROR_DEV_MODE_LOGOUT_PROCESS_FAILED = "Logout: İşlem sırasında hata oluştu - {0}";
        public const string ERROR_FILE_NOT_FOUND = "Geliştirici hesabı verileri bulunamadı!";
        #endregion

        #region AES İşlemleri Hataları
        public const string ERROR_AES_GENERATE_KEY_FAILED = "GenerateAESKey: Hata oluştu - {0}";
        public const string ERROR_AES_ENCRYPT_INIT_FAILED = "Encrypt: Şifreleme motoru başlatılamadı - {0}";
        public const string ERROR_AES_ENCRYPT_PROCESS_FAILED = "Encrypt: Şifreleme işlemi sırasında hata oluştu - {0}";
        public const string ERROR_AES_DECRYPT_INIT_FAILED = "Decrypt: Şifre çözme motoru başlatılamadı - {0}";
        public const string ERROR_AES_DECRYPT_PROCESS_FAILED = "Decrypt: Şifre çözme işlemi sırasında hata oluştu - {0}";
        #endregion

        #region RSA İşlemleri Hataları
        public const string ERROR_RSA_KEY_GENERATION_FAILED = "RSA anahtar çifti oluşturulamadı: {0}";
        public const string ERROR_RSA_ENCRYPTION_INIT_FAILED = "RSA şifreleme motoru başlatılamadı: {0}";
        public const string ERROR_RSA_ENCRYPTION_PROCESS_FAILED = "RSA şifreleme işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_RSA_DECRYPTION_INIT_FAILED = "RSA şifre çözme motoru başlatılamadı: {0}";
        public const string ERROR_RSA_DECRYPTION_PROCESS_FAILED = "RSA şifre çözme işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_RSA_PUBLIC_KEY_EXPORT_FAILED = "RSA public key dışa aktarma işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_RSA_PRIVATE_KEY_EXPORT_FAILED = "RSA private key dışa aktarma işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_RSA_GET_SYSTEM_IDENTITY_FAILED = "GetSystemIdentity: Hata oluştu - {0}";
        public const string ERROR_RSA_ENSURE_KEY_PAIR_FAILED = "EnsureKeyPair: Hata oluştu - {0}";
        public const string ERROR_RSA_ENCRYPT_FAILED = "Encrypt: Hata oluştu - {0}";
        public const string ERROR_RSA_DECRYPT_FAILED = "Decrypt: Hata oluştu - {0}";
        public const string ERROR_RSA_GET_PUBLIC_KEY_PEM_FAILED = "GetPublicKeyPem: Hata oluştu - {0}";
        public const string ERROR_RSA_GET_PRIVATE_KEY_PEM_FAILED = "GetPrivateKeyPem: Hata oluştu - {0}";
        #endregion

        #region Kimlik Oluşturma İşlemleri Hataları
        public const string ERROR_IDENTITY_CREATE_PROCESS_FAILED = "CreateRandomIdentity: İşlem sırasında hata oluştu - {0}";
        #endregion

        public const string ERROR_IMAGE_TOO_SMALL = "Seçilen resim çok küçük. Minimum boyut 256x256 olmalıdır.";


    }
} 