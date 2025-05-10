using System;
using System.Drawing;
using System.Drawing.Imaging; // PixelFormat, BitmapData için
using System.Runtime.InteropServices; // Gerekli değilse de LockBits'te bazen dolaylı kullanılır

namespace ImageBasedEncryptionSystem.BusinessLayer.Helpers // Kendi namespace'inize göre ayarlayın
{
    public static class Cls_LsbChangeMap
    {
        /// <summary>
        /// Bir görüntünün belirtilen renk kanalındaki LSB düzlemini çıkarır.
        /// Sonuç, LSB'lerin 0 (siyah) veya 1 (beyaz) olduğu bir Bitmap'tir.
        /// Bu metot, LSB Değişim Haritası için ve orijinal LSB düzlemini göstermek için kullanılabilir.
        /// </summary>
        /// <param name="sourceImage">Kaynak Bitmap.</param>
        /// <param name="channel">Analiz edilecek renk kanalı.</param>
        /// <returns>LSB düzlemini temsil eden siyah-beyaz bir Bitmap.</returns>
        public static Bitmap ExtractLsbPlane(Bitmap sourceImage, ColorChannel channel)
        {
            if (sourceImage == null) throw new ArgumentNullException(nameof(sourceImage));

            Bitmap lsbPlaneImage = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            BitmapData sourceBmpData = null;
            BitmapData lsbBmpData = null;

            try
            {
                sourceBmpData = sourceImage.LockBits(rect, ImageLockMode.ReadOnly, sourceImage.PixelFormat);
                lsbBmpData = lsbPlaneImage.LockBits(rect, ImageLockMode.WriteOnly, lsbPlaneImage.PixelFormat);

                IntPtr sourcePtr = sourceBmpData.Scan0;
                IntPtr lsbPtr = lsbBmpData.Scan0;
                int sourceStride = sourceBmpData.Stride;
                int lsbStride = lsbBmpData.Stride;
                int bytesPerPixelSource = Image.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
                int bytesPerPixelLsb = 3; // Format24bppRgb için

                if (bytesPerPixelSource < 3 && sourceImage.PixelFormat != PixelFormat.Format8bppIndexed)
                    throw new ArgumentException("LSB düzlemi çıkarma için en az 24bpp renkli veya 8bpp gri tonlama resim gereklidir.", nameof(sourceImage));

                int channelOffset = 0;
                bool isGrayscale = sourceImage.PixelFormat == PixelFormat.Format8bppIndexed;

                if (!isGrayscale)
                {
                    switch (channel)
                    {
                        case ColorChannel.Red: channelOffset = 2; break; // BGR sırası için Kırmızı=2
                        case ColorChannel.Green: channelOffset = 1; break;
                        case ColorChannel.Blue: channelOffset = 0; break;
                    }
                    if (channelOffset >= bytesPerPixelSource)
                        throw new ArgumentException($"Seçilen kanal '{channel}' bu resim formatında ({sourceImage.PixelFormat}) bulunmuyor.");
                }

                unsafe
                {
                    for (int y = 0; y < sourceImage.Height; y++)
                    {
                        byte* currentRowSource = (byte*)sourcePtr + (y * sourceStride);
                        byte* currentRowLsb = (byte*)lsbPtr + (y * lsbStride);

                        for (int x = 0; x < sourceImage.Width; x++)
                        {
                            byte sourcePixelValue;
                            if (isGrayscale)
                            {
                                sourcePixelValue = currentRowSource[x];
                            }
                            else
                            {
                                sourcePixelValue = currentRowSource[x * bytesPerPixelSource + channelOffset];
                            }

                            int lsbBit = sourcePixelValue & 1; // LSB'yi al (0 veya 1)
                            byte colorVal = (byte)(lsbBit * 255); // 0->Siyah, 1->Beyaz

                            currentRowLsb[x * bytesPerPixelLsb] = colorVal;     // Blue
                            currentRowLsb[x * bytesPerPixelLsb + 1] = colorVal; // Green
                            currentRowLsb[x * bytesPerPixelLsb + 2] = colorVal; // Red
                        }
                    }
                }
            }
            finally
            {
                if (sourceBmpData != null) sourceImage.UnlockBits(sourceBmpData);
                if (lsbBmpData != null) lsbPlaneImage.UnlockBits(lsbBmpData);
            }
            return lsbPlaneImage;
        }

