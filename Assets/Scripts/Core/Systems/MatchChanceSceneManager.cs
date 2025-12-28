using UnityEngine;
using System.Collections;

/// <summary>
/// Maç pozisyonu/şans 2D sahne yöneticisi - Zaman kırılması ve pas zinciri sistemi
/// </summary>
public class MatchChanceSceneManager : MonoBehaviour
{
    public static MatchChanceSceneManager Instance { get; private set; }

    [Header("Zaman Kırılması")]
    [Range(0.05f, 0.3f)]
    public float slowMotionSpeed = 0.2f; // Oyunun yavaşlama hızı (%20)
    public float maxTimeAmount = 10f; // Maksimum zaman barı süresi (saniye)
    private float currentTimeAmount; // Mevcut zaman barı
    private bool isTimeActive = true; // Zaman barı aktif mi?

    [Header("Pas Zinciri")]
    private int passChainCount = 0; // Mevcut pas zinciri sayısı
    private float lastPassTime = 0f; // Son pas zamanı
    private float chainTimeout = 3f; // Zincir timeout (3 saniye içinde pas yoksa zincir kırılır)

    [Header("Pozisyon Durumu")]
    private MatchChanceData currentChance;
    private bool isPositionActive = false;
    private MatchChanceResult positionResult = MatchChanceResult.Pending;

    [Header("Event Callbacks")]
    public System.Action<float> OnTimeAmountChanged; // (timeAmount)
    public System.Action<int> OnPassChainChanged; // (chainCount)
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
        
        // Zaman barını doldur
        currentTimeAmount = maxTimeAmount;
        OnTimeAmountChanged?.Invoke(currentTimeAmount / maxTimeAmount);

        // Pozisyonu aktif et
        isPositionActive = true;
        positionResult = MatchChanceResult.Pending;
        passChainCount = 0;

        Debug.Log($"[MatchChanceSceneManager] Position started: {currentChance.chanceType} at minute {currentChance.minute}");
    }

    /// <summary>
    /// Zaman kırılmasını başlat (oyunu yavaşlat)
    /// </summary>
    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionSpeed;
        Debug.Log($"[MatchChanceSceneManager] Slow motion started: {slowMotionSpeed}x");
    }

    /// <summary>
    /// Zaman kırılmasını bitir (normal hıza dön)
    /// </summary>
    private void StopSlowMotion()
    {
        Time.timeScale = 1f;
        Debug.Log("[MatchChanceSceneManager] Slow motion stopped");
    }

    private void Update()
    {
        if (!isPositionActive)
            return;

        // Zaman barını tüket
        if (isTimeActive && currentTimeAmount > 0f)
        {
            currentTimeAmount -= Time.unscaledDeltaTime; // unscaledDeltaTime kullan (zaman kırılmasından etkilenmez)
            currentTimeAmount = Mathf.Max(0f, currentTimeAmount);
            
            float normalizedTime = currentTimeAmount / maxTimeAmount;
            OnTimeAmountChanged?.Invoke(normalizedTime);

            // Zaman bitti
            if (currentTimeAmount <= 0f)
            {
                OnTimeOut();
            }
        }

        // Pas zinciri timeout kontrolü
        if (passChainCount > 0 && Time.unscaledTime - lastPassTime > chainTimeout)
        {
            BreakPassChain();
        }
    }

    /// <summary>
    /// Zaman barı tükendiğinde
    /// </summary>
    private void OnTimeOut()
    {
        Debug.Log("[MatchChanceSceneManager] Time out! Control passed to AI.");
        isTimeActive = false;
        
        // AI kontrolüne geç - panik şut/hatalı pas ihtimali
        // Pozisyon sonucunu belirle (genellikle başarısız)
        positionResult = DetermineAIPositionResult();
        FinishPosition();
    }

    /// <summary>
    /// Zaman harcayan bir hamle yapıldı
    /// </summary>
    public bool ConsumeTime(float amount)
    {
        if (!isTimeActive || currentTimeAmount <= 0f)
            return false;

        currentTimeAmount -= amount;
        currentTimeAmount = Mathf.Max(0f, currentTimeAmount);
        
        OnTimeAmountChanged?.Invoke(currentTimeAmount / maxTimeAmount);

        if (currentTimeAmount <= 0f)
        {
            OnTimeOut();
        }

        return true;
    }

    /// <summary>
    /// Pas yapıldı
    /// </summary>
    public void OnPassMade(bool isSuccessful)
    {
        if (!isTimeActive)
            return;

        // Pas için zaman harca
        ConsumeTime(1f); // 1 saniye zaman harcar

        if (isSuccessful)
        {
            // Pas zincirini artır
            passChainCount++;
            lastPassTime = Time.unscaledTime;
            
            OnPassChainChanged?.Invoke(passChainCount);
            
            Debug.Log($"[MatchChanceSceneManager] Pass chain: {passChainCount}");
            
            // Pas zinciri bonuslarını uygula
            ApplyPassChainBonus();
        }
        else
        {
            // Başarısız pas - zincir kırılır
            BreakPassChain();
        }
    }

    /// <summary>
    /// Şut atıldı
    /// </summary>
    public void OnShotTaken()
    {
        if (!isTimeActive)
            return;

        // Şut için zaman harca
        ConsumeTime(2f); // 2 saniye zaman harcar

        // Şut atıldığında pozisyon biter
        // Şut sonucu başka sistem tarafından belirlenecek
    }

    /// <summary>
    /// Koşu emri verildi
    /// </summary>
    public void OnRunCommand()
    {
        if (!isTimeActive)
            return;

        // Koşu için zaman harca
        ConsumeTime(0.5f); // 0.5 saniye zaman harcar
    }

    /// <summary>
    /// Pas zinciri bonuslarını uygula
    /// </summary>
    private void ApplyPassChainBonus()
    {
        switch (passChainCount)
        {
            case 2:
                // Savunma reaksiyonu yavaşlar
                Debug.Log("[MatchChanceSceneManager] Pass chain 2: Defense reaction slowed");
                break;
            case 3:
                // Şut isabeti artar
                Debug.Log("[MatchChanceSceneManager] Pass chain 3: Shot accuracy increased");
                break;
            case 4:
                // Kaleci hata yapabilir
                Debug.Log("[MatchChanceSceneManager] Pass chain 4: Goalkeeper may make mistake");
                break;
            case 5:
                // Altın pozisyon
                Debug.Log("[MatchChanceSceneManager] Pass chain 5: GOLDEN POSITION!");
                break;
        }
    }

    /// <summary>
    /// Pas zincirini kır
    /// </summary>
    private void BreakPassChain()
    {
        if (passChainCount > 0)
        {
            Debug.Log($"[MatchChanceSceneManager] Pass chain broken at {passChainCount}");
            passChainCount = 0;
            OnPassChainChanged?.Invoke(0);
        }
    }

    /// <summary>
    /// Pozisyon sonucunu belirle (AI kontrolünde)
    /// </summary>
    private MatchChanceResult DetermineAIPositionResult()
    {
        // Zaman bitti, AI kontrolünde genellikle başarısız sonuç
        float random = Random.Range(0f, 1f);
        
        if (random < 0.7f) // %70 ihtimal
            return MatchChanceResult.Miss;
        else if (random < 0.9f) // %20 ihtimal
            return MatchChanceResult.Save;
        else // %10 ihtimal
            return MatchChanceResult.Goal; // Şanslı gol
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
        
        // Pas zinciri bonusu
        float chainBonus = GetPassChainBonus();
        successChance += chainBonus;
        
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
    /// Pas zinciri bonusunu al
    /// </summary>
    private float GetPassChainBonus()
    {
        switch (passChainCount)
        {
            case 0:
            case 1:
                return 0f;
            case 2:
                return 0.1f; // %10 bonus
            case 3:
                return 0.2f; // %20 bonus
            case 4:
                return 0.3f; // %30 bonus
            case 5:
                return 0.5f; // %50 bonus (altın pozisyon)
            default:
                return 0.4f;
        }
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

        // Pas zincirini sıfırla
        BreakPassChain();

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
