using System.Collections.Generic;

/// <summary>
/// Ekonomi verisi - Oyuncunun parası ve eşyaları
/// </summary>
[Serializable]
public class EconomyData
{
    [Header("Para")]
    public int money = 0;              // Mevcut para miktarı
    
    [Header("Eşyalar")]
    public List<OwnedItem> ownedItems; // Sahip olunan item'lar
    public BootsData currentBoots;     // Şu anki kramponlar (giyili olan)
    public List<LuxuryItem> luxuryItems; // Lüks eşyalar (ev, araba, vb.)
    
    [Header("Tüketilebilirler")]
    public int energyDrinkCount = 0;   // Enerji içeceği sayısı
    public int rehabItemCount = 0;     // Rehabilitasyon ürünü sayısı
    
    public EconomyData()
    {
        money = 10000; // Başlangıç parası
        ownedItems = new List<OwnedItem>();
        currentBoots = new BootsData();
        luxuryItems = new List<LuxuryItem>();
        energyDrinkCount = 0;
        rehabItemCount = 0;
    }
    
    /// <summary>
    /// Para ekle
    /// </summary>
    public void AddMoney(int amount)
    {
        money += amount;
        if (money < 0) money = 0; // Negatif para olmasın
    }
    
    /// <summary>
    /// Para harca (yeterli para varsa)
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Yeterli para var mı kontrol et
    /// </summary>
    public bool HasEnoughMoney(int amount)
    {
        return money >= amount;
    }
}

