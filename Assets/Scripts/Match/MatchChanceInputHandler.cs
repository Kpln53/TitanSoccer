using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// MatchChance sahnesi için input handler
/// Tüm touch/mouse input'larını toplayıp ilgili sistemlere dağıtır
/// </summary>
public class MatchChanceInputHandler : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ShotAimSystem shotAimSystem;
    [SerializeField] private PassSystem passSystem;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Double Tap Ayarları")]
    [SerializeField] private float doubleTapTime = 0.3f; // Çift tıklama için maksimum süre
    [SerializeField] private float doubleTapMaxDistance = 2f; // Çift tıklama için maksimum mesafe

    // Input state
    private Vector2 lastTapPosition;
    private float lastTapTime = -1f;
    private bool isDragging = false;
    private bool isAiming = false;
    private Vector2 dragStartScreenPos;

    // Raycast için
    private int playerLayerMask;
    private int groundLayerMask;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Layer mask'leri hazırla
        playerLayerMask = playerLayer.value;
        if (playerLayerMask == 0)
        {
            // Default: Player layer'ı bul
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1)
            {
                playerLayerMask = 1 << playerLayerIndex;
            }
        }

        groundLayerMask = groundLayer.value;
    }

    private void Start()
    {
        // FieldManager'dan oyuncu karakterini al (doğru oyuncu için)
        FieldManager fieldManager = FindObjectOfType<FieldManager>();
        if (fieldManager != null && fieldManager.PlayerCharacter != null)
        {
            playerController = fieldManager.PlayerCharacter;
            Debug.Log($"[InputHandler] Oyuncu karakteri FieldManager'dan alındı: {playerController.name}");
        }
        
        // Bulunamazsa FindObjectOfType ile bul (fallback)
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            Debug.LogWarning("[InputHandler] Oyuncu karakteri FindObjectOfType ile bulundu (fallback)");
        }

        if (shotAimSystem == null)
            shotAimSystem = FindObjectOfType<ShotAimSystem>();

        if (passSystem == null)
            passSystem = FindObjectOfType<PassSystem>();

        // Event'lere abone ol
        if (shotAimSystem != null)
        {
            shotAimSystem.OnShotExecuted += OnShotExecuted;
        }

        if (passSystem != null && playerController != null)
        {
            BallController ballController = null;
            if (fieldManager != null)
            {
                ballController = fieldManager.Ball;
            }
            if (ballController == null)
            {
                ballController = FindObjectOfType<BallController>();
            }
            passSystem.SetReferences(playerController, ballController);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Input işleme
    /// </summary>
    void HandleInput()
    {
        Vector2 inputPos = Vector2.zero;
        bool inputDown = false;
        bool inputHeld = false;
        bool inputUp = false;

        // Mouse input (PC)
        if (Mouse.current != null)
        {
            inputPos = Mouse.current.position.ReadValue();
            inputDown = Mouse.current.leftButton.wasPressedThisFrame;
            inputHeld = Mouse.current.leftButton.isPressed;
            inputUp = Mouse.current.leftButton.wasReleasedThisFrame;
        }

        // Touch input (Mobil)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            inputPos = Touchscreen.current.primaryTouch.position.ReadValue();
            inputDown = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            inputHeld = Touchscreen.current.primaryTouch.press.isPressed;
            inputUp = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        }

        if (mainCamera == null) return;

        // Orthographic kamera için ScreenToWorldPoint dönüşümü
        // Z değeri kameradan uzaklık (orthographic için önemli)
        float zDistance = mainCamera.transform.position.z - 0f; // World Z = 0 (oyuncular)
        Vector3 screenPos3D = new Vector3(inputPos.x, inputPos.y, Mathf.Abs(zDistance));
        Vector3 worldPos3D = mainCamera.ScreenToWorldPoint(screenPos3D);
        Vector2 worldPos = new Vector2(worldPos3D.x, worldPos3D.y);
        
        // Debug için
        if (inputDown)
        {
            Debug.Log($"[InputHandler] Screen({inputPos:F0}) -> World({worldPos:F2}), Camera pos: {mainCamera.transform.position}, Z dist: {zDistance:F2}");
        }

        // Input down (basıldı)
        if (inputDown)
        {
            HandleInputDown(worldPos, inputPos);
        }

        // Input held (sürükleniyor)
        if (inputHeld && isDragging)
        {
            HandleInputDrag(inputPos);
        }

        // Input up (bırakıldı)
        if (inputUp)
        {
            HandleInputUp(worldPos, inputPos);
        }
    }

    /// <summary>
    /// Input basıldığında
    /// </summary>
    void HandleInputDown(Vector2 worldPos, Vector2 screenPos)
    {
        // Oyuncuya mı tıkladı?
        if (IsPointOnPlayer(worldPos) && playerController != null && playerController.HasBall)
        {
            // Şut modu başlat
            StartAiming(screenPos);
            return;
        }

        // Takım arkadaşına mı tıkladı?
        PlayerController teammate = FindPlayerAtPosition(worldPos);
        if (teammate != null && teammate != playerController && playerController != null && playerController.HasBall)
        {
            // Çift tık kontrolü
            if (IsDoubleTap(worldPos))
            {
                // Havadan pas (ileride eklenecek)
                Debug.Log("[InputHandler] Havadan pas - Henüz implement edilmedi");
                // TODO: PassSystem'e havadan pas ekle
                return;
            }

            // Tek tık - Yerden pas
            if (passSystem != null)
            {
                passSystem.PassToTeammate(teammate);
                lastTapTime = Time.unscaledTime;
                lastTapPosition = worldPos;
            }
            return;
        }

        // Boş yere tıkladı - Hareket (şut modunda değilse)
        if (playerController != null && !isAiming && !isDragging)
        {
            // World pozisyonunu debug için logla
            Vector2 playerPos = playerController.Position;
            float distance = Vector2.Distance(playerPos, worldPos);
            Debug.Log($"[InputHandler] Hareket: Oyuncu({playerPos:F2}) -> Hedef({worldPos:F2}), Mesafe: {distance:F2}");
            playerController.MoveTo(worldPos);
        }
        else if (isAiming || isDragging)
        {
            // Şut modunda veya çizgi çizilirken hareket yok
            return;
        }

        // Tap zamanını kaydet (double tap için)
        lastTapTime = Time.unscaledTime;
        lastTapPosition = worldPos;
    }

    /// <summary>
    /// Input sürükleniyor (şut çizme)
    /// </summary>
    void HandleInputDrag(Vector2 screenPos)
    {
        if (isAiming && shotAimSystem != null)
        {
            shotAimSystem.UpdateAiming(screenPos);
        }
    }

    /// <summary>
    /// Input bırakıldığında
    /// </summary>
    void HandleInputUp(Vector2 worldPos, Vector2 screenPos)
    {
        if (isAiming)
        {
            // Şut at
            ExecuteShot();
        }

        isDragging = false;
        isAiming = false;
    }

    /// <summary>
    /// Şut modunu başlat
    /// </summary>
    void StartAiming(Vector2 screenPos)
    {
        if (playerController == null || !playerController.HasBall || shotAimSystem == null)
            return;

        isDragging = true;
        isAiming = true;
        dragStartScreenPos = screenPos;

        shotAimSystem.StartAiming(playerController);

        Debug.Log("[InputHandler] Şut modu başladı!");
    }

    /// <summary>
    /// Şut at
    /// </summary>
    void ExecuteShot()
    {
        if (shotAimSystem == null) return;

        shotAimSystem.ExecuteShot();
    }

    /// <summary>
    /// Oyuncunun üzerinde mi kontrolü
    /// </summary>
    bool IsPointOnPlayer(Vector2 worldPos)
    {
        if (playerController == null) return false;

        Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.5f, playerLayerMask);
        if (hit != null)
        {
            PlayerController hitPlayer = hit.GetComponent<PlayerController>();
            return hitPlayer == playerController;
        }

        return false;
    }

    /// <summary>
    /// Pozisyondaki oyuncuyu bul
    /// </summary>
    PlayerController FindPlayerAtPosition(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 1f, playerLayerMask);
        if (hit != null)
        {
            return hit.GetComponent<PlayerController>();
        }

        return null;
    }

    /// <summary>
    /// Çift tık kontrolü
    /// </summary>
    bool IsDoubleTap(Vector2 currentPos)
    {
        if (lastTapTime < 0f)
            return false;

        float timeSinceLastTap = Time.unscaledTime - lastTapTime;
        float distanceFromLastTap = Vector2.Distance(currentPos, lastTapPosition);

        return timeSinceLastTap < doubleTapTime && distanceFromLastTap < doubleTapMaxDistance;
    }

    /// <summary>
    /// Şut atıldığında çağrılır
    /// </summary>
    void OnShotExecuted(Vector2 direction, float power)
    {
        // MatchChanceSceneManager'e bildir
        if (MatchChanceSceneManager.Instance != null && playerController != null)
        {
            MatchChanceSceneManager.Instance.OnShotTaken();
            
            // Şut sonucunu hesapla (top kaleye gidene kadar bekle, sonra sonuç belirlenecek)
            // Şimdilik direkt hesaplama yapıyoruz
            MatchChanceSceneManager.Instance.CalculateShotResult(direction, power, false, playerController);
        }

        Debug.Log($"[InputHandler] Şut atıldı! Yön: {direction}, Güç: {power:F1}");
    }

    private void OnDestroy()
    {
        // Event'lerden abonelikten çık
        if (shotAimSystem != null)
        {
            shotAimSystem.OnShotExecuted -= OnShotExecuted;
        }
    }
}
