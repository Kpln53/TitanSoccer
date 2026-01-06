using UnityEngine;
using System.Collections.Generic;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Ana Pozisyon Kontrolcüsü
    /// Tüm pozisyon oynanışını yönetir
    /// </summary>
    public class ChanceController : MonoBehaviour
    {
        public static ChanceController Instance { get; private set; }

        [Header("Referanslar")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BallController ballController;
        [SerializeField] private ChanceCamera chanceCamera;
        [SerializeField] private GoalkeeperAI goalkeperAI;

        [Header("Prefabs")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject teammatePrefab;
        [SerializeField] private GameObject opponentPrefab;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject goalkeeperPrefab;

        [Header("Saha Ayarları")]
        [SerializeField] private Vector2 fieldSize = new Vector2(30f, 20f);
        [SerializeField] private Vector2 goalPosition = new Vector2(0f, 10f);
        [SerializeField] private float penaltyAreaWidth = 12f;
        [SerializeField] private float penaltyAreaHeight = 5f;

        [Header("Oyun Durumu")]
        [SerializeField] private ChanceType currentChanceType;
        [SerializeField] private GameFlowState flowState;
        [SerializeField] private ChanceOutcome outcome;

        [Header("Takımlar")]
        [SerializeField] private List<GameObject> teammates = new List<GameObject>();
        [SerializeField] private List<GameObject> opponents = new List<GameObject>();
        [SerializeField] private GameObject goalkeeper;

        // Properties
        public ChanceType CurrentChanceType => currentChanceType;
        public GameFlowState FlowState => flowState;
        public ChanceOutcome Outcome => outcome;
        public PlayerController Player => playerController;
        public BallController Ball => ballController;
        public Vector2 GoalPosition => goalPosition;
        public Vector2 FieldSize => fieldSize;
        public List<GameObject> Teammates => teammates;
        public List<GameObject> Opponents => opponents;
        public ChanceCamera ChanceCamera => chanceCamera;

        // Setup verileri
        private ChanceSetupData setupData;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // SlowMotionManager'ı oluştur (yoksa)
            if (SlowMotionManager.Instance == null)
            {
                var slowMoObj = new GameObject("SlowMotionManager");
                slowMoObj.AddComponent<SlowMotionManager>();
                Debug.Log("[ChanceController] SlowMotionManager created");
            }
        }

        private void Start()
        {
            // MatchContext'ten veri al
            if (MatchContext.Instance != null)
            {
                InitializeFromMatchContext();
            }
            else
            {
                // Test modu - varsayılan değerlerle başlat
                Debug.LogWarning("[ChanceController] MatchContext not found, using test mode");
                InitializeTestMode();
            }
        }

        /// <summary>
        /// MatchContext'ten pozisyonu başlat
        /// </summary>
        private void InitializeFromMatchContext()
        {
            MatchContext ctx = MatchContext.Instance;

            setupData = new ChanceSetupData
            {
                chanceType = ctx.currentChance.chanceType == MatchContext.ChanceData.ChanceType.Attack 
                    ? ChanceType.Attack 
                    : ChanceType.Defense,
                minute = ctx.currentMinute,
                playerPosition = ctx.playerPosition,
                playerOverall = ctx.playerOverall,
                homeTeamPower = ctx.homeTeamPower,
                awayTeamPower = ctx.awayTeamPower,
                playerEnergy = ctx.playerEnergy,
                playerForm = ctx.playerMoral
            };

            // SaveData'dan stat'ları al
            if (GameManager.Instance?.CurrentSave?.playerProfile != null)
            {
                var profile = GameManager.Instance.CurrentSave.playerProfile;
                setupData.shootingStat = profile.shootingSkill;
                setupData.passingStat = profile.passingSkill;
                setupData.dribblingStat = profile.dribblingSkill;
                setupData.speedStat = profile.speed;
                setupData.physicsStat = profile.physicalStrength;
                setupData.defenseStat = profile.defendingSkill;
                setupData.falsoStat = profile.falsoSkill;
            }

            currentChanceType = setupData.chanceType;
            SetupChance();
        }

        /// <summary>
        /// Test modu için başlat
        /// </summary>
        private void InitializeTestMode()
        {
            setupData = new ChanceSetupData
            {
                chanceType = ChanceType.Attack,
                minute = 45,
                playerPosition = PlayerPosition.SF,
                playerOverall = 75,
                homeTeamPower = 70,
                awayTeamPower = 65,
                playerEnergy = 80f,
                playerForm = 70f,
                shootingStat = 75,
                passingStat = 70,
                dribblingStat = 72,
                speedStat = 78,
                physicsStat = 68,
                defenseStat = 45,
                falsoStat = 70
            };

            currentChanceType = ChanceType.Attack;
            SetupChance();
        }

        /// <summary>
        /// Pozisyonu kur
        /// </summary>
        private void SetupChance()
        {
            ClearField();

            if (currentChanceType == ChanceType.Attack)
            {
                SetupAttackChance();
            }
            else
            {
                SetupDefenseChance();
            }

            // Slow-motion ile başla
            flowState = GameFlowState.WaitingForInput;
            SlowMotionManager.Instance?.EnableSlowMotion();

            Debug.Log($"[ChanceController] Chance setup complete: {currentChanceType}");
        }

        /// <summary>
        /// Atak pozisyonu kurulumu
        /// </summary>
        private void SetupAttackChance()
        {
            // Oyuncu pozisyonu (mevkiye göre)
            Vector2 playerPos = GetPlayerStartPosition(setupData.playerPosition, true);
            SpawnPlayer(playerPos);

            // Top oyuncuda başlar
            SpawnBall(playerPos + Vector2.up * 0.3f);
            ballController.AttachToPlayer(playerController.gameObject);
            playerController.GainBall(); // ← EKLENEN: Oyuncuya topu ver!

            // Takım arkadaşları
            SpawnTeammates(true);

            // Rakip savunma + kaleci
            SpawnOpponents(false);
            SpawnGoalkeeper(new Vector2(goalPosition.x, goalPosition.y));

            // Kamera oyuncuya odaklansın
            chanceCamera?.SetTarget(playerController.transform, ChanceCamera.CameraMode.FollowPlayer);
        }

        /// <summary>
        /// Savunma pozisyonu kurulumu
        /// </summary>
        private void SetupDefenseChance()
        {
            // Oyuncu pozisyonu (savunma konumu)
            Vector2 playerPos = GetPlayerStartPosition(setupData.playerPosition, false);
            SpawnPlayer(playerPos);

            // Takım arkadaşları (savunmada)
            SpawnTeammates(false);

            // Rakip atak + top rakipte
            SpawnOpponents(true);
            
            // Top rakip oyuncuda başlar
            if (opponents.Count > 0)
            {
                Vector2 ballPos = (Vector2)opponents[0].transform.position + Vector2.up * 0.3f;
                SpawnBall(ballPos);
                ballController.AttachToPlayer(opponents[0]);
            }

            // Kaleci (bizim kalemiz)
            SpawnGoalkeeper(new Vector2(0f, -goalPosition.y));

            // Kamera oyuncuya
            chanceCamera?.SetTarget(playerController.transform, ChanceCamera.CameraMode.FollowPlayer);
        }

        /// <summary>
        /// Mevkiye göre başlangıç pozisyonu
        /// </summary>
        private Vector2 GetPlayerStartPosition(PlayerPosition position, bool isAttack)
        {
            float yBase = isAttack ? 0f : -5f;
            float xOffset = 0f;

            // Defans mevkileri
            bool isDefensePosition = position == PlayerPosition.STP || 
                                     position == PlayerPosition.SĞB || 
                                     position == PlayerPosition.SLB || 
                                     position == PlayerPosition.MDO;

            if (isAttack)
            {
                // Atak pozisyonunda - mevkiye göre konum
                switch (position)
                {
                    case PlayerPosition.SF:
                        return new Vector2(Random.Range(-3f, 3f), Random.Range(3f, 6f));
                    case PlayerPosition.SĞK:
                    case PlayerPosition.SĞO:
                        return new Vector2(Random.Range(4f, 7f), Random.Range(1f, 4f));
                    case PlayerPosition.SLK:
                    case PlayerPosition.SLO:
                        return new Vector2(Random.Range(-7f, -4f), Random.Range(1f, 4f));
                    case PlayerPosition.MOO:
                        return new Vector2(Random.Range(-2f, 2f), Random.Range(0f, 3f));
                    case PlayerPosition.MDO:
                        return new Vector2(Random.Range(-2f, 2f), Random.Range(-3f, 0f));
                    case PlayerPosition.STP:
                        return new Vector2(Random.Range(-1f, 1f), Random.Range(-5f, -3f));
                    case PlayerPosition.SĞB:
                        return new Vector2(Random.Range(5f, 8f), Random.Range(-4f, -1f));
                    case PlayerPosition.SLB:
                        return new Vector2(Random.Range(-8f, -5f), Random.Range(-4f, -1f));
                    default:
                        return new Vector2(0f, 2f);
                }
            }
            else
            {
                // Savunma pozisyonunda - savunma konumu
                switch (position)
                {
                    case PlayerPosition.STP:
                        return new Vector2(Random.Range(-2f, 2f), Random.Range(-7f, -5f));
                    case PlayerPosition.SĞB:
                        return new Vector2(Random.Range(4f, 7f), Random.Range(-6f, -4f));
                    case PlayerPosition.SLB:
                        return new Vector2(Random.Range(-7f, -4f), Random.Range(-6f, -4f));
                    case PlayerPosition.MDO:
                        return new Vector2(Random.Range(-3f, 3f), Random.Range(-4f, -2f));
                    default:
                        // Atak oyuncuları savunmada uzakta
                        return new Vector2(Random.Range(-4f, 4f), Random.Range(-2f, 2f));
                }
            }
        }

        /// <summary>
        /// Oyuncuyu spawn et
        /// </summary>
        private void SpawnPlayer(Vector2 position)
        {
            GameObject playerObj;
            
            if (playerPrefab != null)
            {
                playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            }
            else
            {
                playerObj = CreateDefaultPlayer(position, Color.yellow, "Player");
            }

            playerController = playerObj.GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = playerObj.AddComponent<PlayerController>();
            }

            // Stats ayarla
            playerController.SetupStats(setupData);
        }

        /// <summary>
        /// Topu spawn et
        /// </summary>
        private void SpawnBall(Vector2 position)
        {
            GameObject ballObj;

            if (ballPrefab != null)
            {
                ballObj = Instantiate(ballPrefab, position, Quaternion.identity);
            }
            else
            {
                ballObj = CreateDefaultBall(position);
            }

            ballController = ballObj.GetComponent<BallController>();
            if (ballController == null)
            {
                ballController = ballObj.AddComponent<BallController>();
            }
        }

        /// <summary>
        /// Takım arkadaşlarını spawn et
        /// </summary>
        private void SpawnTeammates(bool isAttackFormation)
        {
            List<Vector2> positions = GetTeammatePositions(isAttackFormation);

            foreach (Vector2 pos in positions)
            {
                GameObject teammate;
                if (teammatePrefab != null)
                {
                    teammate = Instantiate(teammatePrefab, pos, Quaternion.identity);
                }
                else
                {
                    teammate = CreateDefaultPlayer(pos, Color.blue, "Teammate");
                }

                var ai = teammate.GetComponent<TeammateController>();
                if (ai == null)
                {
                    ai = teammate.AddComponent<TeammateController>();
                }

                teammates.Add(teammate);
            }
        }

        /// <summary>
        /// Rakipleri spawn et
        /// </summary>
        private void SpawnOpponents(bool isAttackFormation)
        {
            List<Vector2> positions = GetOpponentPositions(isAttackFormation);

            foreach (Vector2 pos in positions)
            {
                GameObject opponent;
                if (opponentPrefab != null)
                {
                    opponent = Instantiate(opponentPrefab, pos, Quaternion.identity);
                }
                else
                {
                    opponent = CreateDefaultPlayer(pos, Color.red, "Opponent");
                }

                var ai = opponent.GetComponent<OpponentController>();
                if (ai == null)
                {
                    ai = opponent.AddComponent<OpponentController>();
                }
                ai.SetTeamPower(setupData.awayTeamPower);

                opponents.Add(opponent);
            }
        }

        /// <summary>
        /// Kaleci spawn et
        /// </summary>
        private void SpawnGoalkeeper(Vector2 position)
        {
            if (goalkeeperPrefab != null)
            {
                goalkeeper = Instantiate(goalkeeperPrefab, position, Quaternion.identity);
            }
            else
            {
                goalkeeper = CreateDefaultPlayer(position, Color.green, "Goalkeeper");
            }

            goalkeperAI = goalkeeper.GetComponent<GoalkeeperAI>();
            if (goalkeperAI == null)
            {
                goalkeperAI = goalkeeper.AddComponent<GoalkeeperAI>();
            }
        }

        /// <summary>
        /// Takım arkadaşı pozisyonları
        /// </summary>
        private List<Vector2> GetTeammatePositions(bool isAttack)
        {
            List<Vector2> positions = new List<Vector2>();

            if (isAttack)
            {
                // Atak formasyonu - 3-4 takım arkadaşı
                positions.Add(new Vector2(-5f, 4f));   // Sol kanat
                positions.Add(new Vector2(5f, 4f));    // Sağ kanat
                positions.Add(new Vector2(-2f, 1f));   // Sol orta
                positions.Add(new Vector2(2f, 1f));    // Sağ orta
            }
            else
            {
                // Savunma formasyonu
                positions.Add(new Vector2(-4f, -6f));  // Sol bek
                positions.Add(new Vector2(4f, -6f));   // Sağ bek
                positions.Add(new Vector2(-1f, -5f));  // Sol stoper
                positions.Add(new Vector2(1f, -5f));   // Sağ stoper
            }

            return positions;
        }

        /// <summary>
        /// Rakip pozisyonları
        /// </summary>
        private List<Vector2> GetOpponentPositions(bool opponentIsAttacking)
        {
            List<Vector2> positions = new List<Vector2>();

            if (opponentIsAttacking)
            {
                // Rakip atakta - önde
                positions.Add(new Vector2(0f, 3f));    // Forvet
                positions.Add(new Vector2(-4f, 1f));   // Sol kanat
                positions.Add(new Vector2(4f, 1f));    // Sağ kanat
            }
            else
            {
                // Rakip savunmada
                positions.Add(new Vector2(-3f, 7f));   // Sol bek
                positions.Add(new Vector2(3f, 7f));    // Sağ bek
                positions.Add(new Vector2(-1f, 6f));   // Sol stoper
                positions.Add(new Vector2(1f, 6f));    // Sağ stoper
            }

            return positions;
        }

        /// <summary>
        /// Varsayılan oyuncu oluştur
        /// </summary>
        private GameObject CreateDefaultPlayer(Vector2 position, Color color, string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.position = position;

            // Sprite
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite(32);
            sr.color = color;
            sr.sortingOrder = 1;

            // Collider
            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            // Rigidbody
            Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearDamping = 5f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            return obj;
        }

        /// <summary>
        /// Varsayılan top oluştur
        /// </summary>
        private GameObject CreateDefaultBall(Vector2 position)
        {
            GameObject obj = new GameObject("Ball");
            obj.transform.position = position;
            obj.transform.localScale = Vector3.one * 0.5f;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite(32);
            sr.color = Color.white;
            sr.sortingOrder = 2;

            CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;

            Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            return obj;
        }

        /// <summary>
        /// Daire sprite oluştur
        /// </summary>
        private Sprite CreateCircleSprite(int size)
        {
            Texture2D tex = new Texture2D(size, size);
            float radius = size / 2f;
            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>
        /// Sahayı temizle
        /// </summary>
        private void ClearField()
        {
            foreach (var t in teammates) if (t != null) Destroy(t);
            foreach (var o in opponents) if (o != null) Destroy(o);
            if (goalkeeper != null) Destroy(goalkeeper);
            if (playerController != null) Destroy(playerController.gameObject);
            if (ballController != null) Destroy(ballController.gameObject);

            teammates.Clear();
            opponents.Clear();
        }

        // === GAME FLOW METHODS ===

        /// <summary>
        /// Eylem başlat (slow-motion'dan çık)
        /// </summary>
        public void StartAction()
        {
            if (flowState != GameFlowState.WaitingForInput) return;

            flowState = GameFlowState.Executing;
            SlowMotionManager.Instance?.DisableSlowMotion();
        }

        /// <summary>
        /// Eylem tamamlandı (tekrar slow-motion)
        /// </summary>
        public void ActionCompleted()
        {
            if (flowState == GameFlowState.Ended) return;

            flowState = GameFlowState.WaitingForInput;
            SlowMotionManager.Instance?.EnableSlowMotion();
            
            // Kamera oyuncuya dönsün
            if (chanceCamera != null && playerController != null)
            {
                chanceCamera.SetTarget(playerController.transform, ChanceCamera.CameraMode.FollowPlayer);
            }
        }

        /// <summary>
        /// Top havaya kalktı (pas/şut)
        /// </summary>
        public void BallInFlight()
        {
            flowState = GameFlowState.BallInFlight;
            SlowMotionManager.Instance?.DisableSlowMotion();
            
            // Kamera topu takip etsin
            if (chanceCamera != null && ballController != null)
            {
                chanceCamera.SetTarget(ballController.transform, ChanceCamera.CameraMode.FollowBall);
            }
        }

        /// <summary>
        /// AI oynamaya başladı
        /// </summary>
        public void AITakeOver()
        {
            flowState = GameFlowState.AIPlaying;
        }

        /// <summary>
        /// Pozisyon bitti
        /// </summary>
        public void EndChance(ChanceOutcome result)
        {
            outcome = result;
            flowState = GameFlowState.Ended;
            SlowMotionManager.Instance?.ResetToNormal();

            Debug.Log($"[ChanceController] Chance ended: {result}");

            // Sonucu MatchContext'e yaz
            UpdateMatchContext(result);

            // 2 saniye sonra MatchSim'e dön
            Invoke(nameof(ReturnToMatchSim), 2f);
        }

        /// <summary>
        /// MatchContext güncelle
        /// </summary>
        private void UpdateMatchContext(ChanceOutcome result)
        {
            if (MatchContext.Instance == null) return;

            var ctx = MatchContext.Instance;

            switch (result)
            {
                case ChanceOutcome.Goal:
                    if (currentChanceType == ChanceType.Attack)
                    {
                        ctx.homeScore++;
                        
                        // Golü kim attı?
                        if (lastScorerWasPlayer)
                        {
                            ctx.playerGoals++;
                            ctx.playerMatchRating = Mathf.Min(10f, ctx.playerMatchRating + 1.5f);
                            ctx.AddCommentary($"⚽ GOL! {ctx.playerName} harika bir vuruşla topu ağlara gönderdi!");
                        }
                        else
                        {
                            // Takım arkadaşı gol attı - asist?
                            if (playerMadeLastPass)
                            {
                                ctx.playerAssists++;
                                ctx.playerMatchRating = Mathf.Min(10f, ctx.playerMatchRating + 1.0f);
                                ctx.AddCommentary($"⚽ GOL! {ctx.playerName}'ın asisti ile skor değişti!");
                            }
                            else
                            {
                                ctx.AddCommentary($"⚽ GOL! Takım skoru artırdı!");
                            }
                        }
                    }
                    else
                    {
                        // Rakip gol attı
                        ctx.awayScore++;
                        ctx.playerMatchRating = Mathf.Max(0f, ctx.playerMatchRating - 0.5f);
                        ctx.AddCommentary($"⚽ Rakip GOL! Defans hata yaptı.");
                    }
                    break;

                case ChanceOutcome.Saved:
                    ctx.playerShots++;
                    if (currentChanceType == ChanceType.Attack)
                    {
                        ctx.playerMatchRating = Mathf.Max(0f, ctx.playerMatchRating - 0.1f);
                        ctx.AddCommentary($"Kaleci kurtardı! {ctx.playerName}'ın şutu etkisiz kaldı.");
                    }
                    else
                    {
                        ctx.playerMatchRating = Mathf.Min(10f, ctx.playerMatchRating + 0.5f);
                        ctx.AddCommentary("Kalecimiz harika bir kurtarış yaptı!");
                    }
                    break;

                case ChanceOutcome.Missed:
                    ctx.playerShots++;
                    if (currentChanceType == ChanceType.Attack)
                    {
                        ctx.playerMatchRating = Mathf.Max(0f, ctx.playerMatchRating - 0.2f);
                        ctx.AddCommentary($"Şut dışarı! {ctx.playerName} isabetli vuramadı.");
                    }
                    break;

                case ChanceOutcome.Blocked:
                    ctx.playerShots++;
                    if (currentChanceType == ChanceType.Attack)
                    {
                        ctx.playerMatchRating = Mathf.Max(0f, ctx.playerMatchRating - 0.1f);
                        ctx.AddCommentary("Şut engellendi! Defans müdahale etti.");
                    }
                    break;

                case ChanceOutcome.Cleared:
                case ChanceOutcome.Tackled:
                    if (currentChanceType == ChanceType.Defense)
                    {
                        ctx.playerMatchRating = Mathf.Min(10f, ctx.playerMatchRating + 0.4f);
                        ctx.AddCommentary($"{ctx.playerName} kritik bir müdahale yaptı!");
                    }
                    break;

                case ChanceOutcome.Intercepted:
                    ctx.playerMatchRating = Mathf.Max(0f, ctx.playerMatchRating - 0.3f);
                    ctx.AddCommentary("Top kaybedildi! Rakip topa sahip oldu.");
                    break;
            }
            
            // Şut istatistiğini güncelle
            Debug.Log($"[ChanceController] Stats - Goals: {ctx.playerGoals}, Assists: {ctx.playerAssists}, Shots: {ctx.playerShots}, Rating: {ctx.playerMatchRating:F1}");
        }
        
        // Asist ve gol takibi için
        private bool lastScorerWasPlayer = true;
        private bool playerMadeLastPass = false;
        
        /// <summary>
        /// Oyuncu pas attı (asist takibi için)
        /// </summary>
        public void PlayerMadePass()
        {
            playerMadeLastPass = true;
        }
        
        /// <summary>
        /// Takım arkadaşı gol attı
        /// </summary>
        public void TeammateScored()
        {
            lastScorerWasPlayer = false;
            EndChance(ChanceOutcome.Goal);
        }
        
        /// <summary>
        /// Oyuncu gol attı
        /// </summary>
        public void PlayerScored()
        {
            lastScorerWasPlayer = true;
            playerMadeLastPass = false;
            EndChance(ChanceOutcome.Goal);
        }

        /// <summary>
        /// MatchSim'e dön
        /// </summary>
        private void ReturnToMatchSim()
        {
            SceneFlow.LoadMatchSim();
        }

        /// <summary>
        /// Hedef kale bölgesinde mi kontrol et
        /// </summary>
        public bool IsInGoalArea(Vector2 position)
        {
            float goalY = currentChanceType == ChanceType.Attack ? goalPosition.y : -goalPosition.y;
            return Mathf.Abs(position.x) < penaltyAreaWidth / 2f && 
                   ((currentChanceType == ChanceType.Attack && position.y > goalY - penaltyAreaHeight) ||
                    (currentChanceType == ChanceType.Defense && position.y < goalY + penaltyAreaHeight));
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}

