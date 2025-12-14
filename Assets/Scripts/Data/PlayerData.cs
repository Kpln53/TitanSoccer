using UnityEngine;

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
    [Range(0, 100)] public int speed = 50;
    [Range(0, 100)] public int stamina = 50;
    [Range(0, 100)] public int defendingSkill = 50;
    [Range(0, 100)] public int physicalStrength = 50;
    
    [Header("Ek Bilgiler")]
    public int age = 25;
    public string nationality = "Unknown";
    
    /// <summary>
    /// Overall'ı yeteneklerden otomatik hesapla
    /// </summary>
    public void CalculateOverall()
    {
        float avg = (passingSkill + shootingSkill + dribblingSkill + speed + stamina + defendingSkill + physicalStrength) / 7f;
        overall = Mathf.RoundToInt(avg);
    }
    
    /// <summary>
    /// Overall'a göre yetenekleri otomatik ayarla
    /// </summary>
    public void SetOverall(int newOverall)
    {
        overall = Mathf.Clamp(newOverall, 0, 100);
        
        // Pozisyona göre yetenekleri ayarla
        int baseSkill = overall;
        int variation = 10; // ±10 varyasyon
        
        switch (position)
        {
            case PlayerPosition.KL: // Kaleci
                defendingSkill = baseSkill + Random.Range(-variation, variation);
                physicalStrength = baseSkill + Random.Range(-variation, variation);
                passingSkill = baseSkill - Random.Range(0, variation * 2);
                shootingSkill = baseSkill - Random.Range(0, variation * 2);
                dribblingSkill = baseSkill - Random.Range(0, variation * 2);
                speed = baseSkill - Random.Range(0, variation);
                stamina = baseSkill + Random.Range(-variation, variation);
                break;
                
            case PlayerPosition.STP: // Stoper
            case PlayerPosition.SĞB:
            case PlayerPosition.SLB:
                defendingSkill = baseSkill + Random.Range(-variation, variation);
                physicalStrength = baseSkill + Random.Range(-variation, variation);
                passingSkill = baseSkill + Random.Range(-variation, variation);
                shootingSkill = baseSkill - Random.Range(0, variation);
                dribblingSkill = baseSkill - Random.Range(0, variation);
                speed = baseSkill + Random.Range(-variation, variation);
                stamina = baseSkill + Random.Range(-variation, variation);
                break;
                
            case PlayerPosition.MDO: // Merkez Orta Defans
                defendingSkill = baseSkill + Random.Range(-variation, variation);
                passingSkill = baseSkill + Random.Range(-variation, variation);
                physicalStrength = baseSkill + Random.Range(-variation, variation);
                shootingSkill = baseSkill - Random.Range(0, variation);
                dribblingSkill = baseSkill + Random.Range(-variation, variation);
                speed = baseSkill + Random.Range(-variation, variation);
                stamina = baseSkill + Random.Range(-variation, variation);
                break;
                
            case PlayerPosition.MOO: // Merkez Orta Ofans
                passingSkill = baseSkill + Random.Range(-variation, variation);
                dribblingSkill = baseSkill + Random.Range(-variation, variation);
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
                speed = baseSkill + Random.Range(-variation, variation);
                passingSkill = baseSkill + Random.Range(-variation, variation);
                stamina = baseSkill + Random.Range(-variation, variation);
                defendingSkill = baseSkill - Random.Range(0, variation * 2);
                break;
        }
        
        // Tüm yetenekleri 0-100 arasında tut
        passingSkill = Mathf.Clamp(passingSkill, 0, 100);
        shootingSkill = Mathf.Clamp(shootingSkill, 0, 100);
        dribblingSkill = Mathf.Clamp(dribblingSkill, 0, 100);
        speed = Mathf.Clamp(speed, 0, 100);
        stamina = Mathf.Clamp(stamina, 0, 100);
        defendingSkill = Mathf.Clamp(defendingSkill, 0, 100);
        physicalStrength = Mathf.Clamp(physicalStrength, 0, 100);
    }
}

