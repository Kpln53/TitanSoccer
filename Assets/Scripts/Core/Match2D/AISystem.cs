using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 2D pozisyon sahnesi AI sistemi - Savunma ve hücum AI
/// </summary>
public class AISystem : MonoBehaviour
{
    public static AISystem Instance { get; private set; }

    [Header("Savunma AI")]
    public float pressureRadius = 3f; // Baskı yarıçapı
    public float channelCloseRadius = 5f; // Pas kanalı kapatma yarıçapı
    public int maxPressuringPlayers = 2; // Maksimum baskı yapan oyuncu sayısı

    [Header("Hücum AI")]
    public float runToSpaceRadius = 4f; // Boş alana koşu yarıçapı
    public float passOptionRadius = 3f; // Pas opsiyonu yarıçapı

    private List<PlayerController> allPlayers = new List<PlayerController>();
    private BallController ball;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        ball = BallController.Instance;
        CollectAllPlayers();
    }

    private void Update()
    {
        if (ball == null)
            return;

        UpdateDefenseAI();
        UpdateAttackAI();
    }

    /// <summary>
    /// Tüm oyuncuları topla
    /// </summary>
    private void CollectAllPlayers()
    {
        allPlayers.Clear();
        allPlayers.AddRange(FindObjectsOfType<PlayerController>());
    }

    /// <summary>
    /// Savunma AI'ını güncelle
    /// </summary>
    private void UpdateDefenseAI()
    {
        if (ball == null || ball.GetCurrentOwner() == null)
            return;

        PlayerController ballOwner = ball.GetCurrentOwner();
        Vector2 ballPos = ball.transform.position;

        // Hangi takım hücum yapıyor?
        bool isHomeAttacking = FieldManager.Instance.homePlayers.Contains(ballOwner);
        List<PlayerController> defenders = isHomeAttacking ? 
            FieldManager.Instance.awayPlayers : 
            FieldManager.Instance.homePlayers;

        // Topa en yakın 1-2 oyuncu baskı yapar
        List<PlayerController> pressuringPlayers = defenders
            .OrderBy(p => Vector2.Distance(p.transform.position, ballPos))
            .Take(maxPressuringPlayers)
            .ToList();

        foreach (var defender in defenders)
        {
            if (pressuringPlayers.Contains(defender))
            {
                // Baskı yap - topa doğru koş
                MoveTowards(defender, ballPos, 1.2f);
            }
            else
            {
                // Pas kanalı kapat veya alan savunması yap
                Vector2 targetPos = GetDefensivePosition(defender, ballPos, ballOwner.transform.position);
                MoveTowards(defender, targetPos, 0.8f);
            }
        }
    }

    /// <summary>
    /// Hücum AI'ını güncelle
    /// </summary>
    private void UpdateAttackAI()
    {
        if (ball == null || ball.GetCurrentOwner() == null)
            return;

        PlayerController ballOwner = ball.GetCurrentOwner();
        Vector2 ballPos = ball.transform.position;

        // Hangi takım hücum yapıyor?
        bool isHomeAttacking = FieldManager.Instance.homePlayers.Contains(ballOwner);
        List<PlayerController> attackers = isHomeAttacking ? 
            FieldManager.Instance.homePlayers : 
            FieldManager.Instance.awayPlayers;

        // Kontrol edilen oyuncu hariç diğerleri otomatik koşu yapar
        foreach (var attacker in attackers)
        {
            if (attacker == ballOwner || attacker.isPlayerControlled)
                continue; // Top sahibi veya oyuncu kontrolünde olanlar hariç

            // Boş alana koşu yap
            Vector2 runTarget = FindRunToSpace(attacker, ballPos);
            if (runTarget != Vector2.zero)
            {
                attacker.MoveToPosition(runTarget);
            }
        }
    }

    /// <summary>
    /// Savunma pozisyonu hesapla
    /// </summary>
    private Vector2 GetDefensivePosition(PlayerController defender, Vector2 ballPos, Vector2 attackerPos)
    {
        // Top ve hücum oyuncusu arasındaki çizgiyi kapat
        Vector2 direction = (attackerPos - ballPos).normalized;
        Vector2 defensivePos = ballPos + direction * channelCloseRadius;

        // Mevki sınırlarını kontrol et (oyuncu çok uzaklaşmasın)
        Vector2 originalPos = defender.transform.position;
        float maxDistance = 5f; // Maksimum mevki sapması

        if (Vector2.Distance(defensivePos, originalPos) > maxDistance)
        {
            // Orijinal pozisyona yakın kal
            defensivePos = originalPos + (defensivePos - originalPos).normalized * maxDistance;
        }

        return defensivePos;
    }

    /// <summary>
    /// Boş alana koşu hedefi bul
    /// </summary>
    private Vector2 FindRunToSpace(PlayerController attacker, Vector2 ballPos)
    {
        // Topun önünde boş alan ara
        Vector2 forwardDirection = GetAttackDirection(attacker);
        Vector2 targetPos = ballPos + forwardDirection * runToSpaceRadius;

        // Bu pozisyon boş mu kontrol et
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPos, 1f);
        bool isSpaceFree = !colliders.Any(c => c.tag == "Player");

        if (isSpaceFree)
        {
            return targetPos;
        }

        return Vector2.zero; // Boş alan bulunamadı
    }

    /// <summary>
    /// Hücum yönünü al
    /// </summary>
    private Vector2 GetAttackDirection(PlayerController attacker)
    {
        // Kale yönüne doğru (basit)
        bool isHome = FieldManager.Instance.homePlayers.Contains(attacker);
        return isHome ? Vector2.up : Vector2.down;
    }

    /// <summary>
    /// Oyuncuyu hedefe doğru hareket ettir
    /// </summary>
    private void MoveTowards(PlayerController player, Vector2 target, float speedMultiplier)
    {
        Vector2 currentPos = player.transform.position;
        Vector2 direction = (target - currentPos).normalized;
        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * player.moveSpeed * speedMultiplier * Time.timeScale;
        }
    }
}
