using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/// <summary>
/// Görüntü analizinde kullanılacak renk kanallarını tanımlar.
/// </summary>
public enum ColorChannel
{
    Red,
    Green,
    Blue
    // İstenirse Alpha kanalı da eklenebilir
    // Alpha 
}

public static class Cls_BitPlaneHelper
{
    /// <summary>
    /// Belirtilen bir görüntünün istenen renk kanalındaki belirli bir bit düzlemini çıkarır.
    /// </summary>
    /// <param name="sourceImage">Kaynak Bitmap nesnesi.</param>
    /// <param name="bitPlaneIndex">Çıkarılacak bit düzlemi (0=LSB, 7=MSB).</param>
    /// <param name="channel">Hangi renk kanalının (R, G, B) analiz edileceğini belirtir.</param>
    /// <returns>İstenen bit düzlemini gösteren siyah-beyaz bir Bitmap nesnesi (1=Beyaz, 0=Siyah) veya hata durumunda null.</returns>
    /// <exception cref="ArgumentNullException">sourceImage null ise fırlatılır.</exception>
    /// <exception cref="ArgumentOutOfRangeException">bitPlaneIndex 0-7 aralığı dışındaysa fırlatılır.</exception>
    public static Bitmap ExtractBitPlane(Bitmap sourceImage, int bitPlaneIndex, ColorChannel channel)
    {
        if (sourceImage == null)
        {
            throw new ArgumentNullException(nameof(sourceImage));
        }
        if (bitPlaneIndex < 0 || bitPlaneIndex > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(bitPlaneIndex), "Bit düzlemi indeksi 0 ile 7 arasında olmalıdır.");
        }

        // Hedef bitmap'i kaynakla aynı boyutta ve 24bpp Rgb formatında oluştur. 
        // (Siyah/Beyaz için 1bppIndexed de olabilirdi ancak 24bpp yönetimi daha kolay)
        Bitmap resultBitmap = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format24bppRgb);

        int width = sourceImage.Width;
        int height = sourceImage.Height;

        // Hızlı piksel erişimi için LockBits kullan
        Rectangle rect = new Rectangle(0, 0, width, height);
        BitmapData sourceData = null;
        BitmapData resultData = null;

        try
        {
            // Kaynak ve hedef bitmap'leri belleğe kilitle
            sourceData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat); // Kaynak formatını koru
            resultData = resultBitmap.LockBits(rect, ImageLockMode.WriteOnly, resultBitmap.PixelFormat); // Hedef Format24bppRgb

            // Bellek adreslerinin başlangıcını al
            IntPtr sourceScan0 = sourceData.Scan0;
            IntPtr resultScan0 = resultData.Scan0;

            // Stride (bir piksel satırının byte cinsinden uzunluğu)
            int sourceStride = sourceData.Stride;
            int resultStride = resultData.Stride;

            // Piksel başına düşen byte sayısı (kaynak formatına göre değişebilir)
            int sourceBytesPerPixel = Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            int resultBytesPerPixel = Image.GetPixelFormatSize(resultBitmap.PixelFormat) / 8; // Bu 3 olmalı (24bpp)

            // Kanal ofsetini belirle (BGRA veya BGR sırasına dikkat!)
            // .NET'te genellikle BGR veya BGRA sırası kullanılır.
            int channelOffset = 0;
            switch (channel)
            {
                case ColorChannel.Blue:
                    channelOffset = 0; // Genellikle ilk byte Mavi'dir
                    break;
                case ColorChannel.Green:
                    channelOffset = 1; // İkinci byte Yeşil
                    break;
                case ColorChannel.Red:
                    channelOffset = 2; // Üçüncü byte Kırmızı
                    break;
                    // Eğer Alpha işlenecekse ve format 32bpp ise:
                    // case ColorChannel.Alpha:
                    //     channelOffset = 3; 
                    //     if (sourceBytesPerPixel < 4) throw new InvalidOperationException("Alfa kanalı olmayan format.");
                    //     break;
            }
            
            // Hızlı ve güvensiz kod bloğu (Proje ayarlarından 'Allow unsafe code' işaretlenmeli)
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    // Geçerli satırın başlangıç işaretçilerini al
                    byte* sourceRow = (byte*)sourceScan0 + (y * sourceStride);
                    byte* resultRow = (byte*)resultScan0 + (y * resultStride);

                    for (int x = 0; x < width; x++)
                    {
                        // Kaynak pikselin ilgili kanal değerini al
                        byte channelValue = sourceRow[x * sourceBytesPerPixel + channelOffset];

                        // İstenen bit düzlemindeki biti çıkar (1 veya 0)
                        int bit = (channelValue >> bitPlaneIndex) & 1;

                        // Sonuç pikselin rengini belirle (1 ise Beyaz, 0 ise Siyah)
                        byte outputColor = (byte)(bit * 255); // 1*255=255 (Beyaz), 0*255=0 (Siyah)

                        // Sonuç bitmap'ine siyah/beyaz değeri yaz (BGR formatında)
                        resultRow[x * resultBytesPerPixel] = outputColor;     // Blue
                        resultRow[x * resultBytesPerPixel + 1] = outputColor; // Green
                        resultRow[x * resultBytesPerPixel + 2] = outputColor; // Red
                    }
                }
            }
        }
        finally
        {
            // Bitmap'leri bellekten serbest bırak (hata olsa bile)
            if (sourceData != null)
            {
                sourceImage.UnlockBits(sourceData);
            }
            if (resultData != null)
            {
                resultBitmap.UnlockBits(resultData);
            }
        }

        return resultBitmap;
    }
}