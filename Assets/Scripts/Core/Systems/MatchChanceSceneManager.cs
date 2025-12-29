using UnityEngine;
using System.Collections;

/// <summary>
/// Maç pozisyonu/şans 2D sahne yöneticisi - Sadece oynanış (UI yok)
/// </summary>
public class MatchChanceSceneManager : MonoBehaviour
{
    public static MatchChanceSceneManager Instance { get; private set; }

    [Header("Zaman Kırılması")]
    [Range(0.05f, 0.3f)]
    public float slowMotionSpeed = 0.2f; // Oyunun yavaşlama hızı (%20)

    [Header("Pozisyon Durumu")]
    private MatchChanceData currentChance;
    private bool isPositionActive = false;
    private MatchChanceResult positionResult = MatchChanceResult.Pending;

    [Header("Ball Control")]
    private BallControlSystem ballControlSystem;

    [Header("Event Callbacks")]
    public System.Action<MatchChanceResult> OnPositionFinished; // (result)

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
        InitializePosition();
    }

    /// <summary>
    /// Pozisyonu başlat
    /// </summary>
    private void InitializePosition()
    {
        currentChance = MatchChanceManager.CurrentChance;
        
        if (currentChance == null)
        {
            Debug.LogError("[MatchChanceSceneManager] No chance data available!");
            ReturnToMatch();
            return;
        }

        // Zaman kırılmasını başlat
        StartSlowMotion();

        // Pozisyonu aktif et
        isPositionActive = true;
        positionResult = MatchChanceResult.Pending;

        // Ball Control System'i ayarla
        SetupBallControlSystem();

        Debug.Log($"[MatchChanceSceneManager] Position started: {currentChance.chanceType} at minute {currentChance.minute}");
    }

    /// <summary>
    /// Ball Control System'i ayarla
    /// </summary>
    private void SetupBallControlSystem()
    {
        // BallControlSystem'i bul veya oluştur
        if (ballControlSystem == null)
        {
            ballControlSystem = FindObjectOfType<BallControlSystem>();
            if (ballControlSystem == null)
            {
                GameObject bcObj = new GameObject("BallControlSystem");
                ballControlSystem = bcObj.AddComponent<BallControlSystem>();
            }
        }

        // Oyuncu ve topu bul
        Transform playerTransform = FindPlayerTransform();
        Transform ballTransform = FindBallTransform();

        if (playerTransform != null && ballTransform != null)
        {
            ballControlSystem.EnterChanceMode(playerTransform, ballTransform);
            Debug.Log("[MatchChanceSceneManager] Ball Control System aktif!");
        }
        else
        {
            Debug.LogWarning("[MatchChanceSceneManager] Oyuncu veya top bulunamadı!");
        }
    }

    /// <summary>
    /// Oyuncu Transform'unu bul
    /// </summary>
    private Transform FindPlayerTransform()
    {
        // FieldManager'dan al
        FieldManager fieldManager = FindObjectOfType<FieldManager>();
        if (fieldManager != null && fieldManager.PlayerCharacter != null)
        {
            return fieldManager.PlayerCharacter.transform;
        }

        // Fallback: PlayerController'ı bul
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            return playerController.transform;
        }

        return null;
    }

    /// <summary>
    /// Top Transform'unu bul
    /// </summary>
    private Transform FindBallTransform()
    {
        // FieldManager'dan al
        FieldManager fieldManager = FindObjectOfType<FieldManager>();
        if (fieldManager != null && fieldManager.Ball != null)
        {
            return fieldManager.Ball.transform;
        }

        // Fallback: Tag ile bul
        GameObject ballObj = GameObject.FindGameObjectWithTag("Ball");
        if (ballObj != null)
        {
            return ballObj.transform;
        }

        return null;
    }

    /// <summary>
    /// Zaman kırılmasını başlat (oyunu yavaşlat)
    /// </summary>
    private void StartSlowMotion()
    {
        // TimeFlowManager varsa onu kullan
        if (TimeFlowManager.Instance != null)
        {
            TimeFlowManager.Instance.SetTimeScaleInstant(slowMotionSpeed);
        }
        else
        {
            Time.timeScale = slowMotionSpeed;
        }
        Debug.Log($"[MatchChanceSceneManager] Slow motion started: {slowMotionSpeed}x");
    }

    /// <summary>
    /// Zaman kırılmasını bitir (normal hıza dön)
    /// </summary>
    private void StopSlowMotion()
    {
        // TimeFlowManager varsa onu kullan
        if (TimeFlowManager.Instance != null)
        {
            TimeFlowManager.Instance.NormalizeTime();
        }
        else
        {
            Time.timeScale = 1f;
        }
        Debug.Log("[MatchChanceSceneManager] Slow motion stopped");
    }

    /// <summary>
    /// Şut atıldı
    /// </summary>
    public void OnShotTaken()
    {
        // Sadece log - zaman barı yok
        Debug.Log("[MatchChanceSceneManager] Şut atıldı");
    }

    /// <summary>
    /// Pozisyon sonucunu ayarla (dışarıdan çağrılır)
    /// </summary>
    public void SetPositionResult(MatchChanceResult result)
    {
        positionResult = result;
        FinishPosition();
    }

    /// <summary>
    /// Şut sonucunu hesapla ve pozisyonu bitir
    /// </summary>
    public void CalculateShotResult(Vector2 shotDirection, float power, bool isHighShot, PlayerController shooter)
    {
        if (!isPositionActive)
            return;

        // Oyuncu yeteneklerini al
        SaveData saveData = GameManager.Instance?.CurrentSave;
        PlayerProfile profile = saveData?.playerProfile;
        
        float successChance = currentChance.successChance;
        
        // Oyuncu yetenekleri bonusu
        if (profile != null)
        {
            float skillBonus = (profile.shootingSkill - 50) / 200f; // -0.25 ile +0.25 arası
            successChance += skillBonus;
        }
        
        // Şut gücü ve yönü faktörü
        float powerFactor = power * 0.2f; // Güçlü şutlar daha başarılı
        successChance += powerFactor;
        
        successChance = Mathf.Clamp(successChance, 0f, 1f);
        
        // Sonucu belirle
        float roll = Random.Range(0f, 1f);
        
        if (roll <= successChance)
        {
            // Başarılı şut
            if (roll <= successChance * 0.7f) // %70 ihtimal gol
            {
                positionResult = MatchChanceResult.Goal;
            }
            else // %30 ihtimal kurtarış
            {
                positionResult = MatchChanceResult.Save;
            }
        }
        else
        {
            // Başarısız şut
            positionResult = MatchChanceResult.Miss;
        }
        
        FinishPosition();
    }

    /// <summary>
    /// Pozisyonu bitir
    /// </summary>
    private void FinishPosition()
    {
        if (!isPositionActive)
            return;

        isPositionActive = false;
        StopSlowMotion();

        // Ball Control System'den çık
        if (ballControlSystem != null)
        {
            ballControlSystem.ExitChanceMode();
        }

        // Sonucu MatchSimulationSystem'e bildir
        if (currentChance != null)
        {
            currentChance.result = positionResult;
            
            // MatchSimulationSystem'e sonucu bildir
            if (MatchSimulationSystem.Instance != null)
            {
                MatchSimulationSystem.Instance.OnChanceSceneFinished(currentChance, positionResult);
            }
        }

        OnPositionFinished?.Invoke(positionResult);

        // Match ekranına geri dön
        StartCoroutine(ReturnToMatchAfterDelay(1f));
    }

    /// <summary>
    /// Match ekranına geri dön (kısa bir gecikme ile)
    /// </summary>
    private IEnumerator ReturnToMatchAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ReturnToMatch();
    }

    /// <summary>
    /// Match ekranına geri dön
    /// </summary>
    private void ReturnToMatch()
    {
        MatchChanceManager.Clear();
        Time.timeScale = 1f;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Match);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Match");
        }
    }
}
