using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class StandingsUIBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/📊 Build Standings UI")]
    public static void BuildUI()
    {
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (!canvasObj) return;

        Transform mainPanel = canvasObj.transform.Find("MainPanel/ContentArea");
        if (!mainPanel) return;

        // 1. Clean old
        Transform old = mainPanel.Find("StandingsPanel");
        if (old) DestroyImmediate(old.gameObject);

        // 2. Create Panel
        GameObject panel = new GameObject("StandingsPanel");
        panel.transform.SetParent(mainPanel, false);
        RectTransform pr = panel.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.08f, 1f);

        StandingsUI uiScript = panel.AddComponent<StandingsUI>();

        // --- HEADER ---
        GameObject header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        RectTransform hr = header.AddComponent<RectTransform>();
        hr.anchorMin = new Vector2(0, 0.9f);
        hr.anchorMax = new Vector2(1, 1);
        
        TextMeshProUGUI hTxt = CreateText(header.transform, "PUAN DURUMU", 32, Color.white);
        uiScript.leagueNameText = hTxt;

        // --- STANDINGS LIST ---
        GameObject sScroll = new GameObject("StandingsScroll");
        sScroll.transform.SetParent(panel.transform, false);
        RectTransform ssr = sScroll.AddComponent<RectTransform>();
        ssr.anchorMin = new Vector2(0, 0.4f); // Üst kısım
        ssr.anchorMax = new Vector2(1, 0.9f);
        
        ScrollRect sr = sScroll.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        
        GameObject vp = new GameObject("Viewport");
        vp.transform.SetParent(sScroll.transform, false);
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
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 5;
        vlg.childControlHeight = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;
        
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        sr.content = cr;
        uiScript.standingsListParent = content.transform;

        // --- FIXTURE AREA ---
        GameObject fixtureArea = new GameObject("FixtureArea");
        fixtureArea.transform.SetParent(panel.transform, false);
        RectTransform fr = fixtureArea.AddComponent<RectTransform>();
        fr.anchorMin = new Vector2(0, 0);
        fr.anchorMax = new Vector2(1, 0.4f); // Alt kısım

        Image fBg = fixtureArea.AddComponent<Image>();
        fBg.color = new Color(0.1f, 0.1f, 0.12f);

        // Fixture Header (Week Navigation)
        GameObject fHeader = new GameObject("FixtureHeader");
        fHeader.transform.SetParent(fixtureArea.transform, false);
        RectTransform fhr = fHeader.AddComponent<RectTransform>();
        fhr.anchorMin = new Vector2(0, 0.85f);
        fhr.anchorMax = new Vector2(1, 1);

        uiScript.prevWeekButton = CreateButton(fHeader.transform, "<", new Vector2(0.1f, 0.5f));
        uiScript.nextWeekButton = CreateButton(fHeader.transform, ">", new Vector2(0.9f, 0.5f));
        uiScript.currentWeekText = CreateText(fHeader.transform, "1. HAFTA", 24, Color.yellow);

        // Fixture List
        GameObject fList = new GameObject("FixtureList");
        fList.transform.SetParent(fixtureArea.transform, false);
        RectTransform flr = fList.AddComponent<RectTransform>();
        flr.anchorMin = new Vector2(0.05f, 0.05f);
        flr.anchorMax = new Vector2(0.95f, 0.85f);
        
        VerticalLayoutGroup flg = fList.AddComponent<VerticalLayoutGroup>();
        flg.spacing = 10;
        flg.childControlHeight = false;
        flg.childForceExpandHeight = false;
        flg.childControlWidth = true;
        flg.childForceExpandWidth = true;

        uiScript.fixtureListParent = fList.transform;

        // Connect to CareerHubUI
        CareerHubUI hubUI = canvasObj.GetComponent<CareerHubUI>();
        if (hubUI)
        {
            hubUI.standingsPanel = panel;
            hubUI.standingsButton = canvasObj.transform.Find("MainPanel/ContentArea/HomePanel/StandingsButton")?.GetComponent<Button>();
            EditorUtility.SetDirty(hubUI);
        }

        Debug.Log("✅ Standings UI Built!");
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, int size, Color color)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string text, Vector2 pos)
    {
        GameObject obj = new GameObject("Button");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = pos;
        rt.anchorMax = pos;
        rt.sizeDelta = new Vector2(50, 50);
        
        Image img = obj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.1f);
        
        Button btn = obj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        CreateText(obj.transform, text, 30, Color.white);
        return btn;
    }
}
