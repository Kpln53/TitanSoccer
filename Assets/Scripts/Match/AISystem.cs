using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AI oyuncu sistemi - Tüm AI oyuncuların davranışlarını yönetir
/// </summary>
public class AISystem : MonoBehaviour
{
    [Header("AI Ayarları")]
    [SerializeField] private float aiUpdateInterval = 0.5f; // AI karar verme aralığı (saniye) - daha az spam için
    [SerializeField] private float reactionRadius = 5f; // Topa tepki verme yarıçapı
    [SerializeField] private float pressRadius = 3f; // Topa baskı yapma yarıçapı

    [Header("Hareket Ayarları")]
    [SerializeField] private float defensiveLineY = 10f; // Defans hattı Y pozisyonu
    [SerializeField] private float offensiveLineY = -10f; // Hücum hattı Y pozisyonu

    private FieldManager fieldManager;
    private BallController ball;
    private PlayerController playerCharacter; // Oyuncunun kontrol ettiği karakter (AI değil)
    
    private List<AIPlayerBehavior> aiPlayers = new List<AIPlayerBehavior>();
    private float lastUpdateTime = 0f;

    private void Start()
    {
        // Referansları bul
        fieldManager = FindObjectOfType<FieldManager>();
        
        if (fieldManager != null)
        {
            ball = fieldManager.Ball;
            playerCharacter = fieldManager.PlayerCharacter;
            
            // AI oyuncuları bul ve setup et
            SetupAIPlayers();
        }
    }

    [Header("Debug")]
    [SerializeField] private bool aiEnabled = false; // Geçici olarak AI hareketini durdur

    private void Update()
    {
        // AI devre dışı bırakıldı - geçici olarak hareket yok
        if (!aiEnabled)
            return;

        if (Time.unscaledTime - lastUpdateTime >= aiUpdateInterval)
        {
            UpdateAI();
            lastUpdateTime = Time.unscaledTime;
        }
    }

    /// <summary>
    /// AI oyuncuları setup et
    /// </summary>
    void SetupAIPlayers()
    {
        if (fieldManager == null) return;

        // Tüm oyuncuları al (oyuncu karakteri hariç)
        List<PlayerController> allPlayers = new List<PlayerController>();
        allPlayers.AddRange(fieldManager.HomePlayers);
        allPlayers.AddRange(fieldManager.AwayPlayers);

        foreach (var player in allPlayers)
        {
            // Oyuncu karakteri değilse AI ekle
            if (player != playerCharacter)
            {
                AIPlayerBehavior aiBehavior = player.GetComponent<AIPlayerBehavior>();
                if (aiBehavior == null)
                {
                    aiBehavior = player.gameObject.AddComponent<AIPlayerBehavior>();
                }

                // AI'ı ayarla
                aiBehavior.Initialize(player, ball, fieldManager);
                aiPlayers.Add(aiBehavior);
            }
        }

        Debug.Log($"[AISystem] {aiPlayers.Count} AI oyuncu setup edildi");
    }

    /// <summary>
    /// AI güncellemesi
    /// </summary>
    void UpdateAI()
    {
        if (ball == null || aiPlayers.Count == 0) return;

        foreach (var aiPlayer in aiPlayers)
        {
            if (aiPlayer != null && aiPlayer.Player != null)
            {
                aiPlayer.UpdateBehavior();
            }
        }
    }
}

/// <summary>
/// Tek bir AI oyuncunun davranışı
/// </summary>
public class AIPlayerBehavior : MonoBehaviour
{
    [Header("AI Davranış Ayarları")]
    [SerializeField] private float reactionRadius = 5f;
    [SerializeField] private float pressRadius = 3f;
    [SerializeField] private float idleRadius = 10f;

    private PlayerController player;
    private BallController ball;
    private FieldManager fieldManager;
    
    private Vector2 targetPosition;
    private Vector2 initialPosition; // Başlangıç pozisyonu
    private AIPlayerRole role;
    private float lastDecisionTime = -1f; // Son karar zamanı
    private float decisionCooldown = 2f; // Karar verme bekleme süresi (saniye)

    /// <summary>
    /// AI'ı başlat
    /// </summary>
    public void Initialize(PlayerController playerController, BallController ballController, FieldManager manager)
    {
        player = playerController;
        ball = ballController;
        fieldManager = manager;

        // Oyuncunun pozisyonuna göre rol belirle
        DetermineRole();

        // Başlangıç pozisyonu
        initialPosition = player.Position;
        targetPosition = initialPosition;
    }

    /// <summary>
    /// Rol belirle (pozisyonuna göre)
    /// </summary>
    void DetermineRole()
    {
        // Basit rol belirleme (pozisyon ismine göre)
        string playerName = player.name.ToLower();
        
        if (playerName.Contains("kl") || playerName.Contains("gk"))
        {
            role = AIPlayerRole.Goalkeeper;
        }
        else if (playerName.Contains("stp") || playerName.Contains("sğb") || playerName.Contains("slb") || playerName.Contains("mdo"))
        {
            role = AIPlayerRole.Defender;
        }
        else if (playerName.Contains("moo") || playerName.Contains("sğo") || playerName.Contains("slo"))
        {
            role = AIPlayerRole.Midfielder;
        }
        else
        {
            role = AIPlayerRole.Attacker;
        }
    }

