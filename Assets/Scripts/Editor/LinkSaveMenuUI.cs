using UnityEngine;
using UnityEditor;

public class LinkSaveMenuUI : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔗 Link Save Menu UI")]
    public static void LinkUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (!canvas) return;

        Transform savePanel = canvas.transform.Find("SaveMenuPanel");
        if (!savePanel) return;

        // Scripti ekle (yoksa)
        SaveMenuUI ui = savePanel.GetComponent<SaveMenuUI>();
        if (!ui) ui = savePanel.gameObject.AddComponent<SaveMenuUI>();

        // Referansları bağla
        ui.slotsContainer = savePanel.Find("SlotsContainer");
        
        EditorUtility.SetDirty(ui);
        Debug.Log("✅ Save Menu UI Linked!");
    }
}
