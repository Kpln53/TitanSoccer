using UnityEngine;

[CreateAssetMenu(fileName = "PositionProfile", menuName = "TitanSoccer/PositionProfile")]
public class PositionProfile : ScriptableObject
{
    public PlayerPosition position;

    [Header("Base Position (-1..1 normalize)")]
    [Range(-1f, 1f)] public float baseX;
    [Range(-1f, 1f)] public float baseY;

    [Header("Offsets")]
    public Vector2 attackOffset;   // Top bizdeyken
    public Vector2 defendOffset;   // Top rakipteyken

    [Header("Movement Limits")]
    public float maxRoamDistance = 4f; // oyuncu bölgesinden ne kadar uzaklaþabilir

    [Header("AI Role")]
    public PositionRole role = PositionRole.None;
}

