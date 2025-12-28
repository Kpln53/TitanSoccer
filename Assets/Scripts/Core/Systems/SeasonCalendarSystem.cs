using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Sezon takvimi sistemi - Maç programı yönetimi (Singleton)
/// </summary>
public class SeasonCalendarSystem : MonoBehaviour
{
    public static SeasonCalendarSystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[SeasonCalendarSystem] SeasonCalendarSystem initialized.");
    }

    /// <summary>
    /// Sıradaki maçı getir
    /// </summary>
    public MatchData GetNextMatch(SaveData saveData)
    {
        if (saveData == null || saveData.seasonData == null || saveData.clubData == null)
        {
            Debug.LogWarning("[SeasonCalendarSystem] SaveData or required data is null!");
            return null;
        }

        string playerClubName = saveData.clubData.clubName;
        string leagueName = saveData.clubData.leagueName;

        // Eğer sezon takvimi yoksa oluştur
        if (saveData.seasonData.fixtures == null || saveData.seasonData.fixtures.Count == 0)
        {
            Debug.Log("[SeasonCalendarSystem] No fixtures found, generating season schedule...");
            GenerateSeasonSchedule(saveData);
        }

        // Sıradaki maçı bul (bugünden ileri tarihli ilk maç)
        DateTime today = DateTime.Now.Date;
        MatchData nextMatch = null;
        
        if (saveData.seasonData.fixtures != null)
        {
            foreach (var match in saveData.seasonData.fixtures)
            {
                if (match.matchDate >= today && 
                    (match.homeTeam == playerClubName || match.awayTeam == playerClubName))
                {
                    // opponentTeam'i oyuncunun takımına göre ayarla
                    if (match.homeTeam == playerClubName)
                    {
                        match.isHome = true;
                        match.opponentTeam = match.awayTeam;
                    }
                    else
                    {
                        match.isHome = false;
                        match.opponentTeam = match.homeTeam;
                    }
                    
                    nextMatch = match;
                    break;
                }
            }
        }

        // Eğer fikstür yoksa, ligden rastgele bir takım seç
        if (nextMatch == null)
        {
            nextMatch = CreateFallbackMatch(saveData, playerClubName, leagueName);
        }

        return nextMatch;
    }

    /// <summary>
    /// Fallback maç oluştur (fikstür yoksa ligden rastgele takım seç)
    /// </summary>
    private MatchData CreateFallbackMatch(SaveData saveData, string playerClubName, string leagueName)
    {
        // Ligden rastgele bir takım seç
        string opponentTeam = GetRandomOpponentFromLeague(leagueName, playerClubName);
        
        bool isHomeMatch = UnityEngine.Random.Range(0, 2) == 0; // %50 şans ev sahibi
        
        MatchData fallbackMatch = new MatchData
        {
            matchDate = DateTime.Now.AddDays(7),
            opponentTeam = opponentTeam,
            opponentLeague = leagueName,
            matchType = "League",
            isHome = isHomeMatch,
            homeTeam = isHomeMatch ? playerClubName : opponentTeam,
            awayTeam = isHomeMatch ? opponentTeam : playerClubName
        };
        fallbackMatch.matchDateString = fallbackMatch.matchDate.ToString("yyyy-MM-dd");

        return fallbackMatch;
    }

    /// <summary>
    /// Ligden oyuncunun takımı hariç rastgele bir rakip takım seç
    /// </summary>
    private string GetRandomOpponentFromLeague(string leagueName, string excludeTeam)
    {
        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
        {
            Debug.LogWarning("[SeasonCalendarSystem] DataPackManager or activeDataPack is null!");
            return "Rakip Takım";
        }

        LeagueData league = DataPackManager.Instance.GetLeague(leagueName);
        if (league == null || league.teams == null || league.teams.Count == 0)
        {
            Debug.LogWarning($"[SeasonCalendarSystem] League '{leagueName}' not found or has no teams!");
            return "Rakip Takım";
        }

        // Oyuncunun takımı hariç takımları filtrele
        List<TeamData> opponentTeams = new List<TeamData>();
        foreach (var team in league.teams)
        {
            if (team != null && team.teamName != excludeTeam)
            {
                opponentTeams.Add(team);
            }
        }

        if (opponentTeams.Count == 0)
        {
            return "Rakip Takım";
        }

        // Rastgele bir takım seç
        int randomIndex = UnityEngine.Random.Range(0, opponentTeams.Count);
        return opponentTeams[randomIndex].teamName;
    }

    /// <summary>
    /// Belirli bir hafta için maçları getir
    /// </summary>
    public List<MatchData> GetMatchesForWeek(SaveData saveData, DateTime weekStart)
    {
        List<MatchData> matches = new List<MatchData>();

        // Basit implementasyon
        // Gerçek maç programı oluşturulacak

        return matches;
    }

    /// <summary>
    /// Maç programı oluştur (sezon başında)
    /// </summary>
    public void GenerateSeasonSchedule(SaveData saveData)
    {
        if (saveData == null || saveData.clubData == null)
        {
            Debug.LogWarning("[SeasonCalendarSystem] SaveData or ClubData is null! Cannot generate schedule.");
            return;
        }

        string leagueName = saveData.clubData.leagueName;
        string playerClubName = saveData.clubData.clubName;

        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
        {
            Debug.LogWarning("[SeasonCalendarSystem] DataPackManager or activeDataPack is null! Cannot generate schedule.");
            return;
        }

        LeagueData league = DataPackManager.Instance.GetLeague(leagueName);
        if (league == null || league.teams == null || league.teams.Count < 2)
        {
            Debug.LogWarning($"[SeasonCalendarSystem] League '{leagueName}' not found or has less than 2 teams!");
            return;
        }

        // SeasonData'da fixtures listesi yoksa oluştur
        if (saveData.seasonData.fixtures == null)
        {
            saveData.seasonData.fixtures = new List<MatchData>();
        }
        else
        {
            saveData.seasonData.fixtures.Clear();
        }

        // Basit fikstür: Oyuncunun takımının diğer takımlarla maçları
        List<TeamData> teams = league.teams;
        DateTime currentDate = DateTime.Now;
        int weekNumber = 0;

        // Sadece oyuncunun takımının maçlarını oluştur
        foreach (var team in teams)
        {
            if (team == null || team.teamName == playerClubName) continue;

            weekNumber++;

            // Ev sahibi maç (oyuncunun takımı evde)
            MatchData homeMatch = new MatchData
            {
                matchDate = currentDate.AddDays(weekNumber * 7),
                opponentTeam = team.teamName,
                opponentLeague = leagueName,
                matchType = "League",
                isHome = true,
                homeTeam = playerClubName,
                awayTeam = team.teamName
            };
            homeMatch.matchDateString = homeMatch.matchDate.ToString("yyyy-MM-dd");
            saveData.seasonData.fixtures.Add(homeMatch);

            weekNumber++;

            // Deplasman maçı (oyuncunun takımı deplasmanda)
            MatchData awayMatch = new MatchData
            {
                matchDate = currentDate.AddDays(weekNumber * 7),
                opponentTeam = team.teamName,
                opponentLeague = leagueName,
                matchType = "League",
                isHome = false,
                homeTeam = team.teamName,
                awayTeam = playerClubName
            };
            awayMatch.matchDateString = awayMatch.matchDate.ToString("yyyy-MM-dd");
            saveData.seasonData.fixtures.Add(awayMatch);
        }

        // Tarihe göre sırala
        saveData.seasonData.fixtures.Sort((a, b) => a.matchDate.CompareTo(b.matchDate));

        Debug.Log($"[SeasonCalendarSystem] Season schedule generated with {saveData.seasonData.fixtures.Count} matches.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

/// <summary>
/// Maç verisi (basit yapı)
/// </summary>
[Serializable]
public class MatchData
{
    public DateTime matchDate;
    public string matchDateString;
    public string opponentTeam; // Rakip takım (oyuncunun takımına göre)
    public string opponentLeague;
    public string matchType; // "League", "Cup", "ChampionsLeague", vb.
    public bool isHome; // Ev sahibi mi? (oyuncunun takımına göre)
    public string homeTeam; // Ev sahibi takım adı
    public string awayTeam; // Deplasman takım adı

    // Maç Sonucu (simülasyon sonrası doldurulur)
    public bool isPlayed = false; // Maç oynandı mı?
    public int homeScore = 0;
    public int awayScore = 0;
    public float playerRating = 0f; // Oyuncunun maç rating'i
    public int playerMinutesPlayed = 0; // Oyuncunun oynadığı dakika
    public int playerGoals = 0; // Oyuncunun attığı gol
    public int playerAssists = 0; // Oyuncunun yaptığı asist

    // Maç İstatistikleri
    public int homePossession = 50; // Ev sahibi takımın topa sahip olma yüzdesi
    public int awayPossession = 50;
    public int homeShots = 0;
    public int awayShots = 0;
    public int homeShotsOnTarget = 0;
    public int awayShotsOnTarget = 0;

    // Maç Olayları
    public List<MatchEvent> matchEvents = new List<MatchEvent>();

    public MatchData()
    {
        matchDate = DateTime.Now;
        matchDateString = matchDate.ToString("yyyy-MM-dd");
        matchType = "League";
        isHome = true;
        matchEvents = new List<MatchEvent>();
    }
}

/// <summary>
/// Maç olayları (gol, kart, değişiklik vb.)
/// </summary>
[Serializable]
public class MatchEvent
{
    public int minute; // Olayın gerçekleştiği dakika
    public MatchEventType eventType; // Olay türü
    public string teamName; // Hangi takımdan
    public string playerName; // Olayın kahramanı oyuncu
    public string description; // Olay açıklaması

    public MatchEvent()
    {
        minute = 0;
        eventType = MatchEventType.Goal;
        teamName = "";
        playerName = "";
        description = "";
    }
}

/// <summary>
/// Maç olay türleri
/// </summary>
public enum MatchEventType
{
    Goal,           // Gol
    Assist,         // Asist
    YellowCard,     // Sarı kart
    RedCard,        // Kırmızı kart
    Substitution,   // Değişiklik
    Save,           // Kaleci kurtarışı
    ChanceMissed,   // Kaçan fırsat
    Foul,           // Faul
    HalfTime,       // Devre arası
    MatchStart,     // Maç başlangıcı
    MatchEnd        // Maç sonu
}

