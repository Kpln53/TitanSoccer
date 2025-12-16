using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sezon ve takvim yönetim sistemi
/// </summary>
public class SeasonCalendarSystem : MonoBehaviour
{
    public static SeasonCalendarSystem Instance;

    [Header("Sezon Ayarları")]
    [SerializeField] private int totalWeeksPerSeason = 38; // Standart lig sezonu

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
    /// Hafta ilerlet
    /// </summary>
    public void AdvanceWeek(SaveData saveData)
    {
        if (saveData == null || saveData.seasonData == null)
            return;
        
        saveData.seasonData.currentWeek++;
        
        // Sezon bitti mi?
        if (saveData.seasonData.currentWeek > totalWeeksPerSeason)
        {
            EndSeason(saveData);
        }
        else
        {
            // Hafta başı işlemler
            ProcessWeekStart(saveData);
        }
    }

    /// <summary>
    /// Sezon sonu
    /// </summary>
    private void EndSeason(SaveData saveData)
    {
        saveData.seasonData.seasonNumber++;
        saveData.seasonData.currentWeek = 1;
        
        // Yeni sezon başlangıç işlemleri
        ResetSeasonStats(saveData);
        GenerateNewFixtures(saveData);
    }

    /// <summary>
    /// Hafta başı işlemler
    /// </summary>
    private void ProcessWeekStart(SaveData saveData)
    {
        // Enerji yenileme
        if (FormMoralEnergySystem.Instance != null && saveData.playerProfile != null)
        {
            FormMoralEnergySystem.Instance.RestoreEnergyWeekly(saveData.playerProfile);
        }
        
        // Sakatlık ilerletme
        if (InjurySystem.Instance != null && saveData.playerProfile != null)
        {
            InjurySystem.Instance.ProgressInjuries(saveData.playerProfile, 7); // 7 gün geçti
        }
        
        // Transfer ilgisi kontrolü
        if (TransferSystem.Instance != null)
        {
            TransferSystem.Instance.CheckTransferInterest(
                saveData.playerProfile,
                saveData.clubData,
                saveData.seasonData
            );
        }
    }

    /// <summary>
    /// Sezon istatistiklerini sıfırla
    /// </summary>
    private void ResetSeasonStats(SaveData saveData)
    {
        if (saveData.playerProfile != null)
        {
            // Kariyer istatistiklerine ekle
            saveData.playerProfile.careerStats.totalMatches += saveData.playerProfile.seasonStats.matchesPlayed;
            saveData.playerProfile.careerStats.totalMinutes += saveData.playerProfile.seasonStats.minutesPlayed;
            saveData.playerProfile.careerStats.totalGoals += saveData.playerProfile.seasonStats.goals;
            saveData.playerProfile.careerStats.totalAssists += saveData.playerProfile.seasonStats.assists;
            saveData.playerProfile.careerStats.totalShots += saveData.playerProfile.seasonStats.shots;
            saveData.playerProfile.careerStats.totalPasses += saveData.playerProfile.seasonStats.passes;
            
            // Sezon istatistiklerini sıfırla
            saveData.playerProfile.seasonStats = new SeasonStats();
        }
    }

    /// <summary>
    /// Yeni fikstür oluştur
    /// </summary>
    private void GenerateNewFixtures(SaveData saveData)
    {
        if (saveData.seasonData == null)
            return;
        
        saveData.seasonData.fixtures.Clear();
        saveData.seasonData.completedFixtures.Clear();
        
        // Data Pack'ten lig takımlarını al
        if (DataPackManager.Instance == null || DataPackManager.Instance.GetActiveDataPack() == null)
            return;
        
        var dataPack = DataPackManager.Instance.GetActiveDataPack();
        var league = dataPack.GetLeagueByName(saveData.clubData.leagueName);
        
        if (league == null || league.teams == null)
            return;
        
        List<string> teamNames = new List<string>();
        foreach (var team in league.teams)
        {
            teamNames.Add(team.teamName);
        }
        
        // Basit fikstür oluştur (her takımla 2 maç: ev ve deplasman)
        for (int week = 1; week <= totalWeeksPerSeason; week++)
        {
            // Basit round-robin fikstür (gerçekçi değil ama çalışır)
            int teamIndex = (week - 1) % teamNames.Count;
            string homeTeam = teamNames[teamIndex];
            string awayTeam = teamNames[(teamIndex + 1) % teamNames.Count];
            
            Fixture fixture = new Fixture
            {
                week = week,
                homeTeam = homeTeam,
                awayTeam = awayTeam,
                isPlayed = false
            };
            
            saveData.seasonData.fixtures.Add(fixture);
        }
    }

    /// <summary>
    /// Mevcut haftanın maçını al
    /// </summary>
    public Fixture GetCurrentWeekMatch(SaveData saveData)
    {
        if (saveData == null || saveData.seasonData == null)
            return null;
        
        int currentWeek = saveData.seasonData.currentWeek;
        
        foreach (var fixture in saveData.seasonData.fixtures)
        {
            if (fixture.week == currentWeek && !fixture.isPlayed)
            {
                return fixture;
            }
        }
        
        return null;
    }

    /// <summary>
    /// Maçı tamamla
    /// </summary>
    public void CompleteMatch(SaveData saveData, int homeScore, int awayScore)
    {
        Fixture currentMatch = GetCurrentWeekMatch(saveData);
        if (currentMatch != null)
        {
            currentMatch.isPlayed = true;
            currentMatch.homeScore = homeScore;
            currentMatch.awayScore = awayScore;
            
            saveData.seasonData.completedFixtures.Add(currentMatch);
            saveData.seasonData.fixtures.Remove(currentMatch);
            
            // Puan durumunu güncelle
            UpdateStandings(saveData, currentMatch);
        }
    }

    /// <summary>
    /// Puan durumunu güncelle
    /// </summary>
    private void UpdateStandings(SaveData saveData, Fixture fixture)
    {
        // Basit puan durumu güncelleme
        // Gerçekçi bir sistem için daha fazla mantık gerekir
        // Şimdilik basit tutuyoruz
    }
}

