# Görüntü Tabanlı Hibrit Şifreleme Sistemi

Görüntü Tabanlı Hibrit Şifreleme Sistemi, metin mesajlarını şifreleyerek görüntü dosyaları içerisine güvenli bir şekilde gizleyen, C# tabanlı bir kriptografi ve steganografi uygulamasıdır.

## Proje Hakkında

Bu proje, AES, RSA gibi güçlü şifreleme algoritmalarını ve gelişmiş steganografi tekniklerini birleştirerek, hassas verilerin güvenli bir şekilde iletilmesini sağlamaktadır. Katmanlı mimarisi ile modüler bir yapı sunan proje, geliştirici modu ve analiz araçları ile de ileri seviye güvenlik değerlendirmesi yapabilmektedir.

### Kullanılan Teknolojiler

- C# / .NET Framework
- WinForms (Guna UI2 kütüphanesi)
- AES (Advanced Encryption Standard)
- RSA (Rivest-Shamir-Adleman)
- Wavelet Dönüşümü tabanlı Steganografi

## Gelecek Güncellemeler - Yol Haritası

### 1. Gün - Altyapı ve Wavelet Entegrasyonu
- Accord.NET veya AForge.NET kütüphanesinin projeye eklenmesi
- `Cls_WaveletEncrypt.cs` ve `Cls_WaveletDecrypt.cs` sınıflarının oluşturulması
- Wavelet dönüşümü için temel metotların uygulanması
- Görüntüye veri gömme fonksiyonlarının yazılması
- Dönüşüm katsayılarını değiştirerek veri gizleme algoritmalarının uygulanması
- Temel birim testlerinin oluşturulması ve çalıştırılması

### 2. Gün - Şifreleme/Çözme Sistemi Tamamlama
- `FrmMenu.cs` içindeki `btnEncrypt_Click` ve `btnDecrypt_Click` fonksiyonlarının tamamlanması
- Görüntü yükleme ve kaydetme özelliklerinin eklenmesi
- AES ve RSA şifreleme entegrasyonunun kontrol edilmesi
- UI iyileştirmeleri ve kullanıcı deneyiminin geliştirilmesi
- Hata yakalama ve kullanıcı bildirimlerinin iyileştirilmesi
- İlk end-to-end test: şifreleme ve çözme sürecinin test edilmesi

### 3. Gün - Yapay Zeka Modeli Entegrasyonu
- ML.NET kütüphanesinin projeye eklenmesi
- `Cls_ImageAnalysis.cs` sınıfının oluşturulması
- Görüntü karşılaştırma algoritmaları için yapının hazırlanması
- Basit bir görüntü fark analizi modelinin entegre edilmesi
- Piksel değişimlerini tespit edip ısı haritası oluşturan fonksiyonların eklenmesi
- Gizli veri yoğunluğunu tahmin eden yapay zeka algoritmasının uygulanması

### 4. Gün - Analiz Paneli Geliştirme
- `FrmAnalysis.cs` sınıfının düzenlenmesi
- Görüntü karşılaştırma sonuçlarını gösteren UI bileşenlerinin eklenmesi
- Orijinal ve şifrelenmiş görüntü için yan yana görüntüleme panelinin hazırlanması
- Isı haritası görselleştirme kontrollerinin eklenmesi
- Wavelet katsayılarının görsel analizini yapan grafiklerin eklenmesi
- Şifreleme gücünü ve dayanıklılığını gösteren metriklerin eklenmesi

### 5. Gün - Şifre Analizi ve Kanıt Sistemi
- Görüntüden çıkarılan anahtarları görselleştirme özelliğinin eklenmesi
- Kullanılan şifreleme algoritmalarını tespit eden fonksiyonların yazılması
- Şifreleme gücü analizi ve rapor oluşturma özelliklerinin eklenmesi
- Elde edilen şifreleme bilgilerini kanıt olarak raporlayan sistemin eklenmesi
- Tüm analizlerin PDF/HTML formatında dışa aktarılmasının sağlanması
- Analiz sonuçlarını görsel ve sayısal olarak değerlendiren özet panelinin eklenmesi

### 6. Gün - Test ve Hata Ayıklama
- Kapsamlı test senaryolarının hazırlanması ve uygulanması
- Farklı boyut ve formattaki görüntülerle testlerin yapılması
- Performans ve bellek optimizasyonunun yapılması
- Kullanıcı geri bildirimlerine göre UI iyileştirmeleri
- Çökme ve hata durumlarını ele alan kodların güçlendirilmesi
- Tüm fonksiyonların dokümantasyonunun tamamlanması

