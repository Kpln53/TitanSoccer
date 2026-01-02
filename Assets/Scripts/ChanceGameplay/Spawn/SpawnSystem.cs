using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawn System - Oyuncu spawn sistemi (mevkiye göre pozisyonlar)
/// </summary>
public class SpawnSystem : MonoBehaviour
{
    [Header("Saha Boyutları")]
    public float fieldWidth = 20f;
    public float fieldHeight = 15f;

    /// <summary>
    /// 4-4-2 formasyonu pozisyonları
    /// </summary>
    public List<Vector3> GetFormationPositions(bool isHomeTeam)
    {
        List<Vector3> positions = new List<Vector3>();
        float yOffset = isHomeTeam ? -fieldHeight / 2 : fieldHeight / 2;
        float xCenter = 0f;

        // 4-4-2 formasyonu
        // Kaleci (0)
        positions.Add(new Vector3(xCenter, yOffset, 0));

        // Defans (1-4)
        positions.Add(new Vector3(xCenter - 3f, yOffset + 2f, 0)); // Sol bek
        positions.Add(new Vector3(xCenter - 1f, yOffset + 2.5f, 0)); // Sol stoper
        positions.Add(new Vector3(xCenter + 1f, yOffset + 2.5f, 0)); // Sağ stoper
        positions.Add(new Vector3(xCenter + 3f, yOffset + 2f, 0)); // Sağ bek

        // Orta saha (5-8)
        positions.Add(new Vector3(xCenter - 3f, yOffset + 5f, 0)); // Sol orta
        positions.Add(new Vector3(xCenter - 1f, yOffset + 5.5f, 0)); // Sol merkez
        positions.Add(new Vector3(xCenter + 1f, yOffset + 5.5f, 0)); // Sağ merkez
        positions.Add(new Vector3(xCenter + 3f, yOffset + 5f, 0)); // Sağ orta

        // Forvet (9-10)
        positions.Add(new Vector3(xCenter - 1.5f, yOffset + 8f, 0)); // Sol forvet
        positions.Add(new Vector3(xCenter + 1.5f, yOffset + 8f, 0)); // Sağ forvet

        return positions;
    }

    /// <summary>
    /// Controlled player spawn pozisyonu (pozisyon tipine göre)
    /// </summary>
    public Vector3 GetControlledPlayerSpawnPosition(bool isAttackChance, PlayerPosition playerPosition)
    {
        // Basit: Attack ise ileri, Defense ise geri
        float yPos = isAttackChance ? 3f : -3f;
        float xPos = 0f;

        // Pozisyona göre X ayarı
        switch (playerPosition)
        {
            case PlayerPosition.SĞK:
            case PlayerPosition.SĞO:
            case PlayerPosition.SĞB:
                xPos = 2f;
                break;
            case PlayerPosition.SLK:
            case PlayerPosition.SLO:
            case PlayerPosition.SLB:
                xPos = -2f;
                break;
            default:
                xPos = 0f;
                break;
        }

        return new Vector3(xPos, yPos, 0);
    }
}

