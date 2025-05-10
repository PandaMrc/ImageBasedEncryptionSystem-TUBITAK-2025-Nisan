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
        /// Bir görüntüden LSB yöntemi ile gizlenmiş veriyi çıkarır.
        /// </summary>
        public static byte[] ExtractData(Bitmap image, int maxBytes = 0)
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

                        // Maksimum byte sayısı kontrolü
                        if (maxBytes > 0 && extractedBytes.Count >= maxBytes)
                        {
                            Console.WriteLine($"Maksimum veri boyutu aşıldı: {maxBytes} bayt. Veri çıkarma işlemi sonlandırılıyor.");
                            endMarkerFound = true;
                            break;
                        }

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
            Console.WriteLine(Debug.DEBUG_LSB_EMBED_SIGNATURE_STARTED);

            // Sabit bir imza değeri
            byte[] signature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 }; // "ZORLU" kelimesinin ASCII değerleri
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_ADDED_SIGNATURE, BitConverter.ToString(signature)));
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EMBED_SIGNATURE_INPUT, image.Width, image.Height, signature.Length));

            Bitmap newImage = new Bitmap(image);
            int dataIndex = 0;
            int dataBitIndex = 0;
            int processedPixelCount = 0;

            for (int y = 0; y < newImage.Height && dataIndex < signature.Length; y++)
            {
                for (int x = 0; x < newImage.Width && dataIndex < signature.Length; x++)
                {
                    if (x + y * newImage.Width >= 20) // İlk 20 pikseli kontrol et
                        break;

                    processedPixelCount++;
                    Color pixel = newImage.GetPixel(x, y);
                    byte r = pixel.R;
                    byte g = pixel.G;
                    byte b = pixel.B;

                    // Her bir renk bileşenine veri biti yerleştir
                    int bit = GetBit(signature[dataIndex], dataBitIndex);
                    r = EmbedBit(r, bit);
                    dataBitIndex++;

                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                    {
                        bit = GetBit(signature[dataIndex], dataBitIndex);
                        g = EmbedBit(g, bit);
                        dataBitIndex++;
                    }
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                    {
                        bit = GetBit(signature[dataIndex], dataBitIndex);
                        b = EmbedBit(b, bit);
                        dataBitIndex++;
                    }
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    Color newPixel = Color.FromArgb(r, g, b);
                    newImage.SetPixel(x, y, newPixel);
                }
            }

            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EMBED_SIGNATURE_COMPLETED_COUNT, processedPixelCount));
            Console.WriteLine(Debug.DEBUG_LSB_EMBED_SIGNATURE_COMPLETED);
            return newImage;
        }

        public static bool CheckSignature(Bitmap image)
        {
            Console.WriteLine(Debug.DEBUG_LSB_CHECK_SIGNATURE_STARTED);
            byte[] signature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 };
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_CHECK_SIGNATURE_INPUT, image.Width, image.Height));

            byte[] extractedData = new byte[signature.Length];
            int dataIndex = 0;
            int dataBitIndex = 0;

            for (int y = 0; y < image.Height && dataIndex < signature.Length; y++)
            {
                for (int x = 0; x < image.Width && dataIndex < signature.Length; x++)
                {
                    if (x + y * image.Width >= 20) // İlk 20 pikseli kontrol et
                        break;

                    Color pixel = image.GetPixel(x, y);

                    // R bileşeninden bit çıkar
                    int extractedBit = pixel.R & 1;
                    extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | extractedBit);
                    dataBitIndex++;

                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                    {
                        // G bileşeninden bit çıkar
                        extractedBit = pixel.G & 1;
                        extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | extractedBit);
                        dataBitIndex++;
                        if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }
                    }

                    if (dataIndex < signature.Length)
                    {
                        // B bileşeninden bit çıkar
                        extractedBit = pixel.B & 1;
                        extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | extractedBit);
                        dataBitIndex++;
                        if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }
                    }
                }
            }

            // İmzanın başlangıçta olup olmadığını kontrol et
            bool signatureValid = true;
            for (int i = 0; i < signature.Length; i++)
            {
                if (extractedData[i] != signature[i])
                {
                    signatureValid = false;
                    break;
                }
            }

            Console.WriteLine(string.Format(Debug.DEBUG_LSB_CHECK_SIGNATURE_COMPARISON,
                BitConverter.ToString(signature),
                BitConverter.ToString(extractedData),
                signatureValid ? "Eşleşti" : "Eşleşmedi"));

            if (!signatureValid)
            {
                Console.WriteLine(string.Format(Debug.DEBUG_LSB_SIGNATURE_NOT_FOUND,
                    BitConverter.ToString(signature), BitConverter.ToString(extractedData)));
                Console.WriteLine(Debug.DEBUG_LSB_CHECK_SIGNATURE_COMPLETED);
                return false;
            }

            Console.WriteLine(Debug.DEBUG_LSB_SIGNATURE_VERIFIED);
            Console.WriteLine(Debug.DEBUG_LSB_CHECK_SIGNATURE_COMPLETED);
            return true;
        }
    }
} // End of Cls_LsbHelper
 // End of namespace