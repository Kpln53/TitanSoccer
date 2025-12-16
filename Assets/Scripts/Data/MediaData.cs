/// <summary>
/// Medya verileri
/// </summary>
[System.Serializable]
public class MediaData
{
    [Header("Popülerlik")]
    [Range(0f, 1f)] public float popularity = 0.1f; // 0-1 arası
    
    [Header("Basın Puanı")]
    [Range(0, 100)] public int pressScore = 50;
    
    [Header("Sosyal Medya")]
    public int socialMediaFollowers = 0;
    
    [Header("Son Haberler")]
    public List<NewsItem> recentNews = new List<NewsItem>();
}

/// <summary>
/// Haber öğesi
/// </summary>
[System.Serializable]
public class NewsItem
{
    public string title;
    public string content;
    public NewsType type;
    public string date;
    public bool isRead = false;
}

public enum NewsType
{
    MatchPerformance,
    TransferRumor,
    Discipline,
    Achievement,
    Relationship,
    Scandal
}

