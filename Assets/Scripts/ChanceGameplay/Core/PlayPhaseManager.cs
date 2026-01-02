using UnityEngine;
using System;

/// <summary>
/// Play Phase Manager - Pozisyon fazı yönetimi (Clear kuralı uygulanır)
/// </summary>
public class PlayPhaseManager : MonoBehaviour
{
    public static PlayPhaseManager Instance { get; private set; }

    public enum PhaseType
    {
        AttackChance,   // Atak pozisyonu
        DefenseChance   // Savunma pozisyonu
    }

    public enum EndReason
    {
        Goal,           // Gol
        Clear,          // Clear (top kaybı/kazanımı)
        Miss,           // Kaçan fırsat
        Timeout,        // Süre doldu
        Shot            // Şut
    }

    public enum MatchChanceResult
    {
        Goal,           // Gol
        ClearSuccess,   // Clear başarılı (savunma)
        Miss,           // Kaçan fırsat (atak)
        Timeout         // Süre doldu
    }

    public PhaseType CurrentPhase { get; private set; }
    public bool IsPhaseActive { get; private set; }

    private PossessionManager possessionManager;
    private float phaseStartTime;
    private float phaseTimeout = 20f; // 20 saniye timeout

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
        possessionManager = PossessionManager.Instance;
        if (possessionManager != null)
        {
            possessionManager.OnPossessionChanged += OnPossessionChanged;
        }
    }

    private void Update()
    {
        if (IsPhaseActive)
        {
            // Timeout kontrolü
            if (Time.time - phaseStartTime > phaseTimeout)
            {
                Debug.Log("[PlayPhaseManager] Timeout!");
                EndPhase(EndReason.Timeout, MatchChanceResult.Timeout);
            }
        }
    }

    /// <summary>
    /// Fazı başlat
    /// </summary>
    public void StartPhase(MatchContext.ChanceData chanceData)
    {
        if (MatchContext.Instance == null)
        {
            Debug.LogError("[PlayPhaseManager] MatchContext is null!");
            return;
        }

        CurrentPhase = chanceData.chanceType == MatchContext.ChanceData.ChanceType.Attack
            ? PhaseType.AttackChance
            : PhaseType.DefenseChance;

        IsPhaseActive = true;
        phaseStartTime = Time.time;

        Debug.Log($"[PlayPhaseManager] Phase started: {CurrentPhase}");
    }

    /// <summary>
    /// Possession değişikliği callback - CLEAR KURALI
    /// </summary>
    private void OnPossessionChanged(PossessionManager.Team team, GameObject player)
    {
        if (!IsPhaseActive) return;
        
        // Sadece Controlled state'te clear kuralı geçerli
        if (possessionManager.CurrentBallState != PossessionManager.BallState.Controlled)
            return;

        // CLEAR KURALI:
        // AttackChance: top rakibe geçerse -> rakip clear yapacak -> pozisyon biter (Miss)
        if (CurrentPhase == PhaseType.AttackChance && team == PossessionManager.Team.Away)
        {
            Debug.Log("[PlayPhaseManager] Attack chance: ball lost, opponent will clear");
            EndPhase(EndReason.Clear, MatchChanceResult.Miss);
        }
        // DefenseChance: top bize geçerse -> biz clear yapacağız -> pozisyon biter (ClearSuccess)
        else if (CurrentPhase == PhaseType.DefenseChance && team == PossessionManager.Team.Home)
        {
            Debug.Log("[PlayPhaseManager] Defense chance: ball won, we will clear");
            EndPhase(EndReason.Clear, MatchChanceResult.ClearSuccess);
        }
    }

    /// <summary>
    /// Fazı bitir
    /// </summary>
    public void EndPhase(EndReason reason, MatchChanceResult result)
    {
        if (!IsPhaseActive) return;

        IsPhaseActive = false;

        Debug.Log($"[PlayPhaseManager] Phase ended: {reason}, Result: {result}");

        // MatchContext'i güncelle
        UpdateMatchStats(result);

        // MatchSim'e geri dön
        SceneFlow.LoadMatchSim();
    }

    /// <summary>
    /// Maç istatistiklerini güncelle
    /// </summary>
    private void UpdateMatchStats(MatchChanceResult result)
    {
        if (MatchContext.Instance == null) return;

        MatchContext context = MatchContext.Instance;

        switch (result)
        {
            case MatchChanceResult.Goal:
                context.homeScore++;
                context.playerGoals++;
                context.playerMatchRating = Mathf.Min(10f, context.playerMatchRating + 1.0f);
                context.playerMoral = Mathf.Min(100f, context.playerMoral + 10f);
                break;

            case MatchChanceResult.ClearSuccess:
                // Savunma pozisyonu başarılı - rating artışı
                context.playerMatchRating = Mathf.Min(10f, context.playerMatchRating + 0.3f);
                break;

            case MatchChanceResult.Miss:
                // Atak pozisyonu kaçtı - rating düşüşü
                context.playerMatchRating = Mathf.Max(0f, context.playerMatchRating - 0.2f);
                context.playerMoral = Mathf.Max(0f, context.playerMoral - 5f);
                break;

            case MatchChanceResult.Timeout:
                // Timeout - hafif rating düşüşü
                context.playerMatchRating = Mathf.Max(0f, context.playerMatchRating - 0.1f);
                break;
        }

        context.playerShots++;
    }

    private void OnDestroy()
    {
        if (possessionManager != null)
        {
            possessionManager.OnPossessionChanged -= OnPossessionChanged;
        }
    }
}

