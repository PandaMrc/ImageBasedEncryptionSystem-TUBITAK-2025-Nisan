# ImageBasedEncryptionSystem

Görüntü tabanlı çok katmanlı güvenli veri gizleme sistemi.

## Proje Özeti

ImageBasedEncryptionSystem, dijital görüntülerde steganografi teknikleri kullanarak şifrelenmiş verileri güvenli bir şekilde saklamak için geliştirilmiş çok katmanlı bir güvenlik çözümüdür. Sistem, simetrik ve asimetrik şifreleme algoritmalarını birleştirerek yüksek güvenlik sağlarken aynı zamanda LSB (Least Significant Bit) steganografi tekniğini kullanarak verilerin görsel içerikler içerisine görünmez bir şekilde gizlenmesini sağlar.

## Mimari Yapı

Proje, modüler ve katmanlı bir yapıya sahiptir:

### 1. TypeLayer

Sistem genelinde kullanılan veri tipleri, hata ve başarı mesajlarını içerir:

- **Success.cs**: Başarı durum mesajlarını içeren sabitler
- **Errors.cs**: Hata mesajlarını içeren sabitler
- **Debug.cs**: Debug işlemleri için kullanılan sabitler

### 2. DataLayer

Yapılandırma ayarlarını yöneten ve sistem yapılandırması ile ilgili veri erişimini sağlayan katman:

- **Cls_Config.cs**: Config.json dosyasından ayarları okuma ve yönetme işlevleri
- **Config.json**: Sistem yapılandırma değerleri

### 3. BusinessLayer

Projenin ana işlevselliğini sağlayan ve iş mantığını içeren katman:

#### 3.1. Helpers (Şifreleme Yardımcıları)

- **Cls_LsbHelper.cs**: LSB steganografi tekniği ile veri gizleme ve çıkarma işlevleri
  - `EmbedData()`: Veriyi resmin LSB'lerine gizler
  - `ExtractData()`: Resimden gizlenmiş veriyi çıkarır
  - `EmbedSignature()`: Resme bir imza gizler
  - `CheckSignature()`: Resimdeki imzayı kontrol eder

- **Cls_AesHelper.cs**: AES şifreleme işlemleri
  - `GenerateAESKey()`: Girilen metinden SHA-256 ile AES anahtarı oluşturur
  - `Encrypt()`: Metni AES ile şifreler
  - `Decrypt()`: Şifreli metni AES ile çözer

- **Cls_RsaHelper.cs**: RSA şifreleme işlemleri
  - `EnsureKeyPair()`: Deterministik RSA anahtar çifti oluşturur
  - `Encrypt()`: RSA public key ile veri şifreler
  - `Decrypt()`: RSA private key ile veri çözer
  - `GetPublicKeyPem()`, `GetPrivateKeyPem()`: Anahtarları PEM formatında dışa aktarır

#### 3.2. AnalysisHelpers (Analiz Yardımcıları)

- **Cls_ComprehensiveAnalyser.cs**: Steganaliz işlemlerini yöneten ana sınıf
  - `AnalyzeImageAsync()`: Steganaliz işlemlerini asenkron olarak gerçekleştirir
  - `ExtractDataFromEncryptedImage()`: Şifreli görsellerden veri çıkarır

- **Cls_RsHelper.cs**: RS (Regular-Singular) steganografi analizi
  - `PerformRegularSingularAnalysis()`: Görüntüdeki düzenli ve tekil piksellerin dağılımını analiz eder

- **Cls_SpaHelper.cs**: SPA (Sample Pair Analysis) steganografi analizi
  - `PerformSpaAnalysis()`: Piksel çiftlerini analiz ederek gizli veri varlığını tespit eder

- **Cls_ChangeMapHelper.cs**: İki görsel arasındaki değişimleri analiz eder
  - `GenerateChangeMap()`: Orijinal ve stego görüntü arasındaki farklılıkları görselleştirir

- **Cls_BitPlaneHelper.cs**: Bit düzlemi analizi
  - `GenerateBitPlanes()`: Her renk kanalının bit düzlemlerini ayrı ayrı oluşturur

- **Cls_HTMLReportGenerator.cs**: Analiz sonuçlarını HTML raporuna dönüştürür
  - `GenerateAnalysisReport()`: Kapsamlı analiz raporunu HTML olarak oluşturur

