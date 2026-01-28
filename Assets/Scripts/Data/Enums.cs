/// <summary>
/// Tüm oyun enum'ları bu dosyada toplanmıştır
/// </summary>

/// <summary>
/// Oyuncu pozisyonları
/// </summary>
public enum PlayerPosition
{
    KL,   // Kaleci (Goalkeeper)
    STP,  // Stoper (Center Back)
    SĞB,  // Sağ Bek (Right Back)
    SLB,  // Sol Bek (Left Back)
    MDO,  // Merkez Orta Defans (Defensive Midfielder)
    MOO,  // Merkez Orta Ofans (Attacking Midfielder)
    SĞK,  // Sağ Kanat (Right Wing)
    SLK,  // Sol Kanat (Left Wing)
    SĞO,  // Sağ Orta (Right Midfielder)
    SLO,  // Sol Orta (Left Midfielder)
    SF    // Santrafor (Striker/Forward)
}

/// <summary>
/// Sözleşmede oyuncunun rolü
/// </summary>
public enum ContractRole
{
    Starter,    // İlk 11'de başlayan
    Rotation,   // Rotasyon oyuncusu
    Substitute  // Yedek
}

/// <summary>
/// Oynama süresi garantisi
/// </summary>
public enum PlayingTime
{
    Starter,    // İlk 11 garantisi
    Rotation,   // Rotasyon garantisi
    Substitute  // Yedek
}

/// <summary>
/// Sözleşme maddesi türleri
/// </summary>
public enum ClauseType
{
    ReleaseClause,      // Serbest kalma maddesi
    BuyoutClause,       // Satın alma maddesi
    SalaryIncrease,     // Maaş artışı
    PerformanceBonus,   // Performans bonusu
    TeamSuccessBonus,   // Takım başarısı bonusu
    InjuryInsurance,    // Sakatlık sigortası
    LoyaltyBonus        // Sadakat bonusu
}

/// <summary>
/// Bonus ve prim türleri
/// </summary>
public enum BonusType
{
    SigningBonus,       // İmza parası
    MatchFee,           // Maç başı ücret
    GoalBonus,          // Gol bonusu
    AssistBonus,        // Asist bonusu
    CleanSheet,         // Temiz sayfa bonusu (kaleci/defans için)
    SeasonEnd,          // Sezon sonu bonusu
    TeamSuccess,        // Takım başarısı bonusu
    Loyalty,            // Sadakat bonusu
    WinBonus,           // Galibiyet bonusu
    CleanSheetBonus,    // Gol yememe bonusu
    ChampionshipBonus   // Şampiyonluk bonusu
}

/// <summary>
/// Haber türleri
/// </summary>
public enum NewsType
{
    Transfer,           // Transfer haberleri
    League,             // Lig haberleri
    Injury,             // Sakatlık haberleri
    Achievement,        // Başarı haberleri
    Rumour,             // Söylenti haberleri
    Match,              // Genel Maç haberleri
    MatchWin,           // Galibiyet haberleri
    MatchLoss,          // Mağlubiyet haberleri
    MatchDraw,          // Beraberlik haberleri
    Contract,           // Sözleşme haberleri
    Performance,        // Performans haberleri
    TeamManagement      // Takım yönetimi haberleri
}

/// <summary>
/// Oyuncunun habere verdiği tepki
/// </summary>
public enum NewsReaction
{
    Positive,   // Olumlu
    Negative,   // Olumsuz
    Neutral     // Umursamaz
}

/// <summary>
/// Item türleri
/// </summary>
public enum ItemType
{
    Cleats,         // Krampon
    Luxury,         // Lüks eşya
    EnergyDrink,    // Enerji içeceği
    RehabItem       // Rehabilitasyon ürünü
}

/// <summary>
/// Krampon seviyeleri
/// </summary>
public enum CleatsTier
{
    Basic,      // Temel
    Standard,   // Standart
    Premium,    // Premium
    Elite       // Elit
}

/// <summary>
/// Lüks eşya türleri
/// </summary>
public enum LuxuryType
{
    House,      // Ev
    Car,        // Araba
    Watch,      // Saat
    Jewelry     // Mücevher
}

