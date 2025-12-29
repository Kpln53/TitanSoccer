using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Saha yönetimi - Oyuncuları spawn eder, pozisyonlarını belirler
/// </summary>
public class FieldManager : MonoBehaviour
{
    [Header("Saha Ayarları")]
    [SerializeField] private float fieldWidth = 20f;  // Saha genişliği (X ekseni)
    [SerializeField] private float fieldLength = 30f; // Saha uzunluğu (Y ekseni)

    [Header("Prefab Referansları")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ballPrefab;

    [Header("Formasyon")]
    [SerializeField] private FormationType formation = FormationType.FourFourTwo;

    // Spawn edilen oyuncular
    private List<PlayerController> homePlayers = new List<PlayerController>();
    private List<PlayerController> awayPlayers = new List<PlayerController>();
    private PlayerController playerCharacter; // Oyuncunun kontrol ettiği karakter
    private BallController ball;

    // Events
    public System.Action<PlayerController> OnPlayerCharacterSpawned; // Oyuncu karakteri spawn edildiğinde
    public System.Action<BallController> OnBallSpawned;

    private void Start()
    {
        // Saha görseli oluştur
        CreateFieldVisual();
        
        // Prefab'ları bul veya oluştur
        FindOrCreatePrefabs();
        
        // Takımları spawn et
        SpawnTeams();
        
        // Topu spawn et
        SpawnBall();
    }

    /// <summary>
    /// Saha görseli oluştur (yeşil zemin)
    /// </summary>
    void CreateFieldVisual()
    {
        GameObject fieldObj = new GameObject("FieldVisual");
        fieldObj.transform.position = Vector3.zero;
        
        SpriteRenderer sr = fieldObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateFieldSprite();
        sr.sortingOrder = -10; // En arkada
        sr.color = new Color(0.2f, 0.7f, 0.3f, 1f); // Yeşil
        
        // Saha boyutuna göre scale
        float spriteWidth = 20f;
        float spriteHeight = 30f;
        fieldObj.transform.localScale = new Vector3(fieldWidth / spriteWidth, fieldLength / spriteHeight, 1f);
    }

    /// <summary>
    /// Saha sprite'ı oluştur
    /// </summary>
    Sprite CreateFieldSprite()
    {
        int width = 200;
        int height = 300;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        Color fieldGreen = new Color(0.2f, 0.7f, 0.3f, 1f);
        
        // Basit çizgilerle futbol sahası deseni
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color pixelColor = fieldGreen;
                
                // Orta çizgi
                if (Mathf.Abs(y - height / 2) < 2)
                {
                    pixelColor = Color.white;
                }
                
                // Orta yuvarlak
                Vector2 center = new Vector2(width / 2f, height / 2f);
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist > 25f && dist < 27f)
                {
                    pixelColor = Color.white;
                }
                
                // Kale alanları (kısa kenarlarda)
                if ((y < 30 || y > height - 30) && Mathf.Abs(x - width / 2f) < 35f)
                {
                    pixelColor = new Color(0.15f, 0.6f, 0.25f, 1f); // Daha koyu yeşil
                }
                
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    /// <summary>
    /// Prefab'ları bul veya runtime'da oluştur
    /// </summary>
    void FindOrCreatePrefabs()
    {
        // Player prefab bul veya oluştur
        if (playerPrefab == null)
        {
            // Resources'tan yükle
            playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
            
            // Yoksa runtime'da oluştur
            if (playerPrefab == null)
            {
                playerPrefab = CreateDefaultPlayerPrefab();
            }
        }

        // Ball prefab bul veya oluştur
        if (ballPrefab == null)
        {
            ballPrefab = Resources.Load<GameObject>("Prefabs/Ball");
            
            if (ballPrefab == null)
            {
                ballPrefab = CreateDefaultBallPrefab();
            }
        }
    }

    /// <summary>
    /// Takımları spawn et
    /// </summary>
    void SpawnTeams()
    {
        // Ev sahibi takım
        SpawnTeam(true);
        
        // Deplasman takımı
        SpawnTeam(false);
        
        // Oyuncu karakterini bul
        FindPlayerCharacter();
    }

    /// <summary>
    /// Takım spawn et (basit 4-4-2 formasyonu)
    /// </summary>
    void SpawnTeam(bool isHomeTeam)
    {
        List<PlayerController> teamList = isHomeTeam ? homePlayers : awayPlayers;
        
        // Formasyon pozisyonları (4-4-2)
        // Y ekseni: Home team üstte (pozitif Y), Away team altta (negatif Y)
        float teamY = isHomeTeam ? fieldLength * 0.3f : -fieldLength * 0.3f;
        int direction = isHomeTeam ? 1 : -1; // Home team yukarı, Away team aşağı bakar

        // Kaleci (1)
        Vector2 gkPos = new Vector2(0, isHomeTeam ? fieldLength * 0.45f : -fieldLength * 0.45f);
        SpawnPlayer(PlayerPosition.KL, gkPos, isHomeTeam, teamList);

        // Stoperler (2)
        SpawnPlayer(PlayerPosition.STP, new Vector2(-3f, teamY), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.STP, new Vector2(3f, teamY), isHomeTeam, teamList);

        // Bekler (2)
        SpawnPlayer(PlayerPosition.SĞB, new Vector2(-6f, teamY * 0.7f), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.SLB, new Vector2(6f, teamY * 0.7f), isHomeTeam, teamList);

        // Orta saha (4)
        SpawnPlayer(PlayerPosition.MDO, new Vector2(-2f, teamY * 0.4f), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.MOO, new Vector2(2f, teamY * 0.4f), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.SĞO, new Vector2(-5f, teamY * 0.5f), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.SLO, new Vector2(5f, teamY * 0.5f), isHomeTeam, teamList);

        // Forvetler (2)
        SpawnPlayer(PlayerPosition.SF, new Vector2(-1.5f, teamY * 0.2f), isHomeTeam, teamList);
        SpawnPlayer(PlayerPosition.SF, new Vector2(1.5f, teamY * 0.2f), isHomeTeam, teamList);

        Debug.Log($"[FieldManager] {(isHomeTeam ? "Home" : "Away")} takım spawn edildi: {teamList.Count} oyuncu");
    }

    /// <summary>
    /// Oyuncu spawn et
    /// </summary>
    PlayerController SpawnPlayer(PlayerPosition position, Vector2 position2D, bool isHomeTeam, List<PlayerController> teamList)
    {
        GameObject playerObj = Instantiate(playerPrefab);
        playerObj.name = $"{(isHomeTeam ? "Home" : "Away")}_{position}";
        playerObj.transform.position = new Vector3(position2D.x, position2D.y, 0f);

        PlayerController controller = playerObj.GetComponent<PlayerController>();
        if (controller == null)
        {
            controller = playerObj.AddComponent<PlayerController>();
        }

        // Tag ayarla
        playerObj.tag = "Player";

        // Renk ayarla (görsel)
        SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = isHomeTeam ? Color.blue : Color.red;
        }

        teamList.Add(controller);

        return controller;
    }

