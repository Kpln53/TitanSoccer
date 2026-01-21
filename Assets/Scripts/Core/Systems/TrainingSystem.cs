using UnityEngine;
using System;
using System.Collections.Generic;

public enum TrainingType
{
    Attack,     // Hücum & Bitiricilik
    Physical,   // Fiziksel Kondisyon
    Technique   // Teknik & Pas
}

[Serializable]
public class TrainingSession
{
    public string id;
    public string title;
    public TrainingType type;
    public int energyCost;
    public string statName; // Örn: "Şut", "Güç"
    public int statIncrease;
    public string description;
}

public class TrainingSystem : MonoBehaviour
{
    public static TrainingSystem Instance { get; private set; }

    // Maç başına maksimum antrenman hakkı
    public const int MAX_DAILY_TRAINING = 3;
    public int currentTrainingCount = 0;

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
    /// Antrenmanı uygula
    /// </summary>
    public bool ExecuteTraining(TrainingSession session, out string message)
    {
        // 1. Limit Kontrolü
        if (currentTrainingCount >= MAX_DAILY_TRAINING)
        {
            message = "Bugünlük antrenman hakkın doldu! Maç yapmalısın.";
            return false;
        }

        // 2. Save Verisi Kontrolü
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            message = "Kayıt dosyası bulunamadı.";
            return false;
        }

        var save = GameManager.Instance.CurrentSave;
        var profile = save.playerProfile;

        // 3. Enerji Kontrolü
        if (save.seasonData.energy < session.energyCost)
        {
            message = "Yeterli enerjin yok!";
            return false;
        }

        // --- İŞLEM ---

        // Enerji düş
        save.seasonData.energy -= session.energyCost;
        if (save.seasonData.energy < 0) save.seasonData.energy = 0;

        // Stat artır
        ApplyStatBoost(profile, session.type, session.statIncrease);

        // Moral artır (Antrenman yapmak iyi hissettirir)
        save.seasonData.moral = Mathf.Min(100, save.seasonData.moral + 2);

        // Hakkı azalt
        currentTrainingCount++;

        // Kaydet
        SaveSystem.SaveGame(save, GameManager.Instance.CurrentSaveSlotIndex);

        message = "Antrenman başarılı!";
        return true;
    }

    private void ApplyStatBoost(PlayerProfile profile, TrainingType type, int amount)
    {
        switch (type)
        {
            case TrainingType.Attack:
                profile.shootingSkill += amount;
                // Bazen ekstra stat da artabilir
                if (UnityEngine.Random.value > 0.5f) profile.dribblingSkill += 1;
                break;
            case TrainingType.Physical:
                profile.physicalStrength += amount;
                if (UnityEngine.Random.value > 0.5f) profile.speed += 1;
                break;
            case TrainingType.Technique:
                profile.passingSkill += amount;
                if (UnityEngine.Random.value > 0.5f) profile.falsoSkill += 1;
                break;
        }
    }

    /// <summary>
    /// Maç sonrası antrenman haklarını sıfırla
    /// </summary>
    public void ResetDailyLimit()
    {
        currentTrainingCount = 0;
    }
}
