using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using ImageBasedEncryptionSystem.TypeLayer;

namespace ImageBasedEncryptionSystem.BusinessLayer.Tests
{
    /// <summary>
    /// Birim testleri içeren sınıf
    /// </summary>
    public class UnitTests
    {
        /// <summary>
        /// Birim testlerini çalıştırır
        /// </summary>
        /// <returns>Test sonuçları</returns>
        public Dictionary<string, string> RunAllTests()
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            
            try
            {
                // AES Testi
                results.Add("AES Encryption Test", TestAesEncryption());
                
                // RSA Testi
                results.Add("RSA Encryption Test", TestRsaEncryption());
                
                // Wavelet Testi
                results.Add("Wavelet Data Hiding Test", TestWaveletDataHiding());
                
                // Tam Entegrasyon Testi
                results.Add("Full Integration Test", TestFullIntegration());
            }
            catch (Exception ex)
            {
                results.Add("Error", ex.Message);
            }
            
            return results;
        }
        
        /// <summary>
        /// AES şifreleme ve şifre çözme testini gerçekleştirir
        /// </summary>
        /// <returns>Test sonucu</returns>
        private string TestAesEncryption()
        {
            try
            {
                // Test verileri
                string originalText = "Bu bir test metnidir.";
                string password = "test123";
                
                // Sınıf örnekleri
                Cls_AesEncrypt aesEncrypt = new Cls_AesEncrypt();
                Cls_AesDecrypt aesDecrypt = new Cls_AesDecrypt();
                
                // Şifreleme
                string encryptedText = aesEncrypt.Encrypt(originalText, password);
                
                // Şifre çözme
                string decryptedText = aesDecrypt.Decrypt(encryptedText, password);
                
                // Sonuç kontrolü
                if (originalText == decryptedText)
                    return "PASSED";
                else
                    return "FAILED: Orijinal metin ile çözülen metin eşleşmiyor.";
            }
            catch (Exception ex)
            {
                return "FAILED: " + ex.Message;
            }
        }
        
        /// <summary>
        /// RSA şifreleme ve şifre çözme testini gerçekleştirir
        /// </summary>
        /// <returns>Test sonucu</returns>
        private string TestRsaEncryption()
        {
            try
            {
                // Test verileri
                string originalText = "RSA test metni";
                
                // Sınıf örnekleri
                Cls_RsaEncrypt rsaEncrypt = new Cls_RsaEncrypt();
                Cls_RsaDecrypt rsaDecrypt = new Cls_RsaDecrypt(rsaEncrypt);
                
                // Şifreleme
                string encryptedText = rsaEncrypt.Encrypt(originalText);
                
                // Şifre çözme
                string decryptedText = rsaDecrypt.Decrypt(encryptedText);
                
                // Sonuç kontrolü
                if (originalText == decryptedText)
                    return "PASSED";
                else
                    return "FAILED: Orijinal metin ile çözülen metin eşleşmiyor.";
            }
            catch (Exception ex)
            {
                return "FAILED: " + ex.Message;
            }
        }
        
        /// <summary>
        /// Wavelet veri gizleme ve çıkarma testini gerçekleştirir
        /// </summary>
        /// <returns>Test sonucu</returns>
        private string TestWaveletDataHiding()
        {
            try
            {
                // Test verileri
                string encryptedText = "Şifrelenmiş test verisi";
                string encryptedKey = "Şifrelenmiş anahtar";
                
                // Test için görüntü oluştur
                Bitmap testImage = new Bitmap(512, 512);
                using (Graphics g = Graphics.FromImage(testImage))
                {
                    g.Clear(Color.White);
                }
                
                // Sınıf örnekleri
                Cls_WaveletEncrypt waveletEncrypt = new Cls_WaveletEncrypt();
                Cls_WaveletDecrypt waveletDecrypt = new Cls_WaveletDecrypt();
                
                // Veri gizleme
                Bitmap resultImage = waveletEncrypt.HideData(testImage, encryptedText, encryptedKey);
                
                // Veri çıkarma
                ExtractedData extractedData = waveletDecrypt.ExtractData(resultImage);
                
                // Sonuç kontrolü
                if (extractedData != null && 
                    extractedData.EncryptedText == encryptedText && 
                    extractedData.EncryptedKey == encryptedKey)
                    return "PASSED";
                else
                    return "FAILED: Gizlenen veri ile çıkarılan veri eşleşmiyor.";
            }
            catch (Exception ex)
            {
                return "FAILED: " + ex.Message;
            }
        }
        
        /// <summary>
        /// Tam entegrasyon testini gerçekleştirir
        /// </summary>
        /// <returns>Test sonucu</returns>
        private string TestFullIntegration()
        {
            try
            {
                // Test verileri
                string originalText = "Bu tam bir entegrasyon testi için kullanılacak test metnidir.";
                string password = "Integration123";
                
                // Test için görüntü oluştur
                Bitmap testImage = new Bitmap(512, 512);
                using (Graphics g = Graphics.FromImage(testImage))
                {
                    g.Clear(Color.White);
                }
                
                // Sınıf örnekleri
                Cls_AesEncrypt aesEncrypt = new Cls_AesEncrypt();
                Cls_AesDecrypt aesDecrypt = new Cls_AesDecrypt();
                Cls_RsaEncrypt rsaEncrypt = new Cls_RsaEncrypt();
                Cls_RsaDecrypt rsaDecrypt = new Cls_RsaDecrypt(rsaEncrypt);
                Cls_WaveletEncrypt waveletEncrypt = new Cls_WaveletEncrypt();
                Cls_WaveletDecrypt waveletDecrypt = new Cls_WaveletDecrypt();
                
                // 1. AES şifreleme
                string encryptedText = aesEncrypt.Encrypt(originalText, password);
                
                // 2. Parola RSA ile şifrelenir (AES anahtarı olarak parola kullanıldığı için)
                string encryptedKey = rsaEncrypt.Encrypt(password);
                
                // 3. Wavelet ile veri gizleme
                Bitmap resultImage = waveletEncrypt.HideData(testImage, encryptedText, encryptedKey);
                
                // 4. Wavelet ile veri çıkarma
                ExtractedData extractedData = waveletDecrypt.ExtractData(resultImage);
                
                // 5. RSA ile şifrelenmiş AES anahtarı çözülür
                string decryptedAesKey = rsaDecrypt.Decrypt(extractedData.EncryptedKey);
                
                // 6. AES ile metin çözülür
                string decryptedText = aesDecrypt.Decrypt(extractedData.EncryptedText, decryptedAesKey);
                
                // Sonuç kontrolü
                if (decryptedText == originalText && decryptedAesKey == password)
                    return "PASSED";
                else
                    return "FAILED: Entegrasyon testi sonucunda orijinal veriler ile çözülen veriler eşleşmiyor.";
            }
            catch (Exception ex)
            {
                return "FAILED: " + ex.Message;
            }
        }
    }
} 