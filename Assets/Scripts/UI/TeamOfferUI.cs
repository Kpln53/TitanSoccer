using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Takım teklifi UI - Yeni oyuncuya takım teklifleri gösterir
/// </summary>
public class TeamOfferUI : MonoBehaviour
{
    [Header("Teklif Bilgileri")]
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI leagueNameText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI contractDurationText;
    public TextMeshProUGUI playingTimeText;
    public TextMeshProUGUI signingBonusText;

    [Header("Butonlar")]
    public Button acceptButton;
    public Button rejectButton;
    public Button backButton;

    private TransferOffer currentOffer;

    private void Start()
    {
        SetupButtons();
        GenerateTeamOffer();
    }

    private void SetupButtons()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(OnAcceptButton);

        if (rejectButton != null)
            rejectButton.onClick.AddListener(OnRejectButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    /// <summary>
    /// Takım teklifi oluştur (basit implementasyon)
    /// </summary>
    private void GenerateTeamOffer()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogWarning("[TeamOfferUI] No current save! Cannot generate offer.");
            return;
        }

        PlayerProfile player = GameManager.Instance.CurrentSave.playerProfile;
        if (player == null)
        {
            Debug.LogWarning("[TeamOfferUI] No player profile! Cannot generate offer.");
            return;
        }

        // Basit bir teklif oluştur (gerçek implementasyon TransferAISystem'den gelecek)
        currentOffer = new TransferOffer
        {
            clubName = "Example FC",
            leagueName = "Example League",
            salary = player.overall * 1000,
            contractDuration = 3,
            playingTime = PlayingTime.Rotation,
            signingBonus = player.overall * 2000
        };

        DisplayOffer(currentOffer);
    }

    /// <summary>
    /// Teklifi göster
    /// </summary>
    private void DisplayOffer(TransferOffer offer)
    {
        if (clubNameText != null)
            clubNameText.text = offer.clubName;

        if (leagueNameText != null)
            leagueNameText.text = offer.leagueName;

        if (salaryText != null)
            salaryText.text = $"Maaş: {offer.salary:N0}€/hafta";

        if (contractDurationText != null)
            contractDurationText.text = $"Sözleşme: {offer.contractDuration} yıl";

        if (playingTimeText != null)
        {
            string playingTimeStr = offer.playingTime switch
            {
                PlayingTime.Starter => "İlk 11",
                PlayingTime.Rotation => "Rotasyon",
                PlayingTime.Substitute => "Yedek",
                _ => "Belirsiz"
            };
            playingTimeText.text = $"Oynama Süresi: {playingTimeStr}";
        }

        if (signingBonusText != null)
            signingBonusText.text = $"İmza Parası: {offer.signingBonus:N0}€";
    }

    private void OnAcceptButton()
    {
        if (currentOffer == null || GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
            return;

        // Transfer teklifini kabul et
        if (TransferSystem.Instance != null)
        {
            TransferSystem.Instance.AcceptOffer(GameManager.Instance.CurrentSave, currentOffer);
        }
        else
        {
            // TransferSystem yoksa manuel olarak ayarla
            if (GameManager.Instance.CurrentSave.clubData == null)
                GameManager.Instance.CurrentSave.clubData = new ClubData();
            
            if (GameManager.Instance.CurrentSave.clubData.contract == null)
                GameManager.Instance.CurrentSave.clubData.contract = new ContractData();
            
            GameManager.Instance.CurrentSave.clubData.clubName = currentOffer.clubName;
            GameManager.Instance.CurrentSave.clubData.leagueName = currentOffer.leagueName;
            GameManager.Instance.CurrentSave.clubData.contract.salary = currentOffer.salary;
            GameManager.Instance.CurrentSave.clubData.contract.contractDuration = currentOffer.contractDuration;
            GameManager.Instance.CurrentSave.clubData.contract.playingTime = currentOffer.playingTime;
        }

        // CareerHub'a geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }

    private void OnRejectButton()
    {
        // Teklifi reddet ve yeni teklif oluştur (basit implementasyon)
        GenerateTeamOffer();
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToPreviousState();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
        }
    }
}

