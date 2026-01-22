using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Lig Simülasyon Sistemi
/// Diğer takımların maçlarını ve lig puan durumunu yönetir
/// </summary>
public class LeagueSimulationSystem : MonoBehaviour
{
    public static LeagueSimulationSystem Instance { get; private set; }

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
    /// Haftanın maçlarını simüle et (Oyuncunun maçı hariç)
    /// </summary>
    public void SimulateWeek(int weekNumber, string playerTeamName)
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        var save = GameManager.Instance.CurrentSave;
        var season = save.seasonData;
        
        // Fikstürden o haftanın maçlarını bul
        var weekMatches = season.fixtures.Where(m => m.weekNumber == weekNumber && !m.isPlayed).ToList();
        
        foreach (var match in weekMatches)
        {
            // Oyuncunun maçını atla (zaten oynandı veya oynanacak)
            if (match.homeTeamName == playerTeamName || match.awayTeamName == playerTeamName) continue;

            // Maçı simüle et
            SimulateMatch(match);
            
            // Sonucu kaydet
            season.RecordMatchResult(match.homeTeamName, match.awayTeamName, match.homeScore, match.awayScore);
        }
        
        // Puan durumunu güncelle
        season.UpdatePlayerLeaguePosition(playerTeamName);
        
        Debug.Log($"[LeagueSimulation] Week {weekNumber} simulated. {weekMatches.Count} matches played.");
    }

    /// <summary>
    /// Tek bir maçı simüle et (Takım güçlerine göre)
    /// </summary>
    private void SimulateMatch(MatchData match)
    {
        // Takım güçlerini al (DataPack'ten)
        int homePower = GetTeamPower(match.homeTeamName);
        int awayPower = GetTeamPower(match.awayTeamName);

        // Ev sahibi avantajı (+5 güç)
        homePower += 5;

        // Güç farkı
        int powerDiff = homePower - awayPower;
        
        // Skor üretimi (Poisson dağılımı benzeri basit mantık)
        // Güç farkına göre gol beklentisi (xG)
        float homeXG = 1.5f + (powerDiff / 20f);
        float awayXG = 1.0f - (powerDiff / 20f);
        
        // Rastgelelik ekle
        homeXG = Mathf.Max(0, homeXG + UnityEngine.Random.Range(-1.0f, 1.5f));
        awayXG = Mathf.Max(0, awayXG + UnityEngine.Random.Range(-1.0f, 1.5f));

        match.homeScore = Mathf.RoundToInt(homeXG);
        match.awayScore = Mathf.RoundToInt(awayXG);
        match.isPlayed = true;
        
        // Beraberlik kontrolü (Güçler yakınsa beraberlik ihtimali artar)
        if (Mathf.Abs(powerDiff) < 10 && UnityEngine.Random.value < 0.3f)
        {
            if (match.homeScore != match.awayScore)
            {
                int drawScore = Mathf.Min(match.homeScore, match.awayScore);
                match.homeScore = drawScore;
                match.awayScore = drawScore;
            }
        }
    }

    private int GetTeamPower(string teamName)
    {
        if (DataPackManager.Instance != null)
        {
            var team = DataPackManager.Instance.GetTeam(teamName);
            if (team != null) return team.GetTeamPower();
        }
        return 70; // Varsayılan güç
    }
}
