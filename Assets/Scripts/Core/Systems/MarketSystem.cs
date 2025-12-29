using UnityEngine;

/// <summary>
/// Market sistemi - Krampon ve lüks eşya satışı (Singleton)
/// </summary>
public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance { get; private set; }

    [Header("Krampon Fiyatları")]
    public int basicCleatsPrice = 500;
    public int standardCleatsPrice = 2000;
    public int premiumCleatsPrice = 5000;
    public int eliteCleatsPrice = 10000;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[MarketSystem] MarketSystem initialized.");
    }

    /// <summary>
    /// Krampon satın al
    /// </summary>
    public bool BuyCleats(EconomyData economy, CleatsTier tier)
    {
        if (economy == null)
        {
            Debug.LogWarning("[MarketSystem] EconomyData is null! Cannot buy cleats.");
            return false;
        }

        int price = GetCleatsPrice(tier);

        if (!economy.HasEnoughMoney(price))
        {
            Debug.Log($"[MarketSystem] Not enough money to buy cleats. Need: {price}, Have: {economy.money}");
            return false;
        }

        // Para harca
        economy.SpendMoney(price);

        // Kramponları oluştur
        BootsData newBoots = CreateBoots(tier);
        economy.currentBoots = newBoots;

        // Sahiplenilmiş item listesine ekle
        OwnedItem item = new OwnedItem
        {
            itemType = ItemType.Cleats,
            tier = (int)tier,
            itemName = GetBootsName(tier),
            isEquipped = true
        };
        economy.ownedItems.Add(item);

        Debug.Log($"[MarketSystem] Cleats purchased: {tier} for {price}€");
        return true;
    }

    /// <summary>
    /// Lüks eşya satın al
    /// </summary>
    public bool BuyLuxuryItem(EconomyData economy, LuxuryType luxuryType, string itemName, int price, int happinessBonus = 0)
    {
        if (economy == null)
        {
            Debug.LogWarning("[MarketSystem] EconomyData is null! Cannot buy luxury item.");
            return false;
        }

        if (!economy.HasEnoughMoney(price))
        {
            Debug.Log($"[MarketSystem] Not enough money to buy luxury item. Need: {price}, Have: {economy.money}");
            return false;
        }

        // Para harca
        economy.SpendMoney(price);

        // Lüks eşya oluştur
        LuxuryItem item = new LuxuryItem
        {
            luxuryType = luxuryType,
            itemName = itemName,
            purchasePrice = price,
            value = price, // Değer = fiyat (basit)
            happinessBonus = happinessBonus
        };

        economy.luxuryItems.Add(item);

        Debug.Log($"[MarketSystem] Luxury item purchased: {itemName} for {price}€");
        return true;
    }

    /// <summary>
    /// Enerji içeceği satın al
    /// </summary>
    public bool BuyEnergyDrink(EconomyData economy, int count = 1, int pricePerDrink = 50)
    {
        if (economy == null)
            return false;

        int totalPrice = pricePerDrink * count;

        if (!economy.HasEnoughMoney(totalPrice))
        {
            Debug.Log($"[MarketSystem] Not enough money to buy energy drinks. Need: {totalPrice}, Have: {economy.money}");
            return false;
        }

        economy.SpendMoney(totalPrice);
        economy.energyDrinkCount += count;

        Debug.Log($"[MarketSystem] {count} energy drink(s) purchased for {totalPrice}€");
        return true;
    }

    /// <summary>
    /// Rehabilitasyon ürünü satın al
    /// </summary>
    public bool BuyRehabItem(EconomyData economy, int count = 1, int pricePerItem = 100)
    {
        if (economy == null)
            return false;

        int totalPrice = pricePerItem * count;

        if (!economy.HasEnoughMoney(totalPrice))
        {
            Debug.Log($"[MarketSystem] Not enough money to buy rehab items. Need: {totalPrice}, Have: {economy.money}");
            return false;
        }

        economy.SpendMoney(totalPrice);
        economy.rehabItemCount += count;

        Debug.Log($"[MarketSystem] {count} rehab item(s) purchased for {totalPrice}€");
        return true;
    }

    /// <summary>
    /// Krampon fiyatını getir
    /// </summary>
    private int GetCleatsPrice(CleatsTier tier)
    {
        return tier switch
        {
            CleatsTier.Basic => basicCleatsPrice,
            CleatsTier.Standard => standardCleatsPrice,
            CleatsTier.Premium => premiumCleatsPrice,
            CleatsTier.Elite => eliteCleatsPrice,
            _ => basicCleatsPrice
        };
    }

    /// <summary>
    /// Krampon adını getir
    /// </summary>
    private string GetBootsName(CleatsTier tier)
    {
        return tier switch
        {
            CleatsTier.Basic => "Basic Cleats",
            CleatsTier.Standard => "Standard Cleats",
            CleatsTier.Premium => "Premium Cleats",
            CleatsTier.Elite => "Elite Cleats",
            _ => "Basic Cleats"
        };
    }

    /// <summary>
    /// Krampon oluştur (tier'a göre)
    /// </summary>
    private BootsData CreateBoots(CleatsTier tier)
    {
        BootsData boots = new BootsData
        {
            tier = tier,
            bootsName = GetBootsName(tier),
            durability = 100,
            maxDurability = 100,
            purchasePrice = GetCleatsPrice(tier)
        };

        // Tier'a göre bonusları ayarla
        switch (tier)
        {
            case CleatsTier.Basic:
                boots.speedBonus = 0;
                boots.dribblingBonus = 0;
                boots.shootingBonus = 0;
                boots.passingBonus = 0;
                break;

            case CleatsTier.Standard:
                boots.speedBonus = 2;
                boots.dribblingBonus = 2;
                boots.shootingBonus = 1;
                boots.passingBonus = 1;
                break;

            case CleatsTier.Premium:
                boots.speedBonus = 4;
                boots.dribblingBonus = 4;
                boots.shootingBonus = 3;
                boots.passingBonus = 3;
                break;

            case CleatsTier.Elite:
                boots.speedBonus = 6;
                boots.dribblingBonus = 6;
                boots.shootingBonus = 5;
                boots.passingBonus = 5;
                break;
        }

        return boots;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}