    /// <summary>
    /// Davranış güncelle
    /// </summary>
    public void UpdateBehavior()
    {
        if (player == null || ball == null || player.IsMoving) return;

        // Cooldown kontrolü - çok sık karar verme
        if (Time.unscaledTime - lastDecisionTime < decisionCooldown)
            return;

        Vector2 ballPos = ball.Position;
        Vector2 playerPos = player.Position;
        float distanceToBall = Vector2.Distance(playerPos, ballPos);

        // Top çok yakınsa ve oyuncu karakterinde değilse, topa koş
        if (distanceToBall < pressRadius && !player.HasBall)
        {
            // Topa baskı yap
            PressBall(ballPos);
            lastDecisionTime = Time.unscaledTime;
        }
        // Top yakındaysa ama baskı yapma mesafesindeyse, pozisyona göre davran
        else if (distanceToBall < reactionRadius)
        {
            ReactToBall(ballPos, playerPos);
            lastDecisionTime = Time.unscaledTime;
        }
        // Top uzaktaysa, pozisyonunu koru (hareket etme)
        else
        {
            MaintainPosition();
        }
    }

    /// <summary>
    /// Topa baskı yap
    /// </summary>
    void PressBall(Vector2 ballPos)
    {
        // Topa doğru koş (biraz önünde dur)
        Vector2 direction = (ballPos - player.Position).normalized;
        targetPosition = ballPos - direction * 0.5f; // Topun biraz önünde dur

        if (!player.IsMoving)
        {
            player.MoveTo(targetPosition);
        }
    }

    /// <summary>
    /// Topa tepki ver (rolüne göre)
    /// </summary>
    void ReactToBall(Vector2 ballPos, Vector2 playerPos)
    {
        // Zaten hareket ediyorsa yeni komut verme
        if (player.IsMoving)
            return;

        switch (role)
        {
            case AIPlayerRole.Goalkeeper:
                // Kaleci pozisyonunu korur
                MaintainPosition();
                break;

            case AIPlayerRole.Defender:
                // Defans oyuncuları topa baskı yapar (sadece gerekirse)
                if (ballPos.y > playerPos.y + 2f) // Top ilerideyse ve mesafe varsa
                {
                    PressBall(ballPos);
                }
                else
                {
                    MaintainPosition();
                }
                break;

            case AIPlayerRole.Midfielder:
                // Orta saha oyuncuları dengeli hareket eder (nadiren)
                if (Random.value < 0.3f) // %30 ihtimalle hareket et
                {
                    Vector2 direction = (ballPos - playerPos).normalized;
                    targetPosition = playerPos + direction * 2f;
                    player.MoveTo(targetPosition);
                }
                break;

            case AIPlayerRole.Attacker:
                // Hücum oyuncuları boş alana koşar (nadiren)
                if (Random.value < 0.4f) // %40 ihtimalle hareket et
                {
                    RunToOpenSpace(ballPos, playerPos);
                }
                break;
        }
    }

    /// <summary>
    /// Pozisyonunu koru
    /// </summary>
    void MaintainPosition()
    {
        // Pozisyon koruma - sürekli hareket etme
        // Sadece başlangıç pozisyonundan çok uzaklaştıysa geri dön
        float distanceFromInitial = Vector2.Distance(player.Position, initialPosition);
        
        // Eğer başlangıç pozisyonundan 4 birimden fazla uzaklaştıysa geri dön
        if (distanceFromInitial > 4f && !player.IsMoving)
        {
            targetPosition = initialPosition;
            player.MoveTo(targetPosition);
        }
        else
        {
            // Pozisyonda kal
            targetPosition = player.Position;
        }
    }

    /// <summary>
    /// Boş alana koş (hücum oyuncuları için)
    /// </summary>
    void RunToOpenSpace(Vector2 ballPos, Vector2 playerPos)
    {
        // Topun ilerisine koş (forvet pozisyonu)
        Vector2 ballDirection = ballPos.normalized;
        Vector2 openSpace = ballPos + ballDirection * 3f; // Topun 3 birim ilerisine

        // Saha sınırları içinde tut
        if (fieldManager != null)
        {
            float halfWidth = fieldManager.FieldWidth / 2f;
            float halfLength = fieldManager.FieldLength / 2f;
            
            openSpace.x = Mathf.Clamp(openSpace.x, -halfWidth + 1f, halfWidth - 1f);
            openSpace.y = Mathf.Clamp(openSpace.y, -halfLength + 1f, halfLength - 1f);
        }

        targetPosition = openSpace;

        if (!player.IsMoving)
        {
            player.MoveTo(targetPosition);
        }
    }

    // Getters
    public PlayerController Player => player;
    public AIPlayerRole Role => role;
}

/// <summary>
/// AI oyuncu rolleri
/// </summary>
public enum AIPlayerRole
{
    Goalkeeper,  // Kaleci
    Defender,    // Defans
    Midfielder,  // Orta saha
    Attacker     // Hücum
}
