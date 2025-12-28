using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// DataPack yükleme ve erişim yönetimi (Singleton)
/// </summary>
public class DataPackManager : MonoBehaviour
{
    public static DataPackManager Instance { get; private set; }

    [Header("Aktif DataPack")]
    public DataPack activeDataPack;

    private Dictionary<string, DataPack> loadedDataPacks = new Dictionary<string, DataPack>();
    private List<DataPack> availableDataPacks = new List<DataPack>();

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

        Debug.Log("[DataPackManager] DataPackManager initialized.");
    }

    private void Start()
    {
        LoadAvailableDataPacks();
    }

    /// <summary>
    /// Resources klasöründen mevcut DataPack'leri yükle
    /// </summary>
    private void LoadAvailableDataPacks()
    {
        DataPack[] packs = Resources.LoadAll<DataPack>("DataPacks");
        
        availableDataPacks.Clear();
        availableDataPacks.AddRange(packs);

        Debug.Log($"[DataPackManager] Found {packs.Length} DataPack(s) in Resources.");
    }

    /// <summary>
    /// Aktif DataPack'i ayarla
    /// </summary>
    public void SetActiveDataPack(DataPack dataPack)
    {
        if (dataPack == null)
        {
            Debug.LogWarning("[DataPackManager] Trying to set null DataPack as active!");
            return;
        }

        activeDataPack = dataPack;
        
        // Pack ID yoksa oluştur
        if (string.IsNullOrEmpty(activeDataPack.packId))
        {
            activeDataPack.packId = GeneratePackId(activeDataPack.packName);
        }

        Debug.Log($"[DataPackManager] Active DataPack set: {activeDataPack.packName} (ID: {activeDataPack.packId})");
    }

    /// <summary>
    /// Aktif DataPack'i kaldır
    /// </summary>
    public void ClearActiveDataPack()
    {
        activeDataPack = null;
        Debug.Log("[DataPackManager] Active DataPack cleared.");
    }

    /// <summary>
    /// Belirli bir takımı aktif DataPack'ten getir
    /// </summary>
    public TeamData GetTeam(string teamName)
    {
        if (activeDataPack == null)
        {
            Debug.LogWarning("[DataPackManager] No active DataPack! Cannot get team.");
            return null;
        }

        return activeDataPack.GetTeamByName(teamName);
    }

    /// <summary>
    /// Belirli bir ligi aktif DataPack'ten getir
    /// </summary>
    public LeagueData GetLeague(string leagueName)
    {
        if (activeDataPack == null)
        {
            Debug.LogWarning("[DataPackManager] No active DataPack! Cannot get league.");
            return null;
        }

        return activeDataPack.GetLeagueByName(leagueName);
    }

    /// <summary>
    /// Tüm takımları aktif DataPack'ten getir
    /// </summary>
    public List<TeamData> GetAllTeams()
    {
        if (activeDataPack == null)
        {
            Debug.LogWarning("[DataPackManager] No active DataPack! Cannot get teams.");
            return new List<TeamData>();
        }

        return activeDataPack.GetAllTeams();
    }

    /// <summary>
    /// Tüm ligleri aktif DataPack'ten getir
    /// </summary>
    public List<LeagueData> GetAllLeagues()
    {
        if (activeDataPack == null)
        {
            Debug.LogWarning("[DataPackManager] No active DataPack! Cannot get leagues.");
            return new List<LeagueData>();
        }

        return activeDataPack.leagues ?? new List<LeagueData>();
    }

    /// <summary>
    /// Mevcut DataPack'leri getir
    /// </summary>
    public List<DataPack> GetAvailableDataPacks()
    {
        return new List<DataPack>(availableDataPacks);
    }

    /// <summary>
    /// Pack ID oluştur (packName'den hash)
    /// </summary>
    private string GeneratePackId(string packName)
    {
        if (string.IsNullOrEmpty(packName))
            return "unknown";

        // Basit hash (packName'i ID'ye çevir)
        return packName.Replace(" ", "_").ToLower();
    }

    /// <summary>
    /// DataPack yükle (Resources'tan)
    /// </summary>
    public DataPack LoadDataPack(string packName)
    {
        string path = $"DataPacks/{packName}";
        DataPack pack = Resources.Load<DataPack>(path);

        if (pack != null)
        {
            if (string.IsNullOrEmpty(pack.packId))
            {
                pack.packId = GeneratePackId(pack.packName);
            }

            loadedDataPacks[pack.packId] = pack;
            Debug.Log($"[DataPackManager] DataPack loaded: {packName}");
        }
        else
        {
            Debug.LogWarning($"[DataPackManager] DataPack not found: {path}");
        }

        return pack;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

