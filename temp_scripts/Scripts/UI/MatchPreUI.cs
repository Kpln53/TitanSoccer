using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç öncesi UI - Maç öncesi ekranı
/// </summary>
public class MatchPreUI : MonoBehaviour
{
    [Header("Maç Bilgileri")]
    public TextMeshProUGUI homeTeamNameText;      // Ev sahibi takım adı
    public TextMeshProUGUI awayTeamNameText;      // Deplasman takım adı
    public TextMeshProUGUI matchDateText;         // Maç tarihi (isteğe bağlı)

    [Header("Kadro Durumu")]
    public TextMeshProUGUI squadStatusText;       // "İlk 11" veya kadro durumu
    public TextMeshProUGUI squadReasonText;       // "Neden kadroda? veya değil?" açıklaması

    [Header("Oyuncu Durumu")]
    public Slider energySlider;                   // Enerji slider'ı
    public Slider moraleSlider;                   // Moral slider'ı

    [Header("Butonlar")]
    public Button goToMatchButton;                // "Maça Git" butonu
    public Button backButton;                     // "Ana Menü" / "Geri" butonu

    private void Start()
    {
        SetupButtons();
        RefreshData();
    }

    private void SetupButtons()
    {
        if (goToMatchButton != null)
            goToMatchButton.onClick.AddListener(OnGoToMatchButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    public void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[MatchPreUI] No current save!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;

        // Takım bilgileri
        RefreshTeamNames(saveData);

        // Kadro durumu
        RefreshSquadStatus(saveData);

        // Oyuncu durumu (Enerji ve Moral)
        RefreshPlayerStatus(saveData);
    }

    /// <summary>
    /// Takım isimlerini güncelle
    /// </summary>
    private void RefreshTeamNames(SaveData saveData)
    {
        // SeasonCalendarSystem yoksa oluştur
        if (SeasonCalendarSystem.Instance == null)
        {
            GameObject calendarObj = new GameObject("SeasonCalendarSystem");
            calendarObj.AddComponent<SeasonCalendarSystem>();
            Debug.Log("[MatchPreUI] SeasonCalendarSystem created automatically.");
        }

        // SeasonCalendarSystem'den sıradaki maçı al
        MatchData nextMatch = null;
        if (SeasonCalendarSystem.Instance != null)
        {
            nextMatch = SeasonCalendarSystem.Instance.GetNextMatch(saveData);
        }

        string playerClubName = saveData.clubData != null ? saveData.clubData.clubName : "Takım";
        
        if (nextMatch != null && !string.IsNullOrEmpty(nextMatch.opponentTeam))
        {
            // Gerçek maç bilgisi var
            if (nextMatch.isHome)
            {
                // Ev sahibi maç
                if (homeTeamNameText != null)
                    homeTeamNameText.text = playerClubName;
                if (awayTeamNameText != null)
                    awayTeamNameText.text = nextMatch.opponentTeam;
            }
            else
            {
                // Deplasman maçı
                if (homeTeamNameText != null)
                    homeTeamNameText.text = nextMatch.opponentTeam;
                if (awayTeamNameText != null)
                    awayTeamNameText.text = playerClubName;
            }
        }
        else
        {
            // Fallback
            if (homeTeamNameText != null)
                homeTeamNameText.text = playerClubName;
            if (awayTeamNameText != null)
                awayTeamNameText.text = "Deplasman Takımı";
        }
    }

    /// <summary>
    /// Kadro durumunu güncelle
    /// </summary>
    private void RefreshSquadStatus(SaveData saveData)
    {
        // Kadro durumu (şimdilik "İlk 11" göster - gerçek implementasyonda kontrat rolünden gelecek)
        if (squadStatusText != null && saveData.clubData != null && saveData.clubData.contract != null)
        {
            string status = saveData.clubData.contract.playingTime switch
            {
                PlayingTime.Starter => "İlk 11",
                PlayingTime.Rotation => "Yedek",
                PlayingTime.Substitute => "Yedek",
                _ => "Kadro Dışı"
            };
            squadStatusText.text = status;
        }
        else if (squadStatusText != null)
        {
            squadStatusText.text = "İlk 11";
        }

        // Kadro nedeni açıklaması
        if (squadReasonText != null)
        {
            // Şimdilik örnek metin (gerçek implementasyonda AI manager kararından gelecek)
            squadReasonText.text = "Neden kadroda? veya değil?";
        }
    }

    /// <summary>
    /// Oyuncu durumunu güncelle (Enerji ve Moral)
    /// </summary>
    private void RefreshPlayerStatus(SaveData saveData)
    {
        if (saveData.playerProfile == null) return;

        // Enerji slider
        if (energySlider != null)
        {
            energySlider.value = saveData.playerProfile.energy / 100f; // 0-1 arası değer
        }

        // Moral slider
        if (moraleSlider != null)
        {
            moraleSlider.value = saveData.playerProfile.moral / 100f; // 0-1 arası değer
        }
    }

    /// <summary>
    /// Maça Git butonuna tıklandığında
    /// </summary>
    private void OnGoToMatchButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Match);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Match");
        }
    }

    /// <summary>
    /// Geri butonuna tıklandığında
    /// </summary>
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
