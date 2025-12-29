using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyuncu kontrol sistemi - Hareket ve top temas algılama
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stopDistance = 0.1f;
    
    [Header("Top Kontrolü")]
    [SerializeField] private float ballOffsetDistance = 0.6f; // Topun oyuncunun önünde olacağı mesafe
    [SerializeField] private float ballFollowSpeed = 20f; // Topun oyuncuyu takip etme hızı

    [Header("Layer Ayarları")]
    [SerializeField] private LayerMask groundLayer = 1 << 8; // Ground layer

    private Vector2 targetPosition;
    private bool isMoving = false;
    private bool hasBall = false;
    private BallController ballController; // Top referansı
    private Vector2 lastMoveDirection = Vector2.right; // Son hareket yönü (top pozisyonu için)

    // Components
    private Rigidbody2D rb;
    private CircleCollider2D col;

    // Events
    public System.Action OnMovementStart;
    public System.Action OnMovementComplete;
    public System.Action OnBallGained;
    public System.Action OnBallLost;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 5f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;
        }

        // Layer ayarla
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
        
        // Topla dripling - Her frame topu oyuncunun önünde tut
        if (hasBall && ballController != null)
        {
            UpdateBallDribble();
        }
    }
    
    /// <summary>
    /// Topla dripling - Topu oyuncunun önünde tut (tamamen bağlı)
    /// </summary>
    void UpdateBallDribble()
    {
        if (ballController == null)
        {
            hasBall = false;
            return;
        }
        
        Vector2 playerPos = rb.position;
        Vector2 ballPos = ballController.transform.position;
        
        // Hareket yönünü belirle
        Vector2 ballDirection = lastMoveDirection;
        
        if (isMoving)
        {
            // Hareket ederken, hareket yönünü kullan
            Vector2 moveDir = (targetPosition - playerPos);
            if (moveDir.magnitude > 0.01f)
            {
                ballDirection = moveDir.normalized;
                lastMoveDirection = ballDirection;
            }
        }
        
        // Topun olması gereken pozisyon (oyuncunun önü)
        Vector2 targetBallPos = playerPos + ballDirection * ballOffsetDistance;
        
        // Top mesafesi kontrolü
        float distance = Vector2.Distance(ballPos, targetBallPos);
        
        // Top çok uzaktaysa kaybet
        if (distance > 2f)
        {
            hasBall = false;
            ballController = null;
            OnBallLost?.Invoke();
            
            if (TimeFlowManager.Instance != null)
            {
                TimeFlowManager.Instance.NormalizeTime();
            }
            
            Debug.Log("[PlayerController] Top çok uzaklaştı, kontrol kaybedildi!");
            return;
        }
        
        // Topu hedef pozisyona taşı (smooth veya direkt)
        if (distance > 1.5f)
        {
            // Çok uzaktaysa direkt yerleştir
            ballController.SetPosition(targetBallPos);
        }
        else
        {
            // Smooth bir şekilde takip et
            Vector2 newBallPos = Vector2.Lerp(ballPos, targetBallPos, ballFollowSpeed * Time.deltaTime);
            ballController.SetPosition(newBallPos);
        }
        
        // Top her zaman durmalı (fiziksel hareket yok, sadece pozisyon güncellemesi)
        ballController.Stop();
    }

    /// <summary>
    /// Hedefe hareket et
    /// </summary>
    public void MoveTo(Vector2 targetPos)
    {
        // Ground layer kontrolü (targetPos zaten ground'da olmalı ama kontrol edelim)
        targetPosition = targetPos;
        isMoving = true;
        OnMovementStart?.Invoke();
        
        // Hareket başladığında zaman normal olur
        if (TimeFlowManager.Instance != null)
        {
            TimeFlowManager.Instance.NormalizeTime();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector2 currentPos = rb.position;
        Vector2 direction = targetPosition - currentPos;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Vector2 moveDir = direction.normalized;
            Vector2 movement = moveDir * moveSpeed * Time.deltaTime;

            if (movement.magnitude > distance)
                movement = direction;

            rb.MovePosition(currentPos + movement);
        }
        else
        {
            // Hedefe vardı
            rb.position = targetPosition;
            isMoving = false;
            OnMovementComplete?.Invoke();

            // Hareket bitti, top hala bizdeyse zaman yavaşlar
            if (hasBall && TimeFlowManager.Instance != null)
            {
                TimeFlowManager.Instance.SlowDownTime();
            }
        }
    }

    /// <summary>
    /// Hareketi durdur
    /// </summary>
    public void Stop()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Top temas algılama
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // Top bize geldi
            if (!hasBall)
            {
                ballController = other.GetComponent<BallController>();
                if (ballController == null)
                {
                    ballController = other.GetComponentInParent<BallController>();
                }
                
                if (ballController != null)
                {
                    hasBall = true;
                    
                    // Topu direkt oyuncunun önüne yerleştir
                    Vector2 playerPos = rb.position;
                    Vector2 targetBallPos = playerPos + lastMoveDirection * ballOffsetDistance;
                    ballController.SetPosition(targetBallPos);
                    ballController.Stop();
                    
                    OnBallGained?.Invoke();

                    // Top oyuncudayken zaman yavaşlar
                    if (TimeFlowManager.Instance != null)
                    {
                        TimeFlowManager.Instance.SlowDownTime();
                    }
                    
                    Debug.Log("[PlayerController] Top alındı! Top oyuncunun önüne yerleştirildi.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            // Top bizden ayrıldı - biraz bekle, eğer gerçekten uzaktaysa kaybet
            // (Bu kontrol UpdateBallDribble'da yapılıyor, burada sadece kontrol ediyoruz)
            if (hasBall && ballController != null)
            {
                float distance = Vector2.Distance(rb.position, ballController.transform.position);
                
                // 1.5 birimden uzaksa topu kaybettik
                if (distance > 1.5f)
                {
                    hasBall = false;
                    ballController = null;
                    OnBallLost?.Invoke();

                    // Top kaybedilince zaman normal olur
                    if (TimeFlowManager.Instance != null)
                    {
                        TimeFlowManager.Instance.NormalizeTime();
                    }
                    
                    Debug.Log("[PlayerController] Top kaybedildi!");
                }
            }
        }
    }

    // Getters
    public bool HasBall => hasBall;
    public bool IsMoving => isMoving;
    public Vector2 Position => rb.position;
    public float MoveSpeed => moveSpeed;
}
