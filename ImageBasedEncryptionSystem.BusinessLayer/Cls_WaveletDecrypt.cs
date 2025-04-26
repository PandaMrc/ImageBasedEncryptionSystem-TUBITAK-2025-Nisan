using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Accord.Math.Wavelets;
using Accord.Math;
using System.Runtime.InteropServices;
using ImageBasedEncryptionSystem.TypeLayer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_WaveletDecrypt
    {
        private const int defaultDecompositionLevel = 2;
        private string _lastErrorMessage = string.Empty;
        private string _lastSuccessMessage = string.Empty;
        private const int RSA_KEY_SIZE = 2048;

        /// <summary>
        /// Son hata mesajını döndürür
        /// </summary>
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
        }

        /// <summary>
        /// Son başarı mesajını döndürür
        /// </summary>
        public string LastSuccessMessage
        {
            get { return _lastSuccessMessage; }
        }

        /// <summary>
        /// Wavelet dönüşümü ile şifrelenmiş görüntüden metni çıkarır
        /// </summary>
        /// <param name="encryptedImage">Şifrelenmiş görüntü</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>Çıkarılan metin</returns>
        public string DecryptTextFromImage(Bitmap encryptedImage, string password)
        {
            try
            {
                // Şifre kontrolü
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Görüntü kontrolü
                if (encryptedImage == null || encryptedImage.Width < 300 || encryptedImage.Height < 300)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_TOO_SMALL;
                    return null;
                }

                // Wavelet dönüşümü uygula ve katsayıları al
                byte[] extractedData = ExtractDataFromWaveletCoefficients(encryptedImage, password);
                if (extractedData == null || extractedData.Length == 0)
                {
                    return null;
                }

                // AES şifresini çöz
                string decryptedText = DecryptStringWithAES(extractedData, password);
                if (decryptedText != null)
                {
                    _lastSuccessMessage = Success.WAVELET_DECRYPT_SUCCESS;
                    return decryptedText;
                }
                else
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_DECODE;
                    return null;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Bitmap görüntüsünü wavelet dönüşümüne tabi tutar
        /// </summary>
        /// <param name="inputImage">Kaynak görüntü</param>
        /// <param name="level">Dönüşüm seviyesi</param>
        /// <returns>Wavelet katsayıları (çift boyutlu double dizisi)</returns>
        private double[,] ApplyWaveletTransform(Bitmap inputImage, int level = defaultDecompositionLevel)
        {
            try
            {
                // Görüntüyü gri seviyeye dönüştür
                Bitmap grayImage = ConvertToGrayscale(inputImage);
                
                // Görüntü boyutlarını al
                int width = grayImage.Width;
                int height = grayImage.Height;
                
                // Wavelet dönüşümü için görüntüyü double dizisine dönüştür
                double[,] data = new double[height, width];
                
                // Görüntü verisini diziye aktar
                BitmapData bmpData = grayImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format8bppIndexed);
                
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            data[y, x] = ptr[y * bmpData.Stride + x];
                        }
                    }
                }
                
                grayImage.UnlockBits(bmpData);
                
                // Haar wavelet dönüşümü uygula
                HaarWavelet wavelet = new HaarWavelet();
                double[,] coefficients = data.Copy();
                
                // 2D Discrete Wavelet dönüşümü uygula
                for (int i = 0; i < level; i++)
                {
                    int currentWidth = width >> i;
                    int currentHeight = height >> i;
                    
                    // İlk olarak satırlara dönüşüm uygula
                    for (int y = 0; y < currentHeight; y++)
                    {
                        double[] row = new double[currentWidth];
                        for (int x = 0; x < currentWidth; x++)
                            row[x] = coefficients[y, x];
                            
                        // Wavelet dönüşümü uygula
                        wavelet.Forward(row);
                        
                        // Dönüştürülen değerleri geri yaz
                        for (int x = 0; x < currentWidth; x++)
                            coefficients[y, x] = row[x];
                    }
                    
                    // Sonra sütunlara dönüşüm uygula
                    for (int x = 0; x < currentWidth; x++)
                    {
                        double[] col = new double[currentHeight];
                        for (int y = 0; y < currentHeight; y++)
                            col[y] = coefficients[y, x];
                            
                        // Wavelet dönüşümü uygula
                        wavelet.Forward(col);
                        
                        // Dönüştürülen değerleri geri yaz
                        for (int y = 0; y < currentHeight; y++)
                            coefficients[y, x] = col[y];
                    }
                }
                
                _lastSuccessMessage = Success.WAVELET_TRANSFORM_SUCCESS;
                return coefficients;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_WAVELET_TRANSFORM, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Görüntüyü gri seviyeye dönüştürür
        /// </summary>
        /// <param name="source">Kaynak görüntü</param>
        /// <returns>Gri seviye görüntü</returns>
        private Bitmap ConvertToGrayscale(Bitmap source)
        {
            // 8bpp gri seviye bitmap oluştur
            Bitmap grayImage = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
            
            // Gri seviye renk paletini oluştur
            ColorPalette palette = grayImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            grayImage.Palette = palette;
            
            // Görüntüyü gri seviyeye dönüştür
            BitmapData sourceData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly,
                source.PixelFormat);
                
            BitmapData targetData = grayImage.LockBits(
                new Rectangle(0, 0, grayImage.Width, grayImage.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);
                
            unsafe
            {
                byte* sourcePtr = (byte*)sourceData.Scan0;
                byte* targetPtr = (byte*)targetData.Scan0;
                
                int sourcePixelSize = Image.GetPixelFormatSize(source.PixelFormat) / 8;
                
                for (int y = 0; y < source.Height; y++)
                {
                    byte* sourceLine = sourcePtr + (y * sourceData.Stride);
                    byte* targetLine = targetPtr + (y * targetData.Stride);
                    
                    for (int x = 0; x < source.Width; x++)
                    {
                        byte* sourcePixel = sourceLine + (x * sourcePixelSize);
                        
                        // RGB değerlerini al (formatına göre)
                        byte r, g, b;
                        if (sourcePixelSize == 4) // 32bpp ARGB
                        {
                            b = sourcePixel[0];
                            g = sourcePixel[1];
                            r = sourcePixel[2];
                        }
                        else if (sourcePixelSize == 3) // 24bpp RGB
                        {
                            b = sourcePixel[0];
                            g = sourcePixel[1];
                            r = sourcePixel[2];
                        }
                        else // Diğer formatlar
                        {
                            b = g = r = 0;
                        }
                        
                        // Gri seviye değerini hesapla (BT.601 formülü)
                        byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                        
                        // Gri seviye değerini hedef bitmap'e yaz
                        targetLine[x] = gray;
                    }
                }
            }
            
            source.UnlockBits(sourceData);
            grayImage.UnlockBits(targetData);
            
            return grayImage;
        }

        /// <summary>
        /// Wavelet katsayılarından veri çıkarır
        /// </summary>
        /// <param name="encryptedImage">Şifrelenmiş görüntü</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>Çıkarılan veri</returns>
        private byte[] ExtractDataFromWaveletCoefficients(Bitmap encryptedImage, string password)
        {
            try
            {
                // Wavelet dönüşümü uygula
                double[,] coefficients = ApplyWaveletTransform(encryptedImage);
                if (coefficients == null)
                {
                    return null;
                }
                
                int height = coefficients.GetLength(0);
                int width = coefficients.GetLength(1);
                
                // Parola hash değeri üret (şifreleme işlemindeki ile aynı olmalı)
                byte[] passwordHash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
                
                // Wavelet katsayılarının ortasından veri çıkarmaya başla
                int startX = (width >> (defaultDecompositionLevel + 1));
                int startY = (height >> (defaultDecompositionLevel + 1));
                int currentIndex = 0;
                
                // Önce veri uzunluğunu çıkar (ilk 4 bayt)
                byte[] dataLengthBytes = new byte[4];
                
                for (int i = 0; i < dataLengthBytes.Length; i++)
                {
                    byte currentByte = 0;
                    
                    for (int bit = 0; bit < 8; bit++)
                    {
                        int x = startX + (currentIndex % (width >> defaultDecompositionLevel));
                        int y = startY + (currentIndex / (width >> defaultDecompositionLevel));
                        
                        if (y >= height || x >= width)
                        {
                            _lastErrorMessage = Errors.ERROR_WAVELET_NO_DATA;
                            return null;
                        }
                        
                        // Katsayının ondalık kısmını al
                        double coefficient = coefficients[y, x];
                        double fractionalPart = coefficient - Math.Floor(coefficient);
                        
                        // Bit değerini belirle
                        bool bitValue = (fractionalPart > 0.5);
                        
                        // Bit değerini baytın ilgili bitine yaz
                        if (bitValue)
                        {
                            currentByte |= (byte)(1 << bit);
                        }
                        
                        currentIndex++;
                    }
                    
                    dataLengthBytes[i] = currentByte;
                }
                
                // Veri uzunluğunu hesapla
                int dataLength = BitConverter.ToInt32(dataLengthBytes, 0);
                
                // Veri uzunluğu geçerlilik kontrolü
                if (dataLength <= 0 || dataLength > 1000000)  // 1MB maksimum boyut kontrolü
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_DATA_CORRUPTED;
                    return null;
                }
                
                // Asıl veriyi çıkar
                byte[] extractedData = new byte[dataLength];
                
                for (int i = 0; i < dataLength; i++)
                {
                    byte currentByte = 0;
                    
                    for (int bit = 0; bit < 8; bit++)
                    {
                        int x = startX + (currentIndex % (width >> defaultDecompositionLevel));
                        int y = startY + (currentIndex / (width >> defaultDecompositionLevel));
                        
                        if (y >= height || x >= width)
                        {
                            _lastErrorMessage = Errors.ERROR_WAVELET_DATA_CORRUPTED;
                            return null;
                        }
                        
                        // Katsayının ondalık kısmını al
                        double coefficient = coefficients[y, x];
                        double fractionalPart = coefficient - Math.Floor(coefficient);
                        
                        // Bit değerini belirle
                        bool bitValue = (fractionalPart > 0.5);
                        
                        // Bit değerini baytın ilgili bitine yaz
                        if (bitValue)
                        {
                            currentByte |= (byte)(1 << bit);
                        }
                        
                        currentIndex++;
                    }
                    
                    extractedData[i] = currentByte;
                }
                
                _lastSuccessMessage = Success.WAVELET_DECODE_SUCCESS;
                return extractedData;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_WAVELET_DECODE, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES ile şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="cipherText">Şifreli veri</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>Çözülmüş metin</returns>
        private string DecryptStringWithAES(byte[] cipherText, string password)
        {
            try
            {
                if (cipherText == null || cipherText.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return null;
                }
                
                // İşlem için gerekli nesneleri oluştur
                using (Aes aesAlg = Aes.Create())
                {
                    // Parola ve salt değerden anahtar ve IV türet (şifreleme ile aynı olmalı)
                    byte[] salt = Encoding.UTF8.GetBytes("WaveletSaltValue123");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
                    
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    
                    // Şifre çözme işlemiyle ilgili nesneleri oluştur
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Tüm metni oku
                                string plaintext = srDecrypt.ReadToEnd();
                                return plaintext;
                            }
                        }
                    }
                }
            }
            catch (CryptographicException ex)
            {
                _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                return null;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Wavelet dönüşümünün katsayılarını analiz ederek bir görüntüde gizli veri olup olmadığını kontrol eder
        /// </summary>
        /// <param name="image">Kontrol edilecek görüntü</param>
        /// <returns>Gizli veri tespit edilirse true, aksi halde false</returns>
        public bool HasHiddenData(Bitmap image)
        {
            try
            {
                if (image == null || image.Width < 300 || image.Height < 300)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_TOO_SMALL;
                    return false;
                }
                
                // Wavelet dönüşümü uygula
                double[,] coefficients = ApplyWaveletTransform(image);
                if (coefficients == null)
                {
                    return false;
                }
                
                int height = coefficients.GetLength(0);
                int width = coefficients.GetLength(1);
                
                // Wavelet katsayılarının ortasından belirli sayıda örnek al
                int startX = (width >> (defaultDecompositionLevel + 1));
                int startY = (height >> (defaultDecompositionLevel + 1));
                int sampleSize = 100; // İncelenecek örnek sayısı
                
                int countRegular = 0;  // Düzenli dağılım (normal görüntü) sayacı
                int countBimodal = 0;  // İkili dağılım (gizli veri) sayacı
                
                for (int i = 0; i < sampleSize; i++)
                {
                    int x = startX + (i % 10);
                    int y = startY + (i / 10);
                    
                    if (y >= height || x >= width)
                        break;
                    
                    double coefficient = coefficients[y, x];
                    double fractionalPart = coefficient - Math.Floor(coefficient);
                    
                    // Veri gizleme algoritmasında kullanılan ondalık kısım değerlerine (0.25 veya 0.75) yakın mı?
                    if (Math.Abs(fractionalPart - 0.25) < 0.1 || Math.Abs(fractionalPart - 0.75) < 0.1)
                    {
                        countBimodal++;
                    }
                    else
                    {
                        countRegular++;
                    }
                }
                
                // Eğer ikili dağılım oranı belirli bir eşiği geçerse, veri gizlenmiş olabilir
                double bimodalRatio = (double)countBimodal / sampleSize;
                bool hasHiddenData = bimodalRatio > 0.7; // %70 oranında ikili dağılım görülürse pozitif
                
                return hasHiddenData;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// DataLayer/config.json dosyasından SystemIdentity değerini okur
        /// </summary>
        /// <returns>SystemIdentity değeri</returns>
        private string GetSystemIdentity()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\ImageBasedEncryptionSystem.DataLayer\\config.json");
                
                // Config dosyası için alternatif yol
                if (!File.Exists(configPath))
                {
                    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\ImageBasedEncryptionSystem.DataLayer\\config.json");
                }
                
                if (!File.Exists(configPath))
                {
                    _lastErrorMessage = "Config dosyası bulunamadı: " + configPath;
                    return null;
                }
                
                string json = File.ReadAllText(configPath);
                JObject config = JObject.Parse(json);
                
                JToken identityArray = config["Identity"];
                if (identityArray != null && identityArray.Type == JTokenType.Array && identityArray.HasValues)
                {
                    JToken firstIdentity = identityArray.First;
                    string systemIdentity = firstIdentity["SystemIdentity"]?.ToString();
                    
                    if (string.IsNullOrEmpty(systemIdentity))
                    {
                        _lastErrorMessage = "SystemIdentity değeri config dosyasında bulunamadı.";
                        return null;
                    }
                    
                    return systemIdentity;
                }
                else
                {
                    _lastErrorMessage = "Identity array config dosyasında bulunamadı.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format("Config dosyasını okurken hata: {0}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// SystemIdentity değerine göre deterministik bir RSA anahtar çifti oluşturur
        /// </summary>
        /// <returns>RSA anahtar çifti (RSA sağlayıcı)</returns>
        private RSACryptoServiceProvider GenerateRsaProviderFromSystemIdentity()
        {
            try
            {
                // SystemIdentity değerini al
                string systemIdentity = GetSystemIdentity();
                if (string.IsNullOrEmpty(systemIdentity))
                {
                    // Hata mesajı zaten GetSystemIdentity() içinde ayarlandı
                    return null;
                }
                
                // SystemIdentity'den seed değeri oluştur
                byte[] identityBytes = Encoding.UTF8.GetBytes(systemIdentity);
                byte[] seedBytes;
                
                using (SHA256 sha256 = SHA256.Create())
                {
                    seedBytes = sha256.ComputeHash(identityBytes);
                }
                
                // Seed değerinden rasgele sayı üreteci oluştur (deterministik)
                int seed = BitConverter.ToInt32(seedBytes, 0);
                Random random = new Random(seed);
                
                // RSA anahtarı oluştur
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE);
                
                _lastSuccessMessage = Success.KEY_GENERATION_SUCCESS;
                return rsa;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_RSA_KEY_GENERATION, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// RSA ile şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="encryptedData">Şifreli veri</param>
        /// <returns>Çözülmüş veri</returns>
        public byte[] DecryptWithRSA(byte[] encryptedData)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_RSA_NO_DATA;
                    return null;
                }
                
                // RSA sağlayıcısını oluştur
                using (RSACryptoServiceProvider rsa = GenerateRsaProviderFromSystemIdentity())
                {
                    if (rsa == null)
                    {
                        // Hata mesajı zaten GenerateRsaProviderFromSystemIdentity() içinde ayarlandı
                        return null;
                    }
                    
                    // RSA blok boyutu
                    int blockSize = rsa.KeySize / 8;
                    
                    // Eğer veri tek blok ise doğrudan çöz
                    if (encryptedData.Length == blockSize)
                    {
                        byte[] decryptedData = rsa.Decrypt(encryptedData, true); // OAEP padding kullan
                        _lastSuccessMessage = Success.DECRYPT_SUCCESS_RSA;
                        return decryptedData;
                    }
                    else
                    {
                        // Veri birden fazla blok içeriyorsa
                        // İlk bloğu çözerek blok sayısını al
                        if (encryptedData.Length < blockSize)
                        {
                            _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                            return null;
                        }
                        
                        byte[] firstBlock = new byte[blockSize];
                        Buffer.BlockCopy(encryptedData, 0, firstBlock, 0, blockSize);
                        
                        // İlk bloğu çöz ve blok sayısını al
                        byte[] blockCountBytes;
                        try
                        {
                            blockCountBytes = rsa.Decrypt(firstBlock, true);
                        }
                        catch (CryptographicException)
                        {
                            _lastErrorMessage = Errors.ERROR_RSA_DECRYPT;
                            return null;
                        }
                        
                        int blockCount = BitConverter.ToInt32(blockCountBytes, 0);
                        
                        // Blok sayısı geçerlilik kontrolü
                        if (blockCount <= 0 || blockCount > 1000 || (blockCount + 1) * blockSize != encryptedData.Length)
                        {
                            _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                            return null;
                        }
                        
                        // Her bir bloku çöz ve birleştir
                        using (MemoryStream ms = new MemoryStream())
                        {
                            for (int i = 0; i < blockCount; i++)
                            {
                                int offset = (i + 1) * blockSize; // İlk blok blok sayısı olduğu için +1
                                byte[] encryptedBlock = new byte[blockSize];
                                Buffer.BlockCopy(encryptedData, offset, encryptedBlock, 0, blockSize);
                                
                                try
                                {
                                    byte[] decryptedBlock = rsa.Decrypt(encryptedBlock, true);
                                    ms.Write(decryptedBlock, 0, decryptedBlock.Length);
                                }
                                catch (CryptographicException)
                                {
                                    _lastErrorMessage = Errors.ERROR_RSA_DECRYPT;
                                    return null;
                                }
                            }
                            
                            _lastSuccessMessage = Success.DECRYPT_SUCCESS_RSA;
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_RSA_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// RSA ile şifrelenmiş metni çözer
        /// </summary>
        /// <param name="encryptedData">Şifreli veri</param>
        /// <returns>Çözülmüş metin</returns>
        public string DecryptTextWithRSA(byte[] encryptedData)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_RSA_NO_DATA;
                    return null;
                }
                
                byte[] decryptedData = DecryptWithRSA(encryptedData);
                if (decryptedData == null)
                {
                    // Hata mesajı zaten DecryptWithRSA() içinde ayarlandı
                    return null;
                }
                
                string decryptedText = Encoding.UTF8.GetString(decryptedData);
                _lastSuccessMessage = Success.DECRYPT_SUCCESS_RSA;
                return decryptedText;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_RSA_DECRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Wavelet ve RSA ile şifrelenmiş görüntüden metni çözer
        /// </summary>
        /// <param name="encryptedImage">Şifrelenmiş görüntü</param>
        /// <param name="password">AES şifre çözme parolası</param>
        /// <returns>Çözülmüş metin</returns>
        public string DecryptTextFromImageWithRSA(Bitmap encryptedImage, string password)
        {
            try
            {
                // Şifre kontrolü
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Görüntü kontrolü
                if (encryptedImage == null || encryptedImage.Width < 300 || encryptedImage.Height < 300)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_TOO_SMALL;
                    return null;
                }

                // Wavelet dönüşümü uygula ve gizli veriyi çıkar
                byte[] extractedData = ExtractDataFromWaveletCoefficients(encryptedImage, password);
                if (extractedData == null || extractedData.Length == 0)
                {
                    // Hata mesajı zaten ExtractDataFromWaveletCoefficients() içinde ayarlandı
                    return null;
                }

                // AES şifresini çöz
                byte[] aesDecryptedData = DecryptDataWithAES(extractedData, password);
                if (aesDecryptedData == null)
                {
                    _lastErrorMessage = Errors.ERROR_AES_DECRYPT;
                    return null;
                }

                // RSA şifresini çöz
                string decryptedText = DecryptTextWithRSA(aesDecryptedData);
                if (decryptedText != null)
                {
                    _lastSuccessMessage = Success.DECRYPT_SUCCESS_AES_RSA;
                    return decryptedText;
                }
                else
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_DECODE;
                    return null;
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_GENERAL_UNEXPECTED, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// AES ile şifrelenmiş veriyi çözer (RSA şifre çözme için yardımcı metod)
        /// </summary>
        /// <param name="encryptedData">Şifreli veri</param>
        /// <param name="password">Şifre çözme parolası</param>
        /// <returns>AES ile çözülmüş veri</returns>
        private byte[] DecryptDataWithAES(byte[] encryptedData, string password)
        {
            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_CIPHERTEXT_EMPTY;
                    return null;
                }
                
                // AES şifre çözme için gerekli nesneleri oluştur
                using (Aes aesAlg = Aes.Create())
                {
                    // Parola ve salt değerden anahtar ve IV türet (şifreleme ile aynı olmalı)
                    byte[] salt = Encoding.UTF8.GetBytes("WaveletRsaSaltValue");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
                    
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    
                    // Şifre çözme işlemiyle ilgili nesneleri oluştur
                    using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            // Şifresi çözülmüş veriyi oku
                            using (MemoryStream resultStream = new MemoryStream())
                            {
                                csDecrypt.CopyTo(resultStream);
                                return resultStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                _lastErrorMessage = Errors.ERROR_PASSWORD_WRONG;
                return null;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_DECRYPT, ex.Message);
                return null;
            }
        }
    }
} 