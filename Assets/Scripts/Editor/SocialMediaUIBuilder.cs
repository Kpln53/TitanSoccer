using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Sosyal Medya Panel'ini otomatik oluşturan Editor scripti
/// </summary>
public class SocialMediaUIBuilder : Editor
{
    [MenuItem("TitanSoccer/UI/Rebuild Social Media Panel (Clean)")]
    public static void RebuildSocialMediaPanel()
    {
        // Eski paneli sil
        GameObject existing = GameObject.Find("SocialMediaPanel");
        if (existing != null)
        {
            DestroyImmediate(existing);
            Debug.Log("[SocialMediaUIBuilder] Eski panel silindi");
        }
        
        // ContentArea içindeki eski paneli de sil
        GameObject contentArea = GameObject.Find("ContentArea");
        if (contentArea != null)
        {
            Transform oldPanel = contentArea.transform.Find("SocialMediaPanel");
            if (oldPanel != null)
            {
                DestroyImmediate(oldPanel.gameObject);
            }
        }
        
        // Yenisini oluştur
        CreateCleanSocialMediaPanel();
    }

    [MenuItem("TitanSoccer/UI/Create Social Media Panel (Clean)")]
    public static void CreateCleanSocialMediaPanel()
    {
        // ContentArea'yı bul
        GameObject contentArea = GameObject.Find("ContentArea");
        if (contentArea == null)
        {
            Debug.LogError("[SocialMediaUIBuilder] ContentArea bulunamadı! CareerHub sahnesini aç.");
            return;
        }

        Debug.Log("[SocialMediaUIBuilder] Panel oluşturuluyor...");

        // ========== ANA PANEL ==========
        GameObject mainPanel = new GameObject("SocialMediaPanel");
        mainPanel.transform.SetParent(contentArea.transform, false);
        
        var mainRect = mainPanel.AddComponent<RectTransform>();
        mainRect.anchorMin = Vector2.zero;
        mainRect.anchorMax = Vector2.one;
        mainRect.offsetMin = Vector2.zero;
        mainRect.offsetMax = Vector2.zero;
        
        var mainImage = mainPanel.AddComponent<Image>();
        mainImage.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        
        var mainLayout = mainPanel.AddComponent<VerticalLayoutGroup>();
        mainLayout.padding = new RectOffset(15, 15, 15, 15);
        mainLayout.spacing = 10;
        mainLayout.childControlWidth = true;
        mainLayout.childControlHeight = false;
        mainLayout.childForceExpandWidth = true;
        mainLayout.childForceExpandHeight = false;

        // ========== HEADER ==========
        GameObject header = new GameObject("Header");
        header.transform.SetParent(mainPanel.transform, false);
        
        var headerRect = header.AddComponent<RectTransform>();
        var headerLE = header.AddComponent<LayoutElement>();
        headerLE.preferredHeight = 50;
        
        var headerLayout = header.AddComponent<HorizontalLayoutGroup>();
        headerLayout.spacing = 15;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = false;
        
        // Followers Text
        GameObject followersGO = new GameObject("FollowersText");
        followersGO.transform.SetParent(header.transform, false);
        var followersRect = followersGO.AddComponent<RectTransform>();
        var followersLE = followersGO.AddComponent<LayoutElement>();
        followersLE.flexibleWidth = 1;
        var followersText = followersGO.AddComponent<TextMeshProUGUI>();
        followersText.text = "👥 10.000 Takipçi";
        followersText.fontSize = 22;
        followersText.color = Color.white;
        followersText.alignment = TextAlignmentOptions.MidlineLeft;

        // New Post Button
        GameObject newPostBtn = CreateSimpleButton("NewPostButton", header.transform, "📝 Yeni Post", new Color(0.2f, 0.5f, 0.9f));
        var newPostLE = newPostBtn.AddComponent<LayoutElement>();
        newPostLE.preferredWidth = 180;

        // ========== SCROLL VIEW ==========
        GameObject scrollView = new GameObject("PostScrollView");
        scrollView.transform.SetParent(mainPanel.transform, false);
        
        var scrollRect = scrollView.AddComponent<RectTransform>();
        var scrollLE = scrollView.AddComponent<LayoutElement>();
        scrollLE.flexibleHeight = 1;
        scrollLE.preferredHeight = 400;
        
        var scrollImage = scrollView.AddComponent<Image>();
        scrollImage.color = new Color(0.08f, 0.08f, 0.1f, 1f);
        
        var scroll = scrollView.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.scrollSensitivity = 30;

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        
        var viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        var viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        var mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content (POST CONTAINER)
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        var contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(5, 5, 5, 5);
        contentLayout.spacing = 8;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        
        var contentFitter = content.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        // ========== CREATE POST POPUP ==========
        GameObject popup = CreatePostPopup(mainPanel.transform);

        // ========== SOCIALMEDAUI COMPONENT ==========
        var socialMediaUI = mainPanel.AddComponent<TitanSoccer.UI.Social.SocialMediaUI>();
        
        // Referansları bağla
        SerializedObject so = new SerializedObject(socialMediaUI);
        
        var postContainerProp = so.FindProperty("postContainer");
        if (postContainerProp != null) postContainerProp.objectReferenceValue = content.transform;
        
        var newPostButtonProp = so.FindProperty("newPostButton");
        if (newPostButtonProp != null) newPostButtonProp.objectReferenceValue = newPostBtn.GetComponent<Button>();
        
        var followersTextProp = so.FindProperty("followersText");
        if (followersTextProp != null) followersTextProp.objectReferenceValue = followersText;
        
        var createPostPopupProp = so.FindProperty("createPostPopup");
        if (createPostPopupProp != null) createPostPopupProp.objectReferenceValue = popup;
        
        // Post option buttons
        var optionButtonsProp = so.FindProperty("postOptionButtons");
        if (optionButtonsProp != null)
        {
            var buttons = popup.transform.Find("Content").GetComponentsInChildren<Button>();
            // İlk 4 buton option, son 1 cancel
            optionButtonsProp.arraySize = Mathf.Min(buttons.Length - 1, 4);
            for (int i = 0; i < optionButtonsProp.arraySize && i < buttons.Length - 1; i++)
            {
                optionButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
            }
        }
        
        var cancelPostButtonProp = so.FindProperty("cancelPostButton");
        if (cancelPostButtonProp != null)
        {
            var cancelBtn = popup.transform.Find("Content/CancelButton");
            if (cancelBtn != null) cancelPostButtonProp.objectReferenceValue = cancelBtn.GetComponent<Button>();
        }
        
        so.ApplyModifiedProperties();

        // ========== CAREERHUBUI'YE BAĞLA ==========
        var careerHubUI = Object.FindFirstObjectByType<CareerHubUI>();
        if (careerHubUI != null)
        {
            SerializedObject hubSO = new SerializedObject(careerHubUI);
            var panelProp = hubSO.FindProperty("socialMediaPanel");
            if (panelProp != null)
            {
                panelProp.objectReferenceValue = mainPanel;
                hubSO.ApplyModifiedProperties();
                Debug.Log("[SocialMediaUIBuilder] CareerHubUI'ye bağlandı!");
            }
        }

        // Panel başlangıçta kapalı
        mainPanel.SetActive(false);

        // Sahneyi işaretle
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[SocialMediaUIBuilder] ✅ SocialMediaPanel başarıyla oluşturuldu!");
        Selection.activeGameObject = mainPanel;
    }

