using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Home Panel UI - Career Hub ana sayfa paneli
/// </summary>
public class HomePanelUI : MonoBehaviour
{
    [Header("Bilgiler")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI nextMatchText;
    public Button playMatchButton;

    private void Start()
    {
        SetupButtons();
        RefreshUI();
    }

    private void SetupButtons()
    {
        if (playMatchButton != null)
            playMatchButton.onClick.AddListener(OnPlayMatchButton);
    }

    private void RefreshUI()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            if (welcomeText != null)
                welcomeText.text = "Hoş geldiniz!";
            return;
        }

        SaveData save = GameManager.Instance.CurrentSave;
        if (save == null) return;

        // Hoş geldin mesajı
        if (welcomeText != null)
        {
            welcomeText.text = $"Hoş geldin, {save.playerProfile?.playerName ?? "Oyuncu"}!";
        }

        // Sonraki maç bilgisi (TODO: SeasonCalendarSystem'den gelecek)
        if (nextMatchText != null)
        {
            nextMatchText.text = "Sonraki maç: Henüz belirlenmedi";
        }
    }

    private void OnPlayMatchButton()
    {
        // PreMatch'e geç
        SceneFlow.LoadPreMatch();
    }
}
