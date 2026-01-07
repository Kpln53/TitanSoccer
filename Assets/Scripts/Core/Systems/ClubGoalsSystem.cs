using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kulüp hedefleri sistemi - Hedef takibi ve yönetimi (Singleton)
/// </summary>
public class ClubGoalsSystem : MonoBehaviour
{
    public static ClubGoalsSystem Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[ClubGoalsSystem] ClubGoalsSystem initialized.");
    }

    /// <summary>
    /// Hedefleri kontrol et ve durumu güncelle
    /// </summary>
    public void CheckObjectives(SaveData saveData)
    {
        if (saveData == null || saveData.clubData == null || saveData.clubData.objectives == null)
            return;

        foreach (var objective in saveData.clubData.objectives)
        {
            if (objective.isCompleted)
                continue;

            // Hedef türüne göre kontrol et
            switch (objective.objectiveType)
            {
                case ClubObjective.LeaguePosition:
                    CheckLeaguePositionObjective(saveData, objective);
                    break;

                case ClubObjective.CupWin:
                    // Kupayı kazandı mı kontrol et
                    break;

                case ClubObjective.ChampionsLeague:
                    // Şampiyonlar ligine katıldı mı kontrol et
                    break;

                case ClubObjective.RelegationAvoid:
                    // Küme düşmeden kurtuldu mu kontrol et
                    break;
            }
        }
    }

    /// <summary>
    /// Lig pozisyonu hedefini kontrol et
    /// </summary>
    private void CheckLeaguePositionObjective(SaveData saveData, ClubObjectiveData objective)
    {
        if (saveData.seasonData == null)
            return;

        int currentPosition = saveData.seasonData.leaguePosition;
        objective.currentValue = currentPosition;

        // Hedef pozisyondan daha iyi veya eşit pozisyondaysa tamamlandı
        if (currentPosition <= objective.targetValue && currentPosition > 0)
        {
            objective.isCompleted = true;
            Debug.Log($"[ClubGoalsSystem] Objective completed: {objective.description}");
        }
    }

    /// <summary>
    /// Hedef ekle
    /// </summary>
    public void AddObjective(ClubData clubData, ClubObjective objectiveType, string description, int targetValue)
    {
        if (clubData == null || clubData.objectives == null)
            return;

        ClubObjectiveData objective = new ClubObjectiveData
        {
            objectiveType = objectiveType,
            description = description,
            targetValue = targetValue,
            isCompleted = false,
            currentValue = 0
        };

        clubData.objectives.Add(objective);
        Debug.Log($"[ClubGoalsSystem] Objective added: {description}");
    }

    /// <summary>
    /// Tamamlanan hedef sayısını getir
    /// </summary>
    public int GetCompletedObjectiveCount(ClubData clubData)
    {
        if (clubData == null || clubData.objectives == null)
            return 0;

        int count = 0;
        foreach (var objective in clubData.objectives)
        {
            if (objective.isCompleted)
                count++;
        }

        return count;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}







