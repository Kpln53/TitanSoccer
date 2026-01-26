using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Haber template sistemi - Dinamik haber içeriği üretimi
/// </summary>
[System.Serializable]
public class NewsTemplate
{
    public NewsType type;
    public string titleTemplate;
    public string contentTemplate;
    public string[] sources;
    
    public NewsTemplate(NewsType newsType, string title, string content, params string[] newsSources)
    {
        type = newsType;
        titleTemplate = title;
        contentTemplate = content;
        sources = newsSources;
    }
}

/// <summary>
/// Haber template yöneticisi
/// </summary>
public static class NewsTemplateManager
{
    private static Dictionary<NewsType, List<NewsTemplate>> templates;
    
    static NewsTemplateManager()
    {
        InitializeTemplates();
    }
    
    private static void InitializeTemplates()
    {
        templates = new Dictionary<NewsType, List<NewsTemplate>>();
        
        // --- MAÇ HABERLERİ (GENEL) ---
        AddTemplate(NewsType.Match, 
            "⚽ {playerName} {goals} Golle Parladı!", 
            "{playerName}, {teamName} formasıyla {opponentTeam} karşısında {goals} gol atarak takımını {score} galibiyete taşıdı. Maç sonrası verdiği demeçte: 'Takım için elimden geleni yaptım' dedi.",
            "Spor Gazetesi", "Futbol Haberleri", "Maç Raporu");

        // --- GALİBİYET HABERLERİ ---
        AddTemplate(NewsType.MatchWin,
            "🏆 {teamName} {score} Kazandı!",
            "{teamName}, {opponentTeam} ile oynadığı zorlu maçı {score} kazanmayı başardı. {playerName}'in performansı maçın kaderini belirledi.",
            "Lig Haberleri", "Spor Merkezi");

        AddTemplate(NewsType.MatchWin,
            "🔥 {teamName} Durdurulamıyor!",
            "{teamName}, {opponentTeam} karşısında {score} gibi net bir skorla galip geldi. Taraftarlar maç sonu takımı ayakta alkışladı.",
            "Fanatik", "Tribün Sesi");

        // --- MAĞLUBİYET HABERLERİ ---
        AddTemplate(NewsType.MatchLoss,
            "😞 {teamName} {score} Mağlup Oldu",
            "{teamName}, {opponentTeam} karşısında {score} mağlup oldu. Takım bu sonuçla lig tablosunda puan kaybetti. Teknik direktör: 'Daha sıkı çalışacağız' açıklamasında bulundu.",
            "Spor Gazetesi", "Futbol Analiz");

        AddTemplate(NewsType.MatchLoss,
            "📉 {teamName} İçin Kötü Gece",
            "{opponentTeam} deplasmanında {score} kaybeden {teamName}, sahadan üzgün ayrıldı. {playerName} maç sonu taraftarlardan özür diledi.",
            "Maç Sonu", "Spor Manşet");

        // --- BERABERLİK HABERLERİ ---
        AddTemplate(NewsType.MatchDraw,
            "🤝 {teamName} {score} Berabere Kaldı",
            "{teamName} ile {opponentTeam} arasındaki mücadele {score} sona erdi. İki takım da sahadan birer puanla ayrıldı.",
            "Lig Özeti", "Puan Durumu");

        AddTemplate(NewsType.MatchDraw,
            "⚖️ Puanlar Paylaşıldı: {score}",
            "{teamName}, {opponentTeam} karşısında öne geçmesine rağmen skoru koruyamadı ve maç {score} bitti.",
            "Maç Analizi", "Spor Gündemi");
            
        // Transfer Haberleri
        AddTemplate(NewsType.Transfer,
            "💰 {playerName} {newTeam}'a Transfer Oldu!",
            "{playerName}, {oldTeam}'dan ayrılarak {newTeam} ile {years} yıllık sözleşme imzaladı. Transfer bedeli {amount} milyon euro olarak açıklandı.",
            "Transfer Merkezi", "Futbol Piyasası", "Spor Ekonomisi");
            
        AddTemplate(NewsType.Transfer,
            "🔄 Sürpriz Transfer: {playerName}!",
            "Son dakika gelişmesi! {playerName}, {newTeam} formasını giyecek. {position} pozisyonunu güçlendiren kulüp, bu transferle büyük hedeflere odaklandı.",
            "Transfer Haberleri", "Son Dakika Spor");
            
        // Sakatlık Haberleri
        AddTemplate(NewsType.Injury,
            "🏥 {playerName} Sakatlandı",
            "{playerName}, {matchType} sırasında yaralandı. Yapılan muayenede {injuryType} tespit edildi. Oyuncunun {weeks} hafta sahalardan uzak kalması bekleniyor.",
            "Sağlık Raporu", "Tıbbi Bulletin", "Kulüp Doktoru");
            
        AddTemplate(NewsType.Injury,
            "💊 {playerName} İyileşme Sürecinde",
            "{playerName}'in sakatlığında son durum açıklandı. Oyuncu fizik tedavi sürecini başarıyla tamamlıyor ve {weeks} hafta sonra sahalara dönmesi bekleniyor.",
            "Sağlık Merkezi", "Rehabilitasyon Raporu");
            
        // Performans Haberleri
        AddTemplate(NewsType.Performance,
            "📊 {playerName} İstatistikleri Etkileyici",
            "{playerName}, bu sezon {matches} maçta {goals} gol ve {assists} asist kaydetti. Ortalama {rating} rating ile takımın en istikrarlı oyuncuları arasında yer alıyor.",
            "İstatistik Merkezi", "Performans Analizi", "Futbol Verileri");
            
        AddTemplate(NewsType.Performance,
            "🏅 {playerName} Ayın Oyuncusu Seçildi",
            "{playerName}, {month} ayında gösterdiği performansla ayın oyuncusu seçildi. {goals} gol ve {assists} asist ile takımının en değerli ismi oldu.",
            "Lig Organizasyonu", "Ödül Töreni");
            
        // Lig Haberleri
        AddTemplate(NewsType.League,
            "🏆 {teamName} Liderliğini Sürdürüyor",
            "{teamName}, {week}. hafta sonunda {points} puanla liderliğini koruyor. En yakın takipçisi {rivalTeam} {rivalPoints} puanla 2. sırada bulunuyor.",
            "Lig Tablosu", "Puan Durumu", "Şampiyonluk Yarışı");
            
        AddTemplate(NewsType.League,
            "📈 Lig Tablosunda Hareketlilik",
            "{week}. hafta maçları sonrası lig tablosunda önemli değişiklikler yaşandı. {teamName} {position}. sıraya {direction} ve şampiyonluk yarışında önemli bir adım attı.",
            "Lig Analizi", "Tablo Durumu");
            
        // Sözleşme Haberleri
        AddTemplate(NewsType.Contract,
            "✍️ {playerName} Sözleşme Yeniledi",
            "{playerName}, {teamName} ile {years} yıl daha devam etme kararı aldı. Yeni sözleşmesiyle {salary} euro maaş alacak olan oyuncu: 'Bu kulüpte mutluyum' dedi.",
            "Kulüp Resmi", "Sözleşme Haberleri", "Transfer Merkezi");
            
        // Takım Yönetimi Haberleri
        AddTemplate(NewsType.TeamManagement,
            "🗣️ Teknik Direktör Açıklaması",
            "Teknik direktör {coachName}, basın toplantısında önemli açıklamalarda bulundu: '{statement}' Takımın hedefleri ve stratejisi hakkında detaylar verdi.",
            "Basın Toplantısı", "Teknik Direktör", "Kulüp Açıklaması");
            
        // Başarı Haberleri
        AddTemplate(NewsType.Achievement,
            "🏅 {playerName} Rekor Kırdı!",
            "{playerName}, {achievement} ile yeni bir rekora imza attı. Bu başarı, oyuncunun kariyerindeki en önemli kilometre taşlarından biri olarak kayıtlara geçti.",
            "Rekor Kitabı", "Başarı Hikayeleri", "Futbol Tarihi");
            
        // Söylenti Haberleri
        AddTemplate(NewsType.Rumour,
            "👂 Transfer Söylentisi: {playerName}",
            "Avrupa basınından gelen haberlere göre {playerName} için {interestedTeam} {amount} milyon euroluk teklif hazırlıyor. Kulüp yönetimi henüz resmi açıklama yapmadı.",
            "Transfer Söylentileri", "Avrupa Basını", "Kulüp Kaynakları");
    }
    
