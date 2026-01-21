using UnityEngine;

/// <summary>
/// Form, Moral ve Enerji sistemi - Oyuncunun durumunu yönetir (Singleton)
/// </summary>
public class FormMoralEnergySystem : MonoBehaviour
{
    public static FormMoralEnergySystem Instance { get; private set; }

    [Header("Enerji İçeceği Ayarları")]
    public int energyDrinkRestoreAmount = 20; // Enerji içeceği ne kadar enerji verir

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

        Debug.Log("[FormMoralEnergySystem] FormMoralEnergySystem initialized.");
    }

    /// <summary>
    /// Form'u güncelle (maç performansına göre)
    /// </summary>
    public void UpdateForm(PlayerProfile profile, float matchRating)
    {
        if (profile == null)
            return;

        // Rating'e göre form artır/azalt
        float formChange = (matchRating - 5.0f) * 2f; // 5.0 = ortalama, ±2 form değişimi
        profile.form = Mathf.Clamp(profile.form + formChange, 0f, 100f);

        Debug.Log($"[FormMoralEnergySystem] Form updated: {profile.form:F1} (change: {formChange:+.1f})");
    }

    /// <summary>
    /// Moral'i güncelle (takım başarısına, ilişkilere göre)
    /// </summary>
    public void UpdateMoral(PlayerProfile profile, RelationsData relations, int teamWins, int teamLosses)
    {
        if (profile == null || relations == null)
            return;

        float moralChange = 0f;

        // Takım başarısına göre
        if (teamWins > teamLosses)
            moralChange += 2f;
        else if (teamLosses > teamWins)
            moralChange -= 2f;

        // İlişkilere göre
        moralChange += relations.coachRelation * 0.1f;
        moralChange += relations.managementRelation * 0.1f;
        moralChange = Mathf.Clamp(moralChange, -5f, 5f); // Maksimum değişim sınırı

        profile.moral = Mathf.Clamp(profile.moral + moralChange, 0f, 100f);

        Debug.Log($"[FormMoralEnergySystem] Moral updated: {profile.moral:F1} (change: {moralChange:+.1f})");
    }

    /// <summary>
    /// Enerjiyi güncelle (maç sonrası, antrenman sonrası)
    /// </summary>
    public void UpdateEnergy(PlayerProfile profile, float energyLoss)
    {
        if (profile == null)
            return;

        profile.energy = Mathf.Clamp(profile.energy - energyLoss, 0f, 100f);

        Debug.Log($"[FormMoralEnergySystem] Energy updated: {profile.energy:F1} (loss: {energyLoss:F1})");
    }

    /// <summary>
    /// Enerji içeceği kullan
    /// </summary>
    public bool UseEnergyDrink(PlayerProfile profile, EconomyData economy)
    {
        if (profile == null || economy == null)
            return false;

        if (economy.energyDrinkCount <= 0)
        {
            Debug.Log("[FormMoralEnergySystem] No energy drinks available!");
            return false;
        }

        economy.energyDrinkCount--;
        profile.energy = Mathf.Clamp(profile.energy + energyDrinkRestoreAmount, 0f, 100f);

        Debug.Log($"[FormMoralEnergySystem] Energy drink used. Energy: {profile.energy:F1}");
        return true;
    }

    /// <summary>
    /// Enerjiyi dinlenme ile restore et
    /// </summary>
    public void RestoreEnergyByRest(PlayerProfile profile, float restoreAmount)
    {
        if (profile == null)
            return;

        profile.energy = Mathf.Clamp(profile.energy + restoreAmount, 0f, 100f);

        Debug.Log($"[FormMoralEnergySystem] Energy restored by rest: {profile.energy:F1} (+{restoreAmount:F1})");
    }

    /// <summary>
    /// Toplam durum skorunu getir (form + moral + energy) / 3
    /// </summary>
    public float GetOverallCondition(PlayerProfile profile)
    {
        if (profile == null)
            return 0f;

        return (profile.form + profile.moral + profile.energy) / 3f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}








