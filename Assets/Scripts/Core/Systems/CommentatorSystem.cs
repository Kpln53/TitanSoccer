using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spiker/Spiker sistemi - Maç yorumları ve açıklamalar (Singleton)
/// </summary>
public class CommentatorSystem : MonoBehaviour
{
    public static CommentatorSystem Instance { get; private set; }

    [Header("Yorum Şablonları")]
    private Dictionary<CommentTrigger, List<string>> commentTemplates;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeTemplates();
        Debug.Log("[CommentatorSystem] CommentatorSystem initialized.");
    }

    /// <summary>
    /// Şablonları başlat
    /// </summary>
    private void InitializeTemplates()
    {
        commentTemplates = new Dictionary<CommentTrigger, List<string>>();

        // Maç başlangıcı
        commentTemplates[CommentTrigger.MatchStart] = new List<string>
        {
            "Maç başladı! Her iki takım da hırslı görünüyor.",
            "Karşılaşma başladı. İyi bir maç göreceğimizi umuyoruz.",
            "Ve maç başladı! Oyuncular sahada pozisyonlarını aldılar."
        };

        // Devre arası
        commentTemplates[CommentTrigger.HalfTime] = new List<string>
        {
            "İlk yarı sona erdi. İkinci yarıda neler olacak merak ediyoruz.",
            "İlk 45 dakika bitti. Her iki takım da iyi oynadı.",
            "Devre arası. Teknik direktörler oyuncularına talimat veriyor."
        };

        // İkinci yarı başlangıcı
        commentTemplates[CommentTrigger.SecondHalfStart] = new List<string>
        {
            "İkinci yarı başladı! Takımlar tekrar sahada.",
            "Ve ikinci yarı başladı. Maç daha da kızışacak gibi görünüyor.",
            "İkinci yarıda oyuncular daha agresif görünüyorlar."
        };

        // Gol
        commentTemplates[CommentTrigger.Goal] = new List<string>
        {
            "GOOOL! Muhteşem bir gol!",
            "Gol! Top ağlarla buluştu!",
            "Ve gol! Taraftarlar ayakta!",
            "Harika bir gol! Toplam skor güncellendi."
        };

        // Asist
        commentTemplates[CommentTrigger.Assist] = new List<string>
        {
            "Muhteşem bir pas! Gol fırsatı yaratıldı.",
            "Harika bir orta! Gol geliyor!",
            "Çok güzel bir asist! Oyuncu pozisyona girdi."
        };

        // Kurtarış (kaleci)
        commentTemplates[CommentTrigger.Save] = new List<string>
        {
            "Kaleci muhteşem bir kurtarış yaptı!",
            "Topu kurtardı! Kaleci harika bir refleks gösterdi.",
            "Mükemmel bir kurtarış! Gol olmadı."
        };

        // Faul
        commentTemplates[CommentTrigger.Foul] = new List<string>
        {
            "Faul! Hakem düdük çaldı.",
            "Faul yapıldı. Serbest vuruş verildi.",
            "Faul var! Takım avantaj sağladı."
        };

        // Kaçan fırsat
        commentTemplates[CommentTrigger.ChanceMissed] = new List<string>
        {
            "Fırsat kaçtı! Top kaleyi ıskaladı.",
            "Kaçan bir fırsat! Gol olabilirdi.",
            "Top kalenin dışına gitti. Keşke gol olsaydı!"
        };

        // Maç sonu
        commentTemplates[CommentTrigger.MatchEnd] = new List<string>
        {
            "Maç sona erdi! Son düdük çaldı.",
            "Ve maç bitti. Her iki takım da mücadele etti.",
            "Karşılaşma sona erdi. Skor final haliyle böyle kaldı."
        };
    }

    /// <summary>
    /// Yorum oluştur (trigger'a göre)
    /// </summary>
    public string GenerateCommentary(CommentTrigger trigger, string playerName = "", string teamName = "")
    {
        if (commentTemplates == null || !commentTemplates.ContainsKey(trigger))
        {
            return "Maç devam ediyor...";
        }

        List<string> templates = commentTemplates[trigger];
        if (templates == null || templates.Count == 0)
        {
            return "Maç devam ediyor...";
        }

        // Rastgele bir şablon seç
        string template = templates[Random.Range(0, templates.Count)];

        // Oyuncu/takım adı varsa yerleştir
        if (!string.IsNullOrEmpty(playerName))
        {
            template = template.Replace("{player}", playerName);
        }
        if (!string.IsNullOrEmpty(teamName))
        {
            template = template.Replace("{team}", teamName);
        }

        return template;
    }

    /// <summary>
    /// Özel yorum oluştur (şablon + parametreler)
    /// </summary>
    public string GenerateCustomCommentary(string template, Dictionary<string, string> parameters)
    {
        string commentary = template;

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                commentary = commentary.Replace("{" + param.Key + "}", param.Value);
            }
        }

        return commentary;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}








