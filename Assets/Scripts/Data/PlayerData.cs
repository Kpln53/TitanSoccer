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
}





