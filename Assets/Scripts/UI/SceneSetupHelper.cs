using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene setup helper - Unity Editor'da scene'leri kontrol eder
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [ContextMenu("Check All Scenes")]
    public void CheckAllScenes()
    {
        string[] requiredScenes = {
            "MainMenu",
            "SaveSlots",
            "NewGameFlow",
            "CareerHub",
            "MatchPre",
            "MatchScene",
            "TeamSelection",
            "CharacterCreation",
            "TeamOffer",
            "PlayerStats",
            "PostMatch"
        };
        
        Debug.Log("=== Scene Kontrolü ===");
        
        foreach (string sceneName in requiredScenes)
        {
            bool exists = SceneExists(sceneName);
            if (exists)
            {
                Debug.Log($"✓ {sceneName} - Mevcut");
            }
            else
            {
                Debug.LogWarning($"✗ {sceneName} - EKSİK! Oluşturulmalı.");
            }
        }
    }
    
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    
    [ContextMenu("List All UI Scripts")]
    public void ListAllUIScripts()
    {
        Debug.Log("=== UI Script Listesi ===");
        Debug.Log("1. MainMenuUI");
        Debug.Log("2. SaveSlotsMenu");
        Debug.Log("3. NewGameFlowUI");
        Debug.Log("4. CharacterCreationUI");
        Debug.Log("5. TeamSelectionUI");
        Debug.Log("6. TeamOfferUI");
        Debug.Log("7. CareerHubUI");
        Debug.Log("8. MatchPreScreenUI");
        Debug.Log("9. PostMatchScreenUI");
        Debug.Log("10. PlayerStatsScreenUI");
        Debug.Log("11. CriticalEventPopUpUI");
        Debug.Log("12. DataPackMenuUI");
    }
}


