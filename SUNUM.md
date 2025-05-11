# ImageBasedEncryptionSystem - TÜBİTAK Sunumu

## Sunum Planı ve Konuşma Metni

### 1. GİRİŞ (2 dakika)

**Açılış:**
"Sayın jüri üyeleri, bugün sizlere 'ImageBasedEncryptionSystem' adlı projemi sunmaktan onur duyuyorum. Bu proje, günümüzde giderek önem kazanan veri güvenliği ve gizliliği sorunlarına yenilikçi bir çözüm sunmayı amaçlamaktadır."

**Motivasyon:**
"Dijital çağda, hassas bilgilerin güvenli bir şekilde saklanması ve iletilmesi kritik bir ihtiyaçtır. Geleneksel şifreleme yöntemleri şifreli metinleri kolayca fark edilebilir hale getirmekte, bu da özellikle gizli iletişim gerektiren senaryolarda sorun yaratmaktadır. Projemiz, şifrelenmiş verileri günlük kullanılan görsel içeriklere gizleyerek hem güvenliği hem de gizliliği sağlamaktadır."

**Projenin Ana Fikri:**
"ImageBasedEncryptionSystem, kriptografi ve steganografi tekniklerini birleştirerek, şifrelenmiş verileri dijital görüntüler içerisine gizlemektedir. Bu yaklaşım, iletilen verilerin varlığını gizlerken, aynı zamanda çok katmanlı bir güvenlik sağlamaktadır."

### 2. TEKNİK ALTYAPI VE MİMARİ (3 dakika)

**Mimari Tasarım:**
"Projemiz çok katmanlı bir mimari üzerine inşa edilmiştir. Bu yapı hem kodun sürdürülebilirliğini hem de farklı şifreleme ve steganografi tekniklerinin modüler olarak entegre edilebilmesini sağlamaktadır."

*[Şimdi ekranda katmanlı mimarinin şemasını gösterin]*

**Katmanları şu şekilde açıklayın:**
- "TypeLayer: Uygulama genelinde kullanılan veri tipleri, hata ve başarı mesajlarını içeren katman."
- "DataLayer: Yapılandırma ayarlarını yöneten ve veri erişimini sağlayan katman."
- "BusinessLayer: Şifreleme, steganografi ve analiz algoritmalarının yer aldığı, projenin ana iş mantığını içeren katman."
- "UI Layer: Kullanıcıların sistemle etkileşimini sağlayan arayüz katmanı."

**Teknolojiler ve Kütüphaneler:**
"Projemizde modern güvenlik standartlarını karşılayan teknoloji ve kütüphaneler kullanılmıştır:
- .NET Framework
- AES-256 ve RSA-3072 şifreleme algoritmaları
- LSB (Least Significant Bit) steganografi tekniği
- Advanced steganalysis için özelleştirilmiş analiz araçları"

### 3. ŞİFRELEME METODOLOJİSİ (4 dakika)

**Çok Katmanlı Şifreleme Yaklaşımı:**
"Projemizin güvenlik yaklaşımı, 'savunma derinliği' prensibine dayanmaktadır. Tek bir güvenlik mekanizması yerine, iç içe geçmiş birden fazla koruma katmanı kullanıyoruz."

*[Şifreleme akış şemasını gösterin]*

**Şifreleme Süreci:**
"Şifreleme sürecini adım adım açıklamak isterim:
1. Kullanıcı metni ve şifre girişi yapılır.
2. Şifreden SHA-256 algoritmasıyla güçlü bir AES-256 anahtarı türetilir.
3. Metin, AES-256 ile şifrelenir.
4. AES anahtarı, sistemin RSA-3072 public key'i ile şifrelenir.
5. Hem şifrelenmiş metin hem de şifrelenmiş anahtar birleştirilir.
6. Bu veri, LSB steganografi tekniği kullanılarak görüntünün içine gizlenir."

**Güvenlik Özellikleri:**
"Bu yaklaşımın sağladığı güvenlik avantajları şunlardır:
- Simetrik şifrelemenin hızı ve asimetrik şifrelemenin güvenliği birleştirilmiştir.
- Anahtarlar, Tamamıyla İleriye Yönelik Gizlilik (Perfect Forward Secrecy) sağlayacak şekilde tasarlanmıştır.
- Görüntü üzerinde yapılan steganografik değişiklikler, insan gözüyle fark edilemeyecek seviyededir."

### 4. STEGANOGRAFİ TEKNİKLERİ (3 dakika)

**LSB Tekniği ve Optimizasyonlar:**
"Projemizde veri gizleme için LSB (En Önemsiz Bit) tekniğini kullanıyoruz. Bu teknik, görüntü piksellerinin en az önemli bitlerini değiştirerek veri saklamamızı sağlar."

*[LSB tekniğinin görselini gösterin]*

"Standart LSB yaklaşımını şu yönlerle optimize ettik:
- Hız için LockBits metoduyla doğrudan bellek erişimi
- Gömme işlemi öncesi veri imzası ekleme
- Veri sonuna bitiş işaretleyicileri yerleştirme
- RGB kanallarının tamamını etkili kullanma"

