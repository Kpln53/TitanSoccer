using UnityEngine;

/// <summary>
/// Ana Game Manager - Tüm sistemleri yönetir
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SaveData CurrentSave;
    public int CurrentSaveSlotIndex = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Sistemleri başlat
        InitializeSystems();
    }

    /// <summary>
    /// Tüm sistemleri başlat
    /// </summary>
    private void InitializeSystems()
    {
        // GameStateManager
        if (GameStateManager.Instance == null)
        {
            GameObject gsmObj = new GameObject("GameStateManager");
            gsmObj.AddComponent<GameStateManager>();
        }
        
        // FormMoralEnergySystem
        if (FormMoralEnergySystem.Instance == null)
        {
            GameObject fmeObj = new GameObject("FormMoralEnergySystem");
            fmeObj.AddComponent<FormMoralEnergySystem>();
        }
        
        // InjurySystem
        if (InjurySystem.Instance == null)
        {
            GameObject injObj = new GameObject("InjurySystem");
            injObj.AddComponent<InjurySystem>();
        }
        
        // MarketSystem
        if (MarketSystem.Instance == null)
        {
            GameObject marketObj = new GameObject("MarketSystem");
            marketObj.AddComponent<MarketSystem>();
        }
        
        // TrainingSystem
        if (TrainingSystem.Instance == null)
        {
            GameObject trainObj = new GameObject("TrainingSystem");
            trainObj.AddComponent<TrainingSystem>();
        }
        
        // TransferSystem
        if (TransferSystem.Instance == null)
        {
            GameObject transferObj = new GameObject("TransferSystem");
            transferObj.AddComponent<TransferSystem>();
        }
        
        // NewsSystem
        if (NewsSystem.Instance == null)
        {
            GameObject newsObj = new GameObject("NewsSystem");
            newsObj.AddComponent<NewsSystem>();
        }
        
        // SocialMediaSystem
        if (SocialMediaSystem.Instance == null)
        {
            GameObject socialObj = new GameObject("SocialMediaSystem");
            socialObj.AddComponent<SocialMediaSystem>();
        }
        
        // CriticalEventSystem
        if (CriticalEventSystem.Instance == null)
        {
            GameObject eventObj = new GameObject("CriticalEventSystem");
            eventObj.AddComponent<CriticalEventSystem>();
        }
        
        // SeasonCalendarSystem
        if (SeasonCalendarSystem.Instance == null)
        {
            GameObject seasonObj = new GameObject("SeasonCalendarSystem");
            seasonObj.AddComponent<SeasonCalendarSystem>();
        }
    }

    public void SetCurrentSave(SaveData data, int slotIndex)
    {
        CurrentSave = data;
        CurrentSaveSlotIndex = slotIndex;
        
        // SaveData'yı başlat (eğer yeni oluşturulduysa)
        if (data != null)
        {
            InitializeSaveData(data);
        }
    }

    /// <summary>
    /// SaveData'yı başlat (eksik alanları doldur)
    /// </summary>
    public void InitializeSaveData(SaveData data)
    {
        // PlayerProfile yoksa oluştur
        if (data.playerProfile == null)
        {
            data.playerProfile = new PlayerProfile();
        }
        
        // ClubData yoksa oluştur
        if (data.clubData == null)
        {
            data.clubData = new ClubData();
        }
        
        // SeasonData yoksa oluştur
        if (data.seasonData == null)
        {
            data.seasonData = new SeasonData();
        }
        
        // RelationsData yoksa oluştur
        if (data.relationsData == null)
        {
            data.relationsData = new RelationsData();
        }
        
        // EconomyData yoksa oluştur
        if (data.economyData == null)
        {
            data.economyData = new EconomyData();
        }
        
        // MediaData yoksa oluştur
        if (data.mediaData == null)
        {
            data.mediaData = new MediaData();
        }
    }
}
