using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Market sistemi - Enerji içeceği ve krampon satın alma
/// </summary>
public class MarketSystem : MonoBehaviour
{
    public static MarketSystem Instance;

    [Header("Ürün Fiyatları")]
    [SerializeField] private int energyDrinkPrice = 500; // Euro
    [SerializeField] private int rehabItemPrice = 1000; // Euro
    
    [Header("Krampon Fiyatları")]
    [SerializeField] private int basicCleatsPrice = 2000;
    [SerializeField] private int advancedCleatsPrice = 5000;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Enerji içeceği satın al
    /// </summary>
    public bool BuyEnergyDrink(EconomyData economy, int quantity = 1)
    {
        int totalCost = energyDrinkPrice * quantity;
        
        if (economy.money >= totalCost)
        {
            economy.money -= totalCost;
            economy.energyDrinks += quantity;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Rehab item satın al
    /// </summary>
    public bool BuyRehabItem(EconomyData economy, int quantity = 1)
    {
        int totalCost = rehabItemPrice * quantity;
        
        if (economy.money >= totalCost)
        {
            economy.money -= totalCost;
            economy.rehabItems += quantity;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Krampon satın al
    /// </summary>
    public bool BuyCleats(EconomyData economy, CleatsTier tier)
    {
        int price = GetCleatsPrice(tier);
        
        if (economy.money >= price)
        {
            economy.money -= price;
            economy.ownedItems.Add($"Cleats_{tier}");
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Krampon fiyatını al
    /// </summary>
    public int GetCleatsPrice(CleatsTier tier)
    {
        switch (tier)
        {
            case CleatsTier.Basic:
                return basicCleatsPrice;
            case CleatsTier.Advanced:
                return advancedCleatsPrice;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Krampon stat bonusunu al
    /// </summary>
    public int GetCleatsStatBonus(CleatsTier tier)
    {
        switch (tier)
        {
            case CleatsTier.Basic:
                return 2; // +2 stat
            case CleatsTier.Advanced:
                return 5; // +5 stat
            default:
                return 0;
        }
    }

    /// <summary>
    /// Krampon sahip mi?
    /// </summary>
    public bool HasCleats(EconomyData economy, CleatsTier tier)
    {
        return economy.ownedItems.Contains($"Cleats_{tier}");
    }
}

public enum CleatsTier
{
    Basic,    // Temel krampon (+2 stat)
    Advanced  // Gelişmiş krampon (+5 stat)
}

