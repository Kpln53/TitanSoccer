using UnityEngine;

/// <summary>
/// Antrenman sistemi - Skill artırma (Singleton)
/// </summary>
public class TrainingSystem : MonoBehaviour
{
    public static TrainingSystem Instance { get; private set; }

    [Header("Antrenman Ayarları")]
    public float baseSkillIncrease = 1f;      // Temel skill artışı
    public float energyCostPerTraining = 10f; // Antrenman başına enerji maliyeti

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

        Debug.Log("[TrainingSystem] TrainingSystem initialized.");
    }

    /// <summary>
    /// Belirli bir yeteneği antrenman yaparak artır
    /// </summary>
    public bool TrainSkill(PlayerProfile profile, string skillName, float energyCostMultiplier = 1f)
    {
        if (profile == null)
        {
            Debug.LogWarning("[TrainingSystem] PlayerProfile is null! Cannot train.");
            return false;
        }

        float energyCost = energyCostPerTraining * energyCostMultiplier;

        // Enerji kontrolü
        if (profile.energy < energyCost)
        {
            Debug.Log($"[TrainingSystem] Not enough energy to train! Need: {energyCost}, Have: {profile.energy}");
            return false;
        }

        // Enerji harca
        profile.energy -= energyCost;

        // Skill artır (pozisyona göre maksimum değişir)
        float skillIncrease = baseSkillIncrease;
        bool skillIncreased = false;

        switch (skillName.ToLower())
        {
            case "passing":
                if (profile.passingSkill < 100)
                {
                    profile.passingSkill = Mathf.Clamp(profile.passingSkill + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "shooting":
                if (profile.shootingSkill < 100)
                {
                    profile.shootingSkill = Mathf.Clamp(profile.shootingSkill + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "dribbling":
                if (profile.dribblingSkill < 100)
                {
                    profile.dribblingSkill = Mathf.Clamp(profile.dribblingSkill + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "falso":
                if (profile.falsoSkill < 100)
                {
                    profile.falsoSkill = Mathf.Clamp(profile.falsoSkill + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "speed":
                if (profile.speed < 100)
                {
                    profile.speed = Mathf.Clamp(profile.speed + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "stamina":
                if (profile.stamina < 100)
                {
                    profile.stamina = Mathf.Clamp(profile.stamina + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "defending":
                if (profile.defendingSkill < 100)
                {
                    profile.defendingSkill = Mathf.Clamp(profile.defendingSkill + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "physical":
                if (profile.physicalStrength < 100)
                {
                    profile.physicalStrength = Mathf.Clamp(profile.physicalStrength + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;
        }

        // Overall'ı güncelle
        if (skillIncreased)
        {
            profile.CalculateOverall();
            Debug.Log($"[TrainingSystem] Skill trained: {skillName}. Energy: {profile.energy:F1}");
            return true;
        }

        Debug.Log($"[TrainingSystem] Skill {skillName} is already at maximum or invalid skill name.");
        return false;
    }
    
    /// <summary>
    /// Kaleci için özel antrenman (kaleci yetenekleri)
    /// </summary>
    public bool TrainGoalkeeperSkill(PlayerProfile profile, string skillName, float energyCostMultiplier = 1f)
    {
        if (profile == null || !profile.IsGoalkeeper())
        {
            Debug.LogWarning("[TrainingSystem] Player is not a goalkeeper! Cannot train goalkeeper skills.");
            return false;
        }

        float energyCost = energyCostPerTraining * energyCostMultiplier;

        if (profile.energy < energyCost)
        {
            Debug.Log($"[TrainingSystem] Not enough energy to train! Need: {energyCost}, Have: {profile.energy}");
            return false;
        }

        profile.energy -= energyCost;
        float skillIncrease = baseSkillIncrease;
        bool skillIncreased = false;

        switch (skillName.ToLower())
        {
            case "savereflex":
                if (profile.saveReflex < 100)
                {
                    profile.saveReflex = Mathf.Clamp(profile.saveReflex + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "positioning":
                if (profile.goalkeeperPositioning < 100)
                {
                    profile.goalkeeperPositioning = Mathf.Clamp(profile.goalkeeperPositioning + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "aerial":
                if (profile.aerialAbility < 100)
                {
                    profile.aerialAbility = Mathf.Clamp(profile.aerialAbility + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "oneonone":
                if (profile.oneOnOne < 100)
                {
                    profile.oneOnOne = Mathf.Clamp(profile.oneOnOne + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;

            case "handling":
                if (profile.handling < 100)
                {
                    profile.handling = Mathf.Clamp(profile.handling + (int)skillIncrease, 0, 100);
                    skillIncreased = true;
                }
                break;
        }

        if (skillIncreased)
        {
            profile.CalculateOverall();
            Debug.Log($"[TrainingSystem] Goalkeeper skill trained: {skillName}. Energy: {profile.energy:F1}");
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}







