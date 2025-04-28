using System;
using System.Drawing;
using System.IO;
using System.Text;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Wavelet dönüşümü ile görüntüden gizli veriyi çıkaran sınıf
    /// </summary>
    public class Cls_WaveletDecrypt
    {
        // Görüntüde veri gizleme imzası olarak kullanılan sabit HASH değeri
        private readonly byte[] DataSignature = { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        /// <summary>
        /// Görüntüden gizli veriyi çıkarır
        /// </summary>
        /// <param name="image">Veri gizlenmiş görüntü</param>
        /// <param name="password">Şifre çözme için parola</param>
        /// <returns>Çözülmüş orijinal metin</returns>
        public string ExtractData(Bitmap image, string password)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletDecrypt] ExtractData (Bitmap, string) işlemi başlatıldı.");
            try
            {
                if (image == null || string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_IMAGE_EMPTY);
                    throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
                }
                ExtractedData extractedData = ExtractEncryptedData(image);
                if (extractedData == null || string.IsNullOrEmpty(extractedData.EncryptedText) || string.IsNullOrEmpty(extractedData.EncryptedKey))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_NO_HIDDEN);
                    throw new Exception(Errors.ERROR_DATA_NO_HIDDEN);
                }
                Cls_RsaEncrypt rsaEncrypt = new Cls_RsaEncrypt();
                Cls_RsaDecrypt rsaDecrypt = new Cls_RsaDecrypt(rsaEncrypt);
                string decryptedAesKey = string.Empty;
                try
                {
                    decryptedAesKey = rsaDecrypt.Decrypt(extractedData.EncryptedKey);
                    System.Diagnostics.Debug.WriteLine(Success.DECRYPT_SUCCESS_RSA);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] {Errors.ERROR_RSA_DECRYPT}: {ex.Message}");
                    throw new Exception(Errors.ERROR_RSA_DECRYPT, ex);
                }
                if (decryptedAesKey != password)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_PASSWORD_WRONG);
                    throw new Exception(Errors.ERROR_PASSWORD_WRONG);
                }
                Cls_AesDecrypt aesDecrypt = new Cls_AesDecrypt();
                string decryptedText = aesDecrypt.Decrypt(extractedData.EncryptedText, password);
                if (string.IsNullOrEmpty(decryptedText))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_AES_DECRYPT);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(Success.DECRYPT_DATA_EXTRACTED);
                }
                return decryptedText;
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] {Errors.ERROR_WAVELET_DECODE}: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_WAVELET_DECODE, ex.Message), ex);
            }
        }
        
        /// <summary>
        /// Görüntüden byte dizisi olarak veri çıkarır
        /// </summary>
        /// <param name="image">Veri gizlenmiş görüntü</param>
        /// <returns>Çıkarılan veri (byte dizisi)</returns>
        public byte[] ExtractData(Bitmap image)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletDecrypt] ExtractData (Bitmap) işlemi başlatıldı.");
            try
            {
                if (image == null)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_IMAGE_EMPTY);
                    throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
                }
                for (int i = 0; i < DataSignature.Length; i++)
                {
                    Color pixel = image.GetPixel(i, 0);
                    byte r = (byte)(pixel.R & 0x0F);
                    byte g = (byte)(pixel.G & 0x0F);
                    byte b = (byte)(pixel.B & 0x0F);
                    if (r != DataSignature[i] || g != DataSignature[i] || b != DataSignature[i])
                    {
                        System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_NO_HIDDEN);
                        throw new Exception(Errors.ERROR_DATA_NO_HIDDEN);
                    }
                }
                int dataSize = 0;
                for (int i = 0; i < 4; i++)
                {
                    Color pixel = image.GetPixel(DataSignature.Length + i, 0);
                    byte lowNibble = (byte)(pixel.R & 0x0F);
                    byte highNibble = (byte)(pixel.G & 0x0F);
                    byte sizeByte = (byte)((highNibble << 4) | lowNibble);
                    dataSize |= (sizeByte << (i * 8));
                }
                if (dataSize <= 0 || dataSize > (image.Width * image.Height))
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_CORRUPTED);
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                byte[] data = new byte[dataSize];
                int dataIndex = 0;
                for (int y = 0; y < image.Height && dataIndex < dataSize; y++)
                {
                    for (int x = 0; x < image.Width && dataIndex < dataSize; x++)
                    {
                        if (y == 0 && x < DataSignature.Length + 4)
                            continue;
                        Color pixel = image.GetPixel(x, y);
                        byte r = (byte)((pixel.R & 0x03) << 6);
                        byte g = (byte)((pixel.G & 0x03) << 4);
                        byte b = (byte)((pixel.B & 0x03) << 2);
                        data[dataIndex] = (byte)(r | g | b);
                        dataIndex++;
                    }
                }
                System.Diagnostics.Debug.WriteLine(Success.DECRYPT_DATA_EXTRACTED);
                return data;
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] {Errors.ERROR_WAVELET_DECODE}: {ex.Message}");
                if (ex.Message.Contains(Errors.ERROR_DATA_NO_HIDDEN))
                {
                    throw new Exception(Errors.ERROR_DATA_NO_HIDDEN, ex);
                }
                throw new Exception(string.Format(Errors.ERROR_WAVELET_DECODE, ex.Message), ex);
            }
        }
        
        /// <summary>
        /// Görüntüden şifrelenmiş veriyi çıkarır
        /// </summary>
        /// <param name="image">Veri gizlenmiş görüntü</param>
        /// <returns>Şifrelenmiş metin ve anahtar</returns>
        public ExtractedData ExtractEncryptedData(Bitmap image)
        {
            System.Diagnostics.Debug.WriteLine("[WaveletDecrypt] ExtractEncryptedData işlemi başlatıldı.");
            try
            {
                if (image == null)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_IMAGE_EMPTY);
                    throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
                }
                byte[] data = this.ExtractData(image);
                if (data == null || data.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_EXTRACT_FAILED);
                    throw new Exception(Errors.ERROR_DATA_EXTRACT_FAILED);
                }
                string combinedData = Encoding.UTF8.GetString(data);
                string[] parts = combinedData.Split('|');
                if (parts.Length != 2)
                {
                    System.Diagnostics.Debug.WriteLine(Errors.ERROR_DATA_CORRUPTED);
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                System.Diagnostics.Debug.WriteLine(Success.DECRYPT_DATA_EXTRACTED);
                return new ExtractedData
                {
                    EncryptedText = parts[0],
                    EncryptedKey = parts[1]
                };
            }
            catch (ArgumentException argEx)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] Hata: {argEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WaveletDecrypt] {Errors.ERROR_WAVELET_DECODE}: {ex.Message}");
                throw new Exception(string.Format(Errors.ERROR_WAVELET_DECODE, ex.Message), ex);
            }
        }
    }
}
