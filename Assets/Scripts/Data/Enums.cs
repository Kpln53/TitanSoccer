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
    Loyalty             // Sadakat bonusu
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
    Match,              // Maç haberleri
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
/// Sosyal medya post türleri
/// </summary>
public enum SocialMediaPostType
{
    Normal,         // Normal post
    Match,          // Maç ile ilgili post
    Transfer,       // Transfer ile ilgili post
    Achievement,    // Başarı ile ilgili post
    Personal        // Kişisel post
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
