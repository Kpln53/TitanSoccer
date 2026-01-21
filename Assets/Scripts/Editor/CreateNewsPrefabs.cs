using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class CreateNewsPrefabs : MonoBehaviour
{
    public static void CreateNewsItemPrefab()
    {
        // 1. Root Object
        GameObject root = new GameObject("NewsItemPrefab");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(800, 100);
        
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        
        Button btn = root.AddComponent<Button>();
        btn.targetGraphic = bg;
        
        LayoutElement layout = root.AddComponent<LayoutElement>();
        layout.minHeight = 100;
        layout.preferredHeight = 100;
        layout.flexibleWidth = 1;
        
        NewsItemUI newsUI = root.AddComponent<NewsItemUI>();
        newsUI.backgroundImage = bg;
        newsUI.itemButton = btn;
        
        // 2. Type Icon
        GameObject iconObj = new GameObject("TypeIcon");
        iconObj.transform.SetParent(root.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(40, 0);
        iconRect.sizeDelta = new Vector2(50, 50);
        
        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.color = Color.white; // Placeholder
        newsUI.typeIcon = iconImg;
        
        // 3. Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(root.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(80, 0);
        titleRect.offsetMax = new Vector2(-20, -10);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 20;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.BottomLeft;
        titleText.color = Color.white;
        newsUI.titleText = titleText;
        
        // 4. Date & Source Container
        GameObject infoObj = new GameObject("Info");
        infoObj.transform.SetParent(root.transform, false);
        RectTransform infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0, 0);
        infoRect.anchorMax = new Vector2(1, 0.5f);
        infoRect.offsetMin = new Vector2(80, 10);
        infoRect.offsetMax = new Vector2(-20, 0);
        
        HorizontalLayoutGroup infoLayout = infoObj.AddComponent<HorizontalLayoutGroup>();
        infoLayout.childControlWidth = false;
        infoLayout.childForceExpandWidth = false;
        infoLayout.spacing = 20;
        
        // Date
        GameObject dateObj = new GameObject("Date");
        dateObj.transform.SetParent(infoObj.transform, false);
        TextMeshProUGUI dateText = dateObj.AddComponent<TextMeshProUGUI>();
        dateText.fontSize = 14;
        dateText.color = Color.gray;
        dateText.text = "01.01.2025";
        newsUI.dateText = dateText;
        
        // Source
        GameObject sourceObj = new GameObject("Source");
        sourceObj.transform.SetParent(infoObj.transform, false);
        TextMeshProUGUI sourceText = sourceObj.AddComponent<TextMeshProUGUI>();
        sourceText.fontSize = 14;
        sourceText.color = new Color(0.7f, 0.7f, 0.7f);
        sourceText.text = "Spor Haberleri";
        newsUI.sourceText = sourceText;
        
        // Save Prefab
        string path = "Assets/Prefabs/UI/NewsItemPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
        Debug.Log($"Prefab created at {path}");
    }
    
    public static void CreateNewsDetailPrefab()
    {
        // 1. Root (Background)
        GameObject root = new GameObject("NewsDetailPrefab");
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        
        Button bgBtn = root.AddComponent<Button>();
        bgBtn.targetGraphic = bg;
        
        NewsDetailUI detailUI = root.AddComponent<NewsDetailUI>();
        detailUI.backgroundButton = bgBtn;
        
        // 2. Content Panel
        GameObject panel = new GameObject("ContentPanel");
        panel.transform.SetParent(root.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(800, 600);
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        
        detailUI.contentPanel = panel;
        
        // 3. Header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 80);
        headerRect.anchoredPosition = Vector2.zero;
        
        Image headerBg = header.AddComponent<Image>();
        headerBg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        
        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(header.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-60, 0);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 24;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.MidlineLeft;
        titleText.color = Color.white;
        detailUI.titleText = titleText;
        
        // Close Button
        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(header.transform, false);
        RectTransform closeRect = closeObj.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.pivot = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(50, 50);
        closeRect.anchoredPosition = new Vector2(-10, -15);
        
        Image closeImg = closeObj.AddComponent<Image>();
        closeImg.color = Color.red;
        Button closeBtn = closeObj.AddComponent<Button>();
        closeBtn.targetGraphic = closeImg;
        detailUI.closeButton = closeBtn;
        
        GameObject closeTextObj = new GameObject("X");
        closeTextObj.transform.SetParent(closeObj.transform, false);
        RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.fontSize = 24;
        
        // 4. Content Scroll View
        GameObject scrollObj = new GameObject("ScrollView");
        scrollObj.transform.SetParent(panel.transform, false);
        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.offsetMin = new Vector2(20, 60); // Footer space
        scrollRect.offsetMax = new Vector2(-20, -90); // Header space
        
        ScrollRect sr = scrollObj.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        RectTransform viewRect = viewport.AddComponent<RectTransform>();
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        
        Image viewImg = viewport.AddComponent<Image>();
        viewImg.color = new Color(1,1,1,0.01f);
        Mask viewMask = viewport.AddComponent<Mask>();
        viewMask.showMaskGraphic = false;
        
        sr.viewport = viewRect;
        
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0); // Height will be controlled by ContentSizeFitter
        
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;
        
        sr.content = contentRect;
        
        // Content Text
        GameObject textObj = new GameObject("ContentText");
        textObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI contentText = textObj.AddComponent<TextMeshProUGUI>();
        contentText.fontSize = 18;
        contentText.color = new Color(0.9f, 0.9f, 0.9f);
        contentText.alignment = TextAlignmentOptions.TopLeft;
        contentText.enableWordWrapping = true;
        detailUI.contentText = contentText;
        
        // 5. Footer
        GameObject footer = new GameObject("Footer");
        footer.transform.SetParent(panel.transform, false);
        RectTransform footerRect = footer.AddComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0);
        footerRect.pivot = new Vector2(0.5f, 0);
        footerRect.sizeDelta = new Vector2(0, 50);
        footerRect.anchoredPosition = Vector2.zero;
        
        HorizontalLayoutGroup footerLayout = footer.AddComponent<HorizontalLayoutGroup>();
        footerLayout.padding = new RectOffset(20, 20, 0, 0);
        footerLayout.spacing = 30;
        footerLayout.childControlWidth = false;
        footerLayout.childForceExpandWidth = false;
        
        // Date
        GameObject fDateObj = new GameObject("Date");
        fDateObj.transform.SetParent(footer.transform, false);
        TextMeshProUGUI fDateText = fDateObj.AddComponent<TextMeshProUGUI>();
        fDateText.fontSize = 14;
        fDateText.color = Color.gray;
        detailUI.dateText = fDateText;
        
        // Source
        GameObject fSourceObj = new GameObject("Source");
        fSourceObj.transform.SetParent(footer.transform, false);
        TextMeshProUGUI fSourceText = fSourceObj.AddComponent<TextMeshProUGUI>();
        fSourceText.fontSize = 14;
        fSourceText.color = Color.gray;
        detailUI.sourceText = fSourceText;
        
        // Type
        GameObject fTypeObj = new GameObject("Type");
        fTypeObj.transform.SetParent(footer.transform, false);
        TextMeshProUGUI fTypeText = fTypeObj.AddComponent<TextMeshProUGUI>();
        fTypeText.fontSize = 14;
        fTypeText.color = Color.white;
        detailUI.typeText = fTypeText;
        
        // Save Prefab
        string path = "Assets/Prefabs/UI/NewsDetailPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        GameObject.DestroyImmediate(root);
        Debug.Log($"Prefab created at {path}");
    }
}
