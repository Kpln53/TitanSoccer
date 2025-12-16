using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyun durumlarını yöneten merkezi sistem
/// </summary>
public enum GameState
{
    MainMenu,
    SaveSlots,
    NewCareerFlow,
    CareerHub,
    MatchPre,
    MatchSim,
    Chance,
    Pause,
    PostMatch,
    TransferWindow,
    CriticalEventPopUp
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string saveSlotsScene = "SaveSlots";
    [SerializeField] private string newCareerFlowScene = "NewGameFlow";
    [SerializeField] private string careerHubScene = "CareerHub";
    [SerializeField] private string matchPreScene = "MatchPre";
    [SerializeField] private string matchScene = "MatchScene";
    [SerializeField] private string teamSelectionScene = "TeamSelection";
    [SerializeField] private string characterCreationScene = "CharacterCreation";
    [SerializeField] private string teamOfferScene = "TeamOffer";
    [SerializeField] private string playerStatsScene = "PlayerStats";
    [SerializeField] private string postMatchScene = "PostMatch";

    public GameState CurrentState => currentState;

    // State değişim eventi
    public System.Action<GameState, GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Durum değiştir
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
            return;

        GameState previousState = currentState;
        currentState = newState;

        OnStateChanged?.Invoke(previousState, newState);
        HandleStateChange(previousState, newState);
    }

    private void HandleStateChange(GameState previousState, GameState newState)
    {
        // Duruma göre scene yükleme
        switch (newState)
        {
            case GameState.MainMenu:
                LoadScene(mainMenuScene);
                break;
            case GameState.SaveSlots:
                LoadScene(saveSlotsScene);
                break;
            case GameState.NewCareerFlow:
                LoadScene(newCareerFlowScene);
                break;
            case GameState.CareerHub:
                LoadScene(careerHubScene);
                break;
            case GameState.MatchPre:
                LoadScene(matchPreScene);
                break;
            case GameState.PostMatch:
                LoadScene(postMatchScene);
                break;
            case GameState.MatchSim:
            case GameState.Chance:
                LoadScene(matchScene);
                break;
            case GameState.PostMatch:
                // PostMatch genellikle aynı scene'de pop-up olarak gösterilir
                break;
            case GameState.TransferWindow:
                // TransferWindow genellikle CareerHub içinde gösterilir
                break;
            case GameState.CriticalEventPopUp:
                // Pop-up, mevcut scene'de gösterilir
                break;
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Kritik olay pop-up göster (öncelikli)
    /// </summary>
    public void ShowCriticalEvent()
    {
        ChangeState(GameState.CriticalEventPopUp);
    }

    /// <summary>
    /// Kritik olay pop-up'ı kapat
    /// </summary>
    public void CloseCriticalEvent()
    {
        // Önceki duruma dön
        // Bu durumda genellikle CareerHub'a dönülür
        ChangeState(GameState.CareerHub);
    }
}

