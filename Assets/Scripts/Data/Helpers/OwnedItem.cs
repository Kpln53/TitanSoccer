using System;

/// <summary>
/// Sahip olunan item - Ekonomi sistemi için
/// </summary>
[Serializable]
public class OwnedItem
{
    [Header("Item Bilgileri")]
    public ItemType itemType;      // Item türü
    public int tier;               // Seviye/Tier (CleatsTier için kullanılabilir)
    public string itemName;        // Item adı
    
    [Header("Satın Alma")]
    public DateTime purchaseDate;  // Satın alma tarihi
    public string purchaseDateString; // Tarih (string)
    public int purchasePrice;      // Satın alma fiyatı
    
    [Header("Durum")]
    public bool isEquipped;        // Kullanılıyor mu? (kramponlar için)
    
    public OwnedItem()
    {
        itemType = ItemType.Cleats;
        tier = 0;
        purchaseDate = DateTime.Now;
        purchaseDateString = purchaseDate.ToString("yyyy-MM-dd");
        isEquipped = false;
    }
}

