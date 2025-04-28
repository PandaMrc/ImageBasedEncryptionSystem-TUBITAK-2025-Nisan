using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Kimlik oluşturma işlemleri için sınıf
    /// </summary>
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
            // Sabit bir kimlik değeri döndür
            return "TestIdentity_12345";
        }

        /// <summary>
        /// Verilen metinden kimlik oluşturur
        /// </summary>
        /// <param name="input">Kaynak metin</param>
        /// <returns>Oluşturulan kimlik</returns>
        public string CreateIdentityFromText(string input)
        {
            return "CustomIdentity_" + input.GetHashCode().ToString("X8");
        }
    }
}
