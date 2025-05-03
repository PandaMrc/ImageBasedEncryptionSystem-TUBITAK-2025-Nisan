using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using ImageBasedEncryptionSystem.TypeLayer;
using ImageBasedEncryptionSystem.DataLayer;
using System.Drawing;

public static class Cls_RsaHelper
{
    private static string configPath = Cls_Config.GetConfigFilePath();
    private static string GetDefaultSystemIdentity()
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_DEFAULT_ID_RECEIVING));
            if (!File.Exists(configPath))
            {
                return Cls_Config.GetDefaultSystemIdentity();
                
            }
            Console.WriteLine(string.Format(Debug.DEBUG_DEFAULT_ID_RECEIVED));
            var json = JObject.Parse(File.ReadAllText(configPath));
            string defaultIdentity = json["DefaultSystemIdentity"]?.ToString();
            Console.WriteLine(string.Format(Debug.DEBUG_DEFAULT_ID_PROCESSING));
            if (string.IsNullOrWhiteSpace(defaultIdentity))
            {
                return Cls_Config.GetDefaultSystemIdentity();
            }
            Console.WriteLine(string.Format(Debug.DEBUG_DEFAULT_ID_PROCESSED));
            return defaultIdentity;
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_DEFAULT_ID_NOT_FOUND, ex.Message));
            return Cls_Config.GetDefaultSystemIdentity();
        }
    }

    private static string DefaultSystemIdentity = GetDefaultSystemIdentity();

    private static string _cachedSystemIdentity = null;
    private static AsymmetricCipherKeyPair _keyPair = null;

    /// <summary>
    /// RSA anahtar çiftini üretir (3072-bit, sabit seed ile).
    /// </summary>
    public static void EnsureKeyPair()
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_RSA_ENSURE_KEY_PAIR_STARTED, Cls_Config.GetSystemIdentity()));
            string currentIdentity = Cls_Config.GetSystemIdentity();

            if (_keyPair != null && currentIdentity == _cachedSystemIdentity)
            {
                Console.WriteLine(string.Format(Debug.SUCCESS_RSA_ENSURE_KEY_PAIR_EXISTING, currentIdentity));
                Console.WriteLine(Debug.SUCCESS_RSA_ENSURE_KEY_PAIR_GENERATED);
                return; // değişiklik yok, öncekini kullan
            }

            Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_RECEIVED, currentIdentity));
            Console.WriteLine(string.Format(Debug.DEBUG_CACHED_SYSTEM_IDENTITY, _cachedSystemIdentity));
            Console.WriteLine(string.Format(Debug.DEBUG_EXISTING_KEY_PAIR, _keyPair != null));

            // Deterministik RNG ile RSA anahtar üretimi
            var seed = Encoding.UTF8.GetBytes(currentIdentity);
            var digestRandomGenerator = new DigestRandomGenerator(new Org.BouncyCastle.Crypto.Digests.Sha256Digest());
            digestRandomGenerator.AddSeedMaterial(seed);

            // Debug: SystemIdentity'nin doğruluğunu kontrol et
            Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_CHECK, currentIdentity));

            // Debug: Seed değerinin doğruluğunu kontrol et
            Console.WriteLine(string.Format(Debug.DEBUG_SEED_BYTE_ARRAY, BitConverter.ToString(seed)));

            // Debug: DigestRandomGenerator ve SecureRandom'un başlatılmasını kontrol et
            Console.WriteLine(Debug.DEBUG_DIGEST_RANDOM_GENERATOR_STARTED);
            digestRandomGenerator.AddSeedMaterial(seed);
            Console.WriteLine(Debug.DEBUG_DIGEST_RANDOM_GENERATOR_SEED_ADDED);

            Console.WriteLine(Debug.DEBUG_SECURE_RANDOM_CREATED);
            var random = new SecureRandom(digestRandomGenerator);
            Console.WriteLine(Debug.DEBUG_SECURE_RANDOM_CREATED);

            // Debug: RsaKeyPairGenerator'un başlatılmasını kontrol et
            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATOR_STARTED);
            var rsaGen = new RsaKeyPairGenerator();
            rsaGen.Init(new KeyGenerationParameters(random, 3072));
            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATOR_STARTED);

            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATED);
            _keyPair = rsaGen.GenerateKeyPair();
            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATED);

            _cachedSystemIdentity = currentIdentity;
            Console.WriteLine(string.Format(Debug.DEBUG_SYSTEM_IDENTITY_BYTE_ARRAY, BitConverter.ToString(seed)));
            Console.WriteLine(Debug.DEBUG_SYSTEM_IDENTITY_CHECK);
            Console.WriteLine(Debug.DEBUG_DIGEST_RANDOM_GENERATOR_SEED_ADDED);
            Console.WriteLine(Debug.DEBUG_SECURE_RANDOM_CREATED);
            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATOR_STARTED);
            Console.WriteLine(Debug.DEBUG_RSA_KEY_PAIR_GENERATED);
            Console.WriteLine(Debug.SUCCESS_RSA_ENSURE_KEY_PAIR_GENERATED);
            string publicKey = GetPublicKeyPem();
            string privateKey = GetPrivateKeyPem();
            Console.WriteLine(Debug.DEBUG_NEW_RSA_KEY_PAIR_CREATED);
            Console.WriteLine($"Public Key:\n{publicKey}\nPrivate Key:\n{privateKey}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_RSA_ENSURE_KEY_PAIR_FAILED, ex.Message));
            Console.WriteLine(string.Format(Debug.ERROR_RSA_KEY_GENERATION_FAILED, ex.Message));
            throw;
        }
    }

    /// <summary>
    /// RSA public key ile metni şifreler.
    /// </summary>
    public static string Encrypt(string plainText)
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_RSA_ENCRYPT_STARTED, _keyPair.Public));
            EnsureKeyPair();

            var encryptEngine = new Org.BouncyCastle.Crypto.Engines.RsaEngine();
            Console.WriteLine(Debug.DEBUG_RSA_ENCRYPT_ENGINE_INIT);
            encryptEngine.Init(true, _keyPair.Public);

            var bytesToEncrypt = Encoding.UTF8.GetBytes(plainText);
            Console.WriteLine(Debug.DEBUG_RSA_ENCRYPT_PROCESSING);
            var encrypted = encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
            Console.WriteLine(Debug.DEBUG_RSA_ENCRYPT_PROCESSED);

            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_RSA_ENCRYPT_FAILED, ex.Message));
            Console.WriteLine(string.Format(Debug.ERROR_RSA_ENCRYPTION_PROCESS_FAILED, ex.Message));
            throw;
        }
    }

    /// <summary>
    /// RSA private key ile metni çözer.
    /// </summary>
    public static string Decrypt(string cipherText)
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_RSA_DECRYPT_STARTED, _keyPair.Private));
            EnsureKeyPair();

            var decryptEngine = new Org.BouncyCastle.Crypto.Engines.RsaEngine();
            Console.WriteLine(Debug.DEBUG_RSA_DECRYPT_ENGINE_INIT);
            decryptEngine.Init(false, _keyPair.Private);

            var bytesToDecrypt = Convert.FromBase64String(cipherText);
            Console.WriteLine(Debug.DEBUG_RSA_DECRYPT_PROCESSING);
            var decrypted = decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
            Console.WriteLine(Debug.DEBUG_RSA_DECRYPT_PROCESSED);

            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_RSA_DECRYPT_FAILED, ex.Message));
            Console.WriteLine(string.Format(Debug.ERROR_RSA_DECRYPTION_PROCESS_FAILED, ex.Message));
            throw;
        }
    }

    /// <summary>
    /// Public Key PEM formatında dışa aktarılır.
    /// </summary>
    public static string GetPublicKeyPem()
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_RSA_GET_PUBLIC_KEY_PEM_STARTED, _keyPair.Public));
            EnsureKeyPair();

            using (var sw = new StringWriter())
            {
                var pemWriter = new PemWriter(sw);
                Console.WriteLine(Debug.DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTING);
                pemWriter.WriteObject(_keyPair.Public);
                pemWriter.Writer.Flush();
                Console.WriteLine(Debug.DEBUG_RSA_GET_PUBLIC_KEY_PEM_EXPORTED);
                return sw.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_RSA_GET_PUBLIC_KEY_PEM_FAILED, ex.Message));
            Console.WriteLine(string.Format(Debug.ERROR_RSA_PUBLIC_KEY_EXPORT_FAILED, ex.Message));
            throw;
        }
    }

    /// <summary>
    /// Private Key PEM formatında dışa aktarılır.
    /// </summary>
    public static string GetPrivateKeyPem()
    {
        try
        {
            Console.WriteLine(string.Format(Debug.DEBUG_RSA_GET_PRIVATE_KEY_PEM_STARTED, _keyPair.Private));
            EnsureKeyPair();

            using (var sw = new StringWriter())
            {
                var pemWriter = new PemWriter(sw);
                Console.WriteLine(Debug.DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTING);
                pemWriter.WriteObject(_keyPair.Private);
                pemWriter.Writer.Flush();
                Console.WriteLine(Debug.DEBUG_RSA_GET_PRIVATE_KEY_PEM_EXPORTED);
                return sw.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format(Debug.ERROR_RSA_GET_PRIVATE_KEY_PEM_FAILED, ex.Message));
            Console.WriteLine(string.Format(Debug.ERROR_RSA_PRIVATE_KEY_EXPORT_FAILED, ex.Message));
            throw;
        }
    }
}