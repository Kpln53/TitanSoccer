using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class NewsUIBuilder_v3 : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔨 Build News UI (Reference Style)")]
    public static void BuildNewsUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (canvasObj == null)
        {
            Debug.LogError("CareerHubCanvas not found!");
            return;
        }

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (mainPanel == null)
        {
            Debug.LogError("MainPanel/ContentArea not found!");
            return;
        }

        // 1. Clean up old NewsPanel
        Transform oldPanel = mainPanel.Find("NewsPanel");
        if (oldPanel != null)
        {
            DestroyImmediate(oldPanel.gameObject);
        }

        // 2. Create NewsPanel Container
        GameObject newsPanel = new GameObject("NewsPanel");
        newsPanel.transform.SetParent(mainPanel, false);
        RectTransform panelRect = newsPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Add CareerNewsUI component
        CareerNewsUI careerNewsUI = newsPanel.AddComponent<CareerNewsUI>();

        // Background (Dark Theme)
        Image panelBg = newsPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.12f, 1f);

        // --- FEATURED NEWS (Günün Manşeti) ---
        GameObject featuredSection = new GameObject("FeaturedSection");
        featuredSection.transform.SetParent(newsPanel.transform, false);
        RectTransform fr = featuredSection.AddComponent<RectTransform>();
        fr.anchorMin = new Vector2(0, 0.65f); // Top 35%
        fr.anchorMax = new Vector2(1, 1);
        fr.offsetMin = new Vector2(20, 10);
        fr.offsetMax = new Vector2(-20, -20);

        // Frame/Background for Featured
        Image fBg = featuredSection.AddComponent<Image>();
        fBg.color = new Color(0.15f, 0.15f, 0.18f, 1f);
        
        // "GÜNÜN MANŞETİ" Label
        GameObject fLabel = new GameObject("Label");
        fLabel.transform.SetParent(featuredSection.transform, false);
        RectTransform flr = fLabel.AddComponent<RectTransform>();
        flr.anchorMin = new Vector2(0, 1);
        flr.anchorMax = new Vector2(1, 1);
        flr.offsetMin = new Vector2(10, -30);
        flr.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI flTxt = fLabel.AddComponent<TextMeshProUGUI>();
        flTxt.text = "GÜNÜN MANŞETİ";
        flTxt.fontSize = 14;
        flTxt.color = new Color(1f, 0.8f, 0.2f); // Gold
        flTxt.fontStyle = FontStyles.Bold;
        flTxt.alignment = TextAlignmentOptions.TopLeft;

        // Featured Content Container
        GameObject fContent = new GameObject("FeaturedContent");
        fContent.transform.SetParent(featuredSection.transform, false);
        RectTransform fcr = fContent.AddComponent<RectTransform>();
        fcr.anchorMin = Vector2.zero;
        fcr.anchorMax = Vector2.one;
        fcr.offsetMin = Vector2.zero;
        fcr.offsetMax = Vector2.zero;

        // Add NewsItemUI to Featured Section
        NewsItemUI featuredUI = fContent.AddComponent<NewsItemUI>();
        careerNewsUI.featuredNewsDisplay = featuredUI;
        
        // Featured Image (Placeholder)
        GameObject fImgObj = new GameObject("Image");
        fImgObj.transform.SetParent(fContent.transform, false);
        RectTransform fir = fImgObj.AddComponent<RectTransform>();
        fir.anchorMin = new Vector2(0, 0);
        fir.anchorMax = new Vector2(0.4f, 1); // Left 40%
        fir.offsetMin = new Vector2(10, 10);
        fir.offsetMax = new Vector2(-10, -30); // Below label
        
        Image fImg = fImgObj.AddComponent<Image>();
        fImg.color = new Color(0.2f, 0.2f, 0.25f);
        featuredUI.typeIcon = fImg; // Using typeIcon slot for main image for now

        // Featured Title
        GameObject fTitleObj = new GameObject("Title");
        fTitleObj.transform.SetParent(fContent.transform, false);
        RectTransform ftr = fTitleObj.AddComponent<RectTransform>();
        ftr.anchorMin = new Vector2(0.4f, 0.5f);
        ftr.anchorMax = new Vector2(1, 1);
        ftr.offsetMin = new Vector2(10, 0);
        ftr.offsetMax = new Vector2(-10, -30);
        
        TextMeshProUGUI ftTxt = fTitleObj.AddComponent<TextMeshProUGUI>();
        ftTxt.text = "Manşet Haber Başlığı";
        ftTxt.fontSize = 24;
        ftTxt.fontStyle = FontStyles.Bold;
        ftTxt.color = Color.white;
        ftTxt.enableWordWrapping = true;
        featuredUI.titleText = ftTxt;

        // Featured Date/Source
        GameObject fMetaObj = new GameObject("Meta");
        fMetaObj.transform.SetParent(fContent.transform, false);
        RectTransform fmr = fMetaObj.AddComponent<RectTransform>();
        fmr.anchorMin = new Vector2(0.4f, 0);
        fmr.anchorMax = new Vector2(1, 0.5f);
        fmr.offsetMin = new Vector2(10, 10);
        fmr.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI fmTxt = fMetaObj.AddComponent<TextMeshProUGUI>();
        fmTxt.text = "01.01.2025 • Kaynak";
        fmTxt.fontSize = 14;
        fmTxt.color = Color.gray;
        featuredUI.dateText = fmTxt;

        // Button component for click
        Button fBtn = fContent.AddComponent<Button>();
        fBtn.targetGraphic = fBg;
        featuredUI.itemButton = fBtn;
        featuredUI.backgroundImage = fBg;

        // --- FILTER TABS ---
        GameObject filterSection = new GameObject("FilterSection");
        filterSection.transform.SetParent(newsPanel.transform, false);
        RectTransform fltr = filterSection.AddComponent<RectTransform>();
        fltr.anchorMin = new Vector2(0, 0.58f); // Below featured
        fltr.anchorMax = new Vector2(1, 0.65f); // Height ~7%
        fltr.offsetMin = new Vector2(20, 0);
        fltr.offsetMax = new Vector2(-20, 0);

        HorizontalLayoutGroup flg = filterSection.AddComponent<HorizontalLayoutGroup>();
        flg.spacing = 10;
        flg.childControlWidth = true;
        flg.childForceExpandWidth = true;

        string[] filters = { "Tümü", "Transfer", "Lig", "Kulüp" };
        foreach (string filter in filters)
        {
            CreateFilterButton(filterSection.transform, filter);
        }

        // --- NEWS LIST ---
        GameObject listSection = new GameObject("ListSection");
        listSection.transform.SetParent(newsPanel.transform, false);
        RectTransform lsr = listSection.AddComponent<RectTransform>();
        lsr.anchorMin = new Vector2(0, 0);
        lsr.anchorMax = new Vector2(1, 0.58f); // Rest of the space
        lsr.offsetMin = new Vector2(20, 20);
        lsr.offsetMax = new Vector2(-20, -10);

        // Scroll View
        GameObject scrollView = new GameObject("NewsScrollView");
        scrollView.transform.SetParent(listSection.transform, false);
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;

        ScrollRect sr = scrollView.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.scrollSensitivity = 20;
        careerNewsUI.scrollRect = sr;

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewRect = viewport.AddComponent<RectTransform>();
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        
        Image viewImg = viewport.AddComponent<Image>();
        viewImg.color = new Color(1, 1, 1, 0.01f);
        Mask viewMask = viewport.AddComponent<Mask>();
        viewMask.showMaskGraphic = false;
        sr.viewport = viewRect;

        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 8;
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = contentRect;
        careerNewsUI.newsListParent = content.transform;

        // --- PREFABS ---
        // Re-use existing prefabs or create new ones if needed
        // Assuming ModernNewsItem.prefab and ModernNewsDetail.prefab exist from previous step
        // If not, we should recreate them or use the ones we have.
        // Let's try to load them first.
        GameObject itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/ModernNewsItem.prefab");
        GameObject detailPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/ModernNewsDetail.prefab");
        
        if (itemPrefab != null) careerNewsUI.newsItemPrefab = itemPrefab;
        if (detailPrefab != null) careerNewsUI.newsDetailPrefab = detailPrefab;

        // Status Text (Hidden or small at bottom)
        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(newsPanel.transform, false);
        RectTransform statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0);
        statusRect.sizeDelta = new Vector2(0, 20);
        TextMeshProUGUI st = statusObj.AddComponent<TextMeshProUGUI>();
        st.fontSize = 10;
        st.alignment = TextAlignmentOptions.BottomRight;
        st.color = new Color(1,1,1,0.3f);
        careerNewsUI.statusText = st;

        // Refresh Button (Hidden or integrated)
        // For now, let's add a small refresh button in the corner of Featured section
        GameObject refreshBtn = new GameObject("RefreshButton");
        refreshBtn.transform.SetParent(featuredSection.transform, false);
        RectTransform rbr = refreshBtn.AddComponent<RectTransform>();
        rbr.anchorMin = new Vector2(1, 1);
        rbr.anchorMax = new Vector2(1, 1);
        rbr.sizeDelta = new Vector2(30, 30);
        rbr.anchoredPosition = new Vector2(-10, -10);
        Image rbImg = refreshBtn.AddComponent<Image>();
        rbImg.color = new Color(1,1,1,0.5f);
        Button rb = refreshBtn.AddComponent<Button>();
        rb.targetGraphic = rbImg;
        careerNewsUI.refreshButton = rb;

        // Test Button (Hidden)
        GameObject testBtn = new GameObject("TestNewsButton");
        testBtn.transform.SetParent(newsPanel.transform, false);
        testBtn.SetActive(false); // Hide by default
        careerNewsUI.generateTestNewsButton = testBtn.AddComponent<Button>();

        Debug.Log("✅ Reference Style News UI Built Successfully!");
    }

    private static void CreateFilterButton(Transform parent, string text)
    {
        GameObject btnObj = new GameObject(text);
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.25f);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        RectTransform tr = txtObj.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 14;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
    }
}
