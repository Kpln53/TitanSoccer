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
    /// MatchContext'i GameManager ve DataPack'ten doldur
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

        // SaveData'dan bilgileri al
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[PreMatchUI] No save data, using fallback");
            PopulateFallbackData(context);
            return;
        }

        SaveData save = GameManager.Instance.CurrentSave;
        string playerClubName = save.clubData?.clubName ?? "Takım";
        string leagueName = save.clubData?.leagueName ?? "";

        // Eğer MatchContext zaten HomePanelUI'dan doldurulduysa, takım isimlerini koru
        if (string.IsNullOrEmpty(context.homeTeamName))
        {
            // Sonraki maçı bul
            MatchData nextMatch = null;
            if (save.seasonData?.fixtures != null)
            {
                nextMatch = save.seasonData.fixtures.Find(m => !m.isPlayed && 
                    (m.homeTeamName == playerClubName || m.awayTeamName == playerClubName));
            }

            if (nextMatch != null)
            {
                context.homeTeamName = nextMatch.homeTeamName;
                context.awayTeamName = nextMatch.awayTeamName;
                context.isPlayerHome = nextMatch.homeTeamName == playerClubName;
                context.matchType = nextMatch.matchType;
            }
            else
            {
                context.homeTeamName = playerClubName;
                context.awayTeamName = "Rakip";
                context.isPlayerHome = true;
            }
        }

        // DataPack'ten takım kadrolarını al
        PopulateSquadsFromDataPack(context, leagueName);

        // Oyuncu bilgileri
        if (save.playerProfile != null)
        {
            context.playerName = save.playerProfile.playerName;
            context.playerPosition = save.playerProfile.position;
            context.playerOverall = save.playerProfile.overall;
            context.playerEnergy = save.playerProfile.stamina; // Gerçek enerji sistemi eklenebilir
            context.playerMoral = save.relationsData?.managerRelation ?? 70f;
        }
        context.playerMatchRating = 6.0f; // Başlangıç rating

        Debug.Log($"[PreMatchUI] Match: {context.homeTeamName} vs {context.awayTeamName}");
    }

    /// <summary>
    /// DataPack'ten kadroları doldur
    /// </summary>
    private void PopulateSquadsFromDataPack(MatchContext context, string leagueName)
    {
        context.homeSquad.Clear();
        context.awaySquad.Clear();

        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
        {
            Debug.LogWarning("[PreMatchUI] No DataPack, using fallback squads");
            PopulateFallbackSquads(context);
            return;
        }

        // Home takımı bul
        TeamData homeTeam = DataPackManager.Instance.GetTeam(context.homeTeamName);
        TeamData awayTeam = DataPackManager.Instance.GetTeam(context.awayTeamName);

        // Home kadro
        if (homeTeam != null && homeTeam.players != null)
        {
            int count = 0;
            foreach (var player in homeTeam.players)
            {
                bool isStarter = count < 11;
                context.homeSquad.Add(new MatchContext.SquadPlayer(
                    player.playerName,
                    player.overall,
                    player.position,
                    isStarter
                ));
                count++;
                if (count >= 18) break; // Max 18 oyuncu
            }
            context.homeTeamPower = homeTeam.GetTeamPower();
            Debug.Log($"[PreMatchUI] Home team {homeTeam.teamName}: {count} players, power: {context.homeTeamPower}");
        }
        else
        {
            PopulateFallbackSquad(context.homeSquad, context.homeTeamName, true);
            context.homeTeamPower = 70;
        }

        // Away kadro
        if (awayTeam != null && awayTeam.players != null)
        {
            int count = 0;
            foreach (var player in awayTeam.players)
            {
                bool isStarter = count < 11;
                context.awaySquad.Add(new MatchContext.SquadPlayer(
                    player.playerName,
                    player.overall,
                    player.position,
                    isStarter
                ));
                count++;
                if (count >= 18) break;
            }
            context.awayTeamPower = awayTeam.GetTeamPower();
            Debug.Log($"[PreMatchUI] Away team {awayTeam.teamName}: {count} players, power: {context.awayTeamPower}");
        }
        else
        {
            PopulateFallbackSquad(context.awaySquad, context.awayTeamName, false);
            context.awayTeamPower = 65;
        }

        // Oyuncuyu home kadrosuna ekle (eğer yoksa)
        if (GameManager.Instance?.CurrentSave?.playerProfile != null && context.isPlayerHome)
        {
            var profile = GameManager.Instance.CurrentSave.playerProfile;
            // Oyuncunun kadroda olup olmadığını kontrol et
            bool playerInSquad = context.homeSquad.Exists(p => p.playerName == profile.playerName);
            if (!playerInSquad && context.homeSquad.Count > 0)
            {
                // İlk 11'e ekle veya değiştir
                context.homeSquad[0] = new MatchContext.SquadPlayer(
                    profile.playerName,
                    profile.overall,
                    profile.position,
                    true
                );
            }
        }
    }

    /// <summary>
    /// Fallback kadro doldur
    /// </summary>
    private void PopulateFallbackSquad(List<MatchContext.SquadPlayer> squad, string teamName, bool isHome)
    {
        string prefix = isHome ? "Ev" : "Dep";
        for (int i = 0; i < 11; i++)
        {
            squad.Add(new MatchContext.SquadPlayer(
                $"{prefix} Oyuncu {i + 1}",
                Random.Range(60, 80),
                (PlayerPosition)(i % 11),
                true
            ));
        }
        for (int i = 0; i < 7; i++)
        {
            squad.Add(new MatchContext.SquadPlayer(
                $"{prefix} Yedek {i + 1}",
                Random.Range(55, 75),
                (PlayerPosition)(i % 11),
                false
            ));
        }
    }

    /// <summary>
    /// Fallback kadrolar
    /// </summary>
    private void PopulateFallbackSquads(MatchContext context)
    {
        PopulateFallbackSquad(context.homeSquad, context.homeTeamName, true);
        PopulateFallbackSquad(context.awaySquad, context.awayTeamName, false);
        context.homeTeamPower = 70;
        context.awayTeamPower = 65;
    }

    /// <summary>
    /// Tamamen fallback data
    /// </summary>
    private void PopulateFallbackData(MatchContext context)
    {
        context.homeTeamName = "Ev Sahibi";
        context.awayTeamName = "Deplasman";
        PopulateFallbackSquads(context);
        context.playerName = "Oyuncu";
        context.playerPosition = PlayerPosition.SF;
        context.playerOverall = 70;
        context.playerEnergy = 80f;
        context.playerMoral = 70f;
        context.playerMatchRating = 6.0f;
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
