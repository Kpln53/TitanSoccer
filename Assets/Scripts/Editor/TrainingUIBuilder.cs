using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class TrainingUIBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🏋️ Build Training UI")]
    public static void BuildUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (canvasObj == null)
        {
            Debug.LogError("CareerHubCanvas not found!");
            return;
        }

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (mainPanel == null) return;

        // 1. Clean up old
        Transform oldPanel = mainPanel.Find("TrainingPanel");
        if (oldPanel != null) DestroyImmediate(oldPanel.gameObject);

        // 2. Create Panel
        GameObject panel = new GameObject("TrainingPanel");
        panel.transform.SetParent(mainPanel, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Background (Dark Atmosphere)
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.1f, 1f);

        TrainingUI uiScript = panel.AddComponent<TrainingUI>();

        // --- HEADER ---
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0.1f, 0.85f);
        hr.anchorMax = new Vector2(0.9f, 0.95f);
        hr.offsetMin = Vector2.zero;
        hr.offsetMax = Vector2.zero;

        // Header Frame
        Image hImg = header.AddComponent<Image>();
        hImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        Outline hOut = header.AddComponent<Outline>();
        hOut.effectColor = new Color(0.8f, 0.7f, 0.4f, 0.6f); // Gold outline

        GameObject hTitle = new GameObject("Title");
        hTitle.transform.SetParent(header.transform, false);
        RectTransform htr = hTitle.AddComponent<RectTransform>();
        htr.anchorMin = Vector2.zero;
        htr.anchorMax = new Vector2(1, 1);
        
        TextMeshProUGUI hText = hTitle.AddComponent<TextMeshProUGUI>();
        hText.text = "ANTRENMAN SAHASI\n<size=60%>Kendini Geliştir, Efsane Ol!</size>";
        hText.alignment = TextAlignmentOptions.Center;
        hText.fontSize = 32;
        hText.color = new Color(0.9f, 0.9f, 0.9f);

        // --- CARDS CONTAINER ---
        GameObject container = new GameObject("CardsContainer");
        container.transform.SetParent(panel.transform, false);
        RectTransform cr = container.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0, 0.2f);
        cr.anchorMax = new Vector2(1, 0.8f);
        cr.offsetMin = new Vector2(20, 0);
        cr.offsetMax = new Vector2(-20, 0);

        HorizontalLayoutGroup hlg = container.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 30;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(20, 20, 20, 20);

        uiScript.cardsContainer = container.transform;

        // Create 3 Cards
        CreateTrainingCard(container.transform, "HÜCUM & BİTİRİCİLİK", "Enerji: -15 | Şut: +2");
        CreateTrainingCard(container.transform, "FİZİKSEL KONDİSYON", "Enerji: -20 | Güç: +3");
        CreateTrainingCard(container.transform, "TEKNİK & PAS", "Enerji: -10 | Pas: +1");

        // --- LIMIT TEXT ---
        GameObject limitObj = new GameObject("LimitText");
        limitObj.transform.SetParent(panel.transform, false);
        RectTransform lr = limitObj.AddComponent<RectTransform>();
        lr.anchorMin = new Vector2(0, 0.1f);
        lr.anchorMax = new Vector2(1, 0.15f);
        
        TextMeshProUGUI lText = limitObj.AddComponent<TextMeshProUGUI>();
        lText.text = "Kalan Hak: 3";
        lText.alignment = TextAlignmentOptions.Center;
        lText.fontSize = 24;
        lText.color = new Color(1f, 0.8f, 0.2f);
        uiScript.limitText = lText;

        // --- RESULT POPUP (Overlay) ---
        GameObject popup = new GameObject("ResultPopup");
        popup.transform.SetParent(panel.transform, false);
        RectTransform pr = popup.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        Image pBg = popup.AddComponent<Image>();
        pBg.color = new Color(0, 0, 0, 0.9f); // Dark overlay

        // Popup Content
        GameObject pContent = new GameObject("Content");
        pContent.transform.SetParent(popup.transform, false);
        RectTransform pcr = pContent.AddComponent<RectTransform>();
        pcr.anchorMin = new Vector2(0.5f, 0.5f);
        pcr.anchorMax = new Vector2(0.5f, 0.5f);
        pcr.sizeDelta = new Vector2(700, 800);

        Image pcBg = pContent.AddComponent<Image>();
        pcBg.color = new Color(0.1f, 0.1f, 0.12f);
        Outline pcOut = pContent.AddComponent<Outline>();
        pcOut.effectColor = new Color(1f, 0.8f, 0.2f);
        pcOut.effectDistance = new Vector2(2, 2);

        // Popup Title
        GameObject pt = new GameObject("Title");
        pt.transform.SetParent(pContent.transform, false);
        RectTransform ptr = pt.AddComponent<RectTransform>();
        ptr.anchorMin = new Vector2(0, 0.85f);
        ptr.anchorMax = new Vector2(1, 1);
        TextMeshProUGUI ptTxt = pt.AddComponent<TextMeshProUGUI>();
        ptTxt.text = "ANTRENMAN SONUCU";
        ptTxt.alignment = TextAlignmentOptions.Center;
        ptTxt.fontSize = 36;
        ptTxt.color = Color.white;
        ptTxt.fontStyle = FontStyles.Bold;
        uiScript.resultTitle = ptTxt;

        // Stats
        GameObject stats = new GameObject("Stats");
        stats.transform.SetParent(pContent.transform, false);
        RectTransform sr = stats.AddComponent<RectTransform>();
        sr.anchorMin = new Vector2(0, 0.3f);
        sr.anchorMax = new Vector2(1, 0.8f);
        
        VerticalLayoutGroup vlg = stats.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 20;

        uiScript.resultStatChange = CreateText(stats.transform, "Şut Gücü: +2", Color.green, 40);
        uiScript.resultEnergyChange = CreateText(stats.transform, "Enerji: -15", Color.red, 40);
        uiScript.resultNewEnergy = CreateText(stats.transform, "Yeni Enerji: %70", Color.white, 30);
        uiScript.resultNewMoral = CreateText(stats.transform, "Yeni Moral: %95", Color.white, 30);

        // Close Button
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(pContent.transform, false);
        RectTransform cbr = closeBtn.AddComponent<RectTransform>();
        cbr.anchorMin = new Vector2(0.2f, 0.05f);
        cbr.anchorMax = new Vector2(0.8f, 0.15f);
        
        Image cbImg = closeBtn.AddComponent<Image>();
        cbImg.color = new Color(1f, 0.8f, 0.2f); // Gold
        Button cb = closeBtn.AddComponent<Button>();
        cb.targetGraphic = cbImg;
        
        GameObject cbTxtObj = new GameObject("Text");
        cbTxtObj.transform.SetParent(closeBtn.transform, false);
        RectTransform cbtr = cbTxtObj.AddComponent<RectTransform>();
        cbtr.anchorMin = Vector2.zero;
        cbtr.anchorMax = Vector2.one;
        TextMeshProUGUI cbTxt = cbTxtObj.AddComponent<TextMeshProUGUI>();
        cbTxt.text = "TAMAM";
        cbTxt.alignment = TextAlignmentOptions.Center;
        cbTxt.fontSize = 32;
        cbTxt.color = Color.black;
        cbTxt.fontStyle = FontStyles.Bold;

        uiScript.resultCloseButton = cb;
        uiScript.resultPanel = popup;

        // Hide popup initially
        popup.SetActive(false);

        // Connect to CareerHubUI
        CareerHubUI hubUI = canvasObj.GetComponent<CareerHubUI>();
        if (hubUI)
        {
            hubUI.trainingPanel = panel;
            hubUI.trainingButton = canvasObj.transform.Find("MainPanel/BottomPanel/TrainingButton")?.GetComponent<Button>();
            EditorUtility.SetDirty(hubUI);
        }

        Debug.Log("✅ Training UI Built Successfully!");
    }

    private static void CreateTrainingCard(Transform parent, string title, string desc)
    {
        GameObject card = new GameObject("Card");
        card.transform.SetParent(parent, false);
        
        Image bg = card.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.18f, 0.8f); // Glassy dark
        
        Outline outl = card.AddComponent<Outline>();
        outl.effectColor = new Color(1f, 0.8f, 0.2f, 0.4f); // Gold glow
        outl.effectDistance = new Vector2(2, 2);

        // Content Container
        GameObject content = new GameObject("Content");
        content.transform.SetParent(card.transform, false);
        RectTransform cr = content.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0, 0.25f);
        cr.anchorMax = new Vector2(1, 1);
        
        // Icon (Placeholder)
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(content.transform, false);
        RectTransform ir = icon.AddComponent<RectTransform>();
        ir.anchorMin = new Vector2(0.5f, 0.6f);
        ir.anchorMax = new Vector2(0.5f, 0.6f);
        ir.sizeDelta = new Vector2(150, 150);
        Image iImg = icon.AddComponent<Image>();
        iImg.color = new Color(1f, 0.8f, 0.2f); // Gold icon placeholder

        // Title
        GameObject tObj = new GameObject("Title");
        tObj.transform.SetParent(content.transform, false);
        RectTransform tr = tObj.AddComponent<RectTransform>();
        tr.anchorMin = new Vector2(0, 0.2f);
        tr.anchorMax = new Vector2(1, 0.4f);
        TextMeshProUGUI tTxt = tObj.AddComponent<TextMeshProUGUI>();
        tTxt.text = title;
        tTxt.alignment = TextAlignmentOptions.Center;
        tTxt.fontSize = 24;
        tTxt.fontStyle = FontStyles.Bold;
        tTxt.color = Color.white;
        tTxt.enableWordWrapping = true;

        // Description
        GameObject dObj = new GameObject("Description");
        dObj.transform.SetParent(content.transform, false);
        RectTransform dr = dObj.AddComponent<RectTransform>();
        dr.anchorMin = new Vector2(0, 0);
        dr.anchorMax = new Vector2(1, 0.2f);
        TextMeshProUGUI dTxt = dObj.AddComponent<TextMeshProUGUI>();
        dTxt.text = desc;
        dTxt.alignment = TextAlignmentOptions.Center;
        dTxt.fontSize = 18;
        dTxt.color = new Color(0.8f, 0.8f, 0.8f);

        // Start Button
        GameObject btnObj = new GameObject("StartButton");
        btnObj.transform.SetParent(card.transform, false);
        RectTransform br = btnObj.AddComponent<RectTransform>();
        br.anchorMin = new Vector2(0.1f, 0.05f);
        br.anchorMax = new Vector2(0.9f, 0.2f);
        
        Image bImg = btnObj.AddComponent<Image>();
        bImg.color = new Color(1f, 0.8f, 0.2f, 0.2f); // Transparent gold
        Outline bOut = btnObj.AddComponent<Outline>();
        bOut.effectColor = new Color(1f, 0.8f, 0.2f);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = bImg;

        GameObject btObj = new GameObject("Text");
        btObj.transform.SetParent(btnObj.transform, false);
        RectTransform btr = btObj.AddComponent<RectTransform>();
        btr.anchorMin = Vector2.zero;
        btr.anchorMax = Vector2.one;
        TextMeshProUGUI btTxt = btObj.AddComponent<TextMeshProUGUI>();
        btTxt.text = "BAŞLA";
        btTxt.alignment = TextAlignmentOptions.Center;
        btTxt.fontSize = 24;
        btTxt.color = new Color(1f, 0.8f, 0.2f);
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, Color color, int size)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = color;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }
}
