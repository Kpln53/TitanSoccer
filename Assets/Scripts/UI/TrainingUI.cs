using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Antrenman UI - Yetenek antrenmanı paneli
/// </summary>
public class TrainingUI : MonoBehaviour
{
    [Header("Enerji Göstergesi")]
    public TextMeshProUGUI energyText;

    [Header("Yetenek Butonları")]
    public Button trainPassingButton;
    public Button trainShootingButton;
    public Button trainDribblingButton;
    public Button trainFalsoButton;
    public Button trainSpeedButton;
    public Button trainStaminaButton;
    public Button trainDefendingButton;
    public Button trainPhysicalButton;

    [Header("Yetenek Değerleri")]
    public TextMeshProUGUI passingText;
    public TextMeshProUGUI shootingText;
    public TextMeshProUGUI dribblingText;
    public TextMeshProUGUI falsoText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI defendingText;
    public TextMeshProUGUI physicalText;

    [Header("Kaleci Yetenekleri (Opsiyonel)")]
    public Button trainSaveReflexButton;
    public Button trainGoalkeeperPositioningButton;
    public Button trainAerialAbilityButton;
    public Button trainOneOnOneButton;
    public Button trainHandlingButton;

    private void OnEnable()
    {
        RefreshData();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (trainPassingButton != null)
            trainPassingButton.onClick.AddListener(() => TrainSkill("passing"));

        if (trainShootingButton != null)
            trainShootingButton.onClick.AddListener(() => TrainSkill("shooting"));

        if (trainDribblingButton != null)
            trainDribblingButton.onClick.AddListener(() => TrainSkill("dribbling"));

        if (trainFalsoButton != null)
            trainFalsoButton.onClick.AddListener(() => TrainSkill("falso"));

        if (trainSpeedButton != null)
            trainSpeedButton.onClick.AddListener(() => TrainSkill("speed"));

        if (trainStaminaButton != null)
            trainStaminaButton.onClick.AddListener(() => TrainSkill("stamina"));

        if (trainDefendingButton != null)
            trainDefendingButton.onClick.AddListener(() => TrainSkill("defending"));

        if (trainPhysicalButton != null)
            trainPhysicalButton.onClick.AddListener(() => TrainSkill("physicalstrength"));

        // Kaleci yetenekleri
        if (trainSaveReflexButton != null)
            trainSaveReflexButton.onClick.AddListener(() => TrainSkill("saverelex"));

        if (trainGoalkeeperPositioningButton != null)
            trainGoalkeeperPositioningButton.onClick.AddListener(() => TrainSkill("goalkeeperpositioning"));

        if (trainAerialAbilityButton != null)
            trainAerialAbilityButton.onClick.AddListener(() => TrainSkill("aerialability"));

        if (trainOneOnOneButton != null)
            trainOneOnOneButton.onClick.AddListener(() => TrainSkill("oneonone"));

        if (trainHandlingButton != null)
            trainHandlingButton.onClick.AddListener(() => TrainSkill("handling"));
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    private void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[TrainingUI] No current save!");
            return;
        }

        PlayerProfile player = GameManager.Instance.CurrentSave.playerProfile;
        if (player == null) return;

        // Enerji göster
        if (energyText != null)
            energyText.text = $"Enerji: {(int)player.energy}/100";

        // Yetenekleri göster
        if (passingText != null)
            passingText.text = $"Pas: {player.passingSkill}";

        if (shootingText != null)
            shootingText.text = $"Şut: {player.shootingSkill}";

        if (dribblingText != null)
            dribblingText.text = $"Dribling: {player.dribblingSkill}";

        if (falsoText != null)
            falsoText.text = $"Falso: {player.falsoSkill}";

        if (speedText != null)
            speedText.text = $"Hız: {player.speed}";

        if (staminaText != null)
            staminaText.text = $"Dayanıklılık: {player.stamina}";

        if (defendingText != null)
            defendingText.text = $"Savunma: {player.defendingSkill}";

        if (physicalText != null)
            physicalText.text = $"Fizik: {player.physicalStrength}";
    }

    /// <summary>
    /// Yetenek antrenmanı yap
    /// </summary>
    private void TrainSkill(string skillName)
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        PlayerProfile player = GameManager.Instance.CurrentSave.playerProfile;
        EconomyData economy = GameManager.Instance.CurrentSave.economyData;

        if (player == null || economy == null) return;

        if (TrainingSystem.Instance != null)
        {
            TrainingSystem.Instance.TrainSkill(player, skillName);
        }
        else
        {
            Debug.LogWarning("[TrainingUI] TrainingSystem not found!");
        }

        RefreshData();
    }
}

