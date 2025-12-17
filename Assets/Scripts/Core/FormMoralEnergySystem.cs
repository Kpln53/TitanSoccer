using UnityEngine;

/// <summary>
/// Form, Moral ve Enerji yönetim sistemi
/// </summary>
public class FormMoralEnergySystem : MonoBehaviour
{
    public static FormMoralEnergySystem Instance;

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
    /// Form'u güncelle (maç rating'ine göre)
    /// </summary>
    public void UpdateForm(PlayerProfile profile, float matchRating)
    {
        // Form, son maçların ortalamasına göre belirlenir
        // Rating 6.0 altı → form düşer
        // Rating 7.0+ → form yükselir
        
        float formChange = 0f;
        
        if (matchRating < 6.0f)
        {
            formChange = -0.05f; // Form düşer
        }
        else if (matchRating >= 7.0f)
        {
            formChange = 0.05f; // Form yükselir
        }
        else
        {
            formChange = 0f; // Değişmez
        }
        
        profile.form = Mathf.Clamp01(profile.form + formChange);
    }

    /// <summary>
    /// Moral'ı güncelle
    /// </summary>
    public void UpdateMorale(PlayerProfile profile, MoraleChangeReason reason, float amount)
    {
        profile.morale = Mathf.Clamp01(profile.morale + amount);
    }

    /// <summary>
    /// Enerji'yi güncelle
    /// </summary>
    public void UpdateEnergy(PlayerProfile profile, float energyChange)
    {
        profile.energy = Mathf.Clamp01(profile.energy + energyChange);
    }

    /// <summary>
    /// Maç sonrası enerji tüketimi
    /// </summary>
    public void ConsumeEnergyAfterMatch(PlayerProfile profile, int minutesPlayed)
    {
        // Oynanan dakikaya göre enerji tüketimi
        float energyLoss = (minutesPlayed / 90f) * 0.4f; // 90 dakika = %40 enerji kaybı
        profile.energy = Mathf.Clamp01(profile.energy - energyLoss);
    }

    /// <summary>
    /// Hafta başı enerji yenileme
    /// </summary>
    public void RestoreEnergyWeekly(PlayerProfile profile)
    {
        // Hafta başında enerji %80'e kadar yenilenir
        if (profile.energy < 0.8f)
        {
            profile.energy = Mathf.Min(0.8f, profile.energy + 0.3f);
        }
    }

    /// <summary>
    /// Enerji içeceği kullan
    /// </summary>
    public bool UseEnergyDrink(PlayerProfile profile, EconomyData economy)
    {
        if (economy.energyDrinks > 0)
        {
            economy.energyDrinks--;
            profile.energy = Mathf.Clamp01(profile.energy + 0.3f); // %30 enerji artışı
            return true;
        }
        return false;
    }
}

public enum MoraleChangeReason
{
    MatchWin,
    MatchLoss,
    GoodPerformance,
    BadPerformance,
    TransferInterest,
    CoachPraise,
    CoachCriticism,
    TeamRelationship,
    NewsPositive,
    NewsNegative
}