#### 3.3. Diğer İş Katmanı Sınıfları

- **Cls_DeveloperMode.cs**: Geliştirici modunu yöneten sınıf
- **Cls_IdentityCreate.cs**: Sistem kimliğini oluşturan sınıf
- **ExtractedData.cs**: Çıkarılan veri nesnesi

### 4. UI Layer

Kullanıcı arayüzünü oluşturan formlar ve bileşenler:

- **FrmMenu.cs**: Ana uygulama penceresi
- **FrmAnalysis.cs**: Steganaliz ekranı
- **FrmInfo.cs**: Bilgi ekranı
- **FrmAdmin.cs**: Yönetici paneli
- **FrmLogin.cs**: Giriş ekranı

## Teknik Detaylar

### 1. LSB Steganografi Tekniği

LSB (Least Significant Bit) tekniği, dijital resimdeki her pikselin en az anlamlı bitlerinin (8 bit içindeki en sağdaki bitler) değiştirilmesiyle veri gizlenmesini sağlar. Bu teknik, insan gözünün algılayamayacağı şekilde görüntüde minimal değişiklikler yapar.

**Teknik Optimizasyonlar:**

- **Doğrudan Bellek Erişimi**: `System.Drawing.Imaging.BitmapData.LockBits()` ve `Marshal.Copy()` kullanılarak, piksellere doğrudan bellek erişimi sağlanır. Bu yaklaşım, `GetPixel()` ve `SetPixel()` metodlarına göre çok daha hızlı çalışır.

- **Çok Kanallı Veri Gizleme**: Her pikselin R, G ve B renk kanallarının LSB'leri kullanılır, böylece piksel başına 3 bit veri gizlenebilir.

- **İmza ve Bitiş İşaretçileri**: Verinin başına özel bir imza, sonuna ise bitiş işaretçisi eklenir. Bu sayede veri bütünlüğü kontrolü ve doğru çıkarım sağlanır.

```csharp
// LSB veri gizleme örneği (yalnızca konsept gösterimi)
private static byte EmbedBit(byte colorComponent, int bit)
{
    // Son biti temizle (AND ...11111110)
    colorComponent = (byte)(colorComponent & 0xFE);
    // Yeni biti ekle (OR 0000000(0 veya 1))
    return (byte)(colorComponent | bit);
}
```

**Veri Kapasitesi:**
1024x768 boyutunda bir resim şu kapasiteye sahiptir:
- Piksel sayısı: 1024 × 768 = 786,432
- Kullanılabilir bit sayısı: 786,432 × 3 (RGB) = 2,359,296 bit
- Kullanılabilir bayt sayısı: 2,359,296 ÷ 8 = 294,912 bayt ≈ 288 KB

### 2. Çok Katmanlı Şifreleme

Sistem, verinin güvenliği için birden fazla şifreleme katmanı kullanır:

#### 2.1. SHA-256 ile AES Anahtar Türetimi

Kullanıcı şifresinden güçlü bir AES-256 anahtarı türetilir:

```csharp
public static byte[] GenerateAESKey(string keySource)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(keySource);
        byte[] hash = sha256.ComputeHash(keyBytes);
        return hash;  // AES-256 için 32 byte anahtar
    }
}
```

#### 2.2. AES-256 ile Metin Şifreleme

Orijinal metin AES-256 algoritması ile şifrelenir:

```csharp
public static string Encrypt(string plainText, byte[] aesKey)
{
    using (var aesAlg = System.Security.Cryptography.Aes.Create())
    {
        aesAlg.Key = aesKey;
        aesAlg.IV = IV; // Sabit IV değeri

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using (var msEncrypt = new MemoryStream())
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
            return Convert.ToBase64String(msEncrypt.ToArray());
        }
    }
}
```

#### 2.3. RSA-3072 ile AES Anahtarı Şifreleme

AES anahtarı, RSA-3072 public key ile şifrelenir. RSA anahtar çifti, sistem kimliğine (SystemIdentity) bağlı olarak deterministik bir şekilde üretilir:

