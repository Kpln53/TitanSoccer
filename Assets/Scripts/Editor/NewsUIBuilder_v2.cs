using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class NewsUIBuilder_v2 : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔨 Build Modern News UI")]
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

        // 1. Clean up old NewsPanel if exists
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

        // Background
        Image panelBg = newsPanel.AddComponent<Image>();
        panelBg.color = new Color(0.12f, 0.12f, 0.15f, 1f); // Dark background

        // --- HEADER ---
        GameObject header = new GameObject("Header");
        header.transform.SetParent(newsPanel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 80);
        headerRect.anchoredPosition = Vector2.zero;

        Image headerBg = header.AddComponent<Image>();
        headerBg.color = new Color(0.18f, 0.18f, 0.22f, 1f);

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(header.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.offsetMin = new Vector2(30, 0);
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "HABER MERKEZİ";
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.MidlineLeft;
        titleText.color = new Color(0.9f, 0.9f, 0.9f);

        // Controls Container (Right side of header)
        GameObject controls = new GameObject("Controls");
        controls.transform.SetParent(header.transform, false);
        RectTransform controlsRect = controls.AddComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0.5f, 0);
        controlsRect.anchorMax = new Vector2(1, 1);
        controlsRect.offsetMin = Vector2.zero;
        controlsRect.offsetMax = new Vector2(-30, 0);

        HorizontalLayoutGroup controlsLayout = controls.AddComponent<HorizontalLayoutGroup>();
        controlsLayout.childAlignment = TextAnchor.MiddleRight;
        controlsLayout.spacing = 15;
        controlsLayout.childControlWidth = false;
        controlsLayout.childForceExpandWidth = false;

        // Filter Dropdown
        GameObject dropdownObj = CreateDropdown(controls.transform, "FilterDropdown");
        careerNewsUI.newsTypeFilter = dropdownObj.GetComponent<Dropdown>();

        // Refresh Button
        GameObject refreshBtnObj = CreateButton(controls.transform, "RefreshButton", "Yenile", new Color(0.2f, 0.6f, 1f));
        careerNewsUI.refreshButton = refreshBtnObj.GetComponent<Button>();

        // Test Button
        GameObject testBtnObj = CreateButton(controls.transform, "TestNewsButton", "Test Haber", new Color(1f, 0.6f, 0.2f));
        careerNewsUI.generateTestNewsButton = testBtnObj.GetComponent<Button>();

        // Status Text (Bottom of panel)
        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(newsPanel.transform, false);
        RectTransform statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0);
        statusRect.anchorMax = new Vector2(1, 0);
        statusRect.pivot = new Vector2(0.5f, 0);
        statusRect.sizeDelta = new Vector2(0, 30);
        statusRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI statusText = statusObj.AddComponent<TextMeshProUGUI>();
        statusText.fontSize = 14;
        statusText.alignment = TextAlignmentOptions.MidlineRight;
        statusText.color = Color.gray;
        statusText.margin = new Vector4(0, 0, 20, 0);
        careerNewsUI.statusText = statusText;

        // --- SCROLL VIEW ---
        GameObject scrollView = new GameObject("NewsScrollView");
        scrollView.transform.SetParent(newsPanel.transform, false);
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(20, 40); // Bottom margin for status
        scrollRect.offsetMax = new Vector2(-20, -90); // Top margin for header

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
        viewRect.pivot = new Vector2(0, 1);
        
        Image viewImg = viewport.AddComponent<Image>();
        viewImg.color = new Color(1, 1, 1, 0.01f); // Almost transparent
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
        contentRect.sizeDelta = new Vector2(0, 0); // Height controlled by fitter

        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(0, 10, 10, 10); // Right padding for scrollbar
        vlg.spacing = 8;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = contentRect;
        careerNewsUI.newsListParent = content.transform;

        // Scrollbar Vertical
        GameObject scrollbar = new GameObject("Scrollbar Vertical");
        scrollbar.transform.SetParent(scrollView.transform, false);
        RectTransform sbRect = scrollbar.AddComponent<RectTransform>();
        sbRect.anchorMin = new Vector2(1, 0);
        sbRect.anchorMax = new Vector2(1, 1);
        sbRect.pivot = new Vector2(1, 1);
        sbRect.sizeDelta = new Vector2(10, 0);
        
        Image sbBg = scrollbar.AddComponent<Image>();
        sbBg.color = new Color(0, 0, 0, 0.3f);
        
        Scrollbar sb = scrollbar.AddComponent<Scrollbar>();
        sb.direction = Scrollbar.Direction.BottomToTop;
        
        GameObject slidingArea = new GameObject("Sliding Area");
        slidingArea.transform.SetParent(scrollbar.transform, false);
        RectTransform saRect = slidingArea.AddComponent<RectTransform>();
        saRect.anchorMin = Vector2.zero;
        saRect.anchorMax = Vector2.one;
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slidingArea.transform, false);
        RectTransform hRect = handle.AddComponent<RectTransform>();
        hRect.sizeDelta = new Vector2(0, 0);
        
        Image hImg = handle.AddComponent<Image>();
        hImg.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        
        sb.handleRect = hRect;
        sb.targetGraphic = hImg;
        sr.verticalScrollbar = sb;

        // --- PREFABS ---
        // Create Prefabs and assign them
        careerNewsUI.newsItemPrefab = CreateNewsItemPrefab();
        careerNewsUI.newsDetailPrefab = CreateNewsDetailPrefab();

        Debug.Log("✅ Modern News UI Built Successfully!");
    }

    private static GameObject CreateButton(Transform parent, string name, string text, Color color)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = color;
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        LayoutElement le = btnObj.AddComponent<LayoutElement>();
        le.minWidth = 100;
        le.minHeight = 40;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 16;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        
        return btnObj;
    }

    private static GameObject CreateDropdown(Transform parent, string name)
    {
        // Simplified dropdown creation - Unity's default dropdown structure is complex to build from scratch via script
        // Using a button as placeholder for now, or basic setup
        GameObject ddObj = new GameObject(name);
        ddObj.transform.SetParent(parent, false);
        
        Image img = ddObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.25f);
        
        Dropdown dd = ddObj.AddComponent<Dropdown>();
        dd.targetGraphic = img;
        
        LayoutElement le = ddObj.AddComponent<LayoutElement>();
        le.minWidth = 150;
        le.minHeight = 40;
        
        // Template and other parts would be needed for a full dropdown
        // For this task, we might skip the full dropdown construction or use a simpler approach
        // Let's add a basic label
        GameObject label = new GameObject("Label");
        label.transform.SetParent(ddObj.transform, false);
        RectTransform lr = label.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = new Vector2(10, 0);
        lr.offsetMax = new Vector2(-20, 0);
        Text txt = label.AddComponent<Text>();
        txt.text = "Filtrele...";
        txt.alignment = TextAnchor.MiddleLeft;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        dd.captionText = txt;

        return ddObj;
    }

    private static GameObject CreateNewsItemPrefab()
    {
        string path = "Assets/Prefabs/UI/ModernNewsItem.prefab";
        
        GameObject root = new GameObject("ModernNewsItem");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(0, 90); // Fixed height
        
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.18f, 0.22f, 1f); // Card background
        
        Button btn = root.AddComponent<Button>();
        btn.targetGraphic = bg;
        
        LayoutElement le = root.AddComponent<LayoutElement>();
        le.minHeight = 90;
        le.preferredHeight = 90;
        le.flexibleWidth = 1;
        
        NewsItemUI ui = root.AddComponent<NewsItemUI>();
        ui.backgroundImage = bg;
        ui.itemButton = btn;
        ui.readColor = new Color(0.5f, 0.5f, 0.5f);
        ui.unreadColor = Color.white;
        ui.readBgColor = new Color(0.15f, 0.15f, 0.18f, 1f);
        ui.unreadBgColor = new Color(0.22f, 0.22f, 0.28f, 1f);

        // Icon Container (Left)
        GameObject iconContainer = new GameObject("IconContainer");
        iconContainer.transform.SetParent(root.transform, false);
        RectTransform iconRect = iconContainer.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0);
        iconRect.anchorMax = new Vector2(0, 1);
        iconRect.sizeDelta = new Vector2(80, 0);
        iconRect.anchoredPosition = new Vector2(40, 0);
        
        Image iconImg = iconContainer.AddComponent<Image>();
        iconImg.color = Color.clear; // Transparent container
        
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(iconContainer.transform, false);
        RectTransform ir = icon.AddComponent<RectTransform>();
        ir.sizeDelta = new Vector2(40, 40);
        ir.anchoredPosition = Vector2.zero;
        
        Image iImg = icon.AddComponent<Image>();
        iImg.color = new Color(1f, 0.8f, 0.2f); // Gold icon
        ui.typeIcon = iImg;

        // Content Container (Middle)
        GameObject content = new GameObject("Content");
        content.transform.SetParent(root.transform, false);
        RectTransform cr = content.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0, 0);
        cr.anchorMax = new Vector2(1, 1);
        cr.offsetMin = new Vector2(90, 10);
        cr.offsetMax = new Vector2(-10, -10);
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(content.transform, false);
        RectTransform tr = title.AddComponent<RectTransform>();
        tr.anchorMin = new Vector2(0, 0.5f);
        tr.anchorMax = new Vector2(1, 1);
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;
        
        TextMeshProUGUI titleTxt = title.AddComponent<TextMeshProUGUI>();
        titleTxt.fontSize = 20;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.alignment = TextAlignmentOptions.BottomLeft;
        titleTxt.color = Color.white;
        titleTxt.text = "Haber Başlığı";
        ui.titleText = titleTxt;
        
        // Date & Source
        GameObject meta = new GameObject("Meta");
        meta.transform.SetParent(content.transform, false);
        RectTransform mr = meta.AddComponent<RectTransform>();
        mr.anchorMin = new Vector2(0, 0);
        mr.anchorMax = new Vector2(1, 0.5f);
        mr.offsetMin = Vector2.zero;
        mr.offsetMax = Vector2.zero;
        
        TextMeshProUGUI metaTxt = meta.AddComponent<TextMeshProUGUI>();
        metaTxt.fontSize = 14;
        metaTxt.color = new Color(0.7f, 0.7f, 0.7f);
        metaTxt.alignment = TextAlignmentOptions.TopLeft;
        metaTxt.text = "01.01.2025 • Spor Servisi";
        ui.dateText = metaTxt; // Using dateText for combined meta for now

        // Save Prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateNewsDetailPrefab()
    {
        string path = "Assets/Prefabs/UI/ModernNewsDetail.prefab";
        
        // Root (Overlay)
        GameObject root = new GameObject("ModernNewsDetail");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.85f); // Darker overlay
        
        Button bgBtn = root.AddComponent<Button>();
        bgBtn.targetGraphic = bg;
        
        NewsDetailUI ui = root.AddComponent<NewsDetailUI>();
        ui.backgroundButton = bgBtn;
        
        // Panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(root.transform, false);
        RectTransform pr = panel.AddComponent<RectTransform>();
        pr.anchorMin = new Vector2(0.5f, 0.5f);
        pr.anchorMax = new Vector2(0.5f, 0.5f);
        pr.sizeDelta = new Vector2(700, 500);
        
        Image pImg = panel.AddComponent<Image>();
        pImg.color = new Color(0.15f, 0.15f, 0.18f);
        
        ui.contentPanel = panel;
        
        // Header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0, 1);
        hr.anchorMax = new Vector2(1, 1);
        hr.pivot = new Vector2(0.5f, 1);
        hr.sizeDelta = new Vector2(0, 70);
        hr.anchoredPosition = Vector2.zero;
        
        Image hImg = header.AddComponent<Image>();
        hImg.color = new Color(0.2f, 0.2f, 0.25f);
        
        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(header.transform, false);
        RectTransform tr = title.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = new Vector2(20, 0);
        tr.offsetMax = new Vector2(-60, 0);
        
        TextMeshProUGUI tTxt = title.AddComponent<TextMeshProUGUI>();
        tTxt.text = "Haber Detayı";
        tTxt.fontSize = 24;
        tTxt.fontStyle = FontStyles.Bold;
        tTxt.alignment = TextAlignmentOptions.MidlineLeft;
        tTxt.color = Color.white;
        ui.titleText = tTxt;
        
        // Close Button
        GameObject close = new GameObject("CloseBtn");
        close.transform.SetParent(header.transform, false);
        RectTransform cr = close.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(1, 1);
        cr.anchorMax = new Vector2(1, 1);
        cr.pivot = new Vector2(1, 1);
        cr.sizeDelta = new Vector2(50, 50);
        cr.anchoredPosition = new Vector2(-10, -10);
        
        Image cImg = close.AddComponent<Image>();
        cImg.color = new Color(0.8f, 0.2f, 0.2f);
        Button cBtn = close.AddComponent<Button>();
        cBtn.targetGraphic = cImg;
        ui.closeButton = cBtn;
        
        GameObject xObj = new GameObject("X");
        xObj.transform.SetParent(close.transform, false);
        RectTransform xr = xObj.AddComponent<RectTransform>();
        xr.anchorMin = Vector2.zero;
        xr.anchorMax = Vector2.one;
        TextMeshProUGUI xTxt = xObj.AddComponent<TextMeshProUGUI>();
        xTxt.text = "✕";
        xTxt.alignment = TextAlignmentOptions.Center;
        xTxt.fontSize = 20;
        
        // Scroll View for Content
        GameObject sv = new GameObject("ScrollView");
        sv.transform.SetParent(panel.transform, false);
        RectTransform svr = sv.AddComponent<RectTransform>();
        svr.anchorMin = Vector2.zero;
        svr.anchorMax = Vector2.one;
        svr.offsetMin = new Vector2(20, 20);
        svr.offsetMax = new Vector2(-20, -80); // Below header
        
        ScrollRect sr = sv.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        
        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(sv.transform, false);
        RectTransform vpr = vp.AddComponent<RectTransform>();
        vpr.anchorMin = Vector2.zero;
        vpr.anchorMax = Vector2.one;
        Image vpImg = vp.AddComponent<Image>();
        vpImg.color = new Color(1,1,1,0.01f);
        Mask vpMask = vp.AddComponent<Mask>();
        vpMask.showMaskGraphic = false;
        sr.viewport = vpr;
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(vp.transform, false);
        RectTransform ctr = content.AddComponent<RectTransform>();
        ctr.anchorMin = new Vector2(0, 1);
        ctr.anchorMax = new Vector2(1, 1);
        ctr.pivot = new Vector2(0.5f, 1);
        
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 20;
        
        sr.content = ctr;
        
        // Content Text
        GameObject txtObj = new GameObject("BodyText");
        txtObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI bodyTxt = txtObj.AddComponent<TextMeshProUGUI>();
        bodyTxt.fontSize = 18;
        bodyTxt.color = new Color(0.9f, 0.9f, 0.9f);
        bodyTxt.enableWordWrapping = true;
        ui.contentText = bodyTxt;
        
        // Meta Info (Date/Source)
        GameObject metaObj = new GameObject("MetaText");
        metaObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI mTxt = metaObj.AddComponent<TextMeshProUGUI>();
        mTxt.fontSize = 14;
        mTxt.color = Color.gray;
        mTxt.fontStyle = FontStyles.Italic;
        ui.dateText = mTxt; // Reusing dateText for combined meta
        
        // Save Prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
        return prefab;
    }
}
