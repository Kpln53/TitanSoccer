using UnityEngine;
using System.Collections;

/// <summary>
/// Match Simulation Controller - Maç simülasyonu mantığı (internal speed, no timeScale)
/// </summary>
public class MatchSimController : MonoBehaviour
{
    [Header("Simülasyon Ayarları")]
    public float simulationSpeed = 1f; // Internal speed multiplier (1x, 2x, 3x)
    public float chanceGenerationInterval = 5f; // Her 5 dakikada bir pozisyon kontrolü

    private CommentarySystem commentarySystem;
    private Coroutine simulationCoroutine;
    private bool isSimulating = false;
    private float lastMinuteUpdateTime = 0f;
    private float secondsPerMinute = 1f; // 1 gerçek saniye = 1 dakika (simSpeed'e göre değişir)

    private void Start()
    {
        commentarySystem = CommentarySystem.Instance;

        // Eğer maç devam ediyorsa (MatchChanceGameplay'den dönüldüyse), resume et
        if (MatchContext.Instance != null && MatchContext.Instance.currentMinute > 0 && MatchContext.Instance.currentMinute < 90)
        {
            ResumeMatch();
        }
        else
        {
            StartMatch();
        }
    }

    /// <summary>
    /// Yeni maç başlat
    /// </summary>
    public void StartMatch()
    {
        if (MatchContext.Instance == null)
        {
            Debug.LogError("[MatchSimController] MatchContext is null!");
            return;
        }

        // Context'i temizleme (PreMatch'ten geliyorsa zaten dolu, sadece skorları sıfırla)
        MatchContext context = MatchContext.Instance;
        context.currentMinute = 0;
        context.homeScore = 0;
        context.awayScore = 0;
        context.homePossessionPercent = 50f;
        context.awayPossessionPercent = 50f;
        context.commentaryLines.Clear();

        lastMinuteUpdateTime = Time.time;

        // Simülasyonu başlat
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }
        simulationCoroutine = StartCoroutine(SimulateMatch());
    }

    /// <summary>
    /// Maçı devam ettir (MatchChanceGameplay'den dönüldüğünde)
    /// </summary>
    public void ResumeMatch()
    {
        if (MatchContext.Instance == null)
        {
            Debug.LogError("[MatchSimController] MatchContext is null!");
            return;
        }

        Debug.Log($"[MatchSimController] Resuming match at minute {MatchContext.Instance.currentMinute}");

        lastMinuteUpdateTime = Time.time;

        // Simülasyonu devam ettir
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }
        simulationCoroutine = StartCoroutine(SimulateMatch());
    }

    /// <summary>
    /// Maç simülasyonu coroutine (internal speed kullanır, timeScale kullanmaz)
    /// </summary>
    private IEnumerator SimulateMatch()
    {
        isSimulating = true;
        MatchContext context = MatchContext.Instance;

        if (context == null)
        {
            Debug.LogError("[MatchSimController] MatchContext is null in SimulateMatch!");
            yield break;
        }

        // Maç başlangıcı
        commentarySystem?.AddImportantLine("Maç başladı!");

        // Dakika bazlı simülasyon (internal speed)
        while (context.currentMinute < 90 && isSimulating)
        {
            // Internal speed'e göre bekleme
            float actualSecondsPerMinute = secondsPerMinute / simulationSpeed;
            
            if (Time.time - lastMinuteUpdateTime >= actualSecondsPerMinute)
            {
                context.currentMinute++;
                lastMinuteUpdateTime = Time.time;

                // Possession güncelle
                UpdatePossession(context);

                // Enerji güncelle
                UpdateEnergy(context);

                // Rating güncelle
                UpdateRating(context);

                // Moral güncelle
                UpdateMoral(context);

                // Genel commentary (rastgele) - DataPack'ten gerçek oyuncu isimleri
                if (Random.Range(0f, 1f) < 0.3f && commentarySystem != null)
                {
                    string randomPlayer = GetRandomPlayerName(context, context.homePossessionPercent > 50f);
                    string[] actions = { "top sürüyor", "pasa çıkıyor", "dribling yapıyor", "oyunu yönlendiriyor", "pozisyon arıyor" };
                    commentarySystem.AddGeneralCommentary(randomPlayer, actions[Random.Range(0, actions.Length)]);
                }

                // Pozisyon üret kontrolü
                if (context.currentMinute % (int)chanceGenerationInterval == 0)
                {
                    // %30 şansla pozisyon üret
                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        GenerateChance(context);
                        yield break; // Pozisyon oynanışına geç, simülasyon durur
                    }
                }

                // Devre arası
                if (context.currentMinute == 45)
                {
                    commentarySystem?.AddImportantLine("İlk yarı bitti!");
                    yield return new WaitForSeconds(2f / simulationSpeed);
                }
            }

            yield return null; // Her frame kontrol et
        }

        // Maç bitti
        if (context.currentMinute >= 90)
        {
            commentarySystem?.AddImportantLine("Maç bitti!");
            isSimulating = false;
            SceneFlow.LoadPostMatch();
        }
    }

    /// <summary>
    /// Possession güncelle (team power bias ile)
    /// </summary>
    private void UpdatePossession(MatchContext context)
    {
        // Team power farkına göre possession drift
        float powerDiff = (context.homeTeamPower - context.awayTeamPower) / 100f;
        float change = Random.Range(-1f, 1f) + powerDiff * 0.5f;
        
        context.homePossessionPercent = Mathf.Clamp(context.homePossessionPercent + change, 0f, 100f);
        context.awayPossessionPercent = 100f - context.homePossessionPercent;
    }

    /// <summary>
    /// Enerji güncelle (dakika ilerledikçe düşer)
    /// </summary>
    private void UpdateEnergy(MatchContext context)
    {
        // Her dakika %0.5 enerji kaybı
        context.playerEnergy = Mathf.Max(0f, context.playerEnergy - 0.5f);
    }

    /// <summary>
    /// Rating güncelle (possession, gol, asist'e göre)
    /// </summary>
    private void UpdateRating(MatchContext context)
    {
        float baseRating = 5.0f;
        float possessionBonus = (context.homePossessionPercent / 100f) * 1.0f;
        float goalBonus = context.playerGoals * 1.5f;
        float assistBonus = context.playerAssists * 1.0f;

        context.playerMatchRating = Mathf.Clamp(baseRating + possessionBonus + goalBonus + assistBonus, 0f, 10f);
    }

    /// <summary>
    /// Moral güncelle (skor durumuna göre)
    /// </summary>
    private void UpdateMoral(MatchContext context)
    {
        int scoreDiff = context.homeScore - context.awayScore;
        if (scoreDiff > 0)
        {
            context.playerMoral = Mathf.Min(100f, context.playerMoral + 0.1f);
        }
        else if (scoreDiff < 0)
        {
            context.playerMoral = Mathf.Max(0f, context.playerMoral - 0.1f);
        }
    }

    /// <summary>
    /// Pozisyon üret ve MatchChanceGameplay'e geç
    /// </summary>
    private void GenerateChance(MatchContext context)
    {
        // Pozisyon tipi belirle (%70 atak, %30 savunma)
        MatchContext.ChanceData.ChanceType chanceType = Random.Range(0f, 1f) < 0.7f
            ? MatchContext.ChanceData.ChanceType.Attack
            : MatchContext.ChanceData.ChanceType.Defense;

        // Pozisyon verisi oluştur
        context.currentChance = new MatchContext.ChanceData
        {
            chanceType = chanceType,
            minute = context.currentMinute,
            successChance = Random.Range(0.3f, 0.8f),
            spawnTemplateInfo = $"Chance at {context.currentMinute}'"
        };

        commentarySystem?.AddImportantLine($"{context.currentMinute}. dakikada pozisyon!");

        // Simülasyonu durdur ve MatchChanceGameplay'e geç
        isSimulating = false;
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }
        SceneFlow.LoadChanceGameplay();
    }

    /// <summary>
    /// Simülasyon hızını ayarla
    /// </summary>
    public void SetSimSpeed(float speed)
    {
        simulationSpeed = Mathf.Clamp(speed, 0.5f, 5f);
    }

    /// <summary>
    /// Maçı sona kadar simüle et (Poisson tabanlı skip)
    /// </summary>
    public void SimulateToFullTime()
    {
        if (MatchContext.Instance == null) return;

        isSimulating = false;
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }

        MatchContext context = MatchContext.Instance;

        // Kalan dakikalar
        int remainingMinutes = 90 - context.currentMinute;
        if (remainingMinutes <= 0)
        {
            SceneFlow.LoadPostMatch();
            return;
        }

        // Poisson tabanlı gol üretimi
        float expectedGoalsTotal = 2.4f * (remainingMinutes / 90f);
        
        // Team power ve mevcut duruma göre gol dağılımı
        float powerDiff = (context.homeTeamPower - context.awayTeamPower) / 100f;
        float scoreStateBias = (context.homeScore > context.awayScore ? 0.1f : -0.1f);
        float possessionBias = (context.homePossessionPercent - 50f) / 100f * 0.1f;
        
        float homeGoalShare = 0.5f + powerDiff + scoreStateBias + possessionBias;
        homeGoalShare = Mathf.Clamp(homeGoalShare, 0.2f, 0.8f);
        
        float lambdaHome = expectedGoalsTotal * homeGoalShare;
        float lambdaAway = expectedGoalsTotal * (1f - homeGoalShare);
        
        int additionalGoalsHome = PoissonRandom(lambdaHome);
        int additionalGoalsAway = PoissonRandom(lambdaAway);

        context.homeScore += additionalGoalsHome;
        context.awayScore += additionalGoalsAway;

        // Gol commentary - DataPack'ten gerçek oyuncu isimleri
        for (int i = 0; i < additionalGoalsHome; i++)
        {
            string scorer = GetRandomPlayerName(context, true); // Home takımdan
            commentarySystem?.AddGoalCommentary(scorer);
        }
        for (int i = 0; i < additionalGoalsAway; i++)
        {
            string scorer = GetRandomPlayerName(context, false); // Away takımdan
            commentarySystem?.AddGoalCommentary(scorer);
        }

        // Enerji kalan süre kadar düş
        context.playerEnergy = Mathf.Max(0f, context.playerEnergy - (remainingMinutes * 0.5f));

        // Moral final sonuca göre
        int finalScoreDiff = context.homeScore - context.awayScore;
        if (finalScoreDiff > 0)
        {
            context.playerMoral = Mathf.Min(100f, context.playerMoral + 5f);
        }
        else if (finalScoreDiff < 0)
        {
            context.playerMoral = Mathf.Max(0f, context.playerMoral - 5f);
        }

        context.currentMinute = 90;

        // Final rating güncelle
        UpdateRating(context);

        commentarySystem?.AddImportantLine("Maç bitti! Final düdük!");

        // PostMatch'e geç
        SceneFlow.LoadPostMatch();
    }

    /// <summary>
    /// Poisson dağılımı için rastgele sayı üret
    /// </summary>
    private int PoissonRandom(float lambda)
    {
        if (lambda <= 0) return 0;
        
        int k = 0;
        float p = 1f;
        float L = Mathf.Exp(-lambda);

        do
        {
            k++;
            p *= Random.Range(0f, 1f);
        } while (p > L);

        return k - 1;
    }

    /// <summary>
    /// DataPack'ten veya MatchContext'ten rastgele oyuncu ismi al
    /// </summary>
    private string GetRandomPlayerName(MatchContext context, bool isHomeTeam)
    {
        string teamName = isHomeTeam ? context.homeTeamName : context.awayTeamName;
        
        // Önce MatchContext'teki kadroya bak
        var squad = isHomeTeam ? context.homeSquad : context.awaySquad;
        if (squad != null && squad.Count > 0)
        {
            // İlk 11'den öncelikli seç
            var starters = squad.FindAll(p => p.isStartingXI);
            if (starters.Count > 0)
            {
                return starters[Random.Range(0, starters.Count)].playerName;
            }
            return squad[Random.Range(0, squad.Count)].playerName;
        }
        
        // MatchContext'te yoksa DataPack'e bak
        if (DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            TeamData team = DataPackManager.Instance.GetTeam(teamName);
            if (team != null && team.players != null && team.players.Count > 0)
            {
                // Rastgele oyuncu seç (ilk 11'den öncelikli)
                int playerIndex = Random.Range(0, Mathf.Min(11, team.players.Count));
                return team.players[playerIndex].playerName;
            }
        }
        
        // Hiç bulunamazsa varsayılan
        return isHomeTeam ? "Ev Sahibi Oyuncu" : "Deplasman Oyuncusu";
    }
}
