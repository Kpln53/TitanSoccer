using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class AddDeleteButtonToSlots : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🗑️ Add Delete Buttons to Slots")]
    public static void AddButtons()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (!canvas) return;

        Transform slotsContainer = canvas.transform.Find("SaveMenuPanel/SlotsContainer");
        if (!slotsContainer) return;

        foreach (Transform slot in slotsContainer)
        {
            // Silme butonu var mı kontrol et
            Transform existingBtn = slot.Find("DeleteButton");
            if (existingBtn) DestroyImmediate(existingBtn.gameObject);

            // Yeni buton oluştur
            GameObject delBtnObj = new GameObject("DeleteButton");
            delBtnObj.transform.SetParent(slot, false);
            
            RectTransform rt = delBtnObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.9f, 0.3f);
            rt.anchorMax = new Vector2(0.98f, 0.7f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = delBtnObj.AddComponent<Image>();
            img.color = new Color(0.8f, 0.2f, 0.2f); // Kırmızı

            Button btn = delBtnObj.AddComponent<Button>();
            btn.targetGraphic = img;

            // İkon/Metin
            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(delBtnObj.transform, false);
            RectTransform trt = txtObj.AddComponent<RectTransform>();
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            
            TextMeshProUGUI txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = "X";
            txt.fontSize = 24;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.white;

            // Script'e bağla
            SaveSlotUI ui = slot.GetComponent<SaveSlotUI>();
            if (ui)
            {
                ui.deleteButton = btn;
                EditorUtility.SetDirty(ui);
            }
        }

        Debug.Log("✅ Delete buttons added to slots!");
    }
}
