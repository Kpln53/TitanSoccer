using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Season Calendar System - Sezon takvimi ve fikstür yönetimi
/// </summary>
public class SeasonCalendarSystem : MonoBehaviour
{
    private static SeasonCalendarSystem instance;
    public static SeasonCalendarSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SeasonCalendarSystem");
                instance = obj.AddComponent<SeasonCalendarSystem>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    [Header("Takvim")]
    public List<MatchData> fixtures = new List<MatchData>();
    public DateTime currentDate = DateTime.Now;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    /// <summary>
    /// Sonraki maçı getir
    /// </summary>
    public MatchData GetNextMatch()
    {
        foreach (var match in fixtures)
        {
            if (!match.isPlayed && match.matchDate >= currentDate)
            {
                return match;
            }
        }
        return null;
    }

    /// <summary>
    /// Fikstür oluştur (basit)
    /// </summary>
    public void GenerateFixtures(List<string> teamNames, DateTime seasonStart)
    {
        fixtures.Clear();
        currentDate = seasonStart;

        // Basit round-robin fikstür (her takım bir kez ev sahibi, bir kez deplasman)
        for (int i = 0; i < teamNames.Count; i++)
        {
            for (int j = i + 1; j < teamNames.Count; j++)
            {
                // Ev sahibi maç
                fixtures.Add(new MatchData(teamNames[i], teamNames[j], seasonStart.AddDays(fixtures.Count * 7), MatchData.MatchType.League));
                
                // Deplasman maçı
                fixtures.Add(new MatchData(teamNames[j], teamNames[i], seasonStart.AddDays(fixtures.Count * 7), MatchData.MatchType.League));
            }
        }

        // Tarihe göre sırala
        fixtures.Sort((a, b) => a.matchDate.CompareTo(b.matchDate));
    }

    /// <summary>
    /// Tarihi ilerlet
    /// </summary>
    public void AdvanceDate(int days = 1)
    {
        currentDate = currentDate.AddDays(days);
    }
}
