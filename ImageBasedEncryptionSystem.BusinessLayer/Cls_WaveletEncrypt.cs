using System;
using System.Drawing;
using System.IO;
using System.Text;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Wavelet dönüşümü ile görüntüye veri gizleme işlemlerini yapan sınıf
    /// </summary>
    public class Cls_WaveletEncrypt
    {
        // Görüntüye veri gizlemek için kullanılacak sabit HASH değeri
        // Bu veri gizlenmiş bir görüntüyü tanımlamak için kullanılacak
        private readonly byte[] DataSignature = { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        /// <summary>
        /// Görüntüye veri gizler
        /// </summary>
        /// <param name="plainText">Gizlenecek düz metin</param>
        /// <param name="image">Veri gizlenecek görüntü</param>
        /// <param name="password">Şifreleme için parola</param>
        /// <returns>Veri gizlenmiş görüntü</returns>
        public Bitmap HideData(string plainText, Bitmap image, string password)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletEncrypt] HideData (string, Bitmap, string) işlemi başlatıldı.");
            try
            {
                if (string.IsNullOrEmpty(plainText) || image == null || string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_TEXT_EMPTY);
                    throw new ArgumentException(Errors.ERROR_TEXT_EMPTY);
                }
                Cls_AesEncrypt aesEncrypt = new Cls_AesEncrypt();
                string encryptedText = aesEncrypt.Encrypt(plainText, password);
                Cls_RsaEncrypt rsaEncrypt = new Cls_RsaEncrypt();
                string encryptedKey = rsaEncrypt.Encrypt(password);
                var result = HideData(image, encryptedText, encryptedKey);
                System.Diagnostics.Debug.WriteLine(Success.ENCRYPT_DATA_HIDDEN);
                return result;
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] {Errors.ERROR_WAVELET_ENCODE}: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_WAVELET_ENCODE, ex.Message), ex);
            }
        }
        
        /// <summary>
        /// Görüntüye veri gizler (byte dizisi olarak)
        /// </summary>
        /// <param name="image">Veri gizlenecek görüntü</param>
        /// <param name="data">Gizlenecek veri</param>
        /// <returns>Veri gizlenmiş görüntü</returns>
        public Bitmap HideData(Bitmap image, byte[] data)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletEncrypt] HideData (Bitmap, byte[]) işlemi başlatıldı.");
            try
            {
                if (image == null || data == null || data.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_IMAGE_EMPTY);
                    throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
                }
                if (data.Length > (image.Width * image.Height))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_TOO_LARGE);
                    throw new ArgumentException(Errors.ERROR_DATA_TOO_LARGE);
                }
                Bitmap resultImage = new Bitmap(image.Width, image.Height);
                
                // Görüntü pikselleri üzerinde işlem yap
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        // Orijinal pikseli al
                        Color pixel = image.GetPixel(x, y);
                        
                        // İlk piksellere sabit HASH değerini gizle (veri gizleme imzası)
                        if (y == 0 && x < DataSignature.Length)
                        {
                            // R, G, B son bitlerini DataSignature ile değiştir
                            byte r = (byte)((pixel.R & 0xF0) | DataSignature[x]);
                            byte g = (byte)((pixel.G & 0xF0) | DataSignature[x]);
                            byte b = (byte)((pixel.B & 0xF0) | DataSignature[x]);
                            
                            resultImage.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                        }
                        // Veri boyutunu gizle (sonraki 4 piksel)
                        else if (y == 0 && x >= DataSignature.Length && x < DataSignature.Length + 4)
                        {
                            int index = x - DataSignature.Length;
                            byte sizeByte = (byte)((data.Length >> (index * 8)) & 0xFF);
                            
                            byte r = (byte)((pixel.R & 0xF0) | (sizeByte & 0x0F));
                            byte g = (byte)((pixel.G & 0xF0) | ((sizeByte >> 4) & 0x0F));
                            byte b = pixel.B;
                            
                            resultImage.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                        }
                        // Veriyi gizle
                        else if (x + y * image.Width - (DataSignature.Length + 4) < data.Length)
                        {
                            // Veri indeksi
                            int dataIndex = x + y * image.Width - (DataSignature.Length + 4);
                            
                            // Verinin son 2 bitini her renk kanalının son 2 bitine yerleştir
                            byte dataByte = data[dataIndex];
                            
                            byte r = (byte)((pixel.R & 0xFC) | ((dataByte >> 6) & 0x03));
                            byte g = (byte)((pixel.G & 0xFC) | ((dataByte >> 4) & 0x03));
                            byte b = (byte)((pixel.B & 0xFC) | ((dataByte >> 2) & 0x03));
                            
                            resultImage.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                        }
                        else
                        {
                            // Veri gizleme işlemi bittikten sonra kalan pikseller orijinal haliyle kopyalanır
                            resultImage.SetPixel(x, y, pixel);
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine(Success.ENCRYPT_DATA_HIDDEN);
                return resultImage;
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] {Errors.ERROR_WAVELET_ENCODE}: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_WAVELET_ENCODE, ex.Message), ex);
            }
        }
        
        /// <summary>
        /// Görüntüye şifreli veri gizler
        /// </summary>
        /// <param name="image">Görüntü</param>
        /// <param name="encryptedText">Şifrelenmiş metin</param>
        /// <param name="encryptedKey">Şifrelenmiş anahtar</param>
        /// <returns>Veri gizlenmiş görüntü</returns>
        public Bitmap HideData(Bitmap image, string encryptedText, string encryptedKey)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletEncrypt] HideData (Bitmap, string, string) işlemi başlatıldı.");
            try
            {
                if (image == null || string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(encryptedKey))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_IMAGE_EMPTY);
                    throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
                }
                string combinedData = $"{encryptedText}|{encryptedKey}";
                byte[] dataBytes = Encoding.UTF8.GetBytes(combinedData);
                var result = HideData(image, dataBytes);
                System.Diagnostics.Debug.WriteLine(Success.ENCRYPT_DATA_HIDDEN);
                return result;
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletEncrypt] {Errors.ERROR_WAVELET_ENCODE}: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_WAVELET_ENCODE, ex.Message), ex);
            }
        }
    }
}
