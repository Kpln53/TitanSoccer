using UnityEngine;

/// <summary>
/// Antreman sistemi - Maç başı 2 hak
/// </summary>
public class TrainingSystem : MonoBehaviour
{
    public static TrainingSystem Instance;

    [Header("Antreman Ayarları")]
    [SerializeField] private int maxTrainingPerMatch = 2; // Maç başı maksimum antreman
    [SerializeField] private float baseStatGain = 0.5f; // Temel stat artışı

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
    /// Antreman yap
    /// </summary>
    public TrainingResult DoTraining(PlayerProfile profile, TrainingType type, int difficulty)
    {
        TrainingResult result = new TrainingResult();
        
        // Antreman hakkı kontrolü
        if (profile.seasonStats.matchesPlayed == 0)
        {
            // İlk maç öncesi, antreman hakkı yok
            result.success = false;
            result.message = "İlk maç öncesi antreman yapılamaz.";
            return result;
        }
        
        // Antreman sayısını kontrol et (maç başı 2)
        // Bu bilgiyi SaveData'da tutmamız gerekir, şimdilik basit kontrol
        
        // Zorluk seviyesine göre stat artışı
        float statGain = baseStatGain * difficulty;
        
        // Başarı şansı (enerji ve moral'e bağlı)
        float successChance = 0.5f + (profile.energy * 0.3f) + (profile.morale * 0.2f);
        successChance = Mathf.Clamp01(successChance);
        
        bool success = Random.Range(0f, 1f) < successChance;
        
        if (success)
        {
            // Stat artışı uygula
            ApplyStatGain(profile, type, statGain);
            result.success = true;
            result.message = "Antreman başarılı!";
            result.statGained = statGain;
        }
        else
        {
            result.success = false;
            result.message = "Antreman başarısız.";
        }
        
        // Enerji tüketimi
        profile.energy = Mathf.Clamp01(profile.energy - 0.1f);
        
        // Sakatlık kontrolü
        if (InjurySystem.Instance != null)
        {
            var injury = InjurySystem.Instance.CheckTrainingInjury(profile);
            if (injury != null)
            {
                result.injury = injury;
            }
        }
        
        return result;
    }

    /// <summary>
    /// Stat artışı uygula
    /// </summary>
    private void ApplyStatGain(PlayerProfile profile, TrainingType type, float gain)
    {
        int gainInt = Mathf.RoundToInt(gain);
        
        switch (type)
        {
            case TrainingType.Passing:
                profile.passing = Mathf.Clamp(profile.passing + gainInt, 0, 100);
                break;
            case TrainingType.Shooting:
                profile.shooting = Mathf.Clamp(profile.shooting + gainInt, 0, 100);
                break;
            case TrainingType.Dribbling:
                profile.dribbling = Mathf.Clamp(profile.dribbling + gainInt, 0, 100);
                break;
            case TrainingType.Speed:
                profile.pace = Mathf.Clamp(profile.pace + gainInt, 0, 100);
                break;
            case TrainingType.Defense:
                profile.defense = Mathf.Clamp(profile.defense + gainInt, 0, 100);
                break;
            case TrainingType.Stamina:
                profile.stamina = Mathf.Clamp(profile.stamina + gainInt, 0, 100);
                break;
        }
        
        // Overall'ı yeniden hesapla
        profile.CalculateOverall();
    }
}

/// <summary>
/// Antreman sonucu
/// </summary>
[System.Serializable]
public class TrainingResult
{
    public bool success;
    public string message;
    public float statGained;
    public InjuryResult injury;
}

public enum TrainingType
{
    Passing,    // Pas antremanı
    Shooting,  // Şut antremanı
    Dribbling, // Dribling antremanı
    Speed,     // Sürat antremanı
    Defense,   // Defans antremanı
    Stamina    // Dayanıklılık antremanı
}


