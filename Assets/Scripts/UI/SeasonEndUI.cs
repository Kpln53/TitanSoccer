using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SeasonEndUI : MonoBehaviour
{
    [Header("Header")]
    public TextMeshProUGUI seasonTitleText; // "2025-2026 SEZONU ÖZETİ"

    [Header("Team Stats")]
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI leaguePositionText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI teamStatsText; // W-D-L

    [Header("Player Stats")]
    public TextMeshProUGUI playerStatsTitleText;
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI goalsText;
    public TextMeshProUGUI assistsText;
    public TextMeshProUGUI avgRatingText;

    [Header("Awards")]
    public Transform awardsContainer;
    public GameObject awardPrefab; // Prefab with Icon + Text

    [Header("Buttons")]
    public Button continueButton;

    private void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        SaveData data = GameManager.Instance.CurrentSave;
        SeasonData season = data.seasonData;
        PlayerProfile player = data.playerProfile;

        // Header
        if (seasonTitleText != null)
            seasonTitleText.text = $"{season.seasonName} SEZONU ÖZETİ";

        // Team Stats
        if (teamNameText != null)
            teamNameText.text = data.clubData?.clubName ?? "Free Agent";

        if (leaguePositionText != null)
            leaguePositionText.text = $"Lig Sırası: {season.leaguePosition}.";

        if (pointsText != null)
            pointsText.text = $"Puan: {season.leaguePoints}";

        if (teamStatsText != null)
            teamStatsText.text = $"{season.wins}G - {season.draws}B - {season.losses}M";

        // Player Stats
        if (playerStatsTitleText != null)
            playerStatsTitleText.text = $"{player.playerName} İstatistikleri";

        if (matchesText != null) matchesText.text = $"Maç: {season.matchesPlayed}";
        if (goalsText != null) goalsText.text = $"Gol: {season.goals}";
        if (assistsText != null) assistsText.text = $"Asist: {season.assists}";
        if (avgRatingText != null) avgRatingText.text = $"Ort. Reyting: {season.averageRating:F1}";

        // Awards (Mock logic for now)
        GenerateAwards(season, player);
    }

    private void GenerateAwards(SeasonData season, PlayerProfile player)
    {
        if (awardsContainer == null || awardPrefab == null) return;

        // Clear existing
        foreach (Transform child in awardsContainer)
            Destroy(child.gameObject);

        // Logic to determine awards
        List<string> earnedAwards = new List<string>();

        // 1. Top Scorer (Simple check: > 20 goals)
        if (season.goals > 20)
            earnedAwards.Add("Gol Kralı");

        // 2. Assist King (> 15 assists)
        if (season.assists > 15)
            earnedAwards.Add("Asist Kralı");

        // 3. Player of the Season (Rating > 8.0)
        if (season.averageRating > 8.0f)
            earnedAwards.Add("Sezonun Oyuncusu");

        // 4. Champion (League Position 1)
        if (season.leaguePosition == 1)
            earnedAwards.Add("Lig Şampiyonu");

        // Create UI elements
        foreach (string award in earnedAwards)
        {
            GameObject obj = Instantiate(awardPrefab, awardsContainer);
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = award;
            
            // Optional: Set icon based on award type
        }

        if (earnedAwards.Count == 0)
        {
            GameObject obj = Instantiate(awardPrefab, awardsContainer);
            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = "Bu sezon ödül kazanılmadı.";
        }
    }

    private void OnContinueButton()
    {
        Debug.Log("[SeasonEndUI] Starting new season...");

        // Trigger Season End System
        if (SeasonEndSystem.Instance != null)
        {
            SeasonEndSystem.Instance.EndSeason();
        }
        else
        {
            Debug.LogError("[SeasonEndUI] SeasonEndSystem not found!");
        }

        // Go to CareerHub (New Season)
        SceneFlow.LoadCareerHub();
    }
}
