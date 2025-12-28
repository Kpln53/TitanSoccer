using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Oyuncu İstatistikleri UI - Oyuncu istatistikleri ekranı
/// </summary>
public class PlayerStatsUI : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI ageText;
    public TextMeshProUGUI nationalityText;
    public TextMeshProUGUI overallText;

    [Header("Yetenekler")]
    public TextMeshProUGUI passingText;
    public TextMeshProUGUI shootingText;
    public TextMeshProUGUI dribblingText;
    public TextMeshProUGUI falsoText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI defendingText;
    public TextMeshProUGUI physicalText;

    [Header("Kariyer İstatistikleri")]
    public TextMeshProUGUI totalMatchesText;
    public TextMeshProUGUI totalGoalsText;
    public TextMeshProUGUI totalAssistsText;
    public TextMeshProUGUI averageRatingText;

    [Header("Sezon İstatistikleri")]
    public TextMeshProUGUI seasonMatchesText;
    public TextMeshProUGUI seasonGoalsText;
    public TextMeshProUGUI seasonAssistsText;

    [Header("Durum")]
    public TextMeshProUGUI formText;
    public TextMeshProUGUI moralText;
    public TextMeshProUGUI energyText;

    [Header("Geri Butonu")]
    public Button backButton;

    private void OnEnable()
    {
        RefreshData();
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    private void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[PlayerStatsUI] No current save!");
            return;
        }

        PlayerProfile player = GameManager.Instance.CurrentSave.playerProfile;
        SeasonData season = GameManager.Instance.CurrentSave.seasonData;

        if (player == null) return;

        // Oyuncu bilgileri
        if (playerNameText != null)
            playerNameText.text = player.playerName;

        if (positionText != null)
            positionText.text = $"Pozisyon: {player.position}";

        if (ageText != null)
            ageText.text = $"Yaş: {player.age}";

        if (nationalityText != null)
            nationalityText.text = $"Uyruk: {player.nationality}";

        if (overallText != null)
            overallText.text = $"Overall: {player.overall}";

        // Yetenekler
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

        // Kariyer istatistikleri
        if (totalMatchesText != null)
            totalMatchesText.text = $"Toplam Maç: {player.totalMatches}";

        if (totalGoalsText != null)
            totalGoalsText.text = $"Toplam Gol: {player.totalGoals}";

        if (totalAssistsText != null)
            totalAssistsText.text = $"Toplam Asist: {player.totalAssists}";

        if (averageRatingText != null)
            averageRatingText.text = $"Ortalama Rating: {player.averageRating:F2}";

        // Sezon istatistikleri
        if (season != null)
        {
            if (seasonMatchesText != null)
                seasonMatchesText.text = $"Sezon Maç: {season.matchesPlayed}";

            if (seasonGoalsText != null)
                seasonGoalsText.text = $"Sezon Gol: {season.goals}";

            if (seasonAssistsText != null)
                seasonAssistsText.text = $"Sezon Asist: {season.assists}";
        }

        // Durum
        if (formText != null)
            formText.text = $"Form: {player.currentForm}";

        if (moralText != null)
            moralText.text = $"Moral: {player.currentMoral}";

        if (energyText != null)
            energyText.text = $"Enerji: {player.currentEnergy}";
    }

    private void OnBackButton()
    {
        gameObject.SetActive(false);
    }
}

