using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MatchSceneUI : MonoBehaviour
{
    [Header("Skorboard UI")]
    public TextMeshProUGUI homeTeamNameText;
    public TextMeshProUGUI awayTeamNameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI ratingText;

    [Header("Saha / İstatistik UI")]
    public TextMeshProUGUI possessionText;
    public TextMeshProUGUI miniMapText;

    [Header("Alt Panel / Maç Kontrolleri")]
    public GameObject startButton;
    public GameObject simulateButton;
    public GameObject speedButton;
    public TextMeshProUGUI speedButtonText;
    public TextMeshProUGUI commentaryText;
    public GameObject objectivesButton;
    public GameObject squadButton;

    [Header("Enerji / Moral")]
    public Slider energySlider;
    public Slider moraleSlider;
    public TextMeshProUGUI energyLabelText;
    public TextMeshProUGUI moraleLabelText;

    [Header("Pozisyon UI (1. Aşama: Güç)")]
    public GameObject chancePanel;
    public MatchChanceController chanceController;

    [Header("Şut Ekranı UI (2. Aşama: Yön)")]
    public GameObject shotViewPanel;
    public ShotViewController shotViewController;
    
    [Header("Koşu Fazı (Run Phase)")]
    public GameObject runPhaseRoot;
    public RunPhaseController runPhaseController;

    // Attack UI'de kullanacağımız Şut butonu
    public GameObject attackShootButton;

    private int homeScore = 0;
    private int awayScore = 0;

    private float matchTime = 0f;   // saniye cinsinden
    private float timeScale = 1f;
    private bool isPlaying = false;

    private float nextEventTime = 0f;
    private System.Random rng;

    private float homePossession = 50f;
    private float energy = 0.85f;
    private float morale = 0.8f;

    private readonly List<string> balancedComments = new List<string>
    {
        "Orta sahada top dönüyor, iki takım da kontrollü.",
        "Takımlar birbirini tartıyor, tempo dengede.",
        "Paslaşıyoruz ama rakip de alan bırakmıyor."
    };

    private readonly List<string> homeControlComments = new List<string>
    {
        "Top daha çok bizde, kanattan boşluk arıyoruz.",
        "Topa hükmediyoruz; rakip takım kendi yarı sahasında.",
        "Sabırlı paslarla baskıyı kuruyoruz."
    };

    private readonly List<string> awayControlComments = new List<string>
    {
        "Rakip tempoyu yükseltti, topu geri kazanmamız lazım.",
        "Biraz savunmada kaldık, pas kanallarını kapatıyoruz.",
        "Rakip topa sahip, takımımız dengeli durmaya çalışıyor."
    };

    private readonly List<string> attackIntroComments = new List<string>
    {
        "Oyuncu şimdi pası aldı, ceza sahasına doğru hızlanıyor!",
        "Tehlikeli bir ara pası! Hücum oyuncumuz topla buluştu.",
        "Kanatta boşluk yakaladık, içeriye çevirmek üzereyiz.",
        "Orta sahadan dikine oynadık, savunma arkasına sızıyoruz!"
    };

    private void Start()
    {
        rng = new System.Random();

        // Takım isimleri
        string homeName = "Ev Sahibi";
        string awayName = "Rakip FC";

        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            homeName = GameManager.Instance.CurrentSave.clubName;
        }

        if (homeTeamNameText != null) homeTeamNameText.text = homeName;
        if (awayTeamNameText != null) awayTeamNameText.text = awayName;

        if (scoreText != null) scoreText.text = "0 - 0";
        if (timeText != null) timeText.text = "00:00";
        if (ratingText != null) ratingText.text = "Reyting: 82 - 80";
        if (possessionText != null) possessionText.text = "Topla oynama: %50 - %50";
        if (miniMapText != null) miniMapText.text = "Saha ortasında sakin oyun";
        if (commentaryText != null) commentaryText.text = "Maç başlamak üzere...";

        if (speedButtonText != null) speedButtonText.text = "Hız: 1x";

        RefreshEnergyUI();
        RefreshMoraleUI();

        if (chancePanel != null)
            chancePanel.SetActive(false);

        if (shotViewPanel != null)
            shotViewPanel.SetActive(false);

        ScheduleNextEvent();
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        // Zamanı ilerlet
        matchTime += Time.deltaTime * timeScale;

        UpdateBars(Time.deltaTime * timeScale);

        int totalMinutes = Mathf.FloorToInt(matchTime / 60f);
        int seconds = Mathf.FloorToInt(matchTime % 60f);

        if (timeText != null)
            timeText.text = $"{totalMinutes:00}:{seconds:00}";

        // 90. dakika sonrası maçı bitir
        if (totalMinutes >= 90)
        {
            isPlaying = false;
            if (commentaryText != null)
                commentaryText.text = "Maç bitti! Skor: " + homeScore + " - " + awayScore;
            return;
        }

        // Rastgele olay zamanı
        if (matchTime >= nextEventTime)
        {
            TriggerRandomEvent(totalMinutes);
            ScheduleNextEvent();
        }
    }

    private void ScheduleNextEvent()
    {
        // 5–15 saniye arası rastgele olay aralığı
        float delay = Random.Range(5f, 15f);
        nextEventTime = matchTime + delay;
    }

    private void TriggerRandomEvent(int currentMinute)
    {
        int roll = rng.Next(0, 100);

        UpdatePossession();
        UpdateMiniMapStatus();

        if (ShouldGivePlayerAttack())
        {
            StartPlayerChance();
            return;
        }

        // Basit gol / spiker olayları
        if (roll < 15)
        {
            homeScore++;
            UpdateScoreText();

            if (commentaryText != null)
                commentaryText.text = currentMinute + ". dakikada gol! Ev sahibi öne geçiyor!";

            morale = Mathf.Clamp01(morale + 0.05f);
            RefreshMoraleUI();
        }
        else if (roll < 25)
        {
            awayScore++;
            UpdateScoreText();

            if (commentaryText != null)
                commentaryText.text = currentMinute + ". dakikada gol! Deplasman ekibi golü buldu!";

            morale = Mathf.Clamp01(morale - 0.05f);
            RefreshMoraleUI();
        }
        else
        {
            UpdateCommentaryBasedOnPossession(currentMinute);
        }
    }

    // --- 1. AŞAMA: ÇEK-BIRAK GÜÇ EKRANI ---

    public void StartPlayerChance()
    {
        // Pozisyon başladı: attack modu aktif
        if (commentaryText != null)
            commentaryText.text = GetRandomText(attackIntroComments);

        if (miniMapText != null)
            miniMapText.text = "Ceza sahası çevresinde tehlike";

        // Maç zamanı başlangıçta duruyor, sadece hareket ederken akacak
        isPlaying = false;
        Time.timeScale = 1f;

        if (runPhaseRoot != null)
            runPhaseRoot.SetActive(true);

        if (runPhaseController != null)
            runPhaseController.BeginAttack(this);

        if (attackShootButton != null)
            attackShootButton.SetActive(true);
    }
    public void OnRunStarted()
    {
        // Oyuncu hareket ediyor → maç zamanı yavaş şekilde aksın
        isPlaying = true;
        Time.timeScale = 0.4f;
    }

    public void OnRunStopped()
    {
        // Oyuncu durdu → zaman dursun
        isPlaying = false;
        Time.timeScale = 1f;
    }

    public void OnAttackShootButton()
    {
        // Attack modunda şut başlat
        // Zamanı durdur
        isPlaying = false;
        Time.timeScale = 1f;

        // Koşu UI'lerini kapat
        if (runPhaseRoot != null)
            runPhaseRoot.SetActive(false);

        if (runPhaseController != null)
            runPhaseController.EndAttack();

        if (attackShootButton != null)
            attackShootButton.SetActive(false);

        if (commentaryText != null)
            commentaryText.text = "Şut gücünü ayarla! (Çek-bırak)";

        if (chancePanel != null && chanceController != null)
        {
            chancePanel.SetActive(true);
            chanceController.Begin(this);
        }
    }

    // MatchChanceController bittiğinde buraya powerScore gönderir
    public void OpenShotView(float powerScore)
    {
        // 1. paneli kapat
        if (chancePanel != null)
            chancePanel.SetActive(false);

        // Şut ekranını aç
        if (shotViewPanel != null)
            shotViewPanel.SetActive(true);

        if (commentaryText != null)
            commentaryText.text = "Şut ekranı: Topun neresine vurmak istiyorsan oraya dokun!";

        if (shotViewController != null)
            shotViewController.BeginShot(this, powerScore);

        // Arka planı hafif yavaşlatmak istersen (istersen koyarsın):
        Time.timeScale = 0.2f;
    }

    // --- 2. AŞAMA: ŞUT EKRANI SONUCU ---

    public void OnShotFinalized(bool isGoal)
    {
        if (shotViewPanel != null)
            shotViewPanel.SetActive(false);

        // Zamanı normale al
        Time.timeScale = 1f;

        isPlaying = true; // Maça devam

        if (isGoal)
        {
            homeScore++;
            if (scoreText != null)
                scoreText.text = $"{homeScore} - {awayScore}";

            if (commentaryText != null)
                commentaryText.text += " GOOOOOL!";
        }
        else
        {
            if (commentaryText != null)
                commentaryText.text += " ancak sonuç alamadın...";
        }
        // Attack tamamen kapansın
        if (runPhaseRoot != null)
            runPhaseRoot.SetActive(false);

        if (attackShootButton != null)
            attackShootButton.SetActive(false);

    }

    // --- BUTONLAR ---

    public void OnStartMatchButton()
    {
        isPlaying = true;
        Time.timeScale = 1f;

        if (commentaryText != null)
            commentaryText.text = "Maç başladı!";

        if (startButton != null)
            startButton.SetActive(false);
    }

    public void OnToggleSpeedButton()
    {
        if (timeScale == 1f)
            timeScale = 2f;
        else
            timeScale = 1f;

        if (speedButtonText != null)
            speedButtonText.text = $"Hız: {timeScale}x";
    }

    public void OnSkipMatchButton()
    {
        isPlaying = false;

        if (commentaryText != null)
            commentaryText.text = "Maç simüle edildi. Skor: " + homeScore + " - " + awayScore;

        // Şimdilik direkt CareerHub sahnesine dönüyoruz
        UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
    }

    public void OnObjectivesButton()
    {
        if (commentaryText != null)
            commentaryText.text = "Hedefler sekmesi daha sonra detaylandırılacak.";
    }

    public void OnSquadButton()
    {
        if (commentaryText != null)
            commentaryText.text = "Kadro ekranı maç öncesi düzenlemeler için hazırlanıyor.";
    }

    public void OnSimulateMatchButton()
    {
        OnSkipMatchButton();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{homeScore} - {awayScore}";
    }

    private void UpdateBars(float deltaTime)
    {
        energy = Mathf.Clamp01(energy - deltaTime * 0.002f);
        RefreshEnergyUI();
    }

    private void RefreshEnergyUI()
    {
        if (energySlider != null)
            energySlider.value = energy;

        if (energyLabelText != null)
            energyLabelText.text = $"Enerji: {(int)(energy * 100f)}";
    }

    private void RefreshMoraleUI()
    {
        if (moraleSlider != null)
            moraleSlider.value = morale;

        if (moraleLabelText != null)
            moraleLabelText.text = $"Moral: {(int)(morale * 100f)}";
    }

    private void UpdatePossession()
    {
        float possessionDrift = Random.Range(-5f, 5f);
        homePossession = Mathf.Clamp(homePossession + possessionDrift, 35f, 65f);
        float awayPossession = 100f - homePossession;

        if (possessionText != null)
            possessionText.text = $"Topla oynama: %{Mathf.RoundToInt(homePossession)} - %{Mathf.RoundToInt(awayPossession)}";
    }

    private void UpdateMiniMapStatus()
    {
        string[] zones =
        {
            "Saha ortasında sakin oyun",
            "Sağ kanatta pres yapıyoruz",
            "Sol çizgide topu taşıyoruz",
            "Ceza sahası çevresinde baskı",
            "Rakip kontra için hazırlanıyor"
        };

        if (miniMapText != null)
            miniMapText.text = zones[rng.Next(0, zones.Length)];
    }

    private void UpdateCommentaryBasedOnPossession(int currentMinute)
    {
        List<string> pool = balancedComments;

        if (homePossession > 55f)
            pool = homeControlComments;
        else if (homePossession < 45f)
            pool = awayControlComments;

        string text = GetRandomText(pool);

        if (commentaryText != null)
            commentaryText.text = currentMinute + ". dakika: " + text;
    }

    private bool ShouldGivePlayerAttack()
    {
        float possessionBonus = Mathf.Clamp01((homePossession - 50f) / 50f) * 0.2f;
        float staminaFactor = energy * 0.25f;
        float moraleFactor = morale * 0.25f;
        float relationshipBoost = 0.05f; // ileride ilişkiler sistemi eklenecek

        float chance = 0.18f + possessionBonus + staminaFactor + moraleFactor + relationshipBoost;
        chance = Mathf.Clamp01(chance);

        return rng.NextDouble() < chance;
    }

    private string GetRandomText(List<string> pool)
    {
        if (pool == null || pool.Count == 0)
            return string.Empty;

        int index = rng.Next(0, pool.Count);
        return pool[index];
    }

}
