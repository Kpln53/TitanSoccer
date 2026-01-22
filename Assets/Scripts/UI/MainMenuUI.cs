using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu References")]
    public RectTransform mainMenuPanel;
    public Button careerButton;
    public Button exhibitionButton;
    public Button trainingButton;
    public Button dataPacksButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Save Menu References")]
    public RectTransform saveMenuPanel;
    public Button saveMenuBackButton;

    [Header("Settings Menu References")]
    public RectTransform settingsMenuPanel;
    public Button settingsBackButton;
    
    // Animasyon ayarları
    private float slideDuration = 0.4f;
    private Vector2 offScreenRight;
    private Vector2 centerScreen = Vector2.zero;

    private void Start()
    {
        SetupButtons();
        
        // Ekran genişliğine göre pozisyonları ayarla
        offScreenRight = new Vector2(Screen.width, 0);
        
        // Save menüsünü başlangıçta sağa gizle
        if (saveMenuPanel != null)
        {
            saveMenuPanel.anchoredPosition = new Vector2(1920, 0); // Canvas referansına göre sağda
            saveMenuPanel.gameObject.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        if (careerButton) careerButton.onClick.AddListener(OnCareerButton);
        if (exhibitionButton) exhibitionButton.onClick.AddListener(OnExhibitionButton);
        if (trainingButton) trainingButton.onClick.AddListener(OnTrainingButton);
        if (dataPacksButton) dataPacksButton.onClick.AddListener(OnDataPacksButton);
        if (settingsButton) settingsButton.onClick.AddListener(OnSettingsButton);
        if (quitButton) quitButton.onClick.AddListener(OnQuitButton);
        
        if (saveMenuBackButton) saveMenuBackButton.onClick.AddListener(OnBackFromSaveMenu);
        if (settingsBackButton) settingsBackButton.onClick.AddListener(OnBackFromSettings);
    }

    // --- BUTTON ACTIONS ---

    private void OnCareerButton()
    {
        // Sahne yüklemek yerine paneli kaydır
        StartCoroutine(SlideMenu(saveMenuPanel, true));
        
        // Ana menüyü sola kaydır (gizle)
        if (mainMenuPanel != null)
        {
            StartCoroutine(SlideMainPanel(false));
        }
    }

    private void OnBackFromSaveMenu()
    {
        // Geri kaydır
        StartCoroutine(SlideMenu(saveMenuPanel, false));
        
        // Ana menüyü geri getir
        if (mainMenuPanel != null)
        {
            StartCoroutine(SlideMainPanel(true));
        }
    }

    private IEnumerator SlideMainPanel(bool slideIn)
    {
        float timer = 0f;
        // SlideIn: Soldan (-1920) merkeze (0)
        // SlideOut: Merkezden (0) sola (-1920)
        Vector2 startPos = slideIn ? new Vector2(-1920, 0) : Vector2.zero;
        Vector2 endPos = slideIn ? Vector2.zero : new Vector2(-1920, 0);

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            t = t * t * (3f - 2f * t);
            
            mainMenuPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        mainMenuPanel.anchoredPosition = endPos;
    }

    private void OnExhibitionButton() { Debug.Log("Exhibition"); }
    
    private void OnTrainingButton() 
    { 
        SceneFlow.LoadTrainingGameplay(); 
    }

    private void OnDataPacksButton() { UnityEngine.SceneManagement.SceneManager.LoadScene("DataPackMenu"); }
    
    private void OnSettingsButton() 
    { 
        // Ayarlar panelini getir
        StartCoroutine(SlideMenu(settingsMenuPanel, true));
        // Ana menüyü gizle
        if (mainMenuPanel != null) StartCoroutine(SlideMainPanel(false));
    }

    private void OnBackFromSettings()
    {
        // Ayarlar panelini gizle
        StartCoroutine(SlideMenu(settingsMenuPanel, false));
        // Ana menüyü getir
        if (mainMenuPanel != null) StartCoroutine(SlideMainPanel(true));
    }

    private void OnQuitButton() { Application.Quit(); }

    // --- ANIMATION ---

    private IEnumerator SlideMenu(RectTransform panel, bool slideIn)
    {
        if (panel == null) yield break;

        float timer = 0f;
        Vector2 startPos = slideIn ? new Vector2(1920, 0) : Vector2.zero;
        Vector2 endPos = slideIn ? Vector2.zero : new Vector2(1920, 0);

        if (slideIn) panel.gameObject.SetActive(true);

        while (timer < slideDuration)
        {
            timer += Time.deltaTime;
            float t = timer / slideDuration;
            // Smooth step (ease in out)
            t = t * t * (3f - 2f * t);
            
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        panel.anchoredPosition = endPos;
        if (!slideIn) panel.gameObject.SetActive(false);
    }
}
