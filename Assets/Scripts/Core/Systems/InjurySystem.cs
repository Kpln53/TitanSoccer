using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sakatlık sistemi - Sakatlık oluşturma ve iyileşme (Singleton)
/// </summary>
public class InjurySystem : MonoBehaviour
{
    public static InjurySystem Instance { get; private set; }

    [Header("Sakatlık Ayarları")]
    public float baseInjuryChance = 0.05f; // Temel sakatlık şansı (5%)

    private string[] injuryNames = {
        "Hamstring Strain", "Ankle Sprain", "Knee Injury", "Groin Strain",
        "Shoulder Injury", "Muscle Tear", "Concussion", "Fracture"
    };

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

        Debug.Log("[InjurySystem] InjurySystem initialized.");
    }

    /// <summary>
    /// Rastgele sakatlık oluştur
    /// </summary>
    public InjuryRecord GenerateInjury(PlayerProfile profile, float injuryChance = -1f)
    {
        if (profile == null)
        {
            Debug.LogWarning("[InjurySystem] PlayerProfile is null! Cannot generate injury.");
            return null;
        }

        // Şans verilmediyse base chance kullan
        if (injuryChance < 0f)
        {
            injuryChance = baseInjuryChance;
        }

        // Sakatlık şansı kontrolü
        if (Random.Range(0f, 1f) > injuryChance)
        {
            return null; // Sakatlık oluşmadı
        }

        // Sakatlık şiddeti (1-10)
        int severity = Random.Range(1, 11);

        // Sakatlık süresi (severity'ye göre 3-60 gün)
        int durationDays = severity * 3 + Random.Range(0, 10);

        // Sakatlık adı
        string injuryName = injuryNames[Random.Range(0, injuryNames.Length)];

        // Sakatlık kaydı oluştur
        InjuryRecord injury = new InjuryRecord
        {
            injuryName = injuryName,
            injuryDescription = $"Player suffered from {injuryName}",
            severity = severity,
            durationDays = durationDays,
            daysRemaining = durationDays,
            injuryDate = System.DateTime.Now,
            injuryDateString = System.DateTime.Now.ToString("yyyy-MM-dd"),
            recoveryDate = System.DateTime.Now.AddDays(durationDays),
            recoveryDateString = System.DateTime.Now.AddDays(durationDays).ToString("yyyy-MM-dd"),
            isRecovered = false,
            isRecovering = false
        };

        // Oyuncuya sakatlık ekle
        profile.currentInjury = injury;
        profile.injuryHistory.Add(injury);

        Debug.Log($"[InjurySystem] Injury generated: {injuryName} ({severity}/10 severity, {durationDays} days)");
        return injury;
    }

    /// <summary>
    /// Sakatlıktan iyileş (günlük güncelleme)
    /// </summary>
    public void ProcessInjuryRecovery(PlayerProfile profile)
    {
        if (profile == null || profile.currentInjury == null || profile.currentInjury.isRecovered)
            return;

        InjuryRecord injury = profile.currentInjury;

        // Gün azalt
        injury.daysRemaining = Mathf.Max(0, injury.daysRemaining - 1);

        // İyileşti mi kontrol et
        if (injury.daysRemaining <= 0)
        {
            injury.isRecovered = true;
            injury.isRecovering = false;
            profile.currentInjury = null;

            Debug.Log($"[InjurySystem] Player recovered from injury: {injury.injuryName}");
        }
    }

    /// <summary>
    /// Rehabilitasyon ürünü kullanarak sakatlık süresini azalt
    /// </summary>
    public bool UseRehabItem(PlayerProfile profile, EconomyData economy)
    {
        if (profile == null || economy == null)
            return false;

        if (profile.currentInjury == null || profile.currentInjury.isRecovered)
        {
            Debug.Log("[InjurySystem] No active injury to treat!");
            return false;
        }

        if (economy.rehabItemCount <= 0)
        {
            Debug.Log("[InjurySystem] No rehab items available!");
            return false;
        }

        economy.rehabItemCount--;
        
        // Sakatlık süresini 3 gün azalt
        int daysReduced = 3;
        profile.currentInjury.daysRemaining = Mathf.Max(0, profile.currentInjury.daysRemaining - daysReduced);
        profile.currentInjury.durationDays = Mathf.Max(0, profile.currentInjury.durationDays - daysReduced);

        // İyileşti mi kontrol et
        if (profile.currentInjury.daysRemaining <= 0)
        {
            profile.currentInjury.isRecovered = true;
            profile.currentInjury.isRecovering = false;
            profile.currentInjury = null;
            Debug.Log("[InjurySystem] Player recovered using rehab item!");
        }
        else
        {
            Debug.Log($"[InjurySystem] Rehab item used. {daysReduced} days reduced. Remaining: {profile.currentInjury.daysRemaining} days");
        }

        return true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}







