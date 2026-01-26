using System.Collections.Generic;

/// <summary>
/// Ezeli rekabet verisi (Derbi)
/// </summary>
[System.Serializable]
public class RivalryData
{
    public string team1;
    public string team2;
    public string derbyName; // Örn: "Kıtalararası Derbi"
    public int intensity = 10; // 1-10 arası rekabet şiddeti
}
