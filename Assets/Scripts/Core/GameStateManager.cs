using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Oyun durumu yönetimi - Scene geçişlerini ve oyun durumlarını kontrol eder
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Oyun Durumları")]
    public GameState currentState { get; private set; }
    public GameState previousState { get; private set; }

    // State değişiklik event'i
    public event Action<GameState, GameState> OnStateChanged;

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

        // İlk durumu ayarla
        currentState = GameState.Boot;
        
        Debug.Log("[GameStateManager] GameStateManager initialized.");
    }

    private void Start()
    {
        // Boot'tan MainMenu'ye geç (eğer Boot sahnesindeyse)
        if (SceneManager.GetActiveScene().name == "Boot")
        {
            ChangeState(GameState.MainMenu);
        }
        else
        {
            // Mevcut sahneye göre durumu ayarla
            SetStateFromScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Oyun durumunu değiştir ve sahneyi yükle
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"[GameStateManager] Already in state: {newState}");
            return;
        }

        previousState = currentState;
        currentState = newState;

        Debug.Log($"[GameStateManager] State changed: {previousState} -> {currentState}");

        // Event'i tetikle
        OnStateChanged?.Invoke(previousState, currentState);

        // Sahneyi yükle
        LoadSceneForState(newState);
    }

    /// <summary>
    /// Duruma göre sahneyi yükle
    /// </summary>
    private void LoadSceneForState(GameState state)
    {
        string sceneName = GetSceneNameForState(state);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"[GameStateManager] No scene name for state: {state}");
            return;
        }

        Debug.Log($"[GameStateManager] Loading scene: {sceneName} for state: {state}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Durum için sahne adını getir
    /// </summary>
    private string GetSceneNameForState(GameState state)
    {
        return state switch
        {
            GameState.Boot => "Boot",
            GameState.MainMenu => "MainMenu",
            GameState.DataPackMenu => "DataPackMenu",
            GameState.SaveSlots => "SaveSlots",
            GameState.CharacterCreation => "CharacterCreation",
            GameState.TeamOffer => "TeamOffer",
            GameState.CareerHub => "CareerHub",
            GameState.MatchPre => "MatchPre",
            GameState.Match => "Match",
            GameState.PostMatch => "PostMatch",
            GameState.PlayerStats => "PlayerStats",
            GameState.Standings => "Standings",
            GameState.Settings => "Settings",
            _ => ""
        };
    }

    /// <summary>
    /// Sahne adına göre durumu ayarla
    /// </summary>
    private void SetStateFromScene(string sceneName)
    {
        GameState state = sceneName switch
        {
            "Boot" => GameState.Boot,
            "MainMenu" => GameState.MainMenu,
            "DataPackMenu" => GameState.DataPackMenu,
            "SaveSlots" => GameState.SaveSlots,
            "CharacterCreation" => GameState.CharacterCreation,
            "TeamOffer" => GameState.TeamOffer,
            "CareerHub" => GameState.CareerHub,
            "MatchPre" => GameState.MatchPre,
            "Match" => GameState.Match,
            "PostMatch" => GameState.PostMatch,
            "PlayerStats" => GameState.PlayerStats,
            "Standings" => GameState.Standings,
            "Settings" => GameState.Settings,
            _ => GameState.MainMenu
        };

        currentState = state;
        Debug.Log($"[GameStateManager] State set from scene: {sceneName} -> {state}");
    }

    /// <summary>
    /// Önceki duruma geri dön
    /// </summary>
    public void ReturnToPreviousState()
    {
        if (previousState != GameState.Boot)
        {
            ChangeState(previousState);
        }
        else
        {
            ChangeState(GameState.MainMenu);
        }
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

/// <summary>
/// Oyun durumları enum'u
/// </summary>
public enum GameState
{
    Boot,               // Başlangıç sahnesi
    MainMenu,           // Ana menü
    DataPackMenu,       // Data Pack menüsü
    SaveSlots,          // Kayıt slotları
    CharacterCreation,  // Karakter oluşturma
    TeamOffer,          // Takım teklifleri
    CareerHub,          // Kariyer hub
    MatchPre,           // Maç öncesi
    Match,              // Maç
    PostMatch,          // Maç sonrası
    PlayerStats,        // Oyuncu istatistikleri
    Standings,          // Puan durumu
    Settings            // Ayarlar
}

