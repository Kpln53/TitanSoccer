using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Data Pack Manager - Data pack'leri yükleme ve yönetme sistemi
/// </summary>
public class DataPackManager : MonoBehaviour
{
    private static DataPackManager instance;
    public static DataPackManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataPackManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("DataPackManager");
                    instance = go.AddComponent<DataPackManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    
    [Header("Aktif Data Pack")]
    [SerializeField] private DataPack activeDataPack;
    
    [Header("Yüklü Data Pack'ler")]
    [SerializeField] private List<DataPack> loadedDataPacks = new List<DataPack>();
    
    private Dictionary<string, TeamData> teamCache = new Dictionary<string, TeamData>();
    private Dictionary<string, LeagueData> leagueCache = new Dictionary<string, LeagueData>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        LoadDataPacks();
    }
    
    /// <summary>
    /// Data pack'leri yükle (Resources klasöründen)
    /// </summary>
    void LoadDataPacks()
    {
        loadedDataPacks.Clear();
        
        // Resources klasöründen tüm DataPack'leri yükle
        DataPack[] packs = Resources.LoadAll<DataPack>("DataPacks");
        loadedDataPacks.AddRange(packs);
        
        // Eğer aktif pack yoksa, ilkini seç
        if (activeDataPack == null && loadedDataPacks.Count > 0)
        {
            activeDataPack = loadedDataPacks[0];
        }
        
        // Cache'i güncelle
        UpdateCache();
    }
    
    /// <summary>
    /// Cache'i güncelle
    /// </summary>
    void UpdateCache()
    {
        teamCache.Clear();
        leagueCache.Clear();
        
        if (activeDataPack == null) return;
        
        // Tüm takımları cache'le
        List<TeamData> allTeams = activeDataPack.GetAllTeams();
        foreach (var team in allTeams)
        {
            if (!teamCache.ContainsKey(team.teamName))
            {
                teamCache[team.teamName] = team;
            }
        }
        
        // Tüm ligleri cache'le
        foreach (var league in activeDataPack.leagues)
        {
            if (!leagueCache.ContainsKey(league.leagueName))
            {
                leagueCache[league.leagueName] = league;
            }
        }
    }
    
    /// <summary>
    /// Aktif data pack'i ayarla
    /// </summary>
    public void SetActiveDataPack(DataPack dataPack)
    {
        activeDataPack = dataPack;
        UpdateCache();
    }
    
    /// <summary>
    /// Belirli bir takımı isimle getir
    /// </summary>
    public TeamData GetTeam(string teamName)
    {
        if (teamCache.ContainsKey(teamName))
        {
            return teamCache[teamName];
        }
        
        if (activeDataPack != null)
        {
            TeamData team = activeDataPack.GetTeamByName(teamName);
            if (team != null)
            {
                teamCache[teamName] = team;
                return team;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Belirli bir ligi isimle getir
    /// </summary>
    public LeagueData GetLeague(string leagueName)
    {
        if (leagueCache.ContainsKey(leagueName))
        {
            return leagueCache[leagueName];
        }
        
        if (activeDataPack != null)
        {
            LeagueData league = activeDataPack.GetLeagueByName(leagueName);
            if (league != null)
            {
                leagueCache[leagueName] = league;
                return league;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Tüm takımları getir
    /// </summary>
    public List<TeamData> GetAllTeams()
    {
        if (activeDataPack != null)
        {
            return activeDataPack.GetAllTeams();
        }
        return new List<TeamData>();
    }
    
    /// <summary>
    /// Tüm ligleri getir
    /// </summary>
    public List<LeagueData> GetAllLeagues()
    {
        if (activeDataPack != null)
        {
            return new List<LeagueData>(activeDataPack.leagues);
        }
        return new List<LeagueData>();
    }
    
    /// <summary>
    /// Yüklü data pack'leri getir
    /// </summary>
    public List<DataPack> GetLoadedDataPacks()
    {
        return new List<DataPack>(loadedDataPacks);
    }
    
    /// <summary>
    /// Aktif data pack'i getir
    /// </summary>
    public DataPack GetActiveDataPack()
    {
        return activeDataPack;
    }
}

