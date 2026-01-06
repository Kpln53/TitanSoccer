using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Rakip Kontrolcüsü
    /// Savunma veya atak yapan rakip AI
    /// </summary>
    public class OpponentController : MonoBehaviour
    {
        [Header("AI Ayarları")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float decisionInterval = 0.4f;
        [SerializeField] private float shootRange = 14f;
        [SerializeField] private float passRange = 10f;

        [Header("Stats")]
        [SerializeField] private int teamPower = 65;
        [SerializeField] private int overall = 65;

        [Header("Durum")]
        [SerializeField] private bool hasBall = false;
        [SerializeField] private bool isActive = false;
        [SerializeField] private bool isAttacking = false;

        private Rigidbody2D rb;
        private float decisionTimer;
        private Vector2 targetPosition;
        private bool isMoving = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Savunma pozisyonunda mı atak pozisyonunda mı?
            if (ChanceController.Instance != null)
            {
                isAttacking = ChanceController.Instance.CurrentChanceType == ChanceType.Defense;
            }
        }

        private void Update()
        {
            if (!isActive) return;

            decisionTimer += Time.deltaTime;

            if (decisionTimer >= decisionInterval)
            {
                if (hasBall)
                {
                    MakeAttackDecision();
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
            if (isMoving)
            {
                MoveToTarget();
            }
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

            Debug.Log($"[Opponent] {gameObject.name} received ball");
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

            float random = Random.value;
            float skillFactor = overall / 100f;

            // Şut mesafesindeyse
            if (distToGoal <= shootRange)
            {
                float shootChance = skillFactor * 0.5f;

                if (random < shootChance)
                {
                    AttemptShoot(ourGoal);
                    return;
                }
            }

            // Pas mı hareket mi?
            float passChance = skillFactor * 0.3f;

            if (random < passChance + 0.2f)
            {
                AttemptPass();
            }
            else
            {
                MoveTowardsGoal(ourGoal);
            }
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
        /// Şut dene
        /// </summary>
        private void AttemptShoot(Vector2 goalPos)
        {
            if (ChanceController.Instance?.Ball == null) return;

            Debug.Log($"[Opponent] {gameObject.name} shooting!");

            // Hedef: kale içinde rastgele nokta
            Vector2 targetPos = goalPos + new Vector2(Random.Range(-2f, 2f), 0f);

            float power = 8f + (overall / 100f) * 6f;
            float accuracy = 0.35f + (overall / 100f) * 0.3f;
            float curve = Random.Range(-0.3f, 0.3f);

            ChanceController.Instance.Ball.Shoot(targetPos, power, curve, accuracy, isOpponent: true);

            hasBall = false;
            isActive = false;
        }

        /// <summary>
        /// Pas dene
        /// </summary>
        private void AttemptPass()
        {
            // Basit pas - rastgele takım arkadaşına
            // MVP'de basit tutuyoruz

            Debug.Log($"[Opponent] {gameObject.name} passing...");

            // Top kaybı simüle et (%30 şans)
            if (Random.value < 0.3f)
            {
                Debug.Log("[Opponent] Pass intercepted! (simulated)");
                
                // Top kayboldu - uzaklaştır ve pozisyonu bitir
                hasBall = false;
                isActive = false;
                ChanceController.Instance?.EndChance(ChanceOutcome.Cleared);
                return;
            }

            // Başarılı pas - kaleye doğru ilerle
            MoveTowardsGoal(new Vector2(0f, -ChanceController.Instance.GoalPosition.y));
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

