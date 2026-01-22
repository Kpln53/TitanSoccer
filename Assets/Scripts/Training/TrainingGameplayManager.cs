using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using TitanSoccer.ChanceGameplay;

public class TrainingGameplayManager : MonoBehaviour
{
    public static TrainingGameplayManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI difficultyText;
    public Button returnButton;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    public Button quitButton;

    [Header("Settings")]
    public int targetScore = 10; // Hedef skor (opsiyonel)
    
    private int score = 0;
    private int highScore = 0;
    private int difficultyLevel = 1;
    private bool isGameActive = false;

    private const string HIGH_SCORE_KEY = "TrainingHighScore";

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
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        SetupUI();
        StartTrainingSession();
    }

    private void SetupUI()
    {
        if (returnButton) returnButton.onClick.AddListener(ReturnToMenu);
        if (restartButton) restartButton.onClick.AddListener(RestartGame);
        if (quitButton) quitButton.onClick.AddListener(ReturnToMenu);
        
        if (gameOverPanel) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void StartTrainingSession()
    {
        score = 0;
        difficultyLevel = 1;
        isGameActive = true;
        UpdateUI();

        // ChanceController'ı antrenman modunda başlat
        if (ChanceController.Instance != null)
        {
            ChanceController.Instance.StartTrainingMode();
        }
        else
        {
            Debug.LogError("[TrainingManager] ChanceController not found! Make sure Bootstrap is in the scene.");
        }
    }

    public void OnGoalScored()
    {
        if (!isGameActive) return;

        score++;
        if (score % 3 == 0) IncreaseDifficulty(); // Her 3 golde bir zorluk artar
        
        UpdateUI();
        
        // Yeni pozisyon başlat
        Invoke(nameof(ResetPosition), 1.5f);
    }

    public void OnMiss()
    {
        if (!isGameActive) return;
        
        // Kaçırınca oyun biter
        GameOver();
    }

    private void ResetPosition()
    {
        if (ChanceController.Instance != null)
        {
            ChanceController.Instance.ResetTrainingPosition(difficultyLevel);
        }
    }

    private void IncreaseDifficulty()
    {
        difficultyLevel++;
        // Zorluk arttıkça kaleci güçlenir, mesafe artar vs. (ChanceController içinde işlenecek)
    }

    private void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Skor: {score} | Rekor: {highScore}";
        if (difficultyText) difficultyText.text = $"Seviye: {difficultyLevel}";
    }

    public void GameOver()
    {
        isGameActive = false;
        
        // Rekor kontrolü
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText) finalScoreText.text = $"Skor: {score}\nRekor: {highScore}";
        }
    }

    private void RestartGame()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        StartTrainingSession();
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
