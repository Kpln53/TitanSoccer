using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Transfer sistemi - Transfer teklifleri ve kabul/ret
/// </summary>
public class TransferSystem : MonoBehaviour
{
    public static TransferSystem Instance;

    [Header("Transfer Ayarları")]
    [SerializeField] private float baseTransferInterestChance = 0.1f; // Hafta başı temel ilgi şansı
    [SerializeField] private int minTransferWindowWeek = 1; // Transfer dönemi başlangıcı
    [SerializeField] private int maxTransferWindowWeek = 4; // Transfer dönemi bitişi

    private List<TransferOffer> pendingOffers = new List<TransferOffer>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Hafta başı transfer ilgisi kontrolü
    /// </summary>
    public void CheckTransferInterest(PlayerProfile profile, ClubData clubData, SeasonData seasonData)
    {
        // Transfer dönemi kontrolü
        if (!IsTransferWindow(seasonData.currentWeek))
        {
            return;
        }

        // Performansa göre ilgi şansı
        float interestChance = baseTransferInterestChance;
        
        // Form yüksekse şans artar
        interestChance += profile.form * 0.1f;
        
        // Popülerlik yüksekse şans artar
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            interestChance += GameManager.Instance.CurrentSave.mediaData.popularity * 0.1f;
        }
        
        // OVR yüksekse şans artar
        interestChance += (profile.overall / 100f) * 0.1f;
        
        if (Random.Range(0f, 1f) < interestChance)
        {
            GenerateTransferOffer(profile, clubData);
        }
    }

    /// <summary>
    /// Transfer teklifi oluştur
    /// </summary>
    private void GenerateTransferOffer(PlayerProfile profile, ClubData currentClub)
    {
        // Rastgele bir takım seç (Data Pack'ten)
        TeamData offerTeam = GetRandomTeamForOffer(profile, currentClub);
        
        if (offerTeam == null)
            return;
        
        TransferOffer offer = new TransferOffer
        {
            teamName = offerTeam.teamName,
            teamId = offerTeam.teamName, // ID olarak isim kullan
            monthlySalary = CalculateOfferSalary(profile, offerTeam),
            contractDuration = Random.Range(12, 36), // 12-36 ay
            playingTime = DeterminePlayingTime(profile, offerTeam),
            transferWindow = TransferWindowType.Main
        };
        
        pendingOffers.Add(offer);
    }

    /// <summary>
    /// Teklif için rastgele takım seç
    /// </summary>
    private TeamData GetRandomTeamForOffer(PlayerProfile profile, ClubData currentClub)
    {
        if (DataPackManager.Instance == null || DataPackManager.Instance.GetActiveDataPack() == null)
            return null;
        
        var dataPack = DataPackManager.Instance.GetActiveDataPack();
        var allTeams = dataPack.GetAllTeams();
        
        if (allTeams == null || allTeams.Count == 0)
            return null;
        
        // Mevcut takımı hariç tut
        List<TeamData> availableTeams = new List<TeamData>();
        foreach (var team in allTeams)
        {
            if (team.teamName != currentClub.clubName)
            {
                availableTeams.Add(team);
            }
        }
        
        if (availableTeams.Count == 0)
            return null;
        
        // OVR'a göre uygun takımları filtrele
        List<TeamData> suitableTeams = new List<TeamData>();
        foreach (var team in availableTeams)
        {
            int teamPower = team.GetTeamPower();
            // Oyuncunun OVR'ı takım gücüne yakınsa uygun
            if (Mathf.Abs(profile.overall - teamPower) < 15)
            {
                suitableTeams.Add(team);
            }
        }
        
        if (suitableTeams.Count == 0)
            suitableTeams = availableTeams; // Uygun takım yoksa hepsini kullan
        
        return suitableTeams[Random.Range(0, suitableTeams.Count)];
    }

    /// <summary>
    /// Teklif maaşını hesapla
    /// </summary>
    private int CalculateOfferSalary(PlayerProfile profile, TeamData team)
    {
        int baseSalary = 10000;
        
        // OVR'a göre maaş
        baseSalary += profile.overall * 500;
        
        // Takım gücüne göre maaş
        int teamPower = team.GetTeamPower();
        baseSalary += teamPower * 200;
        
        // Rastgele varyasyon
        baseSalary = Mathf.RoundToInt(baseSalary * Random.Range(0.9f, 1.1f));
        
        return baseSalary;
    }

    /// <summary>
    /// Oynama zamanını belirle
    /// </summary>
    private PlayingTime DeterminePlayingTime(PlayerProfile profile, TeamData team)
    {
        int teamPower = team.GetTeamPower();
        int diff = profile.overall - teamPower;
        
        if (diff > 5)
            return PlayingTime.Starter;
        else if (diff > -5)
            return PlayingTime.Rotation;
        else
            return PlayingTime.Substitute;
    }

    /// <summary>
    /// Transfer dönemi mi?
    /// </summary>
    public bool IsTransferWindow(int week)
    {
        // Yaz transfer dönemi (hafta 1-4)
        if (week >= minTransferWindowWeek && week <= maxTransferWindowWeek)
            return true;
        
        // Ara transfer dönemi (hafta 19-22) - ileride eklenebilir
        // if (week >= 19 && week <= 22)
        //     return true;
        
        return false;
    }

    /// <summary>
    /// Bekleyen teklifleri al
    /// </summary>
    public List<TransferOffer> GetPendingOffers()
    {
        return new List<TransferOffer>(pendingOffers);
    }

    /// <summary>
    /// Teklifi kabul et
    /// </summary>
    public void AcceptOffer(TransferOffer offer, SaveData saveData)
    {
        // Kulüp değiştir
        saveData.clubData.clubName = offer.teamName;
        saveData.clubData.clubId = offer.teamId;
        saveData.clubData.contract.monthlySalary = offer.monthlySalary;
        saveData.clubData.contract.contractDuration = offer.contractDuration;
        saveData.clubData.contract.remainingMonths = offer.contractDuration;
        saveData.clubData.contract.playingTime = offer.playingTime;
        
        // Teklifi listeden çıkar
        pendingOffers.Remove(offer);
        
        // Haber oluştur
        if (NewsSystem.Instance != null)
        {
            NewsSystem.Instance.CreateTransferNews(saveData.playerProfile.playerName, offer.teamName, true);
        }
    }

    /// <summary>
    /// Teklifi reddet
    /// </summary>
    public void RejectOffer(TransferOffer offer)
    {
        pendingOffers.Remove(offer);
    }
}

/// <summary>
/// Transfer teklifi
/// </summary>
[System.Serializable]
public class TransferOffer
{
    public string teamName;
    public string teamId;
    public int monthlySalary;
    public int contractDuration; // Ay cinsinden
    public PlayingTime playingTime;
    public TransferWindowType transferWindow;
}

public enum TransferWindowType
{
    Main,    // Yaz transfer dönemi
    Mid      // Ara transfer dönemi
}


