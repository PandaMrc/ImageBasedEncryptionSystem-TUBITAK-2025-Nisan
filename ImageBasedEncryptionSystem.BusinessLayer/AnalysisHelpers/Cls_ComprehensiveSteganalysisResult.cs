// Analiz sonuçlarını topluca tutacak bir sınıf veya struct
using System;
using System.Collections.Generic;
using System.Drawing;

public class Cls_ComprehensiveSteganalysisResult
{
    // Görüntü bilgileri
    public string FileName { get; set; }
    public DateTime AnalysisTimestamp { get; set; }
    public long FileSize { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }

    // Analiz sonuçları
    public RsAnalysisResult RsGroupsResult { get; set; }
    public SpaAnalysisResult SpaResult { get; set; }
    
    // Orijinal resim analiz sonuçları
    public RsAnalysisResult OriginalRsGroupsResult { get; set; }
    public SpaAnalysisResult OriginalSpaResult { get; set; }
    
    // Değişim haritası sonuçları
    public int ChangedPixelCount { get; set; }
    public double ChangeRatio { get; set; }
    
    // Özet hesaplamalar
    public double AverageEmbeddingRate { get; set; }
    public long EstimatedHiddenBits { get; set; }
    public long EstimatedHiddenBytes { get; set; }
    
    // Şifreli görsellerden çıkarılan veriler
    public bool IsSignatureValid { get; set; }
    public string AesEncryptedText { get; set; }
    public string RsaEncryptedAesKey { get; set; }
    
    // Veri Çıkarma İşlemi Detayları
    public int ExtractedDataSize { get; set; } // Byte cinsinden çıkarılan veri boyutu
    public int ProcessedPixelCount { get; set; } // İşlenen piksel sayısı
    public double ExtractedDataDensity { get; set; } // Piksel başına düşen veri yoğunluğu (bit/piksel)
    
    // Ek özellikler ve metrikler
    public ColorChannel AnalyzedChannel { get; set; }
    
    public Cls_ComprehensiveSteganalysisResult()
    {
        FileName = "unknown";
        AnalysisTimestamp = DateTime.Now;
        RsGroupsResult = new RsAnalysisResult();
        SpaResult = new SpaAnalysisResult();
        OriginalRsGroupsResult = new RsAnalysisResult();
        OriginalSpaResult = new SpaAnalysisResult();
        AesEncryptedText = string.Empty;
        RsaEncryptedAesKey = string.Empty;
        ExtractedDataSize = 0;
        ProcessedPixelCount = 0;
        ExtractedDataDensity = 0;
    }
}