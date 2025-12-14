using UnityEngine;

/// <summary>
/// Oyuncu temel sınıfı - basit ve temiz
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public string playerName;
    public PlayerPosition position;
    public int overall;
    public bool isHomeTeam;

    [Header("Yetenekler")]
    [SerializeField] private float passingSkill;
    [SerializeField] private float shootingSkill;
    [SerializeField] private float dribblingSkill;
    [SerializeField] private float speed;
    [SerializeField] private float stamina;

    [Header("Fizik")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Görsel")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color homeTeamColor = Color.blue;
    [SerializeField] private Color awayTeamColor = Color.red;

    private Vector2 targetPosition;
    private bool isMoving = false;

    public System.Action OnMovementComplete;

    void Awake()
    {
        // Rigidbody
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic - transform ile hareket

        // Collider (Trigger olarak - topa değdiğinde algılansın)
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;
            col.isTrigger = true; // Trigger olarak ayarla
        }
        else
        {
            // Mevcut collider'ı trigger yap
            Collider2D existingCol = GetComponent<Collider2D>();
            if (existingCol is CircleCollider2D circleCol)
            {
                circleCol.isTrigger = true;
            }
        }
    }

    void Start()
    {
        CalculateSkills();
        SetupVisuals();
    }

    void Update()
    {
        // Z pozisyonunu 0'da tut
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    void CalculateSkills()
    {
        float overallNormalized = overall / 100f;
        passingSkill = overallNormalized * Random.Range(0.8f, 1.2f);
        shootingSkill = overallNormalized * Random.Range(0.8f, 1.2f);
        dribblingSkill = overallNormalized * Random.Range(0.8f, 1.2f);
        speed = overallNormalized * Random.Range(0.8f, 1.2f);
        stamina = overallNormalized * Random.Range(0.8f, 1.2f);
        moveSpeed = 3f + (speed * 4f);
    }

    void SetupVisuals()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = CreateDefaultPlayerSprite();
        spriteRenderer.color = isHomeTeam ? homeTeamColor : awayTeamColor;
        spriteRenderer.sortingOrder = 1;
    }

    public void MoveTo(Vector2 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    void MoveTowardsTarget()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = targetPosition - currentPos;
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            Vector2 moveDir = direction.normalized;
            Vector2 movement = moveDir * moveSpeed * Time.deltaTime;
            
            if (movement.magnitude > distance)
                movement = direction;

            transform.position = new Vector3(
                currentPos.x + movement.x,
                currentPos.y + movement.y,
                0f
            );

            // Rotasyon
            if (moveDir != Vector2.zero)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(0, 0, angle - 90f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = new Vector3(targetPosition.x, targetPosition.y, 0f);
            isMoving = false;
            OnMovementComplete?.Invoke();
        }
    }

    public void Stop()
    {
        isMoving = false;
    }

    Sprite CreateDefaultPlayerSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        Color color = isHomeTeam ? homeTeamColor : awayTeamColor;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= radius)
                {
                    float alpha = dist > radius - 2f ? 1f - ((dist - (radius - 2f)) / 2f) : 1f;
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
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

    public Vector2 GetTargetPosition() => targetPosition;
    public float PassingSkill => passingSkill;
    public float ShootingSkill => shootingSkill;
    public float DribblingSkill => dribblingSkill;
    public float Speed => speed;
    public float Stamina => stamina;
    public bool IsMoving => isMoving;
}
