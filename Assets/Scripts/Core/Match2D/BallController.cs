using UnityEngine;

/// <summary>
/// Top kontrolü - 2D pozisyon sahnesinde top fiziği
/// </summary>
public class BallController : MonoBehaviour
{
    public static BallController Instance { get; private set; }

    [Header("Fizik")]
    public float passSpeed = 10f;
    public float shotSpeed = 15f;
    public float friction = 0.95f;
    
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private PlayerController currentOwner; // Topu kontrol eden oyuncu

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        if (isMoving)
        {
            UpdateBallMovement();
        }
        
        // Sürtünme
        rb.linearVelocity *= friction;
        
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Pas yap (hedef pozisyona)
    /// </summary>
    public void PassTo(Vector2 target, bool isHighPass = false)
    {
        targetPosition = target;
        isMoving = true;
        currentOwner = null;
        
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float speed = isHighPass ? passSpeed * 0.8f : passSpeed;
        
        rb.linearVelocity = direction * speed;
    }

    /// <summary>
    /// Şut at
    /// </summary>
    public void Shoot(Vector2 direction, float power, bool isHighShot = false)
    {
        isMoving = true;
        currentOwner = null;
        
        float speed = shotSpeed * power;
        if (isHighShot)
            speed *= 0.9f;
        
        rb.linearVelocity = direction.normalized * speed;
    }

    /// <summary>
    /// Top hareketini güncelle
    /// </summary>
    private void UpdateBallMovement()
    {
        // Top hedefe yaklaştığında yavaşla
        float distance = Vector2.Distance(transform.position, targetPosition);
        
        if (distance < 0.5f)
        {
            isMoving = false;
            rb.linearVelocity *= 0.5f;
        }
    }

    /// <summary>
    /// Topu bir oyuncuya ver
    /// </summary>
    public void GiveToPlayer(PlayerController player)
    {
        currentOwner = player;
        transform.position = player.transform.position;
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
    }

    /// <summary>
    /// Topu kim kontrol ediyor?
    /// </summary>
    public PlayerController GetCurrentOwner()
    {
        return currentOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Oyuncu topa dokunduğunda
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && currentOwner == null)
        {
            // Top otomatik olarak oyuncuya geçer (yakın mesafede)
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < 0.3f)
            {
                GiveToPlayer(player);
            }
        }
    }
}
