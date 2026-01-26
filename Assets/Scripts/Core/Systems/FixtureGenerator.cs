using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Fikstür oluşturucu (Utility Class)
/// Lig takımları için Round-Robin algoritması ile maç takvimi oluşturur.
/// </summary>
public static class FixtureGenerator
{
    /// <summary>
    /// Verilen takımlar için tam bir sezonluk (Home & Away) fikstür oluşturur.
    /// </summary>
    /// <param name="teams">Ligdeki takımların listesi</param>
    /// <param name="startDate">Sezon başlangıç tarihi</param>
    /// <returns>Oluşturulan MatchData listesi</returns>
    public static List<MatchData> GenerateSeasonFixtures(List<TeamData> teams, DateTime startDate)
    {
        List<MatchData> allFixtures = new List<MatchData>();

        if (teams == null || teams.Count < 2)
        {
            Debug.LogError("[FixtureGenerator] Not enough teams to generate fixtures!");
            return allFixtures;
        }

        // Listeyi kopyala (orijinal listeyi bozmamak için)
        List<TeamData> leagueTeams = new List<TeamData>(teams);

        // Eğer takım sayısı tekse, "Bye" (Bay) geçmek için dummy bir takım ekle
        if (leagueTeams.Count % 2 != 0)
        {
            leagueTeams.Add(new TeamData { teamName = "BYE" }); // Dummy team
        }

        int numTeams = leagueTeams.Count;
        int numRounds = numTeams - 1; // Bir devredeki hafta sayısı (N-1)
        int matchesPerRound = numTeams / 2;

        Debug.Log($"[FixtureGenerator] Generating fixtures for {numTeams} teams ({numRounds} rounds per half).");

        // --- 1. DEVRE (First Half) ---
        List<MatchData> firstHalf = GenerateRoundRobin(leagueTeams, startDate, false);
        allFixtures.AddRange(firstHalf);

        // --- 2. DEVRE (Second Half) ---
        // 1. devrenin bitişinden 1 hafta sonra başlasın (Devre arası yok varsayıyoruz veya kısa tutuyoruz)
        DateTime secondHalfStartDate = startDate.AddDays(numRounds * 7); 
        
        // 2. devre için rövanş maçları (Home/Away tersine çevrilir)
        List<MatchData> secondHalf = GenerateRoundRobin(leagueTeams, secondHalfStartDate, true);
        allFixtures.AddRange(secondHalf);

        Debug.Log($"[FixtureGenerator] Generated {allFixtures.Count} matches in total.");

        return allFixtures;
    }

    /// <summary>
    /// Round-Robin algoritması ile tek devrelik fikstür oluşturur
    /// </summary>
    /// <param name="teams">Takım listesi (Çift sayıda olmalı)</param>
    /// <param name="startDate">Başlangıç tarihi</param>
    /// <param name="isSecondHalf">İkinci devre mi? (Ev sahibi/Deplasman değişimi için)</param>
    private static List<MatchData> GenerateRoundRobin(List<TeamData> teams, DateTime startDate, bool isSecondHalf)
    {
        List<MatchData> fixtures = new List<MatchData>();
        int numTeams = teams.Count;
        int numRounds = numTeams - 1;
        int matchesPerRound = numTeams / 2;

        List<string> teamNames = teams.Select(t => t.teamName).ToList();

        for (int round = 0; round < numRounds; round++)
        {
            DateTime matchDate = startDate.AddDays(round * 7); // Her hafta 1 maç
            int weekNum = isSecondHalf ? (numRounds + round + 1) : (round + 1); // Hafta numarası hesapla

            for (int match = 0; match < matchesPerRound; match++)
            {
                int homeIndex = (round + match) % (numTeams - 1);
                int awayIndex = (numTeams - 1 - match + round) % (numTeams - 1);

                // Son takım (sabit) için özel durum
                if (match == 0)
                {
                    awayIndex = numTeams - 1;
                }

                string homeTeam = teamNames[homeIndex];
                string awayTeam = teamNames[awayIndex];

                // "BYE" takımıyla olan maçları atla (Bay geçme)
                if (homeTeam == "BYE" || awayTeam == "BYE")
                    continue;

                // İkinci devreyse ev sahibi ve deplasmanı değiştir
                if (isSecondHalf)
                {
                    string temp = homeTeam;
                    homeTeam = awayTeam;
                    awayTeam = temp;
                }

                // Maç tipini belirle (Derbi kontrolü)
                MatchData.MatchType mType = MatchData.MatchType.League;
                if (IsDerby(homeTeam, awayTeam))
                {
                    mType = MatchData.MatchType.Derby;
                }

                MatchData newMatch = new MatchData(homeTeam, awayTeam, matchDate, mType, weekNum);
                fixtures.Add(newMatch);
            }
        }

        return fixtures;
    }

    private static bool IsDerby(string team1, string team2)
    {
        if (DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            var rivalries = DataPackManager.Instance.activeDataPack.rivalries;
            if (rivalries != null)
            {
                foreach (var r in rivalries)
                {
                    if ((r.team1 == team1 && r.team2 == team2) || (r.team1 == team2 && r.team2 == team1))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
