using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Saha yönetimi - 22 oyuncuyu sahaya yerleştirir ve yönetir
/// </summary>
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }

    [Header("Saha Ölçüleri")]
    public float fieldWidth = 120f;
    public float fieldHeight = 120f;
    
    [Header("Saha Pozisyonu")]
    public Vector2 fieldCenter = new Vector2(540.6165f, 963.9995f); // Sahanın merkez pozisyonu
    
    [Header("Oyuncular")]
    public GameObject playerPrefab;
    public List<PlayerController> homePlayers = new List<PlayerController>();
    public List<PlayerController> awayPlayers = new List<PlayerController>();
    private PlayerController controlledPlayer; // Oyuncu tarafından kontrol edilen oyuncu

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        InitializeField();
    }

    /// <summary>
    /// Sahayı ve oyuncuları başlat
    /// </summary>
    private void InitializeField()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("[FieldManager] No save data available!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;
        
        // Takım bilgilerini al
        string playerTeam = saveData.clubData.clubName;
        MatchData currentMatch = SeasonCalendarSystem.Instance?.GetNextMatch(saveData);
        
        if (currentMatch == null)
        {
            Debug.LogError("[FieldManager] No match data available!");
            return;
        }

        bool isPlayerHome = currentMatch.homeTeam == playerTeam;

        // Oyuncuları sahaya yerleştir (basit formasyon)
        // NOT: Oyuncular her zaman world space'te (0,0) civarında spawn olur
        // Field görsel olarak Canvas altında olabilir ama oyuncular world space'te
        CreatePlayers(isPlayerHome);
    }

    /// <summary>
    /// Oyuncuları oluştur ve mevkilerine göre yerleştir
    /// </summary>
    private void CreatePlayers(bool isPlayerHome)
    {
        // Basit 4-4-2 formasyonu (şimdilik)
        // Home Team
        Vector2[] homePositions = GetFormationPositions(true, isPlayerHome);
        for (int i = 0; i < 11; i++)
        {
            // Oyuncuları sahanın merkez pozisyonuna göre yerleştir
            Vector3 spawnPosition = new Vector3(homePositions[i].x + fieldCenter.x, homePositions[i].y + fieldCenter.y, 0);
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            
            PlayerController player = playerObj.GetComponent<PlayerController>();
            
            if (player == null)
                player = playerObj.AddComponent<PlayerController>();
            
            player.playerId = i;
            player.position = GetPositionForIndex(i);
            
            // Oyuncu tarafından kontrol edilen oyuncuyu işaretle
            // Oyuncunun pozisyonuna göre hangi oyuncunun kontrol edileceğini belirle
            if (isPlayerHome)
            {
                SaveData saveData = GameManager.Instance?.CurrentSave;
                if (saveData?.playerProfile != null)
                {
                    PlayerPosition playerPosition = saveData.playerProfile.position;
                    PlayerPosition currentPlayerPosition = GetPositionForIndex(i);
                    
                    // Eğer bu oyuncunun pozisyonu oyuncunun pozisyonu ile eşleşiyorsa kontrol et
                    if (currentPlayerPosition == playerPosition)
                    {
                        player.isPlayerControlled = true;
                        controlledPlayer = player;
                        Debug.Log($"[FieldManager] Player controlled character set: Position {playerPosition}, Index {i}, GameObject: {player.gameObject.name}");
                    }
                }
                else
                {
                    // Fallback: Eğer save data yoksa merkez orta sahayı kontrol et
                    if (i == 5) // Merkez orta saha (örnek)
                    {
                        player.isPlayerControlled = true;
                        controlledPlayer = player;
                        Debug.Log($"[FieldManager] No save data, using default controlled player (index 5), GameObject: {player.gameObject.name}");
                    }
                }
            }
            
            homePlayers.Add(player);
        }

        // Away Team
        Vector2[] awayPositions = GetFormationPositions(false, isPlayerHome);
        for (int i = 0; i < 11; i++)
        {
            // Oyuncuları sahanın merkez pozisyonuna göre yerleştir
            Vector3 spawnPosition = new Vector3(awayPositions[i].x + fieldCenter.x, awayPositions[i].y + fieldCenter.y, 0);
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            
            PlayerController player = playerObj.GetComponent<PlayerController>();
            
            if (player == null)
                player = playerObj.AddComponent<PlayerController>();
            
            player.playerId = i + 11;
            player.position = GetPositionForIndex(i);
            
            awayPlayers.Add(player);
        }
    }

    /// <summary>
    /// Formasyon pozisyonlarını al (4-4-2) - 120x120 sahaya göre düzgün dağılım
    /// </summary>
    private Vector2[] GetFormationPositions(bool isHome, bool isPlayerHome)
    {
        Vector2[] positions = new Vector2[11];
        
        // Saha merkezine göre relative pozisyonlar (fieldCenter'e eklenecek)
        // Saha 120x120, yani merkezden -60 ile +60 arası
        // Home takım aşağıda (negatif Y), Away takım yukarıda (pozitif Y)
        
        // Saha sınırları: merkezden -60 ile +60
        float sahaMinY = -60f;
        float sahaMaxY = 60f;
        float sahaMinX = -60f;
        float sahaMaxX = 60f;
        
        // Kaleci - Sahanın en arkasında (kendi kalemizde), tam merkezde
        float kaleciY = isHome ? sahaMinY + 5f : sahaMaxY - 5f;
        positions[0] = new Vector2(0f, kaleciY);
        
        // Defans (4) - Kaleci önünde, sahanın genişliğini kullanarak düzgün dağılım
        float defansY = isHome ? sahaMinY + 18f : sahaMaxY - 18f;
        positions[1] = new Vector2(sahaMinX + 12f, defansY); // Sol bek (-48)
        positions[2] = new Vector2(sahaMinX + 30f, defansY); // Sol stoper (-30)
        positions[3] = new Vector2(sahaMaxX - 30f, defansY);  // Sağ stoper (+30)
        positions[4] = new Vector2(sahaMaxX - 12f, defansY);  // Sağ bek (+48)
        
        // Orta saha (4) - Saha ortasında, geniş ve düzgün dağılım
        float ortaSahaY = isHome ? -12f : 12f;
        positions[5] = new Vector2(sahaMinX + 18f, ortaSahaY); // Sol orta (-42)
        positions[6] = new Vector2(sahaMinX + 38f, ortaSahaY); // Sol merkez (-22)
        positions[7] = new Vector2(sahaMaxX - 38f, ortaSahaY);  // Sağ merkez (+22)
        positions[8] = new Vector2(sahaMaxX - 18f, ortaSahaY);   // Sağ orta (+42)
        
        // Forvet (2) - Rakip kaleye yakın, sahanın genişliğini kullan
        float forvetY = isHome ? sahaMaxY - 18f : sahaMinY + 18f;
        positions[9] = new Vector2(sahaMinX + 25f, forvetY); // Sol forvet (-35)
        positions[10] = new Vector2(sahaMaxX - 25f, forvetY); // Sağ forvet (+35)
        
        return positions;
    }

    /// <summary>
    /// İndeks için pozisyon enum'u al
    /// </summary>
    private PlayerPosition GetPositionForIndex(int index)
    {
        switch (index)
        {
            case 0: return PlayerPosition.KL;
            case 1: return PlayerPosition.SLB;
            case 2: return PlayerPosition.STP;
            case 3: return PlayerPosition.STP;
            case 4: return PlayerPosition.SĞB;
            case 5: return PlayerPosition.SLO;
            case 6: return PlayerPosition.MDO;
            case 7: return PlayerPosition.MOO;
            case 8: return PlayerPosition.SĞO;
            case 9: return PlayerPosition.SF;
            case 10: return PlayerPosition.SF;
            default: return PlayerPosition.MOO;
        }
    }

    /// <summary>
    /// Kontrol edilen oyuncuyu al
    /// </summary>
    public PlayerController GetControlledPlayer()
    {
        if (controlledPlayer == null)
        {
            Debug.LogWarning("[FieldManager] GetControlledPlayer() called but controlledPlayer is NULL!");
        }
        return controlledPlayer;
    }

    /// <summary>
    /// Takım arkadaşlarını al (pas için)
    /// </summary>
    public List<PlayerController> GetTeammates(PlayerController player)
    {
        if (homePlayers.Contains(player))
            return homePlayers;
        else
            return awayPlayers;
    }
}
