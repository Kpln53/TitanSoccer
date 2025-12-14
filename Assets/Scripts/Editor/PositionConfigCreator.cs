using UnityEngine;
using UnityEditor;

/// <summary>
/// PositionConfig ScriptableObject oluşturucu - Editor script
/// </summary>
public class PositionConfigCreator
{
    [MenuItem("TitanSoccer/Create Default Position Config")]
    static void CreateDefaultPositionConfig()
    {
        PositionConfig config = ScriptableObject.CreateInstance<PositionConfig>();

        // 11 mevki için pozisyon tanımları
        config.positions = new PositionData[]
        {
            // Kaleci
            new PositionData
            {
                position = PlayerPosition.KL,
                normalizedPosition = new Vector2(0, -0.9f), // Kale önü
                movementArea = new Vector2(-0.3f, 0.3f), // X ekseninde hareket alanı
                isDefensive = true,
                forwardLimit = -0.7f, // İleri gidemez
                backwardLimit = -1f   // Kale çizgisinde
            },

            // Sağ Bek
            new PositionData
            {
                position = PlayerPosition.SĞB,
                normalizedPosition = new Vector2(0.7f, -0.6f),
                movementArea = new Vector2(0.5f, 0.9f),
                isDefensive = true,
                forwardLimit = -0.3f,
                backwardLimit = -0.9f
            },

            // Sol Bek
            new PositionData
            {
                position = PlayerPosition.SLB,
                normalizedPosition = new Vector2(-0.7f, -0.6f),
                movementArea = new Vector2(-0.9f, -0.5f),
                isDefensive = true,
                forwardLimit = -0.3f,
                backwardLimit = -0.9f
            },

            // Stoper
            new PositionData
            {
                position = PlayerPosition.STP,
                normalizedPosition = new Vector2(0, -0.7f),
                movementArea = new Vector2(-0.3f, 0.3f),
                isDefensive = true,
                forwardLimit = -0.4f,
                backwardLimit = -0.9f
            },

            // Sağ Kanat
            new PositionData
            {
                position = PlayerPosition.SĞK,
                normalizedPosition = new Vector2(0.8f, -0.2f),
                movementArea = new Vector2(0.6f, 1f),
                isDefensive = false,
                forwardLimit = 0.3f,
                backwardLimit = -0.5f
            },

            // Sol Kanat
            new PositionData
            {
                position = PlayerPosition.SLK,
                normalizedPosition = new Vector2(-0.8f, -0.2f),
                movementArea = new Vector2(-1f, -0.6f),
                isDefensive = false,
                forwardLimit = 0.3f,
                backwardLimit = -0.5f
            },

            // Merkez Orta Defans
            new PositionData
            {
                position = PlayerPosition.MDO,
                normalizedPosition = new Vector2(0, -0.5f),
                movementArea = new Vector2(-0.4f, 0.4f),
                isDefensive = true,
                forwardLimit = 0f,
                backwardLimit = -0.7f
            },

            // Merkez Orta Ofans
            new PositionData
            {
                position = PlayerPosition.MOO,
                normalizedPosition = new Vector2(0, 0f),
                movementArea = new Vector2(-0.5f, 0.5f),
                isDefensive = false,
                forwardLimit = 0.5f,
                backwardLimit = -0.3f
            },

            // Sağ Ofans
            new PositionData
            {
                position = PlayerPosition.SĞO,
                normalizedPosition = new Vector2(0.6f, 0.2f),
                movementArea = new Vector2(0.4f, 0.8f),
                isDefensive = false,
                forwardLimit = 0.7f,
                backwardLimit = -0.2f
            },

            // Sol Ofans
            new PositionData
            {
                position = PlayerPosition.SLO,
                normalizedPosition = new Vector2(-0.6f, 0.2f),
                movementArea = new Vector2(-0.8f, -0.4f),
                isDefensive = false,
                forwardLimit = 0.7f,
                backwardLimit = -0.2f
            },

            // Santrafor
            new PositionData
            {
                position = PlayerPosition.SF,
                normalizedPosition = new Vector2(0, 0.4f),
                movementArea = new Vector2(-0.3f, 0.3f),
                isDefensive = false,
                forwardLimit = 0.9f,
                backwardLimit = 0.1f
            }
        };

        // Asset olarak kaydet
        string path = "Assets/Scripts/Data/DefaultPositionConfig.asset";
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;

        Debug.Log($"PositionConfig oluşturuldu: {path}");
    }
}

