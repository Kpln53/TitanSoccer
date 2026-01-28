using UnityEngine;
using UnityEditor;

public class TestSeasonEnd : MonoBehaviour
{
    [MenuItem("TitanSoccer/Debug/Force End Season")]
    public static void ForceEndSeason()
    {
        if (SeasonEndSystem.Instance == null)
        {
            // Create temporary instance if needed (though it should be in scene)
            GameObject go = new GameObject("SeasonEndSystem");
            go.AddComponent<SeasonEndSystem>();
        }

        SeasonEndSystem.Instance.EndSeason();
    }
}
