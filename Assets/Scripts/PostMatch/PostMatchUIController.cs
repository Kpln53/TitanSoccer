using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Post-Match UI Controller - Maç sonrası ekran
/// </summary>
public class PostMatchUIController : MonoBehaviour
{
    [Header("Final Skor")]
    public TextMeshProUGUI scoreText;

    [Header("Kadrolar ve Reytingler")]
    public TextMeshProUGUI homeSquadText;
    public TextMeshProUGUI awaySquadText;

    [Header("Bizim Kart")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerRatingText;
    public TextMeshProUGUI playerGoalsText;
    public TextMeshProUGUI playerAssistsText;
    public TextMeshProUGUI playerShotsText;

    [Header("Butonlar")]
    public Button continueButton;
    public Button interviewButton; // Disabled

    private void Start()
    {
        SetupButtons();
        RefreshUI();
    }

    private void SetupButtons()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
        
        if (interviewButton != null)
        {
            interviewButton.interactable = false; // Pasif
        }
    }

    private void RefreshUI()
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;

        // Final skor
        if (scoreText != null)
            scoreText.text = $"{context.homeTeamName} {context.homeScore} - {context.awayScore} {context.awayTeamName}";

        // Kadrolar ve reytingler
        RefreshSquads(context);

        // Bizim kart
        RefreshPlayerCard(context);
    }

    /// <summary>
    /// Kadroları ve reytingleri güncelle
    /// </summary>
    private void RefreshSquads(MatchContext context)
    {
        // Home squad (bizim takım)
        if (homeSquadText != null && context.homeSquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.homeTeamName);
            
            foreach (var player in context.homeSquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName}: {player.matchRating:F1}");
                }
            }
            
            homeSquadText.text = sb.ToString();
        }

        // Away squad (rakip)
        if (awaySquadText != null && context.awaySquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.awayTeamName);
            
            foreach (var player in context.awaySquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName}: {player.matchRating:F1}");
                }
            }
            
            awaySquadText.text = sb.ToString();
        }
    }

    /// <summary>
    /// Bizim kartı güncelle
    /// </summary>
    private void RefreshPlayerCard(MatchContext context)
    {
        if (playerNameText != null)
            playerNameText.text = context.playerName;
        
        if (playerRatingText != null)
            playerRatingText.text = $"Reyting: {context.playerMatchRating:F1}";
        
        if (playerGoalsText != null)
            playerGoalsText.text = $"Gol: {context.playerGoals}";
        
        if (playerAssistsText != null)
            playerAssistsText.text = $"Asist: {context.playerAssists}";
        
        if (playerShotsText != null)
            playerShotsText.text = $"Şut: {context.playerShots}";
    }

    private void OnContinueButton()
    {
        Debug.Log("[PostMatchUIController] Continuing...");
        
        // MatchContext'i temizle (maç bitti)
        if (MatchContext.Instance != null)
        {
            MatchContext.Instance.Clear();
        }

        SceneFlow.LoadCareerHub();
    }
}
