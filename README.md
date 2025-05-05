# Görsel Tabanlı Şifreleme Sistemi (Image Based Encryption System)

## Proje Özeti

Bu proje, görsel tabanlı bir şifreleme sistemi geliştirerek verilerin güvenli bir şekilde saklanması ve iletilmesi amacıyla tasarlanmıştır. Sistem, çeşitli kriptografik algoritmalar kullanarak metin verilerini şifrelemekte ve bu şifrelenmiş verileri görseller içerisine LSB (Least Significant Bit) steganografi tekniği ile gizlemektedir. Bu yöntem ile hem veri güvenliği sağlanmakta hem de steganografi tekniklerinden faydalanılarak şifrelenmiş verilerin fark edilebilirliği azaltılmaktadır.

## Proje Mimarisi

Proje, katmanlı mimari kullanılarak geliştirilmiştir:

1. **BusinessLayer**: Şifreleme ve veri gizleme mantığını içerir
2. **DataLayer**: Yapılandırma ve veri yönetimini sağlar
3. **TypeLayer**: Hata ve başarı mesajları için tip tanımlamaları
4. **UI Katmanı**: Kullanıcı arayüzü bileşenlerini içerir

## Kullanılan Teknolojiler

### Programlama Dili ve Framework
- **C#**: Ana programlama dili
- **.NET Framework**: Uygulama çerçevesi
- **Windows Forms**: Kullanıcı arayüzü için
- **Guna.UI2.WinForms**: Modern ve etkileşimli UI bileşenleri için

### Kriptografik Algoritmalar ve Kütüphaneler
- **AES (Advanced Encryption Standard)**: 256-bit şifreleme anahtarı ile simetrik şifreleme
- **RSA (Rivest-Shamir-Adleman)**: 3072-bit anahtar uzunluğu ile asimetrik şifreleme (BouncyCastle kütüphanesi ile)
- **LSB (Least Significant Bit)**: Görsellerin içerisine veri gizleme için kullanılan steganografi tekniği
- **BouncyCastle**: Güçlü kriptografik işlemler için kullanılan açık kaynaklı kütüphane

## Temel Bileşenler ve İşlevleri

### Şifreleme Modülleri

#### AES Şifreleme (Cls_AesHelper.cs)
- Kullanıcı tarafından girilen parola ile 256-bit AES anahtarı oluşturulur
- Parolaya dayalı anahtar türetme işlemi ile aynı parola her zaman aynı anahtarı üretir
- Metin verisi bu anahtar ile şifrelenir

#### RSA Şifreleme (Cls_RsaHelper.cs)
- Config.json dosyasındaki SystemIdentity değerine bağlı olarak BouncyCastle kütüphanesi kullanılarak 3072-bit RSA anahtar çifti oluşturulur
- AES anahtarları RSA ile şifrelenerek ek bir güvenlik katmanı sağlanır
- Aynı SystemIdentity değeri her zaman aynı RSA anahtar çiftini üretir, böylece farklı oturumlarda tutarlılık sağlanır

#### LSB Tabanlı Veri Gizleme (Cls_LsbHelper.cs)
- Least Significant Bit (En Önemsiz Bit) tekniği kullanılarak şifrelenmiş veriler görsellerin içine gizlenir
- Görsellerin her pikselindeki renk bileşenlerinin (R, G, B) en düşük değerli bitlerini değiştirerek veri gizlenir
- Veri gizlenmiş görsele özel bir imza (ZORLU) eklenerek daha sonra tanınabilirliği sağlanır
- Görsel üzerinde yapılan değişiklikler insan gözüyle fark edilemeyecek düzeydedir

### Yapılandırma ve Yönetim

#### Yapılandırma Yönetimi (Cls_Config.cs)
- Config.json.secure dosyası üzerinden sistem ayarları yönetilir
- Varsayılan değerler ile ilk çalıştırmada otomatik yapılandırma oluşturulur
- Yapılandırma verileri AES ile şifrelenerek saklanır

#### Geliştirici Modu (Cls_DeveloperMode.cs)
- Sistem üzerinde gelişmiş işlemler yapabilmek için geliştirici modu bulunur
- Sadece yetkili kimlik bilgileri ile erişilebilir
- Kimlik yönetimi ve analiz işlemleri için ek özellikler sunar

#### Kimlik Yönetimi (Cls_IdentityCreate.cs)
- Sistem için benzersiz kimlikler oluşturulabilir
- Bu kimlikler RSA anahtar çiftlerinin oluşturulmasında kullanılır
- Min. 10, Max. 50 karakter uzunluğunda, büyük-küçük harfler, rakamlar ve özel karakterlerden oluşabilir

## Kullanıcı Arayüzü

### FrmMenu (Ana Sayfa)
- Resim seçme, metin girme ve şifreleme/şifre çözme işlemlerini gerçekleştirme
- Şifre verisi için parola belirleme
- Geliştirici modu ve diğer sayfalara erişim

### FrmAdmin (Yönetim Paneli)
- Sistem kimliği yönetimi (Değiştirme, rastgele oluşturma)
- RSA anahtar çiftlerinin yeniden yapılandırılması
- Gelişmiş sistem ayarları

### FrmAnalysis (Analiz Paneli)
- Veri gizlenmiş görsellerin analizi
- Orijinal görselle farkların tespit edilmesi
- Görsel üzerindeki değişikliklerin çeşitli yöntemlerle görselleştirilmesi

