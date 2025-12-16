using System.Collections.Generic;

/// <summary>
/// Sezon verileri
/// </summary>
[System.Serializable]
public class SeasonData
{
    public int seasonNumber = 1;
    public int currentWeek = 1;
    public int totalWeeks = 38; // Standart lig sezonu
    
    [Header("Fikstür")]
    public List<Fixture> fixtures = new List<Fixture>();
    public List<Fixture> completedFixtures = new List<Fixture>();
    
    [Header("Puan Durumu")]
    public List<LeagueStanding> standings = new List<LeagueStanding>();
    
    [Header("Takvim")]
    public List<CalendarEvent> calendarEvents = new List<CalendarEvent>();
}

/// <summary>
/// Maç fikstürü
/// </summary>
[System.Serializable]
public class Fixture
{
    public int week;
    public string homeTeam;
    public string awayTeam;
    public bool isPlayed = false;
    public int homeScore = 0;
    public int awayScore = 0;
}

/// <summary>
/// Lig sıralaması
/// </summary>
[System.Serializable]
public class LeagueStanding
{
    public string teamName;
    public int played = 0;
    public int won = 0;
    public int drawn = 0;
    public int lost = 0;
    public int goalsFor = 0;
    public int goalsAgainst = 0;
    public int goalDifference => goalsFor - goalsAgainst;
    public int points => (won * 3) + drawn;
}

/// <summary>
/// Takvim olayı
/// </summary>
[System.Serializable]
public class CalendarEvent
{
    public int week;
    public CalendarEventType eventType;
    public string description;
}

public enum CalendarEventType
{
    Match,
    TransferWindow,
    NationalTeam,
    Training,
    SpecialEvent
}

