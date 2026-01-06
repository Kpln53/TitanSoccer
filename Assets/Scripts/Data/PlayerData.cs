using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Oyuncu veri yapısı - Data Pack sistemi için
/// </summary>
[System.Serializable]
public class PlayerData
{
    [Header("Temel Bilgiler")]
    public string playerName;
    public PlayerPosition position;
    public int overall;
    
    [Header("Yetenekler (0-100)")]
    [Range(0, 100)] public int passingSkill = 50;
    [Range(0, 100)] public int shootingSkill = 50;
    [Range(0, 100)] public int dribblingSkill = 50;
    [Range(0, 100)] public int falsoSkill = 50; // Falso/Çalım yeteneği (yalan vuruş, sahte hareket)
    [Range(0, 100)] public int speed = 50;
    [Range(0, 100)] public int stamina = 50;
    [Range(0, 100)] public int defendingSkill = 50;
    [Range(0, 100)] public int physicalStrength = 50;
    
    [Header("Kaleci Özel Yetenekleri (Sadece Kaleci için kullanılır)")]
    [Range(0, 100)] public int saveReflex = 50;      // Kurtarış refleks yeteneği
    [Range(0, 100)] public int goalkeeperPositioning = 50; // Kaleci pozisyonu
    [Range(0, 100)] public int aerialAbility = 50;   // Havadan top yakalama
    [Range(0, 100)] public int oneOnOne = 50;        // 1v1 durumlar
    [Range(0, 100)] public int handling = 50;        // El kullanımı (top tutma)
    
    [Header("Ek Bilgiler")]
    public int age = 25;
    public string nationality = "Unknown";
    
    [Header("Değer ve Potansiyel")]
    public long marketValue = 500000;           // Piyasa değeri (€)
    [Range(0, 100)] public int potential = 75;  // Potansiyel overall (max ulaşabileceği)
    
    /// <summary>
    /// Overall'ı yeteneklerden otomatik hesapla
    /// </summary>
    public void CalculateOverall()
    {
        if (position == PlayerPosition.KL)
        {
            // Kaleci için özel hesaplama (kaleci yeteneklerini kullan)
            float avg = (saveReflex + goalkeeperPositioning + aerialAbility + oneOnOne + handling) / 5f;
            overall = Mathf.RoundToInt(avg);
        }
        else
        {
            // Diğer pozisyonlar için normal hesaplama (falso dahil)
            float avg = (passingSkill + shootingSkill + dribblingSkill + falsoSkill + speed + stamina + defendingSkill + physicalStrength) / 8f;
            overall = Mathf.RoundToInt(avg);
        }
    }
    
