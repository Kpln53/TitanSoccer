using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Saha yönetimi - saha boyutları, çizgiler, kale pozisyonları
/// </summary>
public class PitchManager : MonoBehaviour
{
    public static PitchManager Instance { get; private set; }

    [Header("Saha Boyutları (2D)")]
    [SerializeField] private float pitchWidth = 68f;  // Genişlik (X ekseni)
    [SerializeField] private float pitchLength = 105f; // Uzunluk (Y ekseni)
    [SerializeField] private float goalWidth = 7.32f;  // Kale genişliği

    [Header("Saha Görsel (2D)")]
    [SerializeField] private SpriteRenderer pitchSpriteRenderer; // Saha sprite
    [SerializeField] private Sprite pitchSprite;
    [SerializeField] private Color pitchColor = new Color(0.2f, 0.6f, 0.2f); // Yeşil

    [Header("Kale Pozisyonları")]
    [SerializeField] private Transform homeGoalCenter; // Ev sahibi kale merkezi (alt)
    [SerializeField] private Transform awayGoalCenter; // Deplasman kale merkezi (üst)

    [Header("Saha Çizgileri")]
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private Material lineMaterial;

    [Header("Saha Detayları")]
    [SerializeField] private bool showCornerFlags = true;
    [SerializeField] private bool showGoalPosts = true;

    // FIFA standart ölçüleri (metre cinsinden)
    private const float PENALTY_AREA_DEPTH = 16.5f;  // Ceza sahası derinliği
    private const float PENALTY_AREA_WIDTH = 40.32f; // Ceza sahası genişliği
    private const float GOAL_AREA_DEPTH = 5.5f;      // Kale alanı derinliği
    private const float GOAL_AREA_WIDTH = 18.32f;    // Kale alanı genişliği
    private const float PENALTY_SPOT_DISTANCE = 11f; // Penaltı noktası mesafesi
    private const float CENTER_CIRCLE_RADIUS = 9.15f; // Orta yuvarlak yarıçapı

    private List<LineRenderer> pitchLines = new List<LineRenderer>();

    // Saha sınırları
    public float PitchWidth => pitchWidth;
    public float PitchLength => pitchLength;
    public float GoalWidth => goalWidth;

