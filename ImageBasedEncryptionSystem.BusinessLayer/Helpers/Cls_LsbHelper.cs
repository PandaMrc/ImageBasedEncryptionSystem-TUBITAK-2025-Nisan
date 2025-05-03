using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Drawing.Imaging;

namespace ImageBasedEncryptionSystem.BusinessLayer.Helpers
{
    public static class Cls_LsbHelper
    {
        public static Bitmap EmbedData(Bitmap image, byte[] data)
        {
            Console.WriteLine(Debug.DEBUG_LSB_EMBED_DATA_STARTED);
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EMBED_DATA_INPUT, image.Width, image.Height, data.Length));

            // Bitiş işareti: Verinin sonunda olduğunu anlamak için
            byte[] endMarker = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }; // 8 adet 0
            
            // Asıl veriye son işareti eklenmiş hali
            byte[] fullData = new byte[data.Length + endMarker.Length];
            Buffer.BlockCopy(data, 0, fullData, 0, data.Length);
            Buffer.BlockCopy(endMarker, 0, fullData, data.Length, endMarker.Length);
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_FULL_DATA_PREPARED, data.Length, fullData.Length));

            int dataIndex = 0;
            int dataBitIndex = 0;
            int processedPixelCount = 0;

            Bitmap newImage = new Bitmap(image);

            for (int y = 0; y < newImage.Height; y++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    if (x + y * newImage.Width < 20) // İlk 20 pikseli atla
                        continue;

                    if (dataIndex >= fullData.Length)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_LSB_PROCESSED_PIXEL_COUNT, processedPixelCount));
                        Console.WriteLine(Debug.DEBUG_LSB_EMBED_DATA_COMPLETED);
                        return newImage; // veri tamamen gömüldü
                    }

                    processedPixelCount++;
                    Color pixel = newImage.GetPixel(x, y);
                    byte r = pixel.R;
                    byte g = pixel.G;
                    byte b = pixel.B;

                    // Her bir renk bileşenine veri biti yerleştir
                    int bit = GetBit(fullData[dataIndex], dataBitIndex);
                    r = EmbedBit(r, bit);
                    dataBitIndex++;
                    
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < fullData.Length)
                    {
                        bit = GetBit(fullData[dataIndex], dataBitIndex);
                        g = EmbedBit(g, bit);
                        dataBitIndex++;
                    }
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < fullData.Length)
                    {
                        bit = GetBit(fullData[dataIndex], dataBitIndex);
                        b = EmbedBit(b, bit);
                        dataBitIndex++;
                    }
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    Color newPixel = Color.FromArgb(r, g, b);
                    newImage.SetPixel(x, y, newPixel);
                    
                    // Her 1000 pikselde bir ilerleme raporu ver
                    if (processedPixelCount % 100000 == 0)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_LSB_EMBED_PROGRESS, 
                            processedPixelCount, dataIndex, fullData.Length));
                    }
                }
            }

            Console.WriteLine(string.Format(Debug.DEBUG_LSB_PROCESSED_PIXEL_COUNT, processedPixelCount));
            Console.WriteLine(Debug.DEBUG_LSB_EMBED_DATA_COMPLETED);
            return newImage;
        }

        private static byte EmbedBit(byte colorComponent, int bit)
        {
            // Son biti temizle, sonra yeni biti ekle
            colorComponent = (byte)(colorComponent & 0xFE); // ...11111110
            return (byte)(colorComponent | bit);
        }

        private static int GetBit(byte b, int bitIndex)
        {
            return (b >> (7 - bitIndex)) & 1;
        }

        ///

        public static byte[] ExtractData(Bitmap image)
        {
            Console.WriteLine(Debug.DEBUG_LSB_EXTRACT_DATA_STARTED);
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EXTRACT_DATA_INPUT, image.Width, image.Height));
            
            var data = new List<byte>();
            int dataBitIndex = 0;
            byte currentByte = 0;
            int processedPixelCount = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (x + y * image.Width < 20) // İlk 20 pikseli atla
                        continue;

                    processedPixelCount++;
                    Color pixel = image.GetPixel(x, y);
                    
                    // R bileşeninden bit çıkar
                    int extractedBit = pixel.R & 1;
                    currentByte = (byte)((currentByte << 1) | extractedBit);
                    dataBitIndex++;
                    
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }

                    // G bileşeninden bit çıkar
                    extractedBit = pixel.G & 1;
                    currentByte = (byte)((currentByte << 1) | extractedBit);
                    dataBitIndex++;
                    
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }

                    // B bileşeninden bit çıkar
                    extractedBit = pixel.B & 1;
                    currentByte = (byte)((currentByte << 1) | extractedBit);
                    dataBitIndex++;
                    
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }
                    
                    // Her 100000 pikselde bir ilerleme raporu ver
                    if (processedPixelCount % 100000 == 0)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_LSB_EXTRACT_PROGRESS, 
                            processedPixelCount, data.Count));
                    }
                }
            }

            // Verinin bittiği yeri bulmak için: 8 tane 0 byte'ı arıyoruz
            int endIndex = FindEndMarker(data);
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EXTRACT_TOTAL, 
                processedPixelCount, data.Count));
                
            if (endIndex == -1)
            {
                Console.WriteLine(Debug.DEBUG_LSB_END_MARKER_NOT_FOUND);
                Console.WriteLine(Debug.DEBUG_LSB_EXTRACT_DATA_COMPLETED);
                return data.ToArray(); // Marker yoksa hepsini döndür
            }
                
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_EXTRACT_DATA_END_MARKER, endIndex));
            byte[] result = data.Take(endIndex).ToArray();
            Console.WriteLine(string.Format(Debug.DEBUG_LSB_DATA_SIZE, result.Length));
            Console.WriteLine(Debug.DEBUG_LSB_EXTRACT_DATA_COMPLETED);
            
            return result; // Sadece gerçek veriyi döndür
        }

        private static int FindEndMarker(List<byte> data)
        {
            for (int i = 0; i < data.Count - 7; i++)
            {
                if (data.Skip(i).Take(8).All(b => b == 0))
                {
                    return i;
                }
            }
            return -1;
        }

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
}