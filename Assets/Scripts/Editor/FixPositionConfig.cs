using UnityEngine;
using UnityEditor;

/// <summary>
/// Mevcut PositionConfig'i düzeltir
/// </summary>
public class FixPositionConfig
{
    [MenuItem("TitanSoccer/Fix Position Config")]
    static void FixConfig()
    {
        // PositionConfig'i bul - önce bilinen yolu kontrol et
        string[] possiblePaths = new string[]
        {
            "Assets/Scripts/Data/PositionConfig.asset",
            "Assets/Scripts/Data/DefaultPositionConfig.asset"
        };

        PositionConfig config = null;
        string path = null;

        // Önce bilinen yolları kontrol et
        foreach (var possiblePath in possiblePaths)
        {
            config = AssetDatabase.LoadAssetAtPath<PositionConfig>(possiblePath);
            if (config != null)
            {
                path = possiblePath;
                Debug.Log($"PositionConfig bulundu: {path}");
                break;
            }
        }

        // Bulunamazsa tüm projede ara
        if (config == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:PositionConfig");
            if (guids.Length == 0)
            {
                Debug.LogError("PositionConfig bulunamadı! Yeni bir tane oluşturuluyor...");
                CreateNewConfig();
                return;
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);
            config = AssetDatabase.LoadAssetAtPath<PositionConfig>(path);
        }

        if (config == null)
        {
            Debug.LogError("PositionConfig yüklenemedi!");
            return;
        }

        // Pozisyonları düzelt
        config.positions = new PositionData[]
        {
            // Kaleci
            new PositionData
            {
                position = PlayerPosition.KL,
                normalizedPosition = new Vector2(0, -0.9f),
                movementArea = new Vector2(-0.3f, 0.3f),
                isDefensive = true,
                forwardLimit = -0.7f,
                backwardLimit = -1f
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

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log($"PositionConfig düzeltildi: {path}");
        Debug.Log($"Toplam {config.positions.Length} pozisyon tanımlandı.");
    }

    static void CreateNewConfig()
    {
        PositionConfig config = ScriptableObject.CreateInstance<PositionConfig>();

        // Pozisyonları ayarla (FixConfig ile aynı)
        config.positions = new PositionData[]
        {
            new PositionData { position = PlayerPosition.KL, normalizedPosition = new Vector2(0, -0.9f), movementArea = new Vector2(-0.3f, 0.3f), isDefensive = true, forwardLimit = -0.7f, backwardLimit = -1f },
            new PositionData { position = PlayerPosition.SĞB, normalizedPosition = new Vector2(0.7f, -0.6f), movementArea = new Vector2(0.5f, 0.9f), isDefensive = true, forwardLimit = -0.3f, backwardLimit = -0.9f },
            new PositionData { position = PlayerPosition.SLB, normalizedPosition = new Vector2(-0.7f, -0.6f), movementArea = new Vector2(-0.9f, -0.5f), isDefensive = true, forwardLimit = -0.3f, backwardLimit = -0.9f },
            new PositionData { position = PlayerPosition.STP, normalizedPosition = new Vector2(0, -0.7f), movementArea = new Vector2(-0.3f, 0.3f), isDefensive = true, forwardLimit = -0.4f, backwardLimit = -0.9f },
            new PositionData { position = PlayerPosition.SĞK, normalizedPosition = new Vector2(0.8f, -0.2f), movementArea = new Vector2(0.6f, 1f), isDefensive = false, forwardLimit = 0.3f, backwardLimit = -0.5f },
            new PositionData { position = PlayerPosition.SLK, normalizedPosition = new Vector2(-0.8f, -0.2f), movementArea = new Vector2(-1f, -0.6f), isDefensive = false, forwardLimit = 0.3f, backwardLimit = -0.5f },
            new PositionData { position = PlayerPosition.MDO, normalizedPosition = new Vector2(0, -0.5f), movementArea = new Vector2(-0.4f, 0.4f), isDefensive = true, forwardLimit = 0f, backwardLimit = -0.7f },
            new PositionData { position = PlayerPosition.MOO, normalizedPosition = new Vector2(0, 0f), movementArea = new Vector2(-0.5f, 0.5f), isDefensive = false, forwardLimit = 0.5f, backwardLimit = -0.3f },
            new PositionData { position = PlayerPosition.SĞO, normalizedPosition = new Vector2(0.6f, 0.2f), movementArea = new Vector2(0.4f, 0.8f), isDefensive = false, forwardLimit = 0.7f, backwardLimit = -0.2f },
            new PositionData { position = PlayerPosition.SLO, normalizedPosition = new Vector2(-0.6f, 0.2f), movementArea = new Vector2(-0.8f, -0.4f), isDefensive = false, forwardLimit = 0.7f, backwardLimit = -0.2f },
            new PositionData { position = PlayerPosition.SF, normalizedPosition = new Vector2(0, 0.4f), movementArea = new Vector2(-0.3f, 0.3f), isDefensive = false, forwardLimit = 0.9f, backwardLimit = 0.1f }
        };

        string path = "Assets/Scripts/Data/PositionConfig.asset";
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;

        Debug.Log($"Yeni PositionConfig oluşturuldu: {path}");
    }
}

