using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç sonrası UI - Maç sonrası ekran (basit placeholder)
/// </summary>
public class PostMatchUI : MonoBehaviour
{
    [Header("Maç Sonucu")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI playerRatingText;

    [Header("Butonlar")]
    public Button continueButton;

    private void Start()
    {
        SetupButtons();
        LoadMatchResult();
    }

    private void SetupButtons()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);
    }

    private void LoadMatchResult()
    {
        // TODO: Maç sonucunu yükle ve göster
        if (resultText != null)
            resultText.text = "Match Result";

        if (playerRatingText != null)
            playerRatingText.text = "Player Rating: 7.5";
    }

    private void OnContinueButton()
    {
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

