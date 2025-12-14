using UnityEngine;

/// <summary>
/// Takım güç hesaplayıcı - Takım gücüne göre AI davranışlarını ayarlar
/// </summary>
public static class TeamPowerCalculator
{
    /// <summary>
    /// Takım gücüne göre AI saldırganlık seviyesi (0-1)
    /// </summary>
    public static float GetAggressionLevel(int teamPower)
    {
        // Güçlü takımlar daha saldırgan (70+ = 0.8, 50-70 = 0.5, 50- = 0.3)
        if (teamPower >= 70)
            return 0.8f;
        else if (teamPower >= 50)
            return 0.5f;
        else
            return 0.3f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI pozisyon alma şansı (0-1)
    /// </summary>
    public static float GetPositionTakingChance(int teamPower)
    {
        // Güçlü takımlar daha iyi pozisyon alır
        if (teamPower >= 70)
            return 0.9f;
        else if (teamPower >= 50)
            return 0.6f;
        else
            return 0.4f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI karar verme hızı (saniye)
    /// </summary>
    public static float GetDecisionSpeed(int teamPower)
    {
        // Güçlü takımlar daha hızlı karar verir
        if (teamPower >= 70)
            return 0.3f;
        else if (teamPower >= 50)
            return 0.5f;
        else
            return 0.7f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI top kovalamak için mesafe eşiği
    /// </summary>
    public static float GetChaseBallThreshold(int teamPower)
    {
        // Güçlü takımlar daha uzaktan topu kovalar
        if (teamPower >= 70)
            return 20f;
        else if (teamPower >= 50)
            return 15f;
        else
            return 10f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI pozisyon toleransı
    /// </summary>
    public static float GetPositionTolerance(int teamPower)
    {
        // Güçlü takımlar pozisyonlarını daha iyi korur
        if (teamPower >= 70)
            return 1.5f;
        else if (teamPower >= 50)
            return 2f;
        else
            return 3f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI pas yapma şansı
    /// </summary>
    public static float GetPassChance(int teamPower)
    {
        // Güçlü takımlar daha çok pas yapar
        if (teamPower >= 70)
            return 0.5f;
        else if (teamPower >= 50)
            return 0.3f;
        else
            return 0.2f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI baskı algılama mesafesi
    /// </summary>
    public static float GetPressureDetectionRange(int teamPower)
    {
        // Güçlü takımlar daha erken baskı algılar
        if (teamPower >= 70)
            return 7f;
        else if (teamPower >= 50)
            return 5f;
        else
            return 3f;
    }
    
    /// <summary>
    /// Takım gücüne göre AI pozisyon alma sıklığı (her X saniyede bir)
    /// </summary>
    public static float GetPositionTakingFrequency(int teamPower)
    {
        // Güçlü takımlar daha sık pozisyon alır
        if (teamPower >= 70)
            return 2f; // Her 2 saniyede bir
        else if (teamPower >= 50)
            return 3f; // Her 3 saniyede bir
        else
            return 5f; // Her 5 saniyede bir
    }
}