    // Kale pozisyonları (2D world space)
    public Vector2 HomeGoalPosition => homeGoalCenter != null ? (Vector2)homeGoalCenter.position : Vector2.zero;
    public Vector2 AwayGoalPosition => awayGoalCenter != null ? (Vector2)awayGoalCenter.position : Vector2.zero;

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
        CreatePitch();
        CreateGoals();
        CreatePitchLines();
        CreateCenterCircle();
        CreatePenaltyAreas();
        CreateGoalAreas();
        CreatePenaltySpots();
        if (showCornerFlags) CreateCornerFlags();
        if (showGoalPosts) CreateGoalPosts();
    }

    void CreatePitch()
    {
        if (pitchSpriteRenderer == null)
        {
            // 2D saha sprite'ı oluştur
            GameObject pitchObj = new GameObject("Pitch");
            pitchObj.transform.position = Vector3.zero;
            
            pitchSpriteRenderer = pitchObj.AddComponent<SpriteRenderer>();
            
            // Sprite atanmışsa kullan, yoksa runtime'da oluştur
            if (pitchSprite != null)
            {
                pitchSpriteRenderer.sprite = pitchSprite;
            }
            else
            {
                // Runtime'da saha sprite'ı oluştur
                pitchSpriteRenderer.sprite = CreateDefaultPitchSprite();
            }
            
            // Saha boyutlarını ayarla
            pitchSpriteRenderer.transform.localScale = new Vector3(pitchLength, pitchWidth, 1f);
            pitchSpriteRenderer.sortingOrder = 0; // En altta
            
            // Collider2D ekle (2D raycast için)
            BoxCollider2D col = pitchObj.AddComponent<BoxCollider2D>();
            col.size = new Vector2(pitchLength, pitchWidth);
        }
    }

    void CreateGoals()
    {
        // Ev sahibi kale (Y = -pitchLength/2 - alt)
        if (homeGoalCenter == null)
        {
            GameObject homeGoal = new GameObject("HomeGoal");
            homeGoal.transform.position = new Vector3(0, -pitchLength / 2f, 0);
            homeGoalCenter = homeGoal.transform;
        }

        // Deplasman kale (Y = pitchLength/2 - üst)
        if (awayGoalCenter == null)
        {
            GameObject awayGoal = new GameObject("AwayGoal");
            awayGoal.transform.position = new Vector3(0, pitchLength / 2f, 0);
            awayGoalCenter = awayGoal.transform;
        }
    }

    /// <summary>
    /// Bir pozisyonun saha içinde olup olmadığını kontrol eder (2D)
    /// </summary>
    public bool IsPositionInPitch(Vector2 position)
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;

        return Mathf.Abs(position.x) <= halfWidth && 
               Mathf.Abs(position.y) <= halfLength;
    }

    /// <summary>
    /// Pozisyonu saha sınırları içine kısıtlar (2D)
    /// </summary>
    public Vector2 ClampToPitch2D(Vector2 position)
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;

        position.x = Mathf.Clamp(position.x, -halfWidth, halfWidth);
        position.y = Mathf.Clamp(position.y, -halfLength, halfLength);

        return position;
    }

    /// <summary>
    /// Vector3 için de destek (geriye dönük uyumluluk)
    /// </summary>
    public Vector3 ClampToPitch(Vector3 position)
    {
        Vector2 pos2D = ClampToPitch2D(new Vector2(position.x, position.y));
        return new Vector3(pos2D.x, pos2D.y, 0f);
    }

    void OnDrawGizmos()
    {
        // Saha sınırlarını görselleştir (2D)
        Gizmos.color = Color.white;
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;

        // Saha çerçevesi (2D - X, Y eksenleri)
        Vector3 topLeft = new Vector3(-halfWidth, halfLength, 0);
        Vector3 topRight = new Vector3(halfWidth, halfLength, 0);
        Vector3 bottomLeft = new Vector3(-halfWidth, -halfLength, 0);
        Vector3 bottomRight = new Vector3(halfWidth, -halfLength, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Kale çizgileri
        Gizmos.color = Color.red;
        float goalHalfWidth = goalWidth / 2f;
        
        // Ev sahibi kale (alt)
        Vector3 homeGoalLeft = new Vector3(-goalHalfWidth, -halfLength, 0);
        Vector3 homeGoalRight = new Vector3(goalHalfWidth, -halfLength, 0);
        Gizmos.DrawLine(homeGoalLeft, homeGoalRight);

        // Deplasman kale (üst)
        Vector3 awayGoalLeft = new Vector3(-goalHalfWidth, halfLength, 0);
        Vector3 awayGoalRight = new Vector3(goalHalfWidth, halfLength, 0);
        Gizmos.DrawLine(awayGoalLeft, awayGoalRight);
    }

    /// <summary>
    /// Varsayılan saha sprite'ı oluştur (runtime'da)
    /// </summary>
    Sprite CreateDefaultPitchSprite()
    {
        int width = 1050;
        int height = 680;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, pitchColor);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1f);
    }

    /// <summary>
    /// Saha çizgilerini oluştur (kenar çizgileri ve orta saha çizgisi)
    /// </summary>
    void CreatePitchLines()
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;

        // Kenar çizgileri (dış çerçeve)
        CreateLine("PitchOutline", new Vector3[]
        {
            new Vector3(-halfWidth, -halfLength, 0), // Sol alt
            new Vector3(halfWidth, -halfLength, 0),  // Sağ alt
            new Vector3(halfWidth, halfLength, 0),   // Sağ üst
            new Vector3(-halfWidth, halfLength, 0), // Sol üst
            new Vector3(-halfWidth, -halfLength, 0)  // Kapat
        });

        // Orta saha çizgisi
        CreateLine("CenterLine", new Vector3[]
        {
            new Vector3(-halfWidth, 0, 0),
            new Vector3(halfWidth, 0, 0)
        });
    }

    /// <summary>
    /// Orta yuvarlak çizgisini oluştur
    /// </summary>
    void CreateCenterCircle()
    {
        GameObject circleObj = new GameObject("CenterCircle");
        circleObj.transform.SetParent(transform);
        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        SetupLineRenderer(lr);

        int segments = 64;
        lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 360f * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * CENTER_CIRCLE_RADIUS;
            float y = Mathf.Sin(angle) * CENTER_CIRCLE_RADIUS;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }

        pitchLines.Add(lr);
    }

    /// <summary>
    /// Ceza sahalarını oluştur (her iki tarafta)
    /// </summary>
    void CreatePenaltyAreas()
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;
        float penaltyHalfWidth = PENALTY_AREA_WIDTH / 2f;

        // Home team ceza sahası (alt)
        CreateLine("HomePenaltyArea", new Vector3[]
        {
            new Vector3(-penaltyHalfWidth, -halfLength, 0),
            new Vector3(penaltyHalfWidth, -halfLength, 0),
            new Vector3(penaltyHalfWidth, -halfLength + PENALTY_AREA_DEPTH, 0),
            new Vector3(-penaltyHalfWidth, -halfLength + PENALTY_AREA_DEPTH, 0),
            new Vector3(-penaltyHalfWidth, -halfLength, 0)
        });

        // Away team ceza sahası (üst)
        CreateLine("AwayPenaltyArea", new Vector3[]
        {
            new Vector3(-penaltyHalfWidth, halfLength, 0),
            new Vector3(penaltyHalfWidth, halfLength, 0),
            new Vector3(penaltyHalfWidth, halfLength - PENALTY_AREA_DEPTH, 0),
            new Vector3(-penaltyHalfWidth, halfLength - PENALTY_AREA_DEPTH, 0),
            new Vector3(-penaltyHalfWidth, halfLength, 0)
        });
    }

    /// <summary>
    /// Kale alanlarını oluştur (her iki tarafta)
    /// </summary>
    void CreateGoalAreas()
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;
        float goalAreaHalfWidth = GOAL_AREA_WIDTH / 2f;

        // Home team kale alanı (alt)
        CreateLine("HomeGoalArea", new Vector3[]
        {
            new Vector3(-goalAreaHalfWidth, -halfLength, 0),
            new Vector3(goalAreaHalfWidth, -halfLength, 0),
            new Vector3(goalAreaHalfWidth, -halfLength + GOAL_AREA_DEPTH, 0),
            new Vector3(-goalAreaHalfWidth, -halfLength + GOAL_AREA_DEPTH, 0),
            new Vector3(-goalAreaHalfWidth, -halfLength, 0)
        });

        // Away team kale alanı (üst)
        CreateLine("AwayGoalArea", new Vector3[]
        {
            new Vector3(-goalAreaHalfWidth, halfLength, 0),
            new Vector3(goalAreaHalfWidth, halfLength, 0),
            new Vector3(goalAreaHalfWidth, halfLength - GOAL_AREA_DEPTH, 0),
            new Vector3(-goalAreaHalfWidth, halfLength - GOAL_AREA_DEPTH, 0),
            new Vector3(-goalAreaHalfWidth, halfLength, 0)
        });
    }

    /// <summary>
    /// Penaltı noktalarını oluştur
    /// </summary>
    void CreatePenaltySpots()
    {
        float halfLength = pitchLength / 2f;

        // Home team penaltı noktası
        CreatePenaltySpot("HomePenaltySpot", new Vector3(0, -halfLength + PENALTY_SPOT_DISTANCE, 0));

        // Away team penaltı noktası
        CreatePenaltySpot("AwayPenaltySpot", new Vector3(0, halfLength - PENALTY_SPOT_DISTANCE, 0));
    }

    /// <summary>
    /// Tek bir penaltı noktası oluştur (küçük daire)
    /// </summary>
    void CreatePenaltySpot(string name, Vector3 position)
    {
        GameObject spotObj = new GameObject(name);
        spotObj.transform.SetParent(transform);
        LineRenderer lr = spotObj.AddComponent<LineRenderer>();
        SetupLineRenderer(lr);

        float radius = 0.3f;
        int segments = 16;
        lr.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 360f * Mathf.Deg2Rad;
            float x = position.x + Mathf.Cos(angle) * radius;
            float y = position.y + Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }

        pitchLines.Add(lr);
    }

    /// <summary>
    /// Köşe bayraklarını oluştur (görsel olarak küçük daireler)
    /// </summary>
    void CreateCornerFlags()
    {
        float halfWidth = pitchWidth / 2f;
        float halfLength = pitchLength / 2f;

        CreateCornerFlag("CornerFlag_TopLeft", new Vector3(-halfWidth, halfLength, 0));
        CreateCornerFlag("CornerFlag_TopRight", new Vector3(halfWidth, halfLength, 0));
        CreateCornerFlag("CornerFlag_BottomLeft", new Vector3(-halfWidth, -halfLength, 0));
        CreateCornerFlag("CornerFlag_BottomRight", new Vector3(halfWidth, -halfLength, 0));
    }

    /// <summary>
    /// Tek bir köşe bayrağı oluştur
    /// </summary>
    void CreateCornerFlag(string name, Vector3 position)
    {
        GameObject flagObj = new GameObject(name);
        flagObj.transform.SetParent(transform);
        flagObj.transform.position = position;

        // Küçük bir sprite renderer ekle (basit görsel)
        SpriteRenderer sr = flagObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(0.5f, Color.yellow);
        sr.sortingOrder = 10; // Çizgilerin üstünde
    }

    /// <summary>
    /// Kale direklerini oluştur (görsel olarak)
    /// </summary>
    void CreateGoalPosts()
    {
        float halfLength = pitchLength / 2f;
        float goalHalfWidth = goalWidth / 2f;

        // Home goal posts (alt)
        CreateGoalPost("HomeGoalPost_Left", new Vector3(-goalHalfWidth, -halfLength, 0));
        CreateGoalPost("HomeGoalPost_Right", new Vector3(goalHalfWidth, -halfLength, 0));

        // Away goal posts (üst)
        CreateGoalPost("AwayGoalPost_Left", new Vector3(-goalHalfWidth, halfLength, 0));
        CreateGoalPost("AwayGoalPost_Right", new Vector3(goalHalfWidth, halfLength, 0));
    }

    /// <summary>
    /// Tek bir kale direği oluştur
    /// </summary>
    void CreateGoalPost(string name, Vector3 position)
    {
        GameObject postObj = new GameObject(name);
        postObj.transform.SetParent(transform);
        postObj.transform.position = position;

        SpriteRenderer sr = postObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(0.4f, Color.white);
        sr.sortingOrder = 10; // Çizgilerin üstünde
    }

    /// <summary>
    /// LineRenderer oluştur ve ayarla
    /// </summary>
    LineRenderer CreateLine(string name, Vector3[] points)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        SetupLineRenderer(lr);

        lr.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i]);
        }

        pitchLines.Add(lr);
        return lr;
    }

    /// <summary>
    /// LineRenderer ayarlarını yap
    /// </summary>
    void SetupLineRenderer(LineRenderer lr)
    {
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.useWorldSpace = true;
        lr.sortingOrder = 5; // Saha zemininin üstünde
        lr.material = lineMaterial != null ? lineMaterial : CreateDefaultLineMaterial();
        lr.loop = false;
    }

    /// <summary>
    /// Varsayılan çizgi materyali oluştur
    /// </summary>
    Material CreateDefaultLineMaterial()
    {
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = lineColor;
        return mat;
    }

    /// <summary>
    /// Küçük bir daire sprite'ı oluştur
    /// </summary>
    Sprite CreateCircleSprite(float radius, Color color)
    {
        int size = Mathf.CeilToInt(radius * 2 * 10);
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float r = radius * 10f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist <= r)
                {
                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 10f);
    }
}

