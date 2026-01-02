using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Match Chance Gameplay UI Controller - Pozisyon oynanışı UI'ı
/// </summary>
public class MatchChanceGameplayUIController : MonoBehaviour
{
    [Header("Pozisyon Bilgisi")]
    public TextMeshProUGUI phaseTypeText;
    public TextMeshProUGUI minuteText;

    [Header("Oyuncu Bilgileri")]
    public TextMeshProUGUI playerRatingText;
    public TextMeshProUGUI playerEnergyText;

    [Header("Skor")]
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;
        PlayPhaseManager phaseManager = PlayPhaseManager.Instance;

        // Pozisyon tipi
        if (phaseTypeText != null && phaseManager != null)
        {
            phaseTypeText.text = phaseManager.CurrentPhase == PlayPhaseManager.PhaseType.AttackChance
                ? "ATTACK"
                : "DEFENSE";
        }

        // Dakika
        if (minuteText != null)
            minuteText.text = $"{context.currentMinute}'";

        // Oyuncu bilgileri
        if (playerRatingText != null)
            playerRatingText.text = $"Rating: {context.playerMatchRating:F1}";
        if (playerEnergyText != null)
            playerEnergyText.text = $"Energy: {context.playerEnergy:F0}%";

        // Skor
        if (scoreText != null)
            scoreText.text = $"{context.homeScore} - {context.awayScore}";
    }
}

