using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using TitanSoccer.Social;

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

    private List<SimulatedMatch> otherMatches = new List<SimulatedMatch>();

    private void OnDestroy()
    {
        // Event listener'ları temizle
        if (continueButton != null)
            continueButton.onClick.RemoveAllListeners();
            
        // Listleri temizle
        otherMatches?.Clear();
    }

    private void Start()
    {
        SetupButtons();
        
        // 1. Maç sonucunu kaydet
        SaveMatchResult();
        
        // 2. Diğer lig maçlarını simüle et
        SimulateOtherMatches();
        
        // 3. UI'ı güncelle
        RefreshUI();
        
        // 4. Agresif memory cleanup (UI yüklendikten sonra)
        StartCoroutine(DelayedCleanup());
    }
    
    private System.Collections.IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(0.5f); // UI render edilsin
        System.GC.Collect(); // Basit memory cleanup
    }

    private void SetupButtons()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
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

                // SOSYAL MEDYA ENTEGRASYONU
                if (TitanSoccer.Social.SocialMediaSystem.Instance != null)
                {
                    TitanSoccer.Social.SocialMediaSystem.Instance.SetLastMatch(match);
                }
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
        
        // 🔥 HABER ÜRETİMİ - Maç sonrası otomatik haber oluştur
        GeneratePostMatchNews();
    }

    /// <summary>
    /// Diğer lig maçlarını simüle et
    /// </summary>
    private void SimulateOtherMatches()
    {
        otherMatches.Clear();

        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;
        
        SaveData save = GameManager.Instance.CurrentSave;
        string playerClub = save.clubData?.clubName ?? "";
        int currentWeek = save.seasonData.currentWeek;

        Debug.Log($"[PostMatchUI] Simulating other matches for Week {currentWeek}...");

        // LeagueSimulationSystem kullanarak haftanın diğer maçlarını simüle et
        if (LeagueSimulationSystem.Instance != null)
        {
            LeagueSimulationSystem.Instance.SimulateWeek(currentWeek, playerClub);
            
            // ⚠️ ÖNEMLİ: Simülasyon sonrası kaydet!
            save.UpdateSaveDate();
            SaveSystem.SaveGame(save, GameManager.Instance.CurrentSaveSlotIndex);
            Debug.Log($"[PostMatchUI] Week {currentWeek} simulation saved to disk.");
        }
        else
        {
            // Fallback: Eğer sistem yoksa oluştur
            Debug.LogWarning("[PostMatchUI] LeagueSimulationSystem not found, creating it...");
            GameObject simSystemObj = new GameObject("LeagueSimulationSystem");
            simSystemObj.AddComponent<LeagueSimulationSystem>();
            
            // Tekrar dene
            if (LeagueSimulationSystem.Instance != null)
            {
                LeagueSimulationSystem.Instance.SimulateWeek(currentWeek, playerClub);
                save.UpdateSaveDate();
                SaveSystem.SaveGame(save, GameManager.Instance.CurrentSaveSlotIndex);
                Debug.Log($"[PostMatchUI] Week {currentWeek} simulation saved (after creating system).");
            }
        }

        // Simüle edilen maçları UI için listeye ekle
        var weekMatches = save.seasonData.fixtures
            .Where(m => m.weekNumber == currentWeek && 
                       m.isPlayed && 
                       m.homeTeamName != playerClub && 
                       m.awayTeamName != playerClub)
            .ToList();
        
        foreach (var match in weekMatches)
        {
            otherMatches.Add(new SimulatedMatch 
            { 
                homeTeamName = match.homeTeamName, 
                awayTeamName = match.awayTeamName, 
                homeScore = match.homeScore, 
                awayScore = match.awayScore 
            });
        }

        Debug.Log($"[PostMatchUI] Displaying {otherMatches.Count} other matches from Week {currentWeek}");
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
            var sb = new StringBuilder(256); // Capacity belirle
            sb.AppendLine($"<b>{context.homeTeamName}</b>");
            
            foreach (var player in context.homeSquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName}: {player.matchRating:F1}");
                }
            }
            
            homeSquadText.text = sb.ToString();
            sb.Clear(); // StringBuilder'ı temizle
        }

        // Away squad
        if (awaySquadText != null && context.awaySquad != null)
        {
            var sb = new StringBuilder(256); // Capacity belirle
            sb.AppendLine($"<b>{context.awayTeamName}</b>");
            
            foreach (var player in context.awaySquad)
            {
                if (player.isStartingXI)
                {
                    sb.AppendLine($"{player.playerName}: {player.matchRating:F1}");
                }
            }
            
            awaySquadText.text = sb.ToString();
            sb.Clear(); // StringBuilder'ı temizle
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
                var sb = new StringBuilder(otherMatches.Count * 50); // Capacity hesapla
                foreach (var match in otherMatches)
                {
                    sb.AppendLine($"{match.homeTeamName} {match.homeScore} - {match.awayScore} {match.awayTeamName}");
                }
                otherMatchesText.text = sb.ToString();
                sb.Clear(); // StringBuilder'ı temizle
            }
        }
    }

    private void OnContinueButton()
    {
        Debug.Log("[PostMatchUIController] Continuing...");
        
        // Listleri temizle
        otherMatches?.Clear();
        
        // Haftayı ilerlet
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            SaveData save = GameManager.Instance.CurrentSave;
            save.seasonData.currentWeek++;
            
            // Kaydet
            save.UpdateSaveDate();
            SaveSystem.SaveGame(save, GameManager.Instance.CurrentSaveSlotIndex);
            
            Debug.Log($"[PostMatchUI] Week advanced to: {save.seasonData.currentWeek}");
        }
        
        // MatchContext'i temizle (maç bitti)
        if (MatchContext.Instance != null)
        {
            MatchContext.Instance.Clear();
        }

        // Sezon sonu kontrolü
        if (CheckSeasonEnd())
        {
            Debug.Log("[PostMatchUI] Season ended! Loading SeasonEnd scene...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("SeasonEnd");
        }
        else
        {
            SceneFlow.LoadCareerHub();
        }
    }

    /// <summary>
    /// Sezon bitti mi kontrol et
    /// </summary>
    private bool CheckSeasonEnd()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return false;
        
        SaveData data = GameManager.Instance.CurrentSave;
        if (data.seasonData == null || data.seasonData.fixtures == null) return false;

        // Tüm maçlar oynandı mı?
        bool anyUnplayed = data.seasonData.fixtures.Any(m => !m.isPlayed);
        
        if (!anyUnplayed)
        {
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Maç sonrası haber üretimi
    /// </summary>
    private void GeneratePostMatchNews()
    {
        // NewsGenerator varsa otomatik haber üret
        if (NewsGenerator.Instance != null)
        {
            Debug.Log("📰 [PostMatchUI] Generating post-match news...");
            NewsGenerator.Instance.GeneratePostMatchNews();
        }
        else
        {
            Debug.LogWarning("📰 [PostMatchUI] NewsGenerator instance not found!");
            
            // NewsGenerator yoksa sahneye ekle
            GameObject newsGeneratorGO = new GameObject("NewsGenerator");
            newsGeneratorGO.AddComponent<NewsGenerator>();
            
            // Tekrar dene
            if (NewsGenerator.Instance != null)
            {
                Debug.Log("📰 [PostMatchUI] NewsGenerator created, generating news...");
                NewsGenerator.Instance.GeneratePostMatchNews();
            }
        }
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