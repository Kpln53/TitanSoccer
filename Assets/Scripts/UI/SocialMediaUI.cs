using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using TitanSoccer.Social;

public class SocialMediaUI : MonoBehaviour
{
    [Header("Main Feed")]
    public Transform feedContent;
    public GameObject postPrefab;
    public TextMeshProUGUI followersText;
    public Button createPostButton;

    [Header("Comments Popup")]
    public GameObject commentsPanel;
    public Transform commentsContent;
    public GameObject commentItemPrefab; // Basit text prefabı
    public Button closeCommentsButton;
    public TextMeshProUGUI commentsPostContent; // Hangi postun yorumları

    [Header("Create Post Popup")]
    public GameObject createPostPanel;
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;
    public Button closeCreatePostButton;
    public TextMeshProUGUI lastMatchInfoText;

    private void Start()
    {
        // Singleton System yoksa oluştur (Test için)
        if (SocialMediaSystem.Instance == null)
        {
            GameObject sys = new GameObject("SocialMediaSystem");
            sys.AddComponent<SocialMediaSystem>();
        }

        SetupUI();
        RefreshFeed(); // Test verileriyle doldur
        UpdateFollowers();
    }

    private void SetupUI()
    {
        if (createPostButton) createPostButton.onClick.AddListener(OpenCreatePostPanel);
        if (closeCommentsButton) closeCommentsButton.onClick.AddListener(() => commentsPanel.SetActive(false));
        if (closeCreatePostButton) closeCreatePostButton.onClick.AddListener(() => createPostPanel.SetActive(false));

        if (commentsPanel) commentsPanel.SetActive(false);
        if (createPostPanel) createPostPanel.SetActive(false);
    }

    private void UpdateFollowers()
    {
        if (followersText && SocialMediaSystem.Instance != null)
        {
            followersText.text = $"TAKİPÇİ: {SocialMediaSystem.Instance.FormatFollowers(SocialMediaSystem.Instance.Followers)}";
        }
    }

    // --- FEED ---

    public void RefreshFeed()
    {
        // Mevcutları temizle
        foreach (Transform child in feedContent) Destroy(child.gameObject);

        // Sistemdeki feed'i çek
        var posts = SocialMediaSystem.Instance.Feed;
        
        // Eğer feed boşsa (ilk açılış), örnek veri ekle
        if (posts.Count == 0)
        {
            SocialMediaSystem.Instance.AddToFeed(new SocialPostData
            {
                authorName = "Real Madrid",
                handle = "@RealMadrid",
                content = "BUGÜN DERBİ GÜNÜ! KADRO HAZIR. 🔥 @CanYildiz",
                likes = 15400,
                commentsCount = 240,
                type = PostType.ClubPost,
                timeAgo = "2s önce"
            });
        }

        foreach (var post in posts)
        {
            CreatePostUI(post);
        }
    }

    private void CreatePostUI(SocialPostData data)
    {
        GameObject obj = Instantiate(postPrefab, feedContent);
        SocialPostUI ui = obj.GetComponent<SocialPostUI>();
        if (ui)
        {
            ui.Setup(data, OnCommentClick);
        }
    }

    // --- COMMENTS ---

    private void OnCommentClick(SocialPostData post)
    {
        if (!commentsPanel) return;

        commentsPanel.SetActive(true);
        if (commentsPostContent) commentsPostContent.text = post.content;

        // Yorumları temizle
        foreach (Transform child in commentsContent) Destroy(child.gameObject);

        // Test yorumları üret (Eğer postun yorumu yoksa)
        if (post.comments == null || post.comments.Count == 0)
        {
            post.comments = SocialMediaSystem.Instance.GenerateComments(post, Sentiment.Positive);
        }

        foreach (var comment in post.comments)
        {
            CreateCommentUI(comment);
        }
    }

