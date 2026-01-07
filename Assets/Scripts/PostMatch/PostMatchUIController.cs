using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Post-Match UI Controller - Maç sonrası ekran
/// </summary>
public class PostMatchUIController : MonoBehaviour
{
    [Header("Final Skor")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI matchResultText; // Galibiyet/Beraberlik/Mağlubiyet

    [Header("Kadrolar ve Reytingler")]
    public TextMeshProUGUI homeSquadText;
    public TextMeshProUGUI awaySquadText;

    [Header("Bizim Kart")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerRatingText;
    public TextMeshProUGUI playerGoalsText;
    public TextMeshProUGUI playerAssistsText;
    public TextMeshProUGUI playerShotsText;

    [Header("Diğer Maçlar")]
    public TextMeshProUGUI otherMatchesTitle;
    public TextMeshProUGUI otherMatchesText;

    [Header("Butonlar")]
    public Button continueButton;
    public Button interviewButton; // Disabled

    private List<SimulatedMatch> otherMatches = new List<SimulatedMatch>();

    private void Start()
    {
        SetupButtons();
        
        // 1. Maç sonucunu kaydet
        SaveMatchResult();
        
        // 2. Diğer lig maçlarını simüle et
        SimulateOtherMatches();
        
        // 3. UI'ı güncelle
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

    /// <summary>
    /// Maç sonucunu SaveData'ya kaydet
    /// </summary>
    private void SaveMatchResult()
    {
        if (MatchContext.Instance == null || GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[PostMatchUI] Cannot save match result - missing data");
            return;
        }

        MatchContext ctx = MatchContext.Instance;
        SaveData save = GameManager.Instance.CurrentSave;
        string playerClub = save.clubData?.clubName ?? "";

        // Oyuncunun takımının maçı mı?
        bool isPlayerMatch = ctx.homeTeamName == playerClub || ctx.awayTeamName == playerClub;
        if (!isPlayerMatch) return;

        // Maçı fikstürde bul ve işaretle
        if (save.seasonData?.fixtures != null)
        {
            MatchData match = save.seasonData.fixtures.Find(m => 
                !m.isPlayed && 
                m.homeTeamName == ctx.homeTeamName && 
                m.awayTeamName == ctx.awayTeamName);

            if (match != null)
            {
                match.homeScore = ctx.homeScore;
                match.awayScore = ctx.awayScore;
                match.isPlayed = true;
                match.playerGoals = ctx.playerGoals;
                match.playerAssists = ctx.playerAssists;
                match.playerRating = ctx.playerMatchRating;
                
                Debug.Log($"[PostMatchUI] Match result saved: {match.homeTeamName} {match.homeScore}-{match.awayScore} {match.awayTeamName}");
            }
        }

        // Puan durumunu güncelle
        if (save.seasonData != null)
        {
            save.seasonData.RecordMatchResult(ctx.homeTeamName, ctx.awayTeamName, ctx.homeScore, ctx.awayScore);
            save.seasonData.UpdatePlayerLeaguePosition(playerClub);
        }

        // Oyuncu istatistiklerini güncelle
        save.seasonData.matchesPlayed++;
        save.seasonData.goals += ctx.playerGoals;
        save.seasonData.assists += ctx.playerAssists;
        save.seasonData.AddMatchRating(ctx.playerMatchRating);

        // Galibiyet/Beraberlik/Mağlubiyet
        bool isHome = ctx.homeTeamName == playerClub;
        int playerTeamScore = isHome ? ctx.homeScore : ctx.awayScore;
        int opponentScore = isHome ? ctx.awayScore : ctx.homeScore;

        if (playerTeamScore > opponentScore)
            save.seasonData.wins++;
        else if (playerTeamScore == opponentScore)
            save.seasonData.draws++;
        else
            save.seasonData.losses++;

        // Kaydet
        save.UpdateSaveDate();
        SaveSystem.SaveGame(save, GameManager.Instance.CurrentSaveSlotIndex);
        
        Debug.Log($"[PostMatchUI] Player stats updated - Matches: {save.seasonData.matchesPlayed}, Goals: {save.seasonData.goals}, Assists: {save.seasonData.assists}");
    }

    /// <summary>
    /// Diğer lig maçlarını simüle et
    /// </summary>
    private void SimulateOtherMatches()
    {
        otherMatches.Clear();

        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;
        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null) return;

        SaveData save = GameManager.Instance.CurrentSave;
        string playerClub = save.clubData?.clubName ?? "";
        MatchContext ctx = MatchContext.Instance;

        if (save.seasonData?.fixtures == null) return;

        // Bu hafta oynanması gereken diğer maçları bul (aynı gün/hafta)
        // Bizim maçımızla aynı tarihli maçlar
        var todaysMatches = save.seasonData.fixtures
            .Where(m => !m.isPlayed && 
                       m.homeTeamName != playerClub && 
                       m.awayTeamName != playerClub)
            .Take(8) // Max 8 maç göster
            .ToList();

        foreach (var match in todaysMatches)
        {
            // Takım güçlerini al
            TeamData homeTeam = DataPackManager.Instance.GetTeam(match.homeTeamName);
            TeamData awayTeam = DataPackManager.Instance.GetTeam(match.awayTeamName);

            int homePower = homeTeam?.GetTeamPower() ?? 70;
            int awayPower = awayTeam?.GetTeamPower() ?? 70;

            // Simüle et
            SimulatedMatch simMatch = SimulateMatch(match.homeTeamName, match.awayTeamName, homePower, awayPower);
            otherMatches.Add(simMatch);

            // Fikstürü güncelle
            match.homeScore = simMatch.homeScore;
            match.awayScore = simMatch.awayScore;
            match.isPlayed = true;

            // Puan durumunu güncelle
            save.seasonData.RecordMatchResult(match.homeTeamName, match.awayTeamName, simMatch.homeScore, simMatch.awayScore);
        }

        // Oyuncunun takımının pozisyonunu güncelle
        save.seasonData.UpdatePlayerLeaguePosition(playerClub);

        Debug.Log($"[PostMatchUI] Simulated {otherMatches.Count} other matches");
    }

    /// <summary>
    /// Tek bir maçı simüle et
    /// </summary>
    private SimulatedMatch SimulateMatch(string homeName, string awayName, int homePower, int awayPower)
    {
        SimulatedMatch match = new SimulatedMatch
        {
            homeTeamName = homeName,
            awayTeamName = awayName
        };

        // Güç farkına göre gol olasılıkları
        float homeAdvantage = 5f; // Ev sahibi avantajı
        float totalPower = homePower + awayPower + homeAdvantage;
        float homeChance = (homePower + homeAdvantage) / totalPower;

        // Toplam gol sayısı (1-5 arası)
        int totalGoals = Random.Range(1, 6);

        // Golleri dağıt
        for (int i = 0; i < totalGoals; i++)
        {
            if (Random.value < homeChance)
                match.homeScore++;
            else
                match.awayScore++;
        }

        // %15 beraberlik şansı
        if (Random.value < 0.15f && match.homeScore != match.awayScore)
        {
            int avg = (match.homeScore + match.awayScore) / 2;
            match.homeScore = avg;
            match.awayScore = avg;
        }

        return match;
    }

    private void RefreshUI()
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;

        // Final skor
        if (scoreText != null)
            scoreText.text = $"{context.homeTeamName} {context.homeScore} - {context.awayScore} {context.awayTeamName}";

        // Maç sonucu metni
        RefreshMatchResult(context);

        // Kadrolar ve reytingler
        RefreshSquads(context);

        // Bizim kart
        RefreshPlayerCard(context);

        // Diğer maçlar
        RefreshOtherMatches();
    }

    /// <summary>
    /// Maç sonucu metnini güncelle
    /// </summary>
    private void RefreshMatchResult(MatchContext context)
    {
        if (matchResultText == null) return;

        string playerClub = GameManager.Instance?.CurrentSave?.clubData?.clubName ?? "";
        bool isHome = context.homeTeamName == playerClub;
        int playerScore = isHome ? context.homeScore : context.awayScore;
        int opponentScore = isHome ? context.awayScore : context.homeScore;

        if (playerScore > opponentScore)
        {
            matchResultText.text = "GALİBİYET!";
            matchResultText.color = Color.green;
        }
        else if (playerScore == opponentScore)
        {
            matchResultText.text = "BERABERLİK";
            matchResultText.color = Color.yellow;
        }
        else
        {
            matchResultText.text = "MAĞLUBİYET";
            matchResultText.color = Color.red;
        }
    }

    /// <summary>
    /// Kadroları ve reytingleri güncelle
    /// </summary>
    private void RefreshSquads(MatchContext context)
    {
        // Home squad
        if (homeSquadText != null && context.homeSquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<b>{context.homeTeamName}</b>");
            
            foreach (var player in context.homeSquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName}: {player.matchRating:F1}");
                }
            }
            
            homeSquadText.text = sb.ToString();
        }

