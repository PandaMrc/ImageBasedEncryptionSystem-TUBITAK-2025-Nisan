using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting; // Grafik kütüphanesi
using System.Linq; // Gerekirse Linq için

// ColorChannel enum ve benzeri yapılar projenizde tanımlı olmalı
public struct SpaAnalysisResult
{
    /// <summary>
    /// Tahmin edilen gömme oranı (LSB'leri değiştirilmiş piksellerin oranı, 0.0 ile 1.0 arasında).
    /// </summary>
    public double EstimatedEmbeddingRateP { get; set; }
    // SPA'nın iç hesaplamalarında kullandığı bazı sayaçlar veya değerler eklenebilir
    // Örneğin: X0, Y0, X1, Y1 gibi (Dumitrescu et al. makalesindeki notasyon)
    public long X0_Count { get; set; }
    public long Y0_Count { get; set; }
    public long X1_Count { get; set; }
    public long Y1_Count { get; set; }


    public override string ToString()
    {
        return $"SPA Tahmini Gömme Oranı (p): {EstimatedEmbeddingRateP:P3}, X0:{X0_Count}, Y0:{Y0_Count}, X1:{X1_Count}, Y1:{Y1_Count}";
    }
}
namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public static class Cls_SpaHelper
    {
        private static byte FlipLsb(byte value) => (byte)(value ^ 1);

        /// <summary>
        /// İkinci dereceden denklemi (ax^2 + bx + c = 0) çözer ve [0,1] aralığındaki kökü döndürür.
        /// SPA'da p/(2-p) gibi bir terimi bulmak için kullanılır.
        /// </summary>
        private static double SolveQuadraticForD(double a, double b, double c)
        {
            if (Math.Abs(a) < 1e-10) // Lineer denklem
            {
                if (Math.Abs(b) < 1e-10) return double.NaN;
                double d = -c / b;
                return (d >= 0 && d <= 1.0) ? d : double.NaN; // d'nin aralığı p/(2-p) için [0,1]
            }

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return double.NaN;

            double sqrtDiscriminant = Math.Sqrt(discriminant);
            double d1 = (-b + sqrtDiscriminant) / (2 * a);
            double d2 = (-b - sqrtDiscriminant) / (2 * a);

            bool d1Valid = (d1 >= 0 && d1 <= 1.0);
            bool d2Valid = (d2 >= 0 && d2 <= 1.0);

            if (d1Valid && d2Valid) return Math.Min(d1, d2); // Genellikle küçük olan kök alınır
            if (d1Valid) return d1;
            if (d2Valid) return d2;

            return double.NaN;
        }

        /// <summary>
        /// Sample Pair Analysis (SPA) gerçekleştirir.
        /// Bu, Dumitrescu, Wu, et al. tarafından önerilen temel bir SPA yaklaşımına dayanmaktadır.
        /// </summary>
        public static SpaAnalysisResult PerformSpaAnalysis(Bitmap sourceImage, ColorChannel channel)
        {
            if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));

            // JPEG uyarısı
            if (sourceImage.RawFormat.Equals(ImageFormat.Jpeg))
            {
                System.Diagnostics.Debug.WriteLine("Uyarı: SPA, JPEG gibi kayıplı formatlarda güvenilir sonuçlar vermeyebilir.");
            }

            long x0 = 0, y0 = 0, x1 = 0, y1 = 0; // SPA sayaçları
            long totalPairs = 0; // İncelenen toplam piksel çifti sayısı

            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData sourceData = null;

            try
            {
                sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                IntPtr sourceScan0 = sourceData.Scan0;
                int sourceStride = sourceData.Stride;
                int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

                if (sourceBytesPerPixel < 3 && sourceImage.PixelFormat != PixelFormat.Format8bppIndexed)
                    throw new ArgumentException("SPA için en az 24bpp renkli veya 8bpp gri tonlama formatında bir resim gereklidir.", nameof(sourceImage));

                int channelOffset = 0;
                bool isGrayscale = sourceImage.PixelFormat == PixelFormat.Format8bppIndexed;

                if (!isGrayscale)
                {
                    switch (channel)
                    {
                        case ColorChannel.Blue: channelOffset = 0; break;
                        case ColorChannel.Green: channelOffset = 1; break;
                        case ColorChannel.Red: channelOffset = 2; break;
                    }
                    if (channelOffset >= sourceBytesPerPixel)
                        throw new ArgumentException($"Seçilen kanal '{channel}' bu resim formatında bulunmuyor.");
                }

                unsafe
                {
                    // Genellikle yatay komşu pikseller (çiftler) analiz edilir
                    for (int y = 0; y < height; y++)
                    {
                        byte* sourceRow = (byte*)sourceScan0 + (y * sourceStride);
                        // Çiftleri analiz etmek için (width - 1)'e kadar git
                        for (int x = 0; x < width - 1; x++) // Son pikselin sağ komşusu olmayacak
                        {
                            byte p1, p2; // Komşu piksel çifti

                            if (isGrayscale)
                            {
                                p1 = sourceRow[x];
                                p2 = sourceRow[x + 1];
                            }
                            else
                            {
                                p1 = sourceRow[x * sourceBytesPerPixel + channelOffset];
                                p2 = sourceRow[(x + 1) * sourceBytesPerPixel + channelOffset];
                            }

                            // SPA Mantığı (Dumitrescu et al. 2003 "Detection of LSB Steganography in Color and Grayscale Images")
                            // F(p) = p XOR 1 (LSB'yi çevirir)
                            // Eğer LSB(F(p1)) < LSB(p2) ise Y0 artır, LSB(F(p1)) > LSB(p2) ise X0 artır
                            // Eğer LSB(p1) < LSB(F(p2)) ise Y1 artır, LSB(p1) > LSB(F(p2)) ise X1 artır

                            if ((FlipLsb(p1) & 1) < (p2 & 1)) y0++;
                            else if ((FlipLsb(p1) & 1) > (p2 & 1)) x0++;
                            // else durumunda (eşitse) sayaçlar artırılmaz

                            if ((p1 & 1) < (FlipLsb(p2) & 1)) y1++;
                            else if ((p1 & 1) > (FlipLsb(p2) & 1)) x1++;
                            // else durumunda (eşitse) sayaçlar artırılmaz
                            
                            totalPairs++;
                        }
                    }
                } // unsafe
            }
            finally
            {
                if (sourceData != null) sourceImage.UnlockBits(sourceData);
            }

            if (totalPairs == 0 || x0 + y0 + x1 + y1 == 0) // Hiçbir çift koşulu sağlamadıysa (çok küçük resim vs.)
            {
                System.Diagnostics.Debug.WriteLine("Uyarı: SPA için yeterli sayıda örneklem çifti bulunamadı.");
                return new SpaAnalysisResult { EstimatedEmbeddingRateP = 0, X0_Count = x0, Y0_Count = y0, X1_Count = x1, Y1_Count = y1 };
            }

            // Sayaçları kontrol et - çok düşük değerler için güvenilir sonuç vermez
            double sampleRatio = (double)(x0 + y0 + x1 + y1) / totalPairs;
            if (sampleRatio < 0.1) // Toplam çiftlerin %10'undan azı koşulları sağlıyorsa
            {
                System.Diagnostics.Debug.WriteLine("Uyarı: SPA için yeterli sayıda anlamlı örnek çifti bulunamadı.");
                return new SpaAnalysisResult { EstimatedEmbeddingRateP = 0, X0_Count = x0, Y0_Count = y0, X1_Count = x1, Y1_Count = y1 };
            }

            // Düzeltilmiş SPA algoritması
            // Dumitrescu et al. makalesindeki formül: p = 2(y0-x1)/(x0+y0+x1+y1)
            double numerator = 2.0 * (y0 - x1);
            double denominator = x0 + y0 + x1 + y1;
            
            if (Math.Abs(denominator) < 1e-10) // Sıfıra bölme kontrolü
            {
                return new SpaAnalysisResult { EstimatedEmbeddingRateP = 0, X0_Count = x0, Y0_Count = y0, X1_Count = x1, Y1_Count = y1 };
            }
            
            double estimatedP = numerator / denominator;
            
            // Sonuçları normalize et ve sınırla
            estimatedP = Math.Max(0.0, Math.Min(0.33, estimatedP));
            
            // Çok küçük değerleri sıfıra yuvarla
            if (Math.Abs(estimatedP) < 0.01)
            {
                estimatedP = 0.0;
            }

            // Sonuçları döndür
            return new SpaAnalysisResult
            {
                EstimatedEmbeddingRateP = estimatedP,
                X0_Count = x0,
                Y0_Count = y0,
                X1_Count = x1,
                Y1_Count = y1
            };
        }

        /// <summary>
        /// SPA Analizi sonucu elde edilen X ve Y sayaçlarını bir bar grafiği olarak görselleştirir.
        /// </summary>
        public static Bitmap VisualizeSpaCounts(SpaAnalysisResult result, Size chartSize, string title = "SPA Sayaçları")
        {
            using (var chart = new Chart { Size = chartSize })
            {
                chart.Palette = ChartColorPalette.Pastel;
                if (!string.IsNullOrEmpty(title))
                {
                    chart.Titles.Add(title);
                }

                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Sayaç Türleri";
                chartArea.AxisY.Title = "Sayı (Adet)";
                chartArea.AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas.Add(chartArea);

                Series seriesCounts = new Series("SPA Sayaçları")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true
                };

                seriesCounts.Points.AddXY("X0", result.X0_Count);
                seriesCounts.Points.AddXY("Y0", result.Y0_Count);
                seriesCounts.Points.AddXY("X1", result.X1_Count);
                seriesCounts.Points.AddXY("Y1", result.Y1_Count);

                chart.Series.Add(seriesCounts);
                // Lejant bu basit grafik için gereksiz olabilir
                // chart.Legends.Add(new Legend("DefaultLegend"));

                Bitmap bmp = new Bitmap(chartSize.Width, chartSize.Height);
                chart.DrawToBitmap(bmp, new Rectangle(0, 0, chartSize.Width, chartSize.Height));
                return bmp;
            }
        }

    } // End of Cls_SamplePairAnalysis
}