using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

public class FixCareerHubReferences : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔧 Fix CareerHub References & Visibility")]
    public static void FixReferences()
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
        
        // 1. Find Panels
        Transform content = canvas.transform.Find("MainPanel/ContentArea");
        if (content)
        {
            Transform newsPanelTr = content.Find("NewsPanel");
            Transform homePanelTr = content.Find("HomePanel");
            
            if (newsPanelTr) 
            {
                hubUI.newsPanel = newsPanelTr.gameObject;
                Debug.Log("✅ Assigned NewsPanel to CareerHubUI");
                
                // Ensure NewsPanel is hidden
                newsPanelTr.gameObject.SetActive(false);
                Debug.Log("✅ Set NewsPanel to Inactive");
            }
            else
            {
                Debug.LogError("❌ NewsPanel not found in ContentArea!");
            }
            
            if (homePanelTr) 
            {
                hubUI.homePanel = homePanelTr.gameObject;
                Debug.Log("✅ Assigned HomePanel to CareerHubUI");
                
                // Ensure HomePanel is shown
                homePanelTr.gameObject.SetActive(true);
                Debug.Log("✅ Set HomePanel to Active");
            }
            else
            {
                Debug.LogError("❌ HomePanel not found in ContentArea!");
            }
        }
        
        // 2. Find Buttons and Wire them
        Transform bottom = canvas.transform.Find("MainPanel/BottomPanel");
        if (bottom)
        {
            // News Button
            Transform newsBtnTr = bottom.Find("NewsButton");
            if (newsBtnTr)
            {
                Button newsBtn = newsBtnTr.GetComponent<Button>();
                hubUI.newsButton = newsBtn;
                
                // Re-wire OnClick
                // Note: We can't easily access private methods like ShowPanel via UnityEventTools without reflection or making them public.
                // However, CareerHubUI.Start() adds listeners via code: newsButton.onClick.AddListener(() => ShowPanel(newsPanel));
                // So we don't need to add persistent listeners for ShowPanel if the script does it.
                // BUT, if the script references are null, it won't work. We just fixed the references above.
                
                Debug.Log("✅ Assigned NewsButton to CareerHubUI");
            }
            
            // Home Button
            Transform homeBtnTr = bottom.Find("HomeButton");
            if (homeBtnTr)
            {
                Button homeBtn = homeBtnTr.GetComponent<Button>();
                hubUI.homeButton = homeBtn;
                Debug.Log("✅ Assigned HomeButton to CareerHubUI");
            }
        }
        
        // 3. Wire Filter Buttons in NewsPanel
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
        
        // 4. Save Changes
        EditorUtility.SetDirty(hubUI);
        if (hubUI.newsPanel) EditorUtility.SetDirty(hubUI.newsPanel);
        
        Debug.Log("✅ CareerHub References Fixed!");
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
                Debug.Log($"✅ Wired filter button: {btnName}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Button component not found on {btnName}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Filter button object not found: {btnName}");
        }
    }
}
