using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Maç sonrası UI - Maç sonrası ekran
/// </summary>
public class PostMatchUI : MonoBehaviour
{
    [Header("Maç Sonucu")]
    public TextMeshProUGUI homeTeamNameText;
    public TextMeshProUGUI awayTeamNameText;
    public TextMeshProUGUI homeScoreText;
    public TextMeshProUGUI awayScoreText;
    public TextMeshProUGUI resultText; // "Kazandınız", "Kaybettiniz", "Berabere"
    
    [Header("Oyuncu İstatistikleri")]
    public TextMeshProUGUI playerRatingText;
    public TextMeshProUGUI playerGoalsText;
    public TextMeshProUGUI playerAssistsText;
    public TextMeshProUGUI playerMinutesText;

    [Header("Maç İstatistikleri")]
    public TextMeshProUGUI possessionText;
    public TextMeshProUGUI shotsText;

    [Header("Butonlar")]
    public Button continueButton;

    private MatchData lastMatch;

    private void Start()
    {
        SetupButtons();
        LoadMatchResult();
    }

    private void SetupButtons()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
    }

    /// <summary>
    /// Maç sonucunu yükle ve göster
    /// </summary>
    private void LoadMatchResult()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("[PostMatchUI] No save data available!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;
        string playerTeamName = saveData.clubData.clubName;

        // Son maçı al (fixture listesinden son oynanan maçı bul)
        lastMatch = GetLastPlayedMatch(saveData);
        
        if (lastMatch == null)
        {
            Debug.LogWarning("[PostMatchUI] No match result found!");
            if (resultText != null)
                resultText.text = "Maç sonucu bulunamadı";
            return;
        }

        // Takım isimlerini göster
        if (homeTeamNameText != null)
            homeTeamNameText.text = lastMatch.homeTeam;
        
        if (awayTeamNameText != null)
            awayTeamNameText.text = lastMatch.awayTeam;

        // Skorları göster
        if (homeScoreText != null)
            homeScoreText.text = lastMatch.homeScore.ToString();
        
        if (awayScoreText != null)
            awayScoreText.text = lastMatch.awayScore.ToString();

        // Sonucu belirle (oyuncunun takımına göre)
        bool isPlayerHome = lastMatch.homeTeam == playerTeamName;
        int playerScore = isPlayerHome ? lastMatch.homeScore : lastMatch.awayScore;
        int opponentScore = isPlayerHome ? lastMatch.awayScore : lastMatch.homeScore;

        string resultMessage = "";
        if (playerScore > opponentScore)
            resultMessage = "Kazandınız!";
        else if (playerScore < opponentScore)
            resultMessage = "Kaybettiniz!";
        else
            resultMessage = "Berabere";

        if (resultText != null)
            resultText.text = resultMessage;

        // Oyuncu istatistikleri
        if (playerRatingText != null)
            playerRatingText.text = $"Rating: {lastMatch.playerRating:F1}";

        if (playerGoalsText != null)
            playerGoalsText.text = $"Goller: {lastMatch.playerGoals}";

        if (playerAssistsText != null)
            playerAssistsText.text = $"Asistler: {lastMatch.playerAssists}";

        if (playerMinutesText != null)
            playerMinutesText.text = $"Dakika: {lastMatch.playerMinutesPlayed}";

        // Maç istatistikleri
        if (possessionText != null)
        {
            int playerPossession = isPlayerHome ? lastMatch.homePossession : lastMatch.awayPossession;
            possessionText.text = $"Top Kontrolü: %{playerPossession}";
        }

        Debug.Log($"[PostMatchUI] Match result loaded: {lastMatch.homeTeam} {lastMatch.homeScore}-{lastMatch.awayScore} {lastMatch.awayTeam}");
    }

    /// <summary>
    /// Son oynanan maçı bul
    /// </summary>
    private MatchData GetLastPlayedMatch(SaveData saveData)
    {
        if (saveData.seasonData == null || saveData.seasonData.fixtures == null)
            return null;

        // Oynanmış maçları bul
        List<MatchData> playedMatches = saveData.seasonData.fixtures
            .Where(m => m.isPlayed)
            .OrderByDescending(m => m.matchDate)
            .ToList();

        if (playedMatches.Count > 0)
            return playedMatches[0];

        // Eğer fixture'da oynanmış maç yoksa, şimdilik boş dön
        return null;
    }

    private void OnContinueButton()
    {
        // CareerHub'a dön
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }
}

