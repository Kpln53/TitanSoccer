using UnityEngine;

/// <summary>
/// Zaman akışı yönetimi - MatchChance sahnesinde timeScale kontrolü
/// </summary>
public class TimeFlowManager : MonoBehaviour
{
    public static TimeFlowManager Instance { get; private set; }

    [Header("Zaman Ayarları")]
    [SerializeField] private float slowMotionScale = 0.2f; // %20 hız
    [SerializeField] private float transitionDuration = 0.3f; // Geçiş süresi (smooth)

    private float targetTimeScale = 1f;
    private float currentTimeScale = 1f;
    private bool isTransitioning = false;

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
        // Başlangıçta normal zaman
        Time.timeScale = 1f;
        currentTimeScale = 1f;
        targetTimeScale = 1f;
    }

    private void Update()
    {
        // Smooth geçiş
        if (isTransitioning)
        {
            currentTimeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, Time.unscaledDeltaTime / transitionDuration);
            Time.timeScale = currentTimeScale;

            // Geçiş tamamlandı mı?
            if (Mathf.Abs(currentTimeScale - targetTimeScale) < 0.01f)
            {
                currentTimeScale = targetTimeScale;
                Time.timeScale = targetTimeScale;
                isTransitioning = false;
            }
        }
    }

    /// <summary>
    /// Zamanı yavaşlat (top oyuncudayken)
    /// </summary>
    public void SlowDownTime()
    {
        if (targetTimeScale != slowMotionScale)
        {
            targetTimeScale = slowMotionScale;
            isTransitioning = true;
            Debug.Log("[TimeFlow] Zaman yavaşlatılıyor...");
        }
    }

    /// <summary>
    /// Zamanı normale döndür (hareket/pas/şut sırasında veya top kaybedilince)
    /// </summary>
    public void NormalizeTime()
    {
        if (targetTimeScale != 1f)
        {
            targetTimeScale = 1f;
            isTransitioning = true;
            Debug.Log("[TimeFlow] Zaman normale döndürülüyor...");
        }
    }

    /// <summary>
    /// Anında zamanı ayarla (geçiş olmadan)
    /// </summary>
    public void SetTimeScaleInstant(float scale)
    {
        targetTimeScale = scale;
        currentTimeScale = scale;
        Time.timeScale = scale;
        isTransitioning = false;
    }

    /// <summary>
    /// Zaman geçişi devam ediyor mu?
    /// </summary>
    public bool IsTransitioning => isTransitioning;

    /// <summary>
    /// Şu anki zaman ölçeği
    /// </summary>
    public float CurrentTimeScale => currentTimeScale;

    private void OnDestroy()
    {
        // Cleanup: Zamanı normale döndür
        Time.timeScale = 1f;
    }
}
