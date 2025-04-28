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

        #region Parola Hataları
        public const string ERROR_PASSWORD_EMPTY = "Şifre boş olamaz!";
        public const string ERROR_PASSWORD_REQUIRED = "Şifreleme işlemi için bir parola girmelisiniz!";
        public const string ERROR_PASSWORD_SPACE = "Parola boşluk karakteri içeremez veya boşluk ile başlayamaz!";
        public const string ERROR_PASSWORD_LENGTH_MIN = "Parola uzunluğu en az 3 karakter olmalıdır!";
        public const string ERROR_PASSWORD_LENGTH_MAX = "Parola uzunluğu en fazla 16 karakter olmalıdır!";
        public const string ERROR_PASSWORD_LENGTH_RANGE = "Parola uzunluğu 3-11 karakter arasında olmalıdır!";
        public const string ERROR_PASSWORD_WRONG = "Hatalı parola girdiniz!";
        #endregion

        #region Metin Hataları
        public const string ERROR_TEXT_EMPTY = "Şifrelenecek metin boş olamaz!";
        public const string ERROR_TEXT_REQUIRED = "Lütfen şifrelenecek metni girin!";
        public const string ERROR_TEXT_SPACE = "Metin boşluk karakteri ile başlayamaz!";
        public const string ERROR_TEXT_LENGTH_MIN = "Metin en az 3 karakter içermelidir!";
        public const string ERROR_TEXT_LENGTH_MAX = "Metin çok uzun! Maksimum 10000 karakter kullanabilirsiniz.";
        #endregion

        #region Görsel Hataları
        public const string ERROR_IMAGE_EMPTY = "Resim verisi boş veya geçersiz!";
        public const string ERROR_IMAGE_REQUIRED = "Lütfen bir resim seçin!";
        public const string ERROR_IMAGE_INVALID_FORMAT = "Geçersiz resim formatı! Lütfen PNG, JPG, BMP formatlarından birini kullanın.";
        public const string ERROR_IMAGE_TOO_SMALL = "Resim çok küçük! Minimum 256x256 piksel gerekli.";
        public const string ERROR_IMAGE_TOO_LARGE = "Seçilen resim çok büyük, daha küçük bir resim seçiniz!";
        public const string ERROR_IMAGE_DAMAGED = "Görüntü dosyası hasarlı veya geçersiz!";
        #endregion

        #region Veri Gizleme/Çıkarma Hataları
        public const string ERROR_DATA_NO_HIDDEN = "Bu görsele hiç metin gizlenmemiş!";
        public const string ERROR_DATA_CORRUPTED = "Görsel içerisindeki veri bozulmuş veya hasar görmüş!";
        public const string ERROR_DATA_EXTRACT_FAILED = "Görsel içerisinden veri çıkarılamadı!";
        public const string ERROR_DATA_INSUFFICIENT_PIXELS = "Görsel, gizlenecek veri için yeterli piksel içermiyor!";
        public const string ERROR_DATA_TOO_LARGE = "Veri boyutu çok büyük, bu görsele sığmaz!";
        #endregion

        #region Şifreleme Algoritma Hataları
        // AES Hataları
        public const string ERROR_AES_ENCRYPT = "AES şifreleme sırasında hata oluştu!";
        public const string ERROR_AES_KEY_GENERATION = "AES anahtarı oluşturulamadı!";
        public const string ERROR_AES_DECRYPT = "AES şifre çözme hatası: Şifreli metin bozuk veya hatalı";
        public const string ERROR_AES_IV_INVALID = "AES başlatma vektörü geçersiz!";
        
        // RSA Hataları
        public const string ERROR_RSA_ENCRYPT = "RSA şifreleme sırasında hata oluştu!";
        public const string ERROR_RSA_KEY_GENERATION = "RSA anahtarı oluşturulamadı!";
        public const string ERROR_RSA_DECRYPT = "RSA şifre çözme işlemi başarısız oldu!";
        public const string ERROR_RSA_NO_DATA = "RSA şifreli veri bulunamadı!";
        public const string ERROR_RSA_KEY_SIZE = "RSA anahtar boyutu geçersiz!";
        public const string ERROR_RSA_PRIVATE_KEY_MISSING = "RSA özel anahtarı bulunamadı!";
        
        // Wavelet Hataları
        public const string ERROR_WAVELET_TRANSFORM = "Dalgacık dönüşümü uygulanırken hata oluştu: {0}";
        public const string ERROR_WAVELET_INVERSE_TRANSFORM = "Ters dalgacık (Wavelet) dönüşümü sırasında hata oluştu!";
        public const string ERROR_WAVELET_ENCODE = "Dalgacık tabanlı veri gizleme işlemi sırasında hata oluştu!";
        public const string ERROR_WAVELET_DECODE = "Dalgacık katsayılarından veri çıkarılırken hata oluştu: {0}";
        public const string ERROR_WAVELET_COEFFICIENTS = "Dalgacık katsayıları hesaplanırken hata oluştu!";
        public const string ERROR_WAVELET_INSUFFICIENT_CAPACITY = "Görsel, wavelet dönüşümü ile veri gizlemek için yeterli kapasiteye sahip değil!";
        public const string ERROR_WAVELET_NO_DATA = "Görüntüde gizli veri bulunamadı";
        public const string ERROR_WAVELET_DATA_CORRUPTED = "Görüntüden çıkarılan veri bozuk veya tamamlanmamış";
        #endregion

        #region Dosya İşlemleri Hataları
        public const string ERROR_FILE_OPEN = "Dosya açılırken hata oluştu!";
        public const string ERROR_FILE_SAVE = "Dosya kaydedilirken hata oluştu!";
        public const string ERROR_FILE_NOT_FOUND = "Dosya bulunamadı veya erişilemedi!";
        public const string ERROR_FILE_UNSUPPORTED = "Desteklenmeyen dosya formatı!";
        public const string ERROR_FILE_IN_USE = "Dosya başka bir işlem tarafından kullanılıyor!";
        public const string ERROR_FILE_ACCESS_DENIED = "Dosyaya erişim reddedildi, yetki gerekiyor!";
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
    }
} 