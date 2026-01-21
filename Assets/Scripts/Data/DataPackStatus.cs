using System;

/// <summary>
/// DataPack durumu - İndirme ve uygulama durumunu takip eder
/// </summary>
[Serializable]
public enum DataPackStatus
{
    NotDownloaded,  // İndirilmemiş
    Downloaded,     // İndirilmiş ama uygulanmamış
    Active          // Uygulanmış (aktif)
}

/// <summary>
/// DataPack bilgileri ve durumu
/// </summary>
[Serializable]
public class DataPackInfo
{
    public string packId;                    // Unique ID (packName veya hash)
    public string packName;                  // Pack adı
    public string packVersion;               // Versiyon
    public string packAuthor;                // Yazar
    public string packDescription;           // Açıklama
    public DataPackStatus status;            // Durum
    public string localPath;                 // Yerel dosya yolu (indirildiyse)
    public DateTime downloadDate;            // İndirme tarihi
    public bool isInstalled;                 // İndirilmiş mi?
    public bool isActive;                    // Aktif/Uygulanmış mı?
    
    public DataPackInfo()
    {
        status = DataPackStatus.NotDownloaded;
        isInstalled = false;
        isActive = false;
        downloadDate = DateTime.MinValue;
    }
}








