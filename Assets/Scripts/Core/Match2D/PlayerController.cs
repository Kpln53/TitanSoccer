using UnityEngine;

/// <summary>
/// 2D pozisyon sahnesinde oyuncu kontrolü (dokunmatik kontroller)
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public int playerId;
    public bool isPlayerControlled = false; // Oyuncu tarafından kontrol edilen oyuncu mu?
    public PlayerPosition position;
    
    [Header("Hareket")]
    public float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool isMoving = false;

    [Header("Pas/Şut")]
    private bool isShooting = false;
    private Vector2 shootDirection;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f; // 2D futbol için yerçekimi yok
    }

    private void Update()
    {
        if (isPlayerControlled)
        {
            HandlePlayerInput();
        }
        
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    /// <summary>
    /// Oyuncu girdilerini işle
    /// </summary>
    private void HandlePlayerInput()
    {
        // Hareket (boş yere dokunma)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 touchPosition = GetTouchPosition();
            
            // Raycast ile sahada boş bir noktaya mı dokunuldu kontrol et
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            
            if (hit.collider == null || hit.collider.tag != "Player")
            {
                // Boş alana dokunuldu - koşu komutu
                MoveToPosition(touchPosition);
            }
        }

        // Şut (basılı tut + sürükle)
        if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            // Oyuncunun sprite'ına basılı tutuluyorsa şut modu
            // (UI sistemi ile daha iyi kontrol edilebilir)
        }
    }

    /// <summary>
    /// Dokunma pozisyonunu al (mouse veya touch)
    /// </summary>
    private Vector2 GetTouchPosition()
    {
        if (Input.touchCount > 0)
        {
            return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    /// <summary>
    /// Belirli bir pozisyona hareket et
    /// </summary>
    public void MoveToPosition(Vector2 target)
    {
        targetPosition = target;
        isMoving = true;
        
        if (MatchChanceSceneManager.Instance != null)
        {
            MatchChanceSceneManager.Instance.OnRunCommand();
        }
    }

    /// <summary>
    /// Hedefe doğru hareket et
    /// </summary>
    private void MoveToTarget()
    {
        Vector2 currentPos = transform.position;
        float distance = Vector2.Distance(currentPos, targetPosition);
        
        if (distance > 0.1f)
        {
            Vector2 direction = (targetPosition - currentPos).normalized;
            rb.linearVelocity = direction * moveSpeed * Time.timeScale;
        }
        else
        {
            // Hedefe ulaşıldı
            rb.linearVelocity = Vector2.zero;
            isMoving = false;
            
            // Oyun tekrar yavaşlar (MatchChanceSceneManager tarafından yapılacak)
            if (MatchChanceSceneManager.Instance != null)
            {
                // Pozisyon yeniden yavaşlatılır
            }
        }
    }

    /// <summary>
    /// Pas yap (takım arkadaşına)
    /// </summary>
    public void PassTo(PlayerController targetPlayer, bool isHighPass = false)
    {
        if (MatchChanceSceneManager.Instance != null)
        {
            // Pas zinciri sistemi
            MatchChanceSceneManager.Instance.OnPassMade(true); // Başarılı pas varsayımı
            
            // Pas fiziği (BallController tarafından yapılacak)
            if (BallController.Instance != null)
            {
                BallController.Instance.PassTo(targetPlayer.transform.position, isHighPass);
            }
        }
    }

    /// <summary>
    /// Şut at
    /// </summary>
    public void Shoot(Vector2 direction, float power, bool isHighShot = false)
    {
        if (MatchChanceSceneManager.Instance != null)
        {
            MatchChanceSceneManager.Instance.OnShotTaken();
            
            // Şut fiziği
            if (BallController.Instance != null)
            {
                BallController.Instance.Shoot(direction, power, isHighShot);
            }
            
            // Şut sonucunu hesapla (pozisyon biter)
            MatchChanceSceneManager.Instance.CalculateShotResult(direction, power, isHighShot, this);
        }
    }

    /// <summary>
    /// Mevkisini al (pozisyon kontrolü için)
    /// </summary>
    public Vector2 GetPositionOnField()
    {
        return transform.position;
    }
}
