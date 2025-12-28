using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç öncesi UI - Maç öncesi ekran (basit placeholder)
/// </summary>
public class MatchPreUI : MonoBehaviour
{
    [Header("Maç Bilgileri")]
    public TextMeshProUGUI homeTeamText;
    public TextMeshProUGUI awayTeamText;
    public TextMeshProUGUI matchDateText;

    [Header("Butonlar")]
    public Button startMatchButton;
    public Button backButton;

    private void Start()
    {
        SetupButtons();
        LoadMatchData();
    }

    private void SetupButtons()
    {
        if (startMatchButton != null)
            startMatchButton.onClick.AddListener(OnStartMatchButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    private void LoadMatchData()
    {
        // TODO: Maç verilerini yükle (SeasonCalendarSystem'den)
        if (homeTeamText != null)
            homeTeamText.text = "Home Team";

        if (awayTeamText != null)
            awayTeamText.text = "Away Team";

        if (matchDateText != null)
            matchDateText.text = "Match Date";
    }

    private void OnStartMatchButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Match);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MatchScene");
        }
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToPreviousState();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }
}