    private static GameObject CreatePostPopup(Transform parent)
    {
        GameObject popup = new GameObject("CreatePostPopup");
        popup.transform.SetParent(parent, false);
        
        var rect = popup.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.05f, 0.15f);
        rect.anchorMax = new Vector2(0.95f, 0.85f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var image = popup.AddComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.18f, 0.98f);
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(popup.transform, false);
        var titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 50);
        titleRect.anchoredPosition = Vector2.zero;
        
        var titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "📝 Yeni Post Paylaş";
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(popup.transform, false);
        
        var contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = new Vector2(20, 20);
        contentRect.offsetMax = new Vector2(-20, -60);
        
        var contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 10;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        
        // Option Buttons
        string[] options = { "Harika bir gün! ⚽", "Çalışmaya devam! 💪", "Takım ruhu! 🤝", "Teşekkürler! 🙏" };
        foreach (var opt in options)
        {
            var btn = CreateSimpleButton($"Option_{opt.Substring(0, 5)}", content.transform, opt, new Color(0.2f, 0.25f, 0.35f));
            var le = btn.AddComponent<LayoutElement>();
            le.preferredHeight = 55;
        }
        
        // Cancel Button
        var cancelBtn = CreateSimpleButton("CancelButton", content.transform, "❌ İptal", new Color(0.5f, 0.2f, 0.2f));
        var cancelLE = cancelBtn.AddComponent<LayoutElement>();
        cancelLE.preferredHeight = 50;
        
        popup.SetActive(false);
        return popup;
    }

    private static GameObject CreateSimpleButton(string name, Transform parent, string text, Color bgColor)
    {
        GameObject btn = new GameObject(name);
        btn.transform.SetParent(parent, false);
        
        var rect = btn.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 50);
        
        var image = btn.AddComponent<Image>();
        image.color = bgColor;
        
        var button = btn.AddComponent<Button>();
        button.targetGraphic = image;
        
        var colors = button.colors;
        colors.highlightedColor = new Color(bgColor.r + 0.1f, bgColor.g + 0.1f, bgColor.b + 0.1f);
        colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f);
        button.colors = colors;
        
        // Text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btn.transform, false);
        
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);
        
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        
        return btn;
    }
}