/// <summary>
/// Menajer tipleri
/// </summary>
public enum ManagerType
{
    Strict,     // Katı
    Supportive, // Destekleyici
    Balanced    // Dengeli
}

/// <summary>
/// Kulüp hedefleri
/// </summary>
public enum ClubObjective
{
    LeaguePosition,         // Lig pozisyonu hedefi
    CupWin,                 // Kupa kazanma hedefi
    ChampionsLeague,        // Şampiyonlar Ligi hedefi
    RelegationAvoid,        // Küme düşme önleme
    FinancialBalance,       // Mali denge
    YouthDevelopment        // Gençlik geliştirme
}

/// <summary>
/// Transfer penceresi türleri
/// </summary>
public enum TransferWindowType
{
    Summer,     // Yaz transfer dönemi
    Winter      // Kış transfer dönemi
}

/// <summary>
/// Transfer teklifi türleri
/// </summary>
public enum TransferOfferType
{
    Permanent,  // Sürekli transfer
    Loan        // Kiralama
}

/// <summary>
/// Kritik olay türleri
/// </summary>
public enum CriticalEventType
{
    TransferOffer,      // Transfer teklifi
    Injury,             // Sakatlık
    ContractRenewal,    // Sözleşme yenileme
    NationalTeamCall,   // Milli takım çağrısı
    ClubObjectiveUpdate,// Kulüp hedefi güncelleme
    ManagerChange,      // Menajer değişikliği
    BriberyAttempt      // Rüşvet girişimi
}

/// <summary>
/// Spiker yorum tetikleyicileri
/// </summary>
public enum CommentTrigger
{
    MatchStart,         // Maç başlangıcı
    HalfTime,           // Devre arası
    SecondHalfStart,    // İkinci yarı başlangıcı
    Goal,               // Gol
    Assist,             // Asist
    Save,               // Kurtarış (kaleci)
    Foul,               // Faul
    YellowCard,         // Sarı kart
    RedCard,            // Kırmızı kart
    Substitution,       // Oyuncu değişikliği
    ChanceMissed,       // Kaçan fırsat
    MatchEnd            // Maç sonu
}

/// <summary>
/// Pozisyon (Chance) türleri - Atak veya Savunma
/// </summary>
public enum ChanceType
{
    Attack,     // Atak pozisyonu
    Defense     // Savunma pozisyonu
}

/// <summary>
/// Oyun akış durumları
/// </summary>
public enum GameFlowState
{
    Idle,               // Beklemede
    WaitingForInput,    // Kullanıcı girişi bekleniyor
    Executing,          // Eylem gerçekleştiriliyor
    BallInFlight,       // Top havada
    BallLoose,          // Top boşta
    Transitioning,      // Geçiş yapılıyor
    Ending,             // Sonlanıyor
    AIPlaying,          // AI oynuyor
    Ended               // Sona erdi
}

/// <summary>
/// Pozisyon sonuçları
/// </summary>
public enum ChanceOutcome
{
    None,           // Henüz sonuç yok
    Goal,           // Gol
    Saved,          // Kurtarıldı
    Missed,         // Kaçırıldı
    Blocked,        // Bloke edildi
    Assist,         // Asist (pas sonucu gol)
    Tackled,        // Top alındı (savunma)
    Cleared,        // Top uzaklaştırıldı
    Intercepted,    // Pas kesildi
    OutOfBounds     // Top dışarı çıktı
}

/// <summary>
/// Çizgi verisi - Pas/Şut/Dribling için
/// </summary>
[System.Serializable]
public class LineData
{
    public UnityEngine.Vector2 startPoint;
    public UnityEngine.Vector2 endPoint;
    public float length;
    public float angle;
    public float drawTime;
    public float curvature;
    public ActionType detectedAction;  // Algılanan aksiyon türü
    public float drawSpeed;            // Çizim hızı
    public UnityEngine.GameObject targetPlayer;  // Hedef oyuncu (pas için)
    public UnityEngine.Vector3 targetPosition;   // Hedef pozisyon
    public System.Collections.Generic.List<UnityEngine.Vector2> points;  // Çizgi noktaları
    
