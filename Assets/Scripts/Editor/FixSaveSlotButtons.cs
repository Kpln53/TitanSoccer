using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixSaveSlotButtons : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/Fix Save Slot Buttons")]
    public static void Fix()
    {
        GameObject panel = GameObject.Find("SaveMenuPanel");
        if (panel == null) return;

        Transform container = panel.transform.Find("SlotsContainer");
        if (container == null) return;

        foreach (Transform slot in container)
        {
            SaveSlotUI ui = slot.GetComponent<SaveSlotUI>();
            if (ui == null) continue;

            // 1. Fix Main Button
            Button mainBtn = slot.GetComponent<Button>();
            if (mainBtn == null) mainBtn = slot.gameObject.AddComponent<Button>();
            
            // Ensure target graphic is the invisible image
            Image img = slot.GetComponent<Image>();
            if (img != null) mainBtn.targetGraphic = img;

            ui.slotButton = mainBtn;

            // 2. Fix Delete Button (if exists in FilledState)
            if (ui.filledStateObject != null)
            {
                // Look for DeleteButton or create one if missing but needed
                // Based on previous prefab creation, it might not have been added to the custom prefab
                // Let's check if there is a delete button
                Transform delBtnTr = ui.filledStateObject.transform.Find("DeleteButton");
                if (delBtnTr != null)
                {
                    ui.deleteButton = delBtnTr.GetComponent<Button>();
                }
                else
                {
                    // Create Delete Button if missing
                    GameObject delBtnObj = new GameObject("DeleteButton");
                    delBtnObj.transform.SetParent(ui.filledStateObject.transform, false);
                    RectTransform delRect = delBtnObj.AddComponent<RectTransform>();
                    delRect.anchorMin = new Vector2(1, 0.5f);
                    delRect.anchorMax = new Vector2(1, 0.5f);
                    delRect.pivot = new Vector2(1, 0.5f);
                    delRect.sizeDelta = new Vector2(50, 50);
                    delRect.anchoredPosition = new Vector2(-25, 0);
                    
                    Image delImg = delBtnObj.AddComponent<Image>();
                    delImg.color = new Color(0.8f, 0.2f, 0.2f); // Red
                    
                    Button delBtn = delBtnObj.AddComponent<Button>();
                    delBtn.targetGraphic = delImg;
                    
                    ui.deleteButton = delBtn;

                    // X Text
                    GameObject delTextObj = new GameObject("X");
                    delTextObj.transform.SetParent(delBtnObj.transform, false);
                    TMPro.TextMeshProUGUI delText = delTextObj.AddComponent<TMPro.TextMeshProUGUI>();
                    delText.text = "X";
                    delText.fontSize = 30;
                    delText.alignment = TMPro.TextAlignmentOptions.Center;
                    delText.color = Color.white;
                    delText.rectTransform.anchorMin = Vector2.zero;
                    delText.rectTransform.anchorMax = Vector2.one;
                }
            }

            EditorUtility.SetDirty(ui);
        }

        // Refresh Menu UI list
        SaveMenuUI menuUI = panel.GetComponent<SaveMenuUI>();
        if (menuUI != null)
        {
            menuUI.RefreshSlots();
            EditorUtility.SetDirty(menuUI);
        }

        Debug.Log("Save Slot Buttons Fixed!");
    }
}
