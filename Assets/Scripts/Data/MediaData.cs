using System.Collections.Generic;

/// <summary>
/// Medya verisi - Haberler ve sosyal medya
/// </summary>
[Serializable]
public class MediaData
{
    [Header("Haberler")]
    public List<NewsItem> recentNews;      // Son haberler
    
    [Header("Sosyal Medya")]
    public List<SocialMediaPost> socialMediaPosts; // Sosyal medya postları
    public int socialMediaFollowers = 0;   // Takipçi sayısı
    
    [Header("Ayarlar")]
    public int maxNewsCount = 50;          // Maksimum saklanacak haber sayısı
    public int maxPostCount = 100;         // Maksimum saklanacak post sayısı
    
    public MediaData()
    {
        recentNews = new List<NewsItem>();
        socialMediaPosts = new List<SocialMediaPost>();
        socialMediaFollowers = 1000; // Başlangıç takipçi sayısı
    }
    
    /// <summary>
    /// Haber ekle (maksimum sayıyı kontrol et)
    /// </summary>
    public void AddNews(NewsItem news)
    {
        if (recentNews == null)
        {
            recentNews = new List<NewsItem>();
        }
        
        recentNews.Add(news);
        
        // Eski haberleri sil (maksimum sayıyı aşmamak için)
        if (recentNews.Count > maxNewsCount)
        {
            recentNews.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Post ekle (maksimum sayıyı kontrol et)
    /// </summary>
    public void AddPost(SocialMediaPost post)
    {
        if (socialMediaPosts == null)
        {
            socialMediaPosts = new List<SocialMediaPost>();
        }
        
        socialMediaPosts.Add(post);
        
        // Eski postları sil (maksimum sayıyı aşmamak için)
        if (socialMediaPosts.Count > maxPostCount)
        {
            socialMediaPosts.RemoveAt(0);
        }
    }
}

