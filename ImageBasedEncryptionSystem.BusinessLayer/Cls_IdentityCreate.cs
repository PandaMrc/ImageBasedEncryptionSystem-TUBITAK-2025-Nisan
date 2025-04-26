using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public class Cls_IdentityCreate
    {
        /// <summary>
        /// Rastgele kimlik oluşturur.
        /// 10 ile 50 karakter arasında bir uzunlukta olacak şekilde rastgele kimlik oluşturur.
        /// Boşluk karakteri içermez, "_" ve "-" karakterleri içerebilir fakat bu karakterler ile başlayamaz veya bitemez.
        /// </summary>
        /// <returns>Oluşturulan rastgele kimlik</returns>
        public string CreateRandomIdentity()
        {
            try
            {
                Random random = new Random();
                
                // Rastgele uzunluk belirle (10-50 arası)
                int length = random.Next(10, 51);
                
                // Kullanılabilecek karakterler
                string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string specialChars = "_-";
                
                StringBuilder identity = new StringBuilder();
                
                // İlk karakter özel karakter olamaz, harf veya rakam olmalı
                identity.Append(allowedChars[random.Next(allowedChars.Length)]);
                
                // Kalan karakterleri oluştur
                for (int i = 1; i < length - 1; i++)
                {
                    // %80 ihtimalle normal karakter, %20 ihtimalle özel karakter kullan
                    if (random.NextDouble() < 0.8)
                    {
                        identity.Append(allowedChars[random.Next(allowedChars.Length)]);
                    }
                    else
                    {
                        identity.Append(specialChars[random.Next(specialChars.Length)]);
                    }
                }
                
                // Son karakter özel karakter olamaz, harf veya rakam olmalı
                identity.Append(allowedChars[random.Next(allowedChars.Length)]);
                
                return identity.ToString();
            }
            catch (Exception ex)
            {
                // Hata durumunda basit bir kimlik döndür
                return "RandomIdentity_" + Guid.NewGuid().ToString().Substring(0, 8);
            }
        }
    }
}
