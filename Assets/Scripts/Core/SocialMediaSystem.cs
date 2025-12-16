using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sosyal medya sistemi
/// </summary>
public class SocialMediaSystem : MonoBehaviour
{
    public static SocialMediaSystem Instance;

    [Header("Sosyal Medya Ayarları")]
    [SerializeField] private int baseFollowers = 100; // Başlangıç takipçi sayısı
    [SerializeField] private float followerGrowthRate = 1.1f; // Takipçi artış oranı

    private List<SocialMediaPost> recentPosts = new List<SocialMediaPost>();

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
    /// Post oluştur
    /// </summary>
    public void CreatePost(PlayerProfile profile, PostType type, string customContent = "")
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        SocialMediaPost post = new SocialMediaPost();
        post.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        post.type = type;
        
        switch (type)
        {
            case PostType.MatchPerformance:
                post.content = customContent != "" ? customContent : "Son maçta iyi oynadım! 💪";
                break;
            case PostType.Transfer:
                post.content = customContent != "" ? customContent : "Yeni bir başlangıç! ⚽";
                break;
            case PostType.Achievement:
                post.content = customContent != "" ? customContent : "Hedefime ulaştım! 🏆";
                break;
            case PostType.Personal:
                post.content = customContent;
                break;
        }
        
        post.likes = CalculateInitialLikes(profile);
        post.comments = Random.Range(5, 50);
        
        recentPosts.Insert(0, post);
        
        // Takipçi artışı
        UpdateFollowers(profile, type);
    }

    /// <summary>
    /// İlk beğeni sayısını hesapla
    /// </summary>
    private int CalculateInitialLikes(PlayerProfile profile)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return 0;
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        int baseLikes = Mathf.RoundToInt(mediaData.socialMediaFollowers * 0.1f); // Takipçinin %10'u beğenir
        
        // Popülerlik ve form etkisi
        float multiplier = 1f + mediaData.popularity + profile.form;
        baseLikes = Mathf.RoundToInt(baseLikes * multiplier);
        
        return Mathf.Max(10, baseLikes); // Minimum 10 beğeni
    }

    /// <summary>
    /// Takipçi güncelle
    /// </summary>
    private void UpdateFollowers(PlayerProfile profile, PostType postType)
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;
        
        var mediaData = GameManager.Instance.CurrentSave.mediaData;
        
        // Post tipine göre takipçi artışı
        int followerGain = 0;
        
        switch (postType)
        {
            case PostType.MatchPerformance:
                followerGain = Random.Range(10, 50);
                break;
            case PostType.Transfer:
                followerGain = Random.Range(50, 200);
                break;
            case PostType.Achievement:
                followerGain = Random.Range(30, 100);
                break;
            case PostType.Personal:
                followerGain = Random.Range(5, 30);
                break;
        }
        
        // Popülerlik etkisi
        followerGain = Mathf.RoundToInt(followerGain * (1f + mediaData.popularity));
        
        mediaData.socialMediaFollowers += followerGain;
        
        // Popülerlik güncelle (takipçi sayısına göre)
        UpdatePopularity(mediaData);
    }

    /// <summary>
    /// Popülerliği güncelle
    /// </summary>
    private void UpdatePopularity(MediaData mediaData)
    {
        // Takipçi sayısına göre popülerlik
        if (mediaData.socialMediaFollowers < 1000)
        {
            mediaData.popularity = 0.1f;
        }
        else if (mediaData.socialMediaFollowers < 10000)
        {
            mediaData.popularity = 0.3f;
        }
        else if (mediaData.socialMediaFollowers < 100000)
        {
            mediaData.popularity = 0.6f;
        }
        else
        {
            mediaData.popularity = 1f;
        }
    }

    /// <summary>
    /// Son postları al
    /// </summary>
    public List<SocialMediaPost> GetRecentPosts(int count = 10)
    {
        int takeCount = Mathf.Min(count, recentPosts.Count);
        return recentPosts.GetRange(0, takeCount);
    }

    /// <summary>
    /// Maç sonrası otomatik post
    /// </summary>
    public void AutoPostAfterMatch(PlayerProfile profile, float matchRating)
    {
        PostType postType;
        string content;
        
        if (matchRating >= 8.0f)
        {
            postType = PostType.Achievement;
            content = "Harika bir maçtı! ⚽🔥";
        }
        else if (matchRating >= 7.0f)
        {
            postType = PostType.MatchPerformance;
            content = "İyi bir performans sergiledim! 💪";
        }
        else if (matchRating < 5.0f)
        {
            postType = PostType.MatchPerformance;
            content = "Daha iyi olmam gerekiyor. Çalışmaya devam! 💪";
        }
        else
        {
            postType = PostType.MatchPerformance;
            content = "Maç bitti. Bir sonraki maça hazırım! ⚽";
        }
        
        CreatePost(profile, postType, content);
    }
}

/// <summary>
/// Sosyal medya postu
/// </summary>
[System.Serializable]
public class SocialMediaPost
{
    public string date;
    public string content;
    public PostType type;
    public int likes;
    public int comments;
}

public enum PostType
{
    MatchPerformance,
    Transfer,
    Achievement,
    Personal
}

