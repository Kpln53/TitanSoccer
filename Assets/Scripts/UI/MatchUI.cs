using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç UI - Maç ekranı (basit placeholder)
/// </summary>
public class MatchUI : MonoBehaviour
{
    [Header("Skor")]
    public TextMeshProUGUI homeScoreText;
    public TextMeshProUGUI awayScoreText;
    public TextMeshProUGUI minuteText;

    [Header("Butonlar")]
    public Button pauseButton;
    public Button quitMatchButton;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButton);

        if (quitMatchButton != null)
            quitMatchButton.onClick.AddListener(OnQuitMatchButton);
    }

    private void OnPauseButton()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isPaused)
                GameManager.Instance.ResumeGame();
            else
                GameManager.Instance.PauseGame();
        }
    }

    private void OnQuitMatchButton()
    {
        // Maçı bitir ve PostMatch'e geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.PostMatch);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("PostMatch");
        }
    }
}

