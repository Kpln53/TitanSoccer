using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sosyal medya sistemi - Post üretimi ve takipçi yönetimi (Singleton)
/// </summary>
public class SocialMediaSystem : MonoBehaviour
{
    public static SocialMediaSystem Instance { get; private set; }

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

        Debug.Log("[SocialMediaSystem] SocialMediaSystem initialized.");
    }

    /// <summary>
    /// Yeni post oluştur
    /// </summary>
    public SocialMediaPost CreatePost(MediaData mediaData, string content, SocialMediaPostType type = SocialMediaPostType.Normal)
    {
        if (mediaData == null)
        {
            Debug.LogWarning("[SocialMediaSystem] MediaData is null! Cannot create post.");
            return null;
        }

        if (string.IsNullOrEmpty(content))
        {
            Debug.LogWarning("[SocialMediaSystem] Post content is empty! Cannot create post.");
            return null;
        }

        SocialMediaPost post = new SocialMediaPost
        {
            content = content,
            author = GetPlayerSocialMediaHandle(),
            type = type,
            date = System.DateTime.Now,
            dateString = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        mediaData.AddPost(post);
        Debug.Log($"[SocialMediaSystem] Post created: {content.Substring(0, Mathf.Min(30, content.Length))}...");

        return post;
    }

    /// <summary>
    /// Post'u beğen
    /// </summary>
    public void LikePost(MediaData mediaData, SocialMediaPost post)
    {
        if (mediaData == null || post == null)
            return;

        if (!post.isLikedByPlayer)
        {
            post.likes++;
            post.isLikedByPlayer = true;
            Debug.Log("[SocialMediaSystem] Post liked.");
        }
        else
        {
            // Beğeniyi geri al
            post.likes = Mathf.Max(0, post.likes - 1);
            post.isLikedByPlayer = false;
            Debug.Log("[SocialMediaSystem] Post unliked.");
        }
    }

    /// <summary>
    /// Post'a yorum ekle
    /// </summary>
    public void CommentOnPost(MediaData mediaData, SocialMediaPost post, string comment)
    {
        if (mediaData == null || post == null || string.IsNullOrEmpty(comment))
            return;

        if (post.commentList == null)
        {
            post.commentList = new List<string>();
        }

        post.commentList.Add(comment);
        post.comments++;
        Debug.Log($"[SocialMediaSystem] Comment added to post.");
    }

    /// <summary>
    /// Son postları getir
    /// </summary>
    public List<SocialMediaPost> GetRecentPosts(MediaData mediaData, int count = 20)
    {
        if (mediaData == null || mediaData.socialMediaPosts == null)
            return new List<SocialMediaPost>();

        // En yeni postlardan belirli sayıda getir
        var posts = mediaData.socialMediaPosts;
        if (posts.Count == 0)
            return new List<SocialMediaPost>();

        int startIndex = Mathf.Max(0, posts.Count - count);
        int takeCount = Mathf.Min(count, posts.Count);
        
        return posts.GetRange(startIndex, takeCount);
    }

    /// <summary>
    /// Takipçi sayısını artır
    /// </summary>
    public void IncreaseFollowers(MediaData mediaData, int amount)
    {
        if (mediaData == null)
            return;

        mediaData.socialMediaFollowers += amount;
        if (mediaData.socialMediaFollowers < 0)
            mediaData.socialMediaFollowers = 0;

        Debug.Log($"[SocialMediaSystem] Followers: {mediaData.socialMediaFollowers}");
    }

    /// <summary>
    /// Oyuncunun sosyal medya handle'ını getir (@OyuncuAdı)
    /// </summary>
    private string GetPlayerSocialMediaHandle()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null && 
            GameManager.Instance.CurrentSave.playerProfile != null)
        {
            return "@" + GameManager.Instance.CurrentSave.playerProfile.playerName.Replace(" ", "");
        }
        return "@Player";
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}






