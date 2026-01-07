using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Match Context - Maç verilerini scene'ler arasında taşır (DontDestroyOnLoad Singleton)
/// </summary>
public class MatchContext : MonoBehaviour
{
    public static MatchContext Instance { get; private set; }

    [Header("Takım Bilgileri")]
    public string homeTeamName = "";
    public string awayTeamName = "";
    public bool isPlayerHome = true;  // Oyuncu ev sahibi mi?
    public MatchData.MatchType matchType = MatchData.MatchType.League;
    public List<SquadPlayer> homeSquad = new List<SquadPlayer>();
    public List<SquadPlayer> awaySquad = new List<SquadPlayer>();

    [Header("Skor")]
    public int homeScore = 0;
    public int awayScore = 0;
    public int currentMinute = 0;

    [Header("Possession")]
    public float homePossessionPercent = 50f;
    public float awayPossessionPercent = 50f;

    [Header("Oyuncu İstatistikleri")]
    public string playerName = "";
    public PlayerPosition playerPosition = PlayerPosition.SF;
    public int playerOverall = 50;
    public float playerEnergy = 100f;
    public float playerMoral = 50f;
    public float playerMatchRating = 5.0f;
    public int playerGoals = 0;
    public int playerAssists = 0;
    public int playerShots = 0;

    [Header("Takım Güçleri")]
    public int homeTeamPower = 50;
    public int awayTeamPower = 50;

    [Header("Mevcut Pozisyon")]
    public ChanceData currentChance = new ChanceData();

    [Header("Yorumlar")]
    public List<string> commentaryLines = new List<string>();

    [System.Serializable]
    public class SquadPlayer
    {
        public string playerName;
        public int overall;
        public PlayerPosition position;
        public float matchRating; // 0-10
        public bool isStartingXI;

        public SquadPlayer(string name, int ovr, PlayerPosition pos, bool starting = true)
        {
            playerName = name;
            overall = ovr;
            position = pos;
            matchRating = 5.0f;
            isStartingXI = starting;
        }
    }

    [System.Serializable]
    public class ChanceData
    {
        public enum ChanceType { Attack, Defense }
        public ChanceType chanceType;
        public int minute;
        public float successChance; // 0-1
        public string spawnTemplateInfo;

        public ChanceData()
        {
            chanceType = ChanceType.Attack;
            minute = 0;
            successChance = 0.5f;
            spawnTemplateInfo = "";
        }
    }

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
    /// Tüm maç verilerini temizle
    /// </summary>
    public void Clear()
    {
        homeTeamName = "";
        awayTeamName = "";
        isPlayerHome = true;
        matchType = MatchData.MatchType.League;
        homeSquad.Clear();
        awaySquad.Clear();
        homeScore = 0;
        awayScore = 0;
        currentMinute = 0;
        homePossessionPercent = 50f;
        awayPossessionPercent = 50f;
        playerName = "";
        playerPosition = PlayerPosition.SF;
        playerOverall = 50;
        playerEnergy = 100f;
        playerMoral = 50f;
        playerMatchRating = 5.0f;
        playerGoals = 0;
        playerAssists = 0;
        playerShots = 0;
        homeTeamPower = 50;
        awayTeamPower = 50;
        currentChance = new ChanceData();
        commentaryLines.Clear();
    }

    /// <summary>
    /// Yorum satırı ekle
    /// </summary>
    public void AddCommentary(string line)
    {
        if (commentaryLines == null)
            commentaryLines = new List<string>();

        commentaryLines.Add(line);

        // Maksimum 50 satır tut (performans için)
        if (commentaryLines.Count > 50)
        {
            commentaryLines.RemoveAt(0);
        }
    }
}

