using UnityEngine;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Top Kontrolcüsü - Yeni Sistem
    /// Pas, şut ve fizik yönetimi
    /// </summary>
    public class BallController : MonoBehaviour
    {
        [Header("Fizik Ayarları")]
        [SerializeField] private float attachOffset = 0.4f;
        [SerializeField] private float arriveDistance = 0.3f;
        [SerializeField] private float curveStrength = 3f;
        [SerializeField] private float friction = 0.98f;
        [SerializeField] private float pickupRadius = 0.8f;  // Serbest topu alma mesafesi

        [Header("Durum")]
        [SerializeField] private BallState currentState = BallState.Free;
        [SerializeField] private GameObject attachedPlayer;

        // Uçuş verileri
        private Vector2 flightTarget;
        private float flightSpeed;
        private float flightCurve;
        private GameObject flightReceiver;
        private bool isOpponentShot = false;
        private bool isPlayerShot = false;  // Oyuncu mu şut attı?

        // Şut verileri
        private Vector2 shotTarget;
        private float shotAccuracy;

        // Components
        private Rigidbody2D rb;
        private TrailRenderer trail;

        // Events
        public event Action<GameObject> OnPassReceived;
        public event Action<ChanceOutcome> OnShotResult;
        public event Action OnBallLost;

        // Properties
        public BallState CurrentState => currentState;
        public GameObject AttachedPlayer => attachedPlayer;

        public enum BallState
        {
            Free,           // Serbest
            Attached,       // Oyuncuya bağlı
            PassFlight,     // Pas uçuşunda
            ShotFlight      // Şut uçuşunda
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // Trail efekti
            trail = GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = gameObject.AddComponent<TrailRenderer>();
                trail.time = 0.2f;
                trail.startWidth = 0.15f;
                trail.endWidth = 0.05f;
                trail.material = new Material(Shader.Find("Sprites/Default"));
                trail.startColor = new Color(1f, 1f, 1f, 0.5f);
                trail.endColor = new Color(1f, 1f, 1f, 0f);
            }
        }

        private void Update()
        {
            switch (currentState)
            {
                case BallState.Attached:
                    UpdateAttached();
                    break;
                case BallState.Free:
                    UpdateFree();
                    break;
                case BallState.PassFlight:
                    UpdatePassFlight();
                    break;
                case BallState.ShotFlight:
                    UpdateShotFlight();
                    break;
            }
        }

        /// <summary>
        /// Serbest top durumu - oyuncular topa yaklaşınca alabilir
        /// </summary>
        private void UpdateFree()
        {
            if (ChanceController.Instance == null) return;

            // Oyuncu kontrolü
            var player = ChanceController.Instance.Player;
            if (player != null)
            {
                float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distToPlayer < pickupRadius)
                {
                    // Oyuncu topu aldı!
                    AttachToPlayer(player.gameObject);
                    ChanceController.Instance.ActionCompleted();
                    Debug.Log("[Ball] Player picked up free ball!");
                    return;
                }
            }

            // Takım arkadaşları kontrolü
            foreach (var teammate in ChanceController.Instance.Teammates)
            {
                if (teammate == null) continue;
                float dist = Vector2.Distance(transform.position, teammate.transform.position);
                if (dist < pickupRadius)
                {
                    AttachToPlayer(teammate);
                    Debug.Log($"[Ball] Teammate {teammate.name} picked up free ball!");
                    return;
                }
            }

            // Rakipler kontrolü
            foreach (var opponent in ChanceController.Instance.Opponents)
            {
                if (opponent == null) continue;
                float dist = Vector2.Distance(transform.position, opponent.transform.position);
                if (dist < pickupRadius)
                {
                    // Rakip topu aldı - pozisyon bitti
                    AttachToPlayer(opponent);
                    Debug.Log($"[Ball] Opponent {opponent.name} picked up free ball - Chance over!");
                    ChanceController.Instance.EndChance(ChanceOutcome.Intercepted);
                    return;
                }
            }
        }

        /// <summary>
        /// Oyuncuya bağlı durumu güncelle
        /// </summary>
        private void UpdateAttached()
        {
            if (attachedPlayer == null)
            {
                currentState = BallState.Free;
                return;
            }

            // Oyuncunun önünde pozisyon al
            Vector2 playerPos = attachedPlayer.transform.position;
            Vector2 offset = Vector2.up * attachOffset;

            // Oyuncu hareket ediyorsa yönüne göre offset
            Rigidbody2D playerRb = attachedPlayer.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.linearVelocity.magnitude > 0.1f)
            {
                offset = playerRb.linearVelocity.normalized * attachOffset;
            }

            transform.position = Vector2.Lerp(transform.position, playerPos + offset, Time.deltaTime * 15f);
        }

        /// <summary>
        /// Pas uçuşu güncelle
        /// </summary>
        private void UpdatePassFlight()
        {
            if (flightReceiver == null)
            {
                currentState = BallState.Free;
                OnBallLost?.Invoke();
                return;
            }

            // Hedef pozisyonu güncelle (oyuncu hareket edebilir)
            flightTarget = flightReceiver.transform.position;

            // Hedefe doğru hareket (eğri ile)
            Vector2 currentPos = transform.position;
            Vector2 direction = (flightTarget - currentPos).normalized;

            // Eğri uygula
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 curvedDirection = direction + perpendicular * flightCurve * 0.1f;

            // Hareket
            Vector2 newPos = currentPos + curvedDirection.normalized * flightSpeed * Time.deltaTime;
            transform.position = newPos;

            // Hedefe ulaştı mı?
            float distToTarget = Vector2.Distance(newPos, flightTarget);
            if (distToTarget <= arriveDistance)
            {
                // Pas tamamlandı
                PassCompleted();
            }

            // Eğri azalt (hedefe yaklaştıkça)
            flightCurve *= 0.98f;
        }

        /// <summary>
        /// Şut uçuşu güncelle
        /// </summary>
        private void UpdateShotFlight()
        {
            Vector2 currentPos = transform.position;
            Vector2 direction = (shotTarget - currentPos).normalized;

            // Eğri uygula
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 curvedDirection = direction + perpendicular * flightCurve * 0.1f;

            // Hareket - HIZLANDIRILDI (unscaledDeltaTime kullan, slow-motion etkilemesin)
            float actualSpeed = flightSpeed * Time.unscaledDeltaTime;
            Vector2 newPos = currentPos + curvedDirection.normalized * actualSpeed;
            transform.position = newPos;

            // Hedefe ulaştı mı?
            float distToTarget = Vector2.Distance(newPos, shotTarget);

            // Kaleci kontrolü
            if (ChanceController.Instance != null)
            {
                var keeper = FindObjectOfType<GoalkeeperAI>();
                if (keeper != null)
                {
                    float distToKeeper = Vector2.Distance(newPos, keeper.transform.position);
                    if (distToKeeper < 1f)
                    {
                        // Kaleci müdahale edebilir
                        bool saved = keeper.TrySave(newPos, shotAccuracy);
                        if (saved)
                        {
                            ShotSaved();
                            return;
                        }
                    }
                }
            }

            if (distToTarget <= arriveDistance * 2f)
            {
                // Hedefe ulaştı - gol veya kaçırma
                DetermineShotResult();
            }

            // Eğri azalt
            flightCurve *= 0.97f;

            // Hız azalt
            flightSpeed *= friction;
        }

        /// <summary>
        /// Pas tamamlandı
        /// </summary>
        private void PassCompleted()
        {
            Debug.Log($"[Ball] Pass completed to {flightReceiver.name}");

            currentState = BallState.Attached;
            attachedPlayer = flightReceiver;

            // Alıcıya bildir
            var teammate = flightReceiver.GetComponent<TeammateController>();
            if (teammate != null)
            {
                teammate.ReceiveBall();
                ChanceController.Instance?.AITakeOver();
            }

            var player = flightReceiver.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GainBall();
                ChanceController.Instance?.ActionCompleted();
            }

            var opponent = flightReceiver.GetComponent<OpponentController>();
            if (opponent != null)
            {
                opponent.ReceiveBall();
            }

            OnPassReceived?.Invoke(flightReceiver);
        }

        /// <summary>
        /// Şut sonucunu belirle
        /// </summary>
        private void DetermineShotResult()
        {
            // İsabet kontrolü
            float random = UnityEngine.Random.value;

            if (random < shotAccuracy)
            {
                // GOL!
                Debug.Log("[Ball] GOAL!");
                currentState = BallState.Free;
                OnShotResult?.Invoke(ChanceOutcome.Goal);
                
                // Golü kim attı?
                if (ChanceController.Instance != null)
                {
                    if (isPlayerShot)
                    {
                        ChanceController.Instance.PlayerScored();
                    }
                    else if (!isOpponentShot)
                    {
                        ChanceController.Instance.TeammateScored();
                    }
                    else
                    {
                        ChanceController.Instance.EndChance(ChanceOutcome.Goal);
                    }
                }
            }
            else
            {
                // Kaçtı - ama pozisyon devam ediyor! Top serbest.
                Debug.Log("[Ball] Shot missed! Ball is free.");
                currentState = BallState.Free;
                OnShotResult?.Invoke(ChanceOutcome.Missed);
                
                // Kamera topu takip etsin
                ChanceController.Instance?.ChanceCamera?.SetTarget(transform, ChanceCamera.CameraMode.FollowBall);
                
                // Slow-motion'a gir - oyuncu topa koşabilsin
                ChanceController.Instance?.ActionCompleted();
            }
        }

        /// <summary>
        /// Şut kurtarıldı
        /// </summary>
        private void ShotSaved()
        {
            Debug.Log("[Ball] Shot saved!");
            currentState = BallState.Free;
            OnShotResult?.Invoke(ChanceOutcome.Saved);
            ChanceController.Instance?.EndChance(ChanceOutcome.Saved);
        }

        // === PUBLIC METHODS ===

        /// <summary>
        /// Oyuncuya bağla
        /// </summary>
        public void AttachToPlayer(GameObject player)
        {
            attachedPlayer = player;
            currentState = BallState.Attached;
            flightReceiver = null;

            // PlayerController'a topu ver
            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.GainBall();
                Debug.Log($"[Ball] Attached to {player.name} (PlayerController found, hasBall set)");
            }
            else
            {
                Debug.Log($"[Ball] Attached to {player.name}");
            }
        }

        /// <summary>
        /// Pas at
        /// </summary>
        public void Pass(GameObject receiver, float speed, float curve, float successChance)
        {
            if (receiver == null) return;

            // Başarı kontrolü
            if (UnityEngine.Random.value > successChance)
            {
                // Pas kesildi
                Debug.Log("[Ball] Pass intercepted!");
                currentState = BallState.Free;
                attachedPlayer = null;
                OnBallLost?.Invoke();
                ChanceController.Instance?.EndChance(ChanceOutcome.Intercepted);
                return;
            }

            flightReceiver = receiver;
            flightTarget = receiver.transform.position;
            flightSpeed = speed;
            flightCurve = curve * curveStrength;
            currentState = BallState.PassFlight;
            attachedPlayer = null;

            Debug.Log($"[Ball] Pass to {receiver.name}, Speed: {speed}, Curve: {curve}");
        }

        /// <summary>
        /// Şut at
        /// </summary>
        public void Shoot(Vector2 targetPos, float power, float curve, float accuracy, bool isOpponent = false, bool isPlayer = true)
        {
            shotTarget = targetPos;
            flightSpeed = power;
            flightCurve = curve * curveStrength;
            shotAccuracy = accuracy;
            isOpponentShot = isOpponent;
            isPlayerShot = isPlayer;
            currentState = BallState.ShotFlight;
            attachedPlayer = null;
            flightReceiver = null;

            Debug.Log($"[Ball] Shot! Target: {targetPos}, Power: {power}, Curve: {curve}, Accuracy: {accuracy}");
        }

        /// <summary>
        /// Topu serbest bırak
        /// </summary>
        public void Release()
        {
            currentState = BallState.Free;
            attachedPlayer = null;
            flightReceiver = null;
        }

        /// <summary>
        /// Uzaklaştır (clear)
        /// </summary>
        public void Clear(Vector2 direction)
        {
            Release();
            
            // Görsel olarak uzaklaştır
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(direction.normalized * 15f, ForceMode2D.Impulse);

            Debug.Log("[Ball] Cleared!");
        }
    }
}
