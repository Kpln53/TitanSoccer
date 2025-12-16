using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Kariyer ana ekranı UI - Yeni sistemlerle entegre
/// </summary>
public class CareerHubUI : MonoBehaviour
{
    [Header("Profil UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI overallText;
    public TextMeshProUGUI ageText;

    [Header("Maç Bilgisi UI")]
    public TextMeshProUGUI nextMatchTitleText;
    public TextMeshProUGUI nextMatchTeamsText;
    public TextMeshProUGUI nextMatchTypeText;
    public Button goToMatchButton;

    [Header("Enerji & Moral")]
    public Slider energySlider;
    public Slider moraleSlider;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI moraleText;

    [Header("Alt Menü Butonları")]
    public Button homeButton;
    public Button newsButton;
    public Button socialMediaButton;
    public Button trainingButton;
    public Button lifeButton;
    public Button playerButton;
    public Button otherButton;

    [Header("Puan Durumu")]
    public Button standingsButton;

    private void Start()
    {
        SetupButtons();
        LoadCareerData();
        
        // Kritik olayları kontrol et
        CheckCriticalEvents();
    }

    private void SetupButtons()
    {
        if (goToMatchButton != null)
            goToMatchButton.onClick.AddListener(OnGoToMatchButton);
        
        if (playerButton != null)
            playerButton.onClick.AddListener(OnPlayerStatsButton);
        
        if (newsButton != null)
            newsButton.onClick.AddListener(OnNewsButton);
        
        if (socialMediaButton != null)
            socialMediaButton.onClick.AddListener(OnSocialMediaButton);
        
        if (trainingButton != null)
            trainingButton.onClick.AddListener(OnTrainingButton);
        
        if (standingsButton != null)
            standingsButton.onClick.AddListener(OnStandingsButton);
    }

    /// <summary>
    /// Kariyer verilerini yükle
    /// </summary>
    private void LoadCareerData()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("CareerHub: CurrentSave bulunamadı!");
            if (playerNameText != null)
                playerNameText.text = "Kayıt bulunamadı";
            return;
        }

        SaveData data = GameManager.Instance.CurrentSave;
        PlayerProfile profile = data.playerProfile;

        if (profile == null)
        {
            Debug.LogError("PlayerProfile bulunamadı!");
            return;
        }

        // Profil bilgileri
        if (playerNameText != null)
            playerNameText.text = $"{profile.playerName} {profile.surname}";

        if (clubNameText != null)
            clubNameText.text = data.clubData.clubName;

        if (seasonText != null)
            seasonText.text = $"Sezon {data.seasonData.seasonNumber} - Hafta {data.seasonData.currentWeek}";

        if (overallText != null)
            overallText.text = $"OVR {profile.overall}";

        if (ageText != null)
            ageText.text = $"Yaş: {profile.age}";

        // Enerji & Moral
        if (energySlider != null)
            energySlider.value = profile.energy;
        
        if (energyText != null)
            energyText.text = $"Enerji: {(profile.energy * 100):F0}%";
        
        if (moraleSlider != null)
            moraleSlider.value = profile.morale;
        
        if (moraleText != null)
            moraleText.text = $"Moral: {(profile.morale * 100):F0}%";

        // Maç bilgisi
        LoadNextMatchInfo(data);
    }

    /// <summary>
    /// Sonraki maç bilgisini yükle
    /// </summary>
    private void LoadNextMatchInfo(SaveData data)
    {
        if (SeasonCalendarSystem.Instance == null)
            return;
        
        Fixture nextMatch = SeasonCalendarSystem.Instance.GetCurrentWeekMatch(data);
        
        if (nextMatch != null)
        {
            if (nextMatchTitleText != null)
                nextMatchTitleText.text = "Sıradaki Maç";
            
            if (nextMatchTeamsText != null)
            {
                // Oyuncunun takımı hangi tarafta?
                bool isHome = nextMatch.homeTeam == data.clubData.clubName;
                if (isHome)
                    nextMatchTeamsText.text = $"{nextMatch.homeTeam} vs {nextMatch.awayTeam}";
                else
                    nextMatchTeamsText.text = $"{nextMatch.awayTeam} vs {nextMatch.homeTeam}";
            }
            
            if (nextMatchTypeText != null)
                nextMatchTypeText.text = "Lig Maçı";
        }
        else
        {
            if (nextMatchTitleText != null)
                nextMatchTitleText.text = "Maç Yok";
            
            if (nextMatchTeamsText != null)
                nextMatchTeamsText.text = "Bu hafta maç yok";
        }
    }

    /// <summary>
    /// Kritik olayları kontrol et
    /// </summary>
    private void CheckCriticalEvents()
    {
        if (CriticalEventSystem.Instance != null && CriticalEventSystem.Instance.HasPendingEvents())
        {
            // CriticalEventPopUpUI otomatik olarak gösterilecek
            // Burada sadece kontrol ediyoruz
        }
    }

    public void OnGoToMatchButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.MatchPre);
        }
        else
        {
            // Fallback: Eski sistem
            UnityEngine.SceneManagement.SceneManager.LoadScene("MatchScene");
        }
    }

    private void OnPlayerStatsButton()
    {
        // Player Stats ekranına git
        // Şimdilik sadece log
        Debug.Log("Player Stats ekranına git");
    }

    private void OnNewsButton()
    {
        // Haberler ekranına git
        Debug.Log("Haberler ekranına git");
    }

    private void OnSocialMediaButton()
    {
        // Sosyal medya ekranına git
        Debug.Log("Sosyal medya ekranına git");
    }

    private void OnTrainingButton()
    {
        // Antreman ekranına git
        Debug.Log("Antreman ekranına git");
    }

    private void OnStandingsButton()
    {
        // Puan durumu ekranına git
        Debug.Log("Puan durumu ekranına git");
    }
}
