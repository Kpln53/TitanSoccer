using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Takım seçim ekranı - Yeni kariyer için takım seçimi
/// </summary>
public class TeamSelectionUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TextMeshProUGUI leagueNameText; // Seçilen lig adını göster
    [SerializeField] private Transform teamListParent;
    [SerializeField] private GameObject teamItemPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI infoText;
    
    private DataPackManager dataPackManager;
    private List<TeamData> availableTeams = new List<TeamData>();
    private TeamData selectedTeam;
    private LeagueData selectedLeague;
    
    // NewGameFlow'dan gelen bilgiler
    private string playerName;
    private string playerPosition;
    private string selectedLeagueName;
    
    void Awake()
    {
        dataPackManager = DataPackManager.Instance;
        if (dataPackManager == null)
        {
            GameObject go = new GameObject("DataPackManager");
            dataPackManager = go.AddComponent<DataPackManager>();
        }
        
        // GameManager'dan oyuncu bilgilerini al
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            playerName = GameManager.Instance.CurrentSave.playerName;
            playerPosition = GameManager.Instance.CurrentSave.position;
        }
    }
    
    void Start()
    {
        SetupButtons();
        LoadSelectedLeague();
    }
    
    /// <summary>
    /// Seçilen ligi yükle (GameManager'dan)
    /// </summary>
    void LoadSelectedLeague()
    {
        Debug.Log("[TeamSelectionUI] LoadSelectedLeague başlatılıyor...");
        
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("[TeamSelectionUI] GameManager veya CurrentSave bulunamadı!");
            if (infoText != null)
                infoText.text = "Hata: Kayıt bulunamadı!";
            return;
        }
        
        selectedLeagueName = GameManager.Instance.CurrentSave.leagueName;
        Debug.Log($"[TeamSelectionUI] Seçilen lig adı: '{selectedLeagueName}'");
        
        if (string.IsNullOrEmpty(selectedLeagueName))
        {
            Debug.LogError("[TeamSelectionUI] Seçilen lig adı bulunamadı!");
            if (infoText != null)
                infoText.text = "Hata: Lig seçilmedi!";
            return;
        }
        
        // Lig adını göster
        if (leagueNameText != null)
        {
            leagueNameText.text = $"Lig: {selectedLeagueName}";
        }
        
        // DataPackManager'dan ligi bul
        DataPack activePack = dataPackManager.GetActiveDataPack();
        if (activePack == null)
        {
            Debug.LogError("[TeamSelectionUI] Aktif Data Pack bulunamadı!");
            if (infoText != null)
                infoText.text = "Hata: Aktif Data Pack bulunamadı!";
            return;
        }
        
        Debug.Log($"[TeamSelectionUI] Aktif Data Pack: {activePack.packName}, Lig sayısı: {activePack.leagues.Count}");
        
        selectedLeague = activePack.GetLeagueByName(selectedLeagueName);
        if (selectedLeague == null)
        {
            Debug.LogError($"[TeamSelectionUI] Lig bulunamadı: {selectedLeagueName}");
            Debug.Log($"[TeamSelectionUI] Mevcut ligler:");
            foreach (var league in activePack.leagues)
            {
                Debug.Log($"  - {league.leagueName} ({league.teams?.Count ?? 0} takım)");
            }
            if (infoText != null)
                infoText.text = $"Hata: {selectedLeagueName} ligi bulunamadı!";
            return;
        }
        
        Debug.Log($"[TeamSelectionUI] Lig bulundu: {selectedLeague.leagueName}, Takım sayısı: {selectedLeague.teams?.Count ?? 0}");
        
        // En düşük güçlü 3 takımı bul ve göster
        FindLowestPowerTeams(selectedLeague);
        DisplayTeams();
    }
    
    /// <summary>
    /// Butonları ayarla
    /// </summary>
    void SetupButtons()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButton);
            confirmButton.interactable = false; // Başlangıçta pasif
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
        }
    }
    
    /// <summary>
    /// En düşük güçlü 3 takımı bul
    /// </summary>
    void FindLowestPowerTeams(LeagueData league)
    {
        availableTeams.Clear();
        
        if (league == null || league.teams == null || league.teams.Count == 0)
        {
            Debug.LogWarning("[TeamSelectionUI] Lig'de takım bulunamadı!");
            return;
        }
        
        // Tüm takımları güçlerine göre sırala (düşükten yükseğe)
        List<TeamData> sortedTeams = league.teams
            .Where(t => t != null)
            .OrderBy(t => t.GetTeamPower())
            .ToList();
        
        // En düşük 3 takımı al
        int teamCount = Mathf.Min(3, sortedTeams.Count);
        for (int i = 0; i < teamCount; i++)
        {
            availableTeams.Add(sortedTeams[i]);
        }
        
        Debug.Log($"[TeamSelectionUI] {league.leagueName} liginden en düşük güçlü {availableTeams.Count} takım bulundu:");
        foreach (var team in availableTeams)
        {
            Debug.Log($"  - {team.teamName} (Güç: {team.GetTeamPower()})");
        }
    }
    
    /// <summary>
    /// Takımları göster
    /// </summary>
    void DisplayTeams()
    {
        Debug.Log($"[TeamSelectionUI] DisplayTeams çağrıldı. teamListParent: {(teamListParent != null ? "Var" : "NULL")}, availableTeams: {availableTeams.Count}");
        
        if (teamListParent == null)
        {
            Debug.LogError("[TeamSelectionUI] teamListParent NULL! Inspector'da atanmalı!");
            if (infoText != null)
                infoText.text = "Hata: UI referansı eksik!";
            return;
        }
        
        // Mevcut item'ları temizle
        ClearTeamList();
        
        // Layout Group ekle (yoksa)
        EnsureLayoutGroup();
        
        if (availableTeams.Count == 0)
        {
            Debug.LogWarning("[TeamSelectionUI] Gösterilecek takım yok!");
            if (infoText != null)
                infoText.text = "Seçilen ligde takım bulunamadı!";
            return;
        }
        
        Debug.Log($"[TeamSelectionUI] {availableTeams.Count} takım gösteriliyor...");
        
        // Her takım için item oluştur
        foreach (var team in availableTeams)
        {
            CreateTeamItem(team);
        }
        
        Debug.Log($"[TeamSelectionUI] {availableTeams.Count} takım item'ı oluşturuldu.");
        
        if (infoText != null)
            infoText.text = $"{selectedLeague.leagueName} liginden en düşük güçlü {availableTeams.Count} takım gösteriliyor.";
    }
    
    /// <summary>
    /// Takım item'ı oluştur
    /// </summary>
    void CreateTeamItem(TeamData team)
    {
        GameObject itemObj;
        
        if (teamItemPrefab != null)
        {
            itemObj = Instantiate(teamItemPrefab, teamListParent);
        }
        else
        {
            itemObj = CreateDefaultTeamItem(team);
        }
        
        // Takım bilgilerini göster
        UpdateTeamItem(itemObj, team);
        
        // Seçim butonu ekle
        Button selectButton = itemObj.GetComponentInChildren<Button>();
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => OnTeamSelected(team));
        }
    }
    
    /// <summary>
    /// Varsayılan takım item'ı oluştur
    /// </summary>
    GameObject CreateDefaultTeamItem(TeamData team)
    {
        GameObject itemObj = new GameObject($"TeamItem_{team.teamName}");
        itemObj.transform.SetParent(teamListParent);
        
        RectTransform rectTransform = itemObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 150);
        
        // Arka plan
        Image bgImage = itemObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        
        LayoutElement layoutElement = itemObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 150f;
        layoutElement.flexibleWidth = 1f;
        
        // Horizontal Layout
        HorizontalLayoutGroup itemLayout = itemObj.AddComponent<HorizontalLayoutGroup>();
        itemLayout.spacing = 15f;
        itemLayout.padding = new RectOffset(15, 15, 10, 10);
        
        // Sol Panel - Takım Bilgileri
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(itemObj.transform);
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        VerticalLayoutGroup leftLayout = leftPanel.AddComponent<VerticalLayoutGroup>();
        leftLayout.spacing = 8f;
        leftLayout.childControlWidth = true;
        leftLayout.childControlHeight = false;
        leftLayout.childForceExpandWidth = true;
        
        LayoutElement leftLayoutElement = leftPanel.AddComponent<LayoutElement>();
        leftLayoutElement.flexibleWidth = 1f;
        
        // Takım Adı
        GameObject nameObj = new GameObject("TeamName");
        nameObj.transform.SetParent(leftPanel.transform);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = team.teamName;
        nameText.fontSize = 24;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = new Color(1f, 0.9f, 0.3f);
        
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 35f;
        
        // Takım Gücü
        GameObject powerObj = new GameObject("TeamPower");
        powerObj.transform.SetParent(leftPanel.transform);
        TextMeshProUGUI powerText = powerObj.AddComponent<TextMeshProUGUI>();
        powerText.text = $"Takım Gücü: {team.GetTeamPower()}";
        powerText.fontSize = 18;
        powerText.color = new Color(0.8f, 0.8f, 0.8f);
        
        LayoutElement powerLayout = powerObj.AddComponent<LayoutElement>();
        powerLayout.preferredHeight = 25f;
        
        // Oyuncu Sayısı
        GameObject playerCountObj = new GameObject("PlayerCount");
        playerCountObj.transform.SetParent(leftPanel.transform);
        TextMeshProUGUI playerCountText = playerCountObj.AddComponent<TextMeshProUGUI>();
        playerCountText.text = $"Oyuncu Sayısı: {team.players.Count}";
        playerCountText.fontSize = 16;
        playerCountText.color = new Color(0.7f, 0.7f, 0.7f);
        
        LayoutElement playerCountLayout = playerCountObj.AddComponent<LayoutElement>();
        playerCountLayout.preferredHeight = 22f;
        
        // Sağ Panel - Seç Butonu
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(itemObj.transform);
        RectTransform rightRect = rightPanel.AddComponent<RectTransform>();
        
        LayoutElement rightLayoutElement = rightPanel.AddComponent<LayoutElement>();
        rightLayoutElement.preferredWidth = 150f;
        
        // Seç Butonu
        GameObject buttonObj = new GameObject("SelectButton");
        buttonObj.transform.SetParent(rightPanel.transform);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.offsetMin = new Vector2(0, 30);
        buttonRect.offsetMax = new Vector2(0, 30);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.7f, 0.3f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.7f, 0.3f);
        colors.highlightedColor = new Color(0.3f, 0.8f, 0.4f);
        colors.pressedColor = new Color(0.15f, 0.6f, 0.25f);
        button.colors = colors;
        
        // Buton Text
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Seç";
        buttonText.fontSize = 20;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        return itemObj;
    }
    
    /// <summary>
    /// Takım item'ını güncelle
    /// </summary>
    void UpdateTeamItem(GameObject itemObj, TeamData team)
    {
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        
        foreach (var text in texts)
        {
            if (text.name == "TeamName")
            {
                text.text = team.teamName;
            }
            else if (text.name == "TeamPower")
            {
                text.text = $"Takım Gücü: {team.GetTeamPower()}";
            }
            else if (text.name == "PlayerCount")
            {
                text.text = $"Oyuncu Sayısı: {team.players.Count}";
            }
        }
        
        // Seçili takım ise vurgula
        if (selectedTeam == team)
        {
            Image bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.2f, 0.3f, 0.4f, 0.9f); // Mavi ton
            }
        }
    }
    
    /// <summary>
    /// Layout Group'u kontrol et ve ekle
    /// </summary>
    void EnsureLayoutGroup()
    {
        if (teamListParent == null) return;
        
        VerticalLayoutGroup layoutGroup = teamListParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = teamListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 15f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
        }
        
        ContentSizeFitter sizeFitter = teamListParent.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = teamListParent.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
    
    /// <summary>
    /// Takım listesini temizle
    /// </summary>
    void ClearTeamList()
    {
        for (int i = teamListParent.childCount - 1; i >= 0; i--)
        {
            Destroy(teamListParent.GetChild(i).gameObject);
        }
    }
    
    /// <summary>
    /// Takım seçildiğinde
    /// </summary>
    void OnTeamSelected(TeamData team)
    {
        selectedTeam = team;
        
        // Tüm item'ları güncelle (seçili olanı vurgula)
        for (int i = 0; i < teamListParent.childCount; i++)
        {
            GameObject itemObj = teamListParent.GetChild(i).gameObject;
            TeamData itemTeam = availableTeams[i];
            UpdateTeamItem(itemObj, itemTeam);
        }
        
        // Onay butonunu aktif et
        if (confirmButton != null)
        {
            confirmButton.interactable = true;
        }
        
        Debug.Log($"[TeamSelectionUI] Takım seçildi: {team.teamName}");
    }
    
    /// <summary>
    /// Onay butonuna tıklandığında
    /// </summary>
    void OnConfirmButton()
    {
        if (selectedTeam == null)
        {
            Debug.LogWarning("[TeamSelectionUI] Takım seçilmedi!");
            return;
        }
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("[TeamSelectionUI] GameManager bulunamadı!");
            return;
        }
        
        // SaveData'yı güncelle
        SaveData data = GameManager.Instance.CurrentSave;
        if (data == null)
        {
            data = new SaveData();
        }
        
        data.clubName = selectedTeam.teamName;
        
        // Kaydet
        int slotIndex = GameManager.Instance.CurrentSaveSlotIndex;
        if (slotIndex >= 0)
        {
            SaveSystem.SaveGame(data, slotIndex);
            GameManager.Instance.SetCurrentSave(data, slotIndex);
        }
        
        Debug.Log($"[TeamSelectionUI] Kariyer başlatılıyor: {data.playerName} - {data.clubName}");
        
        // CareerHub'a geç
        SceneManager.LoadScene("CareerHub");
    }
    
    /// <summary>
    /// Geri butonuna tıklandığında
    /// </summary>
    void OnBackButton()
    {
        SceneManager.LoadScene("NewGameFlow");
    }
}