    private static void AddTemplate(NewsType type, string title, string content, params string[] sources)
    {
        if (!templates.ContainsKey(type))
        {
            templates[type] = new List<NewsTemplate>();
        }
        
        templates[type].Add(new NewsTemplate(type, title, content, sources));
    }
    
    /// <summary>
    /// Belirli türde rastgele template getir
    /// </summary>
    public static NewsTemplate GetRandomTemplate(NewsType type)
    {
        if (!templates.ContainsKey(type) || templates[type].Count == 0)
        {
            // Fallback: Eğer özel tip yoksa (örn MatchWin) ve Match varsa, Match'ten döndür
            if (type == NewsType.MatchWin || type == NewsType.MatchLoss || type == NewsType.MatchDraw)
            {
                if (templates.ContainsKey(NewsType.Match) && templates[NewsType.Match].Count > 0)
                    return templates[NewsType.Match][Random.Range(0, templates[NewsType.Match].Count)];
            }
            
            return GetDefaultTemplate(type);
        }
        
        var typeTemplates = templates[type];
        return typeTemplates[Random.Range(0, typeTemplates.Count)];
    }
    
    /// <summary>
    /// Varsayılan template getir
    /// </summary>
    private static NewsTemplate GetDefaultTemplate(NewsType type)
    {
        return new NewsTemplate(type, 
            $"{GetTypeIcon(type)} Yeni {GetTypeName(type)} Haberi",
            "Bu konuda yeni gelişmeler yaşanıyor. Detaylar yakında açıklanacak.",
            "Genel Haberler");
    }
    
