using System;
using System.Collections.Generic; // List için
using System.Drawing;
using System.Drawing.Imaging; // BitmapData, PixelFormat için
using System.Linq;            // Min(), Max(), Average() gibi Linq metotları için (bazı yardımcılarda kalmış olabilir)
using System.Runtime.InteropServices; // Marshal için
using System.Windows.Forms.DataVisualization.Charting; // Chart nesnesi için

/*
// ChiSquareResult struct (Cls_ChiSquareHelper bunu doğrudan kullanmaz ama başka yerde lazım olabilir)
public struct ChiSquareResult
{
    public double ChiSquareValue { get; set; }
    public int DegreesOfFreedom { get; set; }
    public double PValue { get; set; }
    // ... ToString() metodu ...
}
*/


/// <summary>
/// R/S Analizi ve ilgili görselleştirmeler için yardımcı metotlar içerir.
/// </summary>
public static class Cls_RsHelper
{
    // --- Özel Yardımcı Metotlar ---

    /// <summary>
    /// Verilen veri dizisinin belirtilen uzunluktaki bölümünün standart sapmasını hesaplar.
    /// </summary>
    private static double CalculateStandardDeviation(double[] data, int length)
    {
        if (length <= 1) return 0; // Tek eleman veya boşsa std sapma 0'dır

        // Ortalama hesaplama
        double sum = 0;
        for (int i = 0; i < length; i++) sum += data[i];
        double mean = sum / length;

        // Karelerin toplamı
        double sumOfSquares = 0;
        for (int i = 0; i < length; i++) sumOfSquares += Math.Pow(data[i] - mean, 2);

        // Örneklem standart sapması (N-1 ile bölme)
        return Math.Sqrt(sumOfSquares / (length - 1));
    }

    /// <summary>
    /// Log(n) ve Log(R/S) değerleri arasındaki lineer regresyon ile Hurst üssünü (eğimi) hesaplar.
    /// </summary>
    private static double CalculateHurstExponent(List<double> logN, List<double> logRs)
    {
        int count = logN.Count;
        // Lineer regresyon için en az 2 nokta gerekir
        if (count < 2) return double.NaN;

        // Ortalamaları hesapla (Linq kullanmadan)
        double xMean = 0, yMean = 0;
        for (int i = 0; i < count; i++) { xMean += logN[i]; yMean += logRs[i]; }
        xMean /= count;
        yMean /= count;

        // Eğim (slope) formülü için pay ve paydayı hesapla
        double numerator = 0;   // Sum of (x_i - x_mean) * (y_i - y_mean)
        double denominator = 0; // Sum of (x_i - x_mean)^2

        for (int i = 0; i < count; i++)
        {
            numerator += (logN[i] - xMean) * (logRs[i] - yMean);
            denominator += Math.Pow(logN[i] - xMean, 2);
        }

        // Sıfıra bölme hatasını kontrol et
        if (Math.Abs(denominator) < 1e-10) return double.NaN;

        // Eğim = Hurst Üssü
        return numerator / denominator;
    }


    // --- Ana R/S Analiz Metodu ---

    /// <summary>
    /// Optimize edilmiş R/S Analizi metodu (Non-Overlapping Alt Seriler Kullanarak).
    /// Girdi olarak piksel yoğunluklarını içeren bir double dizisi alır.
    /// </summary>
    /// <param name="data">Analiz edilecek piksel yoğunluk değerleri.</param>
    /// <param name="minSubSeriesLength">Analize dahil edilecek minimum alt seri uzunluğu (2'nin kuvveti olması önerilir, örn. 8).</param>
    /// <returns>Hurst üssü ve her alt seri uzunluğu (n) için ortalama R/S değerlerini içeren tuple.</returns>
    public static (double hurstExponent, List<(int n, double rs)> rsValues) RSAnalysis(double[] data, int minSubSeriesLength = 8)
    {
        // Girdi verisinin yeterliliğini kontrol et
        if (data == null || data.Length < minSubSeriesLength * 2)
        {
            Console.WriteLine("RS Analizi için yetersiz veri sağlandı.");
            return (double.NaN, new List<(int n, double rs)>()); // Boş liste ve NaN Hurst döndür
        }

        List<(int n, double rs)> rsValues = new List<(int n, double rs)>(); // Sonuçları (n, ortalama R/S) tutacak liste
        List<double> logNs = new List<double>();           // log(n) değerlerini tutacak liste (Hurst için)
        List<double> logRsList = new List<double>();       // log(ortalama R/S) değerlerini tutacak liste (Hurst için)
        int dataLength = data.Length;

        // Farklı alt-seri uzunlukları (k = n) üzerinde döngü (2'nin kuvvetleri)
        for (int k = minSubSeriesLength; k <= dataLength / 2; k *= 2)
        {
            List<double> currentRsValues = new List<double>(); // Bu 'k' değeri için hesaplanan tüm R/S değerleri

            // Non-overlapping (üst üste binmeyen) alt seriler üzerinde döngü
            // i, her alt serinin başlangıç indeksidir ve k kadar artar
            for (int i = 0; i <= dataLength - k; i += k)
            {
                // Alt seriyi al (Array.Copy daha performanslı olabilir)
                double[] subSeries = new double[k];
                Array.Copy(data, i, subSeries, 0, k);

                // Ortalama hesapla
                double mean = 0;
                for (int j = 0; j < k; j++) mean += subSeries[j];
                mean /= k;

                // Standart sapma hesapla
                double standardDeviation = CalculateStandardDeviation(subSeries, k);

                // Standart sapma çok küçükse (veya sıfırsa) bu alt seriyi atla
                if (Math.Abs(standardDeviation) < 1e-10) continue;

                // Kümülatif sapmaları ve aralığı (Range) hesapla
                double sumOfDeviations = 0;
                double maxDeviation = double.MinValue;
                double minDeviation = double.MaxValue;
                for (int j = 0; j < k; j++)
                {
                    sumOfDeviations += (subSeries[j] - mean);
                    if (sumOfDeviations > maxDeviation) maxDeviation = sumOfDeviations;
                    if (sumOfDeviations < minDeviation) minDeviation = sumOfDeviations;
                }
                double range = maxDeviation - minDeviation;

                // R/S değerini hesapla
                double rs = range / standardDeviation;

                // Geçerli bir R/S değeri ise listeye ekle
                if (!double.IsNaN(rs) && !double.IsInfinity(rs))
                {
                    currentRsValues.Add(rs);
                }
            } // i döngüsü (alt seriler) sonu

            // Bu 'k' uzunluğu için ortalama R/S değerini hesapla (eğer varsa)
            if (currentRsValues.Count > 0)
            {
                double sumRs = 0;
                foreach (var rsVal in currentRsValues) sumRs += rsVal;
                double averageRs = sumRs / currentRsValues.Count;

                // Ortalama R/S değeri logaritma alınabilirse listelere ekle
                if (averageRs > 1e-10) // Log(0) veya Log(negatif) hatasını önle
                {
                    logNs.Add(Math.Log(k));
                    logRsList.Add(Math.Log(averageRs));
                    // Görselleştirme için n ve ortalama R/S değerini ana listeye ekle
                    rsValues.Add((k, averageRs));
                }
            }
        } // k döngüsü (alt seri uzunlukları) sonu

        // Lineer regresyon ile Hurst üssünü hesapla
        double hurstExponent = CalculateHurstExponent(logNs, logRsList);

        // Hesaplanan Hurst üssünü ve görselleştirme için ortalama R/S değerlerini döndür
        return (hurstExponent, rsValues);
    }


    // --- Görselleştirme Metodu ---

    /// <summary>
    /// R/S analiz sonuçlarını (ortalama R/S değerleri ve Hurst trendi)
    /// log-log ölçekli bir grafik olarak görselleştirir.
    /// </summary>
    /// <param name="avgRsValues">Her alt seri uzunluğu (n) için ortalama R/S değerlerini içeren liste.</param>
    /// <param name="hurstExponent">Hesaplanan Hurst üssü değeri.</param>
    /// <returns>Oluşturulan Chart nesnesi.</returns>
    public static Chart VisualizeRSAnalysis(List<(int n, double rs)> avgRsValues, double hurstExponent)
    {
        // Yeni bir Chart nesnesi oluştur
        Chart chart = new Chart();
        chart.Legends.Add(new Legend("DefaultLegend")); // Lejant ekle

        // Bir ChartArea ekle ve eksenleri ayarla
        ChartArea chartArea = new ChartArea("RSAnalysisArea");
        chartArea.AxisX.IsLogarithmic = true; // X ekseni logaritmik
        chartArea.AxisY.IsLogarithmic = true; // Y ekseni logaritmik
        chartArea.AxisX.Title = "Alt-Seri Uzunluğu (n) - Log Skala";
        chartArea.AxisY.Title = "Ortalama R/S - Log Skala";
        chartArea.AxisX.LabelStyle.Format = "N0"; // X ekseni etiketlerini tam sayı yap
        chartArea.AxisY.LabelStyle.Format = "G3"; // Y ekseni etiket formatı
        chartArea.AxisX.Minimum = Double.NaN; // Min/Max değerlerini otomatik ayarla
        chartArea.AxisY.Minimum = Double.NaN;
        // Izgara çizgilerini açabiliriz (opsiyonel)
        // chartArea.AxisX.MajorGrid.Enabled = true;
        // chartArea.AxisY.MajorGrid.Enabled = true;
        chart.ChartAreas.Add(chartArea);

        // Ortalama R/S değerleri için bir seri oluştur (nokta gösterimi)
        Series seriesRS = new Series("Ortalama R/S")
        {
            ChartType = SeriesChartType.Point,
            MarkerStyle = MarkerStyle.Circle,
            MarkerSize = 7,
            Color = Color.SteelBlue,
            ToolTip = "n=#VALX, Ort. R/S=#VALY{G3}" // Üzerine gelince bilgi göster
        };

        // Hurst trend çizgisi için bir seri oluştur
        Series seriesHurst = new Series("Hurst Trend (H=" + (double.IsNaN(hurstExponent) ? "N/A" : hurstExponent.ToString("F2")) + ")")
        {
            ChartType = SeriesChartType.Line,
            Color = Color.FromArgb(220, Color.Red), // Biraz transparan Kırmızı
            BorderWidth = 2
        };

        // Veri noktalarını R/S serisine ekle
        bool hasValidData = false;
        if (avgRsValues != null)
        {
            foreach (var rsValue in avgRsValues)
            {
                // Sadece logaritması alınabilen pozitif değerleri ekle
                if (rsValue.n > 0 && rsValue.rs > 0)
                {
                    seriesRS.Points.AddXY(rsValue.n, rsValue.rs);
                    hasValidData = true;
                }
            }
        }

        // Eğer geçerli veri varsa ve Hurst üssü hesaplanabildiyse trend çizgisini çiz
        if (hasValidData && !double.IsNaN(hurstExponent))
        {
            try
            {
                // Grafikteki min/max n değerlerini bul
                double minN = avgRsValues.Where(v => v.n > 0).Min(v => v.n);
                double maxN = avgRsValues.Max(v => v.n);

                if (minN > 0 && maxN > minN)
                {
                    // İlk veri noktasından geçen H eğimli çizgiyi hesapla
                    // log(y) = log(c) + H * log(n) => log(c) = log(y) - H * log(n)
                    var firstValidPoint = avgRsValues.First(v => v.n > 0 && v.rs > 0);
                    double logC = Math.Log(firstValidPoint.rs) - hurstExponent * Math.Log(firstValidPoint.n);

                    // Çizginin başlangıç ve bitiş Y değerlerini hesapla
                    double startY = Math.Exp(logC + hurstExponent * Math.Log(minN));
                    double endY = Math.Exp(logC + hurstExponent * Math.Log(maxN));

                    // Çizgiyi seriye ekle
                    seriesHurst.Points.AddXY(minN, startY);
                    seriesHurst.Points.AddXY(maxN, endY);
                }
            }
            catch (Exception ex)
            {
                // Min/Max veya logaritma hatası olabilir, trend çizilemez.
                Console.WriteLine($"Hurst trend çizgisi çizilirken hata: {ex.Message}");
            }
        }

        // Serileri grafiğe ekle
        chart.Series.Add(seriesRS);
        // Sadece içinde nokta varsa Hurst serisini ve Lejantı ekle
        if (seriesHurst.Points.Count > 1)
        {
            chart.Series.Add(seriesHurst);
        }
        else
        {
            // Hurst hesaplanamadıysa veya çizilemediyse, seri başlığını kaldırabiliriz
            chart.Legends["DefaultLegend"].Enabled = false;
        }


        // Chart nesnesini döndür (Bunu Bitmap'e çizdirmek çağıran kodun sorumluluğu)
        return chart;
    }

} // End of Cls_RsHelper