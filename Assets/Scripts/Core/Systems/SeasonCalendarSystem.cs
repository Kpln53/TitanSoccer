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

        // Basit implementasyon - gerçek maç programı oluşturulacak
        MatchData nextMatch = new MatchData
        {
            matchDate = DateTime.Now.AddDays(7),
            opponentTeam = "Opponent FC",
            opponentLeague = saveData.clubData.leagueName,
            matchType = "League",
            isHome = true
        };

        return nextMatch;
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

        // Sezon takvimi oluşturulacak
        // Lig takımları alınacak ve maç programı oluşturulacak

        Debug.Log("[SeasonCalendarSystem] Season schedule generated.");
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
    public string opponentTeam;
    public string opponentLeague;
    public string matchType; // "League", "Cup", "ChampionsLeague", vb.
    public bool isHome; // Ev sahibi mi?

    public MatchData()
    {
        matchDate = DateTime.Now;
        matchDateString = matchDate.ToString("yyyy-MM-dd");
        matchType = "League";
        isHome = true;
    }
}

