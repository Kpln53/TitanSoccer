using UnityEngine;

/// <summary>
/// Match sahnesi kamera kontrolü - Top ve oyuncu takibi
/// </summary>
public class MatchCamera : MonoBehaviour
{
    [Header("Kamera Ayarları")]
    [SerializeField] private float followSpeed = 20f; // Takip hızı (oyuncu için çok hızlı)
    [SerializeField] private float playerFollowSpeed = 50f; // Oyuncu takibi için özel hız (çok agresif)
    [SerializeField] private float defaultOrthographicSize = 8f; // Varsayılan zoom
    [SerializeField] private float overviewOrthographicSize = 12f; // Genel görünüm zoom
    [SerializeField] private float playerFocusOrthographicSize = 6f; // Oyuncu odak zoom
    [SerializeField] private float zoomSpeed = 2f; // Zoom geçiş hızı

    [Header("Takip Ayarları")]
    [SerializeField] private bool followBall = true; // Topu takip et
    [SerializeField] private bool followPlayer = true; // Oyuncuyu takip et
    [SerializeField] private float followOffsetY = 0f; // Y ekseninde offset

    [Header("Sınırlar")]
    [SerializeField] private bool useFieldBounds = true; // Saha sınırları kullan
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -15f;
    [SerializeField] private float maxY = 15f;

    // Referanslar
    private Transform ballTarget;
    private Transform playerTarget;
    private FieldManager fieldManager;
    
    // Kamera
    private Camera cam;
    private float targetOrthographicSize;

    // Kamera modu
    private CameraMode currentMode = CameraMode.Overview;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            cam.orthographic = true;
            targetOrthographicSize = defaultOrthographicSize;
            cam.orthographicSize = targetOrthographicSize;
            
            // Kamera background rengini yeşil yap (saha rengi)
            cam.backgroundColor = new Color(0.2f, 0.7f, 0.3f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            
            // Kamera Z pozisyonunu ayarla (2D için)
            if (transform.position.z == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
            }
        }
    }

    private void Start()
    {
        // FieldManager'ı bul
        fieldManager = FindObjectOfType<FieldManager>();

        if (fieldManager != null)
        {
            // Event'lere abone ol
            fieldManager.OnBallSpawned += OnBallSpawned;
            fieldManager.OnPlayerCharacterSpawned += OnPlayerCharacterSpawned;

            // Saha sınırlarını ayarla
            if (useFieldBounds)
            {
                minX = -fieldManager.FieldWidth / 2f;
                maxX = fieldManager.FieldWidth / 2f;
                minY = -fieldManager.FieldLength / 2f;
                maxY = fieldManager.FieldLength / 2f;
            }
        }

        // Başlangıçta genel görünüm modunda başla
        // Ama oyuncu spawn edildiğinde otomatik PlayerFocus moduna geçecek
        EnterOverviewMode();
        
        // Kameranın Z pozisyonunu garanti et
        if (transform.position.z == 0 || transform.position.z > -1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
    }

    private void Update()
    {
        UpdateCameraPosition();
        UpdateCameraZoom();
    }

    /// <summary>
    /// Kamera pozisyonunu güncelle
    /// </summary>
    void UpdateCameraPosition()
    {
        Vector3 targetPosition = transform.position;

        if (currentMode == CameraMode.PlayerFocus && playerTarget != null && followPlayer)
        {
            // Oyuncu odak modu - tam oyuncunun üstünde (direkt takip)
            targetPosition = new Vector3(
                playerTarget.position.x,
                playerTarget.position.y + followOffsetY,
                transform.position.z
            );
            
            // Çok agresif takip - neredeyse direkt atama
            float distance = Vector3.Distance(transform.position, targetPosition);
            
            // Çok hızlı lerp (agresif takip)
            float lerpSpeed = playerFollowSpeed * Time.unscaledDeltaTime;
            
            // Eğer lerp değeri 1'den büyükse direkt atama yap
            if (lerpSpeed >= 1f || distance < 0.05f)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed);
            }
            
            return; // Early return - sınır kontrolü yok
        }
        else if (currentMode == CameraMode.BallFollow && ballTarget != null && followBall)
        {
            // Top takip modu
            targetPosition = new Vector3(
                ballTarget.position.x,
                ballTarget.position.y + followOffsetY,
                transform.position.z
            );
        }
        else if (currentMode == CameraMode.Overview)
        {
            // Genel görünüm (merkez)
            targetPosition = new Vector3(0f, 0f, transform.position.z);
        }

        // Saha sınırları içinde tut (sadece oyuncu dışı modlar için)
        if (useFieldBounds && currentMode != CameraMode.PlayerFocus)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // Smooth takip
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Kamera zoom'unu güncelle
    /// </summary>
    void UpdateCameraZoom()
    {
        if (cam == null) return;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Genel görünüm moduna geç (tüm sahneyi göster)
    /// </summary>
    public void EnterOverviewMode()
    {
        currentMode = CameraMode.Overview;
        targetOrthographicSize = overviewOrthographicSize;
        Debug.Log("[MatchCamera] Genel görünüm modu");
    }

    /// <summary>
    /// Oyuncu odak moduna geç
    /// </summary>
    public void EnterPlayerFocusMode()
    {
        currentMode = CameraMode.PlayerFocus;
        targetOrthographicSize = playerFocusOrthographicSize;
        
        // Hemen oyuncunun pozisyonuna geç (ani geçiş - smooth değil)
        if (playerTarget != null)
        {
            Vector3 immediatePos = new Vector3(
                playerTarget.position.x,
                playerTarget.position.y + followOffsetY,
                transform.position.z
            );
            transform.position = immediatePos;
            
            Debug.Log($"[MatchCamera] Oyuncu odak modu - Kamera oyuncuya sabitlendi. Oyuncu: {playerTarget.position}, Kamera: {immediatePos}");
        }
    }

    /// <summary>
    /// Top takip moduna geç
    /// </summary>
    public void EnterBallFollowMode()
    {
        currentMode = CameraMode.BallFollow;
        targetOrthographicSize = defaultOrthographicSize;
        Debug.Log("[MatchCamera] Top takip modu");
    }

    /// <summary>
    /// Top referansını ayarla
    /// </summary>
    public void SetBall(Transform ballTransform)
    {
        ballTarget = ballTransform;
        
        // Eğer top varsa otomatik takip moduna geç
        if (ballTarget != null && currentMode == CameraMode.Overview)
        {
            EnterBallFollowMode();
        }
    }

    /// <summary>
    /// Oyuncu referansını ayarla
    /// </summary>
    public void SetPlayerTarget(Transform playerTransform)
    {
        playerTarget = playerTransform;
        
        // Eğer oyuncu varsa odak moduna geç
        if (playerTarget != null)
        {
            EnterPlayerFocusMode();
        }
    }

    /// <summary>
    /// Top spawn edildiğinde çağrılır
    /// </summary>
    void OnBallSpawned(BallController ball)
    {
        if (ball != null)
        {
            SetBall(ball.transform);
        }
    }

    /// <summary>
    /// Oyuncu karakteri spawn edildiğinde çağrılır
    /// </summary>
    void OnPlayerCharacterSpawned(PlayerController player)
    {
        if (player != null)
        {
            playerTarget = player.transform;
            
            // Oyuncuya otomatik odaklan (bu zaten immediate positioning yapıyor)
            EnterPlayerFocusMode();
            
            Debug.Log($"[MatchCamera] Oyuncu karakterine odaklanıldı: {player.name}");
        }
    }

    /// <summary>
    /// Kamera sınırlarını ayarla
    /// </summary>
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        this.useFieldBounds = true;
    }

    private void OnDestroy()
    {
        // Event'lerden abonelikten çık
        if (fieldManager != null)
        {
            fieldManager.OnBallSpawned -= OnBallSpawned;
            fieldManager.OnPlayerCharacterSpawned -= OnPlayerCharacterSpawned;
        }
    }

    // Getters
    public CameraMode CurrentMode => currentMode;
    public float CurrentOrthographicSize => cam != null ? cam.orthographicSize : defaultOrthographicSize;
}

/// <summary>
/// Kamera modları
/// </summary>
public enum CameraMode
{
    Overview,      // Genel görünüm (tüm sahne)
    PlayerFocus,   // Oyuncu odak
    BallFollow     // Top takip
}
