using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    /// <summary>
    /// Kimlik oluşturma işlemleri için sınıf
    /// </summary>
    public class Cls_IdentityCreate
    {
        public string CreateRandomIdentity(int length)
        {
            try
            {
                Console.WriteLine(Success.SUCCESS_IDENTITY_CREATE_STARTED);
                if (length < 10 || length > 100)
                    throw new ArgumentException("Length must be between 10 and 100.");

                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                const string specialChars = "_-";
                Random random = new Random();

                StringBuilder identity = new StringBuilder();

                Console.WriteLine(Success.SUCCESS_IDENTITY_CREATE_PROCESSING);
                // İlk karakter harf veya rakam olmalı
                identity.Append(chars[random.Next(chars.Length)]);

                for (int i = 1; i < length - 1; i++)
                {
                    if (random.NextDouble() < 0.8)
                    {
                        identity.Append(chars[random.Next(chars.Length)]);
                    }
                    else
                    {
                        identity.Append(specialChars[random.Next(specialChars.Length)]);
                    }
                }

                // Son karakter harf veya rakam olmalı
                identity.Append(chars[random.Next(chars.Length)]);

                Console.WriteLine(Success.SUCCESS_IDENTITY_CREATE_PROCESSED);
                return identity.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_IDENTITY_CREATE_PROCESS_FAILED, ex.Message));
                throw;
            }
        }
    }
}