        /// <summary>
        /// İki görüntünün belirtilen renk kanalındaki LSB düzlemlerini karşılaştırarak
        /// bir LSB Değişim Haritası oluşturur.
        /// Farklı LSB'ye sahip pikseller beyaz, aynı LSB'ye sahip pikseller siyah olarak işaretlenir.
        /// </summary>
        /// <param name="image1">Karşılaştırılacak ilk Bitmap (genellikle orijinal).</param>
        /// <param name="image2">Karşılaştırılacak ikinci Bitmap (genellikle stego).</param>
        /// <param name="channel">Karşılaştırma yapılacak renk kanalı.</param>
        /// <returns>LSB Değişim Haritasını temsil eden bir Bitmap.</returns>
        /// <exception cref="ArgumentNullException">Giriş resimleri null ise.</exception>
        /// <exception cref="ArgumentException">Resim boyutları farklıysa.</exception>
        public static Bitmap GenerateLsbChangeMap(Bitmap image1, Bitmap image2, ColorChannel channel)
        {
            if (image1 == null) throw new ArgumentNullException(nameof(image1));
            if (image2 == null) throw new ArgumentNullException(nameof(image2));

            if (image1.Width != image2.Width || image1.Height != image2.Height)
            {
                throw new ArgumentException("LSB Değişim Haritası oluşturmak için resim boyutları aynı olmalıdır.");
            }

            // İki resmin de LSB düzlemlerini çıkar
            // Not: ExtractLsbPlane metodu IDisposable nesneler üretmediği için using'e gerek yok.
            // Ancak, eğer ExtractLsbPlane içinde oluşturulan Bitmap'ler (lsbPlaneImage) dispose edilmiyorsa,
            // bu metotun dışında bir yerde bu Bitmap'lerin yönetilmesi gerekebilir.
            // Bu örnekte, ExtractLsbPlane bir Bitmap döndürüyor ve bu Bitmap'ler burada kullanılıp sonra unutuluyor.
            // Bellek yönetimi için, bu yardımcı Bitmap'leri de using içine almak daha güvenli olabilir.
            using (Bitmap lsbPlane1 = ExtractLsbPlane(image1, channel))
            using (Bitmap lsbPlane2 = ExtractLsbPlane(image2, channel))
            {
                Bitmap changeMap = new Bitmap(image1.Width, image1.Height, PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, image1.Width, image1.Height);
                BitmapData lsb1Data = null;
                BitmapData lsb2Data = null;
                BitmapData changeMapData = null;

                try
                {
                    lsb1Data = lsbPlane1.LockBits(rect, ImageLockMode.ReadOnly, lsbPlane1.PixelFormat);
                    lsb2Data = lsbPlane2.LockBits(rect, ImageLockMode.ReadOnly, lsbPlane2.PixelFormat);
                    changeMapData = changeMap.LockBits(rect, ImageLockMode.WriteOnly, changeMap.PixelFormat);

                    IntPtr ptr1 = lsb1Data.Scan0;
                    IntPtr ptr2 = lsb2Data.Scan0;
                    IntPtr ptrChange = changeMapData.Scan0;

                    int stride = lsb1Data.Stride; // Stride'lar aynı olmalı
                    int bytesPerPixel = 3;        // LSB düzlemleri 24bppRgb olarak oluşturuldu

                    unsafe
                    {
                        for (int y = 0; y < image1.Height; y++)
                        {
                            byte* row1 = (byte*)ptr1 + (y * stride);
                            byte* row2 = (byte*)ptr2 + (y * stride);
                            byte* rowChange = (byte*)ptrChange + (y * stride);

                            for (int x = 0; x < image1.Width; x++)
                            {
                                // LSB düzlemleri siyah (0) veya beyaz (255) olduğu için
                                // ilk byte'ı (Blue kanalı temsili) karşılaştırmak yeterli.
                                bool changed = row1[x * bytesPerPixel] != row2[x * bytesPerPixel];
                                byte colorVal = changed ? (byte)255 : (byte)0; // Değişmişse Beyaz, değişmemişse Siyah

                                rowChange[x * bytesPerPixel] = colorVal;
                                rowChange[x * bytesPerPixel + 1] = colorVal;
                                rowChange[x * bytesPerPixel + 2] = colorVal;
                            }
                        }
                    }
                }
                finally
                {
                    if (lsb1Data != null) lsbPlane1.UnlockBits(lsb1Data);
                    if (lsb2Data != null) lsbPlane2.UnlockBits(lsb2Data);
                    if (changeMapData != null) changeMap.UnlockBits(changeMapData);
                }
                return changeMap;
            } // using lsbPlane1, lsbPlane2 (burada dispose edilirler)
        }
    }
}
