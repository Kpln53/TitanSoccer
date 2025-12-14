using UnityEngine;

/// <summary>
/// Oyuncu pozisyon tipleri
/// </summary>
public enum PlayerPosition
{
    KL,   // Kaleci
    SĞB,  // Sağ Bek
    SLB,  // Sol Bek
    STP,  // Stoper
    SĞK,  // Sağ Kanat
    SLK,  // Sol Kanat
    MDO,  // Merkez Orta Defans
    MOO,  // Merkez Orta Ofans
    SĞO,  // Sağ Ofans
    SLO,  // Sol Ofans
    SF    // Santrafor
}

/// <summary>
/// Oyuncu pozisyon verisi - her mevkinin saha üzerindeki pozisyonu ve hareket alanı
/// </summary>
[System.Serializable]
public class PositionData
{
    public PlayerPosition position;
    public Vector2 normalizedPosition; // Saha üzerinde normalize pozisyon (-1 to 1)
    public Vector2 movementArea; // Hareket alanı (x: min, y: max)
    public bool isDefensive; // Defansif mi ofansif mi
    public float forwardLimit; // İleri gidebileceği maksimum z pozisyonu (normalize)
    public float backwardLimit; // Geri gidebileceği minimum z pozisyonu (normalize)
}

/// <summary>
/// Pozisyon yönetimi - tüm mevkilerin pozisyon ve hareket alanlarını tutar
/// </summary>
[CreateAssetMenu(fileName = "PositionConfig", menuName = "TitanSoccer/Position Config")]
public class PositionConfig : ScriptableObject
{
    [Header("Pozisyon Tanımları")]
    public PositionData[] positions;

    /// <summary>
    /// Belirli bir pozisyon için pozisyon verisini döndürür
    /// </summary>
    public PositionData GetPositionData(PlayerPosition position)
    {
        foreach (var pos in positions)
        {
            if (pos.position == position)
                return pos;
        }
        return null;
    }

    /// <summary>
    /// Normalize pozisyonu world pozisyonuna çevirir (2D)
    /// </summary>
    public Vector2 NormalizedToWorldPosition(Vector2 normalizedPos, PitchManager pitch)
    {
        float x = normalizedPos.x * (pitch.PitchWidth / 2f);
        float y = normalizedPos.y * (pitch.PitchLength / 2f);
        return new Vector2(x, y);
    }

    /// <summary>
    /// World pozisyonu normalize pozisyona çevirir (2D)
    /// </summary>
    public Vector2 WorldToNormalizedPosition(Vector2 worldPos, PitchManager pitch)
    {
        float x = worldPos.x / (pitch.PitchWidth / 2f);
        float y = worldPos.y / (pitch.PitchLength / 2f);
        return new Vector2(x, y);
    }
}

