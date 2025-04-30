using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Imaging.Filters;
using System.Drawing;
using System.Drawing.Imaging;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer.Helpers
{
    public static class Cls_WaveletHelper
    {
        public static void EmbedTextInImage(string text, Bitmap image)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_EMBED_TEXT_STARTED, text, image.Width, image.Height));
                // Wavelet Transform uygulayarak metni göm
                // Bu örnek, metni gömmek için basit bir LSB (Least Significant Bit) tekniği kullanır
                // Daha karmaşık bir wavelet tabanlı algoritma için Accord.NET kütüphanesi kullanılabilir

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    for (int j = 0; j < 8; j++)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_EMBED_TEXT_PROCESSING, c, (c >> j) & 1, i, j));
                        int bit = (c >> j) & 1;
                        Color pixel = image.GetPixel(i, j);
                        int r = (pixel.R & ~1) | bit;
                        Color newPixel = Color.FromArgb(r, pixel.G, pixel.B);
                        image.SetPixel(i, j, newPixel);
                    }
                }
                Console.WriteLine(Debug.DEBUG_WAVELET_EMBED_TEXT_PROCESSED);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_WAVELET_EMBED_TEXT_PROCESS_FAILED, ex.Message));
                throw;
            }
        }

        public static string ExtractTextFromImage(Bitmap image)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_EXTRACT_TEXT_STARTED, image.Width, image.Height));
                // Görselden metni çıkart
                StringBuilder extractedText = new StringBuilder();

                for (int i = 0; i < image.Width; i++)
                {
                    char c = '\0';
                    for (int j = 0; j < 8; j++)
                    {
                        Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_EXTRACT_TEXT_PROCESSING, i, j, (image.GetPixel(i, j).R & 1)));
                        int bit = image.GetPixel(i, j).R & 1;
                        c |= (char)(bit << j);
                    }
                    extractedText.Append(c);
                }

                Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_EXTRACT_TEXT_PROCESSED, extractedText.ToString()));
                return extractedText.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_WAVELET_EXTRACT_TEXT_PROCESS_FAILED, ex.Message));
                throw;
            }
        }

        public static Bitmap EnsureTransparency(Bitmap image)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_WAVELET_TRANSPARENCY_STARTED, image.Width, image.Height));
                // PNG formatındaki verilerde arka planın transparan kalmasını sağla
                Bitmap transparentImage = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(transparentImage))
                {
                    Console.WriteLine(Debug.DEBUG_WAVELET_TRANSPARENCY_PROCESSING);
                    g.Clear(Color.Transparent);
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                }

                Console.WriteLine(Debug.DEBUG_WAVELET_TRANSPARENCY_PROCESSED);
                return transparentImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_WAVELET_TRANSPARENCY_PROCESS_FAILED, ex.Message));
                throw;
            }
        }

        public static void AddHashToImage(Bitmap image)
        {
            try
            {
                Console.WriteLine(Debug.DEBUG_HASH_ADD_STARTED);
                // Basit bir hash ekleme işlemi
                string hash = "ImageBasedEncryptionSystemHash";
                for (int i = 0; i < hash.Length; i++)
                {
                    char c = hash[i];
                    for (int j = 0; j < 8; j++)
                    {
                        int bit = (c >> j) & 1;
                        Color pixel = image.GetPixel(i, j);
                        int r = (pixel.R & ~1) | bit;
                        Color newPixel = Color.FromArgb(r, pixel.G, pixel.B);
                        image.SetPixel(i, j, newPixel);
                    }
                }
                Console.WriteLine(Debug.DEBUG_HASH_ADD_COMPLETED);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_HASH_ADD_FAILED, ex.Message));
                throw;
            }
        }

        public static bool CheckHash(Bitmap image)
        {
            // Basit bir hash kontrolü
            string hash = "ImageBasedEncryptionSystemHash";
            for (int i = 0; i < hash.Length; i++)
            {
                char c = hash[i];
                for (int j = 0; j < 8; j++)
                {
                    int bit = (c >> j) & 1;
                    Color pixel = image.GetPixel(i, j);
                    if ((pixel.R & 1) != bit)
                        return false;
                }
            }
            return true;
        }

        public static string ExtractAesEncryptedText(Bitmap image)
        {
            // Metni çıkartma işlemi
            return ExtractTextFromImage(image);
        }

        public static string ExtractDataFromImage(Bitmap image)
        {
            // Veriyi çıkartma işlemi
            return ExtractTextFromImage(image);
        }

        public static string ExtractRsaEncryptedKey(Bitmap image)
        {
            // RSA şifreli anahtarı çıkartma işlemi
            return ExtractTextFromImage(image);
        }
    }
}
