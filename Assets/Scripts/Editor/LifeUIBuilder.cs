using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class LifeUIBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/❤️ Build Life UI")]
    public static void BuildUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (!canvasObj) return;

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (!mainPanel) return;

        // 1. Clean old
        Transform old = mainPanel.Find("LifePanel");
        if (old) DestroyImmediate(old.gameObject);

        // 2. Create Panel
        GameObject panel = new GameObject("LifePanel");
        panel.transform.SetParent(mainPanel, false);
        RectTransform pr = panel.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        // Background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.05f, 1f);

        LifeUI uiScript = panel.AddComponent<LifeUI>();

        // --- HEADER ---
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0.1f, 0.92f);
        hr.anchorMax = new Vector2(0.9f, 0.98f);
        
        Image hImg = header.AddComponent<Image>();
        hImg.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
        Outline hOut = header.AddComponent<Outline>();
        hOut.effectColor = new Color(1f, 0.8f, 0.2f, 0.5f);

        TextMeshProUGUI hTxt = CreateText(header.transform, "İLİŞKİLER", 32, new Color(1f, 0.9f, 0.7f), FontStyles.Bold);

        // --- SCROLL VIEW (Relationships) ---
        GameObject sv = new GameObject("RelScrollView");
        sv.transform.SetParent(panel.transform, false);
        RectTransform svr = sv.AddComponent<RectTransform>();
        svr.anchorMin = new Vector2(0, 0.15f); // Bottom bar space
        svr.anchorMax = new Vector2(1, 0.9f);
        svr.offsetMin = new Vector2(20, 0);
        svr.offsetMax = new Vector2(-20, -10);

        ScrollRect sr = sv.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;

        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(sv.transform, false);
        RectTransform vpr = vp.AddComponent<RectTransform>();
        vpr.anchorMin = Vector2.zero;
        vpr.anchorMax = Vector2.one;
        vp.AddComponent<Image>().color = new Color(0,0,0,0.01f);
        vp.AddComponent<Mask>().showMaskGraphic = false;
        sr.viewport = vpr;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(vp.transform, false);
        RectTransform cr = content.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0, 1);
        cr.anchorMax = new Vector2(1, 1);
        cr.pivot = new Vector2(0.5f, 1);
        cr.offsetMin = Vector2.zero; // Genişliğin tam oturması için
        cr.offsetMax = Vector2.zero;
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 20;
        vlg.padding = new RectOffset(20, 20, 10, 10); // Kenar boşlukları
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;

        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = cr;
        uiScript.listContent = content.transform;

        // --- BOTTOM BAR (Profile, Shop, Gamble) ---
        GameObject bottomBar = new GameObject("BottomBar");
        bottomBar.transform.SetParent(panel.transform, false);
        RectTransform bbr = bottomBar.AddComponent<RectTransform>();
        bbr.anchorMin = new Vector2(0, 0);
        bbr.anchorMax = new Vector2(1, 0.12f);
        
        Image bbImg = bottomBar.AddComponent<Image>();
        bbImg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        Outline bbOut = bottomBar.AddComponent<Outline>();
        bbOut.effectColor = new Color(1f, 0.8f, 0.2f, 0.3f);
        bbOut.effectDistance = new Vector2(0, 2);

        HorizontalLayoutGroup bblg = bottomBar.AddComponent<HorizontalLayoutGroup>();
        bblg.spacing = 20;
        bblg.padding = new RectOffset(20, 20, 10, 10);
        bblg.childControlWidth = true;
        bblg.childForceExpandWidth = true;

        uiScript.profileButton = CreateBottomButton(bottomBar.transform, "PROFİL", Color.cyan);
        uiScript.shopButton = CreateBottomButton(bottomBar.transform, "ALIŞVERİŞ", Color.yellow);
        uiScript.gambleButton = CreateBottomButton(bottomBar.transform, "ŞANS OYUNLARI", Color.red);

        // --- PREFABS ---
        uiScript.itemPrefab = CreateRelationshipItemPrefab();
        uiScript.popupOptionPrefab = CreatePopupOptionPrefab();

        // --- POPUP ---
        CreateInteractionPopup(panel.transform, uiScript);

        // Connect to CareerHubUI
        CareerHubUI hubUI = canvasObj.GetComponent<CareerHubUI>();
        if (hubUI)
        {
            hubUI.lifePanel = panel;
            hubUI.lifeButton = canvasObj.transform.Find("MainPanel/BottomPanel/LifeButton")?.GetComponent<Button>();
            EditorUtility.SetDirty(hubUI);
        }

        Debug.Log("✅ Life UI Built Successfully!");
    }

    private static GameObject CreateRelationshipItemPrefab()
    {
        GameObject root = new GameObject("RelationshipItem");
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.sizeDelta = new Vector2(0, 140); // Height

        // Glassy BG
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
        Outline outl = root.AddComponent<Outline>();
        outl.effectColor = new Color(1f, 0.8f, 0.2f, 0.4f);

        RelationshipItemUI ui = root.AddComponent<RelationshipItemUI>();

        // Icon
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(root.transform, false);
        RectTransform ir = icon.AddComponent<RectTransform>();
        ir.anchorMin = new Vector2(0, 0.5f);
        ir.anchorMax = new Vector2(0, 0.5f);
        ir.sizeDelta = new Vector2(100, 100);
        ir.anchoredPosition = new Vector2(70, 0);
        Image iImg = icon.AddComponent<Image>();
        iImg.color = Color.gray; // Placeholder
        ui.iconImage = iImg;

        // Name (Top)
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(root.transform, false);
        RectTransform nr = nameObj.AddComponent<RectTransform>();
        nr.anchorMin = new Vector2(0, 1);
        nr.anchorMax = new Vector2(1, 1);
        nr.pivot = new Vector2(0.5f, 1);
        nr.anchoredPosition = new Vector2(0, -10);
        TextMeshProUGUI nTxt = CreateText(nameObj.transform, "1. KIZ ARKADAŞ (Elif)", 22, Color.white, FontStyles.Bold);
        ui.nameText = nTxt;

        // Progress Bar
        GameObject sliderObj = new GameObject("ProgressBar");
        sliderObj.transform.SetParent(root.transform, false);
        RectTransform sr = sliderObj.AddComponent<RectTransform>();
        sr.anchorMin = new Vector2(0.25f, 0.4f);
        sr.anchorMax = new Vector2(0.7f, 0.6f);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        
        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform, false);
        RectTransform bgr = bgObj.AddComponent<RectTransform>();
        bgr.anchorMin = Vector2.zero;
        bgr.anchorMax = Vector2.one;
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform far = fillArea.AddComponent<RectTransform>();
        far.anchorMin = Vector2.zero;
        far.anchorMax = Vector2.one;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fr = fill.AddComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        Image fImg = fill.AddComponent<Image>();
        fImg.color = Color.green;
        
        slider.targetGraphic = bgImg;
        slider.fillRect = fr;
        ui.progressBar = slider;
        ui.fillImage = fImg;

        // Status Text (Below Bar)
        GameObject statusObj = new GameObject("Status");
        statusObj.transform.SetParent(root.transform, false);
        RectTransform str = statusObj.AddComponent<RectTransform>();
        str.anchorMin = new Vector2(0.25f, 0.2f);
        str.anchorMax = new Vector2(0.7f, 0.4f);
        TextMeshProUGUI sTxt = CreateText(statusObj.transform, "Mükemmel: %98", 18, Color.green, FontStyles.Normal);
        ui.statusText = sTxt;

        // Action Button (Right)
        GameObject btnObj = new GameObject("ActionButton");
        btnObj.transform.SetParent(root.transform, false);
        RectTransform br = btnObj.AddComponent<RectTransform>();
        br.anchorMin = new Vector2(0.75f, 0.3f);
        br.anchorMax = new Vector2(0.95f, 0.7f);
        
        Image bImg = btnObj.AddComponent<Image>();
        bImg.color = new Color(1f, 0.8f, 0.2f); // Gold
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bImg;
        ui.actionButton = btn;

        GameObject btObj = new GameObject("Text");
        btObj.transform.SetParent(btnObj.transform, false);
        RectTransform btr = btObj.AddComponent<RectTransform>();
        btr.anchorMin = Vector2.zero;
        btr.anchorMax = Vector2.one;
        TextMeshProUGUI btTxt = CreateText(btObj.transform, "AKTİVİTE", 20, Color.black, FontStyles.Bold);

        string path = "Assets/Prefabs/UI/RelationshipItem.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreatePopupOptionPrefab()
    {
        GameObject root = new GameObject("PopupOption");
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.sizeDelta = new Vector2(0, 70);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f);
        Outline outl = root.AddComponent<Outline>();
        outl.effectColor = new Color(1f, 0.8f, 0.2f);

        Button btn = root.AddComponent<Button>();
        btn.targetGraphic = bg;

        TextMeshProUGUI txt = CreateText(root.transform, "Option", 24, Color.white, FontStyles.Normal);

        string path = "Assets/Prefabs/UI/LifeInteractionOption.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static void CreateInteractionPopup(Transform parent, LifeUI ui)
    {
        GameObject popup = new GameObject("InteractionPopup");
        popup.transform.SetParent(parent, false);
        RectTransform pr = popup.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        Image bg = popup.AddComponent<Image>();
        bg.color = new Color(0,0,0,0.9f);

        GameObject cont = new GameObject("Container");
        cont.transform.SetParent(popup.transform, false);
        RectTransform cr = cont.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.1f, 0.2f);
        cr.anchorMax = new Vector2(0.9f, 0.8f);
        
        Image cBg = cont.AddComponent<Image>();
        cBg.color = new Color(0.1f, 0.1f, 0.12f);
        Outline cOut = cont.AddComponent<Outline>();
        cOut.effectColor = new Color(1f, 0.8f, 0.2f);

        // Title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(cont.transform, false);
        RectTransform tr = title.AddComponent<RectTransform>();
        tr.anchorMin = new Vector2(0, 0.85f);
        tr.anchorMax = new Vector2(1, 1);
        ui.popupTitle = CreateText(title.transform, "Seçim Yap", 30, Color.white, FontStyles.Bold);

        // Options Container
        GameObject opts = new GameObject("Options");
        opts.transform.SetParent(cont.transform, false);
        RectTransform or = opts.AddComponent<RectTransform>();
        or.anchorMin = new Vector2(0.1f, 0.15f);
        or.anchorMax = new Vector2(0.9f, 0.8f);
        
        VerticalLayoutGroup vlg = opts.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 15;
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;
        ui.popupOptionsContainer = opts.transform;

        // Close Button
        GameObject close = new GameObject("CloseBtn");
        close.transform.SetParent(cont.transform, false);
        RectTransform clr = close.AddComponent<RectTransform>();
        clr.anchorMin = new Vector2(0.3f, 0.02f);
        clr.anchorMax = new Vector2(0.7f, 0.12f);
        
        Image clImg = close.AddComponent<Image>();
        clImg.color = Color.red;
        ui.popupCloseButton = close.AddComponent<Button>();
        CreateText(close.transform, "İPTAL", 24, Color.white, FontStyles.Bold);

        ui.popupPanel = popup;
    }

    private static Button CreateBottomButton(Transform parent, string text, Color color)
    {
        GameObject btnObj = new GameObject(text + "Btn");
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(color.r, color.g, color.b, 0.2f);
        Outline outl = btnObj.AddComponent<Outline>();
        outl.effectColor = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        TextMeshProUGUI txt = CreateText(btnObj.transform, text, 20, Color.white, FontStyles.Bold);
        return btn;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, int size, Color color, FontStyles style)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        RectTransform tr = obj.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }
}
