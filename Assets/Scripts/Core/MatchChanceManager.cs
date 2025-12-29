using UnityEngine;

/// <summary>
/// MatchChance verilerini geçici olarak saklamak için (sahne geçişleri için)
/// </summary>
public static class MatchChanceManager
{
    public static MatchChanceData CurrentChance { get; set; }
    
    public static void Clear()
    {
        CurrentChance = null;
    }
}

