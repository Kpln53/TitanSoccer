using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MarketUI : MonoBehaviour
{
    [Header("References")]
    public Transform contentParent;
    public GameObject itemPrefab;
    public ScrollRect scrollRect;

    [Header("Tabs")]
    public Button featuredTab;
    public Button currencyTab;
    public Button packsTab;
    public Button equipmentTab;

    // Dummy Data List
    private List<MarketItemData> allItems = new List<MarketItemData>();

    private void Start()
    {
        SetupTabs();
        GenerateDummyData(); // Gerçek veriler gelene kadar test verisi
        ShowTab(MarketItemType.Featured);
    }

    private void SetupTabs()
    {
        if (featuredTab) featuredTab.onClick.AddListener(() => ShowTab(MarketItemType.Featured));
        if (currencyTab) currencyTab.onClick.AddListener(() => ShowTab(MarketItemType.Currency));
        if (packsTab) packsTab.onClick.AddListener(() => ShowTab(MarketItemType.Packs));
        if (equipmentTab) equipmentTab.onClick.AddListener(() => ShowTab(MarketItemType.Equipment));
    }

    public void ShowTab(MarketItemType type)
    {
        // Clear existing
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Filter and Create
        var itemsToShow = allItems.Where(x => x.type == type).ToList();

        foreach (var itemData in itemsToShow)
        {
            GameObject obj = Instantiate(itemPrefab, contentParent);
            MarketItemUI ui = obj.GetComponent<MarketItemUI>();
            if (ui)
            {
                ui.Setup(itemData, OnBuyItem);
            }
        }
        
        // Reset Scroll
        if (scrollRect) scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnBuyItem(MarketItemData item)
    {
        Debug.Log($"Satın alma işlemi başlatıldı: {item.title} - {item.priceText}");
        // Buraya satın alma mantığı gelecek (IAP veya oyun içi para)
    }

    private void GenerateDummyData()
    {
        allItems.Clear();

        // --- ÖNE ÇIKANLAR ---
        allItems.Add(new MarketItemData {
            title = "BAŞLANGIÇ PAKETİ",
            description = "50.000 Para + 10 Enerji",
            priceText = "₺29.99 SATIN AL",
            type = MarketItemType.Featured,
            isPopular = true
        });

        allItems.Add(new MarketItemData {
            title = "YILDIZ YATIRIMI",
            description = "250.000 Para + 25 Enerji + 1 YP",
            priceText = "₺99.99 SATIN AL",
            type = MarketItemType.Featured,
            isBestValue = true
        });

        allItems.Add(new MarketItemData {
            title = "VIP ELİT PAKET",
            description = "1 Milyon Para + 50 Enerji + Özel VIP Forma",
            priceText = "₺249.99 SATIN AL",
            type = MarketItemType.Featured
        });

        // --- PARA & ALTIN ---
        allItems.Add(new MarketItemData {
            title = "Avuç Dolusu Altın",
            description = "100 Altın",
            priceText = "₺19.99",
            type = MarketItemType.Currency
        });
        
        allItems.Add(new MarketItemData {
            title = "Kasa Dolusu Para",
            description = "500.000 Para",
            priceText = "50 Altın",
            type = MarketItemType.Currency
        });

        // --- DİĞERLERİ ---
        allItems.Add(new MarketItemData {
            title = "Enerji İçeceği x5",
            description = "Enerjini %100 doldurur.",
            priceText = "10 Altın",
            type = MarketItemType.Packs
        });

        // --- YENİ EKLENEN ÜRÜN ---
        allItems.Add(new MarketItemData {
            title = "Süper Krampon",
            description = "Şut gücünü %10 artırır.",
            priceText = "500 Altın",
            type = MarketItemType.Equipment,
            isBestValue = true
        });
    }
}
