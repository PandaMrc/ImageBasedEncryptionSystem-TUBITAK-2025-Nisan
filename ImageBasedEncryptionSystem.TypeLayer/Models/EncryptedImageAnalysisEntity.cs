using System;
using System.Drawing;

namespace ImageBasedEncryptionSystem.TypeLayer.Models
{
    /// <summary>
    /// Şifrelenmiş görüntü analiz verilerini içeren sınıf
    /// </summary>
    public class EncryptedImageAnalysisEntity
    {
        /// <summary>
        /// Analiz ID
        /// </summary>
        public int AnalysisId { get; set; }

        /// <summary>
        /// Analiz zamanı
        /// </summary>
        public DateTime AnalysisTime { get; set; }

        /// <summary>
        /// Orijinal görüntü
        /// </summary>
        public Bitmap OriginalImage { get; set; }

        /// <summary>
        /// Şifrelenmiş görüntü
        /// </summary>
        public Bitmap EncryptedImage { get; set; }

        /// <summary>
        /// Fark ısı haritası
        /// </summary>
        public Bitmap DifferenceHeatmap { get; set; }

        /// <summary>
        /// LSB görselleştirme
        /// </summary>
        public Bitmap LsbVisualization { get; set; }

        /// <summary>
        /// Orijinal metin
        /// </summary>
        public string OriginalText { get; set; }

        /// <summary>
        /// Şifrelenmiş metin
        /// </summary>
        public string EncryptedText { get; set; }

        /// <summary>
        /// AES anahtarı
        /// </summary>
        public string AesKey { get; set; }

        /// <summary>
        /// RSA ile şifrelenmiş anahtar
        /// </summary>
        public string RsaEncryptedKey { get; set; }

        /// <summary>
        /// Şifreleme için kullanılan parola
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Değiştirilen piksel yüzdesi
        /// </summary>
        public double ModifiedPixelsPercentage { get; set; }

        /// <summary>
        /// Orijinal görüntü entropi değeri
        /// </summary>
        public double OriginalImageEntropy { get; set; }

        /// <summary>
        /// Şifrelenmiş görüntü entropi değeri
        /// </summary>
        public double EncryptedImageEntropy { get; set; }

        /// <summary>
        /// Parola güç puanı
        /// </summary>
        public double PasswordStrength { get; set; }

        /// <summary>
        /// Orijinal metin entropi değeri
        /// </summary>
        public double OriginalTextEntropy { get; set; }

        /// <summary>
        /// Şifrelenmiş metin entropi değeri
        /// </summary>
        public double EncryptedTextEntropy { get; set; }

        /// <summary>
        /// Analiz raporu
        /// </summary>
        public string AnalysisReport { get; set; }
    }
} 