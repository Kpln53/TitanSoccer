using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Otomatik haber üreticisi - Oyun olaylarına göre haber oluşturur
/// </summary>
public class NewsGenerator : MonoBehaviour
{
    public static NewsGenerator Instance { get; private set; }
    
    [Header("Ayarlar")]
    [SerializeField] private bool autoGenerateNews = true;
    [SerializeField] private float newsDelayAfterMatch = 1f; // Maç sonrası kaç saat sonra haber
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Maç sonrası haber üret
    /// </summary>
    public void GeneratePostMatchNews()
    {
        if (!autoGenerateNews) return;
        
        var matchContext = MatchContext.Instance;
        if (matchContext == null) return;
        
        var save = GameManager.Instance?.CurrentSave;
        if (save == null) return;
        
        string playerName = save.playerProfile?.playerName ?? "Oyuncu";
        string playerTeam = save.playerProfile?.currentClubName ?? "Takım";
        
        // Maç sonucu analizi
        bool isHome = matchContext.isPlayerHome;
        int playerTeamScore = isHome ? matchContext.homeScore : matchContext.awayScore;
        int opponentScore = isHome ? matchContext.awayScore : matchContext.homeScore;
        string opponentTeam = isHome ? matchContext.awayTeamName : matchContext.homeTeamName;
        
        // Oyuncu performansı
        int playerGoals = matchContext.playerGoals;
        int playerAssists = matchContext.playerAssists;
        float playerRating = matchContext.playerMatchRating;
        
        // Haber üret
        if (playerGoals > 0)
        {
            GenerateGoalNews(playerName, playerTeam, opponentTeam, playerGoals, playerTeamScore, opponentScore);
        }
        
        if (playerAssists > 0)
        {
            GenerateAssistNews(playerName, playerTeam, opponentTeam, playerAssists);
        }
        
        // Maç sonucu haberi
        GenerateMatchResultNews(playerName, playerTeam, opponentTeam, playerTeamScore, opponentScore, playerRating);
        
        // Performans haberi (özel durumlar)
        if (playerRating >= 9.0f)
        {
            GenerateExceptionalPerformanceNews(playerName, playerTeam, playerRating, playerGoals, playerAssists);
        }
        else if (playerRating <= 5.0f)
        {
            GeneratePoorPerformanceNews(playerName, playerTeam, playerRating);
        }
    }
    
