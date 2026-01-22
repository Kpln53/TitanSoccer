using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixMainMenuHierarchyV2 : MonoBehaviour
{
    [MenuItem("TitanSoccer/Fix/🔧 Fix Main Menu Hierarchy V2")]
    public static void FixHierarchy()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        Transform mainPanelTr = canvas.transform.Find("MainPanel");
        if (mainPanelTr == null) return;

        // 1. Move Logo and Futbolcu OUT of MainPanel (back to Canvas root)
        string[] staticElements = new string[] { "Logo", "Futbolcu" };
        foreach (string name in staticElements)
        {
            Transform t = mainPanelTr.Find(name);
            if (t != null)
            {
                t.SetParent(canvas.transform, true);
                t.SetSiblingIndex(1); // Background'dan sonra
            }
        }

        // 2. Rename MainPanel to ButtonsPanel for clarity
        mainPanelTr.name = "ButtonsPanel";

        // 3. Update MainMenuUI reference
        MainMenuUI ui = FindFirstObjectByType<MainMenuUI>();
        if (ui != null)
        {
            ui.mainMenuPanel = mainPanelTr.GetComponent<RectTransform>();
            EditorUtility.SetDirty(ui);
        }

        Debug.Log("✅ Main Menu Hierarchy Fixed V2 (Static elements separated)!");
    }
}
