using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    public static class Debug
    {
        public const string DEBUG_RSA_ENSURE_KEY_PAIR_STARTED = "[DEBUG] EnsureKeyPair metodu başlatıldı.";
        public const string DEBUG_SYSTEM_IDENTITY_RECEIVED = "[DEBUG] Alınan SystemIdentity: {0}";
        public const string DEBUG_CACHED_SYSTEM_IDENTITY = "[DEBUG] Cached SystemIdentity: {0}";
        public const string DEBUG_EXISTING_KEY_PAIR = "[DEBUG] Mevcut anahtar çifti: {0}";
        public const string DEBUG_SYSTEM_IDENTITY_CHECK = "[DEBUG] SystemIdentity kontrol ediliyor: {0}";
        public const string DEBUG_SEED_BYTE_ARRAY = "[DEBUG] Seed byte dizisi: {0}";
        public const string DEBUG_DIGEST_RANDOM_GENERATOR_STARTED = "[DEBUG] DigestRandomGenerator başlatılıyor...";
        public const string DEBUG_DIGEST_RANDOM_GENERATOR_SEED_ADDED = "[DEBUG] DigestRandomGenerator için tohum eklendi.";
        public const string DEBUG_SECURE_RANDOM_CREATED = "[DEBUG] SecureRandom oluşturuldu.";
        public const string DEBUG_RSA_KEY_PAIR_GENERATOR_STARTED = "[DEBUG] RsaKeyPairGenerator başlatılıyor...";
        public const string DEBUG_RSA_KEY_PAIR_GENERATED = "[DEBUG] Anahtar çifti başarıyla üretildi.";
        public const string DEBUG_NEW_RSA_KEY_PAIR_CREATED = "[DEBUG] Yeni bir RSA anahtar çifti oluşturuldu.";
        public const string DEBUG_SYSTEM_IDENTITY_BYTE_ARRAY = "[DEBUG] SystemIdentity byte dizisi: {0}";
        public const string DEBUG_RSA_ENCRYPT_STARTED = "[DEBUG] RSA şifreleme işlemi başlatıldı.";
        public const string DEBUG_RSA_ENCRYPT_ENGINE_INIT = "[DEBUG] RSA şifreleme motoru başlatılıyor.";
        public const string DEBUG_RSA_ENCRYPT_PROCESSING = "[DEBUG] RSA şifreleme işlemi devam ediyor.";
        public const string DEBUG_RSA_ENCRYPT_PROCESSED = "[DEBUG] RSA şifreleme işlemi tamamlandı.";
        public const string DEBUG_RSA_DECRYPT_STARTED = "[DEBUG] RSA şifre çözme işlemi başlatıldı.";
        public const string DEBUG_RSA_DECRYPT_ENGINE_INIT = "[DEBUG] RSA şifre çözme motoru başlatılıyor.";
        public const string DEBUG_RSA_DECRYPT_PROCESSING = "[DEBUG] RSA şifre çözme işlemi devam ediyor.";
        public const string DEBUG_RSA_DECRYPT_PROCESSED = "[DEBUG] RSA şifre çözme işlemi tamamlandı.";
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_STARTED = "[DEBUG] Public Key PEM formatında dışa aktarım başlatıldı.";
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTING = "[DEBUG] Public Key dışa aktarılıyor.";
        public const string DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTED = "[DEBUG] Public Key dışa aktarıldı.";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_STARTED = "[DEBUG] Private Key PEM formatında dışa aktarım başlatıldı.";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTING = "[DEBUG] Private Key dışa aktarılıyor.";
        public const string DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTED = "[DEBUG] Private Key dışa aktarıldı.";
        public const string DEBUG_AES_GENERATE_KEY_STARTED = "[DEBUG] AES anahtar üretimi başlatıldı.";
        public const string DEBUG_AES_GENERATE_KEY_COMPLETED = "[DEBUG] AES anahtar üretimi tamamlandı.";
        public const string DEBUG_AES_ENCRYPT_STARTED = "[DEBUG] AES şifreleme işlemi başlatıldı.";
        public const string DEBUG_AES_ENCRYPT_ENGINE_INIT = "[DEBUG] AES şifreleme motoru başlatılıyor.";
        public const string DEBUG_AES_ENCRYPT_PROCESSING = "[DEBUG] AES şifreleme işlemi devam ediyor.";
        public const string DEBUG_AES_ENCRYPT_PROCESSED = "[DEBUG] AES şifreleme işlemi tamamlandı.";
        public const string DEBUG_AES_DECRYPT_STARTED = "[DEBUG] AES şifre çözme işlemi başlatıldı.";
        public const string DEBUG_AES_DECRYPT_ENGINE_INIT = "[DEBUG] AES şifre çözme motoru başlatılıyor.";
        public const string DEBUG_AES_DECRYPT_PROCESSING = "[DEBUG] AES şifre çözme işlemi devam ediyor.";
        public const string DEBUG_AES_DECRYPT_PROCESSED = "[DEBUG] AES şifre çözme işlemi tamamlandı.";
        public const string DEBUG_WAVELET_EMBED_TEXT_STARTED = "[DEBUG] Metin gömme işlemi başlatıldı. Metin: {0}, Görüntü boyutu: {1}x{2}";
        public const string DEBUG_WAVELET_EMBED_TEXT_PROCESSING = "[DEBUG] Metin gömme işlemi devam ediyor. Karakter: {0}, Bit: {1}, Piksel: ({2},{3})";
        public const string DEBUG_WAVELET_EMBED_TEXT_PROCESSED = "[DEBUG] Metin gömme işlemi tamamlandı.";
        public const string DEBUG_WAVELET_EXTRACT_TEXT_STARTED = "[DEBUG] Metin çıkarma işlemi başlatıldı. Görüntü boyutu: {0}x{1}";
        public const string DEBUG_WAVELET_EXTRACT_TEXT_PROCESSING = "[DEBUG] Metin çıkarma işlemi devam ediyor. Piksel: ({0},{1}), Bit: {2}";
        public const string DEBUG_WAVELET_EXTRACT_TEXT_PROCESSED = "[DEBUG] Metin çıkarma işlemi tamamlandı. Çıkarılan metin: {0}";
        public const string DEBUG_WAVELET_TRANSPARENCY_STARTED = "[DEBUG] Transparanlık sağlama işlemi başlatıldı. Görüntü boyutu: {0}x{1}";
        public const string DEBUG_WAVELET_TRANSPARENCY_PROCESSING = "[DEBUG] Transparanlık sağlama işlemi devam ediyor.";
        public const string DEBUG_WAVELET_TRANSPARENCY_PROCESSED = "[DEBUG] Transparanlık sağlama işlemi tamamlandı.";
        public const string DEBUG_IMAGE_LOAD_STARTED = "[DEBUG] Resim yükleme işlemi başlatıldı. Dosya: {0}";
        public const string DEBUG_IMAGE_LOAD_SUCCESS = "[DEBUG] Resim başarıyla yüklendi. Dosya: {0}";
        public const string DEBUG_DATA_EXTRACTED = "[DEBUG] Veri başarıyla çıkarıldı: {0}";
        public const string DEBUG_HASH_ADD_COMPLETED = "[DEBUG] Hash başarıyla eklendi.";
        public const string DEBUG_HASH_ADD_STARTED = "[DEBUG] Hash ekleme işlemi başlatıldı.";
        public const string DEBUG_HASH_CHECK_PASSED = "[DEBUG] Hash kontrolü başarıyla geçti.";
        public const string DEBUG_PASSWORD_MATCH = "[DEBUG] Şifreler eşleşiyor.";
    }
}
