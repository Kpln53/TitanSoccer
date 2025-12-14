using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Takım yönetimi - oyuncuları spawn eder ve yönetir
/// </summary>
public class TeamManager : MonoBehaviour
{
    // Singleton kaldırıldı - her takım kendi instance'ına sahip olacak
    public static TeamManager HomeTeam { get; private set; }
    public static TeamManager AwayTeam { get; private set; }

    [Header("Takım Ayarları")]
    public bool isHomeTeam;
    [SerializeField] private PositionConfig positionConfig;
    [SerializeField] private TeamData teamData; // Data Pack'ten gelen takım verisi

    /// <summary>
    /// PositionConfig'i ayarla (runtime'da)
    /// </summary>
    public void SetPositionConfig(PositionConfig config)
    {
        positionConfig = config;
    }

    [Header("Oyuncu Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Takım Renkleri")]
    [SerializeField] private Material teamMaterial;

    private List<Player> players = new List<Player>();
    private Dictionary<PlayerPosition, Player> playersByPosition = new Dictionary<PlayerPosition, Player>();
    private int teamPower = 50; // Takım gücü

    void Awake()
    {
        // Home ve Away team referanslarını ayarla
        if (isHomeTeam)
        {
            if (HomeTeam != null && HomeTeam != this)
            {
                Destroy(gameObject);
                return;
            }
            HomeTeam = this;
        }
        else
        {
            if (AwayTeam != null && AwayTeam != this)
            {
                Destroy(gameObject);
                return;
            }
            AwayTeam = this;
        }
    }

    void Start()
    {
        // PositionConfig atanana kadar bekle
        StartCoroutine(WaitForPositionConfigAndSpawn());
    }

    System.Collections.IEnumerator WaitForPositionConfigAndSpawn()
    {
        // PositionConfig atanana kadar bekle (max 5 saniye)
        float timeout = 5f;
        float elapsed = 0f;
        
        while (positionConfig == null && elapsed < timeout)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        if (positionConfig == null)
        {
            Debug.LogError($"[TeamManager] PositionConfig atanmamış! (isHomeTeam: {isHomeTeam})");
            yield break;
        }

        Debug.Log($"[TeamManager] Takım spawn başlıyor... (isHomeTeam: {isHomeTeam})");
        SpawnTeam();
        Debug.Log($"[TeamManager] Takım spawn tamamlandı. Toplam {players.Count} oyuncu.");
    }

    /// <summary>
    /// Takımı spawn et
    /// </summary>
    void SpawnTeam()
    {
        if (positionConfig.positions == null || positionConfig.positions.Length == 0)
        {
            Debug.LogError($"[TeamManager] PositionConfig'de pozisyon tanımları yok! (isHomeTeam: {isHomeTeam})");
            return;
        }

        Debug.Log($"[TeamManager] {positionConfig.positions.Length} pozisyon bulundu.");

        PitchManager pitch = PitchManager.Instance;
        if (pitch == null)
        {
            Debug.LogError($"[TeamManager] PitchManager bulunamadı! (isHomeTeam: {isHomeTeam})");
            return;
        }

        foreach (var posData in positionConfig.positions)
        {
            if (posData == null)
            {
                Debug.LogWarning($"[TeamManager] Null pozisyon verisi atlandı!");
                continue;
            }
            
            Debug.Log($"[TeamManager] Spawning: {posData.position} at ({posData.normalizedPosition.x}, {posData.normalizedPosition.y})");
            SpawnPlayer(posData, pitch);
        }
    }

    void SpawnPlayer(PositionData posData, PitchManager pitch)
    {
        // Normalize pozisyonu world pozisyonuna çevir (2D)
        Vector2 worldPos = positionConfig.NormalizedToWorldPosition(posData.normalizedPosition, pitch);

        // Ev sahibi takım için Y pozisyonunu ters çevir (kendi yarı sahasında - alt)
        if (isHomeTeam)
        {
            worldPos.y = -worldPos.y; // Ev sahibi alt yarı sahada
        }

        Debug.Log($"[TeamManager] Spawning {posData.position} at world pos: {worldPos}");

        // Oyuncuyu oluştur (2D)
        GameObject playerObj;
        if (playerPrefab != null)
        {
            playerObj = Instantiate(playerPrefab, (Vector3)worldPos, Quaternion.identity, transform);
        }
        else
        {
            playerObj = new GameObject($"Player_{posData.position}");
            playerObj.transform.position = (Vector3)worldPos;
            playerObj.transform.SetParent(transform);
        }

        Player player = playerObj.GetComponent<Player>();
        if (player == null)
        {
            player = playerObj.AddComponent<Player>();
        }
        
        // Pozisyonu tekrar ayarla (Player Awake'den sonra)
        playerObj.transform.position = (Vector3)worldPos;

        // Oyuncu bilgilerini ayarla
        player.position = posData.position;
        player.isHomeTeam = isHomeTeam;
        
        // Data Pack'ten oyuncu verisi varsa kullan
        PlayerData playerData = null;
        if (teamData != null)
        {
            playerData = teamData.GetPlayerByPosition(posData.position);
        }
        
        if (playerData != null)
        {
            // Data Pack'ten oyuncu verilerini yükle
            player.playerName = playerData.playerName;
            player.overall = playerData.overall;
        }
        else
        {
            // Varsayılan: rastgele overall
            player.overall = Random.Range(60, 85);
            player.playerName = $"{posData.position}_{(isHomeTeam ? "Home" : "Away")}";
        }

        players.Add(player);
        playersByPosition[posData.position] = player;

        // Takım rengini uygula (2D - SpriteRenderer)
        SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Data Pack'ten renk varsa kullan
            if (teamData != null)
            {
                sr.color = teamData.primaryColor;
            }
            else if (teamMaterial != null)
            {
                // Material yerine color kullan (2D için)
                sr.color = isHomeTeam ? Color.blue : Color.red;
            }
            else
            {
                sr.color = isHomeTeam ? Color.blue : Color.red;
            }
        }
    }
    
    /// <summary>
    /// Takım verisini ayarla (Data Pack'ten)
    /// </summary>
    public void SetTeamData(TeamData data)
    {
        teamData = data;
        if (teamData != null)
        {
            teamPower = teamData.GetTeamPower();
        }
    }
    
    /// <summary>
    /// Takım gücünü al
    /// </summary>
    public int GetTeamPower()
    {
        return teamPower;
    }
    
    /// <summary>
    /// Takım verisini al
    /// </summary>
    public TeamData GetTeamData()
    {
        return teamData;
    }

    /// <summary>
    /// Belirli bir pozisyondaki oyuncuyu getir
    /// </summary>
    public Player GetPlayerByPosition(PlayerPosition position)
    {
        playersByPosition.TryGetValue(position, out Player player);
        return player;
    }

    /// <summary>
    /// Tüm oyuncuları getir
    /// </summary>
    public List<Player> GetAllPlayers()
    {
        return new List<Player>(players);
    }

    /// <summary>
    /// En yakın oyuncuyu bul (2D)
    /// </summary>
    public Player GetNearestPlayer(Vector2 position)
    {
        Player nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var player in players)
        {
            float distance = Vector2.Distance(position, (Vector2)player.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = player;
            }
        }

        return nearest;
    }
}

