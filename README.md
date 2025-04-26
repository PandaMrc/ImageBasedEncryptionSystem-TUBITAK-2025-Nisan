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
- UI güncelleme: Wavelet opsiyonunun eklenmesi, LSB'nin alternatif olarak tutulması
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
- Kullanıcı kılavuzunun oluşturulması
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