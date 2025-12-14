using UnityEngine;
using UnityEditor;

/// <summary>
/// 2D sprite'ları programatik olarak oluşturur
/// </summary>
public class SpriteCreator
{
    [MenuItem("TitanSoccer/Create 2D Sprites")]
    static void CreateSprites()
    {
        // Oyuncu sprite'ı oluştur (daire)
        Texture2D playerTexture = CreateCircleTexture(64, Color.white);
        Sprite playerSprite = CreateSpriteFromTexture(playerTexture, "PlayerSprite", 0.5f, 0.5f);
        SaveSpriteAsset(playerSprite, "Assets/Scripts/Data/PlayerSprite.asset");

        // Top sprite'ı oluştur (küçük daire)
        Texture2D ballTexture = CreateCircleTexture(32, Color.white);
        Sprite ballSprite = CreateSpriteFromTexture(ballTexture, "BallSprite", 0.5f, 0.5f);
        SaveSpriteAsset(ballSprite, "Assets/Scripts/Data/BallSprite.asset");

        // Saha sprite'ı oluştur (dikdörtgen)
        Texture2D pitchTexture = CreateRectangleTexture(1050, 680, new Color(0.2f, 0.6f, 0.2f));
        Sprite pitchSprite = CreateSpriteFromTexture(pitchTexture, "PitchSprite", 0.5f, 0.5f);
        SaveSpriteAsset(pitchSprite, "Assets/Scripts/Data/PitchSprite.asset");

        // Hareket göstergesi sprite'ı (daire, yarı saydam yeşil)
        Texture2D indicatorTexture = CreateCircleTexture(64, new Color(0f, 1f, 0f, 0.5f));
        Sprite indicatorSprite = CreateSpriteFromTexture(indicatorTexture, "MoveIndicatorSprite", 0.5f, 0.5f);
        SaveSpriteAsset(indicatorSprite, "Assets/Scripts/Data/MoveIndicatorSprite.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("2D Sprite'lar oluşturuldu!");
    }

    static Texture2D CreateCircleTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f; // Kenarlardan biraz içeride

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    // Yumuşak kenarlar için
                    float alpha = 1f;
                    if (distance > radius - 2f)
                    {
                        alpha = 1f - ((distance - (radius - 2f)) / 2f);
                    }
                    
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return texture;
    }

    static Texture2D CreateRectangleTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    static Sprite CreateSpriteFromTexture(Texture2D texture, string name, float pivotX, float pivotY)
    {
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(pivotX, pivotY),
            100f // Pixels per unit
        );
        sprite.name = name;
        return sprite;
    }

    static void SaveSpriteAsset(Sprite sprite, string path)
    {
        // Sprite'ı asset olarak kaydet
        AssetDatabase.CreateAsset(sprite, path);
    }
}

