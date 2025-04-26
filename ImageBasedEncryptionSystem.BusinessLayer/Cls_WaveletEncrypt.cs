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
    public class Cls_WaveletEncrypt
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
        /// Wavelet dönüşümünü kullanarak veriyi görüntü içine gizler
        /// </summary>
        /// <param name="inputImage">Kaynak görüntü</param>
        /// <param name="message">Gizlenecek metin</param>
        /// <param name="password">Şifreleme için kullanılacak parola</param>
        /// <returns>Veri gizlenmiş görüntü</returns>
        public Bitmap EncryptTextToImage(Bitmap inputImage, string message, string password)
        {
            try
            {
                // Şifre kontrolü
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Mesaj kontrolü
                if (string.IsNullOrEmpty(message))
                {
                    _lastErrorMessage = Errors.ERROR_TEXT_EMPTY;
                    return null;
                }

                // Görüntü kontrolü
                if (inputImage == null || inputImage.Width < 300 || inputImage.Height < 300)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_TOO_SMALL;
                    return null;
                }

                // Mesajı AES ile şifrele
                byte[] encryptedData = EncryptStringWithAES(message, password);
                if (encryptedData == null)
                {
                    return null;
                }

                // Wavelet dönüşümü uygula ve katsayıları al
                Bitmap processedImage = HideDataInWaveletCoefficients(inputImage, encryptedData, password);
                if (processedImage != null)
                {
                    _lastSuccessMessage = Success.WAVELET_ENCRYPT_SUCCESS;
                    return processedImage;
                }
                else
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_ENCODE;
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
        /// Ters wavelet dönüşümü uygular
        /// </summary>
        /// <param name="coefficients">Wavelet katsayıları</param>
        /// <param name="level">Dönüşüm seviyesi</param>
        /// <returns>Dönüştürülmüş görüntü</returns>
        private Bitmap ApplyInverseWaveletTransform(double[,] coefficients, int level = defaultDecompositionLevel)
        {
            try
            {
                if (coefficients == null)
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_COEFFICIENTS;
                    return null;
                }
                
                int height = coefficients.GetLength(0);
                int width = coefficients.GetLength(1);
                
                // Katsayıları kopyala
                double[,] data = coefficients.Copy();
                
                // Haar wavelet dönüşümü uygula
                HaarWavelet wavelet = new HaarWavelet();
                
                // Ters 2D Discrete Wavelet dönüşümü uygula
                for (int i = level - 1; i >= 0; i--)
                {
                    int currentWidth = width >> i;
                    int currentHeight = height >> i;
                    
                    // Önce sütunlara ters dönüşüm uygula
                    for (int x = 0; x < currentWidth; x++)
                    {
                        double[] col = new double[currentHeight];
                        for (int y = 0; y < currentHeight; y++)
                            col[y] = data[y, x];
                            
                        // Ters wavelet dönüşümü uygula
                        wavelet.Inverse(col);
                        
                        // Dönüştürülen değerleri geri yaz
                        for (int y = 0; y < currentHeight; y++)
                            data[y, x] = col[y];
                    }
                    
                    // Sonra satırlara ters dönüşüm uygula
                    for (int y = 0; y < currentHeight; y++)
                    {
                        double[] row = new double[currentWidth];
                        for (int x = 0; x < currentWidth; x++)
                            row[x] = data[y, x];
                            
                        // Ters wavelet dönüşümü uygula
                        wavelet.Inverse(row);
                        
                        // Dönüştürülen değerleri geri yaz
                        for (int x = 0; x < currentWidth; x++)
                            data[y, x] = row[x];
                    }
                }
                
                // Double dizisini bitmap görüntüsüne dönüştür
                Bitmap resultImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                
                // Renk paletini ayarla
                ColorPalette palette = resultImage.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                resultImage.Palette = palette;
                
                // Verileri bitmap'e aktar
                BitmapData bmpData = resultImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);
                
                unsafe
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Değerleri [0, 255] aralığına sınırla
                            int pixelValue = (int)Math.Round(data[y, x]);
                            pixelValue = Math.Max(0, Math.Min(255, pixelValue));
                            
                            ptr[y * bmpData.Stride + x] = (byte)pixelValue;
                        }
                    }
                }
                
                resultImage.UnlockBits(bmpData);
                
                _lastSuccessMessage = Success.WAVELET_INVERSE_SUCCESS;
                return resultImage;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_WAVELET_INVERSE_TRANSFORM, ex.Message);
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
        /// Veriyi wavelet katsayılarına gizler
        /// </summary>
        /// <param name="inputImage">Kaynak görüntü</param>
        /// <param name="data">Gizlenecek veri</param>
        /// <param name="password">Şifreleme parolası</param>
        /// <returns>Veri gizlenmiş görüntü</returns>
        private Bitmap HideDataInWaveletCoefficients(Bitmap inputImage, byte[] data, string password)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_TEXT_EMPTY;
                    return null;
                }
                
                // Wavelet dönüşümü uygula
                double[,] coefficients = ApplyWaveletTransform(inputImage);
                if (coefficients == null)
                {
                    return null;
                }
                
                int height = coefficients.GetLength(0);
                int width = coefficients.GetLength(1);
                
                // Görüntü kapasitesi kontrolü
                int waveletSubbandSize = (width >> defaultDecompositionLevel) * (height >> defaultDecompositionLevel);
                int availableCapacity = waveletSubbandSize / 8;  // Her bayt için 8 katsayı kullanılacak varsayımı
                
                if (data.Length + 8 > availableCapacity)  // 8 bayt ekstra yer (veri uzunluğu için)
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_INSUFFICIENT_CAPACITY;
                    return null;
                }
                
                // Veri boyutunu gizle (ilk 4 bayt)
                byte[] dataLengthBytes = BitConverter.GetBytes(data.Length);
                
                // Parola hash değeri üret
                byte[] passwordHash;
                using (SHA256 sha256 = SHA256.Create())
                {
                    passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
                
                // Wavelet katsayılarının ortasına doğru verileri gizle
                int startX = (width >> (defaultDecompositionLevel + 1));  // Alt bandın ortasına yakın başla
                int startY = (height >> (defaultDecompositionLevel + 1));
                int currentIndex = 0;
                
                // Önce veri uzunluğunu gizle
                for (int i = 0; i < dataLengthBytes.Length; i++)
                {
                    byte currentByte = dataLengthBytes[i];
                    
                    for (int bit = 0; bit < 8; bit++)
                    {
                        int x = startX + (currentIndex % (width >> defaultDecompositionLevel));
                        int y = startY + (currentIndex / (width >> defaultDecompositionLevel));
                        
                        // Katsayının ondalık kısmını değiştir
                        double coefficient = coefficients[y, x];
                        double integerPart = Math.Floor(coefficient);
                        double fractionalPart = coefficient - integerPart;
                        
                        // Bit değerine göre ondalık kısmı ayarla
                        bool bitValue = (currentByte & (1 << bit)) != 0;
                        double newFractionalPart = bitValue ? 0.75 : 0.25;
                        
                        // Yeni katsayı değerini ata
                        coefficients[y, x] = integerPart + newFractionalPart;
                        
                        currentIndex++;
                    }
                }
                
                // Sonra asıl veriyi gizle
                for (int i = 0; i < data.Length; i++)
                {
                    byte currentByte = data[i];
                    
                    for (int bit = 0; bit < 8; bit++)
                    {
                        // Katsayı dizisinde pozisyon belirle
                        int x = startX + (currentIndex % (width >> defaultDecompositionLevel));
                        int y = startY + (currentIndex / (width >> defaultDecompositionLevel));
                        
                        if (y >= height || x >= width)
                        {
                            _lastErrorMessage = Errors.ERROR_WAVELET_INSUFFICIENT_CAPACITY;
                            return null;
                        }
                        
                        // Katsayının ondalık kısmını değiştir
                        double coefficient = coefficients[y, x];
                        double integerPart = Math.Floor(coefficient);
                        double fractionalPart = coefficient - integerPart;
                        
                        // Bit değerine göre ondalık kısmı ayarla
                        bool bitValue = (currentByte & (1 << bit)) != 0;
                        double newFractionalPart = bitValue ? 0.75 : 0.25;
                        
                        // Yeni katsayı değerini ata
                        coefficients[y, x] = integerPart + newFractionalPart;
                        
                        currentIndex++;
                    }
                }
                
                // Veriler gizlendikten sonra ters wavelet dönüşümünü uygula
                Bitmap resultImage = ApplyInverseWaveletTransform(coefficients);
                
                // İşlem başarılı mesajı
                _lastSuccessMessage = Success.WAVELET_ENCODE_SUCCESS;
                return resultImage;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_WAVELET_ENCODE, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Metni AES ile şifreler
        /// </summary>
        /// <param name="plainText">Şifrelenecek metin</param>
        /// <param name="password">Şifreleme parolası</param>
        /// <returns>Şifrelenmiş veri</returns>
        private byte[] EncryptStringWithAES(string plainText, string password)
        {
            try
            {
                // İşlem için gerekli nesneleri oluştur
                using (Aes aesAlg = Aes.Create())
                {
                    // Parola ve salt değerden anahtar ve IV türet
                    byte[] salt = Encoding.UTF8.GetBytes("WaveletSaltValue123");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
                    
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    
                    // Şifreleme işlemiyle ilgili nesneleri oluştur
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Metni yaz
                                swEncrypt.Write(plainText);
                            }
                            
                            return msEncrypt.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Wavelet tabanlı şifreli görüntüyü kaydeder
        /// </summary>
        /// <param name="image">Kaydedilecek görüntü</param>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>İşlem başarılı ise true, değilse false</returns>
        public bool SaveEncryptedImage(Bitmap image, string filePath)
        {
            try
            {
                if (image == null)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_EMPTY;
                    return false;
                }
                
                // Wavelet ile şifrelenmiş görüntüyü PNG olarak kaydet
                image.Save(filePath, ImageFormat.Png);
                
                _lastSuccessMessage = Success.IMAGE_SAVE_SUCCESS;
                return true;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_FILE_SAVE, ex.Message);
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
        /// RSA ile veriyi şifreler
        /// </summary>
        /// <param name="data">Şifrelenecek veri</param>
        /// <returns>Şifrelenmiş veri</returns>
        public byte[] EncryptWithRSA(byte[] data)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
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
                    
                    // RSA blok boyutu hesapla
                    int keySize = rsa.KeySize;
                    int blockSize = (keySize / 8) - 42; // OAEP padding için gerekli alan (42 byte)
                    
                    // Eğer veri tek bloktan küçükse doğrudan şifrele
                    if (data.Length <= blockSize)
                    {
                        byte[] encryptedData = rsa.Encrypt(data, true); // OAEP padding kullan
                        _lastSuccessMessage = Success.ENCRYPT_SUCCESS_RSA;
                        return encryptedData;
                    }
                    else
                    {
                        // Veri büyükse bloklara böl ve her birini ayrı ayrı şifrele
                        List<byte[]> encryptedBlocks = new List<byte[]>();
                        
                        // İlk blok olarak şifrelenecek veri blok sayısını ekle
                        int blockCount = (data.Length + blockSize - 1) / blockSize;
                        byte[] blockCountBytes = BitConverter.GetBytes(blockCount);
                        encryptedBlocks.Add(rsa.Encrypt(blockCountBytes, true));
                        
                        // Her bloğu şifrele
                        for (int i = 0; i < blockCount; i++)
                        {
                            int offset = i * blockSize;
                            int currentBlockSize = Math.Min(blockSize, data.Length - offset);
                            
                            byte[] block = new byte[currentBlockSize];
                            Buffer.BlockCopy(data, offset, block, 0, currentBlockSize);
                            
                            byte[] encryptedBlock = rsa.Encrypt(block, true);
                            encryptedBlocks.Add(encryptedBlock);
                        }
                        
                        // Şifrelenmiş blokları birleştir
                        int totalSize = 0;
                        foreach (byte[] block in encryptedBlocks)
                        {
                            totalSize += block.Length;
                        }
                        
                        byte[] result = new byte[totalSize];
                        int currentOffset = 0;
                        
                        foreach (byte[] block in encryptedBlocks)
                        {
                            Buffer.BlockCopy(block, 0, result, currentOffset, block.Length);
                            currentOffset += block.Length;
                        }
                        
                        _lastSuccessMessage = Success.ENCRYPT_SUCCESS_RSA;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_RSA_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Metni RSA ile şifreler
        /// </summary>
        /// <param name="text">Şifrelenecek metin</param>
        /// <returns>Şifrelenmiş veri</returns>
        public byte[] EncryptTextWithRSA(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    _lastErrorMessage = Errors.ERROR_TEXT_EMPTY;
                    return null;
                }
                
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                return EncryptWithRSA(textBytes);
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_RSA_ENCRYPT, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Wavelet şifreleme ile RSA şifrelemeyi birleştirir
        /// </summary>
        /// <param name="inputImage">Kaynak görüntü</param>
        /// <param name="message">Şifrelenecek metin</param>
        /// <param name="password">AES şifreleme parolası</param>
        /// <returns>Şifrelenmiş görüntü</returns>
        public Bitmap EncryptTextWithWaveletAndRSA(Bitmap inputImage, string message, string password)
        {
            try
            {
                // Şifre kontrolü
                if (string.IsNullOrEmpty(password))
                {
                    _lastErrorMessage = Errors.ERROR_PASSWORD_EMPTY;
                    return null;
                }

                // Mesaj kontrolü
                if (string.IsNullOrEmpty(message))
                {
                    _lastErrorMessage = Errors.ERROR_TEXT_EMPTY;
                    return null;
                }

                // Görüntü kontrolü
                if (inputImage == null || inputImage.Width < 300 || inputImage.Height < 300)
                {
                    _lastErrorMessage = Errors.ERROR_IMAGE_TOO_SMALL;
                    return null;
                }

                // Metni önce RSA ile şifrele
                byte[] rsaEncryptedData = EncryptTextWithRSA(message);
                if (rsaEncryptedData == null)
                {
                    // Hata mesajı zaten EncryptTextWithRSA() içinde ayarlandı
                    return null;
                }

                // RSA ile şifrelenmiş veriyi AES ile şifrele
                byte[] aesEncryptedData = EncryptDataWithAES(rsaEncryptedData, password);
                if (aesEncryptedData == null)
                {
                    _lastErrorMessage = Errors.ERROR_AES_ENCRYPT;
                    return null;
                }

                // Wavelet dönüşümü uygula ve katsayıları al
                Bitmap processedImage = HideDataInWaveletCoefficients(inputImage, aesEncryptedData, password);
                if (processedImage != null)
                {
                    _lastSuccessMessage = Success.ENCRYPT_SUCCESS_AES_RSA;
                    return processedImage;
                }
                else
                {
                    _lastErrorMessage = Errors.ERROR_WAVELET_ENCODE;
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
        /// Veriyi AES ile şifreler (RSA şifrelemesi için yardımcı metod)
        /// </summary>
        /// <param name="data">Şifrelenecek veri</param>
        /// <param name="password">Şifreleme parolası</param>
        /// <returns>AES ile şifrelenmiş veri</returns>
        private byte[] EncryptDataWithAES(byte[] data, string password)
        {
            try
            {
                if (data == null || data.Length == 0)
                {
                    _lastErrorMessage = Errors.ERROR_DATA_CORRUPTED;
                    return null;
                }

                // AES şifreleme için gerekli nesneleri oluştur
                using (Aes aesAlg = Aes.Create())
                {
                    // Parola ve salt değerden anahtar ve IV türet
                    byte[] salt = Encoding.UTF8.GetBytes("WaveletRsaSaltValue");
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
                    
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                    
                    // Şifreleme işlemiyle ilgili nesneleri oluştur
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            // Veriyi şifreleme akışına yaz
                            csEncrypt.Write(data, 0, data.Length);
                            csEncrypt.FlushFinalBlock();
                            
                            return msEncrypt.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _lastErrorMessage = string.Format(Errors.ERROR_AES_ENCRYPT, ex.Message);
                return null;
            }
        }
    }
}
