using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Sezon sonu işlemlerini yöneten sistem
/// </summary>
public class SeasonEndSystem : MonoBehaviour
{
    public static SeasonEndSystem Instance { get; private set; }

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
    /// Sezonu sonlandır ve yeni sezona geç
    /// </summary>
    public void EndSeason()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogError("[SeasonEndSystem] Cannot end season: No active save!");
            return;
        }

        SaveData data = GameManager.Instance.CurrentSave;
        PlayerProfile player = data.playerProfile;
        SeasonData season = data.seasonData;

        Debug.Log($"[SeasonEndSystem] Ending Season {season.seasonNumber} ({season.seasonName})...");

        // 1. Kariyer Geçmişine Ekle
        if (player.careerHistory == null) player.careerHistory = new List<CareerHistoryEntry>();

        CareerHistoryEntry historyEntry = new CareerHistoryEntry(
            season.seasonName,
            data.clubData?.clubName ?? "Free Agent",
            season.matchesPlayed,
            season.goals,
            season.assists,
            season.averageRating
        );

        player.careerHistory.Add(historyEntry);
        Debug.Log($"[SeasonEndSystem] Archived season stats to career history.");

        // 2. Oyuncu Yaşını Güncelle (Basitçe +1)
        player.age++;
        Debug.Log($"[SeasonEndSystem] Player aged up to {player.age}.");

        // 3. Sezon Verilerini Sıfırla/Güncelle
        season.seasonNumber++;
        int startYear = season.seasonStartDate.Year + 1;
        season.seasonName = $"{startYear}-{startYear + 1}";
        
        // Tarihleri güncelle
        season.seasonStartDate = season.seasonStartDate.AddYears(1);
        season.seasonStartDateString = season.seasonStartDate.ToString("yyyy-MM-dd");
        season.seasonEndDate = season.seasonEndDate.AddYears(1);
        season.seasonEndDateString = season.seasonEndDate.ToString("yyyy-MM-dd");

        // İstatistikleri sıfırla
        season.matchesPlayed = 0;
        season.goals = 0;
        season.assists = 0;
        season.yellowCards = 0;
        season.redCards = 0;
        season.matchRatings.Clear();
        season.averageRating = 0f;
        season.currentWeek = 1;

        // 4. Yeni Fikstür Oluştur (Eğer takım varsa)
        if (data.clubData != null && !string.IsNullOrEmpty(data.clubData.leagueName))
        {
            if (DataPackManager.Instance != null)
            {
                LeagueData league = DataPackManager.Instance.GetLeague(data.clubData.leagueName);
                if (league != null && league.teams != null)
                {
                    season.fixtures = FixtureGenerator.GenerateSeasonFixtures(league.teams, season.seasonStartDate);
                    season.InitializeStandings(league.teams);
                    Debug.Log($"[SeasonEndSystem] Generated new fixtures for {season.seasonName}.");
                }
            }
        }

        // 5. Kaydet
        data.UpdateSaveDate();
        SaveSystem.SaveGame(data, GameManager.Instance.CurrentSaveSlotIndex);
        
        Debug.Log("[SeasonEndSystem] Season transition complete!");
    }
}