```csharp
public static string Encrypt(string plainText)
{
    EnsureKeyPair();

    var encryptEngine = new Org.BouncyCastle.Crypto.Engines.RsaEngine();
    encryptEngine.Init(true, _keyPair.Public);

    var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
    var encrypted = encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);

    return Convert.ToBase64String(encrypted);
}
```

### 3. Steganaliz Özellikleri

Sistem, steganografi ile gizlenmiş verileri tespit etmek için çeşitli steganaliz metotları içerir:

#### 3.1. RS (Regular-Singular) Analizi

RS Analizi, görüntüdeki pikselleri düzenli (regular) ve tekil (singular) gruplara ayırarak, LSB düzleminde yapılan değişiklikleri tespit eder. Gizli veri içeren görüntülerde, bu grupların dağılımı belli bir şekilde bozulur.

```csharp
public static RsAnalysisResult PerformRegularSingularAnalysis(Bitmap image, ColorChannel channel, int groupSize)
{
    // Görüntüyü gruplara ayır
    // Düzenli ve tekil grupları tespit et
    // Orjinal ve fliplenen LSB değerleri için dağılımları hesapla
    // Tahmini gömme oranını hesapla
    // ...
}
```

#### 3.2. SPA (Sample Pair Analysis)

SPA, piksel çiftleri arasındaki istatistiksel ilişkileri kullanarak LSB steganografiyi tespit eder. Doğal görüntülerde piksel çiftleri arasında belli istatistiksel özellikler vardır ve LSB steganografi bu özellikleri bozar.

```csharp
public static SpaAnalysisResult PerformSpaAnalysis(Bitmap image, ColorChannel channel)
{
    // Piksel çiftlerini belirle
    // İstatistiksel özellikleri hesapla
    // Gömme oranını tahmin et
    // ...
}
```

#### 3.3. Değişim Haritası (Change Map)

Orijinal görüntü ve stego görüntü karşılaştırılarak, değişen piksellerin görsel bir haritası oluşturulur. Bu harita, değişikliklerin yoğunluğunu ve desenini gösterir.

```csharp
public static Bitmap GenerateChangeMap(Bitmap originalImage, Bitmap stegoImage, ColorChannel channel)
{
    // İki görüntü arasındaki farklılıkları tespit et
    // Farklılıkları görselleştiren bir bitmap oluştur
    // ...
}
```

#### 3.4. Bit Düzlemi Analizi

Her renk kanalının (R, G, B) bit düzlemleri ayrı ayrı görselleştirilir. Bu sayede insan gözüne görünmeyen anomaliler tespit edilebilir.

```csharp
public static Bitmap[] GenerateBitPlanes(Bitmap image, ColorChannel channel)
{
    // Her bit düzlemi için ayrı görüntü oluştur
    // Bit düzlemini görselleştir
    // ...
}
```

### 4. Şifreleme ve Deşifreleme İş Akışı

#### 4.1. Şifreleme İş Akışı

1. Kullanıcı, şifrelenecek metni ve şifreyi girer
2. Sistemin RSA anahtar çifti, makine kimliğine göre oluşturulur
3. SHA-256 ile şifreden AES-256 anahtarı türetilir
4. Metin, AES-256 ile şifrelenir
5. AES anahtarı, RSA public key ile şifrelenir
6. AES şifreli metin ve RSA şifreli anahtar birleştirilerek hazırlanır
7. Birleştirilmiş veri, LSB tekniği ile görüntüye gizlenir
8. Şifreli görüntü kaydedilir

#### 4.2. Deşifreleme İş Akışı

1. Kullanıcı, şifreli görüntüyü ve şifreyi girer
2. LSB tekniği ile görüntüden gizli veri çıkarılır
3. Çıkarılan veri, AES şifreli metin ve RSA şifreli anahtar olarak ayrıştırılır
4. SHA-256 ile şifreden AES-256 anahtarı türetilir
5. RSA private key kullanılarak RSA şifreli anahtar çözülür
6. Çözülen AES anahtarı ile AES şifreli metin çözülür
7. Orijinal metin kullanıcıya gösterilir

### 5. Güvenlik Mekanizmaları

#### 5.1. Güvenli Anahtar Yönetimi

