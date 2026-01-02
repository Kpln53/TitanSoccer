using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Pre-Match UI Controller - Maç öncesi ekran
/// </summary>
public class PreMatchUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI fixtureText;
    
    [Header("Kadrolar")]
    public TextMeshProUGUI homeSquadText;
    public TextMeshProUGUI awaySquadText;
    
    [Header("Oyuncu Durumu")]
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI playerMoralText;
    
    [Header("Lineup Status")]
    public TextMeshProUGUI lineupStatusText;
    
    [Header("Butonlar")]
    public Button backButton;
    public Button startMatchButton;

    private void Start()
    {
        SetupButtons();
        PopulateMatchContext();
        RefreshUI();
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        if (startMatchButton != null)
            startMatchButton.onClick.AddListener(OnStartMatchButton);
    }

    /// <summary>
    /// MatchContext'i doldur (dummy data - gerçekte GameManager'dan gelecek)
    /// </summary>
    private void PopulateMatchContext()
    {
        // MatchContext yoksa oluştur
        if (MatchContext.Instance == null)
        {
            GameObject contextObj = new GameObject("MatchContext");
            contextObj.AddComponent<MatchContext>();
        }

        MatchContext context = MatchContext.Instance;

        // Takım isimleri
        context.homeTeamName = "Bizim Takım";
        context.awayTeamName = "Rakip Takım";

        // Kadrolar (dummy data - 11 ilk 11, 7 yedek)
        context.homeSquad.Clear();
        context.awaySquad.Clear();

        // Home squad (11 ilk 11, 7 yedek)
        for (int i = 0; i < 11; i++)
        {
            context.homeSquad.Add(new MatchContext.SquadPlayer(
                $"Home Player {i + 1}", 
                Random.Range(60, 85), 
                (PlayerPosition)(i % 11), 
                true // İlk 11
            ));
        }
        for (int i = 0; i < 7; i++)
        {
            context.homeSquad.Add(new MatchContext.SquadPlayer(
                $"Home Sub {i + 1}", 
                Random.Range(55, 75), 
                (PlayerPosition)(i % 11), 
                false // Yedek
            ));
        }

        // Away squad
        for (int i = 0; i < 11; i++)
        {
            context.awaySquad.Add(new MatchContext.SquadPlayer(
                $"Away Player {i + 1}", 
                Random.Range(60, 85), 
                (PlayerPosition)(i % 11), 
                true
            ));
        }
        for (int i = 0; i < 7; i++)
        {
            context.awaySquad.Add(new MatchContext.SquadPlayer(
                $"Away Sub {i + 1}", 
                Random.Range(55, 75), 
                (PlayerPosition)(i % 11), 
                false
            ));
        }

        // Oyuncu bilgileri
        context.playerName = "Oyuncu Adı";
        context.playerPosition = PlayerPosition.SF;
        context.playerOverall = 75;
        context.playerEnergy = Random.Range(70f, 100f);
        context.playerMoral = Random.Range(40f, 80f);
        context.playerMatchRating = 5.0f;

        // Takım güçleri
        context.homeTeamPower = 70;
        context.awayTeamPower = 65;
    }

    private void RefreshUI()
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;

        // Title ve fixture
        if (titleText != null)
            titleText.text = "MAÇ ÖNCESİ";
        if (fixtureText != null)
            fixtureText.text = $"{context.homeTeamName} vs {context.awayTeamName}";

        // Kadrolar
        RefreshSquadText(context);

        // Oyuncu durumu
        if (playerEnergyText != null)
            playerEnergyText.text = $"Enerji: {context.playerEnergy:F0}%";
        if (playerMoralText != null)
            playerMoralText.text = $"Moral: {context.playerMoral:F0}%";

        // Lineup status
        RefreshLineupStatus(context);
    }

    /// <summary>
    /// Kadro metinlerini güncelle
    /// </summary>
    private void RefreshSquadText(MatchContext context)
    {
        // Home squad
        if (homeSquadText != null && context.homeSquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{context.homeTeamName}");
            sb.AppendLine("İlk 11:");
            
            foreach (var player in context.homeSquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"  {player.playerName} ({player.position})");
                }
            }
            
            sb.AppendLine("\nYedek:");
            foreach (var player in context.homeSquad)
            {
                if (!player.isStartingXI)
                {
                    sb.AppendLine($"  {player.playerName} ({player.position})");
                }
            }
            
            homeSquadText.text = sb.ToString();
        }

        // Away squad
        if (awaySquadText != null && context.awaySquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{context.awayTeamName}");
            sb.AppendLine("İlk 11:");
            
            foreach (var player in context.awaySquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"  {player.playerName} ({player.position})");
                }
            }
            
            sb.AppendLine("\nYedek:");
            foreach (var player in context.awaySquad)
            {
                if (!player.isStartingXI)
                {
                    sb.AppendLine($"  {player.playerName} ({player.position})");
                }
            }
            
            awaySquadText.text = sb.ToString();
        }
    }

    /// <summary>
    /// Lineup status ve gerekçeleri güncelle
    /// </summary>
    private void RefreshLineupStatus(MatchContext context)
    {
        if (lineupStatusText == null) return;

        StringBuilder sb = new StringBuilder();
        string status = "";
        List<string> reasons = new List<string>();

        // Lineup status belirleme (basit mantık)
        if (context.playerEnergy < 50f)
        {
            status = "Kadro Dışı";
            reasons.Add("Enerjin düşük");
        }
        else if (context.playerMoral < 30f)
        {
            status = "Yedek";
            reasons.Add("Moral düşük");
        }
        else if (context.playerEnergy > 90f && context.playerMoral > 70f)
        {
            status = "İlk 11";
            reasons.Add("Formun çok iyi");
        }
        else if (context.playerMoral > 60f)
        {
            status = "İlk 11";
            reasons.Add("Moral yüksek");
        }
        else
        {
            status = "Yedek";
            reasons.Add("Rotasyon");
        }

        sb.AppendLine($"Durum: {status}");
        foreach (var reason in reasons)
        {
            sb.AppendLine($"  • {reason}");
        }

        lineupStatusText.text = sb.ToString();
    }

    private void OnStartMatchButton()
    {
        Debug.Log("[PreMatchUIController] Starting match...");
        SceneFlow.LoadMatchSim();
    }

    private void OnBackButton()
    {
        Debug.Log("[PreMatchUIController] Going back...");
        SceneFlow.LoadCareerHub();
    }
}
