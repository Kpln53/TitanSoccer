using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Home Panel UI - Career Hub ana sayfa paneli
/// </summary>
public class HomePanelUI : MonoBehaviour
{
    [Header("Maç Kartı (MatchCard)")]
    public TextMeshProUGUI matchCardTitle;      // matchCardTitle - "SONRAKİ MAÇ"
    public TextMeshProUGUI matchTeamsText;      // matchTeamsText - "Takım A vs Takım B"
    public TextMeshProUGUI matchTypeText;       // matchTypeText - "Lig Maçı - 15 Ocak 2026"
    public Image teamLogo;                      // TeamLogo - Rakip takımın logosu
    public Button goToMatchButton;              // goToMatchButton - Maça git butonu
    
    [Header("Diğer Butonlar")]
    public Button standingsButton;              // StandingsButton - Puan durumu butonu

    [Header("Puan Durumu Paneli")]
    public GameObject standingsPanel;           // Standings paneli referansı

    private MatchData nextMatch;

    private void Start()
    {
        SetupButtons();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    private void SetupButtons()
    {
        if (goToMatchButton != null)
            goToMatchButton.onClick.AddListener(OnPlayMatchButton);
        
        if (standingsButton != null)
            standingsButton.onClick.AddListener(OnStandingsButton);
    }

    private void OnStandingsButton()
    {
        // Standings panelini aç
        if (standingsPanel != null)
        {
            standingsPanel.SetActive(true);
        }
    }

    private void RefreshUI()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            if (matchCardTitle != null)
                matchCardTitle.text = "SONRAKİ MAÇ";
            if (matchTeamsText != null)
                matchTeamsText.text = "Kayıt bulunamadı";
            if (matchTypeText != null)
                matchTypeText.text = "";
            return;
        }

        SaveData save = GameManager.Instance.CurrentSave;
        if (save == null) return;

        // Başlık
        if (matchCardTitle != null)
        {
            matchCardTitle.text = "SONRAKİ MAÇ";
        }

