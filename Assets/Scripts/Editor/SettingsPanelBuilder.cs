using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;

public class SettingsPanelBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/⚙️ Build Settings Panel")]
    public static void BuildSettings()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (!canvas) return;

        // 1. Clean old
        Transform old = canvas.transform.Find("SettingsMenuPanel");
        if (old) DestroyImmediate(old.gameObject);

        // 2. Create Panel
        GameObject panelObj = new GameObject("SettingsMenuPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        RectTransform pr = panelObj.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;
        pr.anchoredPosition = new Vector2(1920, 0); // Off-screen right

        // Background
        Image bg = panelObj.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.95f);

        // 3. Header
        CreateText(panelObj.transform, "AYARLAR", 40, new Color(1f, 0.8f, 0.2f), new Vector2(0, 0.85f), new Vector2(1, 0.95f));

        // 4. Content Container
        GameObject content = new GameObject("Content");
        content.transform.SetParent(panelObj.transform, false);
        RectTransform cr = content.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.1f, 0.2f);
        cr.anchorMax = new Vector2(0.9f, 0.8f);
        
        VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 25;
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;

        SettingsMenuUI ui = panelObj.AddComponent<SettingsMenuUI>();

        // --- AUDIO SECTION ---
        CreateSectionHeader(content.transform, "SES AYARLARI");
        
        // Volume Slider
        GameObject volObj = CreateSlider(content.transform, "Ana Ses");
        ui.masterVolumeSlider = volObj.GetComponentInChildren<Slider>();
        ui.volumeValueText = volObj.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();

        // Toggles Row
        GameObject togglesRow = new GameObject("AudioToggles");
        togglesRow.transform.SetParent(content.transform, false);
        RectTransform trr = togglesRow.AddComponent<RectTransform>();
        trr.sizeDelta = new Vector2(0, 50);
        HorizontalLayoutGroup hlg = togglesRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 20;
        hlg.childControlWidth = true;
        hlg.childForceExpandWidth = true;

        ui.musicToggle = CreateToggle(togglesRow.transform, "Müzik").GetComponent<Toggle>();
        ui.sfxToggle = CreateToggle(togglesRow.transform, "Efektler").GetComponent<Toggle>();

        // --- GAME SECTION ---
        CreateSectionHeader(content.transform, "OYUN AYARLARI");

        // Language Dropdown
        ui.languageDropdown = CreateDropdown(content.transform, "Dil", new List<string> { "Türkçe", "English", "Español", "Deutsch" });

        // Notifications
        ui.notificationsToggle = CreateToggle(content.transform, "Bildirimler (Transfer, Maç Sonucu)").GetComponent<Toggle>();

        // --- CLOUD SECTION ---
        CreateSectionHeader(content.transform, "CLOUD SAVE");
        
        GameObject cloudRow = new GameObject("CloudRow");
        cloudRow.transform.SetParent(content.transform, false);
        RectTransform crr = cloudRow.AddComponent<RectTransform>();
        crr.sizeDelta = new Vector2(0, 50);
        HorizontalLayoutGroup chlg = cloudRow.AddComponent<HorizontalLayoutGroup>();
        chlg.childControlWidth = true;
        chlg.childForceExpandWidth = true;

        ui.cloudSaveToggle = CreateToggle(cloudRow.transform, "Cloud Senkronizasyon").GetComponent<Toggle>();
        
        // Status Text
        GameObject statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(cloudRow.transform, false);
        TextMeshProUGUI stTxt = statusObj.AddComponent<TextMeshProUGUI>();
        stTxt.text = "Durum: PASİF";
        stTxt.fontSize = 20;
        stTxt.alignment = TextAlignmentOptions.Right;
        stTxt.color = Color.gray;
        ui.cloudStatusText = stTxt;

        // 5. Back Button
        GameObject backBtnObj = new GameObject("BackButton");
        backBtnObj.transform.SetParent(panelObj.transform, false);
        RectTransform bbr = backBtnObj.AddComponent<RectTransform>();
        bbr.anchorMin = new Vector2(0.3f, 0.05f);
        bbr.anchorMax = new Vector2(0.7f, 0.12f);

        Image bbImg = backBtnObj.AddComponent<Image>();
        bbImg.color = new Color(1f, 1f, 1f, 0.1f);
        Outline bbOut = backBtnObj.AddComponent<Outline>();
        bbOut.effectColor = new Color(1f, 1f, 1f, 0.5f);

        Button backBtn = backBtnObj.AddComponent<Button>();
        backBtn.targetGraphic = bbImg;

        CreateText(backBtnObj.transform, "GERİ DÖN", 28, Color.white, Vector2.zero, Vector2.one);

        // 6. Link to MainMenuUI
        MainMenuUI mainUI = FindFirstObjectByType<MainMenuUI>();
        if (mainUI)
        {
            mainUI.settingsMenuPanel = pr;
            mainUI.settingsBackButton = backBtn;
            EditorUtility.SetDirty(mainUI);
        }

        Debug.Log("✅ Settings Panel Built!");
    }

    // --- HELPER METHODS ---

    private static void CreateSectionHeader(Transform parent, string title)
    {
        GameObject obj = new GameObject("Header_" + title);
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 40);
        
        TextMeshProUGUI txt = obj.AddComponent<TextMeshProUGUI>();
        txt.text = title;
        txt.fontSize = 24;
        txt.color = new Color(1f, 0.8f, 0.2f); // Gold
        txt.fontStyle = FontStyles.Bold;
        txt.alignment = TextAlignmentOptions.BottomLeft;
    }

    private static GameObject CreateSlider(Transform parent, string label)
    {
        GameObject container = new GameObject("Slider_" + label);
        container.transform.SetParent(parent, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 60);

        // Label
        CreateText(container.transform, label, 20, Color.white, new Vector2(0, 0.5f), new Vector2(0.4f, 1));

        // Value Text
        GameObject valObj = new GameObject("ValueText");
        valObj.transform.SetParent(container.transform, false);
        RectTransform vrt = valObj.AddComponent<RectTransform>();
        vrt.anchorMin = new Vector2(0.9f, 0.5f);
        vrt.anchorMax = new Vector2(1f, 1f);
        TextMeshProUGUI vTxt = valObj.AddComponent<TextMeshProUGUI>();
        vTxt.text = "100%";
        vTxt.fontSize = 20;
        vTxt.color = Color.white;
        vTxt.alignment = TextAlignmentOptions.Right;

        // Slider
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform, false);
        RectTransform srt = sliderObj.AddComponent<RectTransform>();
        srt.anchorMin = new Vector2(0, 0);
        srt.anchorMax = new Vector2(1, 0.4f);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        
        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgr = bg.AddComponent<RectTransform>();
        bgr.anchorMin = new Vector2(0, 0.25f);
        bgr.anchorMax = new Vector2(1, 0.75f);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform far = fillArea.AddComponent<RectTransform>();
        far.anchorMin = new Vector2(0, 0.25f);
        far.anchorMax = new Vector2(1, 0.75f);

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fr = fill.AddComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        Image fImg = fill.AddComponent<Image>();
        fImg.color = new Color(1f, 0.8f, 0.2f);

        // Handle Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform har = handleArea.AddComponent<RectTransform>();
        har.anchorMin = Vector2.zero;
        har.anchorMax = Vector2.one;

        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform hr = handle.AddComponent<RectTransform>();
        hr.sizeDelta = new Vector2(20, 0);
        Image hImg = handle.AddComponent<Image>();
        hImg.color = Color.white;

        slider.targetGraphic = hImg;
        slider.fillRect = fr;
        slider.handleRect = hr;
        slider.maxValue = 1f;
        slider.value = 1f;

        return container;
    }

    private static GameObject CreateToggle(Transform parent, string label)
    {
        GameObject toggleObj = new GameObject("Toggle_" + label);
        toggleObj.transform.SetParent(parent, false);
        RectTransform rt = toggleObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 40);

        Toggle toggle = toggleObj.AddComponent<Toggle>();

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(toggleObj.transform, false);
        RectTransform bgr = bg.AddComponent<RectTransform>();
        bgr.anchorMin = new Vector2(0, 0.1f);
        bgr.anchorMax = new Vector2(0, 0.9f);
        bgr.sizeDelta = new Vector2(32, 0); // Square box
        bgr.anchoredPosition = new Vector2(16, 0);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f);

        // Checkmark
        GameObject check = new GameObject("Checkmark");
        check.transform.SetParent(bg.transform, false);
        RectTransform cr = check.AddComponent<RectTransform>();
        cr.anchorMin = new Vector2(0.2f, 0.2f);
        cr.anchorMax = new Vector2(0.8f, 0.8f);
        Image cImg = check.AddComponent<Image>();
        cImg.color = new Color(1f, 0.8f, 0.2f);
        
        toggle.targetGraphic = bgImg;
        toggle.graphic = cImg;
        toggle.isOn = true;

        // Label
        CreateText(toggleObj.transform, label, 20, Color.white, new Vector2(0, 0), new Vector2(1, 1)).alignment = TextAlignmentOptions.Left;
        // Adjust label padding
        toggleObj.transform.Find("Text").GetComponent<RectTransform>().offsetMin = new Vector2(50, 0);

        return toggleObj;
    }

    private static TMP_Dropdown CreateDropdown(Transform parent, string label, List<string> options)
    {
        GameObject container = new GameObject("Dropdown_" + label);
        container.transform.SetParent(parent, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 50);

        // Label
        CreateText(container.transform, label, 20, Color.white, new Vector2(0, 0), new Vector2(0.3f, 1)).alignment = TextAlignmentOptions.Left;

        // Dropdown
        GameObject ddObj = new GameObject("Dropdown");
        ddObj.transform.SetParent(container.transform, false);
        RectTransform drt = ddObj.AddComponent<RectTransform>();
        drt.anchorMin = new Vector2(0.4f, 0.1f);
        drt.anchorMax = new Vector2(1f, 0.9f);
        
        Image ddBg = ddObj.AddComponent<Image>();
        ddBg.color = new Color(0.2f, 0.2f, 0.2f);
        
        TMP_Dropdown dropdown = ddObj.AddComponent<TMP_Dropdown>();
        dropdown.targetGraphic = ddBg;

        // Label inside dropdown
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(ddObj.transform, false);
        RectTransform lrt = labelObj.AddComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0.1f, 0);
        lrt.anchorMax = new Vector2(0.9f, 1);
        TextMeshProUGUI lTxt = labelObj.AddComponent<TextMeshProUGUI>();
        lTxt.text = options[0];
        lTxt.fontSize = 18;
        lTxt.color = Color.white;
        lTxt.alignment = TextAlignmentOptions.Left;
        dropdown.captionText = lTxt;

        // Template (Popup) - Simplified for builder
        // Note: Building a full functional dropdown template via script is complex.
        // We will add basic components so it doesn't crash, but it might need a prefab in a real scenario.
        // For now, we rely on Unity's default behavior if possible, or just basic setup.
        
        // Adding options
        dropdown.AddOptions(options);

        return dropdown;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, int size, Color color, Vector2 min, Vector2 max)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        
        return tmp;
    }
}
