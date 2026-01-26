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
        // Temel gol beklentisi: 1.3
        float baseGoals = 1.3f;
        
        // Güç farkı etkisi: Her 10 puan fark ~0.2 gol farkı
        float powerFactor = powerDiff / 50f; 
        
        float homeXG = baseGoals + powerFactor;
        float awayXG = baseGoals - powerFactor;
        
        // Rastgelelik ekle (Form, şans vb.)
        homeXG *= UnityEngine.Random.Range(0.7f, 1.4f);
        awayXG *= UnityEngine.Random.Range(0.7f, 1.4f);
        
        // Negatif olamaz
        homeXG = Mathf.Max(0.1f, homeXG);
        awayXG = Mathf.Max(0.1f, awayXG);

        // Poisson benzeri dağılım ile skor belirle
        match.homeScore = GenerateScoreFromXG(homeXG);
        match.awayScore = GenerateScoreFromXG(awayXG);
        match.isPlayed = true;
        
        // Beraberlik kontrolü (Güçler yakınsa beraberlik ihtimali artar)
        // Eğer güç farkı azsa ve skorlar farklıysa, bazen berabere bitir
        if (Mathf.Abs(powerDiff) < 10 && match.homeScore != match.awayScore)
        {
            if (UnityEngine.Random.value < 0.25f) // %25 şansla beraberliğe zorla
            {
                int drawScore = Mathf.Min(match.homeScore, match.awayScore);
                match.homeScore = drawScore;
                match.awayScore = drawScore;
            }
        }
    }
    
    private int GenerateScoreFromXG(float xG)
    {
        // Basit bir Poisson simülasyonu
        float L = Mathf.Exp(-xG);
        float p = 1.0f;
        int k = 0;

        do
        {
            k++;
            p *= UnityEngine.Random.value;
        } while (p > L);

        return k - 1;
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
