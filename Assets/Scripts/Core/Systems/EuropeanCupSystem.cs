using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Avrupa Kupaları Sistemi (Yeni Format - İsviçre Sistemi)
/// </summary>
public class EuropeanCupSystem : MonoBehaviour
{
    public static EuropeanCupSystem Instance { get; private set; }

    [System.Serializable]
    public class LeaguePhaseTable
    {
        public string teamName;
        public int points;
        public int goalDifference;
        public int goalsFor;
        public int matchesPlayed;
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
    /// Yeni sezon için Avrupa kupalarını başlat
    /// </summary>
    public void InitializeSeason(DataPack dataPack)
    {
        // 1. DataPack'ten turnuvaları bul (UCL, UEL, UECL)
        // 2. Takımları seç (Lig sıralamalarına göre - ilk sezon için rastgele veya puana göre)
        // 3. Lig aşaması fikstürünü oluştur (36 takım, 8 maç)
        
        Debug.Log("[EuropeanCupSystem] Initializing European Competitions...");
    }

    /// <summary>
    /// Lig aşaması fikstürü oluştur (Swiss Model benzeri)
    /// </summary>
    public List<MatchData> GenerateLeaguePhaseFixtures(List<string> teamNames)
    {
        List<MatchData> fixtures = new List<MatchData>();
        
        // Basitleştirilmiş mantık: Her takım rastgele 8 rakiple eşleşir
        // Gerçek sistemde torbalar (pots) vardır, şimdilik rastgele yapalım.
        
        // Takımları karıştır
        var shuffled = teamNames.OrderBy(x => Random.value).ToList();
        
        // Her takıma 8 maç ata
        // Not: Bu çok basitleştirilmiş bir algoritma, çakışmalar olabilir.
        // Tam bir Swiss System algoritması çok karmaşıktır.
        
        return fixtures;
    }
}
