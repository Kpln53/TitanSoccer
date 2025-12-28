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
        if (DataPackManager.Instance == null)
        {
            Debug.LogWarning("[DataPackMenuUI] DataPackManager not found!");
            return;
        }

        var packs = DataPackManager.Instance.GetAvailableDataPacks();

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
            itemObj = Instantiate(packItemPrefab, packListParent);
        }
        else
        {
            // Prefab yoksa runtime'da oluştur
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

        // Prefab varsa bile tıklama event'i ekle
        Button itemButton = itemObj.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnPackItemClicked(pack));
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
        if (detailPanel != null)
            detailPanel.SetActive(true);

        if (packNameText != null)
            packNameText.text = pack.packName;

        if (packVersionText != null)
            packVersionText.text = $"Version: {pack.packVersion}";

        if (packAuthorText != null)
            packAuthorText.text = $"Author: {pack.packAuthor}";

        if (packDescriptionText != null)
            packDescriptionText.text = pack.packDescription;

        // Buton durumlarını ayarla (şimdilik basit - gerçek implementasyon gelecekte)
        // TODO: İndirme/uygulama/kaldırma durumunu kontrol et
        if (downloadButton != null)
            downloadButton.gameObject.SetActive(true);

        if (applyButton != null)
            applyButton.gameObject.SetActive(false);

        if (removeButton != null)
            removeButton.gameObject.SetActive(false);
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