    /// <summary>
    /// Overall'a göre yetenekleri otomatik ayarla (pozisyona göre)
    /// </summary>
    public void SetOverall(int newOverall)
    {
        overall = Mathf.Clamp(newOverall, 0, 100);
        
        int baseSkill = overall;
        int variation = 10; // ±10 varyasyon
        
        if (position == PlayerPosition.KL)
        {
            // Kaleci için özel yetenekler
            saveReflex = baseSkill + Random.Range(-variation, variation);
            goalkeeperPositioning = baseSkill + Random.Range(-variation, variation);
            aerialAbility = baseSkill + Random.Range(-variation, variation);
            oneOnOne = baseSkill + Random.Range(-variation, variation);
            handling = baseSkill + Random.Range(-variation, variation);
            
            // Diğer yetenekleri zayıf yap (kaleci olmadığı için)
            passingSkill = baseSkill - Random.Range(0, variation * 2);
            shootingSkill = baseSkill - Random.Range(0, variation * 2);
            dribblingSkill = baseSkill - Random.Range(0, variation * 2);
            falsoSkill = baseSkill - Random.Range(0, variation * 2);
            speed = baseSkill - Random.Range(0, variation);
            stamina = baseSkill + Random.Range(-variation, variation);
            defendingSkill = baseSkill + Random.Range(-variation / 2, variation / 2); // Biraz defans bilgisi
            physicalStrength = baseSkill + Random.Range(-variation, variation);
        }
        else
        {
            // Diğer pozisyonlar için normal yetenekler
            switch (position)
            {
                case PlayerPosition.STP: // Stoper
                case PlayerPosition.SĞB:
                case PlayerPosition.SLB:
                    defendingSkill = baseSkill + Random.Range(-variation, variation);
                    physicalStrength = baseSkill + Random.Range(-variation, variation);
                    passingSkill = baseSkill + Random.Range(-variation, variation);
                    shootingSkill = baseSkill - Random.Range(0, variation);
                    dribblingSkill = baseSkill - Random.Range(0, variation);
                    falsoSkill = baseSkill - Random.Range(0, variation);
                    speed = baseSkill + Random.Range(-variation, variation);
                    stamina = baseSkill + Random.Range(-variation, variation);
                    break;
                    
                case PlayerPosition.MDO: // Merkez Orta Defans
                    defendingSkill = baseSkill + Random.Range(-variation, variation);
                    passingSkill = baseSkill + Random.Range(-variation, variation);
                    physicalStrength = baseSkill + Random.Range(-variation, variation);
                    shootingSkill = baseSkill - Random.Range(0, variation);
                    dribblingSkill = baseSkill + Random.Range(-variation, variation);
                    falsoSkill = baseSkill + Random.Range(-variation / 2, variation / 2);
                    speed = baseSkill + Random.Range(-variation, variation);
                    stamina = baseSkill + Random.Range(-variation, variation);
                    break;
                    
                case PlayerPosition.MOO: // Merkez Orta Ofans
                    passingSkill = baseSkill + Random.Range(-variation, variation);
                    dribblingSkill = baseSkill + Random.Range(-variation, variation);
                    falsoSkill = baseSkill + Random.Range(-variation, variation);
                    shootingSkill = baseSkill + Random.Range(-variation, variation);
                    speed = baseSkill + Random.Range(-variation, variation);
                    stamina = baseSkill + Random.Range(-variation, variation);
                    defendingSkill = baseSkill - Random.Range(0, variation);
                    physicalStrength = baseSkill + Random.Range(-variation, variation);
                    break;
                    
                case PlayerPosition.SĞK: // Sağ/Sol Kanat
                case PlayerPosition.SLK:
                case PlayerPosition.SĞO:
                case PlayerPosition.SLO:
                    speed = baseSkill + Random.Range(-variation, variation);
                    dribblingSkill = baseSkill + Random.Range(-variation, variation);
                    falsoSkill = baseSkill + Random.Range(-variation, variation); // Kanatlar için falso önemli
                    passingSkill = baseSkill + Random.Range(-variation, variation);
                    shootingSkill = baseSkill + Random.Range(-variation, variation);
                    stamina = baseSkill + Random.Range(-variation, variation);
                    defendingSkill = baseSkill - Random.Range(0, variation);
                    physicalStrength = baseSkill + Random.Range(-variation, variation);
                    break;
                    
                case PlayerPosition.SF: // Santrafor
                    shootingSkill = baseSkill + Random.Range(-variation, variation);
                    physicalStrength = baseSkill + Random.Range(-variation, variation);
                    dribblingSkill = baseSkill + Random.Range(-variation, variation);
                    falsoSkill = baseSkill + Random.Range(-variation, variation); // Santraforlar için falso önemli
                    speed = baseSkill + Random.Range(-variation, variation);
                    passingSkill = baseSkill + Random.Range(-variation, variation);
                    stamina = baseSkill + Random.Range(-variation, variation);
                    defendingSkill = baseSkill - Random.Range(0, variation * 2);
                    break;
            }
            
            // Kaleci yeteneklerini sıfırla (kaleci değilse)
            saveReflex = 0;
            goalkeeperPositioning = 0;
            aerialAbility = 0;
            oneOnOne = 0;
            handling = 0;
        }
        
        // Tüm yetenekleri 0-100 arasında tut
        passingSkill = Mathf.Clamp(passingSkill, 0, 100);
        shootingSkill = Mathf.Clamp(shootingSkill, 0, 100);
        dribblingSkill = Mathf.Clamp(dribblingSkill, 0, 100);
        falsoSkill = Mathf.Clamp(falsoSkill, 0, 100);
        speed = Mathf.Clamp(speed, 0, 100);
        stamina = Mathf.Clamp(stamina, 0, 100);
        defendingSkill = Mathf.Clamp(defendingSkill, 0, 100);
        physicalStrength = Mathf.Clamp(physicalStrength, 0, 100);
        saveReflex = Mathf.Clamp(saveReflex, 0, 100);
        goalkeeperPositioning = Mathf.Clamp(goalkeeperPositioning, 0, 100);
        aerialAbility = Mathf.Clamp(aerialAbility, 0, 100);
        oneOnOne = Mathf.Clamp(oneOnOne, 0, 100);
        handling = Mathf.Clamp(handling, 0, 100);
    }
    
    /// <summary>
    /// Kaleci mi kontrol et
    /// </summary>
    public bool IsGoalkeeper()
    {
        return position == PlayerPosition.KL;
    }
    
    /// <summary>
    /// Piyasa değerini hesapla (overall, yaş ve potansiyele göre)
    /// </summary>
    public void CalculateMarketValue()
    {
        // Base değer: overall'a göre üstel artış
        float baseValue = Mathf.Pow(overall, 2.5f) * 100f;
        
        // Yaş çarpanı (22-27 arası peak, sonra düşüş)
        float ageMultiplier = 1f;
        if (age < 22)
        {
            // Genç oyuncular potansiyele göre değerlenir
            float potentialBonus = (potential - overall) / 100f;
            ageMultiplier = 0.8f + potentialBonus * 1.5f;
        }
        else if (age <= 27)
        {
            // Peak yaş - tam değer
            ageMultiplier = 1f + (27 - age) * 0.02f;
        }
        else if (age <= 32)
        {
            // Düşüş başlıyor
            ageMultiplier = 1f - (age - 27) * 0.1f;
        }
        else
        {
            // 32+ ciddi düşüş
            ageMultiplier = 0.5f - (age - 32) * 0.1f;
        }
        ageMultiplier = Mathf.Max(0.1f, ageMultiplier);
        
        // Potansiyel bonusu (genç oyuncular için)
        float potentialMultiplier = 1f;
        if (age < 25 && potential > overall)
        {
            potentialMultiplier = 1f + (potential - overall) / 100f * 0.5f;
        }
        
        // Pozisyon çarpanı (santraforlar ve kanat oyuncuları daha değerli)
        float positionMultiplier = 1f;
        switch (position)
        {
            case PlayerPosition.SF:
                positionMultiplier = 1.3f;
                break;
            case PlayerPosition.SĞK:
            case PlayerPosition.SLK:
            case PlayerPosition.MOO:
                positionMultiplier = 1.2f;
                break;
            case PlayerPosition.KL:
                positionMultiplier = 0.8f;
                break;
        }
        
        // Final hesaplama
        marketValue = (long)(baseValue * ageMultiplier * potentialMultiplier * positionMultiplier);
        
        // Minimum değer: 50K €
        marketValue = System.Math.Max(50000L, marketValue);
    }
    
    /// <summary>
    /// Oyuncu gelişimi (sezon sonu çağrılır)
    /// </summary>
    public void DevelopPlayer(float performanceRating)
    {
        // Sadece potansiyeline ulaşmamış oyuncular gelişir
        if (overall >= potential) return;
        
        // Yaş kontrolü (30+ oyuncular pek gelişmez)
        if (age >= 30) return;
        
        // Gelişim miktarı: performans + yaş bonusu
        float developmentBase = 0f;
        
        // Performansa göre gelişim (6.0+ rating gerekli)
        if (performanceRating >= 7.5f)
        {
            developmentBase = 3f;  // Mükemmel performans
        }
        else if (performanceRating >= 7.0f)
        {
            developmentBase = 2f;  // Çok iyi
        }
        else if (performanceRating >= 6.5f)
        {
            developmentBase = 1f;  // İyi
        }
        else if (performanceRating >= 6.0f)
        {
            developmentBase = 0.5f;  // Orta
        }
        
        // Yaş bonusu (gençler daha hızlı gelişir)
        float ageBonus = 0f;
        if (age <= 21)
        {
            ageBonus = 1.5f;
        }
        else if (age <= 24)
        {
            ageBonus = 1f;
        }
        else if (age <= 27)
        {
            ageBonus = 0.5f;
        }
        
        // Potansiyel farkı bonusu (potansiyele uzaksa daha hızlı gelişir)
        float potentialGap = potential - overall;
        float gapBonus = potentialGap > 10 ? 0.5f : 0f;
        
        // Toplam gelişim
        int development = Mathf.RoundToInt(developmentBase + ageBonus + gapBonus);
        
        // Overall'ı artır (potansiyeli geçemez)
        overall = Mathf.Min(potential, overall + development);
        
        // Yetenekleri de orantılı artır
        if (development > 0)
        {
            int skillIncrease = development;
            passingSkill = Mathf.Min(99, passingSkill + Random.Range(0, skillIncrease + 1));
            shootingSkill = Mathf.Min(99, shootingSkill + Random.Range(0, skillIncrease + 1));
            dribblingSkill = Mathf.Min(99, dribblingSkill + Random.Range(0, skillIncrease + 1));
            speed = Mathf.Min(99, speed + Random.Range(0, skillIncrease + 1));
        }
        
        // Piyasa değerini güncelle
        CalculateMarketValue();
    }
    
    /// <summary>
    /// Oyuncu yaşlanması (her sezon çağrılır)
    /// </summary>
    public void AgePlayer()
    {
        age++;
        
        // 30+ yaşındaki oyuncular yavaşça düşer
        if (age >= 32)
        {
            int decline = Random.Range(1, 3);
            overall = Mathf.Max(40, overall - decline);
            speed = Mathf.Max(30, speed - Random.Range(1, 4));  // Hız en çok düşer
            stamina = Mathf.Max(30, stamina - Random.Range(1, 3));
        }
        else if (age >= 30)
        {
            if (Random.value < 0.3f)  // %30 şansla düşüş
            {
                overall = Mathf.Max(40, overall - 1);
                speed = Mathf.Max(30, speed - Random.Range(0, 2));
            }
        }
        
        // Piyasa değerini güncelle
        CalculateMarketValue();
    }
    
    /// <summary>
    /// Formatlı piyasa değeri string'i
    /// </summary>
    public string GetFormattedMarketValue()
    {
        if (marketValue >= 1000000000)
        {
            return $"€{marketValue / 1000000000f:F1}B";
        }
        else if (marketValue >= 1000000)
        {
            return $"€{marketValue / 1000000f:F1}M";
        }
        else if (marketValue >= 1000)
        {
            return $"€{marketValue / 1000f:F0}K";
        }
        return $"€{marketValue}";
    }
}






