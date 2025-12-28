using System.Collections.Generic;

/// <summary>
/// Kulüp verisi - Oyuncunun kulübü ve hedefleri
/// </summary>
[Serializable]
public class ClubData
{
    [Header("Kulüp Bilgileri")]
    public string clubName;                // Kulüp adı
    public string leagueName;              // Lig adı
    
    [Header("Sözleşme")]
    public ContractData contract;          // Sözleşme bilgileri
    
    [Header("Kulüp Hedefleri")]
    public List<ClubObjectiveData> objectives; // Kulüp hedefleri
    
    public ClubData()
    {
        clubName = "";
        leagueName = "";
        contract = new ContractData();
        objectives = new List<ClubObjectiveData>();
    }
}

/// <summary>
/// Kulüp hedefi verisi
/// </summary>
[Serializable]
public class ClubObjectiveData
{
    public ClubObjective objectiveType;    // Hedef türü
    public string description;             // Hedef açıklaması
    public bool isCompleted;               // Tamamlandı mı?
    public int targetValue;                // Hedef değer (örn: lig pozisyonu, gol sayısı)
    public int currentValue;               // Mevcut değer
    
    public ClubObjectiveData()
    {
        objectiveType = ClubObjective.LeaguePosition;
        description = "";
        isCompleted = false;
        targetValue = 0;
        currentValue = 0;
    }
}

