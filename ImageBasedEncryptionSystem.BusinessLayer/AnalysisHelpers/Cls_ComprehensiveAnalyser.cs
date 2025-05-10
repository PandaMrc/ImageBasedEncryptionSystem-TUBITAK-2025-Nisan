using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ImageBasedEncryptionSystem.BusinessLayer;
using ImageBasedEncryptionSystem.BusinessLayer.Helpers;
using System.Linq;
using System.Runtime.InteropServices;
// Diğer helper sınıflarınızın using bildirimleri
// using ImageBasedEncryptionSystem.BusinessLayer.Helpers; // Cls_RsHelper, Cls_SamplePairAnalysis, Cls_LsbChangeMap
// using static ImageBasedEncryptionSystem.BusinessLayer.Helpers.Cls_RsHelper; // Eğer RsAnalysisResult gibi iç içe tanımlar varsa
// using static ImageBasedEncryptionSystem.BusinessLayer.Helpers.Cls_SamplePairAnalysis; // SpaAnalysisResult için

public static class Cls_ComprehensiveAnalyser
{
    /// <summary>
    /// Kapsamlı steganografi analizi yapar ve sonuçları döndürür.
    /// </summary>
    /// <param name="stegoImage">Analiz edilecek şifreli/stego görüntü</param>
    /// <param name="originalImage">Karşılaştırma için orijinal görüntü (opsiyonel, geçilebilir)</param>
    /// <param name="fileName">Görüntü dosyasının adı</param>
    /// <param name="channel">Analiz edilecek renk kanalı</param>
    /// <param name="rsGroupSize">RS analizi için grup boyutu</param>
    /// <returns>Tüm analiz sonuçlarını içeren sonuç nesnesi</returns>
    public static async Task<Cls_ComprehensiveSteganalysisResult> AnalyzeImageAsync(
        Bitmap stegoImage,
        Bitmap originalImage,
        string fileName,
        ColorChannel channel = ColorChannel.Blue,
        int rsGroupSize = 4)
    {
        if (stegoImage == null)
            throw new ArgumentNullException(nameof(stegoImage));

        var result = new Cls_ComprehensiveSteganalysisResult
        {
            FileName = fileName,
            AnalysisTimestamp = DateTime.Now,
            FileSize = 0, // Dosya boyutunu sonra doldur
            ImageWidth = stegoImage.Width,
            ImageHeight = stegoImage.Height
        };

        try
        {
            // Görüntüyü güvenli olarak klonla (sadece referans değil, tam kopya)
            using (Bitmap safeCopy = (Bitmap)stegoImage.Clone())
            using (Bitmap originalCopy = originalImage != null ? (Bitmap)originalImage.Clone() : null)
            {
                // Dosya boyutunu tahmin et
                using (MemoryStream ms = new MemoryStream())
                {
                    safeCopy.Save(ms, ImageFormat.Png);
                    result.FileSize = ms.Length;
                }

                // 1. Şifreli resim için RS Analizi
                var stegoRsResult = await Task.Run(() => Cls_RsHelper.PerformRegularSingularAnalysis(safeCopy, channel, rsGroupSize));
                result.RsGroupsResult = stegoRsResult;

                // 1.1 Orijinal resim için RS Analizi (eğer originalImage varsa)
                if (originalCopy != null)
                {
                    var originalRsResult = await Task.Run(() => Cls_RsHelper.PerformRegularSingularAnalysis(originalCopy, channel, rsGroupSize));
                    result.OriginalRsGroupsResult = originalRsResult; // Burada Cls_ComprehensiveSteganalysisResult sınıfına OriginalRsGroupsResult özelliği eklenmeli
                }

                // 2. Şifreli resim için SPA Analizi
                var stegoSpaResult = await Task.Run(() => Cls_SpaHelper.PerformSpaAnalysis(safeCopy, channel));
                result.SpaResult = stegoSpaResult;

                // 2.1 Orijinal resim için SPA Analizi (eğer originalImage varsa)
                if (originalCopy != null)
                {
                    var originalSpaResult = await Task.Run(() => Cls_SpaHelper.PerformSpaAnalysis(originalCopy, channel));
                    result.OriginalSpaResult = originalSpaResult; // Burada Cls_ComprehensiveSteganalysisResult sınıfına OriginalSpaResult özelliği eklenmeli
                }

                // 3. Değişim Haritası (eğer originalImage varsa)
                if (originalCopy != null && 
                    originalCopy.Width == safeCopy.Width && 
                    originalCopy.Height == safeCopy.Height)
                {
                    var changedPixels = 0;
                    int pixelTotal = safeCopy.Width * safeCopy.Height;
                    
                    // Tüm RGB kanallarında değişiklikleri kontrol et
                    for (int y = 0; y < safeCopy.Height; y++)
                    {
                        for (int x = 0; x < safeCopy.Width; x++)
                        {
                            Color originalPixel = originalCopy.GetPixel(x, y);
                            Color stegoPixel = safeCopy.GetPixel(x, y);

                            // Seçilen kanalın LSB'sine göre kontrol
                            bool originalLsb = false;
                            bool stegoLsb = false;

                            switch (channel)
                            {
                                case ColorChannel.Red:
                                    originalLsb = (originalPixel.R & 1) == 1;
                                    stegoLsb = (stegoPixel.R & 1) == 1;
                                    break;
                                case ColorChannel.Green:
                                    originalLsb = (originalPixel.G & 1) == 1;
                                    stegoLsb = (stegoPixel.G & 1) == 1;
                                    break;
                                case ColorChannel.Blue:
                                    originalLsb = (originalPixel.B & 1) == 1;
                                    stegoLsb = (stegoPixel.B & 1) == 1;
                                    break;
                            }

                            if (originalLsb != stegoLsb)
                                changedPixels++;
                        }
                    }

                    result.ChangedPixelCount = changedPixels;
                    
                    // Değişim oranını hesapla - toplam piksel sayısına böl
                    result.ChangeRatio = (double)changedPixels / pixelTotal;
                    
                    // Çok küçük değerleri sıfıra yuvarla
                    if (result.ChangeRatio < 0.001) // %0.1'den küçükse
                    {
                        result.ChangeRatio = 0;
                    }
                    
                    // Değişim oranı, doğrudan gömme oranıdır
                    // Eğer değişim haritası varsa, bu en doğru ölçümdür
                    result.AverageEmbeddingRate = result.ChangeRatio;
                    
                    // Tahmini gizli veri boyutunu doğrudan değiştirilmiş piksel sayısından hesapla
                    // Her değiştirilmiş piksel 1 bit veri taşır
                    result.EstimatedHiddenBits = changedPixels;
                    result.EstimatedHiddenBytes = changedPixels / 8;
                    
                    // Diğer analizleri atlayabiliriz çünkü değişim haritası varsa en doğru sonucu verir
                    return result;
                }

                // Değişim haritası yoksa, diğer analizleri kullan
                // 4. Ortalama gömme oranı hesaplama
                double totalEmbeddingRate = 0;
                double validAnalysisCount = 0;

                // RS Analizi sonucu - en güvenilir yöntem
                if (result.RsGroupsResult.EstimatedEmbeddingRateP >= 0 && result.RsGroupsResult.EstimatedEmbeddingRateP <= 1)
                {
                    // RS analizine en yüksek ağırlık ver
                    totalEmbeddingRate += result.RsGroupsResult.EstimatedEmbeddingRateP * 3;
                    validAnalysisCount += 3;
                }

                // SPA Analizi sonucu - daha az güvenilir
                if (!double.IsNaN(result.SpaResult.EstimatedEmbeddingRateP) && 
                    result.SpaResult.EstimatedEmbeddingRateP >= 0 && 
                    result.SpaResult.EstimatedEmbeddingRateP <= 1)
                {
                    // SPA analizine daha düşük ağırlık ver
                    totalEmbeddingRate += result.SpaResult.EstimatedEmbeddingRateP * 0.5;
                    validAnalysisCount += 0.5;
                }

                // Ortalama hesaplama
                result.AverageEmbeddingRate = validAnalysisCount > 0 ? totalEmbeddingRate / validAnalysisCount : 0;

                // Aşırı yüksek tahminleri düzelt - maksimum %33 gömme oranı olabilir (tek kanalda)
                if (result.AverageEmbeddingRate > 0.33)
                {
                    result.AverageEmbeddingRate = 0.33;
                }

                // 5. Tahmini gizli veri uzunluğu - sadece seçili kanalı dikkate al
                long totalPixelCount = stegoImage.Width * stegoImage.Height;

                // Gerçek çıkarılan veri varsa, tahmini değil gerçek değerleri kullan
                if (result.ExtractedDataSize > 0)
                {
                    result.EstimatedHiddenBits = result.ExtractedDataSize * 8;
                    result.EstimatedHiddenBytes = result.ExtractedDataSize;
                }
                else
                {
                    // Gerçek veri yoksa tahmin et
                    result.EstimatedHiddenBits = (long)(totalPixelCount * result.AverageEmbeddingRate);
                    result.EstimatedHiddenBytes = result.EstimatedHiddenBits / 8;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Analiz işlemi sırasında hata oluştu: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Şifreli görselden veri çıkarır ve sonuç nesnesine ekler.
    /// </summary>
    public static void ExtractDataFromEncryptedImage(Bitmap encryptedImage, Cls_ComprehensiveSteganalysisResult result)
    {
        if (encryptedImage == null) throw new ArgumentNullException(nameof(encryptedImage));
        if (result == null) throw new ArgumentNullException(nameof(result));

        try
        {
            // Görüntü kopyasını oluşturalım (belleği yönetmek için)
            using (Bitmap safeCopy = new Bitmap(encryptedImage))
            {
                try
                {
                    // LSB'den veriyi çıkart - boyut sınırlaması ekleyelim
                    byte[] extractedData = Cls_LsbHelper.ExtractData(safeCopy, 1024 * 1024 * 10); // Maksimum 10 MB ile sınırla
                    
                    // Veri boyutu ve işlenen piksel sayısı 
                    result.ExtractedDataSize = extractedData.Length;
                    
                    // İmza için kullanılan pikseller hariç, her 3 bit (RGB) için 1 piksel gerekir
                    // 8 bit = 1 byte, her piksel 3 bit veri içerebilir (R,G,B kanalları)
                    int bitsRequired = extractedData.Length * 8;
                    result.ProcessedPixelCount = (int)Math.Ceiling(bitsRequired / 3.0);
                    
                    // Piksel başına bit yoğunluğunu hesapla (çıkarılan veri / işlenen piksel)
                    double bitsPerPixel = (result.ExtractedDataSize * 8.0) / Math.Max(1, result.ProcessedPixelCount);
                    result.ExtractedDataDensity = bitsPerPixel;

                    // Veriyi ayrıştır
                    string extractedString = Encoding.UTF8.GetString(extractedData);

                    // Hata ayıklama için log
                    Console.WriteLine($"Çıkarılan veri uzunluğu: {extractedData.Length} bayt");
                    Console.WriteLine($"Çıkarılan metin uzunluğu: {extractedString.Length} karakter");

                    // ';' ayracı kontrolü
                    string[] parts = extractedString.Split(';');
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Hata: Çıkarılan veride ';' ayracı bulunamadı.");
                        result.AesEncryptedText = "Format hatası: Veri ayracı bulunamadı. Veri bozuk olabilir.";
                        result.RsaEncryptedAesKey = "Format hatası: Veri ayracı bulunamadı. Veri bozuk olabilir.";
                    }
                    else
                    {
                        try
                        {
                            // Veriyi parçalara ayır
                            result.AesEncryptedText = parts[0];
                            result.RsaEncryptedAesKey = parts[1];

                            // Boş kontrolleri
                            if (string.IsNullOrWhiteSpace(result.AesEncryptedText))
                            {
                                result.AesEncryptedText = "Format hatası: AES şifreli metin boş.";
                            }
                            if (string.IsNullOrWhiteSpace(result.RsaEncryptedAesKey))
                            {
                                result.RsaEncryptedAesKey = "Format hatası: RSA şifreli anahtar boş.";
                            }

                            Console.WriteLine($"AES şifreli metin uzunluğu: {result.AesEncryptedText.Length}");
                            Console.WriteLine($"RSA şifreli anahtar uzunluğu: {result.RsaEncryptedAesKey.Length}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Veri ayrıştırma hatası: {ex.Message}");
                            result.AesEncryptedText = $"Format hatası: Veri ayrıştırılamadı. Hata: {ex.Message}";
                            result.RsaEncryptedAesKey = $"Format hatası: Veri ayrıştırılamadı. Hata: {ex.Message}";
                        }
                    }

                    // İlk 20 pikseldeki imzayı kontrol et
                    result.IsSignatureValid = Cls_LsbHelper.CheckSignature(safeCopy);
                }
                catch (OutOfMemoryException)
                {
                    Console.WriteLine("Bellek yetersiz: Görüntü çok büyük veya çok fazla veri içeriyor.");
                    result.AesEncryptedText = "Bellek yetersiz hatası - görüntü çok büyük";
                    result.RsaEncryptedAesKey = "Bellek yetersiz hatası - görüntü çok büyük";
                    result.IsSignatureValid = false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Veri çıkarma sırasında hata: {ex.Message}");
            // Hata durumunda varsayılan boş değerler kullanılır (zaten constructor'da atanmış)
        }
    }
}