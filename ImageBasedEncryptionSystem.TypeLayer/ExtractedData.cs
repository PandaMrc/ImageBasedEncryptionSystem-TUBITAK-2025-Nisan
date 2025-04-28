using System;

namespace ImageBasedEncryptionSystem.TypeLayer
{
    /// <summary>
    /// Görüntüden çıkarılan şifreli verileri içeren model sınıfı
    /// </summary>
    public class ExtractedData
    {
        /// <summary>
        /// Görüntüden çıkarılan şifrelenmiş metin
        /// </summary>
        public string EncryptedText { get; set; }

        /// <summary>
        /// Görüntüden çıkarılan şifrelenmiş anahtar
        /// </summary>
        public string EncryptedKey { get; set; }
    }
} 