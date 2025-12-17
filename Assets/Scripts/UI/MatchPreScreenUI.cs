using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maç öncesi ekran - Kadro, enerji, moral, neden kadroda/ilk11/yedek
/// </summary>
public class MatchPreScreenUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI homeTeamNameText;
    [SerializeField] private TextMeshProUGUI awayTeamNameText;
    [SerializeField] private TextMeshProUGUI squadStatusText;
    [SerializeField] private TextMeshProUGUI squadReasonText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI moraleText;
    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider moraleSlider;
    [SerializeField] private Button goToMatchButton;
    [SerializeField] private Button backButton;
    
    [Header("Lineup Display")]
    [SerializeField] private Transform homeLineupContainer;
    [SerializeField] private Transform awayLineupContainer;
    [SerializeField] private GameObject playerCardPrefab;

    private void Start()
    {
        SetupButtons();
        LoadMatchData();
    }

    private void SetupButtons()
    {
        if (goToMatchButton != null)
        {
            goToMatchButton.onClick.AddListener(OnGoToMatchButton);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
        }
    }

    /// <summary>
    /// Maç verilerini yükle
    /// </summary>
    private void LoadMatchData()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("GameManager veya SaveData bulunamadı!");
            return;
        }
        
        SaveData saveData = GameManager.Instance.CurrentSave;
        PlayerProfile profile = saveData.playerProfile;
        
        // Mevcut haftanın maçını al
        Fixture currentMatch = SeasonCalendarSystem.Instance?.GetCurrentWeekMatch(saveData);
        
        if (currentMatch != null)
        {
            if (homeTeamNameText != null)
                homeTeamNameText.text = currentMatch.homeTeam;
            
            if (awayTeamNameText != null)
                awayTeamNameText.text = currentMatch.awayTeam;
        }
        
        // Enerji ve moral göster
        if (energyText != null)
            energyText.text = $"Enerji: {(profile.energy * 100):F0}%";
        
        if (energySlider != null)
            energySlider.value = profile.energy;
        
        if (moraleText != null)
            moraleText.text = $"Moral: {(profile.morale * 100):F0}%";
        
        if (moraleSlider != null)
            moraleSlider.value = profile.morale;
        
        // Kadro durumunu belirle
        DetermineSquadStatus(profile, saveData);
    }

    /// <summary>
    /// Kadro durumunu belirle (İlk 11, Yedek, Kadroda Değil)
    /// </summary>
    private void DetermineSquadStatus(PlayerProfile profile, SaveData saveData)
    {
        SquadStatus status = SquadStatus.NotInSquad;
        string reason = "";
        
        // Basit mantık: Form, moral, enerji ve performansa göre
        float score = (profile.form * 0.3f) + (profile.morale * 0.2f) + (profile.energy * 0.2f);
        
        // Son maç performansı
        if (profile.seasonStats.averageRating > 0)
        {
            score += (profile.seasonStats.averageRating / 10f) * 0.3f;
        }
        
        // Sakatlık kontrolü
        if (InjurySystem.Instance != null && InjurySystem.Instance.HasActiveInjury(profile))
        {
            status = SquadStatus.NotInSquad;
            reason = "Sakatlık nedeniyle kadroda değilsiniz.";
        }
        else if (score >= 0.7f)
        {
            status = SquadStatus.InStarting11;
            reason = GetStarting11Reason(profile);
        }
        else if (score >= 0.5f)
        {
            status = SquadStatus.InSquad;
            reason = GetSubstituteReason(profile);
        }
        else
        {
            status = SquadStatus.NotInSquad;
            reason = GetNotInSquadReason(profile);
        }
        
        // UI'da göster
        if (squadStatusText != null)
        {
            string statusText = "";
            switch (status)
            {
                case SquadStatus.InStarting11:
                    statusText = "İlk 11";
                    squadStatusText.color = Color.green;
                    break;
                case SquadStatus.InSquad:
                    statusText = "Yedek";
                    squadStatusText.color = Color.yellow;
                    break;
                case SquadStatus.NotInSquad:
                    statusText = "Kadroda Değil";
                    squadStatusText.color = Color.red;
                    break;
            }
            squadStatusText.text = statusText;
        }
        
        if (squadReasonText != null)
            squadReasonText.text = reason;
    }

    private string GetStarting11Reason(PlayerProfile profile)
    {
        string[] reasons = {
            "Son maçtaki performansınız iyiydi.",
            "Genel performansınız iyi.",
            "Teknik direktör size bir şans vermek istedi.",
            "Formunuz yüksek."
        };
        return reasons[Random.Range(0, reasons.Length)];
    }

    private string GetSubstituteReason(PlayerProfile profile)
    {
        string[] reasons = {
            "Son maçta performansınız iyi değildi.",
            "Teknik direktörle aranız iyi değil.",
            "Takımla aranız iyi değil.",
            "Genel performansınız iyi değil.",
            "Enerjiniz düşük."
        };
        return reasons[Random.Range(0, reasons.Length)];
    }

    private string GetNotInSquadReason(PlayerProfile profile)
    {
        string[] reasons = {
            "Son maçlarda performansınız rezaletti.",
            "Teknik direktörle aranız çok kötü.",
            "Magazinde isminiz çok anılıyor (yüksek seviyede).",
            "Genel performansınız rezalet seviyede.",
            "Enerjiniz çok düşük."
        };
        return reasons[Random.Range(0, reasons.Length)];
    }

    private void OnGoToMatchButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.MatchPre);
            // MatchPre'den MatchSim'e geçiş MatchSimManager'da yapılacak
        }
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
    }
}

public enum SquadStatus
{
    InStarting11,
    InSquad,
    NotInSquad
}


