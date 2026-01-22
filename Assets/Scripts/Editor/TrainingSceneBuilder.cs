using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using TitanSoccer.ChanceGameplay;

public class TrainingSceneBuilder : MonoBehaviour
{
    [MenuItem("TitanSoccer/Scenes/⚽ Build Training Scene")]
    public static void BuildScene()
    {
        // 1. Bootstrap (Tüm sistemi kurar)
        if (FindFirstObjectByType<ChanceSceneBootstrap>() == null)
        {
            GameObject boot = new GameObject("ChanceBootstrap");
            boot.AddComponent<ChanceSceneBootstrap>();
            Debug.Log("✅ Bootstrap created");
        }

        // 2. Training Manager
        if (FindFirstObjectByType<TrainingGameplayManager>() == null)
        {
            GameObject mgr = new GameObject("TrainingManager");
            TrainingGameplayManager script = mgr.AddComponent<TrainingGameplayManager>();
            
            // UI Canvas oluştur
            CreateTrainingUI(script);
            
            Debug.Log("✅ Training Manager created");
        }
    }

    private static void CreateTrainingUI(TrainingGameplayManager mgr)
    {
        GameObject canvasObj = new GameObject("TrainingUI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 101; // HUD'un üstünde
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Score Text (Top Left)
        mgr.scoreText = CreateText(canvasObj.transform, "Skor: 0", new Vector2(0, 1), new Vector2(0, 1), new Vector2(50, -50), 40, Color.white);
        
        // Difficulty Text (Top Right)
        mgr.difficultyText = CreateText(canvasObj.transform, "Seviye: 1", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-50, -50), 40, Color.yellow);

        // Return Button (Bottom Left)
        mgr.returnButton = CreateButton(canvasObj.transform, "ÇIKIŞ", new Vector2(0, 0), new Vector2(50, 50));

        // Game Over Panel
        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvasObj.transform, false);
        RectTransform pr = panel.AddComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.9f);

        mgr.finalScoreText = CreateText(panel.transform, "Skor: 0", new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, 60, Color.white);
        mgr.restartButton = CreateButton(panel.transform, "TEKRAR", new Vector2(0.5f, 0.4f), Vector2.zero);
        mgr.quitButton = CreateButton(panel.transform, "MENÜ", new Vector2(0.5f, 0.25f), Vector2.zero);

        mgr.gameOverPanel = panel;
        panel.SetActive(false);
    }

    private static TextMeshProUGUI CreateText(Transform parent, string text, Vector2 anchor, Vector2 pivot, Vector2 pos, int size, Color color)
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(400, 100);

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        
        return tmp;
    }

    private static Button CreateButton(Transform parent, string text, Vector2 anchor, Vector2 pos)
    {
        GameObject obj = new GameObject("Button");
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200, 60);

        Image img = obj.AddComponent<Image>();
        img.color = Color.white;

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(obj.transform, false);
        RectTransform tr = txtObj.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.color = Color.black;
        tmp.alignment = TextAlignmentOptions.Center;

        return obj.AddComponent<Button>();
    }
}
