using System;

/// <summary>
/// Ana kayıt dosyası - Tüm oyun verilerini içerir
/// </summary>
[Serializable]
public class SaveData
{
    [Header("Kayıt Bilgileri")]
    public string saveName;              // Kayıt adı (kullanıcı tarafından verilebilir)
    public DateTime saveDate;            // Kayıt tarihi
    public string saveDateString;        // Tarih (string)
    public string version;               // Kayıt versiyonu
    
    [Header("Oyuncu Verileri")]
    public PlayerProfile playerProfile;  // Oyuncu profili
    
    [Header("Kulüp Verileri")]
    public ClubData clubData;           // Kulüp bilgileri
    
    [Header("Sezon Verileri")]
    public SeasonData seasonData;       // Sezon istatistikleri
    
    [Header("İlişkiler")]
    public RelationsData relationsData;  // İlişkiler
    
    [Header("Ekonomi")]
    public EconomyData economyData;      // Para ve eşyalar
    
    [Header("Medya")]
    public MediaData mediaData;         // Haberler ve sosyal medya
    
    public SaveData()
    {
        saveName = "New Career";
        saveDate = DateTime.Now;
        saveDateString = saveDate.ToString("yyyy-MM-dd HH:mm:ss");
        version = "1.0.0";
        
        playerProfile = new PlayerProfile();
        clubData = new ClubData();
        seasonData = new SeasonData();
        relationsData = new RelationsData();
        economyData = new EconomyData();
        mediaData = new MediaData();
    }
    
    /// <summary>
    /// Kayıt tarihini güncelle
    /// </summary>
    public void UpdateSaveDate()
    {
        saveDate = DateTime.Now;
        saveDateString = saveDate.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