    /// <summary>
    /// Oyuncu karakterini bul (SaveData'dan pozisyon bilgisi ile)
    /// </summary>
    void FindPlayerCharacter()
    {
        // SaveData'dan oyuncunun pozisyonunu al
        PlayerPosition targetPlayerPosition = PlayerPosition.SF; // Varsayılan: Santrafor

        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            PlayerProfile profile = GameManager.Instance.CurrentSave.playerProfile;
            if (profile != null)
            {
                targetPlayerPosition = profile.position;
            }
        }

        // Home takımından oyuncuyu bul
        // SaveData'dan pozisyon bilgisi ile bul veya ilk forveti al
        bool foundPlayer = false;
        
        foreach (var player in homePlayers)
        {
            // Pozisyon kontrolü yap (name'den pozisyonu çıkar)
            string playerName = player.name.ToLower();
            bool matchesPosition = false;
            
            // Pozisyon kontrolü
            if (targetPlayerPosition == PlayerPosition.SF)
                matchesPosition = playerName.Contains("sf");
            else if (targetPlayerPosition == PlayerPosition.MOO)
                matchesPosition = playerName.Contains("moo");
            else if (targetPlayerPosition == PlayerPosition.MDO)
                matchesPosition = playerName.Contains("mdo");
            else if (targetPlayerPosition == PlayerPosition.SĞK)
                matchesPosition = playerName.Contains("sğk") || playerName.Contains("sagk");
            else if (targetPlayerPosition == PlayerPosition.SLK)
                matchesPosition = playerName.Contains("slk") || playerName.Contains("solk");
            // ... diğer pozisyonlar için de eklenebilir
            
            if (matchesPosition)
            {
                playerCharacter = player;
                foundPlayer = true;
                break;
            }
        }
        
        // Pozisyonla eşleşen bulunamazsa, forvet (SF) bul
        if (!foundPlayer)
        {
            foreach (var player in homePlayers)
            {
                if (player.name.ToLower().Contains("sf"))
                {
                    playerCharacter = player;
                    foundPlayer = true;
                    break;
                }
            }
        }
        
        // Hala bulunamazsa ilk oyuncuyu al
        if (!foundPlayer && homePlayers.Count > 0)
        {
            playerCharacter = homePlayers[0];
        }