    /// <summary>
    /// Gol haberi üret
    /// </summary>
    private void GenerateGoalNews(string playerName, string teamName, string opponentTeam, int goals, int teamScore, int opponentScore)
    {
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.Match);
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"teamName", teamName},
            {"opponentTeam", opponentTeam},
            {"goals", goals.ToString()},
            {"score", $"{teamScore}-{opponentScore}"}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, NewsType.Match, source);
    }
    
    /// <summary>
    /// Asist haberi üret
    /// </summary>
    private void GenerateAssistNews(string playerName, string teamName, string opponentTeam, int assists)
    {
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.Performance);
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"teamName", teamName},
            {"opponentTeam", opponentTeam},
            {"assists", assists.ToString()}
        };
        
        string title = $"🎯 {playerName} {assists} Asist Yaptı!";
        string content = $"{playerName}, {opponentTeam} karşısında {assists} asist yaparak takım arkadaşlarını gole taşıdı. Bu performans oyuncunun sezonun en iyi maçlarından biri olarak kayıtlara geçti.";
        
        CreateNews(title, content, NewsType.Performance, "Performans Analizi");
    }
    
    /// <summary>
    /// Maç sonucu haberi üret
    /// </summary>
    private void GenerateMatchResultNews(string playerName, string teamName, string opponentTeam, int teamScore, int opponentScore, float rating)
    {
        NewsType newsType;
        NewsTemplate template;
        
        if (teamScore > opponentScore)
        {
            // Galibiyet haberi
            newsType = NewsType.MatchWin;
            template = NewsTemplateManager.GetRandomTemplate(NewsType.MatchWin);
        }
        else if (teamScore < opponentScore)
        {
            // Mağlubiyet haberi
            newsType = NewsType.MatchLoss;
            template = NewsTemplateManager.GetRandomTemplate(NewsType.MatchLoss);
        }
        else
        {
            // Beraberlik haberi
            newsType = NewsType.MatchDraw;
            template = NewsTemplateManager.GetRandomTemplate(NewsType.MatchDraw);
        }
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"teamName", teamName},
            {"opponentTeam", opponentTeam},
            {"score", $"{teamScore}-{opponentScore}"},
            {"rating", rating.ToString("F1")}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, newsType, source);
    }
    
    /// <summary>
    /// Olağanüstü performans haberi
    /// </summary>
    private void GenerateExceptionalPerformanceNews(string playerName, string teamName, float rating, int goals, int assists)
    {
        string title = $"⭐ {playerName} Mükemmel Performans!";
        string content = $"{playerName}, dün akşamki maçta {rating:F1} rating alarak mükemmel bir performans sergiledi. ";
        
        if (goals > 0 && assists > 0)
        {
            content += $"{goals} gol ve {assists} asist ile maçın yıldızı oldu.";
        }
        else if (goals > 0)
        {
            content += $"{goals} gol atarak takımının galibiyetine büyük katkı sağladı.";
        }
        else
        {
            content += "Oyun kurma ve savunma performansıyla dikkat çekti.";
        }
        
        CreateNews(title, content, NewsType.Performance, "Performans Raporu");
    }
    
    /// <summary>
    /// Düşük performans haberi
    /// </summary>
    private void GeneratePoorPerformanceNews(string playerName, string teamName, float rating)
    {
        string title = $"😞 {playerName} Zorlu Maç Geçirdi";
        string content = $"{playerName}, dün akşamki maçta {rating:F1} rating alarak zorlu anlar yaşadı. Oyuncu maç sonrası: 'Daha iyisini yapabilirim, çalışmaya devam edeceğim' açıklamasında bulundu.";
        
        CreateNews(title, content, NewsType.Performance, "Maç Analizi");
    }
    
    /// <summary>
    /// Transfer haberi üret
    /// </summary>
    public void GenerateTransferNews(string playerName, string fromTeam, string toTeam, float amount, int years)
    {
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.Transfer);
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"oldTeam", fromTeam},
            {"newTeam", toTeam},
            {"amount", amount.ToString("F1")},
            {"years", years.ToString()}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, NewsType.Transfer, source);
    }
    
    /// <summary>
    /// Sakatlık haberi üret
    /// </summary>
    public void GenerateInjuryNews(string playerName, string injuryType, int weeksOut)
    {
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.Injury);
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"injuryType", injuryType},
            {"weeks", weeksOut.ToString()},
            {"matchType", "antrenman"}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, NewsType.Injury, source);
    }
    
    /// <summary>
    /// Lig durumu haberi üret
    /// </summary>
    public void GenerateLeagueNews(string teamName, int position, int points, string rivalTeam, int rivalPoints, int week)
    {
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.League);
        
        var values = new Dictionary<string, string>
        {
            {"teamName", teamName},
            {"position", position.ToString()},
            {"points", points.ToString()},
            {"rivalTeam", rivalTeam},
            {"rivalPoints", rivalPoints.ToString()},
            {"week", week.ToString()}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, NewsType.League, source);
    }
    
    /// <summary>
    /// Haber oluştur ve kaydet
    /// </summary>
    private void CreateNews(string title, string content, NewsType type, string source)
    {
        var save = GameManager.Instance?.CurrentSave;
        if (save == null) return;
        
        if (save.mediaData == null)
        {
            save.mediaData = new MediaData();
        }
        
        var newsItem = new NewsItem
        {
            title = title,
            content = content,
            type = type,
            source = source,
            date = DateTime.Now.AddHours(newsDelayAfterMatch), // Maç sonrası gecikme
            isRead = false
        };
        newsItem.dateString = newsItem.date.ToString("dd.MM.yyyy HH:mm");
        
        save.mediaData.AddNews(newsItem);
        
        Debug.Log($"📰 Yeni haber oluşturuldu: {title}");
    }
    
    /// <summary>
    /// Haftalık rutin haberler üret
    /// </summary>
    public void GenerateWeeklyNews()
    {
        var save = GameManager.Instance?.CurrentSave;
        if (save == null) return;
        
        // Lig durumu haberi
        if (save.seasonData != null)
        {
            string playerTeam = save.playerProfile?.currentClubName ?? "Takım";
            int position = save.seasonData.leaguePosition;
            int points = save.seasonData.leaguePoints;
            int week = save.seasonData.matchesPlayed;
            
            GenerateLeagueNews(playerTeam, position, points, "Rakip Takım", points - 3, week);
        }
        
        // Rastgele söylenti haberi
        if (UnityEngine.Random.value < 0.3f) // %30 şans
        {
            GenerateRumourNews();
        }
    }
    
    /// <summary>
    /// Söylenti haberi üret
    /// </summary>
    private void GenerateRumourNews()
    {
        var save = GameManager.Instance?.CurrentSave;
        if (save == null) return;
        
        string playerName = save.playerProfile?.playerName ?? "Oyuncu";
        
        // Sadece DataPack'teki takımlardan söylenti üret
        string interestedTeam = "Bilinmeyen Kulüp";
        if (DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            var allTeams = DataPackManager.Instance.activeDataPack.GetAllTeams();
            if (allTeams != null && allTeams.Count > 0)
            {
                interestedTeam = allTeams[UnityEngine.Random.Range(0, allTeams.Count)].teamName;
            }
        }

        float amount = UnityEngine.Random.Range(10f, 50f);
        
        var template = NewsTemplateManager.GetRandomTemplate(NewsType.Rumour);
        
        var values = new Dictionary<string, string>
        {
            {"playerName", playerName},
            {"interestedTeam", interestedTeam},
            {"amount", amount.ToString("F0")}
        };
        
        string title = NewsTemplateManager.ReplacePlaceholders(template.titleTemplate, values);
        string content = NewsTemplateManager.ReplacePlaceholders(template.contentTemplate, values);
        string source = template.sources[UnityEngine.Random.Range(0, template.sources.Length)];
        
        CreateNews(title, content, NewsType.Rumour, source);
    }
}