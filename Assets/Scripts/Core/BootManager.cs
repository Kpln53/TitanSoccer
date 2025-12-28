using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Boot sahnesi yöneticisi - Oyun başlangıcında gerekli sistemleri yükler ve ana menüye yönlendirir
/// </summary>
public class BootManager : MonoBehaviour
{
    [Header("Yönlendirme")]
    public float delayBeforeMainMenu = 0.5f; // Ana menüye geçmeden önce bekleme süresi (saniye)
    public string mainMenuSceneName = "MainMenu"; // Ana menü sahne adı

    private void Awake()
    {
        Debug.Log("[BootManager] Boot scene started. Initializing systems...");
        InitializeSystems();
    }

    private void Start()
    {
        // Gerekli sistemlerin yüklenmesini bekle
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    /// <summary>
    /// Tüm gerekli singleton sistemleri başlat
    /// </summary>
    private void InitializeSystems()
    {
        Debug.Log("[BootManager] Initializing core systems...");

        // GameManager (zaten sahne üzerinde olmalı, yoksa oluştur)
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
            Debug.Log("[BootManager] GameManager created.");
        }

        // DataPackManager (zaten sahne üzerinde olmalı, yoksa oluştur)
        if (DataPackManager.Instance == null)
        {
            GameObject dataPackManagerObj = new GameObject("DataPackManager");
            dataPackManagerObj.AddComponent<DataPackManager>();
            Debug.Log("[BootManager] DataPackManager created.");
        }

        // GameStateManager
        if (GameStateManager.Instance == null)
        {
            GameObject gameStateManagerObj = new GameObject("GameStateManager");
            gameStateManagerObj.AddComponent<GameStateManager>();
            Debug.Log("[BootManager] GameStateManager created.");
        }

        // SeasonCalendarSystem
        if (SeasonCalendarSystem.Instance == null)
        {
            GameObject seasonCalendarSystemObj = new GameObject("SeasonCalendarSystem");
            seasonCalendarSystemObj.AddComponent<SeasonCalendarSystem>();
            Debug.Log("[BootManager] SeasonCalendarSystem created.");
        }

        // TransferSystem
        if (TransferSystem.Instance == null)
        {
            GameObject transferSystemObj = new GameObject("TransferSystem");
            transferSystemObj.AddComponent<TransferSystem>();
            Debug.Log("[BootManager] TransferSystem created.");
        }

        // TransferAISystem
        if (TransferAISystem.Instance == null)
        {
            GameObject transferAISystemObj = new GameObject("TransferAISystem");
            transferAISystemObj.AddComponent<TransferAISystem>();
            Debug.Log("[BootManager] TransferAISystem created.");
        }

        // NewsSystem
        if (NewsSystem.Instance == null)
        {
            GameObject newsSystemObj = new GameObject("NewsSystem");
            newsSystemObj.AddComponent<NewsSystem>();
            Debug.Log("[BootManager] NewsSystem created.");
        }

        // SocialMediaSystem
        if (SocialMediaSystem.Instance == null)
        {
            GameObject socialMediaSystemObj = new GameObject("SocialMediaSystem");
            socialMediaSystemObj.AddComponent<SocialMediaSystem>();
            Debug.Log("[BootManager] SocialMediaSystem created.");
        }

        // MarketSystem
        if (MarketSystem.Instance == null)
        {
            GameObject marketSystemObj = new GameObject("MarketSystem");
            marketSystemObj.AddComponent<MarketSystem>();
            Debug.Log("[BootManager] MarketSystem created.");
        }

        // FormMoralEnergySystem
        if (FormMoralEnergySystem.Instance == null)
        {
            GameObject formMoralEnergySystemObj = new GameObject("FormMoralEnergySystem");
            formMoralEnergySystemObj.AddComponent<FormMoralEnergySystem>();
            Debug.Log("[BootManager] FormMoralEnergySystem created.");
        }

        // TrainingSystem
        if (TrainingSystem.Instance == null)
        {
            GameObject trainingSystemObj = new GameObject("TrainingSystem");
            trainingSystemObj.AddComponent<TrainingSystem>();
            Debug.Log("[BootManager] TrainingSystem created.");
        }

        // InjurySystem
        if (InjurySystem.Instance == null)
        {
            GameObject injurySystemObj = new GameObject("InjurySystem");
            injurySystemObj.AddComponent<InjurySystem>();
            Debug.Log("[BootManager] InjurySystem created.");
        }

        // ClubGoalsSystem
        if (ClubGoalsSystem.Instance == null)
        {
            GameObject clubGoalsSystemObj = new GameObject("ClubGoalsSystem");
            clubGoalsSystemObj.AddComponent<ClubGoalsSystem>();
            Debug.Log("[BootManager] ClubGoalsSystem created.");
        }

        // ManagerAISystem
        if (ManagerAISystem.Instance == null)
        {
            GameObject managerAISystemObj = new GameObject("ManagerAISystem");
            managerAISystemObj.AddComponent<ManagerAISystem>();
            Debug.Log("[BootManager] ManagerAISystem created.");
        }

        // NationalTeamSystem
        if (NationalTeamSystem.Instance == null)
        {
            GameObject nationalTeamSystemObj = new GameObject("NationalTeamSystem");
            nationalTeamSystemObj.AddComponent<NationalTeamSystem>();
            Debug.Log("[BootManager] NationalTeamSystem created.");
        }

        // BriberySystem
        if (BriberySystem.Instance == null)
        {
            GameObject briberySystemObj = new GameObject("BriberySystem");
            briberySystemObj.AddComponent<BriberySystem>();
            Debug.Log("[BootManager] BriberySystem created.");
        }

        // CriticalEventSystem
        if (CriticalEventSystem.Instance == null)
        {
            GameObject criticalEventSystemObj = new GameObject("CriticalEventSystem");
            criticalEventSystemObj.AddComponent<CriticalEventSystem>();
            Debug.Log("[BootManager] CriticalEventSystem created.");
        }

        // CommentatorSystem
        if (CommentatorSystem.Instance == null)
        {
            GameObject commentatorSystemObj = new GameObject("CommentatorSystem");
            commentatorSystemObj.AddComponent<CommentatorSystem>();
            Debug.Log("[BootManager] CommentatorSystem created.");
        }

        // MatchSimulationSystem
        if (MatchSimulationSystem.Instance == null)
        {
            GameObject matchSimSystemObj = new GameObject("MatchSimulationSystem");
            matchSimSystemObj.AddComponent<MatchSimulationSystem>();
            Debug.Log("[BootManager] MatchSimulationSystem created.");
        }

        Debug.Log("[BootManager] All core systems initialized.");
    }

    /// <summary>
    /// Ana menüye yönlendirme işlemini geciktirilmiş olarak yap
    /// </summary>
    private System.Collections.IEnumerator LoadMainMenuAfterDelay()
    {
        // Sistemlerin tamamen yüklenmesi için kısa bir süre bekle
        yield return new WaitForSeconds(delayBeforeMainMenu);
        LoadMainMenu();
    }

    /// <summary>
    /// Ana menü sahnesine yönlendir
    /// </summary>
    private void LoadMainMenu()
    {
        Debug.Log($"[BootManager] Loading main menu: {mainMenuSceneName}");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

