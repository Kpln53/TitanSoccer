using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileCareerUI : MonoBehaviour
{
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject historyItemPrefab;

    public void Setup(List<CareerHistoryEntry> history)
    {
        // Clear existing
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // Add current season (mock data if needed, or from SeasonData)
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            var season = GameManager.Instance.CurrentSave.seasonData;
            var profile = GameManager.Instance.CurrentSave.playerProfile;
            
            // Create entry for current season
            CareerHistoryEntry currentSeason = new CareerHistoryEntry(
                season.seasonName,
                profile.currentClubName,
                season.matchesPlayed,
                season.goals,
                season.assists,
                season.averageRating
            );
            
            CreateItem(currentSeason);
        }

        // Add past seasons
        if (history != null)
        {
            foreach (var entry in history)
            {
                CreateItem(entry);
            }
        }
        
        // Mock data if empty for visualization
        if (contentContainer.childCount == 0)
        {
            CreateItem(new CareerHistoryEntry("2024-25", "Fenerbahçe", 32, 5, 8, 7.2f));
            CreateItem(new CareerHistoryEntry("2023-24", "Fenerbahçe", 20, 3, 4, 7.0f));
        }
    }

    private void CreateItem(CareerHistoryEntry entry)
    {
        if (historyItemPrefab == null) return;

        GameObject item = Instantiate(historyItemPrefab, contentContainer);
        CareerHistoryItem script = item.GetComponent<CareerHistoryItem>();
        if (script != null)
        {
            script.Setup(entry);
        }
    }
}
