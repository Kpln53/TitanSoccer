using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Score Hero tarzı top kontrol sistemi - Şut ve Pas
/// Çizgi çizme: Oyuncudan → İstediğin yöne çiz → Bırak
/// </summary>
public class BallControlSystem : MonoBehaviour
{
    public static BallControlSystem Instance { get; private set; }

    [Header("Referanslar")]
    [SerializeField] private Transform controlledPlayer;
    [SerializeField] private Transform ball;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer aimLine;

    [Header("Şut Ayarları")]
    [SerializeField] private float minShotPower = 8f;
    [SerializeField] private float maxShotPower = 30f;
    [SerializeField] private float maxLineLength = 8f; // Maksimum çizgi uzunluğu
    [SerializeField] private float shotPowerMultiplier = 10f;

    [Header("Pas Ayarları")]
    [SerializeField] private float passSpeed = 12f;

    [Header("Görsel Ayarlar")]
    [SerializeField] private Color aimLineColor = new Color(1f, 1f, 1f, 0.9f);
    [SerializeField] private Color powerLowColor = Color.white;
    [SerializeField] private Color powerHighColor = Color.yellow;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private LineRenderer trajectoryLine;

    [Header("Zaman Ayarları")]
    [SerializeField] private float slowMotionScale = 0.2f;

    // State
    private bool isDrawing = false;
    private Vector2 drawStartPos; // Çizgi başlangıcı (oyuncu pozisyonu)
    private Vector2 currentDrawPos; // Şu anki çizim pozisyonu
    private float currentLineLength = 0f;
    private bool hasBall = false;
    private bool isInChanceMode = false;

    // Double tap detection (pas için)
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;
    private Vector2 lastTapPosition;

    // Ball Rigidbody
    private Rigidbody2D ballRb;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        SetupAimLine();
        FindBall();
    }

    void Update()
    {
        if (!isInChanceMode || controlledPlayer == null) return;

        HandleInput();
        UpdateAimLine();
    }

    /// <summary>
    /// Aim çizgisi ve yörünge çizgisi için LineRenderer ayarla
    /// </summary>
    void SetupAimLine()
    {
        // Ana çizgi (şut yönü)
        if (aimLine == null)
        {
            GameObject lineObj = new GameObject("AimLine");
            lineObj.transform.SetParent(transform);
            aimLine = lineObj.AddComponent<LineRenderer>();
        }

        aimLine.positionCount = 2;
        aimLine.startWidth = lineWidth;
        aimLine.endWidth = lineWidth * 0.5f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = aimLineColor;
        aimLine.endColor = aimLineColor;
        aimLine.sortingOrder = 100;
        aimLine.enabled = false;

        // Yörünge çizgisi (noktalı)
        if (trajectoryLine == null)
        {
            GameObject trajObj = new GameObject("TrajectoryLine");
            trajObj.transform.SetParent(transform);
            trajectoryLine = trajObj.AddComponent<LineRenderer>();
        }

        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.startWidth = lineWidth * 0.4f;
        trajectoryLine.endWidth = lineWidth * 0.1f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.startColor = new Color(1f, 1f, 0.5f, 0.6f);
        trajectoryLine.endColor = new Color(1f, 1f, 0.5f, 0.1f);
        trajectoryLine.sortingOrder = 99;
        trajectoryLine.enabled = false;
    }

    /// <summary>
    /// Topu bul
    /// </summary>
    void FindBall()
    {
        if (ball == null)
        {
            GameObject ballObj = GameObject.FindGameObjectWithTag("Ball");
            if (ballObj != null)
            {
                ball = ballObj.transform;
                ballRb = ball.GetComponent<Rigidbody2D>();
            }
        }
        else
        {
            ballRb = ball.GetComponent<Rigidbody2D>();
        }
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

        // Mouse input
        if (Mouse.current != null)
        {
            inputPos = Mouse.current.position.ReadValue();
            inputDown = Mouse.current.leftButton.wasPressedThisFrame;
            inputHeld = Mouse.current.leftButton.isPressed;
            inputUp = Mouse.current.leftButton.wasReleasedThisFrame;
        }

        // Touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            inputPos = Touchscreen.current.primaryTouch.position.ReadValue();
            inputDown = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            inputHeld = Touchscreen.current.primaryTouch.press.isPressed;
            inputUp = Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        }

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(inputPos);

        // Çizim başladı
        if (inputDown && hasBall)
        {
            // Double tap kontrolü (pas için)
            float timeSinceLastTap = Time.unscaledTime - lastTapTime;
            float distanceFromLastTap = Vector2.Distance(worldPos, lastTapPosition);

            if (timeSinceLastTap < doubleTapThreshold && distanceFromLastTap < 2f)
            {
                // Double tap - Havadan pas
                HandleDoubleTap(worldPos);
                return;
            }

            // Çizim başlat (oyuncu pozisyonundan)
            StartDrawing(worldPos);

            lastTapTime = Time.unscaledTime;
            lastTapPosition = worldPos;
        }

        // Çizim devam ediyor
        if (inputHeld && isDrawing)
        {
            UpdateDrawing(worldPos);
        }

        // Çizim bırakıldı
        if (inputUp && isDrawing)
        {
            ReleaseDrawing();
        }

        // Çizim yoksa ve tek tık varsa - Pas veya hareket kontrolü
        if (inputDown && !isDrawing && hasBall)
        {
            HandleSingleTap(worldPos);
        }
    }

    /// <summary>
    /// Çizim başlat (Score Hero tarzı)
    /// </summary>
    void StartDrawing(Vector2 screenPos)
    {
        if (controlledPlayer == null) return;

        Vector2 playerPos = controlledPlayer.position;
        
        // Oyuncuya çok yakın bir yere tıkladıysa çizim başlatma (hareket için)
        float distanceToPlayer = Vector2.Distance(screenPos, playerPos);
        if (distanceToPlayer < 1f)
        {
            return; // Hareket için bırak
        }

        isDrawing = true;
        drawStartPos = playerPos; // Çizgi oyuncudan başlar
        currentDrawPos = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, mainCamera.nearClipPlane)
        );

        // Slow motion başlat
        if (MatchChanceSceneManager.Instance != null)
        {
            // MatchChanceSceneManager zaten slow motion yönetiyor, burada ekstra bir şey yapmaya gerek yok
        }

        aimLine.enabled = true;
        trajectoryLine.enabled = true;

        Debug.Log("[BallControl] Çizim başladı - Score Hero tarzı!");
    }

    /// <summary>
    /// Çizim güncelle
    /// </summary>
    void UpdateDrawing(Vector2 worldPos)
    {
        if (controlledPlayer == null) return;

        currentDrawPos = worldPos;
        
        // Çizgi uzunluğunu hesapla
        Vector2 direction = (currentDrawPos - drawStartPos);
        currentLineLength = Mathf.Min(direction.magnitude, maxLineLength);
        
        // Güç hesapla
        float powerRatio = currentLineLength / maxLineLength;
    }

    /// <summary>
    /// Aim çizgisini güncelle
    /// </summary>
    void UpdateAimLine()
    {
        if (!isDrawing || aimLine == null || controlledPlayer == null) return;

        Vector2 playerPos = controlledPlayer.position;
        Vector2 direction = (currentDrawPos - drawStartPos).normalized;
        float lineLength = Mathf.Min((currentDrawPos - drawStartPos).magnitude, maxLineLength);

        // Çizgi: Oyuncudan → Çizilen yöne
        Vector3 startPos = new Vector3(playerPos.x, playerPos.y, 0);
        Vector3 endPos = startPos + new Vector3(direction.x, direction.y, 0) * lineLength;

        aimLine.SetPosition(0, startPos);
        aimLine.SetPosition(1, endPos);

        // Güç oranına göre renk ve kalınlık
        float powerRatio = lineLength / maxLineLength;
        Color lineColor = Color.Lerp(powerLowColor, powerHighColor, powerRatio);
        aimLine.startColor = lineColor;
        aimLine.endColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0.5f);
        aimLine.startWidth = lineWidth + (powerRatio * lineWidth * 0.5f);
        aimLine.endWidth = lineWidth * 0.3f;

        // Yörünge çizgisini güncelle
        float shotPower = (powerRatio * maxShotPower * shotPowerMultiplier);
        UpdateTrajectoryLine(startPos, direction, shotPower);
    }

    /// <summary>
    /// Yörünge çizgisini güncelle
    /// </summary>
    void UpdateTrajectoryLine(Vector3 startPos, Vector2 direction, float power)
    {
        if (trajectoryLine == null) return;

        trajectoryLine.enabled = true;

        // Simüle edilmiş yörünge (2D, sürtünme ile)
        float friction = 0.98f; // Ball damping
        float timeStep = 0.1f;
        Vector2 velocity = direction * power;
        Vector2 position = startPos;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            trajectoryLine.SetPosition(i, new Vector3(position.x, position.y, 0));

            // Fizik simülasyonu
            velocity *= friction;
            position += velocity * timeStep;

            // Hız çok düştüyse dur
            if (velocity.magnitude < 0.5f)
            {
                // Kalan noktaları son pozisyonda tut
                for (int j = i + 1; j < trajectoryPoints; j++)
                {
                    trajectoryLine.SetPosition(j, new Vector3(position.x, position.y, 0));
                }
                break;
            }
        }

        // Renk güncelle
        float powerRatio = power / (maxShotPower * shotPowerMultiplier);
        Color trajColor = Color.Lerp(new Color(1f, 1f, 0.5f, 0.4f), new Color(1f, 0.8f, 0f, 0.6f), powerRatio);
        trajectoryLine.startColor = trajColor;
        trajColor.a *= 0.3f;
        trajectoryLine.endColor = trajColor;
    }

    /// <summary>
    /// Çizim bırak - Şut at
    /// </summary>
    void ReleaseDrawing()
    {
        if (!isDrawing) return;

        isDrawing = false;
        aimLine.enabled = false;
        trajectoryLine.enabled = false;

        // Şut yönü ve gücü
        Vector2 direction = (currentDrawPos - drawStartPos).normalized;
        float lineLength = Mathf.Min((currentDrawPos - drawStartPos).magnitude, maxLineLength);
        float powerRatio = lineLength / maxLineLength;
        float shotPower = (powerRatio * maxShotPower * shotPowerMultiplier);

        // Minimum güç kontrolü
        if (shotPower < minShotPower)
        {
            Debug.Log("[BallControl] Şut çok zayıf, iptal edildi.");
            return;
        }

        // Topu at
        ShootBall(direction, shotPower);

        // Top kontrolünü kaybet
        hasBall = false;

        // MatchChanceSceneManager'e bildir
        if (MatchChanceSceneManager.Instance != null)
        {
            MatchChanceSceneManager.Instance.OnShotTaken();
        }

        Debug.Log($"[BallControl] ŞUT! Yön: {direction}, Güç: {shotPower:F1}");

        // Şut sonucunu hesapla
        CalculateShotResult(direction, shotPower);
    }

    /// <summary>
    /// Topu belirtilen yöne ve güçle at
    /// </summary>
    void ShootBall(Vector2 direction, float power)
    {
        if (ball == null || ballRb == null)
        {
            FindBall();
            if (ball == null || ballRb == null) return;
        }

        // Topu oyuncunun önüne yerleştir
        Vector2 playerPos = controlledPlayer.position;
        ball.position = new Vector3(playerPos.x, playerPos.y, 0) + new Vector3(direction.x, direction.y, 0) * 0.5f;

        // Hız uygula
        ballRb.linearVelocity = direction * power;

        // Event tetikle
        OnBallKicked?.Invoke(direction, power, ShotType.Shot);
    }

    /// <summary>
    /// Şut sonucunu hesapla
    /// </summary>
    void CalculateShotResult(Vector2 direction, float power)
    {
        if (MatchChanceSceneManager.Instance == null) return;

        // MatchChanceSceneManager'ın hesaplama metodunu kullan
        PlayerController playerController = null;
        if (controlledPlayer != null)
        {
            playerController = controlledPlayer.GetComponent<PlayerController>();
        }

        MatchChanceSceneManager.Instance.CalculateShotResult(direction, power, false, playerController);
    }

    /// <summary>
    /// Tek tıklama - Yerden pas veya hareket
    /// </summary>
    void HandleSingleTap(Vector2 worldPos)
    {
        // Takım arkadaşına mı tıkladı?
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 1f);
        if (hit != null)
        {
            PlayerController targetPlayer = hit.GetComponent<PlayerController>();
            if (targetPlayer != null && targetPlayer != controlledPlayer.GetComponent<PlayerController>())
            {
                // Yerden pas
                PassBall(hit.transform.position, false);
                Debug.Log("[BallControl] Yerden pas!");
                return;
            }
        }

        // Hareket - PlayerController tarafından işlenecek
    }

    /// <summary>
    /// Çift tıklama - Havadan pas
    /// </summary>
    void HandleDoubleTap(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 1f);
        if (hit != null)
        {
            PlayerController targetPlayer = hit.GetComponent<PlayerController>();
            if (targetPlayer != null && targetPlayer != controlledPlayer.GetComponent<PlayerController>())
            {
                // Havadan pas
                PassBall(hit.transform.position, true);
                Debug.Log("[BallControl] Havadan pas!");
            }
        }
    }

    /// <summary>
    /// Pas at
    /// </summary>
    void PassBall(Vector2 targetPos, bool isLobPass)
    {
        if (ball == null || ballRb == null || !hasBall || controlledPlayer == null) return;

        Vector2 playerPos = controlledPlayer.position;
        Vector2 direction = (targetPos - playerPos).normalized;

        // Topu oyuncunun önüne yerleştir
        ball.position = new Vector3(playerPos.x, playerPos.y, 0) + new Vector3(direction.x, direction.y, 0) * 0.5f;

        float speed = passSpeed;
        if (isLobPass)
        {
            speed *= 0.8f;
            StartCoroutine(LobPassEffect());
        }

        ballRb.linearVelocity = direction * speed;

        // Top kontrolünü kaybet
        hasBall = false;

        OnBallKicked?.Invoke(direction, speed, isLobPass ? ShotType.LobPass : ShotType.GroundPass);
    }

    /// <summary>
    /// Havadan pas görsel efekti
    /// </summary>
    System.Collections.IEnumerator LobPassEffect()
    {
        if (ball == null) yield break;

        Vector3 originalScale = ball.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float heightMultiplier = 1f + Mathf.Sin(t * Mathf.PI) * 0.5f;
            ball.localScale = originalScale * heightMultiplier;
            yield return null;
        }

        ball.localScale = originalScale;
    }

    /// <summary>
    /// Chance moduna gir
    /// </summary>
    public void EnterChanceMode(Transform player, Transform ballTransform)
    {
        controlledPlayer = player;
        ball = ballTransform;
        ballRb = ball?.GetComponent<Rigidbody2D>();
        isInChanceMode = true;
        hasBall = true;

        Debug.Log("[BallControl] Chance modu aktif - Score Hero tarzı!");
    }

    /// <summary>
    /// Chance modundan çık
    /// </summary>
    public void ExitChanceMode()
    {
        isInChanceMode = false;
        hasBall = false;
        isDrawing = false;

        if (aimLine != null)
            aimLine.enabled = false;
        
        if (trajectoryLine != null)
            trajectoryLine.enabled = false;

        Debug.Log("[BallControl] Chance modu bitti.");
    }

    /// <summary>
    /// Oyuncu topa sahip mi?
    /// </summary>
    public void SetHasBall(bool value)
    {
        hasBall = value;
    }

    // Events
    public System.Action<Vector2, float, ShotType> OnBallKicked;

    // Getters
    public bool IsDrawing => isDrawing;
    public bool HasBall => hasBall;
    public float CurrentLineLength => currentLineLength;
    public bool IsInChanceMode => isInChanceMode;
}

/// <summary>
/// Şut/Pas tipi
/// </summary>
public enum ShotType
{
    Shot,       // Şut
    GroundPass, // Yerden pas
    LobPass     // Havadan pas
}
