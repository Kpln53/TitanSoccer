using UnityEngine;

/// <summary>
/// Rüşvet sistemi - Hakem, teknik direktör, yönetim rüşvetleri (Singleton)
/// </summary>
public class BriberySystem : MonoBehaviour
{
    public static BriberySystem Instance { get; private set; }

    [Header("Rüşvet Ayarları")]
    public float baseBribeSuccessChance = 0.7f;    // Temel başarı şansı (%70)
    public float baseBribeDetectionChance = 0.1f;  // Temel yakalanma şansı (%10)
    public int baseBribeCost = 50000;              // Temel rüşvet maliyeti

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

        Debug.Log("[BriberySystem] BriberySystem initialized.");
    }

    /// <summary>
    /// Rüşvet dene
    /// </summary>
    public BriberyResult AttemptBribery(BriberyType briberyType, int bribeAmount, EconomyData economy)
    {
        if (economy == null)
        {
            Debug.LogWarning("[BriberySystem] EconomyData is null! Cannot attempt bribery.");
            return new BriberyResult { success = false, detected = false };
        }

        // Para kontrolü
        if (!economy.HasEnoughMoney(bribeAmount))
        {
            Debug.Log("[BriberySystem] Not enough money for bribe!");
            return new BriberyResult { success = false, detected = false };
        }

        // Para harca
        economy.SpendMoney(bribeAmount);

        // Başarı şansı hesapla
        float successChance = baseBribeSuccessChance;
        successChance += (bribeAmount / baseBribeCost) * 0.2f; // Daha fazla para = daha fazla şans
        successChance = Mathf.Clamp01(successChance);

        bool success = Random.Range(0f, 1f) <= successChance;

        // Yakalanma şansı hesapla
        float detectionChance = baseBribeDetectionChance;
        if (success)
        {
            detectionChance *= 0.5f; // Başarılı olursa yakalanma şansı azalır
        }
        detectionChance = Mathf.Clamp01(detectionChance);

        bool detected = Random.Range(0f, 1f) <= detectionChance;

        BriberyResult result = new BriberyResult
        {
            success = success,
            detected = detected,
            bribeAmount = bribeAmount,
            briberyType = briberyType
        };

        if (detected)
        {
            Debug.LogWarning($"[BriberySystem] Bribery detected! Type: {briberyType}, Amount: {bribeAmount}€");
        }
        else if (success)
        {
            Debug.Log($"[BriberySystem] Bribery successful! Type: {briberyType}, Amount: {bribeAmount}€");
        }
        else
        {
            Debug.Log($"[BriberySystem] Bribery failed. Type: {briberyType}, Amount: {bribeAmount}€");
        }

        return result;
    }

    /// <summary>
    /// Rüşvet türüne göre önerilen miktarı getir
    /// </summary>
    public int GetRecommendedBribeAmount(BriberyType briberyType)
    {
        return briberyType switch
        {
            BriberyType.Referee => baseBribeCost,
            BriberyType.Coach => baseBribeCost * 2,
            BriberyType.Management => baseBribeCost * 3,
            BriberyType.Media => baseBribeCost / 2,
            _ => baseBribeCost
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

/// <summary>
/// Rüşvet türü
/// </summary>
public enum BriberyType
{
    Referee,        // Hakem
    Coach,          // Teknik direktör
    Management,     // Yönetim
    Media           // Medya
}

/// <summary>
/// Rüşvet sonucu
/// </summary>
[System.Serializable]
public class BriberyResult
{
    public bool success;           // Başarılı mı?
    public bool detected;          // Yakalandı mı?
    public int bribeAmount;        // Rüşvet miktarı
    public BriberyType briberyType; // Rüşvet türü
}






