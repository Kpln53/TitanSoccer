using UnityEngine;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Oyuncu Kontrolcüsü
    /// Dokunarak hareket, çizgi ile pas/şut
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Hareket Ayarları")]
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 15f;

        [Header("Stat Etkileri")]
        [SerializeField] private float speedStatMultiplier = 0.02f;  // Her stat puanı için hız artışı

        [Header("Kayarak Müdahale")]
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private float slideSpeed = 12f;
        [SerializeField] private float slideRecoveryTime = 0.8f;
        [SerializeField] private float slideTackleRadius = 1.2f;

        [Header("Saha Sınırları")]
        [SerializeField] private Vector2 fieldMin = new Vector2(-12f, -10f);
        [SerializeField] private Vector2 fieldMax = new Vector2(12f, 10f);

        [Header("Durum")]
        [SerializeField] private bool isSliding = false;
        [SerializeField] private bool isRecovering = false;
        [SerializeField] private bool hasBall = false;

        // Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        // Stats
        private ChanceSetupData stats;
        private float currentSpeed;

        // Slide
        private Vector2 slideDirection;
        private float slideTimer;
        private float recoveryTimer;
        private float baseRecoveryTime;

        // Events
        public event Action OnBallLost;
        public event Action OnBallGained;
        public event Action<bool> OnSlideTackleResult;

        // Properties
        public bool HasBall => hasBall;
        public bool IsSliding => isSliding;
        public bool CanAct => !isSliding && !isRecovering;
        public ChanceSetupData Stats => stats;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.linearDamping = 5f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            baseRecoveryTime = slideRecoveryTime;
        }

        private void Start()
        {
            // Input event'lerini dinle
            if (TouchMoveController.Instance != null)
            {
                TouchMoveController.Instance.OnMoveStarted += OnMoveStarted;
                TouchMoveController.Instance.OnMoveCompleted += OnMoveCompleted;
            }

            if (LineDrawer.Instance != null)
            {
                LineDrawer.Instance.OnLineCompleted += OnLineDrawn;
            }

            if (SlideTackleGesture.Instance != null)
            {
                SlideTackleGesture.Instance.OnSlideTackle += OnSlideTackleInput;
            }

            CalculateSpeed();
        }

        private void Update()
        {
            if (isSliding)
            {
                UpdateSlide();
            }
            else if (isRecovering)
            {
                UpdateRecovery();
            }
        }

        private void FixedUpdate()
        {
            if (!isSliding && !isRecovering)
            {
                UpdateTouchMovement();
            }

            ClampToField();
        }

        /// <summary>
        /// Stats'ları ayarla
        /// </summary>
        public void SetupStats(ChanceSetupData setupData)
        {
            stats = setupData;
            CalculateSpeed();
        }

        /// <summary>
        /// Hızı stat'lara göre hesapla
        /// </summary>
        private void CalculateSpeed()
        {
            if (stats != null)
            {
                currentSpeed = baseSpeed + (stats.speedStat * speedStatMultiplier);
                
                // Enerji etkisi
                float energyMultiplier = 0.7f + (stats.playerEnergy / 100f) * 0.3f;
                currentSpeed *= energyMultiplier;
            }
            else
            {
                currentSpeed = baseSpeed;
            }
        }

        /// <summary>
        /// Dokunarak hareket başladı
        /// </summary>
        private void OnMoveStarted(Vector2 target)
        {
            // Hareket başladığında bir şey yapmaya gerek yok
            // FixedUpdate'te hareket işlenecek
        }

        /// <summary>
        /// Dokunarak hareket tamamlandı
        /// </summary>
        private void OnMoveCompleted()
        {
            // Hareket tamamlandığında dur
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Dokunarak hareket güncelle
        /// </summary>
        private void UpdateTouchMovement()
        {
            if (TouchMoveController.Instance == null) return;
            if (!TouchMoveController.Instance.IsMoving)
            {
                // Hareket yok - yavaşla
                if (rb != null && rb.linearVelocity.magnitude > 0.1f)
                {
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.fixedDeltaTime * deceleration);
                }
                return;
            }

            // Hedefe doğru hareket
            Vector2 direction = TouchMoveController.Instance.GetMoveDirection();
            float distance = TouchMoveController.Instance.GetDistanceToTarget();

            if (direction != Vector2.zero && rb != null)
            {
                // Hedefe yaklaşınca yavaşla
                float speedMultiplier = Mathf.Clamp01(distance / 2f);
                speedMultiplier = Mathf.Max(speedMultiplier, 0.3f); // Minimum hız

                Vector2 targetVelocity = direction * currentSpeed * speedMultiplier;
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
            }
        }

        /// <summary>
        /// Çizgi çizildi - eylem yap
        /// </summary>
        private void OnLineDrawn(LineData lineData)
        {
            Debug.Log($"[Player] OnLineDrawn: Action={lineData.detectedAction}, CanAct={CanAct}, hasBall={hasBall}");
            
            if (!CanAct)
            {
                Debug.LogWarning($"[Player] Cannot act! isSliding={isSliding}, isRecovering={isRecovering}");
                return;
            }

            switch (lineData.detectedAction)
            {
                case ActionType.Pass:
                    ExecutePass(lineData);
                    break;

                case ActionType.Shoot:
                    ExecuteShoot(lineData);
                    break;

                case ActionType.Dribble:
                    // Dribble için dokunarak hareket kullan
                    // Çizginin bitiş noktasına git
                    if (TouchMoveController.Instance != null)
                    {
                        Debug.Log($"[Player] Dribbling to {lineData.endPoint}");
                    }
                    break;
                    
                default:
                    Debug.Log($"[Player] Unknown action type: {lineData.detectedAction}");
                    break;
            }
        }

        /// <summary>
        /// Pas at
        /// </summary>
        private void ExecutePass(LineData lineData)
        {
            if (!hasBall)
            {
                Debug.LogWarning("[Player] ExecutePass failed: No ball!");
                return;
            }
            if (lineData.targetPlayer == null)
            {
                Debug.LogWarning("[Player] ExecutePass failed: No target player!");
                return;
            }

            Debug.Log($"[Player] Passing to {lineData.targetPlayer.name}");

            // Top kontrolcüsüne pas emri ver
            if (ChanceController.Instance?.Ball != null)
            {
                float passSpeed = CalculatePassSpeed(lineData);
                float passCurve = CalculateCurve(lineData);
                float successChance = CalculatePassSuccess(lineData);

                ChanceController.Instance.Ball.Pass(
                    lineData.targetPlayer,
                    passSpeed,
                    passCurve,
                    successChance
                );

                hasBall = false;
                OnBallLost?.Invoke();
                
                // Asist takibi için
                ChanceController.Instance.PlayerMadePass();

                // Kamera topu takip etsin
                ChanceController.Instance.BallInFlight();
            }
        }

        /// <summary>
        /// Şut at
        /// </summary>
        private void ExecuteShoot(LineData lineData)
        {
            if (!hasBall)
            {
                Debug.LogWarning("[Player] ExecuteShoot failed: No ball!");
                return;
            }

            Debug.Log("[Player] Shooting!");

            if (ChanceController.Instance?.Ball != null)
            {
                float shotPower = CalculateShotPower(lineData);
                float shotCurve = CalculateCurve(lineData);
                Vector2 targetPos = lineData.endPoint;
                float accuracy = CalculateShotAccuracy(lineData);

                ChanceController.Instance.Ball.Shoot(
                    targetPos,
                    shotPower,
                    shotCurve,
                    accuracy
                );

                hasBall = false;
                OnBallLost?.Invoke();

                ChanceController.Instance.BallInFlight();
            }
        }

        /// <summary>
        /// Kayarak müdahale input'u
        /// </summary>
        private void OnSlideTackleInput(Vector2 direction)
        {
            if (!CanAct) return;

            StartSlide(direction);
        }

        /// <summary>
        /// Kayarak müdahale başlat
        /// </summary>
        public void StartSlide(Vector2 direction)
        {
            if (isSliding || isRecovering) return;

            isSliding = true;
            slideDirection = direction.normalized;
            slideTimer = 0f;

            // Hareket iptal
            TouchMoveController.Instance?.CancelMove();

            // Görsel feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.cyan;
            }

            Debug.Log($"[Player] Slide tackle started: {direction}");
        }

        /// <summary>
        /// Kayma güncelle
        /// </summary>
        private void UpdateSlide()
        {
            slideTimer += Time.deltaTime;

            if (slideTimer < slideDuration)
            {
                // Kayma hareketi
                float slideMultiplier = 1f;
                
                // Defans stat'ına göre kayma uzunluğu
                if (stats != null)
                {
                    slideMultiplier = 0.8f + (stats.defenseStat / 100f) * 0.4f;
                }

                rb.linearVelocity = slideDirection * slideSpeed * slideMultiplier;

                // Top kapma kontrolü
                CheckTackle();
            }
            else
            {
                // Kayma bitti - toparlanma başla
                EndSlide();
            }
        }

        /// <summary>
        /// Kayma sırasında top kapma kontrolü
        /// </summary>
        private void CheckTackle()
        {
            if (ChanceController.Instance?.Ball == null) return;

            BallController ball = ChanceController.Instance.Ball;
            float distToBall = Vector2.Distance(transform.position, ball.transform.position);
            
            // Rakip oyuncu mesafesi
            float distToOpponent = GetDistanceToNearestOpponent();

            // Önce rakibe mi çarpıyoruz kontrolü (faul riski)
            if (distToOpponent < slideTackleRadius * 0.8f && distToBall > slideTackleRadius * 0.5f)
            {
                // Rakibe çarptık ama top uzak - FAUL!
                CheckFoul();
                return;
            }

            if (distToBall <= slideTackleRadius)
            {
                // Top yakın - kapma şansı hesapla
                float tackleChance = CalculateTackleSuccess();
                
                // Rakip de yakınsa faul riski
                if (distToOpponent < slideTackleRadius * 1.2f)
                {
                    // Hem topa hem rakibe yakın - %30 faul riski
                    if (UnityEngine.Random.value < 0.3f)
                    {
                        CheckFoul();
                        return;
                    }
                }

                if (UnityEngine.Random.value < tackleChance)
                {
                    // Başarılı müdahale
                    Debug.Log("[Player] Tackle successful!");
                    ball.AttachToPlayer(gameObject);
                    hasBall = true;
                    OnBallGained?.Invoke();
                    OnSlideTackleResult?.Invoke(true);

                    // Rakip topu kaybetti
                    NotifyOpponentBallLost();

                    // Pozisyon bitti - clear
                    ChanceController.Instance?.EndChance(ChanceOutcome.Tackled);
                }
                else
                {
                    Debug.Log("[Player] Tackle missed!");
                    OnSlideTackleResult?.Invoke(false);
                }
            }
        }
        
        /// <summary>
        /// En yakın rakibe mesafe
        /// </summary>
        private float GetDistanceToNearestOpponent()
        {
            if (ChanceController.Instance == null) return float.MaxValue;
            
            float minDist = float.MaxValue;
            foreach (var opponent in ChanceController.Instance.Opponents)
            {
                if (opponent == null) continue;
                float dist = Vector2.Distance(transform.position, opponent.transform.position);
                if (dist < minDist) minDist = dist;
            }
            return minDist;
        }
        
        /// <summary>
        /// Rakibe topu kaybettiğini bildir
        /// </summary>
        private void NotifyOpponentBallLost()
        {
            foreach (var opponent in ChanceController.Instance.Opponents)
            {
                if (opponent == null) continue;
                var ai = opponent.GetComponent<OpponentController>();
                if (ai != null && ai.HasBall)
                {
                    ai.LoseBall();
                    break;
                }
            }
        }
        
        /// <summary>
        /// Faul kontrolü
        /// </summary>
        private void CheckFoul()
        {
            Debug.Log("[Player] FOUL! Bad tackle timing!");
            
            // Faul sonucu
            bool isYellowCard = UnityEngine.Random.value < 0.25f;  // %25 sarı kart
            bool isRedCard = UnityEngine.Random.value < 0.05f;     // %5 kırmızı kart
            
            if (isRedCard)
            {
                Debug.Log("[Player] RED CARD!");
                MatchContext.Instance?.AddCommentary("🟥 KIRMIZI KART! Tehlikeli müdahale!");
                // Kırmızı kart - oyuncu atıldı, maç bitti gibi davran
                ChanceController.Instance?.EndChance(ChanceOutcome.Cleared);
            }
            else if (isYellowCard)
            {
                Debug.Log("[Player] YELLOW CARD!");
                MatchContext.Instance?.AddCommentary("🟨 SARI KART! Sert müdahale.");
                // Sarı kart - pozisyon devam (serbest vuruş rakipte)
                ChanceController.Instance?.EndChance(ChanceOutcome.Cleared);
            }
            else
            {
                Debug.Log("[Player] Foul - Free kick for opponent");
                MatchContext.Instance?.AddCommentary("Faul! Serbest vuruş.");
                // Basit faul - pozisyon biter
                ChanceController.Instance?.EndChance(ChanceOutcome.Cleared);
            }
            
            // Rating düşür
            if (MatchContext.Instance != null)
            {
                MatchContext.Instance.playerMatchRating = Mathf.Max(0f, 
                    MatchContext.Instance.playerMatchRating - (isYellowCard ? 0.5f : 0.2f));
            }
            
            OnSlideTackleResult?.Invoke(false);
        }

        /// <summary>
        /// Kayma bitti
        /// </summary>
        private void EndSlide()
        {
            isSliding = false;
            isRecovering = true;
            recoveryTimer = 0f;
            rb.linearVelocity = Vector2.zero;

            // Recovery süresi defans stat'ına göre
            slideRecoveryTime = baseRecoveryTime;
            if (stats != null)
            {
                float recoveryMultiplier = 1.2f - (stats.defenseStat / 100f) * 0.4f;
                slideRecoveryTime *= recoveryMultiplier;
            }
        }

        /// <summary>
        /// Toparlanma güncelle
        /// </summary>
        private void UpdateRecovery()
        {
            recoveryTimer += Time.deltaTime;

            if (recoveryTimer >= slideRecoveryTime)
            {
                isRecovering = false;

                // Görsel normal
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.yellow;
                }
            }
        }

        /// <summary>
        /// Sahada tut
        /// </summary>
        private void ClampToField()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, fieldMin.x, fieldMax.x);
            pos.y = Mathf.Clamp(pos.y, fieldMin.y, fieldMax.y);
            transform.position = pos;
        }

        // === HESAPLAMA METODLARİ ===

        private float CalculatePassSpeed(LineData lineData)
        {
            float basePassSpeed = 8f;
            float lengthBonus = Mathf.Clamp01(lineData.length / 10f) * 4f;
            float statBonus = stats != null ? (stats.passingStat / 100f) * 3f : 0f;

            return basePassSpeed + lengthBonus + statBonus;
        }

        private float CalculatePassSuccess(LineData lineData)
        {
            float baseSuccess = 0.7f;
            float statBonus = stats != null ? (stats.passingStat / 100f) * 0.25f : 0f;
            float distancePenalty = Mathf.Clamp01(lineData.length / 15f) * 0.2f;

            return Mathf.Clamp01(baseSuccess + statBonus - distancePenalty);
        }

        private float CalculateShotPower(LineData lineData)
        {
            // Şut hızı artırıldı - daha hızlı şutlar için
            float basePower = 25f;  // 10 → 25
            float lengthBonus = Mathf.Clamp01(lineData.length / 6f) * 15f;  // Max 15 bonus
            float speedBonus = Mathf.Clamp01(lineData.drawSpeed / 5f) * 10f; // Hızlı çizim = güçlü şut
            float statBonus = stats != null ? (stats.shootingStat / 100f) * 10f : 0f;

            float totalPower = basePower + lengthBonus + speedBonus + statBonus;
            Debug.Log($"[Player] Shot power: {totalPower} (base:{basePower}, length:{lengthBonus:F1}, speed:{speedBonus:F1}, stat:{statBonus:F1})");
            return totalPower;
        }

        private float CalculateShotAccuracy(LineData lineData)
        {
            float baseAccuracy = 0.5f;
            float statBonus = stats != null ? (stats.shootingStat / 100f) * 0.3f : 0f;
            float speedPenalty = Mathf.Clamp01(lineData.drawSpeed / 10f) * 0.1f;

            return Mathf.Clamp01(baseAccuracy + statBonus - speedPenalty);
        }

        private float CalculateCurve(LineData lineData)
        {
            float baseCurve = lineData.curvature;
            
            if (stats != null)
            {
                float falsoMultiplier = 0.5f + (stats.falsoStat / 100f) * 1f;
                baseCurve *= falsoMultiplier;
            }

            return Mathf.Clamp(baseCurve, -1f, 1f);
        }

        private float CalculateTackleSuccess()
        {
            float baseChance = 0.5f;
            
            if (stats != null)
            {
                float statBonus = (stats.defenseStat / 100f) * 0.35f;
                float physicsBonus = (stats.physicsStat / 100f) * 0.15f;
                return Mathf.Clamp01(baseChance + statBonus + physicsBonus);
            }

            return baseChance;
        }

        // === PUBLIC METHODS ===

        /// <summary>
        /// Topu al
        /// </summary>
        public void GainBall()
        {
            hasBall = true;
            OnBallGained?.Invoke();
            Debug.Log($"[Player] GainBall called - hasBall is now TRUE");
        }

        /// <summary>
        /// Topu kaybet
        /// </summary>
        public void LoseBall()
        {
            hasBall = false;
            OnBallLost?.Invoke();
            Debug.Log($"[Player] LoseBall called - hasBall is now FALSE");
        }

        private void OnDestroy()
        {
            // Event'lerden çık
            if (TouchMoveController.Instance != null)
            {
                TouchMoveController.Instance.OnMoveStarted -= OnMoveStarted;
                TouchMoveController.Instance.OnMoveCompleted -= OnMoveCompleted;
            }

            if (LineDrawer.Instance != null)
            {
                LineDrawer.Instance.OnLineCompleted -= OnLineDrawn;
            }

            if (SlideTackleGesture.Instance != null)
            {
                SlideTackleGesture.Instance.OnSlideTackle -= OnSlideTackleInput;
            }
        }
    }
}
