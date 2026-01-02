using UnityEngine;

/// <summary>
/// Ball Controller - Top kontrolü (2D)
/// Controlled: oyuncuya yapışır (dribble)
/// InFlight: hedefe uçar (pas/şut)
/// Clear: güçlü vurma
/// </summary>
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 attachOffset = new Vector2(0.3f, 0f);
    public float arriveDistance = 0.2f;

    private PossessionManager possessionManager;
    private Rigidbody2D rb;

    // InFlight state
    private bool isInFlight;
    private Vector3 flightTarget;
    private float flightSpeed;
    private GameObject flightReceiver;
    private PossessionManager.Team flightTeam;

    // Clear state
    private bool isClearing;
    private Vector3 clearVelocity;
    private float clearDuration = 0.5f;
    private float clearTimer;

    private void Awake()
    {
        possessionManager = PossessionManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Collider2D (eğer yoksa)
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.15f;
            col.isTrigger = true;
        }
    }

    private void LateUpdate()
    {
        if (possessionManager == null) return;

        // Controlled: oyuncuya yapış (dribble)
        if (possessionManager.CurrentBallState == PossessionManager.BallState.Controlled
            && possessionManager.OwnerPlayer != null)
        {
            Vector3 target = possessionManager.OwnerPlayer.transform.position + (Vector3)attachOffset;
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 20f);
            isInFlight = false;
            isClearing = false;
        }
    }

    private void Update()
    {
        // InFlight: pas uçuşu
        if (isInFlight)
        {
            transform.position = Vector3.MoveTowards(transform.position, flightTarget, flightSpeed * Time.unscaledDeltaTime);

            if (Vector3.Distance(transform.position, flightTarget) <= arriveDistance)
            {
                isInFlight = false;

                if (flightReceiver != null && possessionManager != null)
                {
                    possessionManager.SetControlled(flightTeam, flightReceiver);
                }
                else if (possessionManager != null)
                {
                    possessionManager.SetLoose();
                }
            }
        }

        // Clear: kısa süreli ileri fırlatma
        if (isClearing)
        {
            transform.position += clearVelocity * Time.unscaledDeltaTime;
            clearTimer += Time.unscaledDeltaTime;

            if (clearTimer >= clearDuration)
            {
                isClearing = false;
            }
        }
    }

    /// <summary>
    /// Pas at (receiver'a uçur)
    /// </summary>
    public void PassTo(GameObject receiver, PossessionManager.Team team, float speed)
    {
        if (receiver == null) return;

        flightReceiver = receiver;
        flightTeam = team;
        flightTarget = receiver.transform.position;
        flightSpeed = speed;
        isInFlight = true;

        if (possessionManager != null)
        {
            possessionManager.SetInFlight(team, possessionManager.OwnerPlayer, receiver);
        }

        Debug.Log($"[Ball] Pass to {receiver.name} (speed: {speed})");
    }

    /// <summary>
    /// Clear: topu güçlü vur (pozisyon bitiş animasyonu)
    /// </summary>
    public void Clear(Vector3 direction, float speed)
    {
        clearVelocity = direction.normalized * speed;
        clearTimer = 0f;
        isClearing = true;
        isInFlight = false;

        Debug.Log($"[Ball] Clear! Direction: {direction}");
    }
}

