using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting; // Grafik kütüphanesi

// Bu enum ve struct tanımlarının projenizde mevcut olduğunu varsayıyoruz
// (Önceki cevaplardan veya kendi tanımlamalarınızdan)

public struct RsAnalysisResult
{
    /// <summary>
    /// Tahmin edilen gömme oranı (LSB'leri değiştirilmiş piksellerin oranı, 0.0 ile 1.0 arasında).
    /// </summary>
    public double EstimatedEmbeddingRateP { get; set; }
    public long RM_Count { get; set; }      // Maske M için Regular grup sayısı
    public long SM_Count { get; set; }      // Maske M için Singular grup sayısı
    public long RMinusM_Count { get; set; } // Ters Maske -M için Regular grup sayısı
    public long SMinusM_Count { get; set; } // Ters Maske -M için Singular grup sayısı

    public override string ToString()
    {
        return $"Tahmini Gömme Oranı (p): {EstimatedEmbeddingRateP:P2}, RM: {RM_Count}, SM: {SM_Count}, R-M: {RMinusM_Count}, S-M: {SMinusM_Count}";
    }
}

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public static class Cls_RsHelper
    {
        // --- Yardımcı Metotlar ---

        private static byte FlipLsb(byte value)
        {
            return (byte)(value ^ 1); // XOR 1 ile LSB'yi ters çevirir
        }

        /// <summary>
        /// Diskriminasyon fonksiyonu: Bir gruptaki komşu pikseller arası mutlak farkların toplamı.
        /// </summary>
        private static int CalculateDiscrimination(byte[] group, int length)
        {
            int variation = 0;
            for (int i = 0; i < length - 1; i++)
            {
                variation += Math.Abs(group[i] - group[i + 1]);
            }
            return variation;
        }

        /// <summary>
        /// İkinci dereceden denklemin (ax^2 + bx + c = 0) kökünü [0, 0.5] aralığında arar.
        /// RS Analizinde alpha (~p/2) değerini bulmak için kullanılır.
        /// </summary>
        private static double SolveQuadraticForAlpha(double a, double b, double c)
        {
            if (Math.Abs(a) < 1e-10) // Lineer denklem durumu (a ~ 0)
            {
                if (Math.Abs(b) < 1e-10) return double.NaN; // Çözüm yok veya sonsuz çözüm
                double alpha = -c / b;
                return (alpha >= 0 && alpha <= 0.5) ? alpha : double.NaN;
            }

            double discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return double.NaN; // Gerçel kök yok

            double sqrtDiscriminant = Math.Sqrt(discriminant);
            double alpha1 = (-b + sqrtDiscriminant) / (2 * a);
            double alpha2 = (-b - sqrtDiscriminant) / (2 * a);

            // [0, 0.5] aralığındaki kökü seç
            bool alpha1Valid = (alpha1 >= 0 && alpha1 <= 0.5);
            bool alpha2Valid = (alpha2 >= 0 && alpha2 <= 0.5);

            if (alpha1Valid && alpha2Valid)
            {
                return Math.Abs(alpha1) < Math.Abs(alpha2) ? alpha1 : alpha2; // Mutlak değeri küçük olanı seç (genellikle)
            }
            if (alpha1Valid) return alpha1;
            if (alpha2Valid) return alpha2;

            return double.NaN; // Uygun kök bulunamadı
        }

        // --- Ana Regular/Singular Gruplar Analiz Metodu ---

        /// <summary>
        /// Bir görüntünün belirtilen renk kanalında LSB steganografisi varlığını
        /// Regular/Singular (RS) Gruplar Analizi kullanarak analiz eder ve gömme oranını tahmin eder.
        /// </summary>
        /// <param name="sourceImage">Analiz edilecek Bitmap nesnesi.</param>
        /// <param name="channel">Analiz edilecek renk kanalı (R, G, B).</param>
        /// <param name="groupSize">Piksel grup boyutu (genellikle 2, 3 veya 4). Örnek 4 kullanır.</param>
        /// <returns>RS Analiz sonucunu içeren RsAnalysisResult yapısı.</returns>
        public static RsAnalysisResult PerformRegularSingularAnalysis(Bitmap sourceImage, ColorChannel channel, int groupSize = 4)
        {
            if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));
            if (groupSize < 2) throw new ArgumentException("Grup boyutu en az 2 olmalıdır.", nameof(groupSize));

            // JPEG uyarısı
            if (sourceImage.RawFormat.Equals(ImageFormat.Jpeg))
            {
                Console.WriteLine("Uyarı: RS Gruplar Analizi JPEG gibi kayıplı formatlarda güvenilir sonuçlar vermeyebilir.");
            }

            long rm_count = 0, sm_count = 0;         // Maske M için Regular/Singular sayaçları
            long r_minus_m_count = 0, s_minus_m_count = 0; // Ters Maske -M için Regular/Singular sayaçları

            // Maskeleri tanımla (groupSize=4 için örnek)
            // Maske M (0,1,0,1) -> 1. ve 3. indisteki piksellerin LSB'sini çevir (0-tabanlı)
            // Ters Maske -M (1,0,1,0) -> 0. ve 2. indisteki piksellerin LSB'sini çevir
            // Bu, literatürdeki yaygın M = [..0101..] ve -M = [..1010..] maskelerine benzer.
            // Veya daha basit M=[0,1,1,0] ve -M=[1,0,0,1] de kullanılabilir.
            // Bu örnekte, bir grup içindeki bitişiklik farklarına dayalı standart bir yaklaşım kullanalım.
            // F(G) ve F_inv(G) tanımlamaları önemli.
            // Basitlik adına, Maske M: pikselleri olduğu gibi bırakır (F0).
            // Maske M1: tüm LSB'leri çevirir. (Bu RS-diyagramları için kullanılır)
            // Daha yaygın RS Analizi için "çevirme fonksiyonları" kullanılır.
            // F1: Tek indisli piksellerin LSB'sini çevir. F-1: Çift indisli piksellerin LSB'sini çevir.

            int width = sourceImage.Width;
            int height = sourceImage.Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData sourceData = null;

            // Grup verilerini tutmak için diziler (döngü dışında oluşturularak optimize edildi)
            byte[] groupOriginal = new byte[groupSize];
            byte[] groupMaskM = new byte[groupSize];    // Maske M uygulanmış grup
            byte[] groupMaskMinusM = new byte[groupSize];// Ters Maske -M uygulanmış grup

            try
            {
                sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                IntPtr sourceScan0 = sourceData.Scan0;
                int sourceStride = sourceData.Stride;
                int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;

                if (sourceBytesPerPixel < 3)
                    throw new ArgumentException("RS Analizi için en az 24bpp formatında bir resim gereklidir.", nameof(sourceImage));

                int channelOffset = 0;
                switch (channel)
                {
                    case ColorChannel.Blue: channelOffset = 0; break;
                    case ColorChannel.Green: channelOffset = 1; break;
                    case ColorChannel.Red: channelOffset = 2; break;
                }
                if (channelOffset >= sourceBytesPerPixel)
                    throw new ArgumentException($"Seçilen kanal '{channel}' bu resim formatında bulunmuyor.");

                unsafe
                {
                    // Non-overlapping gruplar üzerinde çalış
                    for (int y = 0; y < height; y++) // Satırları dolaş
                    {
                        byte* sourceRow = (byte*)sourceScan0 + (y * sourceStride);
                        for (int x = 0; x <= width - groupSize; x += groupSize) // Sütunları gruplar halinde dolaş
                        {
                            // Grubu ve çevrilmiş grupları doldur
                            for (int i = 0; i < groupSize; i++)
                            {
                                byte pixelValue = sourceRow[(x + i) * sourceBytesPerPixel + channelOffset];
                                groupOriginal[i] = pixelValue;
                                // Maske M: Örneğin, tek indisli pikselleri çevir (indis 1, 3, ...)
                                groupMaskM[i] = (i % 2 != 0) ? FlipLsb(pixelValue) : pixelValue;
                                // Ters Maske -M: Örneğin, çift indisli pikselleri çevir (indis 0, 2, ...)
                                groupMaskMinusM[i] = (i % 2 == 0) ? FlipLsb(pixelValue) : pixelValue;
                            }

                            int fG = CalculateDiscrimination(groupOriginal, groupSize);
                            int fGM = CalculateDiscrimination(groupMaskM, groupSize);
                            int fGMinusM = CalculateDiscrimination(groupMaskMinusM, groupSize);

                            // Maske M için sınıflandırma
                            if (fGM > fG) rm_count++;
                            else if (fGM < fG) sm_count++;

                            // Ters Maske -M için sınıflandırma
                            if (fGMinusM > fG) r_minus_m_count++;
                            else if (fGMinusM < fG) s_minus_m_count++;
                        }
                    }
                } // unsafe
            }
            finally
            {
                if (sourceData != null) sourceImage.UnlockBits(sourceData);
            }

            // Gömme oranını tahmin et
            // Bu, Aletheia gibi araçlarda kullanılan yaygın bir ikinci dereceden denklem çözümüne dayanır.
            // alpha (~p/2) için denklemi çözer, burada p gizli mesajın göreceli uzunluğudur (değiştirilen LSB oranı).
            // Denklem: A * alpha^2 + B * alpha + C = 0
            double R_M = rm_count;
            double S_M = sm_count;
            double R_minus_M = r_minus_m_count;
            double S_minus_M = s_minus_m_count;

            // Eğer sayaçlar çok düşükse, anlamlı bir sonuç çıkmayabilir.
            if (R_M + S_M + R_minus_M + S_minus_M < 100) // Eşik değeri
            {
                Console.WriteLine("Uyarı: RS Analizi için yeterli sayıda Regular/Singular grup bulunamadı.");
                return new RsAnalysisResult { EstimatedEmbeddingRateP = 0, RM_Count = rm_count, SM_Count = sm_count, RMinusM_Count = r_minus_m_count, SMinusM_Count = s_minus_m_count };
            }

            // Friedman ve Fridrich'in formülasyonuna göre katsayılar
            // d0 = R_M - S_M
            // d1 = R_minus_M - S_minus_M
            // Denklem: 2(d1 - d0)x^2 - (d1 + d0)x + d0 = 0
            double d0 = R_M - S_M;
            double d1 = R_minus_M - S_minus_M;

            double coeffA = 2 * (d1 - d0);
            double coeffB = -(d1 + d0);
            double coeffC = d0;

            double alpha = SolveQuadraticForAlpha(coeffA, coeffB, coeffC);
            double estimatedP = 0.0;

            if (!double.IsNaN(alpha))
            {
                estimatedP = 2 * alpha; // p = 2 * alpha (alpha, p/2'yi temsil eder)
                
                // Negatif değerleri sıfıra, 1'den büyük değerleri 1'e çek
                estimatedP = Math.Max(0.0, Math.Min(1.0, estimatedP));
                
                // Çok küçük değerleri sıfıra yuvarla (gürültüyü azalt)
                if (estimatedP < 0.01)
                {
                    estimatedP = 0.0;
                }
            }
            else
            {
                // Alternatif basit hesaplama yöntemi
                // Eğer d0 ve d1 arasındaki fark çok azsa, gömme oranı düşüktür
                double simpleDiff = Math.Abs(d0 - d1) / Math.Max(1.0, R_M + S_M + R_minus_M + S_minus_M);
                estimatedP = Math.Min(simpleDiff * 2, 0.5); // Basit bir ölçekleme
                
                Console.WriteLine("Uyarı: Kuadratik denklemden uygun alpha kökü bulunamadı, basit tahmin kullanılıyor.");
            }

            return new RsAnalysisResult
            {
                EstimatedEmbeddingRateP = estimatedP,
                RM_Count = rm_count,
                SM_Count = sm_count,
                RMinusM_Count = r_minus_m_count,
                SMinusM_Count = s_minus_m_count
            };
        }


        // --- RS Grupları Görselleştirme Metodu ---

        /// <summary>
        /// RS Analizi sonucu elde edilen R ve S grup sayılarını bir bar grafiği olarak görselleştirir.
        /// </summary>
        public static Bitmap VisualizeRsGroupCounts(RsAnalysisResult result, Size chartSize, string title = "RS Grup Sayıları")
        {
            using (var chart = new Chart { Size = chartSize })
            {
                chart.Palette = ChartColorPalette.Pastel; // Renk paleti
                chart.Titles.Add(title);

                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.AxisX.Title = "Grup Türleri";
                chartArea.AxisY.Title = "Sayı (Adet)";
                chartArea.AxisX.MajorGrid.Enabled = false; // X ekseni gridini kapat
                chart.ChartAreas.Add(chartArea);

                Series seriesCounts = new Series("Counts")
                {
                    ChartType = SeriesChartType.Column, // Sütun grafik
                    IsValueShownAsLabel = true // Sütunların üzerinde değerleri göster
                };

                seriesCounts.Points.AddXY("R (Maske M)", result.RM_Count);
                seriesCounts.Points.AddXY("S (Maske M)", result.SM_Count);
                seriesCounts.Points.AddXY("R (Maske -M)", result.RMinusM_Count);
                seriesCounts.Points.AddXY("S (Maske -M)", result.SMinusM_Count);

                // Farkları da ekleyebiliriz (opsiyonel)
                Series seriesDifferences = new Series("Farklar (R-S)")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Color = Color.LightSalmon
                };
                seriesDifferences.Points.AddXY("R_M - S_M", result.RM_Count - result.SM_Count);
                seriesDifferences.Points.AddXY("R_-M - S_-M", result.RMinusM_Count - result.SMinusM_Count);


                chart.Series.Add(seriesCounts);
                // chart.Series.Add(seriesDifferences); // Farkları göstermek isterseniz bu satırı açın

                // Bitmap'e render et
                Bitmap bmp = new Bitmap(chartSize.Width, chartSize.Height);
                chart.DrawToBitmap(bmp, new Rectangle(0, 0, chartSize.Width, chartSize.Height));
                return bmp;
            }
        }

    }
}
// End of Cls_RsHelper