    public LineData()
    {
        startPoint = UnityEngine.Vector2.zero;
        endPoint = UnityEngine.Vector2.zero;
        length = 0f;
        angle = 0f;
        drawTime = 0f;
        curvature = 0f;
        detectedAction = ActionType.None;
        drawSpeed = 0f;
        targetPlayer = null;
        targetPosition = UnityEngine.Vector3.zero;
        points = new System.Collections.Generic.List<UnityEngine.Vector2>();
    }
    
    public LineData(UnityEngine.Vector2 start, UnityEngine.Vector2 end, float time, float curve = 0f)
    {
        startPoint = start;
        endPoint = end;
        length = UnityEngine.Vector2.Distance(start, end);
        angle = UnityEngine.Mathf.Atan2(end.y - start.y, end.x - start.x) * UnityEngine.Mathf.Rad2Deg;
        drawTime = time;
        curvature = curve;
        detectedAction = ActionType.None;
        drawSpeed = time > 0 ? length / time : 0f;
        targetPlayer = null;
        targetPosition = UnityEngine.Vector3.zero;
        points = new System.Collections.Generic.List<UnityEngine.Vector2> { start, end };
    }
    
    /// <summary>
    /// Çizginin yönünü 3D world space'e çevir
    /// </summary>
    public UnityEngine.Vector3 GetWorldDirection()
    {
        UnityEngine.Vector2 dir2D = (endPoint - startPoint).normalized;
        return new UnityEngine.Vector3(dir2D.x, 0f, dir2D.y);
    }
    
    /// <summary>
    /// Güç hesapla (0-1 arası)
    /// </summary>
    public float GetPower(float maxLength = 500f)
    {
        return UnityEngine.Mathf.Clamp01(length / maxLength);
    }
}

/// <summary>
/// Oyuncu aksiyon türleri
/// </summary>
public enum ActionType
{
    None,       // Aksiyon yok
    Move,       // Hareket
    Pass,       // Pas
    Shoot,      // Şut
    Dribble,    // Dribling
    Tackle,     // Top kapma
    Clear       // Topu uzaklaştırma
}

/// <summary>
/// Pozisyon (Chance) başlangıç verileri
/// </summary>
[System.Serializable]
public class ChanceSetupData
{
    public ChanceType chanceType = ChanceType.Attack;
    public UnityEngine.Vector3 playerStartPosition;
    public UnityEngine.Vector3 ballStartPosition;
    public UnityEngine.Vector3 goalPosition;
    public int minute;
    public float successChance;
    public string templateName;
    
    // Oyuncu istatistikleri
    public PlayerPosition playerPosition;
    public int playerOverall;
    public float playerEnergy;
    public float playerForm;
    public int speedStat;
    public int shootingStat;
    public int passingStat;
    public int dribblingStat;      // dribbilingStat yerine
    public int dribbilingStat;     // Eski uyumluluk için
    public int defenseStat;
    public int physicalStat;
    public int physicsStat;        // Eski uyumluluk için
    public int falsoStat;          // Falso yeteneği
    
    // Takım güçleri
    public int homeTeamPower;
    public int awayTeamPower;
    
    // Takım arkadaşları pozisyonları
    public System.Collections.Generic.List<UnityEngine.Vector3> teammatePositions;
    
    // Rakip pozisyonları
    public System.Collections.Generic.List<UnityEngine.Vector3> opponentPositions;
    
    public ChanceSetupData()
    {
        chanceType = ChanceType.Attack;
        playerStartPosition = UnityEngine.Vector3.zero;
        ballStartPosition = UnityEngine.Vector3.zero;
        goalPosition = new UnityEngine.Vector3(0, 0, 30f);
        minute = 0;
        successChance = 0.5f;
        templateName = "Default";
        playerPosition = PlayerPosition.SF;
        playerOverall = 70;
        playerEnergy = 100f;
        playerForm = 70f;
        speedStat = 70;
        shootingStat = 70;
        passingStat = 70;
        dribblingStat = 70;
        dribbilingStat = 70;
        defenseStat = 70;
        physicalStat = 70;
        physicsStat = 70;
        falsoStat = 70;
        homeTeamPower = 70;
        awayTeamPower = 70;
        teammatePositions = new System.Collections.Generic.List<UnityEngine.Vector3>();
        opponentPositions = new System.Collections.Generic.List<UnityEngine.Vector3>();
    }
}
