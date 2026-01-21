using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Career Hub ana UI - Ana kariyer ekranı ve panel yönetimi
/// </summary>
public class CareerHubUI : MonoBehaviour
{
    [Header("Top Panel")]
    public TopPanelUI topPanel; // Her sayfada görünen üst panel

    [Header("Ana Paneller")]
    public GameObject homePanel;
    public GameObject newsPanel;
    public GameObject marketPanel;
    public GameObject trainingPanel;
    public GameObject lifePanel;
    public GameObject playerStatsPanel;
    public GameObject standingsPanel;
    public GameObject socialMediaPanel;

    [Header("Panel Butonları")]
    public Button homeButton;
    public Button newsButton;
    public Button marketButton;
    public Button trainingButton;
    public Button lifeButton;
    public Button playerStatsButton;
    public Button standingsButton;
    public Button socialMediaButton;

    [Header("Diğer Butonlar")]
    public Button goToMatchButton;

    private GameObject currentActivePanel;

    private void Start()
    {
        if (socialMediaPanel == null)
        {
            var found = GameObject.Find("SocialMediaPanel");
            if (found != null && found.transform.parent.name == "Content") // Content içinde olduğundan emin ol (Builder yapısını hatırla)
                socialMediaPanel = found;
             else if (found != null) // Direkt root'ta veya başka yerdeyse
                 socialMediaPanel = found;
        }

        SetupButtons();
        RefreshTopPanel();
        ShowPanel(homePanel); // Başlangıçta Home panelini göster
    }

    /// <summary>
    /// Top panel'i yenile
    /// </summary>
    private void RefreshTopPanel()
    {
        if (topPanel != null)
        {
            topPanel.RefreshData();
        }
    }

    private void SetupButtons()
    {
        // Panel butonları
        if (homeButton != null)
            homeButton.onClick.AddListener(() => ShowPanel(homePanel));

        if (newsButton != null)
            newsButton.onClick.AddListener(() => ShowPanel(newsPanel));

        if (marketButton != null)
            marketButton.onClick.AddListener(() => ShowPanel(marketPanel));

        if (trainingButton != null)
            trainingButton.onClick.AddListener(() => ShowPanel(trainingPanel));

        if (lifeButton != null)
            lifeButton.onClick.AddListener(() => ShowPanel(lifePanel));

        if (playerStatsButton != null)
            playerStatsButton.onClick.AddListener(() => ShowPanel(playerStatsPanel));

        if (standingsButton != null)
            standingsButton.onClick.AddListener(() => ShowPanel(standingsPanel));

        if (socialMediaButton != null)
            socialMediaButton.onClick.AddListener(() => ShowPanel(socialMediaPanel));

        // Diğer butonlar
        if (goToMatchButton != null)
            goToMatchButton.onClick.AddListener(OnGoToMatchButton);
    }

    /// <summary>
    /// Belirli bir paneli göster ve diğerlerini gizle
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        if (panel == null) return;

        // Tüm panelleri gizle
        if (homePanel != null) homePanel.SetActive(false);
        if (newsPanel != null) newsPanel.SetActive(false);
        if (marketPanel != null) marketPanel.SetActive(false);
        if (trainingPanel != null) trainingPanel.SetActive(false);
        if (lifePanel != null) lifePanel.SetActive(false);
        if (playerStatsPanel != null) playerStatsPanel.SetActive(false);
        if (standingsPanel != null) standingsPanel.SetActive(false);
        if (socialMediaPanel != null) socialMediaPanel.SetActive(false);

        // İstenen paneli göster
        panel.SetActive(true);
        currentActivePanel = panel;

        // Top panel'i yenile (her panel değiştiğinde)
        RefreshTopPanel();

        Debug.Log($"[CareerHubUI] Panel shown: {panel.name}");
    }

    private void OnGoToMatchButton()
    {
        Debug.Log("[CareerHubUI] Going to match...");
        SceneFlow.LoadPreMatch();
    }
}

