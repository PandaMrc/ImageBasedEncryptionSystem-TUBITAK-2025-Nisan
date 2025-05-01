# Yapılacaklar Listesi

1. **BusinessLayer Katmanındaki Eksik Sınıfların Tamamlanması:**
   - [x] `Cls_AesHelper.cs`, `Cls_RsaHelper.cs`, `Cls_LsbHelper.cs`, ve `Cls_IdentityCreate` sınıflarını oluşturmalıyız.
   - [x] Her metodun detaylı debug yapılabilecek şekilde tasarlanması gerekiyor. Debug çıktıları ve mesajlar `TypeLayer` katmanındaki `Errors.cs` ve `Succsess.cs` dosyalarından alınacak.
   - [x] `Cls_RsaHelper.cs` için BouncyCastle RSA kütüphanesi, `Cls_LsbHelper.cs` için Accord.NET Wavelet kütüphanesi kullanılacak.

2. **FrmMenu'deki Şifreleme ve Şifre Çözme İşlemleri:**
   - [x] Yeni oluşturulan sınıflardaki metodlar kullanılarak şifreleme ve şifre çözme işlemleri yapılacak.
   - [x] Debug ve mesajlar yine `TypeLayer` katmanından alınacak.

3. **FrmAdmin'deki Kimlik İşlemleri:**
   - [x] Kimlik oluşturma, kaydetme ve sıfırlama işlevleri tamamlanacak. Butonların işlevleri `ProjeYapısı.txt` dosyasında belirtilmiş.

4. **Diğer Metodlar ve Eventlerin Analizi:**
   - Projedeki diğer metodlar ve eventler detaylı bir şekilde analiz edilip, hata ayıklamaya uygun hale getirilecek. Try-catch blokları detaylı bir şekilde kullanılacak.

5. **FrmInfo'daki Bilgilerin Güncellenmesi:**
   - FrmInfo'daki bilgiler güncellenmeli ve proje hakkında daha fazla bilgi eklenmeli. Form yapısını bozmadan güzelleştirmeler yapılabilir.

6. **Analiz Metodları ve UI Güncellemeleri:**
   - Analiz yapabilmek için gerekli metodlar ve UI güncellemeleri yapılacak. Formun tasarımı modern bir şekilde yeniden yapılabilir.

7. **Loglama Değişiklikleri ve Sabit Mesajların TypeLayer'a Taşınması:**
   - [x] Projedeki tüm sınıflara NLog eklendi ve try-catch blokları ile hata ayıklama geliştirildi.
   - [x] Tüm sabit hata ve başarı mesajları TypeLayer katmanındaki Errors.cs ve Success.cs dosyalarına taşındı. 
