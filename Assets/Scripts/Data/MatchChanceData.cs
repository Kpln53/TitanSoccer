using UnityEngine;

/// <summary>
/// Maç pozisyonu/şans verisi - Oyuncuya karar verilecek pozisyonlar için
/// </summary>
[System.Serializable]
public class MatchChanceData
{
    public int minute;                  // Pozisyonun olduğu dakika
    public MatchChanceType chanceType;  // Pozisyon türü
    public string description;          // Pozisyon açıklaması
    public float successChance;         // Başarı ihtimali (0-1)
    public MatchChanceResult result;    // Pozisyon sonucu (karar verildikten sonra)

    public MatchChanceData()
    {
        minute = 0;
        chanceType = MatchChanceType.Shot;
        description = "";
        successChance = 0.5f;
        result = MatchChanceResult.Pending;
    }
}

/// <summary>
/// Pozisyon/şans türleri
/// </summary>
public enum MatchChanceType
{
    Shot,           // Şut
    Pass,           // Pas
    Dribble,        // Çalım/Drubl
    Cross,          // Orta
    ThroughBall,    // Ara pası
    OneOnOne,       // 1v1 (kaleci ile)
    FreeKick,       // Serbest vuruş
    Penalty,        // Penaltı
    Header          // Kafa vuruşu
}

/// <summary>
/// Pozisyon sonucu
/// </summary>
public enum MatchChanceResult
{
    Pending,        // Henüz karar verilmedi
    Goal,           // Gol
    Save,           // Kaleci kurtardı
    Miss,           // Kaçtı/ışkaladı
    Assist,         // Asist (pas sonucu)
    Blocked,        // Bloklandı
    Foul            // Faul yapıldı
}

