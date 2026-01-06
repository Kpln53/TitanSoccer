using UnityEngine;

/// <summary>
/// Menajer AI Sistemi - Deal Score ve pazarlık (Singleton)
/// </summary>
public class ManagerAISystem : MonoBehaviour
{
    public static ManagerAISystem Instance { get; private set; }

    [Header("Menajer Ayarları")]
    public float strictManagerNegotiation = 0.8f;      // Katı menajer pazarlık gücü
    public float supportiveManagerNegotiation = 1.2f;  // Destekleyici menajer pazarlık gücü
    public float balancedManagerNegotiation = 1.0f;    // Dengeli menajer pazarlık gücü

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

        Debug.Log("[ManagerAISystem] ManagerAISystem initialized.");
    }

    /// <summary>
    /// Deal Score hesapla - Teklifin ne kadar iyi olduğunu değerlendir (0-100)
    /// </summary>
    public float CalculateDealScore(TransferOffer offer, PlayerProfile player, ManagerType managerType)
    {
        if (offer == null || player == null)
            return 0f;

        float score = 0f;

        // Maaş değerlendirmesi (0-40 puan)
        int expectedSalary = player.overall * 1000;
        float salaryRatio = (float)offer.salary / expectedSalary;
        score += Mathf.Clamp01(salaryRatio) * 40f;

        // Oynama süresi değerlendirmesi (0-30 puan)
        float playingTimeScore = offer.playingTime switch
        {
            PlayingTime.Starter => 30f,
            PlayingTime.Rotation => 20f,
            PlayingTime.Substitute => 10f,
            _ => 15f
        };
        score += playingTimeScore;

        // İmza parası değerlendirmesi (0-20 puan)
        int expectedBonus = expectedSalary * 3;
        float bonusRatio = (float)offer.signingBonus / expectedBonus;
        score += Mathf.Clamp01(bonusRatio) * 20f;

        // Sözleşme süresi değerlendirmesi (0-10 puan)
        // Uzun sözleşmeler genelde daha iyi (güvenlik)
        float durationScore = Mathf.Clamp01((float)offer.contractDuration / 5f) * 10f;
        score += durationScore;

        // Menajer tipine göre ayarlama
        float negotiationFactor = GetNegotiationFactor(managerType);
        score *= negotiationFactor;

        return Mathf.Clamp(score, 0f, 100f);
    }

    /// <summary>
    /// Menajer tipine göre pazarlık faktörü
    /// </summary>
    private float GetNegotiationFactor(ManagerType managerType)
    {
        return managerType switch
        {
            ManagerType.Strict => strictManagerNegotiation,
            ManagerType.Supportive => supportiveManagerNegotiation,
            ManagerType.Balanced => balancedManagerNegotiation,
            _ => 1.0f
        };
    }

    /// <summary>
    /// Teklifi değerlendir (kabul edilmeli mi?)
    /// </summary>
    public bool EvaluateOffer(TransferOffer offer, PlayerProfile player, ManagerType managerType, float minimumScore = 60f)
    {
        float dealScore = CalculateDealScore(offer, player, managerType);
        return dealScore >= minimumScore;
    }

    /// <summary>
    /// Pazarlık yap (teklifi iyileştirmeye çalış)
    /// </summary>
    public TransferOffer Negotiate(TransferOffer originalOffer, PlayerProfile player, ManagerType managerType)
    {
        if (originalOffer == null || player == null)
            return null;

        TransferOffer negotiatedOffer = new TransferOffer
        {
            clubName = originalOffer.clubName,
            leagueName = originalOffer.leagueName,
            salary = originalOffer.salary,
            contractDuration = originalOffer.contractDuration,
            playingTime = originalOffer.playingTime,
            role = originalOffer.role,
            signingBonus = originalOffer.signingBonus,
            clauses = new System.Collections.Generic.List<ClauseType>(originalOffer.clauses),
            offerType = originalOffer.offerType,
            offerDate = originalOffer.offerDate,
            offerDateString = originalOffer.offerDateString
        };

        float negotiationFactor = GetNegotiationFactor(managerType);

        // Maaşı artırmaya çalış (%5-15)
        int salaryIncrease = Mathf.RoundToInt(negotiatedOffer.salary * (negotiationFactor - 1f) * Random.Range(0.5f, 1.5f));
        negotiatedOffer.salary += salaryIncrease;

        // İmza parasını artırmaya çalış
        int bonusIncrease = Mathf.RoundToInt(negotiatedOffer.signingBonus * (negotiationFactor - 1f) * Random.Range(0.5f, 1.5f));
        negotiatedOffer.signingBonus += bonusIncrease;

        Debug.Log($"[ManagerAISystem] Offer negotiated. Salary: {originalOffer.salary}€ -> {negotiatedOffer.salary}€");
        return negotiatedOffer;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}






