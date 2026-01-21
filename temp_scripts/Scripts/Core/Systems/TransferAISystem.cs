using UnityEngine;

/// <summary>
/// Transfer AI Sistemi - TIS hesaplama ve teklif oluşturma (Singleton)
/// </summary>
public class TransferAISystem : MonoBehaviour
{
    public static TransferAISystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[TransferAISystem] TransferAISystem initialized.");
    }

    /// <summary>
    /// TIS (Transfer Interest Score) hesapla - Kulübün oyuncuya olan ilgisini hesaplar
    /// </summary>
    public float CalculateTIS(PlayerProfile player, TeamData team, LeagueData league)
    {
        if (player == null || team == null)
            return 0f;

        float tis = 0f;

        // Oyuncu overall'ına göre (0-40 puan)
        tis += (player.overall / 100f) * 40f;

        // Takım gücüne göre (güçlü takımlar daha az ilgi gösterir)
        int teamPower = team.GetTeamPower();
        float teamPowerFactor = 1f - (teamPower / 100f) * 0.3f; // Maksimum %30 azalma
        tis *= teamPowerFactor;

        // Lig gücüne göre (güçlü ligler daha fazla ilgi gösterir)
        if (league != null)
        {
            float leaguePowerFactor = 1f + (league.leaguePower / 100f) * 0.2f; // Maksimum %20 artış
            tis *= leaguePowerFactor;
        }

        // Pozisyona göre ayarlama (belirli pozisyonlar daha çok aranır)
        float positionFactor = GetPositionDemandFactor(player.position);
        tis *= positionFactor;

        return Mathf.Clamp(tis, 0f, 100f);
    }

    /// <summary>
    /// Pozisyona göre talep faktörü (hangi pozisyonlar daha çok aranıyor)
    /// </summary>
    private float GetPositionDemandFactor(PlayerPosition position)
    {
        return position switch
        {
            PlayerPosition.SF => 1.2f,    // Santraforlar daha çok aranıyor
            PlayerPosition.MOO => 1.15f,  // Ofansif orta saha
            PlayerPosition.SĞK => 1.1f,   // Kanatlar
            PlayerPosition.SLK => 1.1f,
            PlayerPosition.KL => 0.9f,    // Kaleci daha az aranıyor (genelde)
            _ => 1.0f
        };
    }

    /// <summary>
    /// Transfer teklifi oluştur (TIS'e göre)
    /// </summary>
    public TransferOffer GenerateTransferOffer(PlayerProfile player, string clubName, string leagueName, LeagueData league, TeamData team)
    {
        if (player == null)
        {
            Debug.LogWarning("[TransferAISystem] PlayerProfile is null! Cannot generate offer.");
            return null;
        }

        float tis = CalculateTIS(player, team, league);

        // Düşük güçlü takımlar için bonus (TeamOfferUI'de zaten düşük güçlü takımları seçiyoruz)
        int teamPower = team != null ? team.GetTeamPower() : 50;
        if (teamPower < 75)
        {
            // Düşük güçlü takımlar yeni oyunculara daha fazla ilgi gösterir
            float bonus = (75f - teamPower) * 0.3f; // Her güç puanı için +0.3 bonus
            tis += bonus;
            Debug.Log($"[TransferAISystem] Low power team bonus: +{bonus:F1} (Team Power: {teamPower})");
        }

        // TIS eşiği: Yeni oyuncular için daha düşük eşik (10), deneyimli oyuncular için daha yüksek (20)
        float threshold = player.overall < 70 ? 10f : 20f;

        // TIS düşükse teklif oluşturma
        if (tis < threshold)
        {
            Debug.Log($"[TransferAISystem] TIS too low ({tis:F1}) to generate offer. Threshold: {threshold}");
            return null;
        }

        // Maaş hesaplama (overall'a göre)
        int baseSalary = player.overall * 1000; // Her overall puanı için 1000€
        int salaryVariation = Random.Range(-20, 21); // ±20% varyasyon
        int salary = Mathf.RoundToInt(baseSalary * (1f + salaryVariation / 100f));

        // İmza parası (maaşın 2-5 katı)
        int signingBonus = salary * Random.Range(2, 6);

        // Sözleşme süresi (1-5 yıl)
        int contractDuration = Random.Range(1, 6);

        // Oynama süresi (TIS'e göre)
        PlayingTime playingTime = tis > 70f ? PlayingTime.Starter : 
                                   tis > 50f ? PlayingTime.Rotation : 
                                   PlayingTime.Substitute;

        // Transfer teklifi oluştur
        TransferOffer offer = new TransferOffer
        {
            clubName = clubName,
            leagueName = leagueName,
            salary = salary,
            contractDuration = contractDuration,
            playingTime = playingTime,
            role = ConvertPlayingTimeToRole(playingTime),
            signingBonus = signingBonus,
            clauses = new System.Collections.Generic.List<ClauseType>(),
            offerType = TransferOfferType.Permanent,
            offerDate = System.DateTime.Now,
            offerDateString = System.DateTime.Now.ToString("yyyy-MM-dd"),
            isAccepted = false,
            isRejected = false,
            isExpired = false
        };

        // Rastgele maddeler ekle
        if (Random.Range(0f, 1f) > 0.5f)
        {
            offer.clauses.Add(ClauseType.ReleaseClause);
        }

        Debug.Log($"[TransferAISystem] Transfer offer generated for {clubName} (TIS: {tis:F1}, Salary: {salary}€)");
        return offer;
    }

    /// <summary>
    /// PlayingTime'ı ContractRole'a çevir
    /// </summary>
    private ContractRole ConvertPlayingTimeToRole(PlayingTime playingTime)
    {
        return playingTime switch
        {
            PlayingTime.Starter => ContractRole.Starter,
            PlayingTime.Rotation => ContractRole.Rotation,
            PlayingTime.Substitute => ContractRole.Substitute,
            _ => ContractRole.Rotation
        };
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

