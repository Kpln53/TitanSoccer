using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sakatlık sistemi
/// </summary>
public class InjurySystem : MonoBehaviour
{
    public static InjurySystem Instance;

    [Header("Sakatlık Ayarları")]
    [SerializeField] private float baseInjuryChance = 0.05f; // %5 temel sakatlık şansı
    [SerializeField] private float matchInjuryChance = 0.03f; // Maç başı sakatlık şansı
    [SerializeField] private float trainingInjuryChance = 0.02f; // Antreman başı sakatlık şansı

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
    /// Maç sonrası sakatlık kontrolü
    /// </summary>
    public InjuryResult CheckMatchInjury(PlayerProfile profile, int minutesPlayed)
    {
        // Sakatlık eğilimi yüksekse şans artar
        float injuryChance = matchInjuryChance;
        injuryChance += (profile.injuryProne / 100f) * 0.05f; // Eğilim %5'e kadar ekler
        
        // Enerji düşükse şans artar
        if (profile.energy < 0.3f)
        {
            injuryChance += 0.02f;
        }
        
        // Uzun süre oynadıysa şans artar
        if (minutesPlayed > 80)
        {
            injuryChance += 0.01f;
        }
        
        if (Random.Range(0f, 1f) < injuryChance)
        {
            return GenerateInjury(profile, InjurySource.Match);
        }
        
        return null;
    }

    /// <summary>
    /// Antreman sonrası sakatlık kontrolü
    /// </summary>
    public InjuryResult CheckTrainingInjury(PlayerProfile profile)
    {
        float injuryChance = trainingInjuryChance;
        injuryChance += (profile.injuryProne / 100f) * 0.03f;
        
        if (profile.energy < 0.3f)
        {
            injuryChance += 0.02f;
        }
        
        if (Random.Range(0f, 1f) < injuryChance)
        {
            return GenerateInjury(profile, InjurySource.Training);
        }
        
        return null;
    }

    /// <summary>
    /// Sakatlık oluştur
    /// </summary>
    private InjuryResult GenerateInjury(PlayerProfile profile, InjurySource source)
    {
        InjuryResult injury = new InjuryResult();
        
        // Sakatlık şiddeti belirle
        float severityRoll = Random.Range(0f, 1f);
        
        if (severityRoll < 0.6f) // %60 hafif
        {
            injury.severity = InjurySeverity.Light;
            injury.durationDays = Random.Range(3, 7);
        }
        else if (severityRoll < 0.9f) // %30 orta
        {
            injury.severity = InjurySeverity.Medium;
            injury.durationDays = Random.Range(7, 21);
        }
        else // %10 ağır
        {
            injury.severity = InjurySeverity.Severe;
            injury.durationDays = Random.Range(21, 60);
        }
        
        injury.source = source;
        injury.injuryType = GetInjuryTypeName(injury.severity);
        
        // Profile kaydet
        InjuryRecord record = new InjuryRecord
        {
            injuryType = injury.injuryType,
            durationDays = injury.durationDays,
            date = System.DateTime.Now.ToString("yyyy-MM-dd"),
            isRecovered = false
        };
        
        profile.injuryHistory.Add(record);
        
        return injury;
    }

    private string GetInjuryTypeName(InjurySeverity severity)
    {
        switch (severity)
        {
            case InjurySeverity.Light:
                return "Hafif";
            case InjurySeverity.Medium:
                return "Orta";
            case InjurySeverity.Severe:
                return "Ağır";
            default:
                return "Bilinmeyen";
        }
    }

    /// <summary>
    /// Rehab item kullan
    /// </summary>
    public bool UseRehabItem(PlayerProfile profile, EconomyData economy)
    {
        if (economy.rehabItems > 0 && HasActiveInjury(profile))
        {
            economy.rehabItems--;
            
            // En son sakatlığı iyileştir
            if (profile.injuryHistory.Count > 0)
            {
                var lastInjury = profile.injuryHistory[profile.injuryHistory.Count - 1];
                if (!lastInjury.isRecovered)
                {
                    lastInjury.durationDays = Mathf.Max(1, lastInjury.durationDays - 3); // 3 gün azalt
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Aktif sakatlık var mı?
    /// </summary>
    public bool HasActiveInjury(PlayerProfile profile)
    {
        if (profile.injuryHistory.Count == 0)
            return false;
        
        var lastInjury = profile.injuryHistory[profile.injuryHistory.Count - 1];
        return !lastInjury.isRecovered;
    }

    /// <summary>
    /// Sakatlık günlerini ilerlet (hafta geçişi)
    /// </summary>
    public void ProgressInjuries(PlayerProfile profile, int daysPassed)
    {
        foreach (var injury in profile.injuryHistory)
        {
            if (!injury.isRecovered)
            {
                injury.durationDays -= daysPassed;
                if (injury.durationDays <= 0)
                {
                    injury.isRecovered = true;
                    injury.durationDays = 0;
                }
            }
        }
    }
}

/// <summary>
/// Sakatlık sonucu
/// </summary>
[System.Serializable]
public class InjuryResult
{
    public InjurySeverity severity;
    public int durationDays;
    public string injuryType;
    public InjurySource source;
}

public enum InjurySeverity
{
    Light,   // Hafif (3-7 gün)
    Medium,  // Orta (7-21 gün)
    Severe   // Ağır (21-60 gün)
}

public enum InjurySource
{
    Match,
    Training,
    Other
}

