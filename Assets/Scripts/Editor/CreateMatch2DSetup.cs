using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 2D Maç sahnesi için tüm prefab ve sahne yapısını oluşturan Editor script
/// Canvas 1080x1920, Saha 120x120, Merkez (540.6165, 963.9995)
/// </summary>
public class CreateMatch2DSetup : EditorWindow
{
    [MenuItem("TitanSoccer/Create Match 2D Setup (Complete)")]
    public static void CreateCompleteSetup()
    {
        CreatePlayerPrefab();
        CreateBallPrefab();
        CreateFieldSetup();
        
        AssetDatabase.Refresh();
        Debug.Log("[CreateMatch2DSetup] Complete 2D match setup created successfully!");
        EditorUtility.DisplayDialog("Success", 
            "Complete 2D match setup created!\n\n" +
            "✓ Player prefab created\n" +
            "✓ Ball prefab created\n" +
            "✓ Field setup guide created\n\n" +
            "See console for details.", 
            "OK");
    }

    /// <summary>
    /// Oyuncu prefab'ını oluştur - 120x120 saha için uygun boyutta
    /// </summary>
    private static void CreatePlayerPrefab()
    {
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Eski prefab'ı sil
        string oldPrefabPath = prefabFolder + "/Player.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(oldPrefabPath) != null)
        {
            AssetDatabase.DeleteAsset(oldPrefabPath);
        }

        // Oyuncu GameObject oluştur
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.layer = LayerMask.NameToLayer("Default");

        // SpriteRenderer ekle - 120x120 saha için uygun boyut (8x8 unity units)
        SpriteRenderer spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
        
        // Daha büyük sprite oluştur (120x120 saha için)
        Texture2D playerTexture = new Texture2D(64, 64);
        Color playerColor = new Color(0.2f, 0.6f, 1f, 1f); // Mavi renk
        
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float centerX = 32f;
                float centerY = 32f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                // Dairesel oyuncu, daha büyük
                if (distance <= 28f)
                {
                    playerTexture.SetPixel(x, y, playerColor);
                }
                else if (distance <= 30f)
                {
                    // Kenar çizgisi
                    playerTexture.SetPixel(x, y, new Color(playerColor.r * 0.5f, playerColor.g * 0.5f, playerColor.b * 0.5f, 1f));
                }
                else
                {
                    playerTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        playerTexture.Apply();
        
        // Sprite oluştur - 8 unity units boyutunda (120 saha için uygun)
        Sprite playerSprite = Sprite.Create(
            playerTexture,
            new Rect(0, 0, 64, 64),
            new Vector2(0.5f, 0.5f),
            8f // pixelsPerUnit: 8 (64 pixel / 8 = 8 unity units)
        );
        
        spriteRenderer.sprite = playerSprite;
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.color = Color.white;

        // Rigidbody2D ekle
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.freezeRotation = true; // Rotasyonu dondur

        // CircleCollider2D ekle - oyuncu boyutuna göre
        CircleCollider2D collider = playerObj.AddComponent<CircleCollider2D>();
        collider.radius = 3.5f; // 8 unity units / 2.3 ≈ 3.5

        // PlayerController script'ini ekle
        PlayerController playerController = playerObj.AddComponent<PlayerController>();

        // Prefab olarak kaydet
        string prefabPath = prefabFolder + "/Player.prefab";
        PrefabUtility.SaveAsPrefabAsset(playerObj, prefabPath);
        
        DestroyImmediate(playerObj);
        AssetDatabase.Refresh();
        
        Debug.Log($"[CreateMatch2DSetup] Player prefab created at: {prefabPath}");
    }

    /// <summary>
    /// Top prefab'ını oluştur - 120x120 saha için uygun boyutta
    /// </summary>
    private static void CreateBallPrefab()
    {
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Eski prefab'ı sil
        string oldPrefabPath = prefabFolder + "/Ball.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(oldPrefabPath) != null)
        {
            AssetDatabase.DeleteAsset(oldPrefabPath);
        }

        // Top GameObject oluştur
        GameObject ballObj = new GameObject("Ball");
        ballObj.tag = "Ball";
        ballObj.layer = LayerMask.NameToLayer("Default");

        // SpriteRenderer ekle
        SpriteRenderer spriteRenderer = ballObj.AddComponent<SpriteRenderer>();
        
        // Top sprite'ı oluştur (oyuncudan daha küçük)
        Texture2D ballTexture = new Texture2D(48, 48);
        Color ballColor = Color.white;
        
        for (int x = 0; x < 48; x++)
        {
            for (int y = 0; y < 48; y++)
            {
                float centerX = 24f;
                float centerY = 24f;
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                
                if (distance <= 20f)
                {
                    // Beyaz top, kenarlarda siyah çizgi
                    if (distance >= 18f)
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
        
        // Sprite oluştur - 6 unity units boyutunda
        Sprite ballSprite = Sprite.Create(
            ballTexture,
            new Rect(0, 0, 48, 48),
            new Vector2(0.5f, 0.5f),
            8f // pixelsPerUnit: 8 (48 pixel / 8 = 6 unity units)
        );
        
        spriteRenderer.sprite = ballSprite;
        spriteRenderer.sortingOrder = 0; // Oyuncuların altında
        spriteRenderer.color = Color.white;

        // Rigidbody2D ekle
        Rigidbody2D rb = ballObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 2f;
        rb.angularDamping = 0.5f;
        rb.mass = 0.5f;
        rb.freezeRotation = true;

        // CircleCollider2D ekle
        CircleCollider2D collider = ballObj.AddComponent<CircleCollider2D>();
        collider.radius = 2.5f; // 6 unity units / 2.4 ≈ 2.5
        collider.isTrigger = false; // Normal collider

        // BallController script'ini ekle
        BallController ballController = ballObj.AddComponent<BallController>();

        // Prefab olarak kaydet
        string prefabPath = prefabFolder + "/Ball.prefab";
        PrefabUtility.SaveAsPrefabAsset(ballObj, prefabPath);
        
        DestroyImmediate(ballObj);
        AssetDatabase.Refresh();
        
        Debug.Log($"[CreateMatch2DSetup] Ball prefab created at: {prefabPath}");
    }

    /// <summary>
    /// Saha kurulum rehberi oluştur
    /// </summary>
    private static void CreateFieldSetup()
    {
        string guidePath = "Assets/MATCH2D_FIELD_SETUP.md";
        string guideContent = @"# 2D Maç Sahası Kurulum Rehberi

## Canvas ve Saha Ayarları

### Canvas Ayarları:
- **Canvas Size**: 1080 x 1920 (Portrait)
- **Render Mode**: Screen Space - Overlay
- **Canvas Scaler**: Scale With Screen Size
  - Reference Resolution: 1080 x 1920

### Field GameObject (Canvas Altında):
- **Position**: X: 540, Y: 960, Z: 0
- **Size**: Width: 120, Height: 120
- **Anchor**: Center (0.5, 0.5)
- **Pivot**: Center (0.5, 0.5)

## FieldManager Ayarları:
- **fieldWidth**: 120
- **fieldHeight**: 120
- **fieldCenter**: X: 540.6165, Y: 963.9995 (Inspector'da ayarla)
- **playerPrefab**: Assets/Prefabs/Player.prefab (sürükle-bırak)

## MatchCamera Ayarları:
- **orthographicSize**: 60 (120x120 saha için)
- **minX**: 480.6165 (540.6165 - 60)
- **maxX**: 600.6165 (540.6165 + 60)
- **minY**: 903.9995 (963.9995 - 60)
- **maxY**: 1023.9995 (963.9995 + 60)
- **fieldCenter**: X: 540.6165, Y: 963.9995 (Inspector'da ayarla)

## Formasyon Pozisyonları:
Saha merkezi (540.6165, 963.9995) üzerinden relative pozisyonlar:
- **Kaleci**: Y = ±55 (merkezden)
- **Defans**: Y = ±42, X = -48, -30, +30, +48
- **Orta Saha**: Y = ±12, X = -42, -22, +22, +42
- **Forvet**: Y = ±42, X = -35, +35

## Önemli Notlar:
1. Field GameObject Canvas altında olmalı
2. Field'ın RectTransform pozisyonu (540, 960) olmalı
3. FieldManager ve MatchCamera'da fieldCenter değerlerini Inspector'dan manuel ayarla
4. Oyuncular world space'te spawn olur, fieldCenter offset'i ile

";

        System.IO.File.WriteAllText(guidePath, guideContent);
        AssetDatabase.Refresh();
        Debug.Log($"[CreateMatch2DSetup] Field setup guide created at: {guidePath}");
    }
}

