using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// AES ile şifre çözme işlemleri yapan sınıf
    /// </summary>
    public class Cls_AesDecrypt
    {
        /// <summary>
        /// AES algoritması ile şifrelenmiş veriyi çözer
        /// </summary>
        /// <param name="cipherText">Şifrelenmiş metin (Base64 formatında)</param>
        /// <param name="password">Şifre çözme için kullanılacak parola</param>
        /// <returns>Boş string</returns>
        public string Decrypt(string cipherText, string password)
        {
            return string.Empty;
        }
    }
}
