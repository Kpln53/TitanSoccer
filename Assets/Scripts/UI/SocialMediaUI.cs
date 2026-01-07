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

    [Header("Yeni Post Oluştur (Seçenekli)")]
    public Button newPostButton; // Yeni post panelini açan buton
    public GameObject createPostPanel;
    
    // 3 Seçenek için butonlar ve textler
    public Button[] optionButtons;       // 0: Övgü, 1: Eleştiri, 2: Motivasyon
    public TextMeshProUGUI[] optionTexts; 
    public TextMeshProUGUI lastMatchInfoText; // "Son Maç: 2-1, Rating: 7.5"
    
    public Button cancelCreatePostButton;

    private List<SocialMediaPost> currentPostOptions; // O anki seçenekler

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
        SetupButtons();
        LoadPosts();
        RefreshFollowers();
        if (createPostPanel != null)
            createPostPanel.SetActive(false);
        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        // New Post Button - Yeni post panelini açar
        if (newPostButton != null)
        {
            newPostButton.onClick.RemoveAllListeners();
            newPostButton.onClick.AddListener(OnNewPostButton);
        }

        // Option Buttons (0, 1, 2)
        if (optionButtons != null)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i; // Closure capture fix
                if (optionButtons[i] != null)
                {
                    optionButtons[i].onClick.RemoveAllListeners();
                    optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
                }
            }
        }

        // Cancel Create Post Button - Panel'i kapatır
        if (cancelCreatePostButton != null)
        {
            cancelCreatePostButton.onClick.RemoveAllListeners();
            cancelCreatePostButton.onClick.AddListener(OnCancelCreatePostButton);
        }
    }

    /// <summary>
    /// New Post Button'a tıklandığında - Seçenekleri yükle ve paneli aç
    /// </summary>
    private void OnNewPostButton()
    {
        if (createPostPanel != null)
        {
            createPostPanel.SetActive(true);
            LoadPostOptions();
        }
        else
        {
            Debug.LogWarning("[SocialMediaUI] Create Post Panel not assigned!");
        }
    }

    /// <summary>
    /// Post seçeneklerini sistemden çeker ve butonlara yazar
    /// </summary>
    private void LoadPostOptions()
    {
        if (SocialMediaSystem.Instance == null) return;

        // Son maçı bul
        MatchData lastMatch = null;
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            var fixtures = GameManager.Instance.CurrentSave.seasonData.fixtures;
            // Oynanmış son maçı bul
            lastMatch = fixtures.FindLast(m => m.isPlayed);
        }

        if (lastMatch == null)
        {
            // Eğer hiç maç oynanmamışsa veya veri yoksa dummy/boş veri yerine UI'ı gizle veya uyarı ver
            // Şimdilik test için dummy bırakmıyoruz, null kontrolü yapıyoruz.
            Debug.LogWarning("[SocialMediaUI] Oynanmış maç bulunamadı!");
            if (createPostPanel != null) createPostPanel.SetActive(false);
            return;
        }

        // Son maç bilgisini yaz
        if (lastMatchInfoText != null)
        {
            lastMatchInfoText.text = $"SON MAÇ: {lastMatch.homeTeamName} {lastMatch.homeScore}-{lastMatch.awayScore} {lastMatch.awayTeamName}\n" +
                                     $"Puan: {lastMatch.playerRating:F1} | Gol: {lastMatch.playerGoals}";
        }

        // Seçenekleri oluştur
        currentPostOptions = SocialMediaSystem.Instance.GetPlayerPostOptions(lastMatch);

        // Butonlara ata
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < currentPostOptions.Count && optionButtons[i] != null)
            {
                optionButtons[i].gameObject.SetActive(true);
                
                // Butondaki Text'i güncelle
                if (optionTexts != null && i < optionTexts.Length && optionTexts[i] != null)
                {
                    // Şöyle bir format: "ÖVGÜ PAYLAŞ: Harika goldü..."
                    string prefix = "";
                    switch(currentPostOptions[i].tone)
                    {
                        case SocialMediaPostTone.Positive: prefix = "ÖVGÜ PAYLAŞ:"; break;
                        case SocialMediaPostTone.Negative: prefix = "ELEŞTİRİ PAYLAŞ:"; break;
                        case SocialMediaPostTone.Motivational: prefix = "MOTİVASYON PAYLAŞ:"; break;
                    }
                    
                    optionTexts[i].text = $"<b>{prefix}</b> {currentPostOptions[i].content}";
                }
            }
            else if (optionButtons[i] != null)
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Bir seçenek seçildiğinde
    /// </summary>
    private void OnOptionSelected(int index)
    {
        if (currentPostOptions == null || index < 0 || index >= currentPostOptions.Count) return;

        SocialMediaPost selected = currentPostOptions[index];
        
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            MediaData mediaData = GameManager.Instance.CurrentSave.mediaData;
            SocialMediaSystem.Instance.PublishPost(mediaData, selected);
        }

        // Paneli kapat ve yenile
        if (createPostPanel != null)
            createPostPanel.SetActive(false);

        LoadPosts();
        RefreshFollowers();
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

    [Header("Design Settings")]
    public Color postBackgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.9f); // Koyu Cam
    public Color normalTextColor = Color.white;
    public Color goldTextColor = new Color(1f, 0.84f, 0f, 1f); // Gold
    public Color dateTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);

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
            // Prefab yoksa runtime'da oluştur (Gold/Glass Aesthetic fallback)
            itemObj = new GameObject($"PostItem_{post.author}");
            itemObj.transform.SetParent(postListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            // Genişlik ebeveyne göre ayarlansın
            rect.sizeDelta = new Vector2(0, 120); 

            Image bg = itemObj.AddComponent<Image>();
            bg.color = postBackgroundColor;

            Button button = itemObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnPostItemClicked(post));

            // Post içeriği
            GameObject contentObj = new GameObject("PostContent");
            contentObj.transform.SetParent(itemObj.transform);
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 0.4f);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = new Vector2(20, 5);
            contentRect.offsetMax = new Vector2(-20, -5);

            TextMeshProUGUI contentText = contentObj.AddComponent<TextMeshProUGUI>();
            contentText.text = post.content;
            contentText.fontSize = 18;
            contentText.color = normalTextColor;
            contentText.alignment = TextAlignmentOptions.TopLeft;
            contentText.overflowMode = TextOverflowModes.Ellipsis;

            // Post yazarı ve tarihi
            GameObject infoObj = new GameObject("PostInfo");
            infoObj.transform.SetParent(itemObj.transform);
            RectTransform infoRect = infoObj.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0, 0);
            infoRect.anchorMax = new Vector2(1, 0.4f);
            infoRect.offsetMin = new Vector2(20, 5);
            infoRect.offsetMax = new Vector2(-20, -5);

            TextMeshProUGUI infoText = infoObj.AddComponent<TextMeshProUGUI>();
            // Zengin metin kullanımı
            infoText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(goldTextColor)}><b>{post.author}</b></color> <size=14><color=#{ColorUtility.ToHtmlStringRGB(dateTextColor)}>{post.dateString}</color></size> \n<color=red>❤️</color> {post.likes}";
            infoText.fontSize = 16;
            infoText.alignment = TextAlignmentOptions.BottomLeft;
        }

        // Prefab varsa bile tıklama event'i ekle
        Button itemButton = itemObj.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnPostItemClicked(post));
        }

        // Prefab içindeki componentleri güncelle (Eğer prefab kullanılıyorsa)
        if (postItemPrefab != null)
        {
            // Bu kısım prefab yapısına göre değişir, ancak temel mantık:
            // TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
            // if (texts.Length > 0) texts[0].text = post.content; ...
        }
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

    /// <summary>
    /// Cancel Create Post Button - Panel'i kapatır
    /// </summary>
    private void OnCancelCreatePostButton()
    {
        if (createPostPanel != null)
            createPostPanel.SetActive(false);
    }

    private void OnCloseDetailButton()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);

        selectedPost = null;
    }
}

