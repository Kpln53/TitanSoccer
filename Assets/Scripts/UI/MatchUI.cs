using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç Ekranı UI - Maç simülasyon ekranı (GDB'ye göre)
/// </summary>
public class MatchUI : MonoBehaviour
{
    [Header("Skorboard (Üstte)")]
    public TextMeshProUGUI minuteText;           // Dakika text'i
    public TextMeshProUGUI homeScoreText;        // Ev sahibi takım skoru
    public TextMeshProUGUI awayScoreText;        // Deplasman takım skoru
    public TextMeshProUGUI playerRatingText;     // Oyuncu reytingi

    [Header("Saha (Ortada)")]
    public GameObject fieldPanel;                // Saha paneli (mini harita için)

    [Header("Topla Oynama")]
    public TextMeshProUGUI ballPossessionText;   // Topla oynama yüzdesi ("TOPLA OYNAMA" butonu içindeki text)

    [Header("Durum Barları")]
    public Slider energySlider;                  // Enerji slider'ı (veya TextMeshProUGUI)
    public TextMeshProUGUI energyText;           // Enerji text'i ("ENERJİ" butonu içindeki text)
    public Slider moraleSlider;                  // Moral slider'ı (veya TextMeshProUGUI)
    public TextMeshProUGUI moraleText;           // Moral text'i ("MORAL" butonu içindeki text)

    [Header("Spiker Alanı")]
    public TextMeshProUGUI commentatorText;      // Spiker metin alanı ("SpikerAlanı" içindeki text)

    [Header("Kontrol Butonları (Alt Bar)")]
    public Button squadButton;                   // "KADRO" butonu
    public Button speed1xButton;                 // "1x" hız butonu
    public Button speed2xButton;                 // "2x" hız butonu
    public Button speed3xButton;                 // "3x" hız butonu
    public Button simulationButton;              // "SİMÜLASYON" butonu

    [Header("Diğer Butonlar")]
    public Button pauseButton;                   // Duraklat butonu (isteğe bağlı)
    public Button quitMatchButton;               // Maçtan çık butonu (isteğe bağlı)

    private int currentMatchSpeed = 1; // 1x, 2x, 3x
    private bool isPaused = false;
    private MatchData currentMatch;

    private void Start()
    {
        // MatchSimulationSystem'i oluştur (yoksa)
        if (MatchSimulationSystem.Instance == null)
        {
            GameObject matchSimObj = new GameObject("MatchSimulationSystem");
            matchSimObj.AddComponent<MatchSimulationSystem>();
        }

        SetupButtons();
        InitializeUI();
        SubscribeToMatchEvents();
        LoadMatchData();
        
        // Maçı otomatik başlat
        StartMatchSimulation();
    }
    
    /// <summary>
    /// Maç simülasyonunu başlat
    /// </summary>
    private void StartMatchSimulation()
    {
        if (currentMatch == null)
        {
            Debug.LogWarning("[MatchUI] Cannot start match: no match data!");
            return;
        }
        
        if (MatchSimulationSystem.Instance != null)
        {
            Debug.Log("[MatchUI] Starting match simulation...");
            MatchSimulationSystem.Instance.StartMatch(currentMatch);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromMatchEvents();
    }

    /// <summary>
    /// Maç verilerini yükle ve simülasyonu başlat
    /// </summary>
    private void LoadMatchData()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("[MatchUI] No save data available!");
            return;
        }

        // Bir sonraki maçı al
        currentMatch = SeasonCalendarSystem.Instance?.GetNextMatch(GameManager.Instance.CurrentSave);
        if (currentMatch == null)
        {
            Debug.LogError("[MatchUI] No match data available!");
            return;
        }

        // Oyuncunun takımına göre pozisyonu hesapla
        SaveData saveData = GameManager.Instance.CurrentSave;
        string playerTeamName = saveData.clubData.clubName;
        bool isPlayerHome = currentMatch.homeTeam == playerTeamName;

        // UI'ı güncelle
        UpdateScore(0, 0);
        UpdateMinute(0);
        UpdatePlayerRating(6.5f);
        
        // Enerji ve moral değerlerini yükle
        if (saveData.playerProfile != null)
        {
            UpdateEnergy(saveData.playerProfile.energy);
            UpdateMorale(saveData.playerProfile.moral);
        }

        // Maç başlangıç yorumu
        if (CommentatorSystem.Instance != null)
        {
            string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.MatchStart);
            UpdateCommentator(commentary);
        }

        Debug.Log($"[MatchUI] Match loaded: {currentMatch.homeTeam} vs {currentMatch.awayTeam}");
    }

    /// <summary>
    /// MatchSimulationSystem event'lerine abone ol
    /// </summary>
    private void SubscribeToMatchEvents()
    {
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.OnScoreChanged += HandleScoreChanged;
            MatchSimulationSystem.Instance.OnMinuteChanged += HandleMinuteChanged;
            MatchSimulationSystem.Instance.OnPlayerRatingChanged += HandlePlayerRatingChanged;
            MatchSimulationSystem.Instance.OnBallPossessionChanged += HandleBallPossessionChanged;
            MatchSimulationSystem.Instance.OnCommentaryChanged += HandleCommentaryChanged;
            MatchSimulationSystem.Instance.OnMatchFinished += HandleMatchFinished;
            MatchSimulationSystem.Instance.OnPlayerChance += HandlePlayerChance;
        }
    }

    /// <summary>
    /// MatchSimulationSystem event'lerinden abonelikten çık
    /// </summary>
    private void UnsubscribeFromMatchEvents()
    {
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.OnScoreChanged -= HandleScoreChanged;
            MatchSimulationSystem.Instance.OnMinuteChanged -= HandleMinuteChanged;
            MatchSimulationSystem.Instance.OnPlayerRatingChanged -= HandlePlayerRatingChanged;
            MatchSimulationSystem.Instance.OnBallPossessionChanged -= HandleBallPossessionChanged;
            MatchSimulationSystem.Instance.OnCommentaryChanged -= HandleCommentaryChanged;
            MatchSimulationSystem.Instance.OnMatchFinished -= HandleMatchFinished;
            MatchSimulationSystem.Instance.OnPlayerChance -= HandlePlayerChance;
        }
    }

    // Event handlers
    private void HandleScoreChanged(int homeScore, int awayScore)
    {
        UpdateScore(homeScore, awayScore);
    }

    private void HandleMinuteChanged(int minute)
    {
        UpdateMinute(minute);
    }

    private void HandlePlayerRatingChanged(float rating)
    {
        UpdatePlayerRating(rating);
    }

    private void HandleBallPossessionChanged(int homePossession)
    {
        UpdateBallPossession(homePossession);
    }

    private void HandleCommentaryChanged(string commentary)
    {
        UpdateCommentator(commentary);
    }

    private void HandlePlayerChance(MatchChanceData chance)
    {
        Debug.Log($"[MatchUI] Player chance occurred: {chance.chanceType} at minute {chance.minute}");
        
        // MatchChance ekranına git
        // MatchChanceUI'a chance data'yı aktaracağız (GameManager veya static field ile)
        MatchChanceManager.CurrentChance = chance;
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.MatchChance);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MatchChanceScene");
        }
    }

    private void HandleMatchFinished()
    {
        Debug.Log("[MatchUI] Match finished!");
        
        // Maç sonu - PostMatch ekranına yönlendir
        // Önce kaydı güncelle
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            SaveData saveData = GameManager.Instance.CurrentSave;
            saveData.UpdateSaveDate();
            SaveSystem.SaveGame(saveData, GameManager.Instance.CurrentSaveSlotIndex);
        }

        // PostMatch ekranına git
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.PostMatch);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("PostMatch");
        }
    }

    private void SetupButtons()
    {
        if (squadButton != null)
            squadButton.onClick.AddListener(OnSquadButton);

        if (speed1xButton != null)
            speed1xButton.onClick.AddListener(() => SetMatchSpeed(1));

        if (speed2xButton != null)
            speed2xButton.onClick.AddListener(() => SetMatchSpeed(2));

        if (speed3xButton != null)
            speed3xButton.onClick.AddListener(() => SetMatchSpeed(3));

        if (simulationButton != null)
            simulationButton.onClick.AddListener(OnSimulationButton);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButton);

        if (quitMatchButton != null)
            quitMatchButton.onClick.AddListener(OnQuitMatchButton);
    }

    /// <summary>
    /// UI'ı başlat
    /// </summary>
    private void InitializeUI()
    {
        // Varsayılan değerler (LoadMatchData() bunları güncelleyecek)
        UpdateScore(0, 0);
        UpdateMinute(0);
        UpdatePlayerRating(6.5f);
        UpdateBallPossession(50);
    }

    /// <summary>
    /// Skoru güncelle
    /// </summary>
    public void UpdateScore(int homeScore, int awayScore)
    {
        if (homeScoreText != null)
            homeScoreText.text = homeScore.ToString();

        if (awayScoreText != null)
            awayScoreText.text = awayScore.ToString();
    }

    /// <summary>
    /// Dakikayı güncelle
    /// </summary>
    public void UpdateMinute(int minute)
    {
        if (minuteText != null)
            minuteText.text = minute.ToString();
    }

    /// <summary>
    /// Oyuncu reytingini güncelle
    /// </summary>
    public void UpdatePlayerRating(float rating)
    {
        if (playerRatingText != null)
            playerRatingText.text = rating.ToString("F1");
    }

    /// <summary>
    /// Topla oynama yüzdesini güncelle
    /// </summary>
    public void UpdateBallPossession(int percentage)
    {
        if (ballPossessionText != null)
            ballPossessionText.text = $"%{percentage}";
    }

    /// <summary>
    /// Enerjiyi güncelle
    /// </summary>
    public void UpdateEnergy(float energy)
    {
        if (energySlider != null)
            energySlider.value = energy / 100f; // 0-1 arası

        if (energyText != null)
            energyText.text = $"{(int)energy}%";
    }

    /// <summary>
    /// Morali güncelle
    /// </summary>
    public void UpdateMorale(float morale)
    {
        if (moraleSlider != null)
            moraleSlider.value = morale / 100f; // 0-1 arası

        if (moraleText != null)
            moraleText.text = $"{(int)morale}%";
    }

    /// <summary>
    /// Spiker metnini güncelle
    /// </summary>
    public void UpdateCommentator(string text)
    {
        if (commentatorText != null)
            commentatorText.text = text;
    }

    /// <summary>
    /// Maç hızını ayarla
    /// </summary>
    private void SetMatchSpeed(int speed)
    {
        currentMatchSpeed = speed;
        
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.SetSimulationSpeed(speed);
        }
        
        Debug.Log($"[MatchUI] Match speed set to {speed}x");
    }

    /// <summary>
    /// Kadro butonuna tıklandığında
    /// </summary>
    private void OnSquadButton()
    {
        // TODO: Kadro ekranını aç (modal panel)
        Debug.Log("[MatchUI] Squad button clicked");
    }

    /// <summary>
    /// Simülasyon butonuna tıklandığında (hızlı simülasyon)
    /// </summary>
    private void OnSimulationButton()
    {
        if (currentMatch == null)
        {
            Debug.LogWarning("[MatchUI] No match data to simulate!");
            return;
        }

        Debug.Log("[MatchUI] Simulation button clicked - Starting instant simulation");
        
        // Hızlı simülasyon (UI olmadan)
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.SimulateMatchInstant(currentMatch);
            
            // Maç sonu - PostMatch ekranına yönlendir
            HandleMatchFinished();
        }
    }

    /// <summary>
    /// Duraklat butonuna tıklandığında
    /// </summary>
    private void OnPauseButton()
    {
        isPaused = !isPaused;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPaused(isPaused);
        }
        
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.SetPaused(isPaused);
        }
        
        Debug.Log($"[MatchUI] Match paused: {isPaused}");
    }

    /// <summary>
    /// Maçtan çık butonuna tıklandığında
    /// </summary>
    private void OnQuitMatchButton()
    {
        // Simülasyonu durdur
        if (MatchSimulationSystem.Instance != null)
        {
            MatchSimulationSystem.Instance.StopMatch();
        }
        
        // Time scale'i sıfırla
        Time.timeScale = 1f;
        
        // CareerHub'a dön
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }
}
