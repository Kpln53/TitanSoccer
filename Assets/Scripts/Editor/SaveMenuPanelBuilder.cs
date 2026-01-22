using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;

public class SaveMenuPanelBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/✨ Build Complete Save Menu")]
    public static void BuildComplete()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (!canvas) return;

        // 1. Clean old panel
        Transform oldPanel = canvas.transform.Find("SaveMenuPanel");
        if (oldPanel) DestroyImmediate(oldPanel.gameObject);

        // 2. Create Panel
        GameObject panelObj = new GameObject("SaveMenuPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        RectTransform pr = panelObj.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;
        pr.anchoredPosition = new Vector2(1920, 0); // Start off-screen

        // Background
        Image bg = panelObj.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.95f);

        // 3. Header
        CreateText(panelObj.transform, "KAYIT YUVALARI", 40, new Color(1f, 0.8f, 0.2f), new Vector2(0, 0.85f), new Vector2(1, 0.95f));

        // 4. Slots Container
        GameObject slotsCont = new GameObject("SlotsContainer");
        slotsCont.transform.SetParent(panelObj.transform, false);
        RectTransform scr = slotsCont.AddComponent<RectTransform>();
        scr.anchorMin = new Vector2(0.1f, 0.2f);
        scr.anchorMax = new Vector2(0.9f, 0.8f);
        
        VerticalLayoutGroup vlg = slotsCont.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 30;
        vlg.childControlHeight = false;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandWidth = true;

        // 5. Create Slots & Link Script
        SaveMenuUI menuUI = panelObj.AddComponent<SaveMenuUI>();
        menuUI.slotsContainer = slotsCont.transform;
        menuUI.saveSlots = new List<SaveSlotUI>();

        for (int i = 0; i < 3; i++)
        {
            SaveSlotUI slotUI = CreateSlot(slotsCont.transform, i);
            menuUI.saveSlots.Add(slotUI);
        }

        // 6. Back Button
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

        // 7. Link to MainMenuUI
        MainMenuUI mainUI = FindFirstObjectByType<MainMenuUI>();
        if (mainUI)
        {
            mainUI.saveMenuPanel = pr;
            mainUI.saveMenuBackButton = backBtn;
            EditorUtility.SetDirty(mainUI);
        }

        Debug.Log("✅ Save Menu Panel Built Completely!");
    }

    private static SaveSlotUI CreateSlot(Transform parent, int index)
    {
        GameObject slotObj = new GameObject($"Slot{index + 1}");
        slotObj.transform.SetParent(parent, false);
        
        RectTransform rt = slotObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 180);

        Image bg = slotObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        Outline outline = slotObj.AddComponent<Outline>();
        outline.effectDistance = new Vector2(2, -2);
        
        Button btn = slotObj.AddComponent<Button>();
        btn.targetGraphic = bg;

        SaveSlotUI ui = slotObj.AddComponent<SaveSlotUI>();
        ui.slotButton = btn;
        ui.outline = outline;

        // --- EMPTY STATE ---
        GameObject emptyObj = new GameObject("EmptyState");
        emptyObj.transform.SetParent(slotObj.transform, false);
        RectTransform ert = emptyObj.AddComponent<RectTransform>();
        ert.anchorMin = Vector2.zero;
        ert.anchorMax = Vector2.one;
        ert.offsetMin = Vector2.zero;
        ert.offsetMax = Vector2.zero;
        ui.emptyStateObject = emptyObj;

        // Plus Icon
        GameObject plusObj = new GameObject("Plus");
        plusObj.transform.SetParent(emptyObj.transform, false);
        RectTransform prt = plusObj.AddComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.05f, 0.5f);
        prt.anchorMax = new Vector2(0.05f, 0.5f);
        prt.sizeDelta = new Vector2(60, 60);
        prt.anchoredPosition = new Vector2(50, 0);
        
        TextMeshProUGUI plusTxt = plusObj.AddComponent<TextMeshProUGUI>();
        plusTxt.text = "+";
        plusTxt.fontSize = 60;
        plusTxt.alignment = TextAlignmentOptions.Center;
        plusTxt.color = new Color(1f, 0.8f, 0.2f);

        // Texts
        GameObject eTxtCont = new GameObject("Texts");
        eTxtCont.transform.SetParent(emptyObj.transform, false);
        RectTransform etrt = eTxtCont.AddComponent<RectTransform>();
        etrt.anchorMin = new Vector2(0.2f, 0);
        etrt.anchorMax = new Vector2(1, 1);
        etrt.offsetMin = Vector2.zero;
        etrt.offsetMax = Vector2.zero;

        ui.emptyTitleText = CreateText(eTxtCont.transform, "YENİ KARİYER", 32, Color.white, new Vector2(0, 0.5f), new Vector2(1, 0.9f));
        ui.emptyTitleText.alignment = TextAlignmentOptions.Left;
        
        ui.emptySubtitleText = CreateText(eTxtCont.transform, "Boş Slot", 20, Color.gray, new Vector2(0, 0.1f), new Vector2(1, 0.5f));
        ui.emptySubtitleText.alignment = TextAlignmentOptions.Left;

        // --- FILLED STATE ---
        GameObject filledObj = new GameObject("FilledState");
        filledObj.transform.SetParent(slotObj.transform, false);
        RectTransform frt = filledObj.AddComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = Vector2.zero;
        frt.offsetMax = Vector2.zero;
        ui.filledStateObject = filledObj;
        filledObj.SetActive(false);

        // Icon
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(filledObj.transform, false);
        RectTransform irt = iconObj.AddComponent<RectTransform>();
        irt.anchorMin = new Vector2(0.05f, 0.5f);
        irt.anchorMax = new Vector2(0.05f, 0.5f);
        irt.sizeDelta = new Vector2(80, 80);
        irt.anchoredPosition = new Vector2(60, 0);
        
        Image iconImg = iconObj.AddComponent<Image>();
        iconImg.color = Color.white;
        ui.playerIcon = iconImg;

        // Texts
        GameObject fTxtCont = new GameObject("Texts");
        fTxtCont.transform.SetParent(filledObj.transform, false);
        RectTransform ftrt = fTxtCont.AddComponent<RectTransform>();
        ftrt.anchorMin = new Vector2(0.2f, 0);
        ftrt.anchorMax = new Vector2(1, 1);
        ftrt.offsetMin = Vector2.zero;
        ftrt.offsetMax = Vector2.zero;

        ui.filledTitleText = CreateText(fTxtCont.transform, "KAYITLI SLOT", 32, Color.white, new Vector2(0, 0.5f), new Vector2(1, 0.9f));
        ui.filledTitleText.alignment = TextAlignmentOptions.Left;

        ui.filledSubtitleText = CreateText(fTxtCont.transform, "Detaylar...", 20, new Color(0.8f, 0.8f, 0.8f), new Vector2(0, 0.1f), new Vector2(1, 0.5f));
        ui.filledSubtitleText.alignment = TextAlignmentOptions.Left;

        return ui;
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
