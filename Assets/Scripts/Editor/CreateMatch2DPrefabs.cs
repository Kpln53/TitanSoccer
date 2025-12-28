using UnityEngine;
using UnityEditor;

/// <summary>
/// 2D pozisyon sahnesi için prefab'ları oluşturan Editor script
/// </summary>
public class CreateMatch2DPrefabs : EditorWindow
{
    [MenuItem("TitanSoccer/Create 2D Match Prefabs")]
    public static void CreatePrefabs()
    {
        CreatePlayerPrefab();
        CreateBallPrefab();
        
        AssetDatabase.Refresh();
        Debug.Log("[CreateMatch2DPrefabs] Player and Ball prefabs created successfully!");
        EditorUtility.DisplayDialog("Success", "Player and Ball prefabs created successfully!\n\nPrefabs location:\n- Assets/Prefabs/Player.prefab\n- Assets/Prefabs/Ball.prefab", "OK");
    }

    /// <summary>
    /// Oyuncu prefab'ını oluştur
    /// </summary>
    private static void CreatePlayerPrefab()
    {
        // Prefabs klasörünü oluştur (yoksa)
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Oyuncu GameObject oluştur
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";

        // SpriteRenderer ekle
        SpriteRenderer spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
        
        // Basit bir sprite oluştur (kare şeklinde, renkli)
        Texture2D playerTexture = new Texture2D(32, 32);
        Color playerColor = new Color(0.2f, 0.6f, 1f, 1f); // Mavi renk
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                // Dairesel şekil oluştur
                float centerX = 16f;
                float centerY = 16f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                if (distance <= 14f)
                {
                    playerTexture.SetPixel(x, y, playerColor);
                }
                else
                {
                    playerTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        playerTexture.Apply();
        
        // Sprite oluştur
        Sprite playerSprite = Sprite.Create(
            playerTexture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            32f
        );
        
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.sortingOrder = 1; // Topun üstünde

        // Rigidbody2D ekle
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 5f; // Sürtünme
        rb.angularDamping = 5f;

        // CircleCollider2D ekle
        CircleCollider2D collider = playerObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;

        // PlayerController script'ini ekle
        PlayerController playerController = playerObj.AddComponent<PlayerController>();
        
        // PlayerController default değerlerini ayarla (Inspector'dan ayarlanabilir)
        // playerController.moveSpeed = 5f; // Default değerler script'te zaten var

        // Prefab olarak kaydet
        string prefabPath = prefabFolder + "/Player.prefab";
        PrefabUtility.SaveAsPrefabAsset(playerObj, prefabPath);
        
        // Scene'den sil
        DestroyImmediate(playerObj);
        AssetDatabase.Refresh();
        
        Debug.Log($"[CreateMatch2DPrefabs] Player prefab created at: {prefabPath}");
    }

    /// <summary>
    /// Top prefab'ını oluştur
    /// </summary>
    private static void CreateBallPrefab()
    {
        // Prefabs klasörünü oluştur (yoksa)
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Top GameObject oluştur
        GameObject ballObj = new GameObject("Ball");
        ballObj.tag = "Ball";

        // SpriteRenderer ekle
        SpriteRenderer spriteRenderer = ballObj.AddComponent<SpriteRenderer>();
        
        // Basit bir sprite oluştur (beyaz top)
        Texture2D ballTexture = new Texture2D(32, 32);
        Color ballColor = Color.white;
        
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                // Dairesel şekil oluştur
                float centerX = 16f;
                float centerY = 16f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                if (distance <= 14f)
                {
                    // Beyaz top, kenarlarda siyah çizgi
                    if (distance >= 12f)
                    {
                        ballTexture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        ballTexture.SetPixel(x, y, ballColor);
                    }
                }
                else
                {
                    ballTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        ballTexture.Apply();
        
        // Sprite oluştur
        Sprite ballSprite = Sprite.Create(
            ballTexture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            32f
        );
        
        spriteRenderer.sprite = ballSprite;
        spriteRenderer.sortingOrder = 0; // Oyuncuların altında

        // Rigidbody2D ekle
        Rigidbody2D rb = ballObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 2f; // Top için daha az sürtünme
        rb.angularDamping = 0.5f;
        rb.mass = 0.5f; // Hafif top

        // CircleCollider2D ekle
        CircleCollider2D collider = ballObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;
        collider.isTrigger = true; // Trigger olarak ayarla (OnTriggerEnter2D için)

        // BallController script'ini ekle
        BallController ballController = ballObj.AddComponent<BallController>();

        // Prefab olarak kaydet
        string prefabPath = prefabFolder + "/Ball.prefab";
        PrefabUtility.SaveAsPrefabAsset(ballObj, prefabPath);
        
        // Scene'den sil
        DestroyImmediate(ballObj);
        AssetDatabase.Refresh();
        
        Debug.Log($"[CreateMatch2DPrefabs] Ball prefab created at: {prefabPath}");
    }
}
