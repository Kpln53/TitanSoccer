using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum MarketItemType
{
    Featured,   // Öne Çıkanlar
    Currency,   // Para & Altın
    Packs,      // Paketler
    Equipment   // Ekipman
}

[System.Serializable]
public class MarketItemData
{
    public string id;
    public string title;
    public string description;
    public string priceText;
    public Sprite icon;
    public MarketItemType type;
    public bool isPopular;
    public bool isBestValue;
    public Color glowColor = new Color(1f, 0.8f, 0.2f); // Gold default
}

public class MarketItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Image iconImage;
    public Image backgroundImage;
    public Button buyButton;
    
    [Header("Badges")]
    public GameObject popularBadge;
    public GameObject bestValueBadge;
    
    private MarketItemData currentData;
    private Action<MarketItemData> onBuyClick;

    public void Setup(MarketItemData data, Action<MarketItemData> buyCallback)
    {
        currentData = data;
        onBuyClick = buyCallback;

        if (titleText) titleText.text = data.title;
        if (descriptionText) descriptionText.text = data.description;
        if (priceText) priceText.text = data.priceText;
        if (iconImage && data.icon) iconImage.sprite = data.icon;

        // Badges
        if (popularBadge) popularBadge.SetActive(data.isPopular);
        if (bestValueBadge) bestValueBadge.SetActive(data.isBestValue);

        // Button Setup
        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuyClick?.Invoke(currentData));
        }
    }
}
