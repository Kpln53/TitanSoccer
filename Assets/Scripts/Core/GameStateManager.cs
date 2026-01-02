using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyun durumu yöneticisi - Scene geçişlerini ve oyun durumlarını yönetir (Singleton)
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Oyun Durumu")]
    public GameState currentState = GameState.MainMenu;

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
    }

    private void Start()
    {
        // İlk state'i ayarla
        if (currentState == GameState.MainMenu)
        {
            // Eğer MainMenu scene'indeysek, state'i ayarla
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "MainMenu")
            {
                currentState = GameState.MainMenu;
            }
        }
    }

    /// <summary>
    /// Oyun durumunu değiştir
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"[GameStateManager] Already in state: {newState}");
            return;
        }

        Debug.Log($"[GameStateManager] Changing state from {currentState} to {newState}");

        GameState previousState = currentState;
        currentState = newState;

        // State'e göre scene yükle
        LoadSceneForState(newState);
    }

    /// <summary>
    /// State'e göre scene yükle
    /// </summary>
    private void LoadSceneForState(GameState state)
    {
        string sceneName = GetSceneNameForState(state);
        
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning($"[GameStateManager] No scene defined for state: {state}");
        }
    }

    /// <summary>
    /// State için scene adını getir
    /// </summary>
    private string GetSceneNameForState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                return "MainMenu";
            case GameState.SaveSlots:
                return "SaveSlots";
            case GameState.CharacterCreation:
                return "CharacterCreation";
            case GameState.CareerHub:
                return "CareerHub";
            case GameState.DataPackMenu:
                return "DataPackMenu";
            case GameState.Settings:
                return "Settings";
            case GameState.TeamOffer:
                return "TeamOffer";
            case GameState.PreMatch:
                return "PreMatch";
            case GameState.MatchSim:
                return "MatchSim";
            case GameState.MatchChanceGameplay:
                return "MatchChanceGameplay";
            case GameState.PostMatch:
                return "PostMatch";
            default:
                return null;
        }
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    /// <summary>
    /// Career Hub'a git
    /// </summary>
    public void GoToCareerHub()
    {
        ChangeState(GameState.CareerHub);
    }

    /// <summary>
    /// Mevcut state'i getir
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Önceki state'e dön (basit implementasyon - CharacterCreation'a döner)
    /// </summary>
    public void ReturnToPreviousState()
    {
        // Basit implementasyon: CharacterCreation'a dön
        // İleride state history stack'i eklenebilir
        ChangeState(GameState.CharacterCreation);
    }
}

/// <summary>
/// Oyun durumları enum'u
/// </summary>
public enum GameState
{
    MainMenu,              // Ana menü
    SaveSlots,             // Kayıt slotları menüsü
    CharacterCreation,     // Karakter oluşturma
    CareerHub,             // Kariyer merkezi
    DataPackMenu,          // Data Pack menüsü
    Settings,               // Ayarlar
    TeamOffer,              // Takım teklifi
    PreMatch,               // Maç öncesi
    MatchSim,               // Maç simülasyonu
    MatchChanceGameplay,    // Maç pozisyon oynanışı
    PostMatch               // Maç sonrası
}
