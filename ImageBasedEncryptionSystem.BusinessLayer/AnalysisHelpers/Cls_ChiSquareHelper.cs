using System;
using System.Collections.Generic; // List için eklendi
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting; // Grafik kütüphanesi
using MathNet.Numerics.Distributions;


/// <summary>
/// Ki-Kare analizi sonucunu tutan yapı.
/// </summary>
public struct ChiSquareResult
{
    /// Hesaplanan Ki-Kare istatistik değeri.
    public double ChiSquareValue { get; set; }
    /// Hesaplamada kullanılan serbestlik derecesi.
    public int DegreesOfFreedom { get; set; }
    /// Hesaplanan p-değeri. Düşük p-değerleri (örn. < 0.05) LSB gizlemesi olasılığını gösterir.
    /// MathNet.Numerics kütüphanesi eklenmediyse bu değer NaN olabilir.
    public double PValue { get; set; }

    public override string ToString()
    {
        // P-değeri hesaplanmadıysa farklı bir formatta göster
        string pValueString = double.IsNaN(PValue) ? "Hesaplanmadı" : PValue.ToString("G4");
        return $"Ki-Kare: {ChiSquareValue:F2}, Serbestlik Derecesi: {DegreesOfFreedom}, p-değeri: {pValueString}";
    }
}
/// <summary>
/// Ki-Kare (Chi-Square) LSB Steganaliz yöntemleri ve görselleştirmeleri için yardımcı metotlar içerir.
/// </summary>
public static class Cls_ChiSquareHelper
{
    // --- Özel Yardımcı Metotlar ---

    /// <summary>
    /// Görüntünün belirtilen bölgesindeki piksel frekanslarını hesaplar.
    /// </summary>
    private static long[] CalculateFrequencies(BitmapData sourceData, Rectangle rect, ColorChannel channel)
    {
        long[] frequencies = new long[256];
        int bytesPerPixel = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
        IntPtr scan0 = sourceData.Scan0;
        int stride = sourceData.Stride;

        int channelOffset = 0;
        switch (channel)
        {
            case ColorChannel.Blue: channelOffset = 0; break;
            case ColorChannel.Green: channelOffset = 1; break;
            case ColorChannel.Red: channelOffset = 2; break;
        }
        if (channelOffset >= bytesPerPixel) throw new ArgumentException("Geçersiz kanal.");

        unsafe
        {
            for (int y = rect.Top; y < rect.Bottom && y < sourceData.Height; y++)
            {
                byte* row = (byte*)scan0 + (y * stride);
                for (int x = rect.Left; x < rect.Right && x < sourceData.Width; x++)
                {
                    frequencies[row[x * bytesPerPixel + channelOffset]]++;
                }
            }
        }
        return frequencies;
    }

    /// <summary>
    /// Verilen frekans dizisine göre Ki-Kare sonucunu hesaplar.
    /// </summary>
    private static ChiSquareResult CalculateChiSquareInternal(long[] frequencies)
    {
        double chiSquareSum = 0.0;
        int degreesOfFreedom = 0;
        for (int k = 0; k < 128; k++)
        {
            long obs2k = frequencies[2 * k];
            long obs2kPlus1 = frequencies[2 * k + 1];
            long totalFrequencyInPair = obs2k + obs2kPlus1;

            if (totalFrequencyInPair > 0)
            {
                chiSquareSum += Math.Pow(obs2k - obs2kPlus1, 2) / (double)totalFrequencyInPair;
                degreesOfFreedom++;
            }
        }

        double pValue = double.NaN;
        if (degreesOfFreedom > 0 && chiSquareSum >= 0)
        {
            try { pValue = 1.0 - ChiSquared.CDF(degreesOfFreedom, chiSquareSum); }
            catch { /* MathNet yoksa NaN kalır */ }
        }

        return new ChiSquareResult
        {
            ChiSquareValue = chiSquareSum,
            DegreesOfFreedom = degreesOfFreedom,
            PValue = pValue
        };
    }

    /// <summary>
    /// Bellekte bir Chart nesnesini Bitmap'e render eder.
    /// </summary>
    private static Bitmap RenderChartToBitmap(Chart chart, Size size)
    {
        Bitmap bmp = new Bitmap(size.Width, size.Height);
        chart.DrawToBitmap(bmp, new Rectangle(0, 0, size.Width, size.Height));
        return bmp;
    }

    // --- Genel Görselleştirme Metotları ---

