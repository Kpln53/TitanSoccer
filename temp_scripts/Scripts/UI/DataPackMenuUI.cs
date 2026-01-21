using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Data Pack menüsü UI - Data Pack listesi ve yönetimi
/// </summary>
public class DataPackMenuUI : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Transform packListParent;
    public GameObject packItemPrefab;
    public Button backButton;

    [Header("Detay Paneli")]
    public GameObject detailPanel;
    public TextMeshProUGUI packNameText;
    public TextMeshProUGUI packVersionText;
    public TextMeshProUGUI packAuthorText;
    public TextMeshProUGUI packDescriptionText;
    
    [Header("Detay İstatistikler")]
    public TextMeshProUGUI leagueCountText;
    public TextMeshProUGUI teamCountText;
    public TextMeshProUGUI playerCountText;
    public Image packLogoImage; // Pack logosu gösterimi için
    
    [Header("Detay Butonları")]
    public Button downloadButton;
    public Button applyButton;
    public Button removeButton;
    public Button closeDetailButton;

    private DataPack selectedPack;

    private void Start()
    {
        SetupButtons();
        LoadDataPacks();
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);

        if (downloadButton != null)
            downloadButton.onClick.AddListener(OnDownloadButton);

        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplyButton);

        if (removeButton != null)
            removeButton.onClick.AddListener(OnRemoveButton);

        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(OnCloseDetailButton);

        if (detailPanel != null)
            detailPanel.SetActive(false);
    }

    /// <summary>
    /// Data Pack'leri yükle ve listele
    /// </summary>
    private void LoadDataPacks()
    {
        // DataPackManager yoksa oluştur
        if (DataPackManager.Instance == null)
        {
            GameObject managerObj = new GameObject("DataPackManager");
            DataPackManager manager = managerObj.AddComponent<DataPackManager>();
            // Manuel olarak yükle (Start() henüz çağrılmamış olabilir)
            manager.LoadAvailableDataPacks();
            Debug.Log("[DataPackMenuUI] DataPackManager created automatically.");
        }
        else
        {
            // Eğer zaten varsa ama yüklenmemişse tekrar yükle
            DataPackManager.Instance.LoadAvailableDataPacks();
        }

        var packs = DataPackManager.Instance.GetAvailableDataPacks();
        
        Debug.Log($"[DataPackMenuUI] Found {packs.Count} DataPack(s) to display.");

        if (packListParent == null)
        {
            Debug.LogWarning("[DataPackMenuUI] Pack list parent not found!");
            return;
        }

        // Mevcut item'ları temizle
        foreach (Transform child in packListParent)
        {
            Destroy(child.gameObject);
        }

        // Her pack için item oluştur
        foreach (var pack in packs)
        {
            CreatePackItem(pack);
        }
    }

    /// <summary>
    /// Pack item oluştur
    /// </summary>
    private void CreatePackItem(DataPack pack)
    {
        GameObject itemObj;

        if (packItemPrefab != null)
        {
            // Prefab kullan
            itemObj = Instantiate(packItemPrefab, packListParent);
            
            // DataPackItemUI script'i varsa Setup metodunu çağır
            DataPackItemUI itemUI = itemObj.GetComponent<DataPackItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(pack);
                itemUI.SetOnClickCallback(OnPackItemClicked);
            }
            else
            {
                // Eski yöntem - Button varsa manuel bağla
                Button itemButton = itemObj.GetComponent<Button>();
                if (itemButton != null)
                {
                    itemButton.onClick.RemoveAllListeners();
                    itemButton.onClick.AddListener(() => OnPackItemClicked(pack));
                }
            }
        }
        else
        {
            // Prefab yoksa runtime'da oluştur (fallback)
            itemObj = new GameObject($"PackItem_{pack.packName}");
            itemObj.transform.SetParent(packListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 100);

            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

            Button button = itemObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnPackItemClicked(pack));

            // Pack adı text
            GameObject nameObj = new GameObject("PackName");
            nameObj.transform.SetParent(itemObj.transform);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, 0);

            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = pack.packName;
            nameText.fontSize = 18;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }

    /// <summary>
    /// Pack item'ına tıklandığında
    /// </summary>
    private void OnPackItemClicked(DataPack pack)
    {
        selectedPack = pack;
        ShowPackDetail(pack);
    }

    /// <summary>
    /// Pack detayını göster
    /// </summary>
    private void ShowPackDetail(DataPack pack)
    {
        if (pack == null)
        {
            Debug.LogWarning("[DataPackMenuUI] Trying to show detail for null DataPack!");
            return;
        }

        if (detailPanel != null)
            detailPanel.SetActive(true);

        // Temel Bilgiler
        if (packNameText != null)
            packNameText.text = pack.packName ?? "Unknown Pack";

        if (packVersionText != null)
            packVersionText.text = $"Versiyon: {pack.packVersion ?? "1.0.0"}";

        if (packAuthorText != null)
            packAuthorText.text = $"Yazar: {pack.packAuthor ?? "Unknown"}";

        if (packDescriptionText != null)
            packDescriptionText.text = pack.packDescription ?? "Açıklama bulunmuyor.";

        // Pack Logosu
        if (packLogoImage != null)
        {
            if (pack.packLogo != null)
            {
                packLogoImage.sprite = pack.packLogo;
                packLogoImage.gameObject.SetActive(true);
            }
            else
            {
                packLogoImage.gameObject.SetActive(false);
            }
        }

        // İstatistikler - DataPack helper metodlarını kullan
        int leagueCount = pack.leagues != null ? pack.leagues.Count : 0;
        int teamCount = pack.GetTotalTeamCount();
        int playerCount = pack.GetTotalPlayerCount();

        if (leagueCountText != null)
            leagueCountText.text = $"Lig Sayısı: {leagueCount}";

        if (teamCountText != null)
            teamCountText.text = $"Takım Sayısı: {teamCount}";

        if (playerCountText != null)
            playerCountText.text = $"Oyuncu Sayısı: {playerCount}";

        // Buton durumlarını ayarla
        bool isActive = DataPackManager.Instance != null && 
                       DataPackManager.Instance.activeDataPack == pack;

        if (downloadButton != null)
            downloadButton.gameObject.SetActive(false); // Şimdilik kapalı (indirme sistemi yok)

        if (applyButton != null)
        {
            applyButton.gameObject.SetActive(!isActive);
            // Eğer aktifse "Zaten Aktif" yazısı gösterilebilir
        }

        if (removeButton != null)
        {
            removeButton.gameObject.SetActive(isActive);
            // Sadece aktif pack için kaldırma butonu göster
        }
    }

    private void OnDownloadButton()
    {
        Debug.Log($"[DataPackMenuUI] Download button clicked for: {selectedPack?.packName}");
        // TODO: İndirme implementasyonu
    }

    private void OnApplyButton()
    {
        if (selectedPack != null && DataPackManager.Instance != null)
        {
            DataPackManager.Instance.SetActiveDataPack(selectedPack);
            Debug.Log($"[DataPackMenuUI] DataPack applied: {selectedPack.packName}");
        }
    }

    private void OnRemoveButton()
    {
        if (selectedPack != null && DataPackManager.Instance != null)
        {
            DataPackManager.Instance.ClearActiveDataPack();
            Debug.Log($"[DataPackMenuUI] DataPack removed: {selectedPack.packName}");
            
            // Detay panelini yenile (buton durumları değişti)
            ShowPackDetail(selectedPack);
        }
    }

    private void OnCloseDetailButton()
    {
        if (detailPanel != null)
            detailPanel.SetActive(false);

        selectedPack = null;
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

}

