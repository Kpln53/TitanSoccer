using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixMainMenuHierarchy : MonoBehaviour
{
    [MenuItem("TitanSoccer/Fix/🔧 Fix Main Menu Hierarchy")]
    public static void FixHierarchy()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        // 1. Create MainPanel if not exists
        Transform mainPanelTr = canvas.transform.Find("MainPanel");
        GameObject mainPanelObj;
        
        if (mainPanelTr == null)
        {
            mainPanelObj = new GameObject("MainPanel");
            mainPanelObj.transform.SetParent(canvas.transform, false);
            RectTransform rt = mainPanelObj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            
            // Move it to be behind SaveMenuPanel (which is likely last) but in front of Background
            // Assuming Background is first child.
            mainPanelObj.transform.SetSiblingIndex(1); 
        }
        else
        {
            mainPanelObj = mainPanelTr.gameObject;
        }

        // 2. Move Elements to MainPanel
        string[] elementsToMove = new string[] {
            "Logo", "Futbolcu", 
            "KariyerModu", "GösteriMaçı", "Antrenman", 
            "Paketler", "Ayarlar", "Çıkış"
        };

        foreach (string name in elementsToMove)
        {
            Transform t = canvas.transform.Find(name);
            if (t != null)
            {
                t.SetParent(mainPanelObj.transform, true);
            }
        }

        // 3. Assign to MainMenuUI
        MainMenuUI ui = FindFirstObjectByType<MainMenuUI>();
        if (ui != null)
        {
            ui.mainMenuPanel = mainPanelObj.GetComponent<RectTransform>();
            
            // Re-assign buttons just in case (though they should persist)
            Transform panelT = mainPanelObj.transform;
            if (ui.careerButton == null) ui.careerButton = panelT.Find("KariyerModu")?.GetComponent<Button>();
            if (ui.exhibitionButton == null) ui.exhibitionButton = panelT.Find("GösteriMaçı")?.GetComponent<Button>();
            if (ui.trainingButton == null) ui.trainingButton = panelT.Find("Antrenman")?.GetComponent<Button>();
            if (ui.dataPacksButton == null) ui.dataPacksButton = panelT.Find("Paketler")?.GetComponent<Button>();
            if (ui.settingsButton == null) ui.settingsButton = panelT.Find("Ayarlar")?.GetComponent<Button>();
            if (ui.quitButton == null) ui.quitButton = panelT.Find("Çıkış")?.GetComponent<Button>();
            
            EditorUtility.SetDirty(ui);
        }

        Debug.Log("✅ Main Menu Hierarchy Fixed!");
    }
}
