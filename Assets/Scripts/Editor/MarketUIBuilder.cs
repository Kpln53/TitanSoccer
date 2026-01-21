using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MarketUIBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🛒 Build Premium Market UI")]
    public static void BuildMarketUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (canvasObj == null)
        {
            Debug.LogError("CareerHubCanvas not found!");
            return;
        }

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (mainPanel == null) return;

        // 1. Clean up old MarketPanel
        Transform oldPanel = mainPanel.Find("MarketPanel");
        if (oldPanel != null) DestroyImmediate(oldPanel.gameObject);

        // 2. Create MarketPanel
        GameObject marketPanel = new GameObject("MarketPanel");
        marketPanel.transform.SetParent(mainPanel, false);
        RectTransform panelRect = marketPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        MarketUI marketUI = marketPanel.AddComponent<MarketUI>();

        // Background (Dark Gradient-like)
        Image bg = marketPanel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.07f, 1f); // Very dark blue/black

        // --- TABS CONTAINER (Top) ---
        GameObject tabsContainer = new GameObject("TabsContainer");
        tabsContainer.transform.SetParent(marketPanel.transform, false);
        RectTransform tabsRect = tabsContainer.AddComponent<RectTransform>();
        tabsRect.anchorMin = new Vector2(0, 1);
        tabsRect.anchorMax = new Vector2(1, 1);
        tabsRect.pivot = new Vector2(0.5f, 1);
        tabsRect.sizeDelta = new Vector2(0, 70);
        tabsRect.anchoredPosition = new Vector2(0, -10);

        HorizontalLayoutGroup tabsLayout = tabsContainer.AddComponent<HorizontalLayoutGroup>();
        tabsLayout.childControlWidth = true;
        tabsLayout.childForceExpandWidth = true;
        tabsLayout.spacing = 10;
        tabsLayout.padding = new RectOffset(20, 20, 0, 0);

        // Create Tabs
        marketUI.featuredTab = CreateTabButton(tabsContainer.transform, "Öne Çıkanlar");
        marketUI.currencyTab = CreateTabButton(tabsContainer.transform, "Para & Altın");
        marketUI.packsTab = CreateTabButton(tabsContainer.transform, "Paketler");
        marketUI.equipmentTab = CreateTabButton(tabsContainer.transform, "Ekipman");

        // --- SCROLL VIEW ---
        GameObject scrollView = new GameObject("MarketScrollView");
        scrollView.transform.SetParent(marketPanel.transform, false);
        RectTransform svRect = scrollView.AddComponent<RectTransform>();
        svRect.anchorMin = Vector2.zero;
        svRect.anchorMax = Vector2.one;
        svRect.offsetMin = new Vector2(20, 20);
        svRect.offsetMax = new Vector2(-20, -90); // Below tabs

        ScrollRect sr = scrollView.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        marketUI.scrollRect = sr;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform vpRect = viewport.AddComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        Image vpImg = viewport.AddComponent<Image>();
        vpImg.color = new Color(1,1,1,0.01f);
        Mask vpMask = viewport.AddComponent<Mask>();
        vpMask.showMaskGraphic = false;
        sr.viewport = vpRect;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform cRect = content.AddComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0, 1);
        cRect.anchorMax = new Vector2(1, 1);
        cRect.pivot = new Vector2(0.5f, 1);

        GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(450, 600); // Large vertical cards
        grid.spacing = new Vector2(40, 40);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 2; // 2 Columns
        grid.childAlignment = TextAnchor.UpperCenter;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = cRect;
        marketUI.contentParent = content.transform;

        // --- PREFAB CREATION ---
        marketUI.itemPrefab = CreateMarketItemPrefab();

        Debug.Log("✅ Premium Market UI Built Successfully!");
    }

    private static Button CreateTabButton(Transform parent, string text)
    {
        GameObject btnObj = new GameObject(text + "Tab");
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark grey
        
        // Add Gold Border effect
        Outline outline = btnObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.8f, 0.6f, 0.2f, 0.5f);
        outline.effectDistance = new Vector2(2, -2);

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
        tmp.fontSize = 24; // Bigger font for tabs
        tmp.color = new Color(1f, 0.9f, 0.7f); // Light gold text
        tmp.fontStyle = FontStyles.Bold;

        return btn;
    }

    private static GameObject CreateMarketItemPrefab()
    {
        string path = "Assets/Prefabs/UI/PremiumMarketItem.prefab";
        
        // Root Card
        GameObject root = new GameObject("PremiumMarketItem");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(450, 600);

        // Glassy Background
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        
        // Gold Border
        GameObject border = new GameObject("Border");
        border.transform.SetParent(root.transform, false);
        RectTransform br = border.AddComponent<RectTransform>();
        br.anchorMin = Vector2.zero;
        br.anchorMax = Vector2.one;
        Image bImg = border.AddComponent<Image>();
        bImg.color = new Color(0,0,0,0);
        Outline outline = border.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.8f, 0.2f, 0.6f); // Gold glow
        outline.effectDistance = new Vector2(3, 3);

        MarketItemUI ui = root.AddComponent<MarketItemUI>();
        ui.backgroundImage = bg;

        // Title (Top)
        GameObject title = new GameObject("Title");
        title.transform.SetParent(root.transform, false);
        RectTransform tr = title.AddComponent<RectTransform>();
        tr.anchorMin = new Vector2(0, 0.85f);
        tr.anchorMax = new Vector2(1, 1);
        tr.offsetMin = new Vector2(20, 0);
        tr.offsetMax = new Vector2(-20, -10);
        
        TextMeshProUGUI titleTxt = title.AddComponent<TextMeshProUGUI>();
        titleTxt.text = "ITEM TITLE";
        titleTxt.fontSize = 32;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.alignment = TextAlignmentOptions.Center;
        titleTxt.color = new Color(1f, 0.95f, 0.8f); // Pale gold
        titleTxt.enableWordWrapping = true;
        ui.titleText = titleTxt;

        // Icon (Middle)
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(root.transform, false);
        RectTransform ir = icon.AddComponent<RectTransform>();
        ir.anchorMin = new Vector2(0.5f, 0.5f);
        ir.anchorMax = new Vector2(0.5f, 0.5f);
        ir.sizeDelta = new Vector2(200, 200);
        ir.anchoredPosition = new Vector2(0, 20);
        
        Image iconImg = icon.AddComponent<Image>();
        iconImg.color = Color.white; // Placeholder
        ui.iconImage = iconImg;

        // Description (Below Icon)
        GameObject desc = new GameObject("Description");
        desc.transform.SetParent(root.transform, false);
        RectTransform dr = desc.AddComponent<RectTransform>();
        dr.anchorMin = new Vector2(0, 0.25f);
        dr.anchorMax = new Vector2(1, 0.4f);
        dr.offsetMin = new Vector2(20, 0);
        dr.offsetMax = new Vector2(-20, 0);
        
        TextMeshProUGUI descTxt = desc.AddComponent<TextMeshProUGUI>();
        descTxt.text = "Description goes here...";
        descTxt.fontSize = 20;
        descTxt.alignment = TextAlignmentOptions.Top; // Changed from TopCenter
        descTxt.color = new Color(0.8f, 0.8f, 0.8f);
        ui.descriptionText = descTxt;

        // Buy Button (Bottom)
        GameObject btnObj = new GameObject("BuyButton");
        btnObj.transform.SetParent(root.transform, false);
        RectTransform btr = btnObj.AddComponent<RectTransform>();
        btr.anchorMin = new Vector2(0.1f, 0.05f);
        btr.anchorMax = new Vector2(0.9f, 0.18f);
        btr.offsetMin = Vector2.zero;
        btr.offsetMax = Vector2.zero;
        
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(1f, 0.8f, 0.2f); // Gold button
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        ui.buyButton = btn;

        GameObject priceObj = new GameObject("PriceText");
        priceObj.transform.SetParent(btnObj.transform, false);
        RectTransform pr = priceObj.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        TextMeshProUGUI priceTxt = priceObj.AddComponent<TextMeshProUGUI>();
        priceTxt.text = "₺29.99";
        priceTxt.fontSize = 28;
        priceTxt.fontStyle = FontStyles.Bold;
        priceTxt.alignment = TextAlignmentOptions.Center;
        priceTxt.color = new Color(0.1f, 0.1f, 0.1f); // Dark text on gold
        ui.priceText = priceTxt;

        // Badges (Ribbons)
        // Popular Badge (Top Left Corner)
        GameObject popBadge = new GameObject("PopularBadge");
        popBadge.transform.SetParent(root.transform, false);
        RectTransform pbr = popBadge.AddComponent<RectTransform>();
        pbr.anchorMin = new Vector2(0, 1);
        pbr.anchorMax = new Vector2(0, 1);
        pbr.pivot = new Vector2(0, 1);
        pbr.sizeDelta = new Vector2(120, 40);
        pbr.anchoredPosition = new Vector2(-10, 10);
        pbr.localRotation = Quaternion.Euler(0, 0, 45); // Rotated
        
        Image pbImg = popBadge.AddComponent<Image>();
        pbImg.color = new Color(0.9f, 0.2f, 0.2f); // Red ribbon
        
        GameObject pbTxtObj = new GameObject("Text");
        pbTxtObj.transform.SetParent(popBadge.transform, false);
        RectTransform pbtr = pbTxtObj.AddComponent<RectTransform>();
        pbtr.anchorMin = Vector2.zero;
        pbtr.anchorMax = Vector2.one;
        TextMeshProUGUI pbTxt = pbTxtObj.AddComponent<TextMeshProUGUI>();
        pbTxt.text = "POPÜLER";
        pbTxt.fontSize = 18;
        pbTxt.fontStyle = FontStyles.Bold;
        pbTxt.alignment = TextAlignmentOptions.Center;
        pbTxt.color = Color.white;
        
        ui.popularBadge = popBadge;
        popBadge.SetActive(false);

        // Best Value Badge (Top Right Corner)
        GameObject bestBadge = new GameObject("BestValueBadge");
        bestBadge.transform.SetParent(root.transform, false);
        RectTransform bvr = bestBadge.AddComponent<RectTransform>();
        bvr.anchorMin = new Vector2(1, 1);
        bvr.anchorMax = new Vector2(1, 1);
        bvr.pivot = new Vector2(1, 1);
        bvr.sizeDelta = new Vector2(120, 40);
        bvr.anchoredPosition = new Vector2(10, 10);
        bvr.localRotation = Quaternion.Euler(0, 0, -45); // Rotated
        
        Image bvImg = bestBadge.AddComponent<Image>();
        bvImg.color = new Color(0.2f, 0.8f, 0.2f); // Green ribbon
        
        GameObject bvTxtObj = new GameObject("Text");
        bvTxtObj.transform.SetParent(bestBadge.transform, false);
        RectTransform bvtr = bvTxtObj.AddComponent<RectTransform>();
        bvtr.anchorMin = Vector2.zero;
        bvtr.anchorMax = Vector2.one;
        TextMeshProUGUI bvTxt = bvTxtObj.AddComponent<TextMeshProUGUI>();
        bvTxt.text = "EN İYİ";
        bvTxt.fontSize = 18;
        bvTxt.fontStyle = FontStyles.Bold;
        bvTxt.alignment = TextAlignmentOptions.Center;
        bvTxt.color = Color.white;
        
        ui.bestValueBadge = bestBadge;
        bestBadge.SetActive(false);

        // Save Prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
        return prefab;
    }
}
