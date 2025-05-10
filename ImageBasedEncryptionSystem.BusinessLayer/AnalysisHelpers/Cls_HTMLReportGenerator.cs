// Cls_HtmlReportGenerator.cs içinde

using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;
using System;
using System.Linq;
using System.Net;
using System.Drawing.Drawing2D;
using ImageBasedEncryptionSystem.TypeLayer;

public static class Cls_HtmlReportGenerator
{
    /// <summary>
    /// Analiz sonuçlarından HTML raporu oluşturur.
    /// </summary>
    /// <param name="result">Analiz sonuçlarını içeren nesne</param>
    /// <param name="analyzedChannel">İncelenen renk kanalı</param>
    /// <param name="originalLsbPlane">Orijinal resmin LSB düzlemi</param>
    /// <param name="stegoLsbPlane">Şifreli resmin LSB düzlemi</param>
    /// <param name="lsbChangeMap">LSB değişim haritası</param>
    /// <param name="rsGroupsChart">Şifreli resmin RS gruplarını gösteren grafik</param>
    /// <param name="spaChart">Şifreli resmin SPA analizi grafiği</param>
    /// <param name="originalRsGroupsChart">Orijinal resmin RS gruplarını gösteren grafik</param>
    /// <param name="originalSpaChart">Orijinal resmin SPA analizi grafiği</param>
    /// <returns>HTML formatında rapor içeriği</returns>
    public static string GenerateReport(
        Cls_ComprehensiveSteganalysisResult result,
        ColorChannel analyzedChannel,
        Bitmap originalLsbPlane = null,
        Bitmap stegoLsbPlane = null,
        Bitmap lsbChangeMap = null,
        Bitmap rsGroupsChart = null,
        Bitmap spaChart = null,
        Bitmap originalRsGroupsChart = null,
        Bitmap originalSpaChart = null)
    {
        if (result == null) throw new ArgumentNullException(nameof(result));

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"tr\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"UTF-8\">");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("  <title>Steganografi Analiz Raporu</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    :root {");
        sb.AppendLine("      --primary-color: #2c3e50;");
        sb.AppendLine("      --accent-color: #3498db;");
        sb.AppendLine("      --bg-light: #f8f9fa;");
        sb.AppendLine("      --bg-white: #ffffff;");
        sb.AppendLine("      --shadow: 0 2px 10px rgba(0,0,0,0.1);");
        sb.AppendLine("      --border-radius: 6px;");
        sb.AppendLine("      --border-color: #e6e6e6;");
        sb.AppendLine("    }");
        sb.AppendLine("    * { box-sizing: border-box; margin: 0; padding: 0; }");
        sb.AppendLine("    body { font-family: 'Segoe UI', Arial, sans-serif; margin: 0; line-height: 1.6; background-color: #f0f2f5; color: #333; }");
        sb.AppendLine("    h1 { color: var(--primary-color); font-size: 28px; margin-bottom: 16px; }");
        sb.AppendLine("    h2 { color: var(--accent-color); margin-top: 20px; font-size: 22px; border-left: 4px solid var(--accent-color); padding-left: 10px; }");
        sb.AppendLine("    h3 { font-size: 18px; margin: 15px 0; color: #444; }");
        sb.AppendLine("    .container { max-width: 1200px; margin: 20px auto; padding: 0 20px; }");
        sb.AppendLine("    .header { background: linear-gradient(135deg, var(--primary-color), var(--accent-color)); color: white; padding: 30px; border-radius: var(--border-radius); margin-bottom: 20px; box-shadow: var(--shadow); }");
        sb.AppendLine("    .section { background-color: var(--bg-white); padding: 20px; margin-bottom: 25px; border-radius: var(--border-radius); box-shadow: var(--shadow); }");
        sb.AppendLine("    table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
        sb.AppendLine("    th, td { padding: 12px; text-align: left; border-bottom: 1px solid var(--border-color); }");
        sb.AppendLine("    th { background-color: var(--bg-light); font-weight: 600; }");
        sb.AppendLine("    tr:hover { background-color: #f5f9ff; }");
        sb.AppendLine("    .image-container { display: flex; flex-wrap: wrap; gap: 20px; margin: 15px 0; }");
        sb.AppendLine("    .image-box { flex: 1 1 calc(50% - 20px); min-width: 300px; margin-bottom: 20px; background: var(--bg-white); padding: 15px; border-radius: var(--border-radius); box-shadow: var(--shadow); }");
        sb.AppendLine("    .image-box img { max-width: 100%; height: auto; border: 1px solid var(--border-color); border-radius: 4px; display: block; margin: 0 auto; }");
        sb.AppendLine("    .image-box p { margin: 10px 0; font-weight: 600; text-align: center; color: var(--primary-color); }");
        sb.AppendLine("    .alert { padding: 15px; background-color: #f8d7da; color: #721c24; border-radius: var(--border-radius); margin: 15px 0; }");
        sb.AppendLine("    .alert-success { background-color: #d4edda; color: #155724; }");
        sb.AppendLine("    .footer { margin-top: 40px; padding: 15px; text-align: center; font-size: 0.9em; color: #7f8c8d; background-color: var(--bg-white); border-radius: var(--border-radius); box-shadow: var(--shadow); }");
        sb.AppendLine("    .comparison { display: flex; flex-wrap: wrap; gap: 20px; }");
        sb.AppendLine("    .comparison .image-box { flex: 1 1 calc(50% - 20px); }");
        sb.AppendLine("    .scroll-box { max-height: 200px; overflow-y: auto; border: 1px solid var(--border-color); padding: 10px; background-color: #f8f9fa; border-radius: 4px; margin: 10px 0; word-break: break-all; white-space: pre-wrap; font-family: monospace; }");
        sb.AppendLine("    .btn { display: inline-block; background-color: var(--accent-color); color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; text-decoration: none; font-size: 14px; }");
        sb.AppendLine("    .btn:hover { background-color: #2980b9; }");
        sb.AppendLine("    .btn-secondary { background-color: #6c757d; }");
        sb.AppendLine("    .btn-secondary:hover { background-color: #5a6268; }");
        sb.AppendLine("    .btn-container { margin-top: 10px; display: flex; gap: 10px; }");
        sb.AppendLine("    .tab-container { margin: 20px 0; }");
        sb.AppendLine("    .tabs { display: flex; border-bottom: 1px solid var(--border-color); }");
        sb.AppendLine("    .tab { padding: 10px 15px; cursor: pointer; border-bottom: 3px solid transparent; }");
        sb.AppendLine("    .tab.active { border-bottom-color: var(--accent-color); color: var(--accent-color); }");
        sb.AppendLine("    .tab-content { display: none; padding: 20px; background: var(--bg-white); }");
        sb.AppendLine("    .tab-content.active { display: block; }");
        sb.AppendLine("    .info-badge { display: inline-block; padding: 4px 8px; border-radius: 10px; background-color: var(--accent-color); color: white; font-weight: bold; margin-left: 5px; }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"container\">");

        // Başlık ve Özet
        sb.AppendLine("  <div class=\"header\">");
        sb.AppendLine("    <h1>Steganografi Analiz Raporu</h1>");
        sb.AppendLine($"    <p>Analiz Tarihi: {result.AnalysisTimestamp}</p>");
        sb.AppendLine($"    <p>Analiz Edilen Dosya: {HtmlEncode(result.FileName)}</p>");
        sb.AppendLine("  </div>");

        // Temel Bilgiler
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Görüntü Bilgileri</h2>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <tr><th>Parametre</th><th>Değer</th></tr>");
        sb.AppendLine($"      <tr><td>Dosya Adı</td><td>{HtmlEncode(result.FileName)}</td></tr>");
        sb.AppendLine($"      <tr><td>Boyutlar</td><td>{result.ImageWidth} x {result.ImageHeight} px</td></tr>");
        sb.AppendLine($"      <tr><td>Dosya Boyutu</td><td>{FormatFileSize(result.FileSize)}</td></tr>");
        sb.AppendLine($"      <tr><td>Analiz Edilen Kanal</td><td>{analyzedChannel}</td></tr>");
        sb.AppendLine("    </table>");
        sb.AppendLine("  </div>");

        // Şifreli görselden çıkan veriler
        sb.AppendLine("  <div class='section'>");
        sb.AppendLine("    <h2>Şifreli Görselden Çıkarılan Veriler</h2>");
        
        // Veri Çıkarma İstatistikleri
        sb.AppendLine("    <h3>Veri Çıkarma İstatistikleri</h3>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <tr><th>Metrik</th><th>Değer</th></tr>");
        sb.AppendLine($"      <tr><td>İşlenen Piksel Sayısı</td><td>{result.ProcessedPixelCount:N0}</td></tr>");
        sb.AppendLine($"      <tr><td>Çıkarılan Veri Boyutu</td><td>{result.ExtractedDataSize:N0} bayt</td></tr>");
        sb.AppendLine($"      <tr><td>Veri Yoğunluğu</td><td>{result.ExtractedDataDensity:F4} bit/piksel</td></tr>");
        sb.AppendLine($"      <tr><td>Gereken Piksel Sayısı</td><td>~{(result.ExtractedDataSize * 8 / 3.0):N0} piksel (RGB kanalları kullanılarak)</td></tr>");
        sb.AppendLine("    </table>");
        
        // AES Şifreli Metin
        sb.AppendLine("    <h3>AES ile Şifrelenmiş Metin</h3>");
        if (!string.IsNullOrEmpty(result.AesEncryptedText))
        {
            // Format hatası kontrolü
            if (result.AesEncryptedText.StartsWith("Format hatası:"))
            {
                sb.AppendLine("    <div class=\"alert\" style=\"background-color: #f8d7da; color: #721c24;\">");
                sb.AppendLine("      <strong>Hata!</strong> " + HtmlEncode(result.AesEncryptedText));
                sb.AppendLine("    </div>");
            }
            else
            {
                const int maxDisplayLength = 10000; // Maksimum gösterilecek karakter sayısı
                string displayText = result.AesEncryptedText;
                bool isTruncated = false;
                
                // Metin çok uzunsa kısalt
                if (displayText.Length > maxDisplayLength)
                {
                    displayText = displayText.Substring(0, maxDisplayLength) + "... [Metin çok uzun, kısaltıldı]";
                    isTruncated = true;
                }
                
                sb.AppendLine("    <div class=\"scroll-box\">" + HtmlEncode(displayText) + "</div>");
                sb.AppendLine("    <div class=\"btn-container\">");
                sb.AppendLine("      <button class=\"btn\" onclick=\"downloadText('aes-encrypted.txt', '" + 
                    HtmlEncode(result.AesEncryptedText).Replace("'", "\\'") + "')\">Metni İndir</button>");
                
                if (isTruncated)
                {
                    sb.AppendLine("      <p><small>Not: Metin çok uzun olduğu için kısaltıldı. Tam metni görmek için 'Metni İndir' düğmesini kullanın.</small></p>");
                }
                
                sb.AppendLine("    </div>");
            }
        }
        else
        {
            sb.AppendLine("    <div class=\"alert\" style=\"background-color: #f8d7da; color: #721c24;\">");
            sb.AppendLine("      <strong>Uyarı!</strong> Şifreli metin bulunamadı veya çıkarılamadı.");
            sb.AppendLine("    </div>");
        }
        
        // RSA ile Şifrelenmiş AES Anahtarı
        sb.AppendLine("    <h3>RSA ile Şifrelenmiş AES Anahtarı</h3>");
        if (!string.IsNullOrEmpty(result.RsaEncryptedAesKey))
        {
            // Format hatası kontrolü
            if (result.RsaEncryptedAesKey.StartsWith("Format hatası:"))
            {
                sb.AppendLine("    <div class=\"alert\" style=\"background-color: #f8d7da; color: #721c24;\">");
                sb.AppendLine("      <strong>Hata!</strong> " + HtmlEncode(result.RsaEncryptedAesKey));
                sb.AppendLine("    </div>");
            }
            else
            {
                const int maxDisplayLength = 5000; // Maksimum gösterilecek karakter sayısı
                string displayKey = result.RsaEncryptedAesKey;
                bool isTruncated = false;
                
                // Anahtar çok uzunsa kısalt
                if (displayKey.Length > maxDisplayLength)
                {
                    displayKey = displayKey.Substring(0, maxDisplayLength) + "... [Anahtar çok uzun, kısaltıldı]";
                    isTruncated = true;
                }
                
                sb.AppendLine("    <div class=\"scroll-box\">" + HtmlEncode(displayKey) + "</div>");
                sb.AppendLine("    <div class=\"btn-container\">");
                sb.AppendLine("      <button class=\"btn\" onclick=\"downloadText('rsa-encrypted-key.txt', '" + 
                    HtmlEncode(result.RsaEncryptedAesKey).Replace("'", "\\'") + "')\">Anahtarı İndir</button>");
                
                if (isTruncated)
                {
                    sb.AppendLine("      <p><small>Not: Anahtar çok uzun olduğu için kısaltıldı. Tam anahtarı görmek için 'Anahtarı İndir' düğmesini kullanın.</small></p>");
                }
                
                sb.AppendLine("    </div>");
            }
        }
        else
        {
            sb.AppendLine("    <div class=\"alert\" style=\"background-color: #f8d7da; color: #721c24;\">");
            sb.AppendLine("      <strong>Uyarı!</strong> Şifreli anahtar bulunamadı veya çıkarılamadı.");
            sb.AppendLine("    </div>");
        }
        
        // İmza Durumu
        sb.AppendLine("    <h3>İmza Durumu</h3>");
        sb.AppendLine($"    <p>{(result.IsSignatureValid ? "<span style='color: green; font-weight: bold;'>İmza Doğrulandı ✓</span>" : "<span style='color: red; font-weight: bold;'>İmza Bulunamadı ✗</span>")}</p>");
        sb.AppendLine("  </div>");

        // --- Analiz Sonuçları Özeti ---
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Analiz Sonuçları Özeti</h2>");
        sb.AppendLine("    <table>");
        sb.AppendLine("      <tr><th>Analiz Yöntemi</th><th>Sonuç</th></tr>");
        
        // RS Analizi
        double rsEmbeddingRate = result.RsGroupsResult.EstimatedEmbeddingRateP;
        sb.AppendLine($"      <tr><td>RS Analizi: Gömme Oranı</td><td>{rsEmbeddingRate:P2}</td></tr>");
        
        // SPA Analizi
        double spaEmbeddingRate = result.SpaResult.EstimatedEmbeddingRateP;
        sb.AppendLine($"      <tr><td>SPA Analizi: Gömme Oranı</td><td>{spaEmbeddingRate:P2}</td></tr>");
        
        // Değişim Haritası
        if (result.ChangedPixelCount > 0)
        {
            sb.AppendLine($"      <tr><td>Değiştirilmiş Piksel Sayısı</td><td>{result.ChangedPixelCount:N0} ({result.ChangeRatio:P2})</td></tr>");
        }
        else
        {
            sb.AppendLine("      <tr><td>Değiştirilmiş Piksel Sayısı</td><td>Orijinal görüntü karşılaştırması yapılamadı</td></tr>");
        }
        
        // Ortalama gömme oranı
        sb.AppendLine($"      <tr><td>Ortalama Gömme Oranı</td><td>{result.AverageEmbeddingRate:P2}</td></tr>");
        
        // Tahmini veri uzunluğu
        sb.AppendLine($"      <tr><td>Tahmini Gömülü Veri</td><td>{result.EstimatedHiddenBytes:N0} bayt ({result.EstimatedHiddenBits:N0} bit)</td></tr>");
        
        sb.AppendLine("    </table>");
        sb.AppendLine("  </div>");

        // --- Görsel Analizler (Bit-Plane ve LSB Değişim Haritası) ---
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Görsel Analizler</h2>");

        // TAB navigasyonu ekle
        sb.AppendLine("    <div class=\"tab-container\">");
        sb.AppendLine("      <div class=\"tabs\">");
        sb.AppendLine("        <div class=\"tab active\" onclick=\"openTab(event, 'lsb-planes')\">LSB Düzlemleri</div>");
        sb.AppendLine("        <div class=\"tab\" onclick=\"openTab(event, 'change-map')\">Değişim Haritası</div>");
        sb.AppendLine("        <div class=\"tab\" onclick=\"openTab(event, 'rs-analysis')\">RS Analizi</div>");
        sb.AppendLine("        <div class=\"tab\" onclick=\"openTab(event, 'spa-analysis')\">SPA Analizi</div>");
        sb.AppendLine("      </div>");
        
        // LSB Düzlemleri Tab
        sb.AppendLine("      <div id=\"lsb-planes\" class=\"tab-content active\">");
        if (stegoLsbPlane != null || originalLsbPlane != null)
        {
            sb.AppendLine("        <div class=\"comparison\">");
            
            if (originalLsbPlane != null)
            {
                sb.AppendLine("          <div class=\"image-box\">");
                sb.AppendLine($"            <p>Orijinal Görüntünün LSB Düzlemi ({analyzedChannel})</p>");
                sb.AppendLine($"            <img src=\"{BitmapToBase64(originalLsbPlane)}\" alt=\"Orijinal LSB Düzlemi\">");
                sb.AppendLine("          </div>");
            }
            
            if (stegoLsbPlane != null)
            {
                sb.AppendLine("          <div class=\"image-box\">");
                sb.AppendLine($"            <p>Şifreli Görüntünün LSB Düzlemi ({analyzedChannel})</p>");
                sb.AppendLine($"            <img src=\"{BitmapToBase64(stegoLsbPlane)}\" alt=\"Şifreli LSB Düzlemi\">");
                sb.AppendLine("          </div>");
            }
            
            sb.AppendLine("        </div>");
        }
        else
        {
            sb.AppendLine("        <p>LSB düzlem analizi bulunmuyor.</p>");
        }
        sb.AppendLine("      </div>");
        
        // Değişim Haritası Tab
        sb.AppendLine("      <div id=\"change-map\" class=\"tab-content\">");
        if (lsbChangeMap != null)
        {
            sb.AppendLine("        <div class=\"comparison\">");
            
            // Orijinal resmin gösterimi (eğer originalLsbPlane varsa)
            if (originalLsbPlane != null)
            {
                sb.AppendLine("          <div class=\"image-box\">");
                sb.AppendLine($"            <p>Orijinal Görüntünün LSB Düzlemi ({analyzedChannel})</p>");
                sb.AppendLine($"            <img src=\"{BitmapToBase64(originalLsbPlane)}\" alt=\"Orijinal LSB Düzlemi\">");
                sb.AppendLine("          </div>");
            }
            
            sb.AppendLine("          <div class=\"image-box\">");
            sb.AppendLine($"            <p>LSB Değişim Haritası ({analyzedChannel})</p>");
            sb.AppendLine($"            <img src=\"{BitmapToBase64(lsbChangeMap)}\" alt=\"LSB Değişim Haritası\">");
            sb.AppendLine("          </div>");
            sb.AppendLine("        </div>");
            
            if (result.ChangedPixelCount > 0)
            {
                sb.AppendLine($"        <p>Toplam {result.ChangedPixelCount:N0} piksel değiştirilmiş. Bu değer, tüm piksellerin {result.ChangeRatio:P2}'sini temsil eder.</p>");
            }
        }
        else
        {
            sb.AppendLine("        <p>LSB değişim haritası bulunmuyor.</p>");
        }
        sb.AppendLine("      </div>");
        
        // RS Analizi Tab
        sb.AppendLine("      <div id=\"rs-analysis\" class=\"tab-content\">");
        if (rsGroupsChart != null)
        {
            sb.AppendLine("        <div class=\"comparison\">");
            
            // Orijinal RS grafik
            if (originalRsGroupsChart != null)
            {
                sb.AppendLine("          <div class=\"image-box\">");
                sb.AppendLine($"            <p>Orijinal Regular/Singular (RS) Grup Sayıları ({analyzedChannel})</p>");
                sb.AppendLine($"            <img src=\"{BitmapToBase64(originalRsGroupsChart)}\" alt=\"Orijinal RS Grupları Grafiği\">");
                sb.AppendLine("          </div>");
            }
            
            // Şifreli RS grafik
            sb.AppendLine("          <div class=\"image-box\">");
            sb.AppendLine($"            <p>Şifreli Regular/Singular (RS) Grup Sayıları ({analyzedChannel})</p>");
            sb.AppendLine($"            <img src=\"{BitmapToBase64(rsGroupsChart)}\" alt=\"Şifreli RS Grupları Grafiği\">");
            sb.AppendLine("          </div>");
            sb.AppendLine("        </div>");
            
            sb.AppendLine("        <h3>RS Analizi Detayları</h3>");
            sb.AppendLine("        <table>");
            sb.AppendLine("          <tr><th>Sayaç</th><th>Değer</th></tr>");
            sb.AppendLine($"          <tr><td>Regular Grup (M)</td><td>{result.RsGroupsResult.RM_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>Singular Grup (M)</td><td>{result.RsGroupsResult.SM_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>Regular Grup (-M)</td><td>{result.RsGroupsResult.RMinusM_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>Singular Grup (-M)</td><td>{result.RsGroupsResult.SMinusM_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td><strong>Tahmini Gömme Oranı</strong></td><td>{result.RsGroupsResult.EstimatedEmbeddingRateP:P2}</td></tr>");
            sb.AppendLine("        </table>");
        }
        else
        {
            sb.AppendLine("        <p>RS analiz grafiği bulunmuyor.</p>");
        }
        sb.AppendLine("      </div>");
        
        // SPA Analizi Tab
        sb.AppendLine("      <div id=\"spa-analysis\" class=\"tab-content\">");
        if (spaChart != null)
        {
            sb.AppendLine("        <div class=\"comparison\">");
            
            // Orijinal SPA grafik
            if (originalSpaChart != null)
            {
                sb.AppendLine("          <div class=\"image-box\">");
                sb.AppendLine($"            <p>Orijinal Sample Pair Analizi (SPA) Sayaçları ({analyzedChannel})</p>");
                sb.AppendLine($"            <img src=\"{BitmapToBase64(originalSpaChart)}\" alt=\"Orijinal SPA Grafiği\">");
                sb.AppendLine("          </div>");
            }
            
            // Şifreli SPA grafik
            sb.AppendLine("          <div class=\"image-box\">");
            sb.AppendLine($"            <p>Şifreli Sample Pair Analizi (SPA) Sayaçları ({analyzedChannel})</p>");
            sb.AppendLine($"            <img src=\"{BitmapToBase64(spaChart)}\" alt=\"Şifreli SPA Grafiği\">");
            sb.AppendLine("          </div>");
            sb.AppendLine("        </div>");
            
            sb.AppendLine("        <h3>SPA Analizi Detayları</h3>");
            sb.AppendLine("        <table>");
            sb.AppendLine("          <tr><th>Sayaç</th><th>Değer</th></tr>");
            sb.AppendLine($"          <tr><td>X0 Sayacı</td><td>{result.SpaResult.X0_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>Y0 Sayacı</td><td>{result.SpaResult.Y0_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>X1 Sayacı</td><td>{result.SpaResult.X1_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td>Y1 Sayacı</td><td>{result.SpaResult.Y1_Count:N0}</td></tr>");
            sb.AppendLine($"          <tr><td><strong>Tahmini Gömme Oranı</strong></td><td>{result.SpaResult.EstimatedEmbeddingRateP:P2}</td></tr>");
            sb.AppendLine("        </table>");
        }
        else
        {
            sb.AppendLine("        <p>SPA analiz grafiği bulunmuyor.</p>");
        }
        sb.AppendLine("      </div>");
        
        sb.AppendLine("    </div>"); // tab-container sonu
        sb.AppendLine("  </div>"); // section sonu

        // Rapor Açıklaması ve Sonuçların Yorumlanması
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Analiz Sonuçları Değerlendirmesi</h2>");
        
        string evaluationClass = "info-badge ";
        string evaluationText = "";
        
        if (result.AverageEmbeddingRate > 0.25)
        {
            evaluationClass += "alert-danger";
            evaluationText = "Yüksek Steganografi";
        }
        else if (result.AverageEmbeddingRate > 0.05)
        {
            evaluationClass += "alert-warning";
            evaluationText = "Orta Steganografi";
        }
        else
        {
            evaluationClass += "alert-success";
            evaluationText = "Düşük/Yok";
        }
        
        sb.AppendLine($"    <h3>Sonuç <span class=\"{evaluationClass}\">{evaluationText}</span></h3>");
        
        sb.AppendLine("    <p>Bu rapor, şüpheli görüntüde steganografi kullanımına ilişkin çeşitli analiz yöntemlerinin sonuçlarını içermektedir.</p>");
        sb.AppendLine("    <ul>");
        sb.AppendLine("      <li><strong>Bit Düzlemi Analizi:</strong> Görüntünün en düşük önemli bit (LSB) düzleminin incelenmesi.</li>");
        sb.AppendLine("      <li><strong>RS Analizi:</strong> Piksel gruplarının düzenli/tekil karakteristiklerini değerlendirerek gömme oranını tahmin eder.</li>");
        sb.AppendLine("      <li><strong>Sample Pair Analizi (SPA):</strong> Komşu piksel çiftlerinin istatistiksel özelliklerini inceleyerek gömme oranını tahmin eder.</li>");
        sb.AppendLine("      <li><strong>LSB Değişim Haritası:</strong> Orijinal ve şüpheli görüntü arasındaki LSB farklılıklarını gösterir.</li>");
        sb.AppendLine("    </ul>");
        
        sb.AppendLine("    <p><strong>Yorum:</strong> ");
        if (result.AverageEmbeddingRate > 0.25)
        {
            sb.AppendLine("Analiz sonuçlarına göre incelenen görüntüde anlamlı miktarda LSB steganografisi uygulandığı tespit edilmiştir. İstatistiksel analizler ve değişiklik haritasındaki belirgin değişimler, görüntüde kapsamlı veri gizleme işlemi yapıldığını güçlü bir şekilde göstermektedir.</p>");
        }
        else if (result.AverageEmbeddingRate > 0.05)
        {
            sb.AppendLine("Analiz sonuçları görüntüde düşük-orta miktarda LSB steganografisi uygulandığını göstermektedir. Tespit edilen değişiklikler rastgele olmaktan çok organize bir desen göstermektedir, ancak bunlar sınırlı miktarda veri gizleme işlemine işaret etmektedir.</p>");
        }
        else
        {
            sb.AppendLine("Analiz sonuçları görüntüde LSB steganografisi kullanıldığına dair güçlü bir kanıt sunmamaktadır. Tespit edilen değişiklikler tipik görüntü işleme veya sıkıştırma artifaktları olarak açıklanabilir.</p>");
        }
        
        sb.AppendLine("  </div>");

        // Footer
        sb.AppendLine("  <div class=\"footer\">");
        sb.AppendLine("    <p>Steganografi Analiz Sistemi - © 2023 Zorlu</p>");
        sb.AppendLine("  </div>");

        // JavaScript kodları
        sb.AppendLine("  <script>");
        sb.AppendLine("    function openTab(evt, tabName) {");
        sb.AppendLine("      var i, tabcontent, tablinks;");
        sb.AppendLine("      tabcontent = document.getElementsByClassName('tab-content');");
        sb.AppendLine("      for (i = 0; i < tabcontent.length; i++) {");
        sb.AppendLine("        tabcontent[i].className = tabcontent[i].className.replace(' active', '');");
        sb.AppendLine("      }");
        sb.AppendLine("      tablinks = document.getElementsByClassName('tab');");
        sb.AppendLine("      for (i = 0; i < tablinks.length; i++) {");
        sb.AppendLine("        tablinks[i].className = tablinks[i].className.replace(' active', '');");
        sb.AppendLine("      }");
        sb.AppendLine("      document.getElementById(tabName).className += ' active';");
        sb.AppendLine("      evt.currentTarget.className += ' active';");
        sb.AppendLine("    }");
        
        // Metin indirme fonksiyonu - büyük metinler için güncellendi
        sb.AppendLine("    function downloadText(filename, text) {");
        sb.AppendLine("      try {");
        sb.AppendLine("        // Çok büyük metinler için Blob kullan");
        sb.AppendLine("        var blob = new Blob([text], {type: 'text/plain;charset=utf-8'});");
        sb.AppendLine("        var url = URL.createObjectURL(blob);");
        sb.AppendLine("        var element = document.createElement('a');");
        sb.AppendLine("        element.setAttribute('href', url);");
        sb.AppendLine("        element.setAttribute('download', filename);");
        sb.AppendLine("        element.style.display = 'none';");
        sb.AppendLine("        document.body.appendChild(element);");
        sb.AppendLine("        element.click();");
        sb.AppendLine("        setTimeout(function() {");
        sb.AppendLine("          document.body.removeChild(element);");
        sb.AppendLine("          URL.revokeObjectURL(url);");
        sb.AppendLine("        }, 100);");
        sb.AppendLine("      } catch (error) {");
        sb.AppendLine("        console.error('İndirme hatası:', error);");
        sb.AppendLine("        alert('Dosya indirilirken bir hata oluştu: ' + error.message);");
        sb.AppendLine("      }");
        sb.AppendLine("    }");
        
        sb.AppendLine("  </script>");
        
        sb.AppendLine("</div>"); // container sonu
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Bitmap'i Base64 formatına çevirir.
    /// </summary>
    private static string BitmapToBase64(Bitmap image, int maxDimension = 1024)
    {
        if (image == null)
            return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII="; // 1x1 şeffaf PNG

        try
        {
            // Çok büyük görüntüleri küçültelim
            Bitmap resizedImage = image;
            bool needResize = false;

            if (image.Width > maxDimension || image.Height > maxDimension)
            {
                try
                {
                    // En-boy oranını koru
                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = maxDimension;
                        height = (int)(image.Height * ((float)maxDimension / image.Width));
                    }
                    else
                    {
                        height = maxDimension;
                        width = (int)(image.Width * ((float)maxDimension / image.Height));
                    }

                    resizedImage = new Bitmap(width, height);
                    needResize = true;

                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(image, 0, 0, width, height);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Görüntü yeniden boyutlandırma hatası: {ex.Message}");
                    // Yeniden boyutlandırma başarısız olursa orijinal görüntüyü kullan
                    resizedImage = image;
                    needResize = false;
                }
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        // İlk olarak JPEG formatını dene
                        ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);

                        resizedImage.Save(ms, jpegEncoder, encoderParams);
                    }
                    catch (Exception jpegEx)
                    {
                        // JPEG başarısız olursa PNG dene
                        Console.WriteLine($"JPEG dönüşümü başarısız: {jpegEx.Message}. PNG deneniyor...");
                        ms.SetLength(0); // Stream'i temizle
                        try
                        {
                            ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
                            resizedImage.Save(ms, pngEncoder, null);
                        }
                        catch (Exception pngEx)
                        {
                            // Her iki format da başarısız olursa 1x1 şeffaf PNG döndür
                            Console.WriteLine($"PNG dönüşümü de başarısız: {pngEx.Message}. Varsayılan görüntü döndürülüyor.");
                            return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
                        }
                    }

                    byte[] imageBytes = ms.ToArray();
                    string base64 = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";

                    if (needResize)
                    {
                        resizedImage.Dispose();
                    }

                    return base64;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Base64 dönüşümü sırasında hata: {ex.Message}");
                // Hata durumunda 1x1 şeffaf PNG döndür
                return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Genel görüntü işleme hatası: {ex.Message}");
        }

        // Tüm dönüşümler başarısız olursa 1x1 şeffaf PNG döndür
        return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";
    }

    /// <summary>
    /// Belirtilen görüntü formatı için codec bilgisini döndürür.
    /// </summary>
    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        throw new ArgumentException($"Desteklenmeyen görüntü formatı: {format}");
    }

    /// <summary>
    /// Byte cinsinden boyutu okunabilir formata çevirir.
    /// </summary>
    private static string FormatFileSize(long byteCount)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = byteCount;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]} ({byteCount:N0} bytes)";
    }

    /// <summary>
    /// HTML'de güvenli gösterim için string değerini kodlar.
    /// </summary>
    private static string HtmlEncode(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
            
        return System.Net.WebUtility.HtmlEncode(input);
    }
}