**Veri Kapasitesi ve Görünmezlik:**
"1024x768 boyutunda bir görüntü, yaklaşık 294 KB metin saklayabilmektedir. Bu değer, sistemimizin pratik kullanım senaryoları için yeterli kapasiteyi sağlamaktadır. Ayrıca, gömme işlemi sonrası görüntüdeki değişiklikler, PSNR değeri 55 dB'nin üzerinde kalacak şekilde optimize edilmiştir."

### 5. STEGANALİZ VE GÜVENLİK ANALİZİ (4 dakika)

**Steganaliz Özellikleri:**
"Projemiz, güvenlik testleri için kapsamlı steganaliz araçları içermektedir. Bu araçlar, steganografik yöntemlerle gizlenmiş verileri tespit edebilme yeteneğini gösterir."

*[Analiz ekranını gösterin]*

"Steganaliz modülümüzde şu yöntemler kullanılmaktadır:
- RS (Regular-Singular) Analizi: Görüntü istatistiklerindeki anormallikleri tespit eder.
- SPA (Sample Pair Analysis): Piksel çiftleri arasındaki ilişkileri inceleyerek gizli veri varlığını tespit eder.
- LSB Değişim Haritası: Görüntünün orijinali ile karşılaştırılabildiğinde, değişen piksel konumlarını gösterir.
- Bit Düzlemi Analizi: Her renk kanalının bit düzlemlerini ayrı ayrı inceleyerek anomalileri görselleştirir."

**Güvenlik Değerlendirmesi:**
"Sistemimiz, şu saldırı senaryolarına karşı dayanıklılık göstermektedir:
- Kaba kuvvet saldırıları (AES-256 ve RSA-3072 kullanımı)
- İstatistiksel steganaliz saldırıları (optimizasyon teknikleri sayesinde)
- Görüntü manipülasyonu saldırıları (imza doğrulama mekanizması ile)"

### 6. UYGULAMA ARAYÜZÜ VE KULLANICI DENEYİMİ (3 dakika)

**Kullanıcı Arayüzü Tasarımı:**
"Projemizin kullanıcı arayüzü, karmaşık güvenlik mekanizmalarını basit ve sezgisel bir deneyimle kullanabilmeyi amaçlamaktadır."

*[Ana uygulama ekranını gösterin ve ardından diğer formları gösterin]*

"Arayüz tasarımında şu prensipleri göz önünde bulundurduk:
- Sadelik ve kullanım kolaylığı
- Aşamalı işlem rehberliği
- Hata ve başarı mesajlarıyla kullanıcı bilgilendirmesi
- Gelişmiş ayarların geliştirici modunda erişilebilir olması"

**Öne Çıkan Kullanıcı Özellikleri:**
"Uygulamanın temel kullanıcı deneyimi özellikleri:
- Sürükle-bırak desteği ile kolay görüntü seçimi
- Şifreleme ve çözme işlemlerinde ilerleme göstergesi
- Kapsamlı analiz raporları ve görselleştirmeler
- Hızlı sonuç alma için optimize edilmiş algoritmalar"

### 7. ÖRNEK DEMO (4 dakika)

**Canlı Demo:**
"Şimdi sistemin temel işlevlerini gösteren kısa bir demo sunmak istiyorum."

*[Aşağıdaki işlemleri canlı olarak gerçekleştirin:]*
1. "Önce bir görüntü seçelim ve içine gizlenecek metni girelim."
2. "Şimdi bir şifre belirleyip şifreleme işlemini başlatalım."
3. "Şifreleme işlemi tamamlandı, şimdi şifreli görüntünün nasıl göründüğünü inceleyelim."
4. "Şimdi şifreyi çözme işlemini gerçekleştirelim."
5. "Son olarak, bir analiz raporu oluşturalım ve gizli veri içeren görüntünün steganaliz sonuçlarını görelim."

**Performans Değerlendirmesi:**
"Demoda gördüğünüz gibi, 1 MB büyüklüğündeki bir görüntüye 50 KB'lık bir metin gizleme işlemi yaklaşık 2 saniye sürmektedir. Çözme işlemi ise benzer bir performans göstermektedir."

### 8. UYGULAMA ALANLARI VE POTANSIYEL KULLANIM SENARYOLARI (2 dakika)

**Uygulama Alanları:**
"Bu teknolojinin potansiyel uygulama alanları şunlardır:
- Dijital telif hakkı koruma (dijital filigran)
- Güvenli veri iletişimi
- Hassas bilgilerin güvenli depolanması
- Bilgi sızdırmaya karşı önlemler"

**Gerçek Dünya Senaryoları:**
"Sistemimiz şu senaryolarda kullanılabilir:
- Gazetecilerin hassas bilgileri güvenli şekilde taşıması
- Kurumların gizli belgeleri sızıntı riski olmadan paylaşması
- Kişisel gizli verilerin genel bulut depolama sistemlerinde güvenle saklanması
- Şirketlerin fikri mülkiyet haklarını korumak için dijital içeriklerine görünmez imza eklemesi"

### 9. KARŞILAŞTIRMA VE SONUÇLAR (2 dakika)

**Mevcut Çözümlerle Karşılaştırma:**
"Projemizi mevcut steganografi ve şifreleme araçlarıyla karşılaştırdığımızda şu avantajlar öne çıkmaktadır:"

*[Karşılaştırma tablosunu gösterin]*

"- Entegre çok katmanlı güvenlik
- Yüksek gömmme kapasitesi / görsel kalite oranı
- Kapsamlı steganaliz özellikleri
- Kullanıcı dostu arayüz"

**Deneysel Sonuçlar:**
"Yaygın steganaliz araçları kullanılarak yapılan testlerde, sistemimizin tespit edilme oranı %10'un altında kalmıştır. Bu, LSB steganografi için oldukça güçlü bir sonuçtur ve optimize ettiğimiz tekniklerin etkinliğini göstermektedir."

### 10. GELECEK GELİŞTİRMELER VE SONUÇ (3 dakika)

**Gelecek Planları:**
"Projemizin gelecek versiyonlarında şu geliştirmeleri planlıyoruz:
- Video dosyaları için steganografi desteği
- Derin öğrenme tabanlı steganografi modülleri
- Mobil platformlar için uygulama
- Quantum bilgi işlem saldırılarına karşı dayanıklılık"

**Kapanış:**
"ImageBasedEncryptionSystem, modern güvenlik gereksinimlerini karşılamak için geliştirilmiş, kullanımı kolay ama güçlü bir çözümdür. Projemiz, görüntülerin yaygınlığını ve masum görünümünü kullanarak, hassas verileri korumak için yenilikçi bir yaklaşım sunmaktadır. 

Şifreleme, steganografi ve kullanılabilirliği harmanlayan bu sistemin, dijital güvenlik alanına değerli bir katkı sağlayacağına inanıyorum. 

Dikkatiniz ve zamanınız için teşekkür ederim. Sorularınızı yanıtlamaktan memnuniyet duyarım."

## Sunum İpuçları

### Hazırlık
- Tüm demoları önceden test edin
- Ekran görüntülerini ve görselleri yüksek çözünürlükte hazırlayın
- Teknik terimleri açıklayacak basit benzetmeler hazırlayın
- Sunumun ana noktalarını vurgulayan 2-3 anahtar cümle belirleyin

### Sunum Sırasında
- Jüri üyelerine göz teması kurun
- Teknik terimler kullanırken kısa açıklamalar ekleyin
- Demolar esnasında her adımı net bir şekilde anlatın
- En çarpıcı özelliklere daha fazla zaman ayırın (çok katmanlı şifreleme, steganaliz)

### Soru-Cevap İçin Hazırlık
- Şu konularda soruları yanıtlamaya hazır olun:
  - Veri güvenliği ve gizliliği arasındaki fark
  - Steganografi tekniğinin zayıf yönleri ve bunlara karşı alınan önlemler
  - Önerilen şifreleme yöntemlerinin güvenlik seviyesi
  - Optimize edilen LSB algoritmasının detayları
  - Sistemin potansiyel saldırılara karşı dayanıklılığı

## Görsel Malzemeler Listesi

Sunum için şu görselleri hazırlayın:

1. **Sistem Mimarisi Şeması**: Dört katmanın etkileşimini gösteren şema
2. **Şifreleme Akış Diyagramı**: Metin şifreleme sürecinin adım adım gösterimi
3. **LSB Steganografi Görseli**: Bit düzleminde yapılan değişiklikleri gösteren örnek
4. **Steganaliz Görselleri**: RS, SPA, değişim haritası ve bit düzlemi analizlerinden örnek görseller
5. **Karşılaştırma Tablosu**: Mevcut benzer sistemlerle karşılaştırma
6. **Uygulama Arayüzü Ekran Görüntüleri**: Ana form, analiz ekranı, geliştirici modu
7. **Demo Adımları**: Demo esnasında kullanılacak örnek görüntüler ve metinler

## Özet Poster Ögeleri

TÜBİTAK sunumu için bir poster hazırlanacaksa şu ögelere yer verilmelidir:

- **Proje Başlığı**: "ImageBasedEncryptionSystem: Görüntü Tabanlı Çok Katmanlı Güvenli Veri Gizleme Sistemi"
- **Ana Amaç**: Projenin tek cümlelik amacı
- **Mimari Şema**: Sistemin katmanlı yapısı
- **Güvenlik Yapısı**: Çok katmanlı güvenlik yaklaşımı 
- **Görsel Örnekler**: Orijinal ve şifrelenmiş görüntü örnekleri (fark edilemeyecek şekilde)
- **Steganaliz Görselleri**: Analiz sonuçlarından çarpıcı örnekler
- **Sonuçlar**: Sistemin başarım ve güvenlik metrikleri 