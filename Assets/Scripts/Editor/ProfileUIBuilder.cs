using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ProfileUIBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/TitanSoccer/Build Profile UI")]
    public static void BuildProfileUI()
    {
        // 1. Find ContentArea
        GameObject contentArea = GameObject.Find("CareerHubCanvas/MainPanel/ContentArea");
        if (contentArea == null)
        {
            Debug.LogError("ContentArea not found!");
            return;
        }

        // 2. Create ProfilePanel
        GameObject profilePanel = CreatePanel("ProfilePanel", contentArea.transform);
        
        // 3. Header
        CreateHeader(profilePanel.transform);

        // 4. Tabs
        GameObject tabsContainer = CreateTabs(profilePanel.transform);

        // 5. Content Panels
        GameObject overviewPanel = CreateOverviewPanel(profilePanel.transform);
        GameObject careerPanel = CreateCareerPanel(profilePanel.transform);
        GameObject contractPanel = CreateContractPanel(profilePanel.transform);

        // 6. Setup Main Script
        ProfileUI profileUI = profilePanel.AddComponent<ProfileUI>();
        
        // Link Tabs
        profileUI.GetType().GetField("overviewButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, tabsContainer.transform.GetChild(0).GetComponent<Button>());
        profileUI.GetType().GetField("careerButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, tabsContainer.transform.GetChild(1).GetComponent<Button>());
        profileUI.GetType().GetField("contractButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, tabsContainer.transform.GetChild(2).GetComponent<Button>());

        // Link Panels
        profileUI.GetType().GetField("overviewPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, overviewPanel);
        profileUI.GetType().GetField("careerPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, careerPanel);
        profileUI.GetType().GetField("contractPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, contractPanel);

        // Link Sub-Scripts
        profileUI.GetType().GetField("overviewUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, overviewPanel.GetComponent<ProfileOverviewUI>());
        profileUI.GetType().GetField("careerUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, careerPanel.GetComponent<ProfileCareerUI>());
        profileUI.GetType().GetField("contractUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(profileUI, contractPanel.GetComponent<ProfileContractUI>());

        Debug.Log("Profile UI Built Successfully!");
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // Background (Dark overlay)
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);

        return panel;
    }

    private static void CreateHeader(Transform parent)
    {
        GameObject header = new GameObject("Header");
        header.transform.SetParent(parent, false);
        RectTransform rect = header.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(0, 80);
        rect.anchoredPosition = new Vector2(0, -20);

        TextMeshProUGUI text = header.AddComponent<TextMeshProUGUI>();
        text.text = "PROFIL";
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.4f, 1f, 0.4f); // Neon Green
        text.fontStyle = FontStyles.Bold;
    }

    private static GameObject CreateTabs(Transform parent)
    {
        GameObject container = new GameObject("Tabs");
        container.transform.SetParent(parent, false);
        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(-40, 60); // Padding
        rect.anchoredPosition = new Vector2(0, -100);

        HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;

        CreateTabButton("GENEL BAKIŞ", container.transform);
        CreateTabButton("KARIYER", container.transform);
        CreateTabButton("SÖZLEŞME", container.transform);

        return container;
    }

    private static void CreateTabButton(string text, Transform parent)
    {
        GameObject btnObj = new GameObject(text + "Btn");
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.1f, 0.3f, 0.1f); // Dark Green

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
    }

    private static GameObject CreateOverviewPanel(Transform parent)
    {
        GameObject panel = new GameObject("OverviewPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -180); // Below tabs

        // Add Script
        ProfileOverviewUI ui = panel.AddComponent<ProfileOverviewUI>();

        // Frame
        Image frame = panel.AddComponent<Image>();
        frame.color = new Color(0, 0, 0, 0.5f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 1f, 0.4f);
        outline.effectDistance = new Vector2(2, 2);

        // Left Side (Photo & Info)
        GameObject leftSide = new GameObject("LeftSide");
        leftSide.transform.SetParent(panel.transform, false);
        RectTransform leftRect = leftSide.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0, 0);
        leftRect.anchorMax = new Vector2(0.4f, 1);
        leftRect.offsetMin = new Vector2(20, 20);
        leftRect.offsetMax = new Vector2(-10, -20);

        // Photo
        GameObject photoObj = new GameObject("Photo");
        photoObj.transform.SetParent(leftSide.transform, false);
        RectTransform photoRect = photoObj.AddComponent<RectTransform>();
        photoRect.anchorMin = new Vector2(0.5f, 1);
        photoRect.anchorMax = new Vector2(0.5f, 1);
        photoRect.sizeDelta = new Vector2(200, 200);
        photoRect.anchoredPosition = new Vector2(0, -120);
        Image photo = photoObj.AddComponent<Image>();
        photo.color = Color.gray;
        
        // Link Photo
        ui.GetType().GetField("playerPhoto", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, photo);

        // Name
        GameObject nameObj = CreateText("Name", "ARDA GÜLER", 32, leftSide.transform, new Vector2(0, -240));
        ui.GetType().GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, nameObj.GetComponent<TextMeshProUGUI>());

        // Age
        GameObject ageObj = CreateText("Age", "19", 28, leftSide.transform, new Vector2(0, -280));
        ui.GetType().GetField("ageText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, ageObj.GetComponent<TextMeshProUGUI>());

        // Nationality
        GameObject natObj = CreateText("Nationality", "Türkiye", 24, leftSide.transform, new Vector2(0, -320));
        ui.GetType().GetField("nationalityText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, natObj.GetComponent<TextMeshProUGUI>());

        // Position
        GameObject posObj = CreateText("Position", "OS (Ofansif Orta Saha)", 24, leftSide.transform, new Vector2(0, -360));
        ui.GetType().GetField("positionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, posObj.GetComponent<TextMeshProUGUI>());

        // Overall
        GameObject ovrObj = CreateText("Overall", "82 (Genel)", 36, leftSide.transform, new Vector2(0, -410));
        ui.GetType().GetField("overallText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, ovrObj.GetComponent<TextMeshProUGUI>());


        // Right Side (Stats)
        GameObject rightSide = new GameObject("RightSide");
        rightSide.transform.SetParent(panel.transform, false);
        RectTransform rightRect = rightSide.AddComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(0.4f, 0);
        rightRect.anchorMax = new Vector2(1, 1);
        rightRect.offsetMin = new Vector2(10, 20);
        rightRect.offsetMax = new Vector2(-20, -20);

        VerticalLayoutGroup statsLayout = rightSide.AddComponent<VerticalLayoutGroup>();
        statsLayout.spacing = 15;
        statsLayout.padding = new RectOffset(20, 20, 20, 20);
        statsLayout.childControlHeight = false;
        statsLayout.childForceExpandHeight = false;

        // Link Stats Container
        ui.GetType().GetField("statsContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, rightSide.transform);

        // Create Stat Prefab (Hidden)
        GameObject statPrefab = CreateStatPrefab(panel.transform);
        statPrefab.SetActive(false);
        ui.GetType().GetField("statItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, statPrefab);

        return panel;
    }

    private static GameObject CreateStatPrefab(Transform parent)
    {
        GameObject prefab = new GameObject("StatItem");
        prefab.transform.SetParent(parent, false);
        RectTransform rect = prefab.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);

        TextMeshProUGUI text = prefab.AddComponent<TextMeshProUGUI>();
        text.fontSize = 28;
        text.color = new Color(0.6f, 1f, 0.6f); // Light Green
        text.alignment = TextAlignmentOptions.Left;

        return prefab;
    }

    private static GameObject CreateText(string name, string content, int fontSize, Transform parent, Vector2 pos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(0, fontSize + 10);
        rect.anchoredPosition = pos;

        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        return obj;
    }

    private static GameObject CreateCareerPanel(Transform parent)
    {
        GameObject panel = new GameObject("CareerPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -180);
        panel.SetActive(false);

        ProfileCareerUI ui = panel.AddComponent<ProfileCareerUI>();

        // Frame
        Image frame = panel.AddComponent<Image>();
        frame.color = new Color(0, 0, 0, 0.5f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 1f, 0.4f);
        outline.effectDistance = new Vector2(2, 2);

        // Title
        CreateText("Title", "KARIYER İSTATİSTİKLERİ", 32, panel.transform, new Vector2(0, -30));

        // Headers
        GameObject headers = new GameObject("Headers");
        headers.transform.SetParent(panel.transform, false);
        RectTransform hRect = headers.AddComponent<RectTransform>();
        hRect.anchorMin = new Vector2(0, 1);
        hRect.anchorMax = new Vector2(1, 1);
        hRect.sizeDelta = new Vector2(0, 40);
        hRect.anchoredPosition = new Vector2(0, -80);
        
        HorizontalLayoutGroup hLayout = headers.AddComponent<HorizontalLayoutGroup>();
        hLayout.padding = new RectOffset(10, 10, 0, 0);
        hLayout.childControlWidth = true;
        hLayout.childForceExpandWidth = true;

        CreateHeaderText("SEZON", headers.transform);
        CreateHeaderText("TAKIM", headers.transform);
        CreateHeaderText("MAÇ", headers.transform);
        CreateHeaderText("GOL", headers.transform);
        CreateHeaderText("ASİST", headers.transform);
        CreateHeaderText("ORT. REY.", headers.transform);

        // Scroll View
        GameObject scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(panel.transform, false);
        RectTransform sRect = scrollObj.AddComponent<RectTransform>();
        sRect.anchorMin = new Vector2(0, 0);
        sRect.anchorMax = new Vector2(1, 1);
        sRect.offsetMin = new Vector2(10, 10);
        sRect.offsetMax = new Vector2(-10, -130);

        ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        RectTransform vRect = viewport.AddComponent<RectTransform>();
        vRect.anchorMin = Vector2.zero;
        vRect.anchorMax = Vector2.one;
        Image mask = viewport.AddComponent<Image>();
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        scroll.viewport = vRect;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform cRect = content.AddComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0, 1);
        cRect.anchorMax = new Vector2(1, 1);
        cRect.pivot = new Vector2(0.5f, 1);
        scroll.content = cRect;

        VerticalLayoutGroup vLayout = content.AddComponent<VerticalLayoutGroup>();
        vLayout.childControlHeight = false;
        vLayout.childForceExpandHeight = false;
        vLayout.spacing = 5;
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Link Content
        ui.GetType().GetField("contentContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, content.transform);

        // Create Item Prefab (Hidden)
        GameObject itemPrefab = CreateCareerItemPrefab(panel.transform);
        itemPrefab.SetActive(false);
        ui.GetType().GetField("historyItemPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, itemPrefab);

        return panel;
    }

    private static void CreateHeaderText(string text, Transform parent)
    {
        GameObject obj = new GameObject("Header_" + text);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        
        // Background for header
        Image bg = obj.AddComponent<Image>();
        bg.color = new Color(0.4f, 1f, 0.4f); // Green bg
    }

    private static GameObject CreateCareerItemPrefab(Transform parent)
    {
        GameObject prefab = new GameObject("CareerItem");
        prefab.transform.SetParent(parent, false);
        RectTransform rect = prefab.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);

        HorizontalLayoutGroup layout = prefab.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        layout.padding = new RectOffset(10, 10, 0, 0);

        CareerHistoryItem script = prefab.AddComponent<CareerHistoryItem>();

        // Create 6 text fields
        script.GetType().GetField("seasonText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));
        script.GetType().GetField("teamText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));
        script.GetType().GetField("matchesText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));
        script.GetType().GetField("goalsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));
        script.GetType().GetField("assistsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));
        script.GetType().GetField("ratingText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(script, CreateCellText(prefab.transform));

        return prefab;
    }

    private static TextMeshProUGUI CreateCellText(Transform parent)
    {
        GameObject obj = new GameObject("Cell");
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 20;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static GameObject CreateContractPanel(Transform parent)
    {
        GameObject panel = new GameObject("ContractPanel");
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -180);
        panel.SetActive(false);

        ProfileContractUI ui = panel.AddComponent<ProfileContractUI>();

        // Frame
        Image frame = panel.AddComponent<Image>();
        frame.color = new Color(0, 0, 0, 0.5f);
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 1f, 0.4f);
        outline.effectDistance = new Vector2(2, 2);

        // Title
        CreateText("Title", "MEVCUT SÖZLEŞME", 32, panel.transform, new Vector2(0, -30));

        // Content Container
        GameObject content = new GameObject("Content");
        content.transform.SetParent(panel.transform, false);
        RectTransform cRect = content.AddComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0, 0);
        cRect.anchorMax = new Vector2(1, 1);
        cRect.offsetMin = new Vector2(20, 20);
        cRect.offsetMax = new Vector2(-20, -80);

        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;

        // Fields
        ui.GetType().GetField("teamText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, CreateDetailText(content.transform));
        ui.GetType().GetField("durationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, CreateDetailText(content.transform));
        ui.GetType().GetField("wageText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, CreateDetailText(content.transform));
        ui.GetType().GetField("bonusesText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, CreateDetailText(content.transform));
        ui.GetType().GetField("releaseClauseText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ui, CreateDetailText(content.transform));

        return panel;
    }

    private static TextMeshProUGUI CreateDetailText(Transform parent)
    {
        GameObject obj = new GameObject("Detail");
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);
        
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        return tmp;
    }
#endif
}
