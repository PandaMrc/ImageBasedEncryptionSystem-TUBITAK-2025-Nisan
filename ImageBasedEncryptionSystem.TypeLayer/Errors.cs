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

        #region Eski Format Hatalar (Geriye Uyumluluk)
        // Bu bölüm, eski formattaki hata kodlarını yeni formatta tanımlar
        // ve geriye uyumluluk sağlar. Bu sabitler eski kodları bozmamak için korunmuştur.
        // Yeni kodlarda kullanmayınız, bunun yerine aşağıdaki kategorilerdeki sabitleri kullanınız.
        
        // ERROR1 ve türevleri - Parola Hataları 
        public const string ERROR1 = "Şifre boş olamaz!"; // Parola boş hatası
        public const string ERROR1_SPACE = "Parola boşluk karakteri içeremez veya boşluk ile başlayamaz!"; // Parola boşluk hatası
        public const string ERROR1_LENGTH = "Parola uzunluğu 3-11 karakter arasında olmalıdır!"; // Parola uzunluk hatası
        public const string ERROR1_ENCRYPT_ALT = "Şifreleme sırasında bir hata oluştu"; // Genel şifreleme hatası
        
        // ERROR2 ve türevleri - Metin Hataları
        public const string ERROR2 = "Şifrelenecek metin boş olamaz!"; // Metin boş hatası
        public const string ERROR2_TEXT = "Şifrelenecek bir metin girmelisiniz!"; // Metin gerekli hatası
        public const string ERROR2_PASSWORD = "Şifreleme işlemi için bir parola girmelisiniz!"; // Parola gerekli hatası
        public const string ERROR2_SPACE = "Metin boşluk karakteri ile başlayamaz!"; // Metin boşluk hatası
        public const string ERROR2_LENGTH = "Metin çok uzun! Maksimum 10000 karakter kullanabilirsiniz."; // Metin uzunluk hatası
        
        // ERROR3 ve türevleri - Şifre Çözme Hataları
        public const string ERROR3 = "Şifre çözme işlemi sırasında bir hata oluştu!"; // Genel şifre çözme hatası
        public const string ERROR3_NO_DATA = "Bu görsele hiç metin gizlenmemiş!"; // Veri bulunamadı hatası
        public const string ERROR3_WRONG_PASSWORD = "Hatalı parola girdiniz!"; // Yanlış parola hatası
        public const string ERROR3_EXTRACTION_FAILED = "Görsel içerisinden veri çıkarılamadı!"; // Veri çıkarma hatası
        
        // ERROR4 ve türevleri - Görsel Hataları
        public const string ERROR4 = "Görsel seçilmedi!"; // Görsel seçili değil hatası
        public const string ERROR4_TOO_SMALL = "Seçilen resim çok küçük, en az (300x300) boyutunda olmalı, daha büyük bir resim seçiniz!"; // Görsel boyut hatası
        public const string ERROR4_INVALID_FORMAT = "Seçilen dosya geçerli bir resim formatında değil!"; // Görsel format hatası
        
        // ERROR5 ve türevleri - AES Şifreleme Hataları
        public const string ERROR5_AES = "AES şifreleme sırasında hata oluştu!"; // AES şifreleme hatası
        public const string ERROR5_AES_KEY = "AES anahtarı oluşturulamadı!"; // AES anahtar hatası
        
        // ERROR6 ve türevleri - RSA Şifreleme Hataları
        public const string ERROR6_RSA = "RSA şifreleme sırasında hata oluştu!"; // RSA şifreleme hatası
        public const string ERROR6_RSA_KEY = "RSA anahtar çifti oluşturulamadı veya yüklenemedi!"; // RSA anahtar hatası
        public const string ERROR6_RSA_DECRYPT = "RSA şifre çözme işlemi başarısız oldu!"; // RSA çözme hatası
        
        // ERROR7 ve türevleri - LSB Veri Gizleme Hataları
        public const string ERROR7_LSB = "LSB veri gizleme işlemi sırasında hata oluştu!"; // LSB gizleme hatası
        public const string ERROR7_LSB_CAPACITY = "Resim, gizlenecek veri için yeterli kapasiteye sahip değil!"; // LSB kapasite hatası
        public const string ERROR7_LSB_EXTRACT = "LSB veri çıkarma işlemi sırasında hata oluştu!"; // LSB çıkarma hatası
        
        // ERROR8 ve türevleri - Dosya İşlemi Hataları
        public const string ERROR8_FILE_OPEN = "Dosya açılırken hata oluştu!"; // Dosya açma hatası
        public const string ERROR8_FILE_SAVE = "Dosya kaydedilirken hata oluştu!"; // Dosya kaydetme hatası
        public const string ERROR8_FILE_FORMAT = "Dosya formatı uygun değil!"; // Eski ERROR8_FILE_FORMAT ile aynı
        
        // ERROR9 ve türevleri - İleri RSA Hataları
        public const string ERROR9_RSA_NO_DATA = "RSA şifreli veri bulunamadı!"; // RSA veri eksik hatası
        public const string ERROR9_RSA_NO_PASSWORD = "RSA şifre çözme için parola gerekli!"; // RSA parola eksik hatası
        public const string ERROR9_RSA_DECRYPT = "RSA şifresi çözülürken hata oluştu!"; // RSA çözme hatası
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
        public const string ERROR_IMAGE_TOO_SMALL = "Resim çok küçük! Minimum 300x300 piksel gerekli.";
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
        
        // LSB Hataları
        public const string ERROR_LSB_HIDE = "LSB veri gizleme işlemi sırasında hata oluştu!";
        public const string ERROR_LSB_EXTRACT = "LSB veri çıkarma işlemi sırasında hata oluştu!";
        
        // Wavelet Hataları
        public const string ERROR_WAVELET_TRANSFORM = "Dalgacık (Wavelet) dönüşümü sırasında hata oluştu!";
        public const string ERROR_WAVELET_INVERSE_TRANSFORM = "Ters dalgacık (Wavelet) dönüşümü sırasında hata oluştu!";
        public const string ERROR_WAVELET_ENCODE = "Dalgacık tabanlı veri gizleme işlemi sırasında hata oluştu!";
        public const string ERROR_WAVELET_DECODE = "Dalgacık tabanlı veri çıkarma işlemi sırasında hata oluştu!";
        public const string ERROR_WAVELET_COEFFICIENTS = "Dalgacık katsayıları hesaplanırken hata oluştu!";
        public const string ERROR_WAVELET_INSUFFICIENT_CAPACITY = "Görsel, wavelet dönüşümü ile veri gizlemek için yeterli kapasiteye sahip değil!";
        public const string ERROR_WAVELET_NO_DATA = "Bu görselde wavelet ile gizlenmiş veri bulunamadı!";
        public const string ERROR_WAVELET_DATA_CORRUPTED = "Wavelet ile gizlenmiş veri bozulmuş veya hasar görmüş!";
        public const string ERROR_WAVELET_LEVEL_INVALID = "Geçersiz dalgacık (wavelet) ayrıştırma seviyesi!";
        public const string ERROR_WAVELET_FAMILY_INVALID = "Geçersiz dalgacık (wavelet) ailesi!";
        public const string ERROR_WAVELET_ENCRYPT = "Dalgacık tabanlı şifreleme işlemi sırasında hata oluştu!";
        public const string ERROR_WAVELET_DECRYPT = "Dalgacık tabanlı şifre çözme işlemi sırasında hata oluştu!";
        #endregion

        #region Dosya İşlemleri Hataları
        public const string ERROR_FILE_OPEN = "Dosya açılırken hata oluştu!";
        public const string ERROR_FILE_SAVE = "Dosya kaydedilirken hata oluştu!";
        public const string ERROR_FILE_NOT_FOUND = "Dosya bulunamadı veya erişilemedi!";
        public const string ERROR_FILE_UNSUPPORTED = "Desteklenmeyen dosya formatı!";
        public const string ERROR_FILE_IN_USE = "Dosya başka bir işlem tarafından kullanılıyor!";
        public const string ERROR_FILE_ACCESS_DENIED = "Dosyaya erişim reddedildi, yetki gerekiyor!";
        public const string ERROR_FILE_PATH_TOO_LONG = "Dosya yolu çok uzun!";
        public const string ERROR_FILE_INVALID_PATH = "Geçersiz dosya yolu!";
        #endregion

        #region Giriş/Kullanıcı Hataları
        public const string ERROR_LOGIN_USERNAME_EMPTY = "Kullanıcı adı boş olamaz!";
        public const string ERROR_LOGIN_PASSWORD_EMPTY = "Şifre boş olamaz!";
        public const string ERROR_LOGIN_CREDENTIALS = "Kullanıcı adı veya şifre hatalı!";
        public const string ERROR_LOGIN_FAILED = "Giriş işlemi sırasında bir hata oluştu: {0}";
        public const string ERROR_LOGIN_BACKGROUND = "Arka plan resmi yüklenemedi: {0}";
        #endregion

        #region Analiz Hataları
        public const string ERROR_ANALYSIS_IMAGES_REQUIRED = "Analiz için her iki görüntü de gereklidir!";
        public const string ERROR_ANALYSIS_PROCESS = "Analiz işlemi sırasında bir hata oluştu: {0}";
        #endregion

        #region Geliştirici Modu Hataları
        public const string ERROR_DEV_MODE_ACTIVATE = "Geliştirici modu etkinleştirilirken bir hata oluştu: {0}";
        public const string ERROR_DEV_MODE_DEACTIVATE = "Geliştirici modu kapatılırken bir hata oluştu: {0}";
        public const string ERROR_DEV_MODE_ACCESS_DENIED = "Geliştirici moduna erişim izniniz yok!";
        public const string ERROR_DEV_MODE_ALREADY_ACTIVE = "Geliştirici modu zaten etkin durumda.";
        public const string ERROR_DEV_MODE_ALREADY_DEACTIVE = "Geliştirici modu zaten devre dışı.";
        public const string ERROR_DEV_MODE_REQUIRED = "Bu özelliği kullanabilmek için geliştirici modunu etkinleştirmeniz gerekiyor.";
        public const string ERROR_DEV_MODE_INVALID_ACTION = "Geliştirici modunda geçersiz işlem: {0}";
        #endregion

        #region İçerik ve Metin Hataları
        public const string ERROR_CIPHERTEXT_EMPTY = "Şifreli metin boş olamaz!";
        public const string ERROR_NO_KEY_GENERATED = "Henüz bir anahtar oluşturulmadı!";
        #endregion

        #region Parola ve Kimlik Doğrulama Hataları
        public const string ERROR_KEY_REQUIRED = "Şifre çözmek için anahtar gerekli!";
        #endregion
    }
} 