using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging; // PixelFormat, BitmapData için
using System.Linq;
using System.Runtime.InteropServices; // Marshal için
using System.Text;
using ImageBasedEncryptionSystem.TypeLayer; // Debug sabitleri için

namespace ImageBasedEncryptionSystem.BusinessLayer.Helpers
{
    public static class Cls_LsbHelper
    {
        private const int SIGNATURE_PIXEL_COUNT = 20; // İmza için kullanılacak piksel sayısı

        // --- Bit İşleme Yardımcı Metotları ---

        /// <summary>
        /// Verilen byte'ın belirtilen bit indeksindeki değeri (0 veya 1) alır.
        /// (Index 0 en anlamlı bit - MSB, Index 7 en anlamsız bit - LSB)
        /// </summary>
        private static int GetBit(byte b, int bitIndex)
        {
            // bitIndex 0'dan 7'ye kadar olmalı
            if (bitIndex < 0 || bitIndex > 7) throw new ArgumentOutOfRangeException(nameof(bitIndex));
            // Biti en sağa kaydır ve diğer bitleri maskele
            return (b >> (7 - bitIndex)) & 1;
        }

        /// <summary>
        /// Bir renk bileşeninin en anlamsız bitine (LSB) verilen biti (0 veya 1) gömer.
        /// </summary>
        private static byte EmbedBit(byte colorComponent, int bit)
        {
            if (bit != 0 && bit != 1) throw new ArgumentException("Gömülecek bit 0 veya 1 olmalıdır.", nameof(bit));
            // Son biti temizle (AND ...11111110)
            colorComponent = (byte)(colorComponent & 0xFE);
            // Yeni biti ekle (OR 0000000(0 veya 1))
            return (byte)(colorComponent | bit);
        }

        // --- Ana LSB Metotları (Optimize Edilmiş) ---

        /// <summary>
        /// Verilen byte dizisini görüntünün LSB'lerine gömer (LockBits ile hızlandırılmış).
        /// İlk SIGNATURE_PIXEL_COUNT pikseli atlar.
        /// Verinin sonuna 8 adet 0 byte'ı ekler.
        /// </summary>
        public static Bitmap EmbedData(Bitmap image, byte[] data)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (data == null) throw new ArgumentNullException(nameof(data));

            Console.WriteLine(Debug.DEBUG_LSB_EMBED_DATA_STARTED);
            Console.WriteLine($"Input: Image({image.Width}x{image.Height}), Data Length({data.Length})");

            // Bitiş işareti ve asıl veri birleştirilir
            byte[] endMarker = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }; // 8 adet 0
            byte[] fullData = new byte[data.Length + endMarker.Length];
            Buffer.BlockCopy(data, 0, fullData, 0, data.Length);
            Buffer.BlockCopy(endMarker, 0, fullData, data.Length, endMarker.Length);
            int totalBitsToEmbed = fullData.Length * 8;
            Console.WriteLine($"Data prepared: Original({data.Length}), With Marker({fullData.Length}), Total Bits({totalBitsToEmbed})");

            // Görüntünün kapasitesini kontrol et (RGB kanalları kullanıldığı için piksel * 3 bit)
            long capacity = ((long)image.Width * image.Height - SIGNATURE_PIXEL_COUNT) * 3;
            if (totalBitsToEmbed > capacity)
            {
                throw new ArgumentException($"Veri boyutu ({totalBitsToEmbed} bit) resim kapasitesini ({capacity} bit) aşıyor.");
            }

            // Orijinal görüntünün kopyası üzerinde çalış
            Bitmap newImage = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData bmpData = null;

            try
            {
                // Bitmap'i kilitle (ReadWrite çünkü değiştireceğiz)
                bmpData = newImage.LockBits(rect, ImageLockMode.ReadWrite, newImage.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;
                // Sadece 24bpp ve 32bpp desteklenir varsayımı (en az RGB gerekli)
                if (bytesPerPixel < 3) throw new NotSupportedException("LSB gömme için en az 24bpp görüntü gereklidir.");

                int totalBytes = Math.Abs(stride) * newImage.Height;
                byte[] pixels = new byte[totalBytes];
                Marshal.Copy(ptr, pixels, 0, totalBytes); // Piksel verisini diziye kopyala

                int currentBitIndex = 0; // Gömülecek toplam bit indeksi
                int pixelIndex = 0;      // İşlenen piksel sayısı

                // Pikseller üzerinde dolaş
                for (int y = 0; y < newImage.Height; y++)
                {
                    for (int x = 0; x < newImage.Width; x++)
                    {
                        pixelIndex = y * newImage.Width + x;

                        // İlk SIGNATURE_PIXEL_COUNT pikseli atla
                        if (pixelIndex < SIGNATURE_PIXEL_COUNT) continue;

                        // Gömülecek bit kalmadıysa döngüden çık
                        if (currentBitIndex >= totalBitsToEmbed) break;

                        // Geçerli pikselin bellek adresini hesapla
                        int lineStartIndex = y * Math.Abs(stride);
                        int pixelStartIndex = lineStartIndex + x * bytesPerPixel;

                        // RGB bileşenlerine bitleri göm
                        // R kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int dataByteIndex = currentBitIndex / 8;
                            int dataBitIndexInByte = currentBitIndex % 8;
                            int bitToEmbed = GetBit(fullData[dataByteIndex], dataBitIndexInByte);
                            pixels[pixelStartIndex + 2] = EmbedBit(pixels[pixelStartIndex + 2], bitToEmbed); // Offset 2: Red
                            currentBitIndex++;
                        }
                        // G kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int dataByteIndex = currentBitIndex / 8;
                            int dataBitIndexInByte = currentBitIndex % 8;
                            int bitToEmbed = GetBit(fullData[dataByteIndex], dataBitIndexInByte);
                            pixels[pixelStartIndex + 1] = EmbedBit(pixels[pixelStartIndex + 1], bitToEmbed); // Offset 1: Green
                            currentBitIndex++;
                        }
                        // B kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int dataByteIndex = currentBitIndex / 8;
                            int dataBitIndexInByte = currentBitIndex % 8;
                            int bitToEmbed = GetBit(fullData[dataByteIndex], dataBitIndexInByte);
                            pixels[pixelStartIndex + 0] = EmbedBit(pixels[pixelStartIndex + 0], bitToEmbed); // Offset 0: Blue
                            currentBitIndex++;
                        }

                        // İlerleme raporu (isteğe bağlı, performansı biraz düşürebilir)
                        if ((pixelIndex - SIGNATURE_PIXEL_COUNT + 1) % 100000 == 0)
                        {
                            Console.WriteLine($"Embed Progress: Processed Pixels({pixelIndex - SIGNATURE_PIXEL_COUNT + 1}), Bits Embedded({currentBitIndex}/{totalBitsToEmbed})");
                        }
                    }
                    if (currentBitIndex >= totalBitsToEmbed) break; // Dış döngüden de çık
                }

                // Değiştirilmiş piksel verisini Bitmap'e geri kopyala
                Marshal.Copy(pixels, 0, ptr, totalBytes);
                Console.WriteLine($"Embed completed: Total Bits Embedded({currentBitIndex})");
            }
            finally
            {
                // Kilidi kaldır
                if (bmpData != null)
                {
                    newImage.UnlockBits(bmpData);
                }
            }

            Console.WriteLine(Debug.DEBUG_LSB_EMBED_DATA_COMPLETED);
            return newImage;
        }

        /// <summary>
        /// Görüntünün LSB'lerinden veri çıkarır (LockBits ile hızlandırılmış).
        /// İlk SIGNATURE_PIXEL_COUNT pikseli atlar.
        /// 8 adet sıfır byte'ı bulunca durur.
        /// </summary>
        public static byte[] ExtractData(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            Console.WriteLine(Debug.DEBUG_LSB_EXTRACT_DATA_STARTED);
            Console.WriteLine($"Input: Image({image.Width}x{image.Height})");

            List<byte> extractedBytes = new List<byte>();
            byte currentByte = 0;
            int bitCount = 0;
            bool endMarkerFound = false;
            int processedPixelCount = 0;

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = null;

            try
            {
                // Bitmap'i kilitle (ReadOnly)
                bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;
                if (bytesPerPixel < 3) throw new NotSupportedException("LSB çıkarma için en az 24bpp görüntü gereklidir.");

                int totalBytes = Math.Abs(stride) * image.Height;
                byte[] pixels = new byte[totalBytes];
                Marshal.Copy(ptr, pixels, 0, totalBytes); // Piksel verisini diziye kopyala

                int pixelIndex = 0;

                // Pikseller üzerinde dolaş
                for (int y = 0; y < image.Height && !endMarkerFound; y++)
                {
                    for (int x = 0; x < image.Width && !endMarkerFound; x++)
                    {
                        pixelIndex = y * image.Width + x;

                        // İlk SIGNATURE_PIXEL_COUNT pikseli atla
                        if (pixelIndex < SIGNATURE_PIXEL_COUNT) continue;

                        processedPixelCount++;

                        // Geçerli pikselin bellek adresini hesapla
                        int lineStartIndex = y * Math.Abs(stride);
                        int pixelStartIndex = lineStartIndex + x * bytesPerPixel;

                        // RGB bileşenlerinden bitleri çıkar
                        // R kanalı
                        int bitR = pixels[pixelStartIndex + 2] & 1; // Offset 2: Red
                        currentByte = (byte)((currentByte << 1) | bitR);
                        bitCount++;
                        if (bitCount == 8) { extractedBytes.Add(currentByte); currentByte = 0; bitCount = 0; if (CheckForEndMarker(extractedBytes)) { endMarkerFound = true; break; } }

                        // G kanalı
                        int bitG = pixels[pixelStartIndex + 1] & 1; // Offset 1: Green
                        currentByte = (byte)((currentByte << 1) | bitG);
                        bitCount++;
                        if (bitCount == 8) { extractedBytes.Add(currentByte); currentByte = 0; bitCount = 0; if (CheckForEndMarker(extractedBytes)) { endMarkerFound = true; break; } }

                        // B kanalı
                        int bitB = pixels[pixelStartIndex + 0] & 1; // Offset 0: Blue
                        currentByte = (byte)((currentByte << 1) | bitB);
                        bitCount++;
                        if (bitCount == 8) { extractedBytes.Add(currentByte); currentByte = 0; bitCount = 0; if (CheckForEndMarker(extractedBytes)) { endMarkerFound = true; break; } }

                        // İlerleme raporu (isteğe bağlı)
                        if (processedPixelCount % 100000 == 0)
                        {
                            Console.WriteLine($"Extract Progress: Processed Pixels({processedPixelCount}), Bytes Found({extractedBytes.Count})");
                        }
                    }
                }
                Console.WriteLine($"Extract completed: Total Pixels Processed({processedPixelCount}), Total Bytes Found({extractedBytes.Count})");

            }
            finally
            {
                // Kilidi kaldır
                if (bmpData != null)
                {
                    image.UnlockBits(bmpData);
                }
            }

            // Sonuçları işle
            if (endMarkerFound)
            {
                Console.WriteLine($"End marker found. Returning data before marker ({extractedBytes.Count - 8} bytes).");
                // Son 8 byte (marker) hariç veriyi al
                return extractedBytes.Take(extractedBytes.Count - 8).ToArray();
            }
            else
            {
                Console.WriteLine("End marker not found. Returning all extracted data.");
                return extractedBytes.ToArray(); // Marker yoksa hepsini döndür
            }
            // Not: Önceki FindEndMarker metodu burada doğrudan CheckForEndMarker ile değiştirildi.
        }

        /// <summary>
        /// Verilen byte listesinin son 8 byte'ının bitiş işaretçisi olup olmadığını kontrol eder.
        /// </summary>
        private static bool CheckForEndMarker(List<byte> data)
        {
            if (data.Count < 8) return false;
            // Son 8 byte'ın hepsi 0 mı?
            for (int i = data.Count - 8; i < data.Count; i++)
            {
                if (data[i] != 0) return false;
            }
            return true;
        }


        /// <summary>
        /// Görüntünün ilk SIGNATURE_PIXEL_COUNT pikseline sabit bir imza gömer (LockBits ile hızlandırılmış).
        /// </summary>
        public static Bitmap EmbedSignature(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            Console.WriteLine(Debug.DEBUG_LSB_EMBED_SIGNATURE_STARTED);
            byte[] signature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 }; // "ZORLU"
            int totalBitsToEmbed = signature.Length * 8;
            Console.WriteLine($"Embedding signature: {BitConverter.ToString(signature)} ({totalBitsToEmbed} bits)");


            // Gerekli piksel sayısını kontrol et
            if (SIGNATURE_PIXEL_COUNT * 3 < totalBitsToEmbed)
            {
                throw new InvalidOperationException($"İmza ({totalBitsToEmbed} bit) ayrılan piksel alanına ({SIGNATURE_PIXEL_COUNT * 3} bit) sığmıyor.");
            }

            Bitmap newImage = new Bitmap(image);
            Rectangle rect = new Rectangle(0, 0, newImage.Width, newImage.Height);
            BitmapData bmpData = null;

            try
            {
                bmpData = newImage.LockBits(rect, ImageLockMode.ReadWrite, newImage.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;
                if (bytesPerPixel < 3) throw new NotSupportedException("İmza gömme için en az 24bpp görüntü gereklidir.");

                int totalBytes = Math.Abs(stride) * newImage.Height;
                byte[] pixels = new byte[totalBytes];
                Marshal.Copy(ptr, pixels, 0, totalBytes);

                int currentBitIndex = 0;
                int pixelIndex = 0;

                // İlk SIGNATURE_PIXEL_COUNT pikseli işle
                for (int y = 0; y < newImage.Height && pixelIndex < SIGNATURE_PIXEL_COUNT; y++)
                {
                    for (int x = 0; x < newImage.Width && pixelIndex < SIGNATURE_PIXEL_COUNT; x++)
                    {
                        int lineStartIndex = y * Math.Abs(stride);
                        int pixelStartIndex = lineStartIndex + x * bytesPerPixel;

                        // R kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int bitToEmbed = GetBit(signature[currentBitIndex / 8], currentBitIndex % 8);
                            pixels[pixelStartIndex + 2] = EmbedBit(pixels[pixelStartIndex + 2], bitToEmbed); // Red
                            currentBitIndex++;
                        }
                        // G kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int bitToEmbed = GetBit(signature[currentBitIndex / 8], currentBitIndex % 8);
                            pixels[pixelStartIndex + 1] = EmbedBit(pixels[pixelStartIndex + 1], bitToEmbed); // Green
                            currentBitIndex++;
                        }
                        // B kanalı
                        if (currentBitIndex < totalBitsToEmbed)
                        {
                            int bitToEmbed = GetBit(signature[currentBitIndex / 8], currentBitIndex % 8);
                            pixels[pixelStartIndex + 0] = EmbedBit(pixels[pixelStartIndex + 0], bitToEmbed); // Blue
                            currentBitIndex++;
                        }

                        pixelIndex++; // İşlenen piksel sayısını artır
                        if (currentBitIndex >= totalBitsToEmbed) break; // İmza bittiyse çık
                    }
                    if (currentBitIndex >= totalBitsToEmbed) break; // İmza bittiyse çık
                }

                Marshal.Copy(pixels, 0, ptr, totalBytes); // Değişiklikleri geri yaz
                Console.WriteLine($"Signature embed completed. Pixels used: {pixelIndex}, Bits embedded: {currentBitIndex}");
            }
            finally
            {
                if (bmpData != null) newImage.UnlockBits(bmpData);
            }
            Console.WriteLine(Debug.DEBUG_LSB_EMBED_SIGNATURE_COMPLETED);
            return newImage;
        }

        /// <summary>
        /// Görüntünün başındaki imzayı kontrol eder (LockBits ile hızlandırılmış).
        /// </summary>
        public static bool CheckSignature(Bitmap image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            Console.WriteLine(Debug.DEBUG_LSB_CHECK_SIGNATURE_STARTED);
            byte[] expectedSignature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 }; // "ZORLU"
            int totalBitsToCheck = expectedSignature.Length * 8;
            byte[] extractedBytes = new byte[expectedSignature.Length];
            int currentBitIndex = 0;
            byte currentByte = 0;
            int pixelIndex = 0;

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = null;
            bool signatureValid = false;

            try
            {
                bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytesPerPixel = Image.GetPixelFormatSize(bmpData.PixelFormat) / 8;
                if (bytesPerPixel < 3) throw new NotSupportedException("İmza kontrolü için en az 24bpp görüntü gereklidir.");

                int totalBytes = Math.Abs(stride) * image.Height;
                byte[] pixels = new byte[totalBytes];
                Marshal.Copy(ptr, pixels, 0, totalBytes);

                // İlk SIGNATURE_PIXEL_COUNT pikseli işle
                for (int y = 0; y < image.Height && pixelIndex < SIGNATURE_PIXEL_COUNT && currentBitIndex < totalBitsToCheck; y++)
                {
                    for (int x = 0; x < image.Width && pixelIndex < SIGNATURE_PIXEL_COUNT && currentBitIndex < totalBitsToCheck; x++)
                    {
                        int lineStartIndex = y * Math.Abs(stride);
                        int pixelStartIndex = lineStartIndex + x * bytesPerPixel;

                        // R kanalı
                        if (currentBitIndex < totalBitsToCheck)
                        {
                            currentByte = (byte)((currentByte << 1) | (pixels[pixelStartIndex + 2] & 1));
                            currentBitIndex++;
                            if (currentBitIndex % 8 == 0) { extractedBytes[currentBitIndex / 8 - 1] = currentByte; currentByte = 0; }
                        }
                        // G kanalı
                        if (currentBitIndex < totalBitsToCheck)
                        {
                            currentByte = (byte)((currentByte << 1) | (pixels[pixelStartIndex + 1] & 1));
                            currentBitIndex++;
                            if (currentBitIndex % 8 == 0) { extractedBytes[currentBitIndex / 8 - 1] = currentByte; currentByte = 0; }
                        }
                        // B kanalı
                        if (currentBitIndex < totalBitsToCheck)
                        {
                            currentByte = (byte)((currentByte << 1) | (pixels[pixelStartIndex + 0] & 1));
                            currentBitIndex++;
                            if (currentBitIndex % 8 == 0) { extractedBytes[currentBitIndex / 8 - 1] = currentByte; currentByte = 0; }
                        }
                        pixelIndex++;
                    }
                }

                // Çıkarılan byte'ları beklenen imza ile karşılaştır
                if (currentBitIndex >= totalBitsToCheck) // Yeterli bit çıkarıldı mı?
                {
                    signatureValid = true; // Başlangıçta geçerli kabul et
                    for (int i = 0; i < expectedSignature.Length; i++)
                    {
                        if (extractedBytes[i] != expectedSignature[i])
                        {
                            signatureValid = false;
                            break;
                        }
                    }
                }
                else // Yeterli bit çıkarılamadı (resim çok küçük?)
                {
                    signatureValid = false;
                }

                Console.WriteLine($"Signature check: Expected({BitConverter.ToString(expectedSignature)}), Extracted({BitConverter.ToString(extractedBytes)}), Match({signatureValid})");

            }
            finally
            {
                if (bmpData != null) image.UnlockBits(bmpData);
            }

            if (signatureValid) Console.WriteLine(Debug.DEBUG_LSB_SIGNATURE_VERIFIED);
            else Console.WriteLine(Debug.DEBUG_LSB_SIGNATURE_NOT_FOUND);

            Console.WriteLine(Debug.DEBUG_LSB_CHECK_SIGNATURE_COMPLETED);
            return signatureValid;
        }

    } // End of Cls_LsbHelper
} // End of namespace