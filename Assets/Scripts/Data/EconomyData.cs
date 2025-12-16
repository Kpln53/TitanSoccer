using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ekonomi verileri
/// </summary>
[System.Serializable]
public class EconomyData
{
    [Header("Para")]
    public int money = 0; // Euro cinsinden
    
    [Header("Envanter")]
    public int energyDrinks = 0;
    public int rehabItems = 0;
    
    [Header("Sahip Olunan Eşyalar")]
    public List<string> ownedItems = new List<string>(); // Ev, araba, kozmetik ID'leri
    
    [Header("Gelirler")]
    public int monthlySalary = 0;
    public int sponsorIncome = 0;
    public int bonusIncome = 0;
    
    [Header("Giderler")]
    public int monthlyExpenses = 0;
}

