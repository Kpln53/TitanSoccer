using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ana menü UI - Ana menü ekranı
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Butonlar")]
    public Button careerButton;
    public Button exhibitionButton;
    public Button trainingButton;
    public Button dataPacksButton;
    public Button settingsButton;
    public Button quitButton;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (careerButton != null)
            careerButton.onClick.AddListener(OnCareerButton);

        if (exhibitionButton != null)
            exhibitionButton.onClick.AddListener(OnExhibitionButton);

        if (trainingButton != null)
            trainingButton.onClick.AddListener(OnTrainingButton);

        if (dataPacksButton != null)
            dataPacksButton.onClick.AddListener(OnDataPacksButton);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsButton);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButton);
    }

    private void OnCareerButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.SaveSlots);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SaveSlots");
        }
    }

    private void OnExhibitionButton()
    {
        Debug.Log("[MainMenuUI] Exhibition mode selected (Not implemented yet)");
        // Exhibition modu gelecekte eklenecek
    }

    private void OnTrainingButton()
    {
        Debug.Log("[MainMenuUI] Training mode selected (Not implemented yet)");
        // Training modu gelecekte eklenecek
    }

    private void OnDataPacksButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.DataPackMenu);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DataPackMenu");
        }
    }

    private void OnSettingsButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Settings);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Settings");
        }
    }

    private void OnQuitButton()
    {
        Debug.Log("[MainMenuUI] Quit button pressed");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}






