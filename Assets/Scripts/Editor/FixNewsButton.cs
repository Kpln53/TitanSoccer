using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;

public class FixNewsButton : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔧 Fix News Button")]
    public static void Fix()
    {
        GameObject canvas = GameObject.Find("CareerHubCanvas");
        if (!canvas) return;

        CareerHubUI hubUI = canvas.GetComponent<CareerHubUI>();
        if (!hubUI) return;
        
        // Find News Button
        Transform newsBtnTr = canvas.transform.Find("MainPanel/BottomPanel/NewsButton");
        if (newsBtnTr)
        {
            Button btn = newsBtnTr.GetComponent<Button>();
            
            // Clear existing listeners
            int count = btn.onClick.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(btn.onClick, i);
            }
            
            // Add listener to show NewsPanel
            // Since ShowPanel is private, we can't add it directly via UnityEventTools easily for runtime.
            // However, CareerHubUI.Start() adds it via code.
            // The issue might be that CareerHubUI.Start() runs BEFORE we fixed the references in the previous step if the game was running.
            // Or the references were lost.
            
            // Let's ensure the references are correct in the Inspector.
            hubUI.newsButton = btn;
            
            // Also find NewsPanel
            Transform newsPanel = canvas.transform.Find("MainPanel/ContentArea/NewsPanel");
            if (newsPanel)
            {
                hubUI.newsPanel = newsPanel.gameObject;
            }
            
            EditorUtility.SetDirty(hubUI);
            Debug.Log("✅ News Button Fixed");
        }
    }
}
