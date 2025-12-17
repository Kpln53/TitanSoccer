using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Oyuncu istatistikleri ekranı - Season, Career, Attributes sekmeleri
/// </summary>
public class PlayerStatsScreenUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI clubNameText;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI ovrText;
    [SerializeField] private TextMeshProUGUI marketValueText;
    
    [Header("Sekmeler")]
    [SerializeField] private Button seasonTabButton;
    [SerializeField] private Button careerTabButton;
    [SerializeField] private Button attributesTabButton;
    [SerializeField] private GameObject seasonTabContent;
    [SerializeField] private GameObject careerTabContent;
    [SerializeField] private GameObject attributesTabContent;
    
    [Header("Season Tab")]
    [SerializeField] private Transform seasonStatsContainer;
    [SerializeField] private GameObject statRowPrefab;
    
    [Header("Career Tab")]
    [SerializeField] private Transform careerStatsContainer;
    
    [Header("Attributes Tab")]
    [SerializeField] private Transform attributesContainer;
    [SerializeField] private GameObject attributeBarPrefab;
    
    [Header("Mini Kartlar")]
    [SerializeField] private GameObject formTrendCard;
    [SerializeField] private GameObject injuryHistoryCard;
    [SerializeField] private GameObject disciplineCard;
    
    [Header("Butonlar")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button trainingButton;
    [SerializeField] private Button equipmentButton;

    private PlayerProfile currentProfile;
    private StatsTab currentTab = StatsTab.Season;

    private void Start()
    {
        SetupButtons();
        LoadPlayerData();
        ShowTab(StatsTab.Season);
    }

    private void SetupButtons()
    {
        if (seasonTabButton != null)
            seasonTabButton.onClick.AddListener(() => ShowTab(StatsTab.Season));
        
        if (careerTabButton != null)
            careerTabButton.onClick.AddListener(() => ShowTab(StatsTab.Career));
        
        if (attributesTabButton != null)
            attributesTabButton.onClick.AddListener(() => ShowTab(StatsTab.Attributes));
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        
        if (trainingButton != null)
            trainingButton.onClick.AddListener(OnTrainingButton);
        
        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(OnEquipmentButton);
    }

    /// <summary>
    /// Oyuncu verilerini yükle
    /// </summary>
    private void LoadPlayerData()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("GameManager veya SaveData bulunamadı!");
            return;
        }
        
        currentProfile = GameManager.Instance.CurrentSave.playerProfile;
        SaveData saveData = GameManager.Instance.CurrentSave;
        
        if (currentProfile == null)
            return;
        
        // Temel bilgiler
        if (playerNameText != null)
            playerNameText.text = $"{currentProfile.playerName} {currentProfile.surname}";
        
        if (clubNameText != null)
            clubNameText.text = saveData.clubData.clubName;
        
        if (positionText != null)
            positionText.text = GetPositionString(currentProfile.position);
        
        if (ageText != null)
            ageText.text = $"Yaş: {currentProfile.age}";
        
        if (ovrText != null)
            ovrText.text = $"OVR: {currentProfile.overall}";
        
        if (marketValueText != null)
            marketValueText.text = $"Piyasa Değeri: {currentProfile.marketValue:N0} €";
        
        // Sekme içeriklerini doldur
        FillSeasonTab();
        FillCareerTab();
        FillAttributesTab();
        
        // Mini kartları doldur
        FillMiniCards();
    }

    /// <summary>
    /// Season sekmesini doldur
    /// </summary>
    private void FillSeasonTab()
    {
        if (seasonStatsContainer == null || statRowPrefab == null || currentProfile == null)
            return;
        
        // Mevcut içeriği temizle
        foreach (Transform child in seasonStatsContainer)
        {
            Destroy(child.gameObject);
        }
        
        SeasonStats stats = currentProfile.seasonStats;
        
        // İstatistik satırları oluştur
        CreateStatRow("Oynanan Maç", stats.matchesPlayed.ToString());
        CreateStatRow("Oynanan Dakika", stats.minutesPlayed.ToString());
        CreateStatRow("Gol", stats.goals.ToString());
        CreateStatRow("Asist", stats.assists.ToString());
        CreateStatRow("Şut", stats.shots.ToString());
        CreateStatRow("İsabetli Şut", stats.shotsOnTarget.ToString());
        CreateStatRow("Pas Başarı %", $"{stats.GetPassSuccessRate():F1}%");
        CreateStatRow("Ortalama Rating", $"{stats.averageRating:F1}");
    }

    /// <summary>
    /// Career sekmesini doldur
    /// </summary>
    private void FillCareerTab()
    {
        if (careerStatsContainer == null || statRowPrefab == null || currentProfile == null)
            return;
        
        foreach (Transform child in careerStatsContainer)
        {
            Destroy(child.gameObject);
        }
        
        CareerStats stats = currentProfile.careerStats;
        
        CreateStatRow("Toplam Maç", stats.totalMatches.ToString());
        CreateStatRow("Toplam Dakika", stats.totalMinutes.ToString());
        CreateStatRow("Toplam Gol", stats.totalGoals.ToString());
        CreateStatRow("Toplam Asist", stats.totalAssists.ToString());
        CreateStatRow("Toplam Şut", stats.totalShots.ToString());
        CreateStatRow("Toplam Pas", stats.totalPasses.ToString());
        CreateStatRow("Kariyer Ortalama Rating", $"{stats.careerAverageRating:F1}");
    }

    /// <summary>
    /// Attributes sekmesini doldur
    /// </summary>
    private void FillAttributesTab()
    {
        if (attributesContainer == null || attributeBarPrefab == null || currentProfile == null)
            return;
        
        foreach (Transform child in attributesContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Stat barları oluştur
        CreateAttributeBar("Hız (Pace)", currentProfile.pace);
        CreateAttributeBar("Şut (Shooting)", currentProfile.shooting);
        CreateAttributeBar("Pas (Passing)", currentProfile.passing);
        CreateAttributeBar("Dribling", currentProfile.dribbling);
        CreateAttributeBar("Savunma (Defense)", currentProfile.defense);
        CreateAttributeBar("Dayanıklılık (Stamina)", currentProfile.stamina);
    }

    /// <summary>
    /// İstatistik satırı oluştur
    /// </summary>
    private void CreateStatRow(string label, string value)
    {
        GameObject row = Instantiate(statRowPrefab, seasonStatsContainer);
        TextMeshProUGUI labelText = row.transform.Find("LabelText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI valueText = row.transform.Find("ValueText")?.GetComponent<TextMeshProUGUI>();
        
        if (labelText != null)
            labelText.text = label;
        
        if (valueText != null)
            valueText.text = value;
    }

    /// <summary>
    /// Attribute bar oluştur
    /// </summary>
    private void CreateAttributeBar(string label, int value)
    {
        GameObject bar = Instantiate(attributeBarPrefab, attributesContainer);
        TextMeshProUGUI labelText = bar.transform.Find("LabelText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI valueText = bar.transform.Find("ValueText")?.GetComponent<TextMeshProUGUI>();
        Slider barSlider = bar.transform.Find("BarSlider")?.GetComponent<Slider>();
        
        if (labelText != null)
            labelText.text = label;
        
        if (valueText != null)
            valueText.text = value.ToString();
        
        if (barSlider != null)
        {
            barSlider.value = value / 100f;
        }
    }

    /// <summary>
    /// Mini kartları doldur
    /// </summary>
    private void FillMiniCards()
    {
        if (currentProfile == null)
            return;
        
        // Form trendi
        if (formTrendCard != null)
        {
            List<float> trend = currentProfile.GetFormTrend();
            // Form trendi grafiği burada gösterilebilir
        }
        
        // Sakatlık geçmişi
        if (injuryHistoryCard != null)
        {
            TextMeshProUGUI injuryText = injuryHistoryCard.transform.Find("InjuryText")?.GetComponent<TextMeshProUGUI>();
            if (injuryText != null)
            {
                injuryText.text = $"Toplam Sakatlık: {currentProfile.injuryHistory.Count}";
            }
        }
        
        // Disiplin
        if (disciplineCard != null)
        {
            TextMeshProUGUI disciplineText = disciplineCard.transform.Find("DisciplineText")?.GetComponent<TextMeshProUGUI>();
            if (disciplineText != null)
            {
                disciplineText.text = $"Sarı Kart: {currentProfile.yellowCards}\nKırmızı Kart: {currentProfile.redCards}";
            }
        }
    }

    /// <summary>
    /// Sekme göster
    /// </summary>
    private void ShowTab(StatsTab tab)
    {
        currentTab = tab;
        
        // Tüm sekmeleri gizle
        if (seasonTabContent != null)
            seasonTabContent.SetActive(tab == StatsTab.Season);
        
        if (careerTabContent != null)
            careerTabContent.SetActive(tab == StatsTab.Career);
        
        if (attributesTabContent != null)
            attributesTabContent.SetActive(tab == StatsTab.Attributes);
        
        // Buton durumlarını güncelle
        UpdateTabButtons();
    }

    private void UpdateTabButtons()
    {
        // Aktif sekme butonunu vurgula
        // Bu kısım UI tasarımına göre özelleştirilebilir
    }

    private string GetPositionString(PlayerPosition position)
    {
        switch (position)
        {
            case PlayerPosition.STP: return "Stoper";
            case PlayerPosition.SĞB: return "Sağ Bek";
            case PlayerPosition.SLB: return "Sol Bek";
            case PlayerPosition.MDO: return "Merkez Orta Defans";
            case PlayerPosition.MOO: return "Merkez Orta Ofans";
            case PlayerPosition.SĞK: return "Sağ Kanat";
            case PlayerPosition.SLK: return "Sol Kanat";
            case PlayerPosition.SĞO: return "Sağ Orta";
            case PlayerPosition.SLO: return "Sol Orta";
            case PlayerPosition.SF: return "Santrafor";
            default: return "Bilinmeyen";
        }
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
    }

    private void OnTrainingButton()
    {
        // Antreman ekranına git
        // Şimdilik CareerHub'a dön
        OnBackButton();
    }

    private void OnEquipmentButton()
    {
        // Ekipman ekranına git
        // Şimdilik CareerHub'a dön
        OnBackButton();
    }
}

public enum StatsTab
{
    Season,
    Career,
    Attributes
}