        if (playerCharacter != null)
        {
            // Oyuncu karakterini vurgula (sarı renk)
            SpriteRenderer sr = playerCharacter.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.yellow;
                // Sorting order'ı artır (diğer oyuncuların üstünde görünsün)
                sr.sortingOrder = 10;
            }

            OnPlayerCharacterSpawned?.Invoke(playerCharacter);
            Debug.Log($"[FieldManager] Oyuncu karakteri bulundu ve sarı ile işaretlendi: {playerCharacter.name}");
        }
        else
        {
            Debug.LogWarning("[FieldManager] Oyuncu karakteri bulunamadı!");
        }
    }

    /// <summary>
    /// Topu spawn et
    /// </summary>
    void SpawnBall()
    {
        // Zaten top varsa yeni spawn etme (2 top sorununu önle)
        BallController existingBall = FindObjectOfType<BallController>();
        if (existingBall != null && existingBall.gameObject != null)
        {
            Debug.Log("[FieldManager] Top zaten var, yeni spawn edilmedi. Mevcut top: " + existingBall.gameObject.name);
            ball = existingBall;
            OnBallSpawned?.Invoke(ball);
            return;
        }

        GameObject ballObj = Instantiate(ballPrefab);
        ballObj.name = "Ball";
        ballObj.tag = "Ball"; // Tag'ı ayarla
        ballObj.transform.position = Vector3.zero;

        ball = ballObj.GetComponent<BallController>();
        if (ball == null)
        {
            ball = ballObj.AddComponent<BallController>();
        }

        // Başlangıçta topu oyuncu karakterinin yanına koy
        if (playerCharacter != null)
        {
            Vector2 playerPos = playerCharacter.Position;
            ball.SetPosition(playerPos + Vector2.right * 0.5f);
        }

        OnBallSpawned?.Invoke(ball);
        Debug.Log("[FieldManager] Top spawn edildi");
    }

    /// <summary>
    /// Default player prefab oluştur (runtime'da)
    /// </summary>
    GameObject CreateDefaultPlayerPrefab()
    {
        GameObject prefab = new GameObject("Player");
        
        // SpriteRenderer
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = CreateDefaultPlayerSprite();
        sr.sortingOrder = 1;

        // Rigidbody2D
        Rigidbody2D rb = prefab.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // CircleCollider2D
        CircleCollider2D col = prefab.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;

        // PlayerController
        prefab.AddComponent<PlayerController>();

        // Tag
        prefab.tag = "Player";

        // Layer
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
        {
            prefab.layer = playerLayer;
        }

        return prefab;
    }

    /// <summary>
    /// Default ball prefab oluştur (runtime'da)
    /// </summary>
    GameObject CreateDefaultBallPrefab()
    {
        GameObject prefab = new GameObject("Ball");

        // SpriteRenderer
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = CreateDefaultBallSprite();
        sr.sortingOrder = 2;

        // Rigidbody2D
        Rigidbody2D rb = prefab.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 2f;
        rb.mass = 0.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // CircleCollider2D (trigger)
        CircleCollider2D col = prefab.AddComponent<CircleCollider2D>();
        col.radius = 0.3f;
        col.isTrigger = true;

        // BallController
        prefab.AddComponent<BallController>();

        // Tag
        prefab.tag = "Ball";

        // Layer
        int ballLayer = LayerMask.NameToLayer("Ball");
        if (ballLayer != -1)
        {
            prefab.layer = ballLayer;
        }

        return prefab;
    }

    /// <summary>
    /// Default oyuncu sprite'ı oluştur
    /// </summary>
    Sprite CreateDefaultPlayerSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        Color color = Color.blue;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius)
                {
                    float alpha = dist > radius - 2f ? 1f - ((dist - (radius - 2f)) / 2f) : 1f;
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    /// <summary>
    /// Default top sprite'ı oluştur
    /// </summary>
    Sprite CreateDefaultBallSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                if (dist <= radius)
                {
                    // Futbol topu deseni (siyah-beyaz)
                    bool isBlack = ((x + y) % 8 < 4);
                    Color color = isBlack ? Color.black : Color.white;
                    float alpha = 1f;
                    if (dist > radius - 1f)
                    {
                        alpha = 1f - (dist - (radius - 1f));
                    }
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    // Getters
    public float FieldWidth => fieldWidth;
    public float FieldLength => fieldLength;
    public List<PlayerController> HomePlayers => homePlayers;
    public List<PlayerController> AwayPlayers => awayPlayers;
    public PlayerController PlayerCharacter => playerCharacter;
    public BallController Ball => ball;
}

/// <summary>
/// Formasyon türleri
/// </summary>
public enum FormationType
{
    FourFourTwo,    // 4-4-2
    FourThreeThree, // 4-3-3
    ThreeFiveTwo,   // 3-5-2
    FourTwoFour     // 4-2-4
}
