using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixNewsUI : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔧 Fix News UI References")]
    public static void FixReferences()
    {
        // 1. Find CareerHubUI
        GameObject canvasObj = GameObject.Find("CareerHubCanvas");
        if (canvasObj == null) return;
        
        CareerHubUI hubUI = canvasObj.GetComponent<CareerHubUI>();
        if (hubUI == null) return;
        
        // 2. Find NewsPanel
        Transform newsPanel = canvasObj.transform.Find("MainPanel/ContentArea/NewsPanel");
        if (newsPanel == null)
        {
            Debug.LogError("NewsPanel not found!");
            return;
        }
        
        // 3. Assign NewsPanel to CareerHubUI
        hubUI.newsPanel = newsPanel.gameObject;
        Debug.Log("✅ Assigned NewsPanel to CareerHubUI");
        
        // 4. Setup Filter Buttons
        CareerNewsUI newsUI = newsPanel.GetComponent<CareerNewsUI>();
        if (newsUI != null)
        {
            Transform filterSection = newsPanel.Find("FilterSection");
            if (filterSection != null)
            {
                // Tümü
                SetupFilterButton(filterSection, "Tümü", newsUI);
                // Transfer
                SetupFilterButton(filterSection, "Transfer", newsUI);
                // Lig
                SetupFilterButton(filterSection, "Lig", newsUI);
                // Kulüp
                SetupFilterButton(filterSection, "Kulüp", newsUI);
            }
        }
        
        // 5. Ensure NewsPanel is hidden initially (so HomePanel shows first)
        newsPanel.gameObject.SetActive(false);
        Debug.Log("✅ NewsPanel hidden initially");
        
        // 6. Ensure HomePanel is visible
        if (hubUI.homePanel != null)
        {
            hubUI.homePanel.SetActive(true);
        }
    }
    
    private static void SetupFilterButton(Transform parent, string btnName, CareerNewsUI ui)
    {
        Transform btnTrans = parent.Find(btnName);
        if (btnTrans != null)
        {
            Button btn = btnTrans.GetComponent<Button>();
            if (btn != null)
            {
                // Remove old listeners to avoid duplicates
                // Note: In Editor script we can't easily remove runtime listeners added via code, 
                // but Persistent listeners can be managed.
                // Since we are in Editor mode, we can't use onClick.AddListener directly for runtime logic 
                // unless we use UnityEventTools which is Editor-only.
                
                // For runtime functionality, we need a runtime script or assign via Inspector.
                // Let's use UnityEventTools to make it persistent.
                
                // Remove all persistent listeners first to be safe
                while (btn.onClick.GetPersistentEventCount() > 0)
                {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
                }
                
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btn.onClick, ui.FilterByString, btnName);
                
                Debug.Log($"✅ Setup filter button: {btnName}");
            }
        }
    }
}
