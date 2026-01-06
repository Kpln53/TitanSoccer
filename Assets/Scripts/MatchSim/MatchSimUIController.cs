using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

/// <summary>
/// Match Simulation UI Controller - Maç simülasyonu UI'ı
/// </summary>
public class MatchSimUIController : MonoBehaviour
{
    [Header("Scoreboard")]
    public TextMeshProUGUI scoreText; // "Biz X - Y Rakip" formatında
    public TextMeshProUGUI minuteText;
    public TextMeshProUGUI ratingText;

    [Header("Oyuncu Bilgileri")]
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI playerMoralText;

    [Header("Mini Saha (Placeholder)")]
    public UnityEngine.UI.Image miniFieldImage; // İleride top ikonu gösterilecek

    [Header("Possession")]
    public TextMeshProUGUI possessionText;

    [Header("Spiker/Commentary")]
    public TextMeshProUGUI commentaryText;
    public ScrollRect commentaryScrollRect;

    [Header("Kadrolar")]
    public TextMeshProUGUI homeSquadText;
    public TextMeshProUGUI awaySquadText;
    public GameObject squadPanel;           // Kadro paneli
    public Button squadButton;              // Kadro butonu
    public Button closeSquadButton;         // Kadro panelini kapat butonu

    [Header("Kontroller")]
    public Button speed1xButton;
    public Button speed2xButton;
    public Button speed3xButton;
    public Button simulateMatchButton;

    [Header("Confirm Dialog")]
    public ConfirmDialog confirmDialog;

    private MatchSimController matchSimController;

    private void Start()
    {
        matchSimController = FindObjectOfType<MatchSimController>();
        if (matchSimController == null)
        {
            Debug.LogError("[MatchSimUIController] MatchSimController not found!");
        }

        SetupButtons();
        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void SetupButtons()
    {
        if (speed1xButton != null)
            speed1xButton.onClick.AddListener(() => SetSpeed(1f));
        if (speed2xButton != null)
            speed2xButton.onClick.AddListener(() => SetSpeed(2f));
        if (speed3xButton != null)
            speed3xButton.onClick.AddListener(() => SetSpeed(3f));
        if (simulateMatchButton != null)
            simulateMatchButton.onClick.AddListener(OnSimulateMatchButton);
        
        // Kadro butonu
        if (squadButton != null)
            squadButton.onClick.AddListener(OpenSquadPanel);
        if (closeSquadButton != null)
            closeSquadButton.onClick.AddListener(CloseSquadPanel);
        
        // Başlangıçta kadro paneli kapalı
        if (squadPanel != null)
            squadPanel.SetActive(false);
    }

    /// <summary>
    /// Kadro panelini aç
    /// </summary>
    public void OpenSquadPanel()
    {
        if (squadPanel != null)
        {
            squadPanel.SetActive(true);
            RefreshSquads(MatchContext.Instance);
        }
    }

    /// <summary>
    /// Kadro panelini kapat
    /// </summary>
    public void CloseSquadPanel()
    {
        if (squadPanel != null)
        {
            squadPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Kadro paneli toggle
    /// </summary>
    public void ToggleSquadPanel()
    {
        if (squadPanel != null)
        {
            bool isActive = squadPanel.activeSelf;
            squadPanel.SetActive(!isActive);
            
            if (!isActive)
            {
                RefreshSquads(MatchContext.Instance);
            }
        }
    }

    private void SetSpeed(float speed)
    {
        if (matchSimController != null)
        {
            matchSimController.SetSimSpeed(speed);
        }
    }

    private void OnSimulateMatchButton()
    {
        // Confirm dialog göster
        if (confirmDialog != null)
        {
            confirmDialog.Show(
                "Maçı sona kadar simüle etmek istediğinize emin misiniz?",
                OnSimulateConfirmed,
                null
            );
        }
        else
        {
            // Dialog yoksa direkt simüle et
            OnSimulateConfirmed();
        }
    }

    private void OnSimulateConfirmed()
    {
        if (matchSimController != null)
        {
            matchSimController.SimulateToFullTime();
        }
    }

    private void RefreshUI()
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;

        // Scoreboard
        if (scoreText != null)
            scoreText.text = $"{context.homeTeamName} {context.homeScore} - {context.awayScore} {context.awayTeamName}";
        
        if (minuteText != null)
            minuteText.text = $"{context.currentMinute}'";
        
        if (ratingText != null)
            ratingText.text = $"Reyting: {context.playerMatchRating:F1}";

        // Oyuncu bilgileri
        if (playerEnergyText != null)
            playerEnergyText.text = $"Enerji: {context.playerEnergy:F0}%";
        if (playerMoralText != null)
            playerMoralText.text = $"Moral: {context.playerMoral:F0}%";

        // Possession
        if (possessionText != null)
            possessionText.text = $"Topla Oynama: {context.homePossessionPercent:F0}% - {context.awayPossessionPercent:F0}%";

        // Commentary
        RefreshCommentary(context);

        // Kadrolar
        RefreshSquads(context);
    }

    private void RefreshCommentary(MatchContext context)
    {
        if (commentaryText != null && context.commentaryLines != null && context.commentaryLines.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in context.commentaryLines)
            {
                sb.AppendLine(line);
            }
            commentaryText.text = sb.ToString();

            // Scroll'u en alta kaydır
            if (commentaryScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                commentaryScrollRect.verticalNormalizedPosition = 0f;
            }
        }
    }

    private void RefreshSquads(MatchContext context)
    {
        // Home squad (sadece ilk 11)
        if (homeSquadText != null && context.homeSquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.homeTeamName);
            foreach (var player in context.homeSquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName} ({player.matchRating:F1})");
                }
            }
            homeSquadText.text = sb.ToString();
        }

        // Away squad (sadece ilk 11)
        if (awaySquadText != null && context.awaySquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.awayTeamName);
            foreach (var player in context.awaySquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName} ({player.matchRating:F1})");
                }
            }
            awaySquadText.text = sb.ToString();
        }
    }
}