        // Sonraki maçı bul
        RefreshNextMatch(save);
    }

    /// <summary>
    /// Sonraki maçı fikstürden bul ve göster
    /// </summary>
    private void RefreshNextMatch(SaveData save)
    {
        nextMatch = null;

        if (save.seasonData == null || save.seasonData.fixtures == null || save.seasonData.fixtures.Count == 0)
        {
            if (matchTeamsText != null)
                matchTeamsText.text = "Fikstür bulunamadı!";
            if (matchTypeText != null)
                matchTypeText.text = "Lütfen yeni bir kariyer başlatın.";
            
            if (goToMatchButton != null)
                goToMatchButton.interactable = false;
            
            // Logo'yu gizle
            if (teamLogo != null)
                teamLogo.gameObject.SetActive(false);
            
            Debug.LogWarning("[HomePanelUI] No fixtures found in seasonData!");
            return;
        }

        string playerClub = save.clubData?.clubName ?? "";

        // Oyuncunun takımının oynanmamış ilk maçını bul
        nextMatch = save.seasonData.fixtures
            .FirstOrDefault(m => !m.isPlayed && 
                           (m.homeTeamName == playerClub || m.awayTeamName == playerClub));

        if (nextMatch != null)
        {
            // Rakip takımı belirle
            string opponentTeamName = nextMatch.homeTeamName == playerClub 
                ? nextMatch.awayTeamName 
                : nextMatch.homeTeamName;
            
            // Takımları göster
            if (matchTeamsText != null)
            {
                matchTeamsText.text = $"{nextMatch.homeTeamName}  vs  {nextMatch.awayTeamName}";
            }

            // Maç tipi ve tarih
            if (matchTypeText != null)
            {
                string matchTypeStr = nextMatch.matchType switch
                {
                    MatchData.MatchType.League => "Lig Maçı",
                    MatchData.MatchType.Cup => "Kupa Maçı",
                    MatchData.MatchType.Derby => "Derbi",
                    MatchData.MatchType.Friendly => "Hazırlık Maçı",
                    _ => "Maç"
                };
                string dateStr = nextMatch.matchDate.ToString("dd MMMM yyyy");
                string homeAway = nextMatch.homeTeamName == playerClub ? "🏠 Ev Sahibi" : "✈️ Deplasman";
                matchTypeText.text = $"{matchTypeStr} • {dateStr} • {homeAway}";
            }
            
            // Rakip takımın logosunu yükle
            LoadOpponentLogo(opponentTeamName);
            
            if (goToMatchButton != null)
                goToMatchButton.interactable = true;
            
            Debug.Log($"[HomePanelUI] Next match: {nextMatch.homeTeamName} vs {nextMatch.awayTeamName}, Opponent: {opponentTeamName}");
        }
        else
        {
            // Tüm maçlar oynandı veya oyuncunun takımının maçı yok
            if (matchTeamsText != null)
                matchTeamsText.text = "Maç bulunamadı";
            if (matchTypeText != null)
                matchTypeText.text = "Sezon bitti veya fikstürde maç yok.";
            
            if (goToMatchButton != null)
                goToMatchButton.interactable = false;
            
            // Logo'yu gizle
            if (teamLogo != null)
                teamLogo.gameObject.SetActive(false);
            
            Debug.Log("[HomePanelUI] No upcoming matches for player's club.");
        }
    }
    
    /// <summary>
    /// Rakip takımın logosunu yükle
    /// </summary>
    private void LoadOpponentLogo(string teamName)
    {
        if (teamLogo == null)
        {
            Debug.LogWarning("[HomePanelUI] teamLogo Image is not assigned!");
            return;
        }
        
        // DataPackManager'dan takım logosunu al
        if (DataPackManager.Instance != null)
        {
            TeamData team = DataPackManager.Instance.GetTeam(teamName);
            if (team != null && team.teamLogo != null)
            {
                teamLogo.sprite = team.teamLogo;
                teamLogo.gameObject.SetActive(true);
                Debug.Log($"[HomePanelUI] Loaded logo for team: {teamName}");
            }
            else
            {
                // Logo bulunamadı, placeholder göster veya gizle
                teamLogo.sprite = null;
                teamLogo.gameObject.SetActive(false);
                Debug.LogWarning($"[HomePanelUI] Logo not found for team: {teamName}");
            }
        }
        else
        {
            Debug.LogWarning("[HomePanelUI] DataPackManager instance not found!");
            teamLogo.gameObject.SetActive(false);
        }
    }

    private void OnPlayMatchButton()
    {
        if (nextMatch == null)
        {
            Debug.LogWarning("[HomePanelUI] No next match to play!");
            return;
        }

        // MatchContext'e maç bilgilerini kaydet
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            SaveData save = GameManager.Instance.CurrentSave;
            string playerClub = save.clubData?.clubName ?? "";
            
            // Oyuncunun takımı ev sahibi mi deplasman mı?
            bool isHome = nextMatch.homeTeamName == playerClub;
            
            // MatchContext yoksa oluştur
            if (MatchContext.Instance == null)
            {
                GameObject matchContextObj = new GameObject("MatchContext");
                matchContextObj.AddComponent<MatchContext>();
            }
            
            MatchContext.Instance.homeTeamName = nextMatch.homeTeamName;
            MatchContext.Instance.awayTeamName = nextMatch.awayTeamName;
            MatchContext.Instance.isPlayerHome = isHome;
            MatchContext.Instance.matchType = nextMatch.matchType;
            
            // Oyuncu bilgilerini de kaydet
            if (save.playerProfile != null)
            {
                MatchContext.Instance.playerName = save.playerProfile.playerName;
                MatchContext.Instance.playerPosition = save.playerProfile.position;
                MatchContext.Instance.playerOverall = save.playerProfile.overall;
            }
            
            Debug.Log($"[HomePanelUI] MatchContext set: {nextMatch.homeTeamName} vs {nextMatch.awayTeamName}, IsPlayerHome: {isHome}");
        }

        // PreMatch'e geç
        SceneFlow.LoadPreMatch();
    }
}
