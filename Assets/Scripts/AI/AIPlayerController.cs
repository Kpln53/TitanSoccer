using UnityEngine;
using System.Collections;

/// <summary>
/// AI oyuncu kontrol sistemi - AI oyuncuların davranışlarını yönetir
/// </summary>
public class AIPlayerController : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Player player;
    [SerializeField] private Transform ball; // Top referansı
    [SerializeField] private PositionConfig positionConfig;
    
    [Header("AI Ayarları")]
    [SerializeField] private float decisionInterval = 0.5f; // Karar verme aralığı (saniye)
    [SerializeField] private float ballDetectionRange = 15f; // Top algılama mesafesi
    [SerializeField] private float positionTolerance = 2f; // Pozisyon toleransı
    [SerializeField] private float chaseBallThreshold = 10f; // Topu kovalamak için mesafe eşiği
    
    [Header("AI Davranışları")]
    [SerializeField] private bool shouldChaseBall = true; // Topu kovalasın mı?
    [SerializeField] private bool shouldReturnToPosition = true; // Pozisyonuna dönsün mü?
    [SerializeField] private float aggressionLevel = 0.5f; // Saldırganlık seviyesi (0-1) - Takım gücüne göre ayarlanır
    [SerializeField] private float ballControlDistance = 0.5f; // Topu kontrol etmek için mesafe (daha küçük)
    [SerializeField] private float ballPossessionDistance = 1.5f; // Topu "aldı" saymak için mesafe
    [SerializeField] private float pressureDetectionRange = 5f; // Baskı algılama mesafesi - Takım gücüne göre ayarlanır
    [SerializeField] private float passToPlayerChance = 0.2f; // Oyuncuya pas yapma şansı (%20) - Takım gücüne göre ayarlanır
    
    private int teamPower = 50; // Takım gücü (varsayılan 50)
    
    private AIStateData stateData;
    private TeamManager myTeam;
    private TeamManager opponentTeam;
    private PositionData myPositionData;
    private Vector2 homePosition; // Mevkiye göre ev pozisyonu
    private float lastDecisionTime = 0f;
    private float gameStartTime = 0f; // Oyun başlangıç zamanı
    private bool hasBallPossession = false; // Topu kontrol ediyor mu?
    private Vector2 lastTargetPosition = Vector2.zero; // Son hedef pozisyon
    private float lastMoveTime = 0f; // Son hareket zamanı
    
    void Awake()
    {
        if (player == null)
            player = GetComponent<Player>();
        
        stateData = new AIStateData();
    }
    
    void Start()
    {
        // Takım referanslarını al
        myTeam = player.isHomeTeam ? TeamManager.HomeTeam : TeamManager.AwayTeam;
        opponentTeam = player.isHomeTeam ? TeamManager.AwayTeam : TeamManager.HomeTeam;
        
        // PositionConfig'i al
        if (positionConfig == null && myTeam != null)
        {
            // TeamManager'dan PositionConfig'i al (reflection ile)
            var field = typeof(TeamManager).GetField("positionConfig", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                positionConfig = field.GetValue(myTeam) as PositionConfig;
            }
        }
        
        // Mevki verisini al
        if (positionConfig != null)
        {
            myPositionData = positionConfig.GetPositionData(player.position);
            if (myPositionData != null)
            {
                CalculateHomePosition();
            }
        }
        
        // Top referansını bul
        if (ball == null)
        {
            GameObject ballObj = GameObject.Find("Ball");
            if (ballObj != null)
                ball = ballObj.transform;
        }
        
        // Başlangıç durumu
        stateData.currentState = AIState.Idle;
        gameStartTime = Time.time;
        hasBallPossession = false;
        
        // Takım gücünü al ve AI parametrelerini ayarla
        SetupTeamPower();
    }
    
    /// <summary>
    /// Takım gücünü al ve AI parametrelerini ayarla
    /// </summary>
    void SetupTeamPower()
    {
        // TeamManager'dan takım gücünü al
        if (myTeam != null)
        {
            teamPower = myTeam.GetTeamPower();
            
            // Takım gücüne göre AI parametrelerini ayarla
            aggressionLevel = TeamPowerCalculator.GetAggressionLevel(teamPower);
            decisionInterval = TeamPowerCalculator.GetDecisionSpeed(teamPower);
            chaseBallThreshold = TeamPowerCalculator.GetChaseBallThreshold(teamPower);
            positionTolerance = TeamPowerCalculator.GetPositionTolerance(teamPower);
            pressureDetectionRange = TeamPowerCalculator.GetPressureDetectionRange(teamPower);
            passToPlayerChance = TeamPowerCalculator.GetPassChance(teamPower);
            
            Debug.Log($"[AIPlayerController] {player.playerName} - Takım gücü: {teamPower}, Saldırganlık: {aggressionLevel:F2}");
        }
        else
        {
            // Varsayılan değerler
            teamPower = 50;
        }
    }
    
    
    void Update()
    {
        if (player == null) return;
        
        // Top referansı yoksa bulmaya çalış
        if (ball == null)
        {
            FindBall();
            // Top hala yoksa sadece pozisyonuna dön
            if (ball == null)
            {
                if (shouldReturnToPosition && IsAwayFromHomePosition())
                {
                    MoveToPosition(homePosition);
                }
                return;
            }
        }
        
        // Karar verme zamanı geldi mi?
        if (Time.time - lastDecisionTime >= decisionInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }
        
        // Mevcut duruma göre davranışı güncelle
        UpdateBehavior();
    }
    
    /// <summary>
    /// Top referansını bul
    /// </summary>
    void FindBall()
    {
        // Önce GameObject.Find ile dene
        GameObject ballObj = GameObject.Find("Ball");
        if (ballObj != null)
        {
            ball = ballObj.transform;
            return;
        }
        
        // Tag ile dene (eğer tag varsa)
        try
        {
            ballObj = GameObject.FindGameObjectWithTag("Ball");
            if (ballObj != null)
            {
                ball = ballObj.transform;
                return;
            }
        }
        catch
        {
            // Tag yoksa devam et
        }
        
        // MatchChanceSceneManager'dan al
        MatchChanceSceneManager sceneManager = FindObjectOfType<MatchChanceSceneManager>();
        if (sceneManager != null)
        {
            // Reflection ile currentBall'ı al
            var field = typeof(MatchChanceSceneManager).GetField("currentBall", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                GameObject ballGameObj = field.GetValue(sceneManager) as GameObject;
                if (ballGameObj != null)
                {
                    ball = ballGameObj.transform;
                    return;
                }
            }
        }
        
    }
    
    /// <summary>
    /// AI karar verme sistemi
    /// </summary>
    void MakeDecision()
    {
        if (player == null || ball == null) return;
        
        Vector2 myPos = (Vector2)transform.position;
        Vector2 ballPos = (Vector2)ball.position;
        float distanceToBall = Vector2.Distance(myPos, ballPos);
        
        // Durum makinesi mantığı
        switch (stateData.currentState)
        {
            case AIState.Idle:
                // Top yakınsa hemen kovala
                if (shouldChaseBall && distanceToBall <= chaseBallThreshold)
                {
                    ChangeState(AIState.ChasingBall);
                }
                // Pozisyonundan uzaksa dön
                else if (shouldReturnToPosition && IsAwayFromHomePosition())
                {
                    ChangeState(AIState.ReturningToPosition);
                }
                break;
                
            case AIState.ChasingBall:
                // Top çok uzaksa pozisyonuna dön
                if (distanceToBall > ballDetectionRange)
                {
                    ChangeState(AIState.ReturningToPosition);
                    hasBallPossession = false;
                }
                // Topa yakınsa pozisyonuna dön (top kontrolü yok artık)
                else if (distanceToBall < 1.0f)
                {
                    // Topa çok yakınsa bekle
                    if (player.IsMoving)
                    {
                        player.Stop();
                    }
                }
                break;
                
            case AIState.ReturningToPosition:
                // Pozisyonuna vardıysa bekle
                if (!IsAwayFromHomePosition())
                {
                    ChangeState(AIState.Idle);
                }
                // Top çok yakınsa kovala
                else if (shouldChaseBall && distanceToBall <= chaseBallThreshold)
                {
                    ChangeState(AIState.ChasingBall);
                }
                break;
                
            case AIState.Attacking:
                // Topu kaybettiyse pozisyonuna dön
                if (distanceToBall > ballPossessionDistance)
                {
                    hasBallPossession = false;
                    ChangeState(AIState.ReturningToPosition);
                }
                // Topu hala kontrol ediyor mu?
                else if (distanceToBall > ballControlDistance)
                {
                    // Topu kaybetmek üzere, tekrar kovala
                    hasBallPossession = false;
                    ChangeState(AIState.ChasingBall);
                }
                break;
        }
    }
    
    /// <summary>
    /// Mevcut duruma göre davranışı güncelle
    /// </summary>
    void UpdateBehavior()
    {
        switch (stateData.currentState)
        {
            case AIState.Idle:
                // Beklemede - hareket etme
                if (player != null && player.IsMoving)
                {
                    player.Stop();
                }
                break;
                
            case AIState.ChasingBall:
                // Topu kovala
                if (ball != null)
                {
                    Vector2 ballPos = (Vector2)ball.position;
                    Vector2 myPos = (Vector2)transform.position;
                    float distanceToBall = Vector2.Distance(myPos, ballPos);
                    
                    // Topa çok yakınsa bekle
                    if (distanceToBall < 0.5f)
                    {
                        if (player.IsMoving)
                        {
                            player.Stop();
                        }
                    }
                    else
                    {
                        // Topu kovala
                        MoveToPosition(ballPos);
                    }
                }
                break;
                
            case AIState.ReturningToPosition:
                // Pozisyonuna dön
                MoveToPosition(homePosition);
                break;
                
            case AIState.Attacking:
                // Hücum et - kaleye doğru git
                if (hasBallPossession && ball != null)
                {
                    Vector2 goalPos = GetOpponentGoalPosition();
                    Vector2 currentPos = (Vector2)transform.position;
                    Vector2 directionToGoal = (goalPos - currentPos).normalized;
                    
                    // Kaleye doğru git
                    Vector2 targetPos = currentPos + directionToGoal * 5f;
                    MoveToPosition(targetPos);
                }
                else
                {
                    // Topu kontrol etmiyorsa, topu kovala
                    if (ball != null)
                    {
                        Vector2 ballPos = (Vector2)ball.position;
                        MoveToPosition(ballPos);
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// Durumu değiştir
    /// </summary>
    void ChangeState(AIState newState)
    {
        if (stateData.currentState == newState) return;
        
        stateData.currentState = newState;
        stateData.stateTimer = 0f;
        
        Debug.Log($"[AIPlayerController] {player.playerName} durumu: {newState}");
    }
    
    /// <summary>
    /// Pozisyona hareket et (sadece hedef değiştiğinde veya yeterince uzaktayken)
    /// </summary>
    void MoveToPosition(Vector2 targetPos)
    {
        if (player == null) return;
        
        // Saha sınırları içinde mi kontrol et
        PitchManager pitch = PitchManager.Instance;
        if (pitch != null)
        {
            targetPos = pitch.ClampToPitch2D(targetPos);
        }
        
        // Mevki sınırları içinde mi kontrol et
        if (myPositionData != null)
        {
            targetPos = ClampToPositionLimits(targetPos);
        }
        
        // Mevcut pozisyon
        Vector2 currentPos = (Vector2)transform.position;
        float distanceToTarget = Vector2.Distance(currentPos, targetPos);
        
        // Eğer hedef değiştiyse veya yeterince uzaktaysa hareket et
        float distanceToLastTarget = Vector2.Distance(currentPos, lastTargetPosition);
        bool targetChanged = Vector2.Distance(targetPos, lastTargetPosition) > 1f;
        bool shouldMove = targetChanged || (distanceToTarget > 1f && Time.time - lastMoveTime > 0.5f);
        
        if (shouldMove)
        {
            // Oyuncu zaten hareket ediyorsa ve hedef yakınsa bekle
            if (player.IsMoving && distanceToTarget < 2f)
            {
                return; // Hareket etme, mevcut hareketi tamamla
            }
            
            // Oyuncuyu hareket ettir
            player.MoveTo(targetPos);
            lastTargetPosition = targetPos;
            lastMoveTime = Time.time;
        }
    }
    
    /// <summary>
    /// Ev pozisyonunu hesapla
    /// </summary>
    void CalculateHomePosition()
    {
        if (myPositionData == null || positionConfig == null) return;
        
        PitchManager pitch = PitchManager.Instance;
        if (pitch == null) return;
        
        // Normalize pozisyonu world pozisyonuna çevir
        Vector2 normalizedPos = myPositionData.normalizedPosition;
        
        // Ev sahibi takım için Y pozisyonunu ters çevir
        if (player.isHomeTeam)
        {
            normalizedPos.y = -normalizedPos.y;
        }
        
        homePosition = positionConfig.NormalizedToWorldPosition(normalizedPos, pitch);
    }
    
    /// <summary>
    /// Ev pozisyonundan uzakta mı?
    /// </summary>
    bool IsAwayFromHomePosition()
    {
        if (myPositionData == null) return false;
        
        float distance = Vector2.Distance((Vector2)transform.position, homePosition);
        return distance > positionTolerance;
    }
    
    /// <summary>
    /// Pozisyon sınırları içinde tut
    /// </summary>
    Vector2 ClampToPositionLimits(Vector2 position)
    {
        if (myPositionData == null || positionConfig == null) return position;
        
        PitchManager pitch = PitchManager.Instance;
        if (pitch == null) return position;
        
        // Normalize pozisyona çevir
        Vector2 normalizedPos = positionConfig.WorldToNormalizedPosition(position, pitch);
        
        // Ev sahibi takım için Y'yi ters çevir
        if (player.isHomeTeam)
        {
            normalizedPos.y = -normalizedPos.y;
        }
        
        // Sınırları kontrol et
        normalizedPos.x = Mathf.Clamp(normalizedPos.x, myPositionData.movementArea.x, myPositionData.movementArea.y);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, myPositionData.backwardLimit, myPositionData.forwardLimit);
        
        // Ev sahibi takım için Y'yi tekrar ters çevir
        if (player.isHomeTeam)
        {
            normalizedPos.y = -normalizedPos.y;
        }
        
        // World pozisyonuna çevir
        return positionConfig.NormalizedToWorldPosition(normalizedPos, pitch);
    }
    
    /// <summary>
    /// Rakip kale pozisyonunu al
    /// </summary>
    Vector2 GetOpponentGoalPosition()
    {
        PitchManager pitch = PitchManager.Instance;
        if (pitch == null) return Vector2.zero;
        
        // Ev sahibi takım ise rakip kale (yukarı), değilse ev sahibi kale (aşağı)
        if (player.isHomeTeam)
        {
            return pitch.AwayGoalPosition;
        }
        else
        {
            return pitch.HomeGoalPosition;
        }
    }
    
    /// <summary>
    /// Top referansını ayarla
    /// </summary>
    public void SetBall(Transform ballTransform)
    {
        ball = ballTransform;
    }
    
    /// <summary>
    /// Bir pozisyona en yakın rakip oyuncunun mesafesini al
    /// </summary>
    float GetNearestOpponentDistance(Vector2 position)
    {
        if (opponentTeam == null) return float.MaxValue;
        
        float minDistance = float.MaxValue;
        var opponents = opponentTeam.GetAllPlayers();
        
        foreach (var opponent in opponents)
        {
            Vector2 opponentPos = (Vector2)opponent.transform.position;
            float distance = Vector2.Distance(position, opponentPos);
            
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }
        
        return minDistance;
    }
    
    /// <summary>
    /// Mevcut durumu al
    /// </summary>
    public AIState GetCurrentState()
    {
        return stateData.currentState;
    }
}