### 7. Gün - Bilimsel Raporlama ve Son Rötuşlar
- TÜBİTAK için bilimsel rapor hazırlanması
- Algoritma karşılaştırmaları ve performans metriklerinin derlenmesi
- Kullanım kılavuzunun oluşturulması
- Son hatalar ve iyileştirmeler
- Derleme ve kurulum paketi oluşturma
- Proje sunumu için demo hazırlama

## Teknik Notlar

### Wavelet Entegrasyonu

```csharp
// Accord.NET veya AForge.NET ile Wavelet Dönüşümü
using Accord.Math.Transforms;
// veya
using AForge.Imaging;

// Wavelet Dönüşümü uygulama
public byte[] ApplyWaveletTransform(Bitmap image) {
    // Görüntüyü gri tona çevir veya RGB kanalları üzerinde ayrı ayrı çalış
    // Haar dalgacık dönüşümü uygula
    // Katsayıları değiştirerek veri göm
    // Ters dönüşüm uygula
}
```

### ML.NET Entegrasyonu

```csharp
// ML.NET ile görüntü analizi
using Microsoft.ML;
using Microsoft.ML.Vision;

// Görüntü farkı analizi
public Bitmap AnalyzeImageDifference(Bitmap original, Bitmap encrypted) {
    // Piksel farklarını hesapla
    // Isı haritası oluştur
    // Anomali tespiti yap
}
```

### Analiz Raporu Oluşturma

```csharp
public void GenerateAnalysisReport(string outputPath) {
    // Analiz sonuçlarını topla
    // PDF veya HTML raporu oluştur
    // Görselleştirmeleri ekle
}
```

## YAPILACAKLAR LİSTESİ

### Acil Tamamlanması Gerekenler (1-2 Gün)

1. **BusinessLayer Tamamlanması**
   - Wavelet dönüşümü için yeni sınıflar oluştur ve uygula:
     - `Cls_WaveletEncrypt.cs`
     - `Cls_WaveletDecrypt.cs`
   - AES ve RSA entegrasyonunu kontrol et/tamamla

2. **UI Katmanı Tamamlanması**
   - `FrmMenu.cs` içindeki şifreleme ve şifre çözme butonlarının işlevlerini tamamla
   - Hata işleme ve kullanıcı bildirimlerini ekle

3. **Temel Dosya İşlemleri**
   - Görüntü seçme, kaydetme işlemlerini tamamla
   - Şifrelenmiş görüntüleri kaydetme ve yükleme fonksiyonlarını ekle

### Orta Öncelikli İşler (3-4 Gün)

4. **Analiz Paneli Geliştirme**
   - `FrmAnalysis.cs` formunu tamamla
   - ML.NET kütüphanesini entegre et
   - Görüntü karşılaştırma algoritmasını uygula
   - Isı haritası oluşturma fonksiyonunu ekle

5. **Yapay Zeka Entegrasyonu**
   - Basit görüntü fark analizi modelini ekle
   - Şifreleme gücü analizi algoritmasını uygula

6. **Testler ve Hata Ayıklama**
   - Her fonksiyon için birim testleri yaz
   - Farklı boyuttaki görüntülerle end-to-end testler yap
   - Performans ve bellek optimizasyonu yap

### Son Rötuşlar (5-7 Gün)

7. **Dokümantasyon ve Raporlama**
   - Analiz sonuçlarını PDF/HTML olarak dışa aktarma özelliği ekle
   - Kullanım kılavuzu hazırla
   - Bilimsel rapor hazırla (TÜBİTAK için)

8. **Dağıtım Hazırlığı**
   - Derleme ve kurulum paketi oluştur
   - Proje sunumu için demo hazırla

### Örnek Kod Parçaları