    /// <summary>
    /// Haber türü ikonu getir
    /// </summary>
    public static string GetTypeIcon(NewsType type)
    {
        return type switch
        {
            NewsType.Match => "⚽",
            NewsType.MatchWin => "🏆",
            NewsType.MatchLoss => "😞",
            NewsType.MatchDraw => "🤝",
            NewsType.Transfer => "💰",
            NewsType.Injury => "🏥",
            NewsType.Performance => "📊",
            NewsType.League => "🏆",
            NewsType.Contract => "✍️",
            NewsType.TeamManagement => "🗣️",
            NewsType.Achievement => "🏅",
            NewsType.Rumour => "👂",
            _ => "📰"
        };
    }
    
    /// <summary>
    /// Haber türü adı getir
    /// </summary>
    public static string GetTypeName(NewsType type)
    {
        return type switch
        {
            NewsType.Match => "Maç",
            NewsType.MatchWin => "Galibiyet",
            NewsType.MatchLoss => "Mağlubiyet",
            NewsType.MatchDraw => "Beraberlik",
            NewsType.Transfer => "Transfer",
            NewsType.Injury => "Sakatlık",
            NewsType.Performance => "Performans",
            NewsType.League => "Lig",
            NewsType.Contract => "Sözleşme",
            NewsType.TeamManagement => "Yönetim",
            NewsType.Achievement => "Başarı",
            NewsType.Rumour => "Söylenti",
            _ => "Genel"
        };
    }
    
    /// <summary>
    /// Template'deki placeholder'ları değiştir
    /// </summary>
    public static string ReplacePlaceholders(string template, Dictionary<string, string> values)
    {
        string result = template;
        
        foreach (var kvp in values)
        {
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value);
        }
        
        return result;
    }
}
