using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer
{
    public static class Cls_AesHelper
    {
        private static readonly byte[] IV = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }; // Sabit IV değeri

        public static byte[] GenerateAESKey(string keySource)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_AES_GENERATE_KEY_STARTED, keySource));
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(keySource);
                    byte[] hash = sha256.ComputeHash(keyBytes);
                    Console.WriteLine(string.Format(Debug.DEBUG_AES_GENERATE_KEY_COMPLETED, BitConverter.ToString(hash.Take(32).ToArray())));
                    return hash.Take(32).ToArray(); // AES-256 için 32 byte anahtar
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_AES_GENERATE_KEY_FAILED, ex.Message));
                throw;
            }
        }

        public static string Encrypt(string plainText, byte[] aesKey)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_AES_ENCRYPT_STARTED, BitConverter.ToString(aesKey), BitConverter.ToString(IV)));
                using (var aesAlg = System.Security.Cryptography.Aes.Create())
                {
                    Console.WriteLine(Debug.DEBUG_AES_ENCRYPT_ENGINE_INIT);
                    aesAlg.Key = aesKey;
                    aesAlg.IV = IV;

                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                Console.WriteLine(Debug.DEBUG_AES_ENCRYPT_PROCESSING);
                                swEncrypt.Write(plainText);
                            }
                            Console.WriteLine(Debug.DEBUG_AES_ENCRYPT_PROCESSED);
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_AES_ENCRYPT_PROCESS_FAILED, ex.Message));
                throw;
            }
        }

        public static string Decrypt(string cipherText, byte[] aesKey)
        {
            try
            {
                Console.WriteLine(string.Format(Debug.DEBUG_AES_DECRYPT_STARTED, BitConverter.ToString(aesKey), BitConverter.ToString(IV)));
                using (var aesAlg = System.Security.Cryptography.Aes.Create())
                {
                    Console.WriteLine(Debug.DEBUG_AES_DECRYPT_ENGINE_INIT);
                    aesAlg.Key = aesKey;
                    aesAlg.IV = IV;

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                Console.WriteLine(Debug.DEBUG_AES_DECRYPT_PROCESSING);
                                string result = srDecrypt.ReadToEnd();
                                Console.WriteLine(Debug.DEBUG_AES_DECRYPT_PROCESSED);
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Errors.ERROR_AES_DECRYPT_PROCESS_FAILED, ex.Message));
                throw;
            }
        }
    }
}
