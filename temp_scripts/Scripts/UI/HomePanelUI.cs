using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Home Panel UI - Kariyer hub ana sayfası
/// </summary>
public class HomePanelUI : MonoBehaviour
{

    [Header("Sıradaki Maç Kartı")]
    public TextMeshProUGUI matchTeamsText;     // "Takım A vs Takım B" formatında
    public TextMeshProUGUI matchTypeText;      // "Lig Maçı" gibi maç türü
    public Button goToMatchButton;             // "Maça Git" butonu

    [Header("Hızlı Erişim Butonları")]
    public Button trainingButton;
    public Button marketButton;

    private void OnEnable()
    {
        RefreshData();
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    public void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[HomePanelUI] No current save!");
            return;
        }

        SaveData saveData = GameManager.Instance.CurrentSave;

        // Sıradaki maç bilgilerini göster
        RefreshNextMatch(saveData);
    }

    /// <summary>
    /// Sıradaki maç kartını güncelle
    /// </summary>
    private void RefreshNextMatch(SaveData saveData)
    {
        // SeasonCalendarSystem yoksa oluştur
        if (SeasonCalendarSystem.Instance == null)
        {
            GameObject calendarObj = new GameObject("SeasonCalendarSystem");
            calendarObj.AddComponent<SeasonCalendarSystem>();
            Debug.Log("[HomePanelUI] SeasonCalendarSystem created automatically.");
        }

        // SeasonCalendarSystem'den sıradaki maçı al
        MatchData nextMatch = null;
        if (SeasonCalendarSystem.Instance != null)
        {
            nextMatch = SeasonCalendarSystem.Instance.GetNextMatch(saveData);
        }

        if (matchTeamsText != null)
        {
            if (nextMatch != null && !string.IsNullOrEmpty(nextMatch.opponentTeam))
            {
                // Gerçek maç bilgisi var
                string homeTeam = saveData.clubData != null && !string.IsNullOrEmpty(saveData.clubData.clubName) 
                    ? saveData.clubData.clubName 
                    : "Ev Sahibi Takım";
                string awayTeam = nextMatch.opponentTeam;
                
                // Ev sahibi/deplasman durumuna göre göster
                if (nextMatch.isHome)
                {
                    matchTeamsText.text = $"{homeTeam} vs {awayTeam}";
                }
                else
                {
                    matchTeamsText.text = $"{awayTeam} vs {homeTeam}";
                }
            }
            else if (saveData.clubData != null && !string.IsNullOrEmpty(saveData.clubData.clubName))
            {
                // Maç bilgisi yok, fallback
                matchTeamsText.text = $"{saveData.clubData.clubName} vs Takım B";
            }
            else
            {
                matchTeamsText.text = "Takım A vs Takım B";
            }
        }

        if (matchTypeText != null)
        {
            if (nextMatch != null && !string.IsNullOrEmpty(nextMatch.matchType))
            {
                // Maç türünü göster
                string matchTypeStr = nextMatch.matchType switch
                {
                    "League" => "Lig Maçı",
                    "Cup" => "Kupa Maçı",
                    "ChampionsLeague" => "Şampiyonlar Ligi",
                    "EuropaLeague" => "Avrupa Ligi",
                    _ => "Maç"
                };
                matchTypeText.text = matchTypeStr;
            }
            else
            {
                matchTypeText.text = "Lig Maçı";
            }
        }
    }

    /// <summary>
    /// Maça Git butonuna tıklandığında
    /// </summary>
    private void OnGoToMatchButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.MatchPre);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MatchPre");
        }
    }

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (goToMatchButton != null)
            goToMatchButton.onClick.AddListener(OnGoToMatchButton);

        if (trainingButton != null)
            trainingButton.onClick.AddListener(OnTrainingButton);

        if (marketButton != null)
            marketButton.onClick.AddListener(OnMarketButton);
    }

    private void OnTrainingButton()
    {
        // Training panel'e geç (CareerHubUI üzerinden)
        // Bu butonun işlevi CareerHubUI'da tanımlı olabilir
    }

    private void OnMarketButton()
    {
        // Market panel'e geç (CareerHubUI üzerinden)
        // Bu butonun işlevi CareerHubUI'da tanımlı olabilir
    }
}