#### FrmMenu.cs için Şifreleme Butonu
```csharp
private void btnEncrypt_Click(object sender, EventArgs e) {
    try {
        // Metin kontrolü
        if (string.IsNullOrEmpty(txtInput.Text))
            throw new ArgumentException(Errors.ERROR_TEXT_EMPTY);
            
        // Parola kontrolü
        if (string.IsNullOrEmpty(txtPassword.Text))
            throw new ArgumentException(Errors.ERROR_PASSWORD_EMPTY);
            
        // Görüntü kontrolü
        if (selectedImagePath == string.Empty)
            throw new ArgumentException(Errors.ERROR_IMAGE_NOT_SELECTED);
        
        // AES şifreleme
        string encryptedText = aesEncrypt.EncryptTextToBase64(txtInput.Text, txtPassword.Text);
        
        // AES anahtarını al ve RSA ile şifrele
        byte[] aesKey = aesEncrypt.GetLastAesKey();
        byte[] encryptedKey = rsaEncrypt.EncryptData(aesKey);
        
        // Şifrelenmiş metni ve anahtarı Wavelet dönüşümü ile görüntüye gizle
        Bitmap resultImage = waveletEncrypt.HideData(
            new Bitmap(selectedImagePath), 
            encryptedText, 
            Convert.ToBase64String(encryptedKey)
        );
        
        // Sonucu göster
        pboxImage.Image = resultImage;
        
        // Kullanıcıya bilgi ver
        MessageBox.Show(Success.SUCCESS_ENCRYPTION_COMPLETED, "Başarılı", 
                      MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        // Kaydetme seçeneği
        SaveResultImage(resultImage);
    }
    catch (Exception ex) {
        MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

#### Wavelet Sınıfı İçin Temel Uygulama
```csharp
public class Cls_WaveletEncrypt {
    // DWT (Ayrık Dalgacık Dönüşümü) parametreleri
    private const string WaveletType = "Haar";
    private const int DecompositionLevel = 3;
    
    public Bitmap HideData(Bitmap originalImage, string encryptedText, string encryptedKey) {
        // Veri boyutunu kontrol et
        if (IsDataTooLarge(originalImage, encryptedText, encryptedKey))
            throw new ArgumentException(Errors.ERROR_DATA_TOO_LARGE);
            
        // Görüntüyü kopyala
        Bitmap resultImage = new Bitmap(originalImage);
        
        // Her renk kanalı için (R, G, B) ayrı ayrı DWT uygula
        // Kırmızı kanal için örnek
        double[,] redChannel = GetChannel(resultImage, 0); // Kırmızı kanal
        double[,] transformedRed = ApplyDWT(redChannel);
        
        // Veriyi katsayılara gizle
        // Genellikle yüksek frekans bandı (HH) tercih edilir
        transformedRed = HideDataInCoefficients(transformedRed, encryptedText);
        
        // Ters DWT uygula
        double[,] recoveredRed = ApplyInverseDWT(transformedRed);
        
        // Kanalı görüntüye yaz
        SetChannel(resultImage, 0, recoveredRed);
        
        // Yeşil ve mavi kanallar için benzer işlemler...
        // ...
        
        return resultImage;
    }
    
    // Yardımcı metotlar...
}
```

#### Görüntü Analizi İçin ML.NET Entegrasyonu
```csharp
public class Cls_ImageAnalysis {
    private MLContext mlContext;
    
    public Cls_ImageAnalysis() {
        mlContext = new MLContext(seed: 1);
    }
    
    public Bitmap CreateHeatmap(Bitmap original, Bitmap steganographic) {
        // İki görüntü arasındaki farkları hesapla
        Bitmap heatmap = new Bitmap(original.Width, original.Height);
        
        // Her piksel için karşılaştırma yap
        for (int x = 0; x < original.Width; x++) {
            for (int y = 0; y < original.Height; y++) {
                Color originalColor = original.GetPixel(x, y);
                Color stegoColor = steganographic.GetPixel(x, y);
                
                // Renk farklarını hesapla
                int diffR = Math.Abs(originalColor.R - stegoColor.R);
                int diffG = Math.Abs(originalColor.G - stegoColor.G);
                int diffB = Math.Abs(originalColor.B - stegoColor.B);
                
                // Farkın derecesine göre ısı haritası oluştur
                // Kırmızı = Çok değişiklik, Mavi = Az değişiklik
                Color heatColor = CalculateHeatColor(diffR, diffG, diffB);
                heatmap.SetPixel(x, y, heatColor);
            }
        }
        
        return heatmap;
    }
    
    // ML.NET ile şifreleme gücü analizi
    public double AnalyzeEncryptionStrength(string encryptedText) {
        // ML.NET ile basit bir model kullanarak şifreleme entropisini değerlendir
        // ...
        return 0.0; // Puan (0-1 arası)
    }
}