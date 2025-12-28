using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Market UI - Krampon ve eşya satın alma paneli
/// </summary>
public class MarketUI : MonoBehaviour
{
    [Header("Para Göstergesi")]
    public TextMeshProUGUI moneyText;

    [Header("Kramponlar")]
    public Transform bootsListParent;
    public GameObject bootsItemPrefab;

    [Header("Lüks Eşyalar")]
    public Transform luxuryListParent;
    public GameObject luxuryItemPrefab;

    [Header("Tüketilebilirler")]
    public Button buyEnergyDrinkButton;
    public Button buyRehabItemButton;
    public TextMeshProUGUI energyDrinkPriceText;
    public TextMeshProUGUI rehabItemPriceText;

    private void OnEnable()
    {
        RefreshData();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (buyEnergyDrinkButton != null)
            buyEnergyDrinkButton.onClick.AddListener(OnBuyEnergyDrinkButton);

        if (buyRehabItemButton != null)
            buyRehabItemButton.onClick.AddListener(OnBuyRehabItemButton);
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    private void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[MarketUI] No current save!");
            return;
        }

        EconomyData economy = GameManager.Instance.CurrentSave.economyData;
        if (economy == null) return;

        // Para göster
        if (moneyText != null)
            moneyText.text = $"Para: {economy.currentMoney:N0}€";

        // Fiyatları göster
        if (energyDrinkPriceText != null)
            energyDrinkPriceText.text = "50€";

        if (rehabItemPriceText != null)
            rehabItemPriceText.text = "100€";

        // Kramponları göster (TODO: MarketSystem'den krampon listesi alınacak)
        // DisplayBoots();

        // Lüks eşyaları göster (TODO: MarketSystem'den lüks eşya listesi alınacak)
        // DisplayLuxuryItems();
    }

    private void OnBuyEnergyDrinkButton()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        EconomyData economy = GameManager.Instance.CurrentSave.economyData;
        if (economy == null) return;

        int price = 50;

        if (MarketSystem.Instance != null)
        {
            MarketSystem.Instance.BuyEnergyDrink(economy, price);
        }
        else
        {
            if (economy.currentMoney >= price)
            {
                economy.AddMoney(-price);
                economy.energyDrinkCount++;
                Debug.Log("[MarketUI] Energy drink purchased!");
            }
            else
            {
                Debug.LogWarning("[MarketUI] Not enough money!");
            }
        }

        RefreshData();
    }

    private void OnBuyRehabItemButton()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave()) return;

        EconomyData economy = GameManager.Instance.CurrentSave.economyData;
        if (economy == null) return;

        int price = 100;

        if (MarketSystem.Instance != null)
        {
            MarketSystem.Instance.BuyRehabItem(economy, price);
        }
        else
        {
            if (economy.currentMoney >= price)
            {
                economy.AddMoney(-price);
                economy.rehabItemCount++;
                Debug.Log("[MarketUI] Rehab item purchased!");
            }
            else
            {
                Debug.LogWarning("[MarketUI] Not enough money!");
            }
        }

        RefreshData();
    }
}

