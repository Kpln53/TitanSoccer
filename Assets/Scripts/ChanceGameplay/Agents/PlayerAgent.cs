using UnityEngine;

/// <summary>
/// Player Agent - Oyuncu AI ve istatistikler (overall-based)
/// </summary>
public class PlayerAgent : MonoBehaviour
{
    [Header("Info")]
    public string playerName = "Player";
    public PlayerPosition position = PlayerPosition.SF;
    public int overall = 50;
    public bool IsHomeTeam = true;

    [Header("Derived Stats (overall'dan türetilir)")]
    public float speed = 5f;
    public float passAccuracy = 50f;
    public float tackleSkill = 50f;
    public float reaction = 50f;
    public float control = 50f;

    [Header("Team Factor")]
    public float teamStrengthFactor = 1.0f; // 0.90-1.10

    private Rigidbody2D rb;

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

        // Collider
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.3f;
        }
    }

    private void Start()
    {
        CalculateStats();
    }

    /// <summary>
    /// Overall'dan stat türet
    /// </summary>
    private void CalculateStats()
    {
        speed = 3f + (overall / 100f) * 4f;          // 3-7
        passAccuracy = overall;
        tackleSkill = overall;
        reaction = overall;
        control = overall;
    }

    /// <summary>
    /// Team factor uygula
    /// </summary>
    public float GetEffectiveStat(float baseStat)
    {
        return baseStat * teamStrengthFactor;
    }
}

