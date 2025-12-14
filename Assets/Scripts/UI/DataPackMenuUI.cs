using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Data Pack menü UI - Aktif ve indirilebilir data pack'leri gösterir
/// </summary>
public class DataPackMenuUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private Transform dataPackListParent; // Data pack listesinin parent'ı
    [SerializeField] private GameObject dataPackItemPrefab; // Data pack item prefab'ı
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI activeDataPackText;
    
    [Header("Data Pack Item UI")]
    [SerializeField] private TextMeshProUGUI packNameText;
    [SerializeField] private TextMeshProUGUI packVersionText;
    [SerializeField] private TextMeshProUGUI packAuthorText;
    [SerializeField] private TextMeshProUGUI packDescriptionText;
    [SerializeField] private Button useButton;
    
    private DataPack selectedDataPack;
    
    private DataPackManager dataPackManager;
    private List<GameObject> dataPackItems = new List<GameObject>();
    
    void Awake()
    {
        dataPackManager = DataPackManager.Instance;
        if (dataPackManager == null)
        {
            GameObject go = new GameObject("DataPackManager");
            dataPackManager = go.AddComponent<DataPackManager>();
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
        }
    }
    
    void Start()
    {
        LoadDataPacks();
        UpdateActiveDataPackDisplay();
    }
    
    /// <summary>
    /// Data pack'leri yükle ve listele
    /// </summary>
    void LoadDataPacks()
    {
        // Mevcut item'ları temizle
        ClearDataPackList();
        
        // DataPackManager'dan yüklü data pack'leri al
        List<DataPack> loadedPacks = dataPackManager.GetLoadedDataPacks();
        
        if (loadedPacks == null || loadedPacks.Count == 0)
        {
            Debug.LogWarning("[DataPackMenuUI] Yüklü data pack bulunamadı!");
            return;
        }
        
        // Her data pack için item oluştur
        foreach (var pack in loadedPacks)
        {
            CreateDataPackItem(pack);
        }
    }
    
    /// <summary>
    /// Data pack item'ı oluştur
    /// </summary>
    void CreateDataPackItem(DataPack pack)
    {
        if (dataPackListParent == null)
        {
            Debug.LogError("[DataPackMenuUI] DataPackListParent atanmamış!");
            return;
        }
        
        // Layout Group ekle (eğer yoksa)
        EnsureLayoutGroup();
        
        GameObject itemObj;
        
        // Prefab varsa kullan, yoksa runtime'da oluştur
        if (dataPackItemPrefab != null)
        {
            itemObj = Instantiate(dataPackItemPrefab, dataPackListParent);
        }
        else
        {
            itemObj = CreateDefaultDataPackItem(pack);
        }
        
        // Data pack bilgilerini göster
        UpdateDataPackItem(itemObj, pack);
        
        dataPackItems.Add(itemObj);
    }
    
    /// <summary>
    /// Layout Group'u kontrol et ve ekle (yoksa)
    /// </summary>
    void EnsureLayoutGroup()
    {
        if (dataPackListParent == null) return;
        
        // Vertical Layout Group var mı kontrol et
        VerticalLayoutGroup layoutGroup = dataPackListParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = dataPackListParent.gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10f; // Item'lar arası boşluk
            layoutGroup.padding = new RectOffset(10, 10, 10, 10); // Kenar boşlukları
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
        }
        
        // Content Size Fitter ekle (liste boyutunu otomatik ayarlar)
        ContentSizeFitter sizeFitter = dataPackListParent.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = dataPackListParent.gameObject.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
    
    /// <summary>
    /// Varsayılan data pack item'ı oluştur
    /// </summary>
    GameObject CreateDefaultDataPackItem(DataPack pack)
    {
        GameObject itemObj = new GameObject($"DataPackItem_{pack.packName}");
        itemObj.transform.SetParent(dataPackListParent);
        
        // RectTransform ekle
        RectTransform rectTransform = itemObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 120); // Width Layout Group tarafından kontrol edilecek
        
        // Image ekle (arka plan) - daha güzel görünüm
        Image bgImage = itemObj.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        
        // Layout Element ekle (Layout Group için)
        UnityEngine.UI.LayoutElement layoutElement = itemObj.AddComponent<UnityEngine.UI.LayoutElement>();
        layoutElement.preferredHeight = 120f;
        layoutElement.flexibleWidth = 1f;
        
        // Horizontal Layout Group ekle (içerik düzeni için)
        HorizontalLayoutGroup itemLayout = itemObj.AddComponent<HorizontalLayoutGroup>();
        itemLayout.spacing = 15f;
        itemLayout.padding = new RectOffset(15, 15, 10, 10);
        itemLayout.childControlWidth = false;
        itemLayout.childControlHeight = true;
        itemLayout.childForceExpandWidth = false;
        itemLayout.childForceExpandHeight = true;
        
        // Sol taraf - Bilgiler
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(itemObj.transform);
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        VerticalLayoutGroup leftLayout = leftPanel.AddComponent<VerticalLayoutGroup>();
        leftLayout.spacing = 5f;
        leftLayout.childControlWidth = true;
        leftLayout.childControlHeight = false;
        leftLayout.childForceExpandWidth = true;
        leftLayout.childForceExpandHeight = false;
        leftLayout.childAlignment = TextAnchor.MiddleLeft;
        
        LayoutElement leftLayoutElement = leftPanel.AddComponent<LayoutElement>();
        leftLayoutElement.flexibleWidth = 1f;
        leftLayoutElement.preferredWidth = 500f;
        
        // Pack Name Text
        GameObject nameObj = new GameObject("PackName");
        nameObj.transform.SetParent(leftPanel.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = pack.packName;
        nameText.fontSize = 22;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = new Color(1f, 0.9f, 0.3f); // Altın sarısı
        
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 30f;
        
        // Pack Version Text
        GameObject versionObj = new GameObject("PackVersion");
        versionObj.transform.SetParent(leftPanel.transform);
        RectTransform versionRect = versionObj.AddComponent<RectTransform>();
        TextMeshProUGUI versionText = versionObj.AddComponent<TextMeshProUGUI>();
        versionText.text = $"Versiyon: {pack.packVersion}";
        versionText.fontSize = 14;
        versionText.color = new Color(0.8f, 0.8f, 0.8f);
        
        LayoutElement versionLayout = versionObj.AddComponent<LayoutElement>();
        versionLayout.preferredHeight = 20f;
        
        // Pack Author Text
        GameObject authorObj = new GameObject("PackAuthor");
        authorObj.transform.SetParent(leftPanel.transform);
        RectTransform authorRect = authorObj.AddComponent<RectTransform>();
        TextMeshProUGUI authorText = authorObj.AddComponent<TextMeshProUGUI>();
        authorText.text = $"Yazar: {pack.packAuthor}";
        authorText.fontSize = 14;
        authorText.color = new Color(0.7f, 0.7f, 0.7f);
        
        LayoutElement authorLayout = authorObj.AddComponent<LayoutElement>();
        authorLayout.preferredHeight = 20f;
        
        // Pack Description Text (kısa)
        if (!string.IsNullOrEmpty(pack.packDescription))
        {
            GameObject descObj = new GameObject("PackDescription");
            descObj.transform.SetParent(leftPanel.transform);
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            string shortDesc = pack.packDescription.Length > 60 ? pack.packDescription.Substring(0, 60) + "..." : pack.packDescription;
            descText.text = shortDesc;
            descText.fontSize = 12;
            descText.color = new Color(0.6f, 0.6f, 0.6f);
            descText.enableWordWrapping = false;
            
            LayoutElement descLayout = descObj.AddComponent<LayoutElement>();
            descLayout.preferredHeight = 18f;
        }
        
        // Sağ taraf - Buton
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(itemObj.transform);
        RectTransform rightRect = rightPanel.AddComponent<RectTransform>();
        
        LayoutElement rightLayoutElement = rightPanel.AddComponent<LayoutElement>();
        rightLayoutElement.preferredWidth = 120f;
        rightLayoutElement.flexibleWidth = 0f;
        
        // Use Button
        GameObject buttonObj = new GameObject("UseButton");
        buttonObj.transform.SetParent(rightPanel.transform);
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.offsetMin = new Vector2(0, 20);
        buttonRect.offsetMax = new Vector2(0, 20);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.7f, 0.3f); // Daha parlak yeşil
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Button hover colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.7f, 0.3f);
        colors.highlightedColor = new Color(0.3f, 0.8f, 0.4f);
        colors.pressedColor = new Color(0.15f, 0.6f, 0.25f);
        button.colors = colors;
        
        // Button Text
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform);
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Kullan";
        buttonText.fontSize = 18;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        // Button click event
        button.onClick.AddListener(() => OnUseDataPack(pack));
        
        return itemObj;
    }
    
    /// <summary>
    /// Data pack item'ını güncelle
    /// </summary>
    void UpdateDataPackItem(GameObject itemObj, DataPack pack)
    {
        // Item içindeki text componentlerini bul ve güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        
        foreach (var text in texts)
        {
            if (text.name == "PackName")
            {
                text.text = pack.packName;
            }
            else if (text.name == "PackVersion")
            {
                text.text = $"Versiyon: {pack.packVersion}";
            }
            else if (text.name == "PackAuthor")
            {
                text.text = $"Yazar: {pack.packAuthor}";
            }
        }
        
        // Aktif data pack ise "Aktif" yazısı ekle
        DataPack activePack = dataPackManager.GetActiveDataPack();
        if (activePack == pack)
        {
            // "Aktif" badge ekle
            GameObject activeBadge = itemObj.transform.Find("ActiveBadge")?.gameObject;
            if (activeBadge == null)
            {
                activeBadge = new GameObject("ActiveBadge");
                activeBadge.transform.SetParent(itemObj.transform);
                RectTransform badgeRect = activeBadge.AddComponent<RectTransform>();
                badgeRect.anchorMin = new Vector2(0.85f, 0.7f);
                badgeRect.anchorMax = new Vector2(0.95f, 0.95f);
                badgeRect.offsetMin = Vector2.zero;
                badgeRect.offsetMax = Vector2.zero;
                
                Image badgeImage = activeBadge.AddComponent<Image>();
                badgeImage.color = new Color(0.2f, 0.8f, 0.2f);
                
                GameObject badgeTextObj = new GameObject("Text");
                badgeTextObj.transform.SetParent(activeBadge.transform);
                RectTransform badgeTextRect = badgeTextObj.AddComponent<RectTransform>();
                badgeTextRect.anchorMin = Vector2.zero;
                badgeTextRect.anchorMax = Vector2.one;
                badgeTextRect.offsetMin = Vector2.zero;
                badgeTextRect.offsetMax = Vector2.zero;
                
                TextMeshProUGUI badgeText = badgeTextObj.AddComponent<TextMeshProUGUI>();
                badgeText.text = "Aktif";
                badgeText.fontSize = 14;
                badgeText.alignment = TextAlignmentOptions.Center;
                badgeText.color = Color.white;
            }
            activeBadge.SetActive(true);
        }
    }
    
    /// <summary>
    /// Data pack listesini temizle
    /// </summary>
    void ClearDataPackList()
    {
        foreach (var item in dataPackItems)
        {
            if (item != null)
                Destroy(item);
        }
        dataPackItems.Clear();
    }
    
    /// <summary>
    /// Data pack kullan butonuna tıklandığında
    /// </summary>
    void OnUseDataPack(DataPack pack)
    {
        if (pack == null) return;
        
        dataPackManager.SetActiveDataPack(pack);
        selectedDataPack = pack;
        
        Debug.Log($"[DataPackMenuUI] Aktif Data Pack: {pack.packName}");
        
        // UI'ı güncelle
        LoadDataPacks();
        UpdateActiveDataPackDisplay();
    }
    
    /// <summary>
    /// Aktif data pack gösterimini güncelle
    /// </summary>
    void UpdateActiveDataPackDisplay()
    {
        if (activeDataPackText == null) return;
        
        DataPack activePack = dataPackManager.GetActiveDataPack();
        if (activePack != null)
        {
            activeDataPackText.text = $"Aktif Data Pack: {activePack.packName} v{activePack.packVersion}";
        }
        else
        {
            activeDataPackText.text = "Aktif Data Pack: Yok";
        }
    }
    
    /// <summary>
    /// Geri butonuna tıklandığında
    /// </summary>
    void OnBackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

