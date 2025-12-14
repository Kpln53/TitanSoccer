using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// MatchChanceScene yönetimi - pozisyon geldiğinde sahneyi kurar
/// </summary>
public class MatchChanceSceneManager : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private PitchManager pitchManager;
    [SerializeField] private TeamManager homeTeamManager;
    [SerializeField] private TeamManager awayTeamManager;
    [SerializeField] private MatchCamera matchCamera;
    [SerializeField] private GameObject ballPrefab;

    [Header("Spawn Ayarları")]
    [SerializeField] private bool spawnOnStart = true;

    private GameObject currentBall;
    private Player playerCharacter; // Oyuncunun kontrol ettiği karakter

    void Start()
    {
        if (spawnOnStart)
        {
            InitializeScene();
        }
    }

    void InitializeScene()
    {
        // PitchManager'ı kontrol et
        if (pitchManager == null)
        {
            pitchManager = FindObjectOfType<PitchManager>();
            if (pitchManager == null)
            {
                GameObject pitchObj = new GameObject("PitchManager");
                pitchManager = pitchObj.AddComponent<PitchManager>();
            }
        }

        // Takımları spawn et
        SpawnTeams();

        // Topu spawn et
        SpawnBall();

        // Kamerayı ayarla (temel ayarlar)
        SetupCamera();

        // Oyuncu karakterini bul (SaveData'dan pozisyona göre)
        // Takımlar spawn edildikten sonra çağrılmalı
        StartCoroutine(FindPlayerCharacterDelayed());
        
        // AI oyuncuları ayarla
        StartCoroutine(SetupAIPlayersDelayed());
    }

    void SpawnTeams()
    {
        // PositionConfig'i yükle
        PositionConfig positionConfig = Resources.Load<PositionConfig>("PositionConfig");
        if (positionConfig == null)
        {
            // Asset path'ten yükle (runtime'da çalışmaz ama editor'da test için)
            #if UNITY_EDITOR
            positionConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<PositionConfig>("Assets/Scripts/Data/PositionConfig.asset");
            #endif
        }

        if (positionConfig == null)
        {
            Debug.LogError("[MatchChanceSceneManager] PositionConfig bulunamadı! Lütfen Unity Editor'da 'TitanSoccer > Fix Position Config' menüsünü çalıştırın.");
        }

        // DataPackManager'dan takımları al
        DataPackManager dataPackManager = DataPackManager.Instance;
        TeamData homeTeamData = null;
        TeamData awayTeamData = null;
        
        if (dataPackManager != null)
        {
            // Varsayılan olarak ilk iki takımı al (sonra maç sistemi ile değiştirilebilir)
            var allTeams = dataPackManager.GetAllTeams();
            if (allTeams.Count >= 2)
            {
                homeTeamData = allTeams[0];
                awayTeamData = allTeams[1];
            }
            else if (allTeams.Count >= 1)
            {
                homeTeamData = allTeams[0];
            }
        }

        // Ev sahibi takım
        if (homeTeamManager == null)
        {
            GameObject homeTeamObj = new GameObject("HomeTeam");
            homeTeamManager = homeTeamObj.AddComponent<TeamManager>();
            homeTeamManager.isHomeTeam = true; // Ev sahibi takım
            if (positionConfig != null)
            {
                homeTeamManager.SetPositionConfig(positionConfig);
            }
            if (homeTeamData != null)
            {
                homeTeamManager.SetTeamData(homeTeamData);
            }
        }

        // Deplasman takımı
        if (awayTeamManager == null)
        {
            GameObject awayTeamObj = new GameObject("AwayTeam");
            awayTeamManager = awayTeamObj.AddComponent<TeamManager>();
            awayTeamManager.isHomeTeam = false; // Deplasman takımı
            if (positionConfig != null)
            {
                awayTeamManager.SetPositionConfig(positionConfig);
            }
            if (awayTeamData != null)
            {
                awayTeamManager.SetTeamData(awayTeamData);
            }
        }
    }

    void SpawnBall()
    {
        if (ballPrefab == null)
        {
            // 2D top oluştur
            currentBall = new GameObject("Ball");
            currentBall.transform.position = Vector3.zero;
            
            // Tag ekle (AI'ların bulması için)
            currentBall.tag = "Ball";
            
            // SpriteRenderer ekle
            SpriteRenderer sr = currentBall.AddComponent<SpriteRenderer>();
            
            // Runtime'da top sprite'ı oluştur
            sr.sprite = CreateDefaultBallSprite();
            sr.sortingOrder = 2; // Oyuncuların üstünde
            
            // Rigidbody2D ekle
            Rigidbody2D rb = currentBall.AddComponent<Rigidbody2D>();
            rb.mass = 0.43f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            rb.gravityScale = 0f; // Yerçekimi kapalı (2D)
            
            // 2D için constraints - sadece X ve Y eksenlerinde hareket (Z sabit)
            // Not: Rigidbody2D zaten 2D'de çalışır, ama rotasyonu donduralım
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Rotasyonu dondur
            
            // CircleCollider2D ekle
            CircleCollider2D col = currentBall.AddComponent<CircleCollider2D>();
            col.radius = 0.11f; // ~22cm yarıçap
        }
        else
        {
            currentBall = Instantiate(ballPrefab);
            currentBall.tag = "Ball";
        }

        // Topu sahanın ortasına yerleştir (2D - kesinlikle 0, 0, 0)
        // Parent'ı null yap (parent transform pozisyonu etkileyebilir)
        currentBall.transform.SetParent(null);
        currentBall.transform.position = new Vector3(0f, 0f, 0f);
        currentBall.transform.localPosition = Vector3.zero;
        currentBall.transform.localRotation = Quaternion.identity;
        
        // Rigidbody2D pozisyonunu da ayarla
        Rigidbody2D ballRb = currentBall.GetComponent<Rigidbody2D>();
        if (ballRb != null)
        {
            ballRb.position = Vector2.zero;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
        
        // Pozisyonu bir sonraki frame'de de kontrol et (bazı scriptler değiştirebilir)
        StartCoroutine(EnsureBallPosition());
        
        // Debug: Kale pozisyonlarını da logla
        PitchManager pitch = PitchManager.Instance;
        if (pitch != null)
        {
            Debug.Log($"[MatchChanceSceneManager] Top spawn edildi. Pozisyon: {currentBall.transform.position}");
            Debug.Log($"[MatchChanceSceneManager] Home Goal: {pitch.HomeGoalPosition}, Away Goal: {pitch.AwayGoalPosition}");
            Debug.Log($"[MatchChanceSceneManager] Saha boyutları: {pitch.PitchWidth} x {pitch.PitchLength}");
        }
        else
        {
            Debug.LogWarning("[MatchChanceSceneManager] PitchManager bulunamadı! Top pozisyonu kontrol edilemiyor.");
        }
    }

    void SetupCamera()
    {
        if (matchCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                matchCamera = mainCam.GetComponent<MatchCamera>();
                if (matchCamera == null)
                {
                    matchCamera = mainCam.gameObject.AddComponent<MatchCamera>();
                }
            }
        }

        if (matchCamera != null && currentBall != null)
        {
            matchCamera.SetBall(currentBall.transform);
            // Başlangıçta genel görünüm modunda başla (tüm sahneyi görmek için)
            matchCamera.EnterOverviewMode();
        }
    }

    /// <summary>
    /// Oyuncu karakterini bul (takımlar spawn edildikten sonra)
    /// </summary>
    IEnumerator FindPlayerCharacterDelayed()
    {
        // Takımların spawn edilmesini bekle
        yield return new WaitForSeconds(0.1f);
        
        // HomeTeamManager'ın hazır olmasını bekle
        while (homeTeamManager == null || homeTeamManager.GetAllPlayers().Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        FindPlayerCharacter();
    }

    void FindPlayerCharacter()
    {
        PlayerPosition targetPosition = PlayerPosition.SF; // Varsayılan: Santrafor
        
        // SaveData'dan oyuncunun pozisyonunu al
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            string playerPosString = GameManager.Instance.CurrentSave.position;
            
            // String'i PlayerPosition enum'a çevir
            if (System.Enum.TryParse<PlayerPosition>(playerPosString, out PlayerPosition playerPos))
            {
                targetPosition = playerPos;
                Debug.Log($"[MatchChanceSceneManager] SaveData'dan pozisyon bulundu: {targetPosition}");
            }
            else
            {
                Debug.LogWarning($"[MatchChanceSceneManager] Geçersiz pozisyon string: {playerPosString}. Varsayılan kullanılıyor: {targetPosition}");
            }
        }
        else
        {
            Debug.LogWarning("[MatchChanceSceneManager] SaveData bulunamadı. Varsayılan pozisyon kullanılıyor: SF (Santrafor)");
        }

        // Ev sahibi takımdan oyuncuyu bul
        if (homeTeamManager != null)
        {
            playerCharacter = homeTeamManager.GetPlayerByPosition(targetPosition);
            
            if (playerCharacter == null)
            {
                Debug.LogWarning($"[MatchChanceSceneManager] {targetPosition} pozisyonunda oyuncu bulunamadı. İlk oyuncu seçiliyor.");
                var players = homeTeamManager.GetAllPlayers();
                if (players.Count > 0)
                {
                    playerCharacter = players[0];
                }
            }
        }

        if (playerCharacter != null)
        {
            Debug.Log($"[MatchChanceSceneManager] Oyuncu karakteri bulundu: {playerCharacter.playerName} ({playerCharacter.position})");
            
            // Oyuncu kontrol sistemini ayarla
            SetupPlayerController();
            
            // Kamerayı oyuncuya odakla
            FocusCameraOnPlayer();
        }
        else
        {
            Debug.LogError("[MatchChanceSceneManager] Oyuncu karakteri bulunamadı!");
        }
    }

    void SetupPlayerController()
    {
        if (playerCharacter == null) return;

        // PlayerController component'ini kontrol et
        PlayerController controller = playerCharacter.GetComponent<PlayerController>();
        if (controller == null)
        {
            controller = playerCharacter.gameObject.AddComponent<PlayerController>();
        }

        // Kontrolü aktif et
        controller.SetControlledPlayer(playerCharacter);
        controller.SetControlled(true);

        // Oyuncu karakterini görsel olarak ayırt et
        HighlightPlayerCharacter();

        Debug.Log($"Oyuncu kontrolü ayarlandı: {playerCharacter.playerName} ({playerCharacter.position})");
        Debug.Log("Oyuncu hareket ederken oyun duracak. Hedefe vardığında bir sonraki hareketi bekleyecek.");
    }

    /// <summary>
    /// Oyuncu karakterini görsel olarak vurgula (farklı renk/işaret)
    /// </summary>
    void HighlightPlayerCharacter()
    {
        if (playerCharacter == null) return;

        SpriteRenderer sr = playerCharacter.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Oyuncu karakterini sarı renkle vurgula (diğer oyuncular mavi/kırmızı)
            sr.color = Color.yellow;
            
            // Sorting order'ı artır (üstte görünsün)
            sr.sortingOrder = 5;
            
            // Scale'i biraz büyüt (daha belirgin)
            playerCharacter.transform.localScale = Vector3.one * 1.2f;
        }
    }

    /// <summary>
    /// Kamerayı oyuncu karakterine odakla
    /// </summary>
    void FocusCameraOnPlayer()
    {
        if (playerCharacter == null || matchCamera == null) return;

        // MatchCamera'ya oyuncu referansını ver
        matchCamera.SetPlayerTarget(playerCharacter.transform);
        
        // Kamerayı oyuncuya odakla
        matchCamera.EnterPlayerFocusMode();
        
        Debug.Log($"[MatchChanceSceneManager] Kamera oyuncuya odaklandı: {playerCharacter.playerName}");
    }

    /// <summary>
    /// Maç sahnesine geri dön
    /// </summary>
    public void ReturnToMatchScene()
    {
        SceneManager.LoadScene("MatchScene");
    }

    /// <summary>
    /// Varsayılan top sprite'ı oluştur (runtime'da)
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
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    // Futbol topu deseni (siyah-beyaz)
                    bool isBlack = ((x + y) % 8 < 4);
                    Color color = isBlack ? Color.black : Color.white;
                    
                    float alpha = 1f;
                    if (distance > radius - 1f)
                    {
                        alpha = 1f - (distance - (radius - 1f));
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

    /// <summary>
    /// AI oyuncuları ayarla (takımlar spawn edildikten sonra)
    /// </summary>
    IEnumerator SetupAIPlayersDelayed()
    {
        // Takımların spawn edilmesini bekle
        yield return new WaitForSeconds(0.5f);
        
        // Top'un oluşturulmasını bekle
        while (currentBall == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Biraz daha bekle (top tamamen hazır olsun)
        yield return new WaitForSeconds(0.2f);
        
        SetupAIPlayers();
    }
    
    /// <summary>
    /// Tüm AI oyunculara AIPlayerController ekle
    /// </summary>
    void SetupAIPlayers()
    {
        // Ev sahibi takım AI oyuncuları (oyuncu karakteri hariç)
        if (homeTeamManager != null)
        {
            var homePlayers = homeTeamManager.GetAllPlayers();
            foreach (var player in homePlayers)
            {
                // Oyuncu karakteri değilse AI ekle
                if (player != playerCharacter)
                {
                    AddAIController(player);
                }
            }
        }
        
        // Deplasman takım AI oyuncuları (hepsi AI)
        if (awayTeamManager != null)
        {
            var awayPlayers = awayTeamManager.GetAllPlayers();
            foreach (var player in awayPlayers)
            {
                AddAIController(player);
            }
        }
        
        Debug.Log("[MatchChanceSceneManager] AI oyuncular ayarlandı.");
    }
    
    /// <summary>
    /// Oyuncuya AI controller ekle
    /// </summary>
    void AddAIController(Player player)
    {
        if (player == null) return;
        
        // Zaten AI controller var mı kontrol et
        AIPlayerController aiController = player.GetComponent<AIPlayerController>();
        if (aiController == null)
        {
            aiController = player.gameObject.AddComponent<AIPlayerController>();
        }
        
        // Top referansını ayarla
        if (currentBall != null)
        {
            aiController.SetBall(currentBall.transform);
        }
        
        Debug.Log($"[MatchChanceSceneManager] AI controller eklendi: {player.playerName}");
    }

    /// <summary>
    /// Topun pozisyonunun doğru olduğundan emin ol
    /// </summary>
    System.Collections.IEnumerator EnsureBallPosition()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        
        if (currentBall != null)
        {
            Vector3 pos = currentBall.transform.position;
            PitchManager pitch = PitchManager.Instance;
            
            // Saha sınırlarını kontrol et
            bool needsCorrection = false;
            
            if (pitch != null)
            {
                float halfLength = pitch.PitchLength / 2f; // 52.5
                float halfWidth = pitch.PitchWidth / 2f; // 34
                
                // Eğer pozisyon saha dışındaysa veya kale içindeyse düzelt
                if (Mathf.Abs(pos.y) >= halfLength - 1f || Mathf.Abs(pos.x) >= halfWidth - 1f)
                {
                    Debug.LogWarning($"[MatchChanceSceneManager] Top saha dışında veya kale içinde: {pos}. Sahanın ortasına getiriliyor...");
                    needsCorrection = true;
                }
            }
            else if (Mathf.Abs(pos.x) > 0.1f || Mathf.Abs(pos.y) > 0.1f || Mathf.Abs(pos.z) > 0.1f)
            {
                Debug.LogWarning($"[MatchChanceSceneManager] Top pozisyonu yanlış: {pos}. Düzeltiliyor...");
                needsCorrection = true;
            }
            
            if (needsCorrection)
            {
                // Parent'ı null yap
                currentBall.transform.SetParent(null);
                
                // Pozisyonu sıfırla
                currentBall.transform.position = Vector3.zero;
                currentBall.transform.localPosition = Vector3.zero;
                
                // Rigidbody2D pozisyonunu da sıfırla
                Rigidbody2D ballRb = currentBall.GetComponent<Rigidbody2D>();
                if (ballRb != null)
                {
                    ballRb.position = Vector2.zero;
                    ballRb.linearVelocity = Vector2.zero;
                    ballRb.angularVelocity = 0f;
                }
                
                Debug.Log($"[MatchChanceSceneManager] Top pozisyonu düzeltildi: {currentBall.transform.position}");
            }
        }
    }

    void OnDestroy()
    {
        // Cleanup
    }
}

