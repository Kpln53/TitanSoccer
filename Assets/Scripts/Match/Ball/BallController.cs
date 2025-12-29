using UnityEngine;

/// <summary>
/// Top kontrol sistemi - Fizik ve kontrol durumu
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Fizik Ayarları")]
    [SerializeField] private float drag = 2f;
    [SerializeField] private float mass = 0.5f;

    private Rigidbody2D rb;
    private bool isControlled = false; // Şu an oyuncu kontrolünde mi?
    private PlayerController controllingPlayer;

    // Events
    public System.Action<PlayerController> OnControlGained;
    public System.Action OnControlLost;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.mass = mass;
        rb.linearDamping = drag;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Layer ve Tag
        gameObject.layer = LayerMask.NameToLayer("Ball");
        gameObject.tag = "Ball";

        // Collider
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
            col.isTrigger = true; // Trigger olarak (PlayerController temas algılayacak)
        }
    }

    private void Start()
    {
        // Başlangıçta top oyuncuya yakın olmalı (sahne kurulumunda ayarlanacak)
    }

    /// <summary>
    /// Topu belirli bir hızla fırlat (şut/pas)
    /// </summary>
    public void Launch(Vector2 direction, float speed)
    {
        isControlled = false;
        controllingPlayer = null;

        rb.linearVelocity = direction.normalized * speed;
        OnControlLost?.Invoke();

        Debug.Log($"[BallController] Top fırlatıldı! Yön: {direction}, Hız: {speed}");
    }

    /// <summary>
    /// Topu durdur
    /// </summary>
    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    /// <summary>
    /// Topu belirli bir pozisyona yerleştir
    /// </summary>
    public void SetPosition(Vector2 position)
    {
        rb.position = position;
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Top hızını al
    /// </summary>
    public float GetSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    /// <summary>
    /// Top durdu mu? (pas/şut animasyonu bitmiş mi?)
    /// </summary>
    public bool IsStopped()
    {
        return rb.linearVelocity.magnitude < 0.1f;
    }

    // Getters
    public bool IsControlled => isControlled;
    public PlayerController ControllingPlayer => controllingPlayer;
    public Vector2 Position => rb.position;
    public Vector2 Velocity => rb.linearVelocity;
}
