using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    public static class Debug
    {
        #region AES İşlemleri
        // AES Anahtar Üretimi
        public const string DEBUG_AES_KEY_GENERATION_STARTED = "[DEBUG] AES anahtar üretme sistemi";
        public const string ERROR_AES_GENERATE_KEY_FAILED = "[ERROR] AES anahtar üretimi başarısız oldu. Hata: {0}";
        public const string DEBUG_AES_GENERATE_KEY_STARTED = "[DEBUG] AES anahtar üretimi başlatıldı. Kaynak: {0}";
        public const string DEBUG_AES_KEY_GENERATION_COMPLETED = "[DEBUG] AES anahtar üretimi tamamlandı. Anahtar: {0}";

        // AES Şifreleme
        public const string ERROR_AES_ENCRYPT_PROCESS_FAILED = "[ERROR] AES şifreleme işlemi başarısız oldu. Hata: {0}";
        public const string DEBUG_AES_ENCRYPT_STARTED = "[DEBUG] AES şifreleme işlemi başlatıldı. Anahtar: {0}, IV: {1}";
        public const string DEBUG_AES_ENCRYPTION_STARTED = "[DEBUG] AES şifreleme işlemi başlatıldı. Anahtar: {0}, IV: {1}";
        public const string DEBUG_AES_ENCRYPTION_COMPLETED = "[DEBUG] AES şifreleme işlemi tamamlandı. Şifreli metin: {0}";
        public const string DEBUG_AES_ENCRYPT_PROCESSING = "[DEBUG] AES şifreleme işlemi devam ediyor.";
        public const string DEBUG_AES_ENCRYPT_ENGINE_INIT = "[DEBUG] AES şifreleme motoru başlatıldı.";

        // AES Şifre Çözme
        public const string ERROR_AES_DECRYPT_PROCESS_FAILED = "[ERROR] AES şifre çözme işlemi başarısız oldu. Hata: {0}";
        public const string DEBUG_AES_DECRYPTION_STARTED = "[DEBUG] AES şifre çözme işlemi başlatıldı. Anahtar: {0}, IV: {1}";
        public const string DEBUG_AES_DECRYPTION_COMPLETED = "[DEBUG] AES şifre çözme işlemi tamamlandı. Çözülen metin: {0}";
        public const string DEBUG_AES_DECRYPT_PROCESSING = "[DEBUG] AES şifre çözme işlemi devam ediyor.";
        public const string DEBUG_AES_DECRYPT_ENGINE_INIT = "[DEBUG] AES şifre çözme motoru başlatıldı.";
        #endregion

        #region SHA256 İşlemleri
        public const string DEBUG_SHA256_HASH_STARTED = "[DEBUG] SHA256 hash işlemi başlatıldı.";
        public const string DEBUG_SHA256_HASH_COMPLETED = "[DEBUG] SHA256 hash işlemi tamamlandı. Hash: {0}";
        #endregion

        #region Bellek Akışı İşlemleri
        public const string DEBUG_MEMORY_STREAM_STARTED = "[DEBUG] Bellek akışı başlatıldı.";
        public const string DEBUG_MEMORY_STREAM_COMPLETED = "[DEBUG] Bellek akışı tamamlandı.";
        #endregion

        #region Şifreleme İşlemleri
        public const string DEBUG_ENCRYPT_PROCESS_STARTED = "[DEBUG] Şifreleme işlemi başlatıldı.";
        public const string DEBUG_ENCRYPT_PROCESS_COMPLETED = "[DEBUG] Şifreleme işlemi tamamlandı.";
        #endregion

        #region Şifre Çözme İşlemleri
        public const string DEBUG_DECRYPT_PROCESS_STARTED = "[DEBUG] Şifre çözme işlemi başlatıldı.";
        public const string DEBUG_DECRYPT_PROCESS_COMPLETED = "[DEBUG] Şifre çözme işlemi tamamlandı.";
        #endregion

        #region Resim İşleme
        public const string DEBUG_IMAGE_LOAD_STARTED = "[DEBUG] Resim yükleme işlemi başlatıldı: {0}";
        public const string DEBUG_IMAGE_LOAD_SUCCESS = "[DEBUG] Resim başarıyla yüklendi: {0}";
        public const string DEBUG_SIGNATURE_ADD_STARTED = "[DEBUG] İmza ekleme işlemi başlatıldı";
        public const string DEBUG_HASH_ADD_COMPLETED = "[DEBUG] Hash ekleme işlemi tamamlandı";
        public const string DEBUG_PASSWORD_MATCH = "[DEBUG] Parola eşleşme kontrolü";
        public const string DEBUG_DATA_EXTRACTED = "[DEBUG] Veri çıkarıldı";
        #endregion

        #region RSA İşlemleri
        // RSA Anahtar Üretimi
        public const string DEBUG_RSA_ENSURE_KEY_PAIR_STARTED = "[DEBUG] RSA anahtar çifti üretimi başlatıldı. Kimlik: {0}";
        public const string DEBUG_EXISTING_KEY_PAIR = "[DEBUG] Mevcut anahtar çifti: {0}";
        public const string DEBUG_RSA_KEY_PAIR_GENERATOR_STARTED = "[DEBUG] RSAKeyPairGenerator başlatıldı.";
        public const string DEBUG_RSA_KEY_PAIR_GENERATED = "[DEBUG] RSA anahtar çifti üretildi.";
        public const string DEBUG_NEW_RSA_KEY_PAIR_CREATED = "[DEBUG] Yeni RSA anahtar çifti oluşturuldu.";
        public const string ERROR_RSA_ENSURE_KEY_PAIR_FAILED = "[ERROR] RSA anahtar çifti üretimi başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_KEY_GENERATION_FAILED = "[ERROR] RSA anahtar üretimi başarısız oldu. Hata: {0}";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_EXISTING = "[SUCCESS] Mevcut RSA anahtar çifti kullanılıyor. Kimlik: {0}";
        public const string SUCCESS_RSA_ENSURE_KEY_PAIR_GENERATED = "[SUCCESS] RSA anahtar çifti başarıyla üretildi.";

        // RSA Şifreleme
        public const string DEBUG_RSA_ENCRYPT_ENGINE_INIT = "[DEBUG] RSA şifreleme motoru başlatıldı.";
        public const string DEBUG_RSA_ENCRYPT_PROCESSING = "[DEBUG] RSA şifreleme işlemi devam ediyor.";
        public const string DEBUG_RSA_ENCRYPT_PROCESSED = "[DEBUG] RSA şifreleme işlemi tamamlandı.";
        public const string ERROR_RSA_ENCRYPT_FAILED = "[ERROR] RSA şifreleme işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_ENCRYPTION_PROCESS_FAILED = "[ERROR] RSA şifreleme işlemi sırasında hata oluştu. Hata: {0}";

        // RSA Şifre Çözme
        public const string DEBUG_RSA_DECRYPT_STARTED = "[DEBUG] RSA şifre çözme işlemi başlatıldı. Anahtar: {0}";
        public const string DEBUG_RSA_DECRYPT_ENGINE_INIT = "[DEBUG] RSA şifre çözme motoru başlatıldı.";
        public const string DEBUG_RSA_DECRYPT_PROCESSING = "[DEBUG] RSA şifre çözme işlemi devam ediyor.";
        public const string DEBUG_RSA_DECRYPT_PROCESSED = "[DEBUG] RSA şifre çözme işlemi tamamlandı.";
        public const string ERROR_RSA_DECRYPT_FAILED = "[ERROR] RSA şifre çözme işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_DECRYPTION_PROCESS_FAILED = "[ERROR] RSA şifre çözme işlemi sırasında hata oluştu. Hata: {0}";

        // RSA Anahtar Dışa Aktarım
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_STARTED = "[DEBUG] Public Key PEM formatında dışa aktarım başlatıldı. Anahtar: {0}";
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTING = "[DEBUG] Public Key PEM formatında dışa aktarılıyor.";
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTED = "[DEBUG] Public Key PEM formatında dışa aktarıldı.";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_STARTED = "[DEBUG] Private Key PEM formatında dışa aktarım başlatıldı. Anahtar: {0}";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTING = "[DEBUG] Private Key PEM formatında dışa aktarılıyor.";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTED = "[DEBUG] Private Key PEM formatında dışa aktarıldı.";
        public const string ERROR_RSA_GET_PUBLIC_KEY_PEM_FAILED = "[ERROR] Public Key PEM formatında dışa aktarım başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_PUBLIC_KEY_EXPORT_FAILED = "[ERROR] Public Key dışa aktarımı başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_GET_PRIVATE_KEY_PEM_FAILED = "[ERROR] Private Key PEM formatında dışa aktarım başarısız oldu. Hata: {0}";
        public const string ERROR_RSA_PRIVATE_KEY_EXPORT_FAILED = "[ERROR] Private Key dışa aktarımı başarısız oldu. Hata: {0}";
        #endregion

        #region LSB İşlemleri
        // LSB Veri Gömme
        public const string DEBUG_LSB_EMBED_DATA_STARTED = "[DEBUG] LSB veri gömme işlemi başlatıldı.";
        public const string DEBUG_LSB_EMBED_DATA_COMPLETED = "[DEBUG] LSB veri gömme işlemi tamamlandı.";
        public const string DEBUG_LSB_EMBED_DATA_INPUT = "[DEBUG] LSB veri gömme girişi - Resim boyutu: {0}x{1}, Veri boyutu: {2} byte";
        public const string DEBUG_LSB_FULL_DATA_PREPARED = "[DEBUG] LSB tam veri hazırlandı - Orijinal veri: {0} byte, İşaretçiyle birlikte: {1} byte";
        public const string DEBUG_LSB_PROCESSED_PIXEL_COUNT = "[DEBUG] İşlenen toplam piksel sayısı: {0}";
        public const string DEBUG_LSB_EMBED_PROGRESS = "[DEBUG] İşlenen piksel: {0}, Yerleştirilen veri: {1}/{2} byte";

        // LSB Veri Çıkarma
        public const string DEBUG_LSB_EXTRACT_DATA_STARTED = "[DEBUG] LSB veri çıkarma işlemi başlatıldı.";
        public const string DEBUG_LSB_EXTRACT_DATA_COMPLETED = "[DEBUG] LSB veri çıkarma işlemi tamamlandı.";
        public const string DEBUG_LSB_EXTRACT_DATA_INPUT = "[DEBUG] LSB veri çıkarma girişi - Resim boyutu: {0}x{1}";
        public const string DEBUG_LSB_EXTRACT_DATA_END_MARKER = "[DEBUG] LSB son işaretçisi bulundu - Pozisyon: {0}";
        public const string DEBUG_LSB_EXTRACT_PROGRESS = "[DEBUG] İşlenen piksel: {0}, Çıkarılan veri: {1} byte";
        public const string DEBUG_LSB_EXTRACT_TOTAL = "[DEBUG] İşlenen toplam piksel sayısı: {0}, Toplam çıkarılan veri: {1} byte";
        public const string DEBUG_LSB_END_MARKER_NOT_FOUND = "[DEBUG] Son işareti bulunamadı, tüm veri döndürülüyor.";

        // LSB İmza İşlemleri
        public const string DEBUG_LSB_EMBED_SIGNATURE_STARTED = "[DEBUG] LSB imza ekleme işlemi başlatıldı.";
        public const string DEBUG_LSB_EMBED_SIGNATURE_COMPLETED = "[DEBUG] LSB imza ekleme işlemi tamamlandı.";
        public const string DEBUG_LSB_EMBED_SIGNATURE_INPUT = "[DEBUG] LSB imza gömme girişi - Resim boyutu: {0}x{1}, İmza boyutu: {2} byte";
        public const string DEBUG_LSB_EMBED_SIGNATURE_COMPLETED_COUNT = "[DEBUG] İmza gömme işlemi tamamlandı. İşlenen piksel: {0}";
        public const string DEBUG_LSB_CHECK_SIGNATURE_STARTED = "[DEBUG] LSB imza kontrol işlemi başlatıldı.";
        public const string DEBUG_LSB_CHECK_SIGNATURE_COMPLETED = "[DEBUG] LSB imza kontrol işlemi tamamlandı.";
        public const string DEBUG_LSB_CHECK_SIGNATURE_INPUT = "[DEBUG] LSB imza kontrol girişi - Resim boyutu: {0}x{1}";
        public const string DEBUG_LSB_CHECK_SIGNATURE_COMPARISON = "[DEBUG] LSB imza karşılaştırma - Beklenen: {0}, Bulunan: {1}, Eşleşme Durumu: {2}";
        public const string DEBUG_LSB_SIGNATURE_NOT_FOUND = "[DEBUG] İmza bulunamadı. Beklenen: {0}, Bulunan: {1}";
        public const string DEBUG_LSB_SIGNATURE_VERIFIED = "[DEBUG] İmza doğrulandı.";
        public const string DEBUG_LSB_DATA_SIZE = "[DEBUG] LSB veri boyutu: {0}";
        public const string DEBUG_LSB_ADDED_SIGNATURE = "[DEBUG] LSB imza eklendi.";
        #endregion

        #region Kimlik Oluşturma İşlemleri
        public const string DEBUG_IDENTITY_CREATE_STARTED = "[DEBUG] Kimlik oluşturma işlemi başlatıldı. Uzunluk: {0}";
        public const string DEBUG_IDENTITY_CREATE_CHARACTER_ADDED = "[DEBUG] Kimliğe karakter eklendi: {0}";
        public const string DEBUG_IDENTITY_CREATE_SPECIAL_CHARACTER_ADDED = "[DEBUG] Kimliğe özel karakter eklendi: {0}";
        public const string DEBUG_IDENTITY_CREATE_COMPLETED = "[DEBUG] Kimlik oluşturma işlemi tamamlandı. Kimlik: {0}";
        public const string SUCCESS_IDENTITY_CREATE_PROCESSING = "[SUCCESS] Kimlik oluşturma işlemi devam ediyor.";
        public const string ERROR_IDENTITY_CREATE_PROCESS_FAILED = "[SUCCESS] Kimlik oluşturma işlemi başarısız.";
        #endregion

        #region Geliştirici Modu İşlemleri
        // Geliştirici Modu Giriş/Çıkış
        public const string DEBUG_DEV_MODE_LOGIN_STARTED = "[DEBUG] Geliştirici modu girişi başlatıldı.";
        public const string DEBUG_DEV_MODE_LOGIN_PROCESSING = "[DEBUG] Geliştirici modu girişi devam ediyor.";
        public const string DEBUG_DEV_MODE_LOGIN_PROCESSED = "[DEBUG] Geliştirici modu girişi tamamlandı.";
        public const string ERROR_DEV_MODE_LOGIN_PROCESS_FAILED = "[ERROR] Geliştirici modu girişi başarısız oldu. Hata: {0}";
        public const string DEBUG_DEV_MODE_LOGOUT_STARTED = "[DEBUG] Geliştirici modu çıkışı başlatıldı.";
        public const string DEBUG_DEV_MODE_LOGOUT_PROCESSING = "[DEBUG] Geliştirici modu çıkışı devam ediyor.";
        public const string DEBUG_DEV_MODE_LOGOUT_PROCESSED = "[DEBUG] Geliştirici modu çıkışı tamamlandı.";
        public const string ERROR_DEV_MODE_LOGOUT_PROCESS_FAILED = "[ERROR] Geliştirici modu çıkışı başarısız oldu. Hata: {0}";

        // Geliştirici Modu Etkinleştirme/Devre Dışı Bırakma
        public const string DEBUG_DEV_MODE_ACTIVATE_STARTED = "[DEBUG] Geliştirici modu etkinleştirme başlatıldı.";
        public const string DEBUG_DEV_MODE_ACTIVATE_COMPLETED = "[DEBUG] Geliştirici modu etkinleştirme tamamlandı.";
        public const string ERROR_DEV_MODE_ACTIVATE = "[ERROR] Geliştirici modu etkinleştirme başarısız oldu. Hata: {0}";
        public const string DEBUG_DEV_MODE_DEACTIVATE_STARTED = "[DEBUG] Geliştirici modu devre dışı bırakma başlatıldı.";
        public const string DEBUG_DEV_MODE_DEACTIVATE_COMPLETED = "[DEBUG] Geliştirici modu devre dışı bırakma tamamlandı.";
        public const string ERROR_DEV_MODE_DEACTIVATE = "[ERROR] Geliştirici modu devre dışı bırakma başarısız oldu. Hata: {0}";

        // Geliştirici Modu Diğer İşlemler
        public const string DEBUG_ENCRYPTION_HISTORY_ADD_STARTED = "[DEBUG] Şifreleme geçmişi ekleme işlemi başlatıldı.";
        public const string DEBUG_ENCRYPTION_HISTORY_ADD_COMPLETED = "[DEBUG] Şifreleme geçmişi ekleme işlemi tamamlandı.";
        public const string ERROR_ENCRYPTION_HISTORY_ADD_FAILED = "[ERROR] Şifreleme geçmişi ekleme işlemi başarısız oldu. Hata: {0}";
        public const string DEBUG_DEV_MODE_STATUS_CHECK_STARTED = "[DEBUG] Geliştirici modu durumu kontrolü başlatıldı.";
        public const string DEBUG_LOGIN_STATUS_CHECK_STARTED = "[DEBUG] Giriş durumu kontrolü başlatıldı.";
        public const string DEBUG_ACCESS_VALIDATION_STARTED = "[DEBUG] Erişim doğrulama işlemi başlatıldı.";
        public const string DEBUG_ACCESS_VALIDATION_COMPLETED = "[DEBUG] Erişim doğrulama işlemi tamamlandı.";
        public const string DEBUG_SAVE_IDENTITY_STARTED = "[DEBUG] Kimlik kaydetme işlemi başlatıldı.";
        public const string DEBUG_SAVE_IDENTITY_COMPLETED = "[DEBUG] Kimlik kaydetme işlemi tamamlandı.";
        public const string ERROR_SAVE_IDENTITY_FAILED = "[ERROR] Kimlik kaydetme işlemi başarısız oldu. Hata: {0}";
        public const string DEBUG_DEV_MODE_ACTIVE = "[DEBUG] Geliştirici modu aktif.";
        public const string DEBUG_DEV_MODE_INACTIVE = "[DEBUG] Geliştirici modu devre dışı.";
        public const string DEBUG_LOGIN_ACTIVE = "[DEBUG] Geliştirici girişi aktif.";
        public const string DEBUG_LOGIN_INACTIVE = "[DEBUG] Geliştirici girişi devre dışı.";
        #endregion

        #region Config İşlemleri
        // Config Dosya İşlemleri
        public const string DEBUG_GET_CONFIG_FILE_PATH_STARTED = "[DEBUG] Config dosya yolu alma işlemi başlatıldı.";
        public const string DEBUG_CONFIG_FILE_FOUND_RELATIVE = "[DEBUG] Config dosyası göreceli yolda bulundu.";
        public const string DEBUG_CONFIG_FILE_FOUND_LOCAL = "[DEBUG] Config dosyası yerel yolda bulundu.";
        public const string DEBUG_CONFIG_FILE_FOUND_APPDATA = "[DEBUG] Config dosyası AppData'da bulundu.";
        public const string DEBUG_ENCRYPTED_CONFIG_FILE_FOUND = "[DEBUG] Şifrelenmiş config dosyası bulundu.";
        public const string DEBUG_CONFIG_FILE_DECRYPTED = "[DEBUG] Config dosyası başarıyla çözüldü.";
        public const string DEBUG_DEFAULT_CONFIG_FILE_CREATED = "[DEBUG] Varsayılan config dosyası oluşturuldu.";
        public const string DEBUG_DEFAULT_CONFIG_FILE_CREATED_LOCAL = "[DEBUG] Yerel yolda varsayılan config dosyası oluşturuldu.";
        public const string DEBUG_RETURNING_DEFAULT_RELATIVE_PATH = "[DEBUG] Varsayılan göreceli yol döndürülüyor.";
        public const string DEBUG_CREATE_DEFAULT_CONFIG_FILE_STARTED = "[DEBUG] Varsayılan config dosyası oluşturma işlemi başlatıldı.";
        public const string DEBUG_CREATE_DEFAULT_CONFIG_FILE_COMPLETED = "[DEBUG] Varsayılan config dosyası oluşturma işlemi tamamlandı.";
        public const string DEBUG_ENCRYPT_CONFIG_FILE_STARTED = "[DEBUG] Config dosyası şifreleme işlemi başlatıldı.";
        public const string DEBUG_ENCRYPT_CONFIG_FILE_COMPLETED = "[DEBUG] Config dosyası şifreleme işlemi tamamlandı.";
        public const string ERROR_CONFIG_FILE_DECRYPTION_FAILED = "[ERROR] Config dosyası şifre çözme işlemi başarısız oldu.";
        public const string ERROR_DEFAULT_CONFIG_FILE_CREATION_FAILED = "[ERROR] Varsayılan config dosyası oluşturma işlemi başarısız oldu.";
        public const string ERROR_DEFAULT_CONFIG_FILE_CREATION_FAILED_LOCAL = "[ERROR] Yerel yolda varsayılan config dosyası oluşturma işlemi başarısız oldu.";
        public const string ERROR_GET_CONFIG_FILE_PATH_FAILED = "[ERROR] Config dosya yolu alma işlemi başarısız oldu.";
        public const string ERROR_CONFIG_FILE_NOT_FOUND = "[ERROR] Config dosyası bulunamadı.";
        public const string ERROR_ENCRYPT_CONFIG_FILE_FAILED = "[ERROR] Config dosyası şifreleme işlemi başarısız oldu. Hata: {0}";
        public const string DEBUG_DECRYPT_CONFIG_FILE_STARTED = "[DEBUG] Config dosyası şifre çözme işlemi başlatıldı.";
        public const string DEBUG_DECRYPT_CONFIG_FILE_COMPLETED = "[DEBUG] Config dosyası şifre çözme işlemi tamamlandı.";
        public const string ERROR_DECRYPT_CONFIG_FILE_FAILED = "[ERROR] Config dosyası şifre çözme işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_ENCRYPTED_CONFIG_FILE_NOT_FOUND = "[ERROR] Şifrelenmiş config dosyası bulunamadı.";

        // Config Değer İşlemleri
        public const string DEBUG_READ_CONFIG_VALUE_STARTED = "[DEBUG] Config değeri okuma işlemi başlatıldı.";
        public const string DEBUG_READ_CONFIG_VALUE_COMPLETED = "[DEBUG] Config değeri okuma işlemi tamamlandı.";
        public const string ERROR_READ_CONFIG_VALUE_FAILED = "[ERROR] Config değeri okuma işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_CONFIG_VALUE_NOT_FOUND = "[ERROR] Config değeri bulunamadı.";
        public const string DEBUG_WRITE_CONFIG_VALUE_STARTED = "[DEBUG] Config değeri yazma işlemi başlatıldı.";
        public const string DEBUG_WRITE_CONFIG_VALUE_COMPLETED = "[DEBUG] Config değeri yazma işlemi tamamlandı.";
        public const string ERROR_WRITE_CONFIG_VALUE_FAILED = "[ERROR] Config değeri yazma işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_CONFIG_SECTION_NOT_FOUND = "[ERROR] Config bölümü bulunamadı.";

        // Sistem Kimliği İşlemleri
        public const string DEBUG_GET_SYSTEM_IDENTITY_STARTED = "[DEBUG] Sistem kimliği alma işlemi başlatıldı.";
        public const string DEBUG_GET_SYSTEM_IDENTITY_COMPLETED = "[DEBUG] Sistem kimliği alma işlemi tamamlandı.";
        public const string ERROR_GET_SYSTEM_IDENTITY_FAILED = "[ERROR] Sistem kimliği alma işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_SYSTEM_IDENTITY_NOT_FOUND = "[ERROR] Sistem kimliği bulunamadı.";
        public const string DEBUG_UPDATE_SYSTEM_IDENTITY_STARTED = "[DEBUG] Sistem kimliği güncelleme işlemi başlatıldı.";
        public const string DEBUG_UPDATE_SYSTEM_IDENTITY_COMPLETED = "[DEBUG] Sistem kimliği güncelleme işlemi tamamlandı.";
        public const string ERROR_UPDATE_SYSTEM_IDENTITY_FAILED = "[ERROR] Sistem kimliği güncelleme işlemi başarısız oldu. Hata: {0}";
        public const string ERROR_IDENTITY_SECTION_NOT_FOUND = "[ERROR] Kimlik bölümü bulunamadı.";
        public const string DEBUG_SYSTEM_IDENTITY_CHECK = "[DEBUG] Sistem kimliği kontrol ediliyor.";
        public const string DEBUG_SYSTEM_IDENTITY_RECEIVED = "[DEBUG] Sistem kimliği alındı: {0}";
        public const string DEBUG_CACHED_SYSTEM_IDENTITY = "[DEBUG] Önbellekteki sistem kimliği: {0}";
        public const string DEBUG_SEED_BYTE_ARRAY = "[DEBUG] Seed byte dizisi: {0}";
        public const string DEBUG_SYSTEM_IDENTITY_BYTE_ARRAY = "[DEBUG] Sistem kimliği byte dizisi: {0}";
        #endregion

        #region Form İşlemleri
        // Genel Form İşlemleri
        public const string DEBUG_FORM_INITIALIZE_STARTED = "[DEBUG] Form başlatma işlemi başlatıldı.";
        public const string DEBUG_DEV_MODE_OBJECT_INITIALIZED = "[DEBUG] Geliştirici modu nesnesi başlatıldı.";
        public const string DEBUG_MAX_LENGTH_SET = "[DEBUG] Maksimum karakter sınırı ayarlandı.";
        public const string DEBUG_FORM_SETTINGS_APPLIED = "[DEBUG] Form ayarları uygulandı.";
        public const string DEBUG_BACKGROUND_CREATED = "[DEBUG] Arka plan oluşturuldu.";
        public const string DEBUG_EVENTS_BOUND = "[DEBUG] Olaylar bağlandı.";
        public const string DEBUG_DEV_MODE_STATUS_CHECKED = "[DEBUG] Geliştirici modu durumu kontrol edildi.";
        public const string DEBUG_IMAGE_SELECTION_STARTED = "[DEBUG] Resim seçme işlemi başlatıldı.";
        public const string DEBUG_ENCRYPTION_STARTED = "[DEBUG] Şifreleme işlemi başlatıldı.";
        public const string DEBUG_DECRYPTION_STARTED = "[DEBUG] Şifre çözme işlemi başlatıldı.";
        public const string DEBUG_ANALYSIS_STARTED = "[DEBUG] Analiz işlemi başlatıldı.";
        public const string DEBUG_HELP_STARTED = "[DEBUG] Yardım işlemi başlatıldı.";
        public const string DEBUG_DEV_MODE_TOGGLE_STARTED = "[DEBUG] Geliştirici modu değiştirme işlemi başlatıldı.";
        public const string DEBUG_LOGIN_ICON_CLICKED = "[DEBUG] Giriş ikonu tıklandı.";
        public const string DEBUG_LOGIN_ICON_MOUSE_ENTER = "[DEBUG] Giriş ikonu üzerine gelindi.";
        public const string DEBUG_LOGIN_ICON_MOUSE_LEAVE = "[DEBUG] Giriş ikonu üzerinden ayrıldı.";
        public const string DEBUG_DEV_MODE_ACTIVATION_STARTED = "[DEBUG] Geliştirici modu etkinleştirme işlemi başlatıldı.";
        public const string DEBUG_ADMIN_BUTTON_CLICKED = "[DEBUG] Admin butonuna tıklandı.";
        public const string DEBUG_ADMIN_FORM_OPENING = "[DEBUG] Admin formu açılıyor.";
        public const string DEBUG_SIGNATURE_CHECK_STARTED = "[DEBUG] İmza kontrol işlemi başladı.";
        public const string DEBUG_SIGNATURE_CHECK_COMPLETED = "[DEBUG] İmza kontrolü tamamlandı.";
        #endregion

        #region Login Form İşlemleri
        public const string DEBUG_LOGIN_FORM_INITIALIZE_STARTED = "[DEBUG] Login form başlatma işlemi başlatıldı.";
        public const string DEBUG_LOGIN_FORM_CLOSING_EVENT_BOUND = "[DEBUG] Login form kapanma olayı bağlandı.";
        public const string DEBUG_LOGIN_FORM_LOADED = "[DEBUG] Login form yüklendi.";
        public const string DEBUG_LOGIN_FORM_POSITION_SET = "[DEBUG] Login form konumu ayarlandı.";
        public const string DEBUG_LOGIN_FORM_BACKGROUND_CREATED = "[DEBUG] Login form arka planı oluşturuldu.";
        public const string DEBUG_LOGIN_PROCESS_STARTED = "[DEBUG] Giriş işlemi başlatıldı.";
        public const string DEBUG_LOGIN_CREDENTIALS_OBTAINED = "[DEBUG] Giriş bilgileri alındı. Geliştirici ID: {0}";
        public const string DEBUG_LOGIN_RESULT_SUCCESSFUL = "[DEBUG] Giriş işlemi başarılı.";
        public const string DEBUG_LOGIN_PARENT_FORM_NOTIFIED = "[DEBUG] Ana forma giriş bilgisi gönderildi.";
        public const string DEBUG_LOGIN_FORM_CLOSING = "[DEBUG] Login form kapatılıyor.";
        public const string DEBUG_LOGIN_ENTER_KEY_PRESSED = "[DEBUG] Enter tuşuna basıldı, giriş işlemi başlatılıyor.";
        public const string DEBUG_LOGIN_CANCEL_PROCESS_STARTED = "[DEBUG] Giriş iptal işlemi başlatıldı.";
        public const string DEBUG_LOGIN_FORM_CLOSING_HANDLER = "[DEBUG] Login form kapanma işleyicisi çalıştırıldı.";
        #endregion

        #region Admin Form İşlemleri
        public const string DEBUG_ADMIN_FORM_INITIALIZE_STARTED = "[DEBUG] Admin form başlatma işlemi başlatıldı.";
        public const string DEBUG_ADMIN_FORM_CLOSING_EVENT_BOUND = "[DEBUG] Admin form kapanma olayı bağlandı.";
        public const string DEBUG_ADMIN_FORM_LOADED = "[DEBUG] Admin form yüklendi.";
        public const string DEBUG_ADMIN_SET_DEV_MODE_STARTED = "[DEBUG] Admin form için geliştirici modu ayarlama başlatıldı.";
        public const string DEBUG_ADMIN_DEV_MODE_ACCESS_DENIED = "[DEBUG] Admin form için geliştirici modu erişimi reddedildi.";
        public const string DEBUG_ADMIN_DEV_MODE_STATUS_CHECKED = "[DEBUG] Admin form için geliştirici modu durumu kontrol edildi.";
        public const string DEBUG_ADMIN_DEV_MODE_NOT_ACTIVE = "[DEBUG] Admin form için geliştirici modu aktif değil.";
        public const string DEBUG_ADMIN_DEV_MODE_SET_SUCCESS = "[DEBUG] Admin form için geliştirici modu başarıyla ayarlandı.";
        public const string DEBUG_ADMIN_DEV_MODE_UI_UPDATED = "[DEBUG] Admin form için geliştirici modu UI güncellendi: {0}";
        public const string DEBUG_ADMIN_FORM_SETTINGS_APPLIED = "[DEBUG] Admin form ayarları uygulandı.";
        public const string DEBUG_ADMIN_TITLE_BAR_SETUP = "[DEBUG] Admin form başlık çubuğu ayarlandı.";
        public const string DEBUG_ADMIN_BACKGROUND_CREATED = "[DEBUG] Admin form arka planı oluşturuldu.";
        public const string DEBUG_ADMIN_TEXT_BOXES_PREPARED = "[DEBUG] Admin form metin kutuları hazırlandı.";
        public const string DEBUG_ADMIN_INITIAL_DATA_LOAD_STARTED = "[DEBUG] Admin form ilk veri yükleme başlatıldı.";
        public const string DEBUG_ADMIN_CONFIG_FILE_PATH_OBTAINED = "[DEBUG] Admin form config dosya yolu alındı: {0}";
        public const string DEBUG_ADMIN_CONFIG_FILE_CHECKED = "[DEBUG] Admin form config dosyası kontrol edildi.";
        public const string DEBUG_ADMIN_IDENTITY_EXTRACTED = "[DEBUG] Admin form kimlik bilgisi çıkarıldı: {0}";
        public const string DEBUG_ADMIN_DEFAULT_IDENTITY_USED = "[DEBUG] Admin form varsayılan kimlik kullanıldı.";
        public const string DEBUG_ADMIN_RSA_KEYS_LOADED = "[DEBUG] Admin form RSA anahtarları yüklendi.";
        public const string DEBUG_ADMIN_FORM_CLOSING = "[DEBUG] Admin form kapatılıyor.";
        public const string DEBUG_ADMIN_RANDOM_IDENTITY_STARTED = "[DEBUG] Admin form rastgele kimlik oluşturma işlemi başlatıldı.";
        public const string DEBUG_ADMIN_RANDOM_IDENTITY_LENGTH = "[DEBUG] Admin form rastgele kimlik uzunluğu: {0}";
        public const string DEBUG_ADMIN_RANDOM_IDENTITY_CREATED = "[DEBUG] Admin form rastgele kimlik oluşturuldu: {0}";
        public const string DEBUG_ADMIN_SAVE_IDENTITY_STARTED = "[DEBUG] Admin form kimlik kaydetme işlemi başlatıldı.";
        public const string DEBUG_ADMIN_SAVE_IDENTITY_VALIDATION = "[DEBUG] Admin form kimlik kaydetme doğrulama.";
        public const string DEBUG_ADMIN_IDENTITY_SAVED = "[DEBUG] Admin form kimlik kaydedildi.";
        public const string DEBUG_ADMIN_RSA_KEYS_UPDATED = "[DEBUG] Admin form RSA anahtarları güncellendi.";
        public const string DEBUG_ADMIN_RESET_IDENTITY_STARTED = "[DEBUG] Admin form kimlik sıfırlama işlemi başlatıldı.";
        public const string DEBUG_ADMIN_RESET_CONFIRMATION = "[DEBUG] Admin form kimlik sıfırlama onayı istendi.";
        public const string DEBUG_ADMIN_RESET_DEV_MODE_CHECKED = "[DEBUG] Admin form sıfırlama için geliştirici modu kontrolü.";
        public const string DEBUG_ADMIN_DEFAULT_IDENTITY_LOADED = "[DEBUG] Admin form varsayılan kimlik yüklendi: {0}";
        public const string DEBUG_ADMIN_IDENTITY_RESET_SUCCESS = "[DEBUG] Admin form kimlik sıfırlama başarılı.";
        public const string DEBUG_ADMIN_HELP_BUTTON_CLICKED = "[DEBUG] Admin form yardım butonu tıklandı.";
        #endregion

        #region RSA İşlemleri
        public const string DEBUG_RSA_ENCRYPT_STARTED = "[DEBUG] RSA şifreleme işlemi başlatıldı.";
        #endregion

        #region Varsayılan Kimlik İşlemleri
        public const string DEBUG_DEFAULT_ID_RECEIVING = "[DEBUG] Varsayılan kimlik alınıyor.";
        public const string DEBUG_DEFAULT_ID_RECEIVED = "[DEBUG] Varsayılan kimlik alındı.";
        public const string DEBUG_DEFAULT_ID_PROCESSING = "[DEBUG] Varsayılan kimlik işleniyor.";
        public const string DEBUG_DEFAULT_ID_PROCESSED = "[DEBUG] Varsayılan kimlik işlendi.";
        public const string ERROR_DEFAULT_ID_NOT_FOUND = "[ERROR] Varsayılan kimlik bulunamadı.";
        #endregion

        #region Rastgele Üretici İşlemleri
        public const string DEBUG_DIGEST_RANDOM_GENERATOR_STARTED = "[DEBUG] DigestRandomGenerator başlatıldı.";
        public const string DEBUG_DIGEST_RANDOM_GENERATOR_SEED_ADDED = "[DEBUG] Seed DigestRandomGenerator'a eklendi.";
        public const string DEBUG_SECURE_RANDOM_CREATED = "[DEBUG] SecureRandom oluşturuldu.";
        #endregion

        #region Geliştirici Bilgileri İşlemleri
        public const string DEBUG_GET_DEVELOPER_INFO_STARTED = "[DEBUG] Geliştirici bilgileri alma işlemi başlatıldı.";
        public const string DEBUG_GET_DEVELOPER_INFO_COMPLETED = "[DEBUG] Geliştirici bilgileri alma işlemi tamamlandı.";
        public const string ERROR_NO_DEVELOPERS_FOUND = "[ERROR] Geliştirici bilgileri bulunamadı.";
        public const string ERROR_GET_DEVELOPER_INFO_FAILED = "[ERROR] Geliştirici bilgileri alma işlemi başarısız oldu. Hata: {0}";
        #endregion
    }
}