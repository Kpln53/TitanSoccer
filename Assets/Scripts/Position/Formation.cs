using UnityEngine;

[CreateAssetMenu(fileName = "Formation", menuName = "TitanSoccer/Formation")]
public class Formation : ScriptableObject
{
    public PositionProfile[] positions = new PositionProfile[11];
}