- **Deterministik RSA Anahtar Üretimi**: RSA anahtar çifti, sistem kimliğinden türetilir, böylece aynı makinede her zaman aynı anahtar çifti üretilir.
- **Anahtar Saklama**: Anahtarlar hafızada tutulur, disk üzerinde saklanmaz.
- **Perfect Forward Secrecy**: Her mesaj için farklı bir AES anahtarı kullanılabilir.

#### 5.2. Saldırı Senaryolarına Karşı Dayanıklılık

- **Kaba Kuvvet Saldırıları**: AES-256 ve RSA-3072 kullanımı ile 2^256 ve 2^3072 olası anahtar kombinasyonu.
- **İstatistiksel Steganaliz**: Optimizasyon teknikleri ile tespit edilme olasılığını minimize etme.
- **Görüntü Manipülasyon Saldırıları**: İmza mekanizması ile manipüle edilmiş görüntüleri tespit etme.

#### 5.3. Sınırlamalar ve Zayıf Noktalar

- **Sıkıştırma Saldırıları**: LSB steganografi sıkıştırma (örn. JPEG) işlemleri sırasında kaybolabilir.
- **Maksimum Veri Boyutu**: Görüntü boyutuna bağlı olarak sınırlı veri gizleme kapasitesi.
- **Meta-veri Analizi**: Görüntü meta verileri manipülasyonu gösterebilir.

### 6. Performans Optimizasyonları

- **Doğrudan Bellek Erişimi**: LockBits() kullanımı ile hızlı piksel erişimi.
- **Paralel İşleme**: Analiz işlemleri için Task-based paralel işleme.
- **Bellek Yönetimi**: Büyük veri işleme için optimize edilmiş bellek kullanımı.

### 7. Kullanılan Teknolojiler ve Kütüphaneler

- **.NET Framework**: Ana geliştirme platformu
- **BouncyCastle**: RSA şifreleme için kriptografi kütüphanesi
- **Guna.UI2**: Modern kullanıcı arayüzü bileşenleri
- **System.Drawing**: Görüntü işleme
- **Newtonsoft.Json**: JSON işleme

## Kurulum ve Kullanım

### Sistem Gereksinimleri

- Windows 7 veya üzeri
- .NET Framework 4.7.2 veya üzeri
- 2 GB RAM (minimum)
- 100 MB disk alanı

### Kurulum Adımları

1. Projeyi derleyin veya yayınlanmış setup dosyasını çalıştırın
2. Uygulamayı başlatın
3. (İlk çalıştırmada) Sistem otomatik olarak gerekli yapılandırma dosyalarını oluşturacaktır

### Temel Kullanım

#### Veri Şifreleme ve Görüntüye Gizleme

1. Ana menüden "Resim Seç" butonuna tıklayın ve bir görüntü dosyası seçin
2. Şifrelenecek metni metin kutusuna yazın
3. Şifre belirleyin
4. "Şifrele" butonuna tıklayın
5. Şifreli görüntüyü kaydedin

#### Şifreli Görüntüden Veri Çıkarma

1. Ana menüden "Resim Seç" butonuna tıklayın ve şifreli görüntüyü seçin
2. Şifreyi girin
3. "Şifre Çöz" butonuna tıklayın
4. Çözülen metin görüntülenecektir

#### Steganaliz İşlemleri

1. "Analiz" butonuna tıklayın
2. Analiz edilecek görüntüyü seçin
3. İsterseniz karşılaştırma için orijinal görüntüyü de seçin
4. Analiz parametrelerini belirleyin
5. "Analiz Başlat" butonuna tıklayın
6. Analiz sonuçları görüntülenecektir

### Geliştirici Modu

Gelişmiş ayarlara erişmek için geliştirici modunu etkinleştirin:

1. Login butonuna tıklayın
2. Geliştirici şifresini girin
3. Geliştirici modu etkinleştikten sonra ek araçlara ve ayarlara erişim sağlanır

## Gelecek Planları

- Video dosyaları için steganografi desteği
- Derin öğrenme tabanlı steganografi modülleri
- Mobil platformlar için uygulama
- Quantum bilgi işlem saldırılarına karşı dayanıklılık

## Lisans

Bu projenin lisans bilgileri için LICENSE dosyasına bakınız.

## İletişim

Proje ile ilgili sorularınız için [iletişim-bilgileri] adresinden iletişime geçebilirsiniz. 