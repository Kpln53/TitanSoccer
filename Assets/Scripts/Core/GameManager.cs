using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyun durumu ve kayıt yönetimi için ana yönetici (Singleton)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Kayıt Yönetimi")]
    public SaveData CurrentSave { get; private set; }
    public int CurrentSaveSlotIndex { get; private set; } = -1;

    [Header("Oyun Ayarları")]
    public bool isPaused = false;

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

        Debug.Log("[GameManager] GameManager initialized.");
    }

    private void Start()
    {
        // İlk başlatma işlemleri
        Initialize();
    }

    /// <summary>
    /// GameManager'ı başlat
    /// </summary>
    private void Initialize()
    {
        // Gerekirse başlangıç kontrolleri yapılabilir
    }

    /// <summary>
    /// Mevcut kayıt dosyasını ayarla
    /// </summary>
    public void SetCurrentSave(SaveData saveData, int slotIndex)
    {
        CurrentSave = saveData;
        CurrentSaveSlotIndex = slotIndex;
        
        Debug.Log($"[GameManager] Current save set: Slot {slotIndex}, Player: {saveData?.playerProfile?.playerName ?? "None"}");
    }

    /// <summary>
    /// Mevcut kayıt dosyasını temizle
    /// </summary>
    public void ClearCurrentSave()
    {
        CurrentSave = null;
        CurrentSaveSlotIndex = -1;
        
        Debug.Log("[GameManager] Current save cleared.");
    }

    /// <summary>
    /// Kayıt var mı kontrol et
    /// </summary>
    public bool HasCurrentSave()
    {
        return CurrentSave != null;
    }

    /// <summary>
    /// Oyunu duraklat/devam ettir
    /// </summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        
        Debug.Log($"[GameManager] Game paused: {paused}");
    }

    /// <summary>
    /// Sahne yükle
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Sahne yükle (async)
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

