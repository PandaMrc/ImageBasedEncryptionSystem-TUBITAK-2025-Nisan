using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageBasedEncryptionSystem.BusinessLayer.Helpers
{
    public static class LsbHelper
    {
        public static Bitmap EmbedData(Bitmap image, byte[] data)
        {
            // Bitiş işareti: Verinin sonunda olduğunu anlamak için
            byte[] endMarker = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }; // 8 adet 0

            // Asıl veriye son işareti eklenmiş hali
            byte[] fullData = new byte[data.Length + endMarker.Length];
            Buffer.BlockCopy(data, 0, fullData, 0, data.Length);
            Buffer.BlockCopy(endMarker, 0, fullData, data.Length, endMarker.Length);

            int dataIndex = 0;
            int dataBitIndex = 0;

            Bitmap newImage = new Bitmap(image);

            for (int y = 0; y < newImage.Height; y++)
            {
                for (int x = 0; x < newImage.Width; x++)
                {
                    if (x + y * newImage.Width < 20) // İlk 20 pikseli atla
                        continue;

                    if (dataIndex >= fullData.Length)
                        return newImage; // veri tamamen gömüldü

                    Color pixel = newImage.GetPixel(x, y);
                    byte r = pixel.R;
                    byte g = pixel.G;
                    byte b = pixel.B;

                    // Her bir renk bileşenine veri biti yerleştir
                    r = EmbedBit(r, GetBit(fullData[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < fullData.Length)
                        g = EmbedBit(g, GetBit(fullData[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < fullData.Length)
                        b = EmbedBit(b, GetBit(fullData[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    Color newPixel = Color.FromArgb(r, g, b);
                    newImage.SetPixel(x, y, newPixel);
                }
            }

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
            var data = new List<byte>();
            int dataBitIndex = 0;
            byte currentByte = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (x + y * image.Width < 20) // İlk 20 pikseli atla
                        continue;

                    Color pixel = image.GetPixel(x, y);
                    currentByte = (byte)((currentByte << 1) | (pixel.R & 1));
                    dataBitIndex++;
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }

                    currentByte = (byte)((currentByte << 1) | (pixel.G & 1));
                    dataBitIndex++;
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }

                    currentByte = (byte)((currentByte << 1) | (pixel.B & 1));
                    dataBitIndex++;
                    if (dataBitIndex == 8)
                    {
                        data.Add(currentByte);
                        currentByte = 0;
                        dataBitIndex = 0;
                    }
                }
            }

            // Verinin bittiği yeri bulmak için: 8 tane 0 byte'ı arıyoruz
            int endIndex = FindEndMarker(data);
            if (endIndex == -1)
                return data.ToArray(); // Marker yoksa hepsini döndür

            return data.Take(endIndex).ToArray(); // Sadece gerçek veriyi döndür
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
            Console.WriteLine("İmza ekleme işlemi başlatıldı.");
            // Sabit bir imza değeri
            byte[] signature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 }; // "ZORLU" kelimesinin ASCII değerleri
            Console.WriteLine("Eklenen İmza: {0}", BitConverter.ToString(signature));

            Bitmap newImage = new Bitmap(image);
            int dataIndex = 0;
            int dataBitIndex = 0;

            for (int y = 0; y < newImage.Height && dataIndex < signature.Length; y++)
            {
                for (int x = 0; x < newImage.Width && dataIndex < signature.Length; x++)
                {
                    if (x + y * newImage.Width >= 20) // İlk 20 pikseli kontrol et
                        break;

                    Color pixel = newImage.GetPixel(x, y);
                    byte r = pixel.R;
                    byte g = pixel.G;
                    byte b = pixel.B;

                    // Her bir renk bileşenine veri biti yerleştir
                    r = EmbedBit(r, GetBit(signature[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                        g = EmbedBit(g, GetBit(signature[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                        b = EmbedBit(b, GetBit(signature[dataIndex], dataBitIndex++));
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    Color newPixel = Color.FromArgb(r, g, b);
                    newImage.SetPixel(x, y, newPixel);
                }
            }

            Console.WriteLine("İmza başarıyla eklendi.");
            return newImage;
        }

        public static bool CheckSignature(Bitmap image)
        {
            Console.WriteLine("İmza kontrol işlemi başlatıldı.");
            byte[] signature = new byte[] { 0x5A, 0x4F, 0x52, 0x4C, 0x55 };
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
                    extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | (pixel.R & 1));
                    dataBitIndex++;
                    if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }

                    if (dataIndex < signature.Length)
                    {
                        extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | (pixel.G & 1));
                        dataBitIndex++;
                        if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }
                    }

                    if (dataIndex < signature.Length)
                    {
                        extractedData[dataIndex] = (byte)((extractedData[dataIndex] << 1) | (pixel.B & 1));
                        dataBitIndex++;
                        if (dataBitIndex == 8) { dataBitIndex = 0; dataIndex++; }
                    }
                }
            }

            // İmzanın başlangıçta olup olmadığını kontrol et
            for (int i = 0; i < signature.Length; i++)
            {
                if (extractedData[i] != signature[i])
                {
                    Console.WriteLine("İmza bulunamadı. Beklenen: {0}, Bulunan: {1}", BitConverter.ToString(signature), BitConverter.ToString(extractedData));
                    return false;
                }
            }

            Console.WriteLine("İmza doğrulandı.");
            return true;
        }

    }
}



