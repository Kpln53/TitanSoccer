using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Top Panel UI - Her sayfada görünen üst panel (oyuncu bilgileri, durum, sezon)
/// </summary>
public class TopPanelUI : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI ageText; // Yaş bilgisi

    [Header("Durum")]
    public TextMeshProUGUI moralText;
    public TextMeshProUGUI energyText;

    [Header("Sezon Bilgileri")]
    public TextMeshProUGUI seasonText; // "Sezon 1 - Hafta 1" formatında

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
            Debug.LogWarning("[TopPanelUI] No current save!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;

        // Oyuncu bilgileri
        if (playerNameText != null && saveData.playerProfile != null)
            playerNameText.text = saveData.playerProfile.playerName;

        if (clubNameText != null && saveData.clubData != null)
            clubNameText.text = saveData.clubData.clubName;

        // Yaş bilgisi (görselde "Yaş: 18" formatında)
        if (ageText != null && saveData.playerProfile != null)
            ageText.text = $"Yaş: {saveData.playerProfile.age}";

        // Durum (görselde yüzde olarak gösteriliyor)
        if (moralText != null && saveData.playerProfile != null)
            moralText.text = $"Moral: {(int)saveData.playerProfile.moral}%";

        if (energyText != null && saveData.playerProfile != null)
            energyText.text = $"Enerji: {(int)saveData.playerProfile.energy}%";

        // Sezon bilgileri (görselde "Sezon 1 - Hafta 1" formatında)
        if (seasonText != null && saveData.seasonData != null)
        {
            // Hafta bilgisi yoksa matchesPlayed + 1 kullan, minimum 1
            int weekNumber = Mathf.Max(1, saveData.seasonData.matchesPlayed + 1);
            seasonText.text = $"Sezon {saveData.seasonData.seasonNumber} - Hafta {weekNumber}";
        }
    }
}






