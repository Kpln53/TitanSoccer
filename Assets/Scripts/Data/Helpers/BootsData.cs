using UnityEngine;

/// <summary>
/// Krampon verisi - Oyuncunun giydiği kramponlar
/// </summary>
[System.Serializable]
public class BootsData
{
    [Header("Krampon Bilgileri")]
    public CleatsTier tier;        // Krampon seviyesi
    public string bootsName;       // Krampon adı
    
    [Header("İstatistik Bonusları")]
    public int speedBonus;         // Hız bonusu (0-10)
    public int dribblingBonus;     // Dripling bonusu (0-10)
    public int shootingBonus;      // Şut bonusu (0-10)
    public int passingBonus;       // Pas bonusu (0-10)
    
    [Header("Dayanıklılık")]
    public int durability;         // Dayanıklılık (0-100)
    public int maxDurability;      // Maksimum dayanıklılık (100)
    
    [Header("Satın Alma")]
    public int purchasePrice;      // Satın alma fiyatı
    
    public BootsData()
    {
        tier = CleatsTier.Basic;
        bootsName = "Basic Cleats";
        speedBonus = 0;
        dribblingBonus = 0;
        shootingBonus = 0;
        passingBonus = 0;
        durability = 100;
        maxDurability = 100;
        purchasePrice = 0;
    }
    
    /// <summary>
    /// Toplam bonus değerini getir
    /// </summary>
    public int GetTotalBonus()
    {
        return speedBonus + dribblingBonus + shootingBonus + passingBonus;
    }
    
    /// <summary>
    /// Dayanıklılık yüzdesini getir
    /// </summary>
    public float GetDurabilityPercent()
    {
        if (maxDurability <= 0) return 0f;
        return (float)durability / maxDurability;
    }
}

