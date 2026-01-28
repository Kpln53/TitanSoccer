using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Oyuncu İstatistikleri UI - Oyuncu istatistikleri ekranı (Tab'lı yapı)
/// </summary>
public class PlayerStatsUI : MonoBehaviour
{
    [Header("Tabs")]
    public Button overviewTabButton;
    public Button careerTabButton;
    public Button contractTabButton;
    public Image overviewTabBg;
    public Image careerTabBg;
    public Image contractTabBg;

    [Header("Panels")]
    public GameObject overviewPanel;
    public GameObject careerPanel;
    public GameObject contractPanel;

    [Header("Overview - Left Side")]
    public Image largeAvatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ageCountryText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI overallText;
    public Image countryFlagImage; // Optional

    [Header("Overview - Right Side (Attributes)")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI shootingText;
    public TextMeshProUGUI passingText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI techniqueText; // Falso/Teknik
    public TextMeshProUGUI intelligenceText; // Zeka/Mental

    [Header("Career - Table")]
    public Transform careerContentContainer;
    public GameObject careerRowPrefab; // Prefab for a row in the table

    [Header("Contract - Details")]
    public TextMeshProUGUI contractTeamText;
    public TextMeshProUGUI contractDurationText;
    public TextMeshProUGUI contractWageText;
    public TextMeshProUGUI contractBonusesText;
    public TextMeshProUGUI contractReleaseClauseText;

    [Header("Avatar Resources")]
    public Sprite defaultAvatar;
    public List<Sprite> avatarSprites;

    [Header("Colors")]
    public Color activeTabColor = new Color(0, 1, 0, 1); // Green
    public Color inactiveTabColor = new Color(0.2f, 0.2f, 0.2f, 1); // Dark Grey

    private void OnEnable()
    {
        SetupTabs();
        RefreshData();
        ShowPanel(0); // Default to Overview
    }

    private void SetupTabs()
    {
        if (overviewTabButton != null) overviewTabButton.onClick.AddListener(() => ShowPanel(0));
        if (careerTabButton != null) careerTabButton.onClick.AddListener(() => ShowPanel(1));
        if (contractTabButton != null) contractTabButton.onClick.AddListener(() => ShowPanel(2));
    }

    private void ShowPanel(int index)
    {
        // Hide all
        if (overviewPanel != null) overviewPanel.SetActive(false);
        if (careerPanel != null) careerPanel.SetActive(false);
        if (contractPanel != null) contractPanel.SetActive(false);

        // Reset Tab Colors
        if (overviewTabBg != null) overviewTabBg.color = inactiveTabColor;
        if (careerTabBg != null) careerTabBg.color = inactiveTabColor;
        if (contractTabBg != null) contractTabBg.color = inactiveTabColor;

        // Show selected
        switch (index)
        {
            case 0:
                if (overviewPanel != null) overviewPanel.SetActive(true);
                if (overviewTabBg != null) overviewTabBg.color = activeTabColor;
                break;
            case 1:
                if (careerPanel != null) careerPanel.SetActive(true);
                if (careerTabBg != null) careerTabBg.color = activeTabColor;
                break;
            case 2:
                if (contractPanel != null) contractPanel.SetActive(true);
                if (contractTabBg != null) contractTabBg.color = activeTabColor;
                break;
        }
    }

    public void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        SaveData data = GameManager.Instance.CurrentSave;
        PlayerProfile player = data.playerProfile;
        
        if (player == null) return;

        // --- Overview ---
        if (nameText != null) nameText.text = player.playerName.ToUpper();
        if (ageCountryText != null) ageCountryText.text = $"{player.age}\n{player.nationality}";
        if (positionText != null) positionText.text = $"{player.position} ({(int)player.position})"; // Enum to string might need helper
        if (overallText != null) overallText.text = $"{player.overall} (Genel)";

        // Avatar
        if (largeAvatarImage != null)
        {
            int avatarId = player.avatarId;
            if (avatarSprites != null && avatarId > 0 && avatarId <= avatarSprites.Count)
                largeAvatarImage.sprite = avatarSprites[avatarId - 1];
            else if (defaultAvatar != null)
                largeAvatarImage.sprite = defaultAvatar;
        }

        // Attributes
        if (speedText != null) speedText.text = $"Hız: {player.speed}";
        if (shootingText != null) shootingText.text = $"Şut: {player.shootingSkill}";
        if (passingText != null) passingText.text = $"Pas: {player.passingSkill}";
        if (staminaText != null) staminaText.text = $"Dayanıklılık: {player.stamina}";
        if (techniqueText != null) techniqueText.text = $"Teknik: {player.falsoSkill}"; // Mapping Falso to Technique
        if (intelligenceText != null) intelligenceText.text = $"Zeka: {player.goalkeeperPositioning}"; // Placeholder mapping

        // --- Career ---
        RefreshCareerTab(player, data);

        // --- Contract ---
        RefreshContractTab(data);
    }

    private void RefreshCareerTab(PlayerProfile player, SaveData data)
    {
        if (careerContentContainer == null || careerRowPrefab == null) return;

        // Clear existing rows
        foreach (Transform child in careerContentContainer)
        {
            if (child.gameObject != careerRowPrefab) // Don't destroy the prefab if it's in the container
                Destroy(child.gameObject);
        }

        // 1. Add Current Season (Top Row)
        if (data.seasonData != null)
        {
            CreateCareerRow("2025-26", // TODO: Get real season year
                data.clubData?.clubName ?? "-",
                data.seasonData.matchesPlayed.ToString(),
                data.seasonData.goals.ToString(),
                data.seasonData.assists.ToString(),
                player.careerAverageRating.ToString("F1"));
        }

        // 2. Add History
        if (player.careerHistory != null)
        {
            // Reverse loop to show newest first (if list is ordered chronologically)
            for (int i = player.careerHistory.Count - 1; i >= 0; i--)
            {
                var entry = player.careerHistory[i];
                CreateCareerRow(entry.season, entry.teamName, 
                    entry.matches.ToString(), 
                    entry.goals.ToString(), 
                    entry.assists.ToString(), 
                    entry.averageRating.ToString("F1"));
            }
        }
    }

    private void CreateCareerRow(string season, string team, string matches, string goals, string assists, string rating)
    {
        GameObject row = Instantiate(careerRowPrefab, careerContentContainer);
        row.SetActive(true);
        
        // Assuming the prefab structure from RebuildPlayerStatsPanel:
        // Row -> Cell -> TextMeshProUGUI
        // We need to get the text components in order
        TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (texts.Length >= 6)
        {
            texts[0].text = season;
            texts[1].text = team;
            texts[2].text = matches;
            texts[3].text = goals;
            texts[4].text = assists;
            texts[5].text = rating;
        }
    }

    private void RefreshContractTab(SaveData data)
    {
        if (data.clubData == null) return;

        if (contractTeamText != null) 
            contractTeamText.text = $"Takım: {data.clubData.clubName}";

        ContractData contract = data.clubData.contract;
        if (contract != null)
        {
            if (contractDurationText != null)
            {
                int remainingDays = contract.GetDaysRemaining();
                float remainingYears = remainingDays / 365f;
                contractDurationText.text = $"Süre: {contract.contractDuration} Yıl (Kalan: {remainingYears:F1} Yıl)";
            }

            if (contractWageText != null) 
                contractWageText.text = $"Maaş: €{contract.salary:N0} / Ay";

            if (contractBonusesText != null)
            {
                // Bonusları listele
                string bonusesStr = "Bonuslar: ";
                if (contract.bonuses != null && contract.bonuses.Count > 0)
                {
                    List<string> bonusParts = new List<string>();
                    foreach (var bonus in contract.bonuses)
                    {
                        string typeStr = bonus.type.ToString(); // Enum to string (localize later)
                        bonusParts.Add($"{typeStr}: €{bonus.amount:N0}");
                    }
                    bonusesStr += string.Join(", ", bonusParts);
                }
                else
                {
                    bonusesStr += "Yok";
                }
                contractBonusesText.text = bonusesStr;
            }

            if (contractReleaseClauseText != null) 
                contractReleaseClauseText.text = "Serbest Kalma Bedeli: -"; // Data'da yoksa tire koy
        }
    }
}
