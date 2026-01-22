using UnityEngine;

public class EnsureGameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (GameManager.Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
            Debug.Log("[EnsureGameManager] Created GameManager instance.");
        }
    }
}
