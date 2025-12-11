using UnityEngine;

public class MatchBootstrap : MonoBehaviour
{
    [SerializeField] private MatchSceneUI ui;
    [SerializeField] private TouchInputController inputController;

    void Start()
    {
        if (inputController != null)
            inputController.enabled = false; // maça kadar kapalý

        if (ui != null)
            ui.OnStartPressed += HandleMatchStart;
    }

    void HandleMatchStart()
    {
        if (inputController != null)
            inputController.enabled = true;

        // Ýstersen buraya:
        // - Zamaný baþlat
        // - Kamera takip modunu aç
        // - Spiker "Maç baþladý" vs. ekleyebiliriz
        Debug.Log("Match started – input enabled.");
    }

    void OnDestroy()
    {
        if (ui != null)
            ui.OnStartPressed -= HandleMatchStart;
    }
}
