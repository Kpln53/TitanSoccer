using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyuncu kontrol sistemi - ekrana basılan yere gitme
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Player controlledPlayer; // Kontrol edilen oyuncu
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer; // Zemin layer'ı

    [Header("Ayarlar")]
    [SerializeField] private float maxMoveDistance = 50f; // Maksimum hareket mesafesi
    [SerializeField] private float raycastDistance = 1000f; // Raycast mesafesi (çok uzun)
    [SerializeField] private bool showMoveIndicator = true; // Hareket göstergesi
    [SerializeField] private GameObject moveIndicatorPrefab; // Hareket göstergesi prefab

    [Header("Pause Sistemi")]
    [SerializeField] private PauseOverlay pauseOverlay;

    private GameObject currentMoveIndicator;
    private bool isControlled = false;
    private bool canReceiveInput = true; // Input alabilir mi?

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (controlledPlayer == null)
            controlledPlayer = GetComponent<Player>();
    }
    
    void Start()
    {
        // Ground layer'ı ayarla - tüm layer'ları kabul et (Everything)
        if (groundLayer.value == 0)
        {
            groundLayer = ~0; // Tüm layer'lar (Everything)
        }
        
        // PauseOverlay'i bul veya oluştur
        if (pauseOverlay == null)
        {
            pauseOverlay = FindObjectOfType<PauseOverlay>();
            if (pauseOverlay == null)
            {
                GameObject pauseObj = new GameObject("PauseOverlay");
                pauseOverlay = pauseObj.AddComponent<PauseOverlay>();
            }
        }
        
        // Başlangıçta oyunu duraklat (oyuncu hareket etmiyor)
        if (pauseOverlay != null)
        {
            pauseOverlay.PauseGame();
        }
    }

    void Update()
    {
        if (!isControlled || controlledPlayer == null) return;

        // Mouse/Touch input kontrolü
        HandleInput();
    }

    void HandleInput()
    {
        // Input alınamıyorsa çık
        if (!canReceiveInput) return;

        // Mouse input (PC)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            MoveToScreenPosition(screenPos);
        }

        // Touch input (Mobil)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            MoveToScreenPosition(screenPos);
        }
    }

    /// <summary>
    /// Ekran pozisyonundan dünya pozisyonuna çevir ve oyuncuyu hareket ettir (2D)
    /// </summary>
    void MoveToScreenPosition(Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera bulunamadı!");
            return;
        }

        // 2D için Camera.ScreenToWorldPoint kullan (Z = 0, 2D düzlem)
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
        Vector2 targetPosition = new Vector2(worldPos.x, worldPos.y);

        // Saha sınırları içinde mi kontrol et
        PitchManager pitch = PitchManager.Instance;
        if (pitch != null)
        {
            targetPosition = pitch.ClampToPitch2D(targetPosition);
        }

        // Oyuncuyu hareket ettir
        if (controlledPlayer != null)
        {
            // Önceki event'i temizle (eğer varsa)
            controlledPlayer.OnMovementComplete = null;
            
            // Input'u devre dışı bırak (hareket tamamlanana kadar)
            canReceiveInput = false;
            
            // Oyunu devam ettir (oyuncu hareket ediyor)
            if (pauseOverlay != null)
            {
                pauseOverlay.ResumeGame();
            }
            
            // Hareket tamamlandığında input'u tekrar aktif et
            controlledPlayer.OnMovementComplete += OnPlayerReachedTarget;
            
            controlledPlayer.MoveTo(targetPosition);
            
            // Hareket göstergesi göster
            if (showMoveIndicator)
            {
                ShowMoveIndicator2D(targetPosition);
            }
            
            Debug.Log($"Oyuncu hareket ediyor... Screen: {screenPosition}, World: {targetPosition}");
        }
    }

    /// <summary>
    /// Hareket göstergesi göster (hedef pozisyon) - 2D
    /// </summary>
    void ShowMoveIndicator2D(Vector2 position)
    {
        // Eski göstergiyi kaldır
        if (currentMoveIndicator != null)
        {
            Destroy(currentMoveIndicator);
        }

        // Yeni gösterge oluştur
        if (moveIndicatorPrefab != null)
        {
            currentMoveIndicator = Instantiate(moveIndicatorPrefab, position, Quaternion.identity);
        }
        else
        {
            // Basit bir gösterge oluştur (2D daire sprite)
            currentMoveIndicator = new GameObject("MoveIndicator");
            currentMoveIndicator.transform.position = position;
            
            // SpriteRenderer ekle
            SpriteRenderer sr = currentMoveIndicator.AddComponent<SpriteRenderer>();
            
            // Runtime'da hareket göstergesi sprite'ı oluştur
            sr.sprite = CreateDefaultIndicatorSprite();
            sr.sortingOrder = 10; // Üstte görünsün
            currentMoveIndicator.transform.localScale = Vector3.one * 0.5f;
        }

        // 2 saniye sonra göstergeyi kaldır
        Destroy(currentMoveIndicator, 2f);
    }

    /// <summary>
    /// Bu oyuncuyu kontrol et
    /// </summary>
    public void SetControlled(bool controlled)
    {
        isControlled = controlled;
        
        if (controlled && controlledPlayer != null)
        {
            Debug.Log($"Oyuncu kontrolü aktif: {controlledPlayer.playerName}");
        }
    }

    /// <summary>
    /// Kontrol edilen oyuncuyu ayarla
    /// </summary>
    public void SetControlledPlayer(Player player)
    {
        controlledPlayer = player;
        isControlled = (player != null);
    }

    /// <summary>
    /// Oyuncu hedefe vardığında çağrılır
    /// </summary>
    void OnPlayerReachedTarget()
    {
        // Event'i önce temizle (tekrar set etmeden önce)
        if (controlledPlayer != null)
        {
            controlledPlayer.OnMovementComplete = null;
        }
        
        // Input'u tekrar aktif et - bir sonraki hareketi bekliyoruz
        canReceiveInput = true;
        
        // Oyunu duraklat (oyuncu hareket etmiyor)
        if (pauseOverlay != null)
        {
            pauseOverlay.PauseGame();
        }
        
        Debug.Log("Oyuncu hedefe vardı. Bir sonraki hareketi bekliyor...");
    }

    /// <summary>
    /// Kontrol durumunu kontrol et
    /// </summary>
    public bool IsControlled => isControlled;

    /// <summary>
    /// Input alabilir mi?
    /// </summary>
    public bool CanReceiveInput => canReceiveInput;

    /// <summary>
    /// Varsayılan hareket göstergesi sprite'ı oluştur (runtime'da)
    /// </summary>
    Sprite CreateDefaultIndicatorSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        Color indicatorColor = new Color(0f, 1f, 0f, 0.5f); // Yarı saydam yeşil

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    float alpha = indicatorColor.a;
                    if (distance > radius - 2f)
                    {
                        alpha = indicatorColor.a * (1f - ((distance - (radius - 2f)) / 2f));
                    }
                    texture.SetPixel(x, y, new Color(indicatorColor.r, indicatorColor.g, indicatorColor.b, alpha));
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
}

