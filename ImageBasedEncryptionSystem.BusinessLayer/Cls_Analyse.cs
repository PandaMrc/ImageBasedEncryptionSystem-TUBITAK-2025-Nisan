using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Resim ve veri analizi için çeşitli metotlar içeren sınıf
    /// </summary>
    public class Cls_Analyse
    {
        /// <summary>
        /// İki resmi karşılaştırır ve farklılıkları gösterir
        /// </summary>
        /// <param name="originalImage">Orijinal resim</param>
        /// <param name="modifiedImage">Değiştirilmiş resim</param>
        /// <returns>Farklılıkları gösteren resim</returns>
        public Bitmap CompareImages(Bitmap originalImage, Bitmap modifiedImage)
        {
            return new Bitmap(1, 1);
        }

        /// <summary>
        /// Resmin histogramını oluşturur
        /// </summary>
        /// <param name="image">Analiz edilecek resim</param>
        /// <returns>Histogram resmi</returns>
        public Bitmap CreateHistogram(Bitmap image)
        {
            return new Bitmap(1, 1);
        }

        /// <summary>
        /// Resim gürültü seviyesini analiz eder
        /// </summary>
        /// <param name="image">Analiz edilecek resim</param>
        /// <returns>Gürültü seviye analiz sonucu</returns>
        public string AnalyzeNoise(Bitmap image)
        {
            return string.Empty;
        }

        /// <summary>
        /// Resimden PSNR (Peak Signal-to-Noise Ratio) değerini hesaplar
        /// </summary>
        /// <param name="originalImage">Orijinal resim</param>
        /// <param name="modifiedImage">Değiştirilmiş resim</param>
        /// <returns>PSNR değeri</returns>
        public double CalculatePSNR(Bitmap originalImage, Bitmap modifiedImage)
        {
            return 0.0;
        }
    }
}
