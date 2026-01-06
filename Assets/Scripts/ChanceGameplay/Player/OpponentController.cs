using UnityEngine;
using System.Collections;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Rakip Kontrolcüsü
    /// Savunma pozisyonlarında atak yapan rakip AI
    /// </summary>
    public class OpponentController : MonoBehaviour
    {
        [Header("AI Ayarları")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float decisionInterval = 1.2f;  // Daha uzun - oyuncuya müdahale şansı
        [SerializeField] private float shootRange = 10f;
        [SerializeField] private float passRange = 8f;
        [SerializeField] private float dribblingSpeed = 4f;

        [Header("Stats")]
        [SerializeField] private int teamPower = 65;
        [SerializeField] private int overall = 65;
        [SerializeField] private int shootingStat = 65;
        [SerializeField] private int passingStat = 65;

        [Header("Durum")]
        [SerializeField] private bool hasBall = false;
        [SerializeField] private bool isActive = false;
        [SerializeField] private bool isAttacking = false;
        [SerializeField] private bool isPreparingAction = false;  // Şut/pas hazırlığında

        private Rigidbody2D rb;
        private float decisionTimer;
        private Vector2 targetPosition;
        private bool isMoving = false;
        private bool firstDecisionMade = false;

        public bool HasBall => hasBall;
        public bool IsPreparingAction => isPreparingAction;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            // Collider
            if (GetComponent<CircleCollider2D>() == null)
            {
                var col = gameObject.AddComponent<CircleCollider2D>();
                col.radius = 0.4f;
            }
        }

        private void Start()
        {
            // Savunma pozisyonunda mı atak pozisyonunda mı?
            if (ChanceController.Instance != null)
            {
                isAttacking = ChanceController.Instance.CurrentChanceType == ChanceType.Defense;
                
                // Savunma pozisyonunda rakip aktif başlar
                if (isAttacking)
                {
                    isActive = true;
                }
            }
        }

        private void Update()
        {
            if (!isActive) return;
            if (isPreparingAction) return;  // Şut/pas hazırlığındayken bekleme

            decisionTimer += Time.deltaTime;

            // İlk karar biraz gecikmeli
            float currentInterval = firstDecisionMade ? decisionInterval : 0.8f;

            if (decisionTimer >= currentInterval)
            {
                if (hasBall)
                {
                    MakeAttackDecision();
                    firstDecisionMade = true;
                }
                else if (isAttacking)
                {
                    // Toplu rakip değilsek destek pozisyonu al
                    MakeSupportDecision();
                }
                else
                {
                    MakeDefenseDecision();
                }
                decisionTimer = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (isMoving && !isPreparingAction)
            {
                MoveToTarget();
            }
        }
        
        /// <summary>
        /// Destek pozisyonu kararı (toplu değilken)
        /// </summary>
        private void MakeSupportDecision()
        {
            if (ChanceController.Instance == null) return;
            
            // Kaleye doğru destek pozisyonu al
            Vector2 ourGoal = new Vector2(0f, -ChanceController.Instance.GoalPosition.y);
            Vector2 myPos = transform.position;
            
            // Kaleye yaklaş ama çok girme
            float targetY = Mathf.Lerp(myPos.y, ourGoal.y, 0.3f);
            targetY = Mathf.Max(targetY, ourGoal.y + 3f);
            
            targetPosition = new Vector2(myPos.x + Random.Range(-2f, 2f), targetY);
            isMoving = true;
        }

        /// <summary>
        /// Takım gücünü ayarla
        /// </summary>
        public void SetTeamPower(int power)
        {
            teamPower = power;
            overall = Mathf.Clamp(power + Random.Range(-10, 10), 40, 95);
        }

        /// <summary>
        /// Topu al (atak başlat)
        /// </summary>
        public void ReceiveBall()
        {
            hasBall = true;
            isActive = true;
            isAttacking = true;
            decisionTimer = 0f;
            firstDecisionMade = false;  // Yeni karar döngüsü başlat
            isPreparingAction = false;

            // Slow-motion'dan çık
            SlowMotionManager.Instance?.DisableSlowMotion();

            Debug.Log($"[Opponent] {gameObject.name} received ball");
        }
        
        /// <summary>
        /// Top ile başlat (ilk rakip)
        /// </summary>
        public void StartWithBall()
        {
            hasBall = true;
            isActive = true;
            isAttacking = true;
            decisionTimer = 0f;
            firstDecisionMade = false;

            Debug.Log($"[Opponent] {gameObject.name} started with ball");
        }

        /// <summary>
        /// Atak kararı ver
        /// </summary>
        private void MakeAttackDecision()
        {
            if (ChanceController.Instance == null) return;

            // Kaleye uzaklık (bizim kalemiz - y negatif tarafta)
            Vector2 ourGoal = new Vector2(0f, -ChanceController.Instance.GoalPosition.y);
            float distToGoal = Vector2.Distance(transform.position, ourGoal);
            
            // Oyuncuya uzaklık (tehdit seviyesi)
            float distToPlayer = GetDistanceToPlayer();
            bool playerClose = distToPlayer < 3f;
            bool playerVeryClose = distToPlayer < 1.5f;

            float skillFactor = overall / 100f;

            Debug.Log($"[Opponent] Decision - DistToGoal: {distToGoal:F1}, DistToPlayer: {distToPlayer:F1}");

            // Oyuncu çok yakınsa hemen karar ver
            if (playerVeryClose)
            {
                // Panik! Hızlı şut veya pas
                if (distToGoal <= shootRange * 1.2f && Random.value < 0.6f)
                {
                    StartCoroutine(ShootSequence(ourGoal, true));  // Hızlı şut
                }
                else
                {
                    AttemptPass();
                }
                return;
            }

            // Şut mesafesindeyse ve oyuncu uzaksa
            if (distToGoal <= shootRange && !playerClose)
            {
                float shootChance = 0.5f + skillFactor * 0.3f;

                if (Random.value < shootChance)
                {
                    StartCoroutine(ShootSequence(ourGoal, false));
                    return;
                }
            }

            // Oyuncu yakınsa pas düşün
            if (playerClose)
            {
                float passChance = 0.4f + (passingStat / 100f) * 0.3f;
                if (Random.value < passChance)
                {
                    AttemptPass();
                    return;
                }
            }

            // Dribling yap - kaleye ilerle
            MoveTowardsGoal(ourGoal);
        }
        
        /// <summary>
        /// Oyuncuya mesafe
        /// </summary>
        private float GetDistanceToPlayer()
        {
            if (ChanceController.Instance?.Player == null) return float.MaxValue;
            return Vector2.Distance(transform.position, ChanceController.Instance.Player.transform.position);
        }

        /// <summary>
        /// Savunma kararı ver
        /// </summary>
        private void MakeDefenseDecision()
        {
            if (ChanceController.Instance == null) return;

            // Topa veya top sahibine yaklaş
            BallController ball = ChanceController.Instance.Ball;
            if (ball == null) return;

            Vector2 ballPos = ball.transform.position;
            float distToBall = Vector2.Distance(transform.position, ballPos);

            // Topa yakınsa savunma pozisyonu al
            if (distToBall < 5f)
            {
                // Pas çizgisini kes veya baskı yap
                Vector2 interceptPos = ballPos + (Vector2)(transform.position - ball.transform.position).normalized * 2f;
                targetPosition = interceptPos;
                isMoving = true;
            }
            else
            {
                // Savunma bloğu oluştur
                Vector2 goalPos = ChanceController.Instance.GoalPosition;
                Vector2 defenseLine = (ballPos + goalPos) / 2f;
                targetPosition = defenseLine + new Vector2(Random.Range(-2f, 2f), 0f);
                isMoving = true;
            }
        }

        /// <summary>
        /// Şut sekansı (coroutine)
        /// </summary>
        private IEnumerator ShootSequence(Vector2 goalPos, bool isQuickShot)
        {
            isPreparingAction = true;
            rb.linearVelocity = Vector2.zero;
            isMoving = false;

            Debug.Log($"[Opponent] {gameObject.name} preparing to shoot...");

            // Hızlı değilse hazırlık süresi
            if (!isQuickShot)
            {
                yield return new WaitForSeconds(0.4f);
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
            }

            // Top hala bizdeyse şut at
            if (!hasBall || ChanceController.Instance?.Ball == null)
            {
                isPreparingAction = false;
                yield break;
            }

            Debug.Log($"[Opponent] {gameObject.name} SHOOTING!");

            // Hedef: kale içinde rastgele nokta
            Vector2 targetPos = goalPos + new Vector2(Random.Range(-2.5f, 2.5f), 0f);

            float power = 20f + (shootingStat / 100f) * 15f;
            float accuracy = 0.3f + (shootingStat / 100f) * 0.35f;
            float curve = Random.Range(-0.4f, 0.4f);

            ChanceController.Instance.Ball.Shoot(
                targetPos, 
                power, 
                curve, 
                accuracy, 
                isOpponent: true, 
                isPlayer: false
            );

            hasBall = false;
            isPreparingAction = false;
            
            // Kamera topu takip etsin
            ChanceController.Instance.BallInFlight();
        }

        /// <summary>
        /// Pas dene
        /// </summary>
        private void AttemptPass()
        {
            if (ChanceController.Instance == null) return;
            
            // En iyi pas hedefini bul
            GameObject target = FindBestPassTarget();
            
            if (target == null)
            {
                // Pas atılacak kimse yok - dribling yap
                Debug.Log("[Opponent] No pass target, dribbling instead");
                Vector2 ourGoal = new Vector2(0f, -ChanceController.Instance.GoalPosition.y);
                MoveTowardsGoal(ourGoal);
                return;
            }

            Debug.Log($"[Opponent] {gameObject.name} passing to {target.name}");

            // Pas at
            if (ChanceController.Instance.Ball != null)
            {
                float passSpeed = 12f + (passingStat / 100f) * 8f;
                float successChance = 0.5f + (passingStat / 100f) * 0.35f;

                ChanceController.Instance.Ball.Pass(
                    target,
                    passSpeed,
                    Random.Range(-0.2f, 0.2f),
                    successChance
                );

                hasBall = false;
                
                // Kamera topu takip etsin
                ChanceController.Instance.BallInFlight();
            }
        }
        
        /// <summary>
        /// En iyi pas hedefini bul
        /// </summary>
        private GameObject FindBestPassTarget()
        {
            if (ChanceController.Instance == null) return null;

            GameObject bestTarget = null;
            float bestScore = float.MinValue;

            foreach (var opponent in ChanceController.Instance.Opponents)
            {
                if (opponent == null || opponent == gameObject) continue;

                var ai = opponent.GetComponent<OpponentController>();
                if (ai == null || ai.HasBall) continue;

                // Skor hesapla: kaleye yakınlık + oyuncudan uzaklık
                Vector2 ourGoal = new Vector2(0f, -ChanceController.Instance.GoalPosition.y);
                float distToGoal = Vector2.Distance(opponent.transform.position, ourGoal);
                float distFromPlayer = 0f;
                
                if (ChanceController.Instance.Player != null)
                {
                    distFromPlayer = Vector2.Distance(opponent.transform.position, 
                        ChanceController.Instance.Player.transform.position);
                }

                // Kaleye yakın + oyuncudan uzak = iyi hedef
                float score = -distToGoal * 0.4f + distFromPlayer * 0.3f;
                
                // Önümde mi?
                Vector2 toTarget = (Vector2)opponent.transform.position - (Vector2)transform.position;
                Vector2 toGoal = ourGoal - (Vector2)transform.position;
                if (Vector2.Dot(toTarget.normalized, toGoal.normalized) > 0)
                {
                    score += 2f;  // İleri pas bonus
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = opponent;
                }
            }

            return bestTarget;
        }

        /// <summary>
        /// Kaleye doğru ilerle
        /// </summary>
        private void MoveTowardsGoal(Vector2 goalPos)
        {
            Vector2 direction = (goalPos - (Vector2)transform.position).normalized;
            direction = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * direction;

            targetPosition = (Vector2)transform.position + direction * 2.5f;
            isMoving = true;
        }

        /// <summary>
        /// Hedefe hareket et
        /// </summary>
        private void MoveToTarget()
        {
            if (rb == null) return;

            Vector2 direction = (targetPosition - (Vector2)transform.position);

            if (direction.magnitude < 0.3f)
            {
                isMoving = false;
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = direction.normalized * moveSpeed;
        }

        /// <summary>
        /// Topu kaybet
        /// </summary>
        public void LoseBall()
        {
            hasBall = false;
            
            // Top kaybedildi - uzaklaştır
            if (isAttacking)
            {
                Debug.Log("[Opponent] Ball lost - clearing");
                ChanceController.Instance?.EndChance(ChanceOutcome.Cleared);
            }
        }

        /// <summary>
        /// AI'ı aktifleştir
        /// </summary>
        public void Activate()
        {
            isActive = true;
        }

        /// <summary>
        /// AI'ı deaktive et
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
            isMoving = false;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}

