using UnityEngine;

/// <summary>
/// Pozisyon durumları ve tipleri
/// </summary>
namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Pozisyon tipi (Atak/Savunma)
    /// </summary>
    public enum ChanceType
    {
        Attack,     // Atak pozisyonu - top bizde başlar
        Defense     // Savunma pozisyonu - top rakipte başlar
    }

    /// <summary>
    /// Pozisyon sonucu
    /// </summary>
    public enum ChanceOutcome
    {
        None,           // Henüz sonuçlanmadı
        Goal,           // Gol!
        Saved,          // Kaleci kurtardı
        Missed,         // Kaçırdık (aut, üstten vs.)
        Blocked,        // Defans blokları
        Cleared,        // Top uzaklaştırıldı (pozisyon bitti)
        Intercepted,    // Pas kesildi
        Tackled         // Top kapıldı
    }

    /// <summary>
    /// Oyun akış durumu
    /// </summary>
    public enum GameFlowState
    {
        WaitingForInput,    // Slow-motion, hamle bekleniyor
        Executing,          // Eylem yapılıyor (normal hız)
        BallInFlight,       // Top havada (pas/şut)
        AIPlaying,          // AI oynuyor (pas sonrası takım arkadaşı)
        Ended               // Pozisyon bitti
    }

    /// <summary>
    /// Eylem tipleri
    /// </summary>
    public enum ActionType
    {
        None,
        Move,           // Hareket
        Pass,           // Pas
        Shoot,          // Şut
        SlideTackle,    // Kayarak müdahale
        Dribble         // Top sürme
    }

    /// <summary>
    /// Çizgi analiz sonucu
    /// </summary>
    [System.Serializable]
    public class LineData
    {
        public Vector2 startPoint;      // Başlangıç noktası
        public Vector2 endPoint;        // Bitiş noktası
        public Vector2[] points;        // Tüm noktalar
        public float length;            // Toplam uzunluk
        public float curvature;         // Eğrilik (-1 sol, +1 sağ, 0 düz)
        public float drawSpeed;         // Çizim hızı
        public ActionType detectedAction; // Tespit edilen eylem
        public GameObject targetPlayer; // Hedef oyuncu (pas için)
        public Vector2 targetPosition;  // Hedef pozisyon

        public LineData()
        {
            points = new Vector2[0];
            curvature = 0f;
            drawSpeed = 0f;
            detectedAction = ActionType.None;
        }
    }

    /// <summary>
    /// Pozisyon başlangıç verileri
    /// </summary>
    [System.Serializable]
    public class ChanceSetupData
    {
        public ChanceType chanceType;
        public int minute;
        public PlayerPosition playerPosition;   // Oyuncumuzun mevkisi
        public int playerOverall;
        public int homeTeamPower;
        public int awayTeamPower;
        public float playerEnergy;
        public float playerForm;

        // Stat verileri
        public int shootingStat;
        public int passingStat;
        public int dribblingStat;
        public int speedStat;
        public int physicsStat;
        public int defenseStat;
        public int falsoStat;
    }
}

