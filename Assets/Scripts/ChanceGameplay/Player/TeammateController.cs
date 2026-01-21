using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Takım Arkadaşı Kontrolcüsü
    /// Pas aldığında AI ile oynamaya devam eder
    /// </summary>
    public class TeammateController : MonoBehaviour
    {
        [Header("AI Ayarları")]
        [SerializeField] private float decisionInterval = 0.5f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float passRange = 10f;
        [SerializeField] private float shootRange = 12f;

        [Header("Stats")]
        [SerializeField] private int overall = 65;
        [SerializeField] private int shootingStat = 60;
        [SerializeField] private int passingStat = 65;

        [Header("Durum")]
        [SerializeField] private bool hasBall = false;
        [SerializeField] private bool isActive = false;

        private Rigidbody2D rb;
        private float decisionTimer;
        private Vector2 targetPosition;
        private bool isMoving = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!isActive || !hasBall) return;

            decisionTimer += Time.deltaTime;

            if (decisionTimer >= decisionInterval)
            {
                MakeDecision();
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
        /// Topu al ve AI'ı aktifleştir
        /// </summary>
        public void ReceiveBall()
        {
            hasBall = true;
            isActive = true;
            decisionTimer = 0f;

            Debug.Log($"[Teammate] {gameObject.name} received ball");

            // Kısa gecikme sonrası karar vermeye başla
            Invoke(nameof(StartDecisionMaking), 0.3f);
        }

        private void StartDecisionMaking()
        {
            MakeDecision();
        }

        /// <summary>
        /// AI karar ver
        /// </summary>
        private void MakeDecision()
        {
            if (!hasBall || ChanceController.Instance == null) return;

            Vector2 goalPos = ChanceController.Instance.GoalPosition;
            float distToGoal = Vector2.Distance(transform.position, goalPos);

            // Karar ağacı
            float random = Random.value;

            // Şut mesafesindeyse
            if (distToGoal <= shootRange)
            {
                float shootChance = (shootingStat / 100f) * 0.6f;
                
                if (random < shootChance)
                {
                    // Şut at
                    AttemptShoot();
                    return;
                }
            }

            // Pas mı hareket mi?
            float passChance = (passingStat / 100f) * 0.4f;

            if (random < passChance + 0.3f) // %30 + stat bonusu
            {
                // Pas at
                AttemptPass();
            }
            else
            {
                // İlerle
                MoveForward();
            }
        }

        /// <summary>
        /// Şut dene
        /// </summary>
        private void AttemptShoot()
        {
            if (ChanceController.Instance?.Ball == null) return;

            Debug.Log($"[Teammate] {gameObject.name} shooting!");

            Vector2 goalPos = ChanceController.Instance.GoalPosition;
            
            // Hedef: kale içinde rastgele nokta
            Vector2 targetPos = goalPos + new Vector2(Random.Range(-2f, 2f), 0f);
            
            float power = 10f + (shootingStat / 100f) * 5f;
            float accuracy = 0.4f + (shootingStat / 100f) * 0.3f;
            float curve = Random.Range(-0.3f, 0.3f);

            ChanceController.Instance.Ball.Shoot(targetPos, power, curve, accuracy);

            hasBall = false;
            isActive = false;
        }

        /// <summary>
        /// Pas dene
        /// </summary>
        private void AttemptPass()
        {
            if (ChanceController.Instance == null) return;

            // En yakın takım arkadaşını bul
            GameObject bestTarget = null;
            float bestScore = float.MinValue;

            foreach (var teammate in ChanceController.Instance.Teammates)
            {
                if (teammate == null || teammate == gameObject) continue;

                float dist = Vector2.Distance(transform.position, teammate.transform.position);
                if (dist > passRange) continue;

                // Kaleye yakınlık bonusu
                float distToGoal = Vector2.Distance(teammate.transform.position, 
                    ChanceController.Instance.GoalPosition);
                float score = -distToGoal + Random.Range(0f, 3f); // Biraz rastgelelik

                if (score > bestScore)
                {
                    bestScore = score;
                    bestTarget = teammate;
                }
            }

            // Oyuncuya da pas atılabilir
            if (ChanceController.Instance.Player != null)
            {
                float distToPlayer = Vector2.Distance(transform.position, 
                    ChanceController.Instance.Player.transform.position);
                
                if (distToPlayer <= passRange)
                {
                    float playerScore = Random.Range(0f, 5f); // Oyuncuya pas önceliği
                    if (playerScore > bestScore)
                    {
                        bestTarget = ChanceController.Instance.Player.gameObject;
                    }
                }
            }

            if (bestTarget != null && ChanceController.Instance.Ball != null)
            {
                Debug.Log($"[Teammate] {gameObject.name} passing to {bestTarget.name}");

                float passSpeed = 6f + (passingStat / 100f) * 4f;
                float successChance = 0.6f + (passingStat / 100f) * 0.25f;

                ChanceController.Instance.Ball.Pass(bestTarget, passSpeed, 0f, successChance);

                hasBall = false;
                isActive = false;
            }
            else
            {
                // Pas hedefi yok - ilerle
                MoveForward();
            }
        }

        /// <summary>
        /// Kaleye doğru ilerle
        /// </summary>
        private void MoveForward()
        {
            if (ChanceController.Instance == null) return;

            Vector2 goalPos = ChanceController.Instance.GoalPosition;
            Vector2 direction = (goalPos - (Vector2)transform.position).normalized;

            // Biraz rastgele sapma
            direction = Quaternion.Euler(0, 0, Random.Range(-15f, 15f)) * direction;

            targetPosition = (Vector2)transform.position + direction * 3f;
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
            isActive = false;
            isMoving = false;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Stats ayarla
        /// </summary>
        public void SetStats(int ovr, int shooting, int passing)
        {
            overall = ovr;
            shootingStat = shooting;
            passingStat = passing;
        }
    }
}