        // Away squad
        if (awaySquadText != null && context.awaySquad != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<b>{context.awayTeamName}</b>");
            
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
        {
            playerRatingText.text = $"Reyting: {context.playerMatchRating:F1}";
            // Reyting rengini ayarla
            if (context.playerMatchRating >= 8f)
                playerRatingText.color = Color.green;
            else if (context.playerMatchRating >= 6f)
                playerRatingText.color = Color.yellow;
            else
                playerRatingText.color = Color.red;
        }
        
        if (playerGoalsText != null)
            playerGoalsText.text = $"Gol: {context.playerGoals}";
        
        if (playerAssistsText != null)
            playerAssistsText.text = $"Asist: {context.playerAssists}";
        
        if (playerShotsText != null)
            playerShotsText.text = $"Şut: {context.playerShots}";
    }

    /// <summary>
    /// Diğer maçları göster
    /// </summary>
    private void RefreshOtherMatches()
    {
        if (otherMatchesTitle != null)
        {
            otherMatchesTitle.text = "DİĞER SONUÇLAR";
        }

        if (otherMatchesText != null)
        {
            if (otherMatches.Count == 0)
            {
                otherMatchesText.text = "Bu hafta başka maç yok.";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var match in otherMatches)
                {
                    sb.AppendLine($"{match.homeTeamName} {match.homeScore} - {match.awayScore} {match.awayTeamName}");
                }
                otherMatchesText.text = sb.ToString();
            }
        }
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

    /// <summary>
    /// Simüle edilmiş maç verisi
    /// </summary>
    [System.Serializable]
    public class SimulatedMatch
    {
        public string homeTeamName;
        public string awayTeamName;
        public int homeScore;
        public int awayScore;
    }
}
