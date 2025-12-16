using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Takım teklifi UI - 3 takım teklifi ve sözleşme imzalama
/// </summary>
public class TeamOfferUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private Transform offersContainer;
    [SerializeField] private GameObject offerCardPrefab;
    [SerializeField] private GameObject contractDetailPanel;
    
    [Header("Contract Detail Panel")]
    [SerializeField] private TextMeshProUGUI contractTeamNameText;
    [SerializeField] private TextMeshProUGUI contractSalaryText;
    [SerializeField] private TextMeshProUGUI contractDurationText;
    [SerializeField] private TextMeshProUGUI contractPlayingTimeText;
    [SerializeField] private TextMeshProUGUI contractBonusText;
    [SerializeField] private Button signButton;
    [SerializeField] private Button backButton;
    
    [Header("Signature Animation")]
    [SerializeField] private GameObject signatureAnimation;
    [SerializeField] private Image signatureLine;
    
    private List<TeamOffer> offers = new List<TeamOffer>();
    private TeamOffer selectedOffer;

    private void Start()
    {
        GenerateTeamOffers();
        DisplayOffers();
        SetupButtons();
        
        if (contractDetailPanel != null)
            contractDetailPanel.SetActive(false);
    }

    /// <summary>
    /// 3 takım teklifi oluştur (seçilen ligin en düşük güçlü 3 takımı)
    /// </summary>
    private void GenerateTeamOffers()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("GameManager veya SaveData bulunamadı!");
            return;
        }
        
        SaveData saveData = GameManager.Instance.CurrentSave;
        string leagueName = saveData.clubData.leagueName;
        
        if (DataPackManager.Instance == null || DataPackManager.Instance.GetActiveDataPack() == null)
        {
            Debug.LogError("DataPackManager veya aktif Data Pack bulunamadı!");
            return;
        }
        
        var dataPack = DataPackManager.Instance.GetActiveDataPack();
        var league = dataPack.GetLeagueByName(leagueName);
        
        if (league == null || league.teams == null)
        {
            Debug.LogError("Lig veya takımlar bulunamadı!");
            return;
        }
        
        // Takımları güce göre sırala
        List<TeamData> sortedTeams = new List<TeamData>(league.teams);
        sortedTeams.Sort((a, b) => a.GetTeamPower().CompareTo(b.GetTeamPower()));
        
        // En düşük güçlü 3 takımı al
        int offerCount = Mathf.Min(3, sortedTeams.Count);
        
        for (int i = 0; i < offerCount; i++)
        {
            TeamData team = sortedTeams[i];
            TeamOffer offer = CreateOffer(team, saveData.playerProfile);
            offers.Add(offer);
        }
    }

    /// <summary>
    /// Teklif oluştur
    /// </summary>
    private TeamOffer CreateOffer(TeamData team, PlayerProfile profile)
    {
        TeamOffer offer = new TeamOffer();
        offer.teamName = team.teamName;
        offer.teamId = team.teamName;
        
        // Maaş hesapla (takım gücüne ve oyuncu OVR'ına göre)
        int teamPower = team.GetTeamPower();
        int baseSalary = 5000 + (teamPower * 200) + (profile.overall * 300);
        offer.monthlySalary = baseSalary;
        
        // Sözleşme süresi (12-24 ay)
        offer.contractDuration = Random.Range(12, 25);
        
        // Oynama zamanı (OVR'a göre)
        int diff = profile.overall - teamPower;
        if (diff > 5)
            offer.playingTime = PlayingTime.Starter;
        else if (diff > -5)
            offer.playingTime = PlayingTime.Rotation;
        else
            offer.playingTime = PlayingTime.Substitute;
        
        // Bonuslar
        offer.goalBonus = 1000;
        offer.assistBonus = 500;
        offer.winBonus = 2000;
        
        return offer;
    }

    /// <summary>
    /// Teklifleri göster
    /// </summary>
    private void DisplayOffers()
    {
        if (offersContainer == null || offerCardPrefab == null)
        {
            Debug.LogError("Offers container veya prefab bulunamadı!");
            return;
        }
        
        // Mevcut kartları temizle
        foreach (Transform child in offersContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Her teklif için kart oluştur
        foreach (var offer in offers)
        {
            GameObject cardObj = Instantiate(offerCardPrefab, offersContainer);
            SetupOfferCard(cardObj, offer);
        }
    }

    /// <summary>
    /// Teklif kartını ayarla
    /// </summary>
    private void SetupOfferCard(GameObject cardObj, TeamOffer offer)
    {
        // Kart içindeki UI elementlerini bul ve doldur
        TextMeshProUGUI teamNameText = cardObj.transform.Find("TeamNameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI salaryText = cardObj.transform.Find("SalaryText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI playingTimeText = cardObj.transform.Find("PlayingTimeText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI durationText = cardObj.transform.Find("DurationText")?.GetComponent<TextMeshProUGUI>();
        Button detailButton = cardObj.transform.Find("DetailButton")?.GetComponent<Button>();
        
        if (teamNameText != null)
            teamNameText.text = offer.teamName;
        
        if (salaryText != null)
            salaryText.text = $"{offer.monthlySalary:N0} €/ay";
        
        if (playingTimeText != null)
        {
            string playingTimeStr = "";
            switch (offer.playingTime)
            {
                case PlayingTime.Starter:
                    playingTimeStr = "İlk 11";
                    break;
                case PlayingTime.Rotation:
                    playingTimeStr = "Rotasyon";
                    break;
                case PlayingTime.Substitute:
                    playingTimeStr = "Yedek";
                    break;
            }
            playingTimeText.text = playingTimeStr;
        }
        
        if (durationText != null)
            durationText.text = $"{offer.contractDuration} ay";
        
        if (detailButton != null)
        {
            detailButton.onClick.RemoveAllListeners();
            detailButton.onClick.AddListener(() => ShowContractDetails(offer));
        }
    }

    /// <summary>
    /// Sözleşme detaylarını göster
    /// </summary>
    private void ShowContractDetails(TeamOffer offer)
    {
        selectedOffer = offer;
        
        if (contractDetailPanel == null)
            return;
        
        contractDetailPanel.SetActive(true);
        
        if (contractTeamNameText != null)
            contractTeamNameText.text = offer.teamName;
        
        if (contractSalaryText != null)
            contractSalaryText.text = $"Aylık Maaş: {offer.monthlySalary:N0} €";
        
        if (contractDurationText != null)
            contractDurationText.text = $"Sözleşme Süresi: {offer.contractDuration} ay";
        
        if (contractPlayingTimeText != null)
        {
            string playingTimeStr = "";
            switch (offer.playingTime)
            {
                case PlayingTime.Starter:
                    playingTimeStr = "İlk 11";
                    break;
                case PlayingTime.Rotation:
                    playingTimeStr = "Rotasyon";
                    break;
                case PlayingTime.Substitute:
                    playingTimeStr = "Yedek";
                    break;
            }
            contractPlayingTimeText.text = $"Oynama Zamanı: {playingTimeStr}";
        }
        
        if (contractBonusText != null)
        {
            contractBonusText.text = $"Gol Bonusu: {offer.goalBonus:N0} €\n" +
                                     $"Asist Bonusu: {offer.assistBonus:N0} €\n" +
                                     $"Galibiyet Bonusu: {offer.winBonus:N0} €";
        }
    }

    /// <summary>
    /// Butonları ayarla
    /// </summary>
    private void SetupButtons()
    {
        if (signButton != null)
        {
            signButton.onClick.RemoveAllListeners();
            signButton.onClick.AddListener(OnSignButton);
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(OnBackButton);
        }
    }

    /// <summary>
    /// İmzala butonu
    /// </summary>
    private void OnSignButton()
    {
        if (selectedOffer == null)
            return;
        
        // Sözleşmeyi kaydet
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            SaveData saveData = GameManager.Instance.CurrentSave;
            
            saveData.clubData.clubName = selectedOffer.teamName;
            saveData.clubData.clubId = selectedOffer.teamId;
            saveData.clubData.contract.monthlySalary = selectedOffer.monthlySalary;
            saveData.clubData.contract.contractDuration = selectedOffer.contractDuration;
            saveData.clubData.contract.remainingMonths = selectedOffer.contractDuration;
            saveData.clubData.contract.playingTime = selectedOffer.playingTime;
            saveData.clubData.contract.goalBonus = selectedOffer.goalBonus;
            saveData.clubData.contract.assistBonus = selectedOffer.assistBonus;
            saveData.clubData.contract.winBonus = selectedOffer.winBonus;
            
            // İmza animasyonu
            StartCoroutine(PlaySignatureAnimation());
        }
    }

    /// <summary>
    /// İmza animasyonu
    /// </summary>
    private System.Collections.IEnumerator PlaySignatureAnimation()
    {
        if (signatureAnimation != null)
            signatureAnimation.SetActive(true);
        
        if (signatureLine != null)
        {
            float duration = 1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                
                // İmza çizgisini çiz
                signatureLine.fillAmount = progress;
                
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Kariyer menüsüne geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
    }

    private void OnBackButton()
    {
        if (contractDetailPanel != null)
            contractDetailPanel.SetActive(false);
    }
}

/// <summary>
/// Takım teklifi
/// </summary>
[System.Serializable]
public class TeamOffer
{
    public string teamName;
    public string teamId;
    public int monthlySalary;
    public int contractDuration;
    public PlayingTime playingTime;
    public int goalBonus;
    public int assistBonus;
    public int winBonus;
}