### FrmLogin (Giriş Sayfası)
- Geliştirici kimliği ve parola ile güvenli giriş
- Yetkilendirme yönetimi

### FrmInfo (Bilgi Sayfası)
- Sistem hakkında genel bilgiler
- Kullanım kılavuzu ve yardım içeriği

## Güvenlik Özellikleri

1. **Çoklu Şifreleme Katmanları**: 
   - Metin verisi AES ile şifrelenir
   - AES anahtarı RSA ile şifrelenir
   - Tüm şifrelenmiş veri görsele LSB tekniği ile gizlenir

2. **Güvenli Anahtar Yönetimi**:
   - Parolalar doğrudan kullanılmaz, anahtar türetme fonksiyonları kullanılır
   - RSA anahtar çiftleri BouncyCastle kütüphanesi ile sistem kimliğine bağlı olarak oluşturulur
   - Yapılandırma dosyaları şifreli saklanır

3. **Veri Bütünlüğü Kontrolü**:
   - Şifrelenmiş görseller özel imza (ZORLU) ile işaretlenir
   - Şifre çözme işlemi öncesi imza kontrolü yapılır

4. **Yetkisiz Erişim Koruması**:
   - Geliştirici özellikleri yalnızca yetkili kullanıcılar tarafından erişilebilir
   - Oturum yönetimi ve güvenli giriş mekanizmaları

## Hata Yönetimi

- TypeLayer katmanında tanımlanan hata ve başarı mesajları
- Kategorize edilmiş ve detaylı hata açıklamaları
- Try-Catch blokları ile güvenli hata yakalama ve kullanıcı bilgilendirme

## Teknik Gereksinimler

### Minimum Sistem Gereksinimleri
- İşletim Sistemi: Windows 10 veya üzeri
- .NET Framework 4.7.2 veya üzeri
- 4 GB RAM
- 100 MB boş disk alanı
- 1366x768 ekran çözünürlüğü

### Bağımlılıklar
- BouncyCastle.NetFramework 1.8.5 (kriptografik işlemler için)
- Guna.UI2.WinForms (kullanıcı arayüzü için)
- Newtonsoft.Json (yapılandırma yönetimi için)

## Kurulum ve Kullanım

1. Uygulamayı çalıştırdıktan sonra ana menü (FrmMenu) üzerinden resim seçme işlemi yapılır
2. Şifrelenecek metin ve parola girişi yapılır
3. "Şifrele" butonuna basılarak metin şifrelenir ve seçilen görsele gizlenir
4. Şifre çözmek için, veri gizlenmiş görsel seçilir ve doğru parola girilerek "Şifre Çöz" butonuna basılır
5. Çözülen metin ekranda görüntülenir

## LSB Steganografi Tekniği

LSB (Least Significant Bit - En Önemsiz Bit) steganografi tekniği, dijital görsellerdeki her pikselin renk bileşenlerinin (Kırmızı, Yeşil, Mavi) en düşük değerli bitlerini değiştirerek verileri gizlemeye dayanır. Bu teknik, görselde insan gözüyle fark edilemeyecek minimal değişiklikler yaparak büyük miktarda veri gizlenmesine olanak tanır.

Projemizde kullanılan LSB steganografi algoritması şu özelliklere sahiptir:
- Her pikselin R, G, B değerlerinin her birinin en düşük değerli biti değiştirilerek veri gizlenir
- Veri sonu için özel bir işaretçi (endMarker) kullanılır
- Görsel tanımlama için ilk 20 piksele "ZORLU" imzası yerleştirilir
- Veri çıkarma işlemi, önce imza kontrolü yaparak devam eder

## BouncyCastle ile RSA Şifreleme

Projede kullanılan RSA şifreleme, açık kaynaklı BouncyCastle kriptografi kütüphanesi ile gerçekleştirilmektedir. Bu kütüphane ile:
- Deterministik RSA anahtar çiftleri oluşturulur (aynı giriş her zaman aynı anahtarı üretir)
- 3072-bit uzunluğunda güçlü RSA anahtarları kullanılır
- PEM formatında anahtar dışa aktarımı desteklenir
- Yüksek güvenlikli şifreleme işlemleri gerçekleştirilir

## Geliştirici Notları

- Sistemde geliştirici modu, gelişmiş analizler ve kimlik yönetimi için kullanılabilir
- Kimlik değişiklikleri RSA anahtar çiftlerini değiştirir, bu nedenle dikkatli kullanılmalıdır
- Yeni özellikler eklenirken mevcut kod yapısı ve mimari prensiplerine uyulması önerilir

## Proje Ekibi

Bu proje, TÜBİTA'a sunmak üzere geliştirilmiştir. Proje ekibi, İzmir Kanuni Sultan Süleyman Anadolu Lisesi 12/A öğrencilerinden oluşmaktadır.

## Lisans

Bu proje, TÜBİTAK projesi kapsamında geliştirilmiştir. Tüm hakları saklıdır.

---

**Not**: Bu README dosyası, projenin genel yapısı ve temel özellikleri hakkında bilgi vermek amacıyla hazırlanmıştır. Daha detaylı teknik dokümantasyon için ilgili sınıf dosyalarına ve yorum satırlarına bakınız.
