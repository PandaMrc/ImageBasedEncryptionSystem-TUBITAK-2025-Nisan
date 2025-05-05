using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/// <summary>
/// Histogram hesaplama modunu belirtir.
/// </summary>
public enum HistogramMode
{
    /// <summary>
    /// Kırmızı (Red) kanalı için histogram hesaplar.
    /// </summary>
    Red,
    /// <summary>
    /// Yeşil (Green) kanalı için histogram hesaplar.
    /// </summary>
    Green,
    /// <summary>
    /// Mavi (Blue) kanalı için histogram hesaplar.
    /// </summary>
    Blue,
    /// <summary>
    /// Gri tonlama (Luminosity - BT.709 standardı) için histogram hesaplar.
    /// </summary>
    Grayscale
}

public static partial class Cls_Histogram
{
    /// <summary>
    /// Bir görüntünün belirtilen modda (Renk kanalı veya Gri tonlama) 
    /// piksel yoğunluk histogramını hesaplar.
    /// </summary>
    /// <param name="sourceImage">Analiz edilecek Bitmap nesnesi.</param>
    /// <param name="mode">Hesaplanacak histogram modu (Red, Green, Blue, Grayscale).</param>
    /// <returns>0-255 arasındaki yoğunluk değerlerinin frekanslarını içeren 256 elemanlı bir long dizisi. Dizin değeri yoğunluğu, dizi değeri frekansı temsil eder.</returns>
    /// <exception cref="ArgumentNullException">sourceImage null ise fırlatılır.</exception>
    /// <exception cref="ArgumentException">Resim formatı uygun değilse veya kanal bulunamazsa fırlatılabilir.</exception>
    public static long[] CalculateHistogram(Bitmap sourceImage, HistogramMode mode)
    {
        if (sourceImage == null)
        {
            throw new ArgumentNullException(nameof(sourceImage));
        }

        // Histogram dizisi: index 0-255 -> frekans
        long[] histogram = new long[256]; // long kullanmak büyük resimlerde taşmayı önler

        int width = sourceImage.Width;
        int height = sourceImage.Height;
        Rectangle rect = new Rectangle(0, 0, width, height);
        BitmapData sourceData = null;

        try
        {
            // Veriye hızlı erişim için LockBits
            sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            IntPtr sourceScan0 = sourceData.Scan0;
            int sourceStride = sourceData.Stride;
            int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;

            // 24bpp (RGB) veya 32bpp (ARGB) formatları desteklenir
            if (sourceBytesPerPixel < 3)
            {
                throw new ArgumentException("Histogram hesaplaması için en az 24bpp formatında bir resim gereklidir.", nameof(sourceImage));
            }

            // Kanal ofsetleri (BGR sırası varsayılır)
            const int blueOffset = 0;
            const int greenOffset = 1;
            const int redOffset = 2;
            // Alpha ofseti (32bpp ise) = 3;

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* sourceRow = (byte*)sourceScan0 + (y * sourceStride);
                    for (int x = 0; x < width; x++)
                    {
                        // Pikselin başlangıç adresi
                        byte* pixel = sourceRow + (x * sourceBytesPerPixel);

                        byte intensity; // Histogramda artırılacak yoğunluk değeri

                        switch (mode)
                        {
                            case HistogramMode.Red:
                                intensity = pixel[redOffset]; // Kırmızı kanalı al
                                break;
                            case HistogramMode.Green:
                                intensity = pixel[greenOffset]; // Yeşil kanalı al
                                break;
                            case HistogramMode.Blue:
                                intensity = pixel[blueOffset]; // Mavi kanalı al
                                break;
                            case HistogramMode.Grayscale:
                                // Gri tonlama hesapla (Luminosity - ITU R-REC BT.709)
                                // Y = 0.2126 * R + 0.7152 * G + 0.0722 * B
                                double grayValue = 0.2126 * pixel[redOffset] +
                                                   0.7152 * pixel[greenOffset] +
                                                   0.0722 * pixel[blueOffset];

                                // Yuvarla ve 0-255 aralığında kalmasını sağla
                                intensity = (byte)Math.Max(0, Math.Min(255, Math.Round(grayValue)));
                                break;
                            default: // Normalde buraya düşmemeli
                                throw new ArgumentOutOfRangeException(nameof(mode), "Geçersiz histogram modu.");
                        }

                        // İlgili yoğunluk değerinin frekansını artır
                        histogram[intensity]++;
                    }
                }
            }
        }
        finally
        {
            if (sourceData != null)
            {
                sourceImage.UnlockBits(sourceData);
            }
        }

        return histogram;
    }

    public static Bitmap VisualizeHistogram(long[] histogram, Size chartSize, string title = "Histogram")
    {
        if (histogram == null || histogram.Length != 256)
        {
            throw new ArgumentException("Histogram verisi geçersiz.", nameof(histogram));
        }

        using (var chart = new System.Windows.Forms.DataVisualization.Charting.Chart { Size = chartSize })
        {
            chart.Titles.Add(title);
            var chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartArea.AxisX.Title = "Yoğunluk Değerleri";
            chartArea.AxisY.Title = "Frekans";
            chart.ChartAreas.Add(chartArea);

            var series = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column
            };

            for (int i = 0; i < histogram.Length; i++)
            {
                series.Points.AddXY(i, histogram[i]);
            }

            chart.Series.Add(series);

            Bitmap bitmap = new Bitmap(chartSize.Width, chartSize.Height);
            chart.DrawToBitmap(bitmap, new Rectangle(0, 0, chartSize.Width, chartSize.Height));
            return bitmap;
        }
    }
}