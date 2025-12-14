using UnityEngine;

/// <summary>
/// Maç kamera sistemi - topa odaklı, pozisyon geldiğinde yakınlaşma
/// </summary>
public class MatchCamera : MonoBehaviour
{
    [Header("Kamera Referansları")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform ball; // Top referansı
    [SerializeField] private Transform playerTarget; // Oyuncu referansı (bizim karakterimiz)

    [Header("Kamera Modları")]
    [SerializeField] private CameraMode currentMode = CameraMode.Overview;
    
    [Header("Overview Modu (Genel Görünüm - 2D)")]
    [SerializeField] private float overviewOrthographicSize = 60f; // 2D Orthographic size
    [SerializeField] private Vector3 overviewPosition = Vector3.zero; // Kameranın pozisyonu

    [Header("Chance Modu (Pozisyon Yakınlaşma - 2D)")]
    [SerializeField] private float chanceOrthographicSize = 20f; // Yakınlaşma için küçük size
    
    [Header("Player Focus Modu (Oyuncu Odaklama - 2D)")]
    [SerializeField] private float playerFocusOrthographicSize = 25f; // Oyuncuya odaklanma için orta size
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Smooth Follow")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isInChanceMode = false;
    private bool isInPlayerFocusMode = false;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            mainCamera = GetComponent<Camera>();
        
        // 2D için kamera ayarları
        if (mainCamera != null)
        {
            mainCamera.orthographic = true; // 2D için orthographic
            mainCamera.orthographicSize = overviewOrthographicSize;
        }
    }

    void Start()
    {
        SetCameraMode(CameraMode.Overview);
        
        // Mobil için kamera ayarlarını optimize et
        OptimizeForMobile();
    }
    
    void OptimizeForMobile()
    {
        if (mainCamera == null) return;
        
        // 2D Orthographic kamera için
        mainCamera.orthographic = true;
        
        // Mobil cihazlar için kamera ayarları
        // Portre modda (1080x1920) dikey görüş daha önemli
        if (Screen.width < Screen.height)
        {
            // Portre mod için daha büyük orthographic size
            overviewOrthographicSize = 70f;
        }
        
        mainCamera.orthographicSize = overviewOrthographicSize;
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        if (mainCamera == null) return;
        
        if (isInPlayerFocusMode && playerTarget != null)
        {
            // Oyuncu odaklama modu: Oyuncuya yakın (2D)
            Vector2 playerPos = (Vector2)playerTarget.position;
            
            // Kamerayı oyuncuya odakla
            targetPosition = new Vector3(playerPos.x, playerPos.y, transform.position.z);
            
            // Orthographic size'ı ayarla (oyuncuya odaklanma)
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, playerFocusOrthographicSize, zoomSpeed * Time.deltaTime);
        }
        else if (isInChanceMode && ball != null)
        {
            // Pozisyon modu: Topa yakın (2D)
            Vector2 ballPos = (Vector2)ball.position;
            
            // Kamerayı topa odakla
            targetPosition = new Vector3(ballPos.x, ballPos.y, transform.position.z);
            
            // Orthographic size'ı küçült (yakınlaşma)
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, chanceOrthographicSize, zoomSpeed * Time.deltaTime);
        }
        else
        {
            // Genel görünüm: Sahanın ortasına bak (2D)
            PitchManager pitch = PitchManager.Instance;
            if (pitch != null)
            {
                // Sahanın merkezine bak
                targetPosition = new Vector3(0, 0, transform.position.z);
            }
            else if (ball != null)
            {
                Vector2 ballPos = (Vector2)ball.position;
                targetPosition = new Vector3(ballPos.x, ballPos.y, transform.position.z);
            }
            else
            {
                // Hiçbir referans yoksa, kamerayı olduğu yerde tut
                targetPosition = transform.position;
            }
            
            // Orthographic size'ı büyüt (uzaklaşma)
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, overviewOrthographicSize, zoomSpeed * Time.deltaTime);
        }

        // Smooth geçiş (2D'de sadece pozisyon, rotasyon yok)
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    Vector2 GetDirectionToGoal(Vector2 fromPosition)
    {
        // Topun bulunduğu yarı sahaya göre hangi kaleye gideceğini belirle (2D)
        PitchManager pitch = PitchManager.Instance;
        if (pitch == null) return Vector2.up;

        // Eğer top sahanın alt yarısındaysa (y < 0), yukarıdaki kaleye (away goal)
        // Eğer top sahanın üst yarısındaysa (y > 0), aşağıdaki kaleye (home goal)
        if (fromPosition.y < 0)
        {
            return (pitch.AwayGoalPosition - fromPosition).normalized;
        }
        else
        {
            return (pitch.HomeGoalPosition - fromPosition).normalized;
        }
    }

    /// <summary>
    /// Pozisyon moduna geç (yakınlaşma)
    /// </summary>
    public void EnterChanceMode()
    {
        isInChanceMode = true;
        currentMode = CameraMode.Chance;
    }

    /// <summary>
    /// Genel görünüm moduna dön
    /// </summary>
    public void EnterOverviewMode()
    {
        isInChanceMode = false;
        isInPlayerFocusMode = false;
        currentMode = CameraMode.Overview;
    }

    /// <summary>
    /// Oyuncu odaklama moduna geç
    /// </summary>
    public void EnterPlayerFocusMode()
    {
        isInChanceMode = false;
        isInPlayerFocusMode = true;
        currentMode = CameraMode.PlayerFocus;
    }

    /// <summary>
    /// Top referansını ayarla
    /// </summary>
    public void SetBall(Transform ballTransform)
    {
        ball = ballTransform;
    }

    /// <summary>
    /// Oyuncu referansını ayarla
    /// </summary>
    public void SetPlayerTarget(Transform playerTransform)
    {
        playerTarget = playerTransform;
    }

    void SetCameraMode(CameraMode mode)
    {
        currentMode = mode;
        isInChanceMode = (mode == CameraMode.Chance);
        isInPlayerFocusMode = (mode == CameraMode.PlayerFocus);
    }
}

public enum CameraMode
{
    Overview,    // Genel görünüm
    Chance,      // Pozisyon yakınlaşma
    PlayerFocus  // Oyuncu odaklama
}

