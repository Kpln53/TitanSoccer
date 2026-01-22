using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;

public class RebuildSaveSlots : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/♻️ Rebuild Save Slots")]
    public static void Rebuild()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (!canvas) return;

        Transform savePanel = canvas.transform.Find("SaveMenuPanel");
        if (!savePanel) 
        {
            Debug.LogError("SaveMenuPanel not found!");
            return;
        }

        Transform slotsContainer = savePanel.Find("Slotlar") ?? savePanel.Find("SlotsContainer");
        if (!slotsContainer)
        {
            // Create if missing
            GameObject sc = new GameObject("Slotlar");
            sc.transform.SetParent(savePanel, false);
            slotsContainer = sc.transform;
            
            RectTransform rt = sc.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.2f);
            rt.anchorMax = new Vector2(0.9f, 0.8f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            VerticalLayoutGroup vlg = sc.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 30;
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
        }

        // 1. Clear existing slots
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in slotsContainer) children.Add(child.gameObject);
        foreach (GameObject child in children) DestroyImmediate(child);

        // 2. Create 3 Standard Slots
        for (int i = 0; i < 3; i++)
        {
            CreateStandardSlot(slotsContainer, i);
        }

        // 3. Link to SaveMenuUI
        SaveMenuUI menuUI = savePanel.GetComponent<SaveMenuUI>();
        if (!menuUI) menuUI = savePanel.gameObject.AddComponent<SaveMenuUI>();
        
        menuUI.slotsContainer = slotsContainer;
        menuUI.saveSlots = new List<SaveSlotUI>();
        foreach(Transform child in slotsContainer)
        {
            menuUI.saveSlots.Add(child.GetComponent<SaveSlotUI>());
        }
        
        EditorUtility.SetDirty(menuUI);
        Debug.Log("✅ Save Slots Rebuilt & Linked!");
    }

    private static void CreateStandardSlot(Transform parent, int index)
    {
        GameObject slotObj = new GameObject($"Slot{index + 1}");
        slotObj.transform.SetParent(parent, false);
        
        RectTransform rt = slotObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 180);

        // Background & Button
        Image bg = slotObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // Dark bg
        
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

        // Texts Container (Empty)
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
        filledObj.SetActive(false); // Default hidden

        // Player Icon
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

        // Texts Container (Filled)
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
        
        return tmp;
    }
}
