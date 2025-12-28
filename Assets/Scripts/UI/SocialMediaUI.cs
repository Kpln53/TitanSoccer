using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Sosyal medya UI - Sosyal medya paneli
/// </summary>
public class SocialMediaUI : MonoBehaviour
{
    [Header("Post Listesi")]
    public Transform postListParent;
    public GameObject postItemPrefab;

    [Header("Yeni Post Oluştur")]
    public GameObject createPostPanel;
    public TMP_InputField postContentInput;
    public Button createPostButton;
    public Button cancelCreatePostButton;

    [Header("Post Detay Paneli")]
    public GameObject detailPanel;
    public TextMeshProUGUI detailAuthorText;
    public TextMeshProUGUI detailContentText;
    public TextMeshProUGUI detailDateText;
    public TextMeshProUGUI detailLikesText;
    public Button likeButton;
    public Button closeDetailButton;
    public Transform commentsParent;
    public GameObject commentItemPrefab;

    [Header("Takipçi Sayısı")]
    public TextMeshProUGUI followersText;

    private List<SocialMediaPost> currentPosts;
    private SocialMediaPost selectedPost;

    private void OnEnable()
    {
        LoadPosts();
        RefreshFollowers();
        if (createPostPanel != null)
            createPostPanel.SetActive(false);
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    /// <summary>
    /// Postları yükle ve göster
    /// </summary>
    private void LoadPosts()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[SocialMediaUI] No current save!");
            return;
        }

        MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null)
        {
            Debug.LogWarning("[SocialMediaUI] No media data!");
            return;
        }

        if (SocialMediaSystem.Instance != null)
        {
            currentPosts = SocialMediaSystem.Instance.GetRecentPosts(mediaData, 20);
        }
        else
        {
            currentPosts = mediaData.socialMediaPosts != null ? mediaData.socialMediaPosts.Take(20).ToList() : new List<SocialMediaPost>();
        }

        DisplayPosts();
        SetupCreatePostButtons();
    }

    /// <summary>
    /// Postları göster
    /// </summary>
    private void DisplayPosts()
    {
        if (postListParent == null)
        {
            Debug.LogWarning("[SocialMediaUI] Post list parent not found!");
            return;
        }

        // Mevcut item'ları temizle
        foreach (Transform child in postListParent)
        {
            Destroy(child.gameObject);
        }

        // Her post için item oluştur
        foreach (var post in currentPosts)
        {
            CreatePostItem(post);
        }
    }

    /// <summary>
    /// Post item'ı oluştur
    /// </summary>
    private void CreatePostItem(SocialMediaPost post)
    {
        GameObject itemObj;

        if (postItemPrefab != null)
        {
            itemObj = Instantiate(postItemPrefab, postListParent);
        }
        else
        {
            // Prefab yoksa runtime'da oluştur
            itemObj = new GameObject($"PostItem_{post.authorName}");
            itemObj.transform.SetParent(postListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 150);

            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

            Button button = itemObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnPostItemClicked(post));

            // Post içeriği
            GameObject contentObj = new GameObject("PostContent");
            contentObj.transform.SetParent(itemObj.transform);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0.3f);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(10, 5);
            contentRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI contentText = contentObj.AddComponent<TextMeshProUGUI>();
            contentText.text = post.content;
            contentText.fontSize = 16;
            contentText.color = Color.white;
            contentText.alignment = TextAlignmentOptions.TopLeft;

            // Post yazarı ve tarihi
            GameObject infoObj = new GameObject("PostInfo");
            infoObj.transform.SetParent(itemObj.transform);
            RectTransform infoRect = infoObj.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0, 0);
            infoRect.anchorMax = new Vector2(1, 0.3f);
            infoRect.offsetMin = new Vector2(10, 5);
            infoRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI infoText = infoObj.AddComponent<TextMeshProUGUI>();
            infoText.text = $"{post.author} - {post.dateString} - ❤️ {post.likes}";
            infoText.fontSize = 14;
            infoText.color = Color.gray;
            infoText.alignment = TextAlignmentOptions.BottomLeft;
        }

        // Prefab varsa bile tıklama event'i ekle
        Button itemButton = itemObj.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnPostItemClicked(post));
        }
    }

    private void SetupCreatePostButtons()
    {
        if (createPostButton != null)
            createPostButton.onClick.AddListener(OnCreatePostButton);

        if (cancelCreatePostButton != null)
            cancelCreatePostButton.onClick.AddListener(OnCancelCreatePostButton);
    }

    /// <summary>
    /// Post item'ına tıklandığında
    /// </summary>
    private void OnPostItemClicked(SocialMediaPost post)
    {
        selectedPost = post;
        ShowPostDetail(post);
    }

    /// <summary>
    /// Post detayını göster
    /// </summary>
    private void ShowPostDetail(SocialMediaPost post)
    {
        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (detailAuthorText != null)
            detailAuthorText.text = post.author;

        if (detailContentText != null)
            detailContentText.text = post.content;

        if (detailDateText != null)
            detailDateText.text = post.dateString;

        if (detailLikesText != null)
            detailLikesText.text = $"❤️ {post.likes} Beğeni";

        if (likeButton != null)
        {
            likeButton.onClick.RemoveAllListeners();
            likeButton.onClick.AddListener(() => OnLikeButton(post));
            // Buton görünümünü güncelle
            TextMeshProUGUI likeButtonText = likeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (likeButtonText != null)
            {
                likeButtonText.text = post.isLikedByPlayer ? "❤️ Beğenmekten Vazgeç" : "🤍 Beğen";
            }
        }

        if (closeDetailButton != null)
        {
            closeDetailButton.onClick.RemoveAllListeners();
            closeDetailButton.onClick.AddListener(OnCloseDetailButton);
        }

        // Yorumları göster
        DisplayComments(post);
    }

    private void DisplayComments(SocialMediaPost post)
    {
        if (commentsParent == null || post.commentList == null) return;

        // Mevcut yorumları temizle
        foreach (Transform child in commentsParent)
        {
            Destroy(child.gameObject);
        }

        // Her yorum için item oluştur
        foreach (var comment in post.commentList)
        {
            CreateCommentItem(comment);
        }
    }

    private void CreateCommentItem(string comment)
    {
        GameObject itemObj;

        if (commentItemPrefab != null)
        {
            itemObj = Instantiate(commentItemPrefab, commentsParent);
        }
        else
        {
            itemObj = new GameObject("CommentItem");
            itemObj.transform.SetParent(commentsParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(750, 50);

            TextMeshProUGUI commentText = itemObj.AddComponent<TextMeshProUGUI>();
            commentText.text = comment;
            commentText.fontSize = 14;
            commentText.color = Color.white;
        }

        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = comment;
        }
    }

    private void OnLikeButton(SocialMediaPost post)
    {
        if (post == null || GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (SocialMediaSystem.Instance != null)
        {
            SocialMediaSystem.Instance.LikePost(mediaData, post);
            RefreshFollowers();
        }
        else
        {
            post.isLikedByPlayer = !post.isLikedByPlayer;
            if (post.isLikedByPlayer) post.likes++;
            else post.likes = Mathf.Max(0, post.likes - 1);
        }

        ShowPostDetail(post); // Detayı yenile
    }

    private void RefreshFollowers()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (followersText != null && mediaData != null)
        {
            followersText.text = $"Takipçiler: {mediaData.socialMediaFollowers:N0}";
        }
    }

    private void OnCreatePostButton()
    {
        if (postContentInput == null || string.IsNullOrEmpty(postContentInput.text)) return;

        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
        if (mediaData == null) return;

        string playerName = GameManager.Instance.CurrentSave.playerProfile != null 
            ? GameManager.Instance.CurrentSave.playerProfile.playerName 
            : "Player";

        if (SocialMediaSystem.Instance != null)
        {
            SocialMediaSystem.Instance.CreatePost(mediaData, postContentInput.text, SocialMediaPostType.Normal);
        }
        else
        {
            // Manuel olarak post oluştur
            SocialMediaPost newPost = new SocialMediaPost
            {
                content = postContentInput.text,
                author = playerName,
                type = SocialMediaPostType.Normal
            };
            mediaData.AddPost(newPost);
        }

        postContentInput.text = "";
        if (createPostPanel != null)
            createPostPanel.SetActive(false);

        LoadPosts();
        RefreshFollowers();
    }

    private void OnCancelCreatePostButton()
    {
        if (createPostPanel != null)
            createPostPanel.SetActive(false);

        if (postContentInput != null)
            postContentInput.text = "";
    }

    private void OnCloseDetailButton()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);

        selectedPost = null;
    }
}

