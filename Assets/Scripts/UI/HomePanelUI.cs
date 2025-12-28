using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Home Panel UI - Kariyer hub ana sayfası
/// </summary>
public class HomePanelUI : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI overallText;

    [Header("Durum")]
    public TextMeshProUGUI formText;
    public TextMeshProUGUI moralText;
    public TextMeshProUGUI energyText;

    [Header("Sezon Bilgileri")]
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI leaguePositionText;

    [Header("Para")]
    public TextMeshProUGUI moneyText;

    [Header("Hızlı Erişim Butonları")]
    public Button nextMatchButton;
    public Button trainingButton;
    public Button marketButton;

    private void OnEnable()
    {
        RefreshData();
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    public void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[HomePanelUI] No current save!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;

        // Oyuncu bilgileri
        if (playerNameText != null && saveData.playerProfile != null)
            playerNameText.text = saveData.playerProfile.playerName;

        if (clubNameText != null && saveData.clubData != null)
            clubNameText.text = saveData.clubData.clubName;

        if (positionText != null && saveData.playerProfile != null)
            positionText.text = saveData.playerProfile.position.ToString();

        if (overallText != null && saveData.playerProfile != null)
            overallText.text = $"OVR: {saveData.playerProfile.overall}";

        // Durum
        if (formText != null && saveData.playerProfile != null)
            formText.text = $"Form: {saveData.playerProfile.currentForm}";

        if (moralText != null && saveData.playerProfile != null)
            moralText.text = $"Moral: {saveData.playerProfile.currentMoral}";

        if (energyText != null && saveData.playerProfile != null)
            energyText.text = $"Enerji: {saveData.playerProfile.currentEnergy}";

        // Sezon bilgileri
        if (seasonText != null && saveData.seasonData != null)
            seasonText.text = $"Sezon {saveData.seasonData.seasonNumber}";

        if (leaguePositionText != null && saveData.seasonData != null)
            leaguePositionText.text = $"Lig Pozisyonu: {saveData.seasonData.currentLeaguePosition}";

        // Para
        if (moneyText != null && saveData.economyData != null)
            moneyText.text = $"Para: {saveData.economyData.currentMoney:N0}€";
    }
}