    /// <summary>
    /// 1. Çift Frekans Karşılaştırma Grafiği oluşturur.
    /// </summary>
    public static Bitmap GeneratePairedFrequencyChart(Bitmap sourceImage, ColorChannel channel, Size chartSize, string title = "Çift Frekans Karşılaştırması")
    {
        if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));

        long[] frequencies = null;
        Rectangle rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
        BitmapData sourceData = null;
        try
        {
            sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            frequencies = CalculateFrequencies(sourceData, rect, channel);
        }
        finally
        {
            if (sourceData != null) sourceImage.UnlockBits(sourceData);
        }

        using (var chart = new Chart { Size = chartSize })
        {
            chart.Titles.Add(title + $" ({channel})");
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Değer Çiftleri (2k, 2k+1)";
            chartArea.AxisY.Title = "Frekans";
            chartArea.AxisX.Interval = 16; // X eksenini biraz seyrelt
            chartArea.AxisX.LabelStyle.Angle = -45; // Etiketleri eğik yaz
            chart.ChartAreas.Add(chartArea);

            Series seriesEven = new Series("Çift Değerler (2k)") { ChartType = SeriesChartType.Column };
            Series seriesOdd = new Series("Tek Değerler (2k+1)") { ChartType = SeriesChartType.Column };

            for (int k = 0; k < 128; k++)
            {
                string pairLabel = $"{2 * k}-{2 * k + 1}";
                // Sadece belirli aralıklarla etiket ekle, yoksa çok kalabalık olur
                string label = (k % 8 == 0) ? pairLabel : "";
                seriesEven.Points.Add(new DataPoint(k, frequencies[2 * k]) { AxisLabel = label });
                seriesOdd.Points.Add(new DataPoint(k, frequencies[2 * k + 1]) { AxisLabel = label }); // Aynı etiketi kullanır
            }

            chart.Series.Add(seriesEven);
            chart.Series.Add(seriesOdd);
            chart.Legends.Add(new Legend("DefaultLegend"));

            return RenderChartToBitmap(chart, chartSize);
        }
    }

    /// <summary>
    /// 2. Frekans Farkları Grafiği oluşturur.
    /// </summary>
    public static Bitmap GenerateFrequencyDifferenceChart(Bitmap sourceImage, ColorChannel channel, Size chartSize, string title = "Frekans Farkları")
    {
        if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));

        long[] frequencies = null;
        Rectangle rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
        BitmapData sourceData = null;
        try
        {
            sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            frequencies = CalculateFrequencies(sourceData, rect, channel);
        }
        finally
        {
            if (sourceData != null) sourceImage.UnlockBits(sourceData);
        }

        using (var chart = new Chart { Size = chartSize })
        {
            chart.Titles.Add(title + $" ({channel})");
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Değer Çifti İndeksi (k)";
            chartArea.AxisY.Title = "Frekans Farkı (F[2k] - F[2k+1])";
            chartArea.AxisX.Interval = 16;
            chart.ChartAreas.Add(chartArea);

            Series seriesDiff = new Series("Farklar") { ChartType = SeriesChartType.Column }; // veya Line

            for (int k = 0; k < 128; k++)
            {
                seriesDiff.Points.AddXY(k, frequencies[2 * k] - frequencies[2 * k + 1]);
            }

            chart.Series.Add(seriesDiff);

            return RenderChartToBitmap(chart, chartSize);
        }
    }

    /// <summary>
    /// 3. Ki-Kare Katkı Grafiği oluşturur.
    /// </summary>
    public static Bitmap GenerateChiSquareContributionChart(Bitmap sourceImage, ColorChannel channel, Size chartSize, string title = "Ki-Kare Katkıları")
    {
        if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));

        long[] frequencies = null;
        Rectangle rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
        BitmapData sourceData = null;
        try
        {
            sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            frequencies = CalculateFrequencies(sourceData, rect, channel);
        }
        finally
        {
            if (sourceData != null) sourceImage.UnlockBits(sourceData);
        }

        using (var chart = new Chart { Size = chartSize })
        {
            chart.Titles.Add(title + $" ({channel})");
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Değer Çifti İndeksi (k)";
            chartArea.AxisY.Title = "Çifte Ait Ki-Kare Katkısı";
            chartArea.AxisX.Interval = 16;
            chart.ChartAreas.Add(chartArea);

            Series seriesContrib = new Series("Katkılar") { ChartType = SeriesChartType.Column };

            for (int k = 0; k < 128; k++)
            {
                long obs2k = frequencies[2 * k];
                long obs2kPlus1 = frequencies[2 * k + 1];
                long totalFrequencyInPair = obs2k + obs2kPlus1;
                double contribution = 0;
                if (totalFrequencyInPair > 0)
                {
                    contribution = Math.Pow(obs2k - obs2kPlus1, 2) / (double)totalFrequencyInPair;
                }
                seriesContrib.Points.AddXY(k, contribution);
            }

            chart.Series.Add(seriesContrib);

            return RenderChartToBitmap(chart, chartSize);
        }
    }

    /// <summary>
    /// 4. p-Değeri / Ki-Kare Değeri vs. Veri Miktarı Grafiği oluşturur (Blok bazlı yaklaşım).
    /// </summary>
    public static Bitmap GeneratePValueVsDataAmountChart(Bitmap sourceImage, ColorChannel channel, Size chartSize, int steps = 20, string title = "p-Değeri vs. Analiz Edilen Veri")
    {
        if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));
        if (steps <= 0) throw new ArgumentException("Adım sayısı pozitif olmalıdır.", nameof(steps));

        using (var chart = new Chart { Size = chartSize })
        {
            chart.Titles.Add(title + $" ({channel})");
            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Analiz Edilen Satır Oranı (%)";
            chartArea.AxisY.Title = "p-Değeri";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.Maximum = 1.1; // 1'in biraz üstü daha iyi görünür
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = 100;
            chart.ChartAreas.Add(chartArea);

            Series seriesPValue = new Series("p-Değeri") { ChartType = SeriesChartType.Line, BorderWidth = 2 };

            BitmapData sourceData = null;
            try
            {
                Rectangle fullRect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                sourceData = sourceImage.LockBits(fullRect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);

                for (int i = 1; i <= steps; i++)
                {
                    int currentHeight = (int)Math.Ceiling(sourceImage.Height * (double)i / steps);
                    if (currentHeight <= 0) continue;
                    currentHeight = Math.Min(currentHeight, sourceImage.Height); // Sınırı aşma

                    Rectangle currentRect = new Rectangle(0, 0, sourceImage.Width, currentHeight);

                    // Mevcut blok için frekansları hesapla
                    long[] currentFrequencies = CalculateFrequencies(sourceData, currentRect, channel);

                    // Ki-Kare sonucunu hesapla
                    ChiSquareResult currentResult = CalculateChiSquareInternal(currentFrequencies);

                    double percentage = (double)i / steps * 100.0;
                    // Geçerli p-değeri varsa ekle, yoksa belki 1.0 (veya önceki değer) eklenebilir
                    double pValueToAdd = double.IsNaN(currentResult.PValue) ? 1.0 : currentResult.PValue;

                    seriesPValue.Points.AddXY(percentage, pValueToAdd);
                }
            }
            finally
            {
                if (sourceData != null) sourceImage.UnlockBits(sourceData);
            }


            chart.Series.Add(seriesPValue);

            return RenderChartToBitmap(chart, chartSize);
        }
    }

    // 5. Ki-Kare Isı Haritası - Bu metot bir önceki cevapta zaten yazılmıştı.
    // O metodu buraya kopyalayıp yapıştırabilir veya mevcut yerinde bırakıp
    // bu class içinden çağırabilirsiniz. Tutarlılık için buraya ekleyelim:

    /// <summary>
    /// Verilen görüntü için Ki-Kare p-değerine dayalı bir ısı haritası oluşturur.
    /// </summary>
    /// <param name="sourceImage">Analiz edilecek Bitmap.</param>
    /// <param name="channel">Analiz edilecek renk kanalı.</param>
    /// <param name="blockSize">Analiz bloklarının piksel cinsinden boyutu (örn. 16).</param>
    /// <param name="colorMapFunc">p-değerini (0-1 aralığında) System.Drawing.Color'a çeviren fonksiyon. Null ise varsayılan kullanılır.</param>
    /// <returns>Oluşturulan ısı haritası Bitmap'i.</returns>
    public static Bitmap GenerateChiSquareHeatmap(
        Bitmap sourceImage,
        ColorChannel channel,
        int blockSize,
        Func<double, Color> colorMapFunc = null)
    {
        if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));
        if (blockSize <= 0) throw new ArgumentException("Blok boyutu pozitif olmalıdır.", nameof(blockSize));

        var colorMap = colorMapFunc ?? DefaultColorMap; // Varsayılan renk haritasını kullan
        int width = sourceImage.Width;
        int height = sourceImage.Height;
        Bitmap heatmapImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        Rectangle fullRect = new Rectangle(0, 0, width, height);
        BitmapData sourceData = null;
        BitmapData heatmapData = null;

        try
        {
            sourceData = sourceImage.LockBits(fullRect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            heatmapData = heatmapImage.LockBits(fullRect, ImageLockMode.WriteOnly, heatmapImage.PixelFormat);
            IntPtr heatmapScan0 = heatmapData.Scan0;
            int heatmapStride = heatmapData.Stride;
            int heatmapBytesPerPixel = 3;

            for (int blockY = 0; blockY < height; blockY += blockSize)
            {
                for (int blockX = 0; blockX < width; blockX += blockSize)
                {
                    Rectangle blockRect = new Rectangle(blockX, blockY, blockSize, blockSize);
                    // Kenarları taşan bloklar için hesaplama helper metot içinde kontrol ediliyor.

                    ChiSquareResult blockResult = CalculateChiSquareForBlock(sourceData, blockRect, channel);
                    Color blockColor = colorMap(blockResult.PValue);

                    unsafe
                    {
                        for (int y = blockY; y < blockY + blockSize && y < height; y++)
                        {
                            byte* heatmapRow = (byte*)heatmapScan0 + (y * heatmapStride);
                            for (int x = blockX; x < blockX + blockSize && x < width; x++)
                            {
                                byte* pixel = heatmapRow + (x * heatmapBytesPerPixel);
                                pixel[0] = blockColor.B;
                                pixel[1] = blockColor.G;
                                pixel[2] = blockColor.R;
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            if (sourceData != null) sourceImage.UnlockBits(sourceData);
            if (heatmapData != null) heatmapImage.UnlockBits(heatmapData);
        }
        return heatmapImage;
    }

    // Varsayılan Renk Haritası (Heatmap için, önceki cevapta vardı)
    public static Color DefaultColorMap(double pValue)
    {
        if (double.IsNaN(pValue)) return Color.Gray;
        if (pValue < 0.0) pValue = 0.0;
        if (pValue > 1.0) pValue = 1.0;
        if (pValue <= 0.05) { int green = (int)(255 * (pValue / 0.05)); return Color.FromArgb(255, green, 0); } // Red->Yellow
        else if (pValue <= 0.5) { double t = (pValue - 0.05) / (0.5 - 0.05); int red = 255 - (int)(255 * t); return Color.FromArgb(red, 255, 0); } // Yellow->Green
        else { double t = (pValue - 0.5) / (1.0 - 0.5); int green = 255 - (int)(255 * t); int blue = (int)(255 * t); return Color.FromArgb(0, green, blue); } // Green->Blue
    }

    // CalculateChiSquareForBlock metodu da buraya eklenmeli (önceki cevapta vardı)
    // ... (CalculateChiSquareForBlock metodunun kodu buraya gelecek) ...
    /// <summary>
    /// Belirtilen bir görüntü bloğu için Ki-Kare analizini gerçekleştirir.
    /// </summary>
    private static ChiSquareResult CalculateChiSquareForBlock(
        BitmapData sourceData,
        Rectangle blockRect,
        ColorChannel channel)
    {
        long[] frequencies = new long[256];
        int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceData.PixelFormat) / 8;
        IntPtr sourceScan0 = sourceData.Scan0;
        int sourceStride = sourceData.Stride;

        // Kanal ofsetini belirle
        int channelOffset = 0;
        switch (channel)
        {
            case ColorChannel.Blue: channelOffset = 0; break;
            case ColorChannel.Green: channelOffset = 1; break;
            case ColorChannel.Red: channelOffset = 2; break;
        }
        if (channelOffset >= sourceBytesPerPixel) // Güvenlik kontrolü
            return new ChiSquareResult { PValue = double.NaN }; // Geçersiz kanal


        unsafe
        {
            // Sadece belirtilen blok içindeki pikselleri işle
            for (int y = blockRect.Top; y < blockRect.Bottom; y++)
            {
                // Resmin sınırları dışına çıkmamayı garantile (özellikle kenar bloklar için)
                if (y >= sourceData.Height) continue;

                byte* sourceRow = (byte*)sourceScan0 + (y * sourceStride);

                for (int x = blockRect.Left; x < blockRect.Right; x++)
                {
                    if (x >= sourceData.Width) continue; // Genişlik sınırını kontrol et

                    byte value = sourceRow[x * sourceBytesPerPixel + channelOffset];
                    frequencies[value]++;
                }
            }
        }

        // Ki-Kare hesaplaması (PerformChiSquareAttack içindeki ile aynı mantık)
        double chiSquareSum = 0.0;
        int degreesOfFreedom = 0;
        for (int k = 0; k < 128; k++)
        {
            long obs2k = frequencies[2 * k];
            long obs2kPlus1 = frequencies[2 * k + 1];
            long totalFrequencyInPair = obs2k + obs2kPlus1;

            if (totalFrequencyInPair > 0)
            {
                chiSquareSum += Math.Pow(obs2k - obs2kPlus1, 2) / (double)totalFrequencyInPair;
                degreesOfFreedom++;
            }
        }

        // p-değerini hesapla
        double pValue = double.NaN;
        if (degreesOfFreedom > 0 && chiSquareSum >= 0) // Geçerli değerler varsa
        {
            try
            {
                // Math.Net sıralaması: dof, value
                pValue = 1.0 - ChiSquared.CDF(degreesOfFreedom, chiSquareSum);
            }
            catch { /* MathNet yoksa veya hata olursa NaN kalır */ }
        }

        return new ChiSquareResult
        {
            ChiSquareValue = chiSquareSum,
            DegreesOfFreedom = degreesOfFreedom,
            PValue = pValue
        };
    }


} // End of Cls_ChiSquareHelper