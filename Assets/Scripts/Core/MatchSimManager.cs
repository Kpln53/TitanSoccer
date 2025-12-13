using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSimManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MatchDashboardUI ui;

    [Header("Commentary")]
    [SerializeField] private CommentaryBank commentaryBank;
    [SerializeField] private float commentaryInterval = 5f; // saniye

    [Header("Match Settings")]
    [SerializeField] private float totalMinutes = 90f;
    [SerializeField] private float secondsPerMatchMinute = 3f; // 3 sn ger�ek zaman = 1 dk ma�
    [SerializeField] private float baseChancePerMinute = 0.08f; // dakika ba��na temel pozisyon ihtimali (%8)

    [Header("State (debug)")]
    [SerializeField] private float currentMinute;
    [SerializeField, Range(0, 100)] private float homePossession = 50f;
    [SerializeField] private float playerEnergy = 1f;
    [SerializeField] private float playerMorale = 1f;
    [SerializeField] private float playerRating = 6.0f;
    [SerializeField] private int homeScore = 0;
    [SerializeField] private int awayScore = 0;

    bool _isRunning;
    bool _isFastSpeed;
    float _commentaryTimer;

    void Start()
    {
        if (!ui)
            ui = FindObjectOfType<MatchDashboardUI>();

        // SaveData'dan başlangıç değerlerini al (eğer varsa)
        InitializeFromSaveData();

        if (ui)
        {
            ui.SetTeams("Ev", "Deplasman");
            ui.SetScore(0, 0);
            ui.SetMinute(0);
            ui.SetPossession(homePossession, 100f - homePossession);
            ui.SetEnergy(playerEnergy);
            ui.SetMorale(playerMorale);
            ui.SetCommentary("Ma� ba�lamak �zere.");

            ui.OnStartClicked += HandleStartClicked;
            ui.OnSpeedToggled += HandleSpeedToggled;
            ui.OnSimulateClicked += HandleSimulateClicked;
            // Objectives & Lineup butonlar� ileride kullan�lacak.
        }
        // MatchSimManager.Start() i�ine (ui set edildikten sonra)
        if (MatchContext.I != null && MatchContext.I.hasPendingResult)
        {
            ApplyChanceResult(MatchContext.I.lastOutcome);
            MatchContext.I.ClearOutcome();
        }

    }

    void InitializeFromSaveData()
    {
        // Eğer MatchContext'te devam eden bir maç varsa, oradan değerleri al
        if (MatchContext.I != null)
        {
            homeScore = MatchContext.I.homeScore;
            awayScore = MatchContext.I.awayScore;
            currentMinute = MatchContext.I.minute;
            homePossession = MatchContext.I.homePossession;
            playerEnergy = MatchContext.I.energy01;
            playerMorale = MatchContext.I.morale01;
            playerRating = MatchContext.I.rating;
        }
        // Yoksa SaveData'dan veya varsayılan değerlerden başla
        else if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            // Başlangıç enerji ve moral değerleri (ileride SaveData'ya eklenebilir)
            playerEnergy = 1f;
            playerMorale = 1f;
            playerRating = 6.0f;
        }
    }

    void OnDestroy()
    {
        if (ui)
        {
            ui.OnStartClicked -= HandleStartClicked;
            ui.OnSpeedToggled -= HandleSpeedToggled;
            ui.OnSimulateClicked -= HandleSimulateClicked;
        }
    }

    void Update()
    {
        if (!_isRunning) return;

        float speedFactor = _isFastSpeed ? 2f : 1f;
        float deltaMatchMinutes = (Time.deltaTime * speedFactor) / secondsPerMatchMinute;
        currentMinute = Mathf.Min(currentMinute + deltaMatchMinutes, totalMinutes);

        // Dakika UI
        if (ui)
        {
            ui.SetMinute(currentMinute);
            ui.SetEnergy(playerEnergy);
            ui.SetMorale(playerMorale);
            ui.SetPlayerRating(playerRating);
            ui.SetScore(homeScore, awayScore);
        }

        // Topla oynama hafif random y�r�y��
        SimulatePossession(deltaMatchMinutes);

        // Spiker
        _commentaryTimer -= Time.deltaTime * speedFactor;
        if (_commentaryTimer <= 0f)
        {
            _commentaryTimer = commentaryInterval;
            EmitRandomCommentary();
        }

        // Pozisyon gelme
        TrySpawnChance(deltaMatchMinutes);

        if (currentMinute >= totalMinutes)
        {
            EndMatch();
        }
    }

    void HandleStartClicked()
    {
        _isRunning = true;
        _commentaryTimer = 0f;
        if (ui) ui.SetCommentary("Ma� ba�lad�!");
    }

    void HandleSpeedToggled(bool isFast)
    {
        _isFastSpeed = isFast;
    }

    void HandleSimulateClicked()
    {
        // Ma�� full sim�le et: s�reyi sonuna kadar at
        currentMinute = totalMinutes;
        EndMatch();
    }

    void SimulatePossession(float deltaMatchMinutes)
    {
        // K���k random kaymalar
        float drift = Random.Range(-3f, 3f) * deltaMatchMinutes;
        homePossession = Mathf.Clamp(homePossession + drift, 30f, 70f);
        if (ui)
            ui.SetPossession(homePossession, 100f - homePossession);

        // Mini harita i�in top konumu (sadece �rnek: possession'a g�re ileri/geri)
        if (ui)
        {
            float y = Mathf.Lerp(-0.3f, 0.3f, homePossession / 100f);
            float x = Random.Range(-0.2f, 0.2f);
            ui.SetBallOnMiniMap(new Vector2(x, y));
        }
    }

    void EmitRandomCommentary()
    {
        if (!commentaryBank || commentaryBank.neutralComments.Length == 0) return;

        string line = null;

        if (homePossession > 60f && commentaryBank.homeDominatingComments.Length > 0)
        {
            line = RandomFrom(commentaryBank.homeDominatingComments);
        }
        else if (homePossession < 40f && commentaryBank.awayDominatingComments.Length > 0)
        {
            line = RandomFrom(commentaryBank.awayDominatingComments);
        }
        else
        {
            line = RandomFrom(commentaryBank.neutralComments);
        }

        if (ui && !string.IsNullOrEmpty(line))
            ui.SetCommentary(line);
    }

    void TrySpawnChance(float deltaMatchMinutes)
    {
        // �leride enerji, moral, ili�kiler vs ekleyerek form�l� zenginle�tirece�iz.
        float energyFactor = Mathf.Lerp(0.5f, 1.5f, playerEnergy);
        float moraleFactor = Mathf.Lerp(0.7f, 1.3f, playerMorale);
        float possessionFactor = Mathf.Lerp(0.5f, 1.5f, (homePossession - 50f) / 50f + 0.5f);

        float chancePerMinute = baseChancePerMinute * energyFactor * moraleFactor * possessionFactor;

        float probThisStep = chancePerMinute * deltaMatchMinutes;
        if (Random.value < probThisStep)
        {
            SpawnChance();
        }
    }
    void ApplyChanceResult(ChanceOutcome outcome)
    {
        // Basit �rnek: iyi/k�t� yorum + rating/enerji/moral etkisi
        switch (outcome)
        {
            case ChanceOutcome.Goal:
                homeScore++;
                playerRating = Mathf.Clamp(playerRating + 0.3f, 0f, 10f);
                playerMorale = Mathf.Clamp01(playerMorale + 0.12f);
                if (ui && commentaryBank && commentaryBank.goodOutcomeComments.Length > 0)
                    ui.SetCommentary(RandomFrom(commentaryBank.goodOutcomeComments));
                break;

            case ChanceOutcome.Assist:
                playerRating = Mathf.Clamp(playerRating + 0.2f, 0f, 10f);
                playerMorale = Mathf.Clamp01(playerMorale + 0.06f);
                if (ui && commentaryBank && commentaryBank.goodOutcomeComments.Length > 0)
                    ui.SetCommentary(RandomFrom(commentaryBank.goodOutcomeComments));
                break;

            case ChanceOutcome.ShotMiss:
                playerRating = Mathf.Clamp(playerRating - 0.1f, 0f, 10f);
                playerMorale = Mathf.Clamp01(playerMorale - 0.05f);
                if (ui && commentaryBank && commentaryBank.badOutcomeComments.Length > 0)
                    ui.SetCommentary(RandomFrom(commentaryBank.badOutcomeComments));
                break;

            case ChanceOutcome.Turnover:
                playerRating = Mathf.Clamp(playerRating - 0.15f, 0f, 10f);
                playerMorale = Mathf.Clamp01(playerMorale - 0.08f);
                if (ui && commentaryBank && commentaryBank.badOutcomeComments.Length > 0)
                    ui.SetCommentary(RandomFrom(commentaryBank.badOutcomeComments));
                break;
        }

        // Enerji her pozisyonda biraz d��s�n
        playerEnergy = Mathf.Clamp01(playerEnergy - 0.03f);

        // UI refresh
        if (ui)
        {
            ui.SetEnergy(playerEnergy);
            ui.SetMorale(playerMorale);
            ui.SetPlayerRating(playerRating);
            ui.SetScore(homeScore, awayScore);
            ui.SetPossession(homePossession, 100f - homePossession);
        }

        // tekrar ak�� devam
        _isRunning = true;
    }

    void SpawnChance()
    {
        _isRunning = false;

        // Context'e state yaz
        if (MatchContext.I != null)
        {
            MatchContext.I.minute = currentMinute;
            MatchContext.I.homeScore = 0;  // sende skor de�i�kenleri varsa onlar� yaz
            MatchContext.I.awayScore = awayScore;
            MatchContext.I.homePossession = homePossession;
            MatchContext.I.energy01 = playerEnergy;
            MatchContext.I.morale01 = playerMorale;
            MatchContext.I.rating = playerRating;
            MatchContext.I.ClearOutcome();
        }

        if (ui && commentaryBank && commentaryBank.chanceStartComments.Length > 0)
            ui.SetCommentary(RandomFrom(commentaryBank.chanceStartComments));

        SceneManager.LoadScene("MatchChanceScene");
    }

    void EndMatch()
    {
        _isRunning = false;
        if (ui)
            ui.SetCommentary("Ma� sona erdi.");
        Debug.Log("Ma� bitti. Buradan sonu� ekran�na gidebilirsin.");
    }

    string RandomFrom(string[] arr)
    {
        if (arr == null || arr.Length == 0) return "";
        return arr[Random.Range(0, arr.Length)];
    }
}
