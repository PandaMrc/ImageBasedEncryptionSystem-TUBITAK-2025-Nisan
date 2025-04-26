using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using ImageBasedEncryptionSystem.TypeLayer;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Globalization;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// LSB yöntemi ile görüntüden veri çıkarmak için kullanılan sınıf
    /// </summary>
    public class Cls_LsbDecrypt
    {
        // İmza sabiti - Şifreli verinin başında bulunmalı
        private const string SIGNATURE = "IBES";

        /// <summary>
        /// Görüntüden gizli veriyi çıkarır (şifreli metin ve anahtar)
        /// </summary>
        /// <param name="image">Veri çıkarılacak resim</param>
        /// <param name="encryptedText">Çıkarılan şifreli metin</param>
        /// <param name="encryptedKey">Çıkarılan şifreli AES anahtarı</param>
        /// <returns>İşlem başarılı ise true</returns>
        public bool ExtractData(Bitmap image, out byte[] encryptedText, out byte[] encryptedKey)
        {
            encryptedText = null;
            encryptedKey = null;

            // Resim kontrolü
            if (image == null)
            {
                throw new ArgumentException(Errors.ERROR_IMAGE_EMPTY);
            }

            try
            {
                // 1. İmzayı kontrol et
                int signatureLength = SIGNATURE.Length;
                byte[] signatureBytes = ExtractBytesFromImage(image, 0, signatureLength);
                
                string extractedSignature = Encoding.UTF8.GetString(signatureBytes);
                if (extractedSignature != SIGNATURE)
                {
                    throw new Exception(Errors.ERROR_DATA_NO_HIDDEN);
                }
                
                // 2. Metin uzunluğunu oku
                int currentPosition = signatureLength;
                byte[] textLengthBytes = ExtractBytesFromImage(image, currentPosition, 4);
                int textLength = BitConverter.ToInt32(textLengthBytes, 0);
                
                // Uzunluk kontrolü
                if (textLength <= 0 || textLength > 1000000) // 1MB'den büyük veri olmamalı
                {
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                
                // 3. Anahtar uzunluğunu oku
                currentPosition += 4;
                byte[] keyLengthBytes = ExtractBytesFromImage(image, currentPosition, 4);
                int keyLength = BitConverter.ToInt32(keyLengthBytes, 0);
                
                // Uzunluk kontrolü
                if (keyLength <= 0 || keyLength > 10000) // 10KB'den büyük anahtar olmamalı
                {
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                
                // Toplam veri boyutunu hesapla ve resim kapasitesini kontrol et
                int totalDataLength = signatureLength + 4 + 4 + textLength + keyLength;
                int requiredBits = totalDataLength * 8;
                int availableBits = image.Width * image.Height * 3;
                
                if (requiredBits > availableBits)
                {
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                
                // 4. Metni çıkar
                currentPosition += 4;
                encryptedText = ExtractBytesFromImage(image, currentPosition, textLength);
                
                // 5. Anahtarı çıkar
                currentPosition += textLength;
                encryptedKey = ExtractBytesFromImage(image, currentPosition, keyLength);
                
                // Veri kontrolü
                if (encryptedText == null || encryptedText.Length == 0 || 
                    encryptedKey == null || encryptedKey.Length == 0)
                {
                    throw new Exception(Errors.ERROR_DATA_CORRUPTED);
                }
                
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception(Errors.ERROR_LSB_EXTRACT);
            }
            catch (Exception ex)
            {
                if (ex.Message == Errors.ERROR_DATA_NO_HIDDEN || ex.Message == Errors.ERROR_DATA_CORRUPTED)
                    throw;
                    
                throw new Exception(Errors.ERROR_DATA_EXTRACT_FAILED + ": " + ex.Message);
            }
        }
        
        /// <summary>
        /// Görüntüden belirtilen konumdan başlayarak byte dizisi çıkarır
        /// </summary>
        /// <param name="image">Veri çıkarılacak görüntü</param>
        /// <param name="startByteIndex">Başlangıç byte indeksi</param>
        /// <param name="length">Okunacak byte sayısı</param>
        /// <returns>Çıkarılan byte dizisi</returns>
        private byte[] ExtractBytesFromImage(Bitmap image, int startByteIndex, int length)
        {
            if (length <= 0)
                return new byte[0];
                
            byte[] result = new byte[length];
            int width = image.Width;
            int height = image.Height;
            
            // Her byte'ın ilk biti için başlangıç pozisyonunu hesapla
            int startBitPosition = startByteIndex * 8;
            
            // Her byte için
            for (int byteIndex = 0; byteIndex < length; byteIndex++)
            {
                byte extractedByte = 0;
                
                // Her bit için
                for (int bitIndex = 0; bitIndex < 8; bitIndex++)
                {
                    // Şu anki bit pozisyonunu hesapla
                    int currentBitPosition = startBitPosition + (byteIndex * 8) + bitIndex;
                    
                    // Piksel koordinatlarını hesapla
                    int x = (currentBitPosition / 3) % width;
                    int y = (currentBitPosition / 3) / width;
                    
                    // Sınırları kontrol et
                    if (y >= height)
                    {
                        throw new IndexOutOfRangeException(Errors.ERROR_LSB_EXTRACT);
                    }
                    
                    // Renk kanalını belirle (0:R, 1:G, 2:B)
                    int colorChannel = currentBitPosition % 3;
                    
                    // Pikseli al
                    Color pixel = image.GetPixel(x, y);
                    
                    // İlgili kanalın en düşük bitini al
                    int bit = 0;
                    
                    switch (colorChannel)
                    {
                        case 0: // Kırmızı
                            bit = pixel.R & 1;
                            break;
                        case 1: // Yeşil
                            bit = pixel.G & 1;
                            break;
                        case 2: // Mavi
                            bit = pixel.B & 1;
                            break;
                    }
                    
                    // Biti doğru pozisyona yerleştir
                    extractedByte |= (byte)(bit << bitIndex);
                }
                
                // Çıkarılan byte'ı sonuç dizisine ekle
                result[byteIndex] = extractedByte;
            }
            
            return result;
        }
        
        /// <summary>
        /// Belirtilen dosya yolundan resmi yükler
        /// </summary>
        /// <param name="filePath">Resim dosyası yolu</param>
        /// <returns>Yüklenen resim veya null</returns>
        public Bitmap LoadImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException(Errors.ERROR_IMAGE_REQUIRED);
            }
            
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(Errors.ERROR_FILE_NOT_FOUND);
                }
                
                // Dosya uzantısını kontrol et
                string extension = Path.GetExtension(filePath)?.ToLower();
                
                if (string.IsNullOrEmpty(extension) || (extension != ".png" && extension != ".bmp" && extension != ".jpg" && extension != ".jpeg"))
                {
                    throw new Exception(Errors.ERROR_FILE_UNSUPPORTED);
                }
                
                // Resmi yükle
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    return new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is FileNotFoundException || 
                    ex.Message == Errors.ERROR_FILE_UNSUPPORTED)
                    throw;
                
                if (ex is OutOfMemoryException)
                    throw new Exception(Errors.ERROR_IMAGE_DAMAGED);
                
                if (ex is IOException)
                    throw new Exception(Errors.ERROR_FILE_IN_USE);
                    
                throw new Exception(Errors.ERROR_FILE_OPEN);
            }
        }

        /// <summary>
        /// Görüntüden metin çıkarır ve şifresini çözer
        /// </summary>
        public bool ExtractAndDecryptText(Bitmap image, string password, out string decryptedText, out string errorMessage)
        {
            decryptedText = string.Empty;
            errorMessage = string.Empty;

            try
            {
                // 1. Görüntüden gizli veriyi çıkar
                if (!ExtractData(image, out byte[] encryptedTextBytes, out byte[] encryptedKeyBytes))
                {
                    errorMessage = Errors.ERROR_DATA_NO_HIDDEN;
                    return false;
                }

                // 2. RSA ile şifrelenmiş AES anahtarını çöz
                Cls_RsaDecrypt rsaDecryptor = new Cls_RsaDecrypt();
                byte[] aesKeyBytes = rsaDecryptor.DecryptData(encryptedKeyBytes, password, out errorMessage);
                
                if (aesKeyBytes == null || aesKeyBytes.Length == 0)
                {
                    errorMessage = Errors.ERROR_PASSWORD_WRONG + (string.IsNullOrEmpty(errorMessage) ? "" : " - " + errorMessage);
                    return false;
                }

                // 3. AES ile şifrelenmiş metni çöz
                Cls_AesDecrypt aesDecryptor = new Cls_AesDecrypt();
                byte[] decryptedTextBytes = aesDecryptor.DecryptData(encryptedTextBytes, aesKeyBytes, out errorMessage);
                
                if (decryptedTextBytes == null || decryptedTextBytes.Length == 0)
                {
                    errorMessage = Errors.ERROR_DATA_CORRUPTED + (string.IsNullOrEmpty(errorMessage) ? "" : " - " + errorMessage);
                    return false;
                }

                // 4. Metni UTF8'e dönüştür
                decryptedText = Encoding.UTF8.GetString(decryptedTextBytes);
                
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Errors.ERROR_PASSWORD_WRONG) || 
                    ex.Message.Contains(Errors.ERROR_AES_DECRYPT))
                {
                    errorMessage = ex.Message;
                }
                else
                {
                    errorMessage = Errors.ERROR_DATA_EXTRACT_FAILED + " - " + ex.Message;
                }
                return false;
            }
        }
    }
} 