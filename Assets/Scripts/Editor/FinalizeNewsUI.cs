using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

public class FinalizeNewsUI : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/✅ Finalize News UI Wiring")]
    public static void WireUp()
    {
        GameObject canvas = GameObject.Find("CareerHubCanvas");
        if (!canvas)
        {
            Debug.LogError("CareerHubCanvas not found!");
            return;
        }

        CareerHubUI hubUI = canvas.GetComponent<CareerHubUI>();
        if (!hubUI)
        {
            Debug.LogError("CareerHubUI component not found!");
            return;
        }
        
        // 1. Assign Panels to CareerHubUI
        Transform content = canvas.transform.Find("MainPanel/ContentArea");
        if (content)
        {
            Transform newsPanelTr = content.Find("NewsPanel");
            Transform homePanelTr = content.Find("HomePanel");
            
            if (newsPanelTr) hubUI.newsPanel = newsPanelTr.gameObject;
            if (homePanelTr) hubUI.homePanel = homePanelTr.gameObject;
            
            // Assign other panels if needed, but focus on News/Home for now
        }
        
        // 2. Assign Buttons to CareerHubUI (if missing)
        Transform bottom = canvas.transform.Find("MainPanel/BottomPanel");
        if (bottom)
        {
            if (!hubUI.homeButton) hubUI.homeButton = bottom.Find("HomeButton")?.GetComponent<Button>();
            if (!hubUI.newsButton) hubUI.newsButton = bottom.Find("NewsButton")?.GetComponent<Button>();
        }
        
        // 3. Wire Filter Buttons (Persistent Listeners)
        if (hubUI.newsPanel)
        {
            CareerNewsUI newsUI = hubUI.newsPanel.GetComponent<CareerNewsUI>();
            Transform filters = hubUI.newsPanel.transform.Find("FilterSection");
            
            if (newsUI && filters)
            {
                WireFilter(filters, "Tümü", newsUI, "Tümü");
                WireFilter(filters, "Transfer", newsUI, "Transfer");
                WireFilter(filters, "Lig", newsUI, "Lig");
                WireFilter(filters, "Kulüp", newsUI, "Kulüp");
            }
        }
        
        // 4. Set Initial State
        if (hubUI.newsPanel) hubUI.newsPanel.SetActive(false);
        if (hubUI.homePanel) hubUI.homePanel.SetActive(true);
        
        // 5. Save Changes
        EditorUtility.SetDirty(hubUI);
        if (hubUI.newsPanel) EditorUtility.SetDirty(hubUI.newsPanel);
        
        Debug.Log("✅ News UI Wiring Complete!");
    }
    
    static void WireFilter(Transform parent, string btnName, CareerNewsUI ui, string arg)
    {
        Transform t = parent.Find(btnName);
        if (t)
        {
            Button b = t.GetComponent<Button>();
            if (b)
            {
                // Clear existing listeners to avoid duplicates
                int count = b.onClick.GetPersistentEventCount();
                for (int i = count - 1; i >= 0; i--)
                {
                    UnityEventTools.RemovePersistentListener(b.onClick, i);
                }
                
                // Add new listener
                UnityEventTools.AddStringPersistentListener(b.onClick, ui.FilterByString, arg);
                Debug.Log($"Wired filter button: {btnName}");
            }
        }
    }
}