    private void CreateCommentUI(SocialCommentData comment)
    {
        GameObject obj = Instantiate(commentItemPrefab, commentsContent);
        
        // Basit yapı: İsim: Yorum
        TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt)
        {
            txt.text = $"<color=#FFD700>{comment.handle}:</color> {comment.content}";
        }
    }

    // --- CREATE POST ---

    private void OpenCreatePostPanel()
    {
        if (!SocialMediaSystem.Instance.CanPost())
        {
            Debug.Log("Post hakkı kalmadı!");
            // Uyarı popup'ı eklenebilir
            return;
        }

        createPostPanel.SetActive(true);

        // Son maç bilgisini güncelle
        var lastMatch = SocialMediaSystem.Instance.LastMatchContext;
        if (lastMatch != null && lastMatchInfoText != null)
        {
            string result = lastMatch.homeScore > lastMatch.awayScore ? "Galibiyet" : 
                           (lastMatch.homeScore < lastMatch.awayScore ? "Mağlubiyet" : "Beraberlik");
            
            // Eğer deplasmandaysak ve yendiysek (away > home) -> Galibiyet
            if (!lastMatch.isHomeTeam)
            {
                 result = lastMatch.awayScore > lastMatch.homeScore ? "Galibiyet" : 
                          (lastMatch.awayScore < lastMatch.homeScore ? "Mağlubiyet" : "Beraberlik");
            }

            lastMatchInfoText.text = $"SON MAÇ: {lastMatch.homeTeamName} {lastMatch.homeScore}-{lastMatch.awayScore} {lastMatch.awayTeamName}\n" +
                                     $"Durum: {result} | Rating: {lastMatch.playerRating:F1}";
        }
        else if (lastMatchInfoText != null)
        {
            lastMatchInfoText.text = "Henüz maç oynanmadı.";
        }

        // Seçenekleri temizle
        foreach (Transform child in optionsContainer) Destroy(child.gameObject);

        // Seçenekleri al (LastMatchContext otomatik kullanılır)
        var options = SocialMediaSystem.Instance.GeneratePostOptions(null); 

        foreach (var opt in options)
        {
            CreateOptionUI(opt);
        }
    }

    private void CreateOptionUI(PostOption option)
    {
        GameObject obj = Instantiate(optionButtonPrefab, optionsContainer);
        
        // Buton metni (Örn: ÖVGÜ PAYLAŞ: Açıklama...)
        TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (txt)
        {
            txt.text = $"<color=#FFD700>{option.buttonText}:</color> {option.description}";
        }

        Button btn = obj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnSharePost(option));
    }

    private void OnSharePost(PostOption option)
    {
        // 1. Postu oluştur
        SocialPostData newPost = new SocialPostData
        {
            authorName = "Can Yıldız", // Oyuncu adı
            handle = "@CanYildiz",
            content = option.postContent,
            likes = 0,
            commentsCount = 0,
            type = PostType.PlayerPost,
            timeAgo = "Şimdi"
        };

        // 2. Etkileşim Hesapla (Takipçi sayısına göre)
        SocialMediaSystem.Instance.CalculateEngagement(newPost);

        // 3. Yorumları üret (Seçimin sonucuna göre)
        newPost.comments = SocialMediaSystem.Instance.GenerateComments(newPost, option.predictedOutcome);
        
        // 4. Feed'e ekle (En başa)
        SocialMediaSystem.Instance.AddToFeed(newPost); // Sisteme de ekle
        CreatePostUI(newPost);
        obj_MoveToTop(feedContent.GetChild(feedContent.childCount - 1)); // Son ekleneni başa al

        // 4. Takipçi güncelle (Sonuca göre)
        if (option.predictedOutcome == Sentiment.Positive)
            SocialMediaSystem.Instance.AddFollowers(Random.Range(1000, 5000));
        else
            SocialMediaSystem.Instance.AddFollowers(Random.Range(-500, 500)); // Riskli

        UpdateFollowers();

        // 5. Hakkı düş ve kapat
        SocialMediaSystem.Instance.UsePostRight();
        createPostPanel.SetActive(false);
    }

    private void obj_MoveToTop(Transform t)
    {
        t.SetSiblingIndex(0);
    }
}
