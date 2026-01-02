using UnityEngine;

/// <summary>
/// Team AI Controller - Takım AI davranışı (placeholder - gelecekte genişletilecek)
/// </summary>
public class TeamAIController : MonoBehaviour
{
    [Header("Team Settings")]
    public int teamPower = 50;
    public float teamStrengthFactor = 1.0f; // 0.90-1.10 arası

    /// <summary>
    /// Team power'a göre strength factor hesapla
    /// </summary>
    public void CalculateTeamStrengthFactor(int baseTeamPower)
    {
        teamPower = baseTeamPower;
        // Basit mapping: 50 power = 1.0, 70 power = 1.1, 30 power = 0.9
        teamStrengthFactor = 0.9f + ((teamPower - 30f) / 200f) * 0.2f;
        teamStrengthFactor = Mathf.Clamp(teamStrengthFactor, 0.90f, 1.10f);
    }
}

