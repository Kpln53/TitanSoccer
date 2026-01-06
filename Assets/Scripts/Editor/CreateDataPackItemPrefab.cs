using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// DataPack Item Prefab otomatik oluşturucu
/// </summary>
public class CreateDataPackItemPrefab : EditorWindow
{
    [MenuItem("TitanSoccer/Create DataPack Item Prefab")]
    public static void CreatePrefab()
    {
        CreateDataPackItemPrefabWindow window = GetWindow<CreateDataPackItemPrefabWindow>("Create DataPack Item Prefab");
        window.Show();
    }
}

public class CreateDataPackItemPrefabWindow : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.Label("DataPack Item Prefab Oluşturucu", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Bu buton, Assets/Prefabs/UI/ klasöründe 'DataPackItem' prefab'ını otomatik olarak oluşturacaktır.", MessageType.Info);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Prefab'ı Oluştur", GUILayout.Height(40)))
        {
            CreatePrefabAsset();
        }
    }
    
    private void CreatePrefabAsset()
    {
        // Klasör yoksa oluştur
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }
        
        // Ana GameObject oluştur
        GameObject prefabRoot = new GameObject("DataPackItem");
        
        // RectTransform
        RectTransform rootRect = prefabRoot.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(900, 110);
        
        // Image (Background)
        Image bgImage = prefabRoot.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);
        
        // Button
        Button button = prefabRoot.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.selectedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        button.colors = colors;
        
        // DataPackItemUI Script
        DataPackItemUI itemUI = prefabRoot.AddComponent<DataPackItemUI>();
        
        // PackLogo GameObject (Opsiyonel)
        GameObject packLogoObj = new GameObject("PackLogo");
        packLogoObj.transform.SetParent(prefabRoot.transform);
        RectTransform logoRect = packLogoObj.AddComponent<RectTransform>();
        logoRect.anchorMin = new Vector2(0f, 0.5f);
        logoRect.anchorMax = new Vector2(0f, 0.5f);
        logoRect.pivot = new Vector2(0f, 0.5f);
        logoRect.anchoredPosition = new Vector2(15f, 0f);
        logoRect.sizeDelta = new Vector2(80f, 80f);
        
        Image logoImage = packLogoObj.AddComponent<Image>();
        logoImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        logoImage.gameObject.SetActive(false); // Varsayılan olarak kapalı (logo yoksa)
        
        // PackInfo Container
        GameObject packInfoObj = new GameObject("PackInfo");
        packInfoObj.transform.SetParent(prefabRoot.transform);
        RectTransform infoRect = packInfoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0f, 0f);
        infoRect.anchorMax = new Vector2(1f, 1f);
        infoRect.offsetMin = new Vector2(105f, 10f); // Left: 105 (logo'dan sonra), Bottom: 10
        infoRect.offsetMax = new Vector2(-10f, -10f); // Right: 10, Top: 10
        
        VerticalLayoutGroup infoLayout = packInfoObj.AddComponent<VerticalLayoutGroup>();
        infoLayout.spacing = 5f;
        infoLayout.padding = new RectOffset(10, 10, 0, 0);
        infoLayout.childControlWidth = true;
        infoLayout.childControlHeight = false;
        infoLayout.childForceExpandWidth = true;
        infoLayout.childForceExpandHeight = false;
        
        // PackNameText
        GameObject packNameObj = new GameObject("PackNameText");
        packNameObj.transform.SetParent(packInfoObj.transform);
        RectTransform nameRect = packNameObj.AddComponent<RectTransform>();
        nameRect.sizeDelta = new Vector2(0f, 30f);
        
        TextMeshProUGUI nameText = packNameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = "Pack Name";
        nameText.fontSize = 22;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.enableWordWrapping = false;
        
        // PackMeta Container
        GameObject packMetaObj = new GameObject("PackMeta");
        packMetaObj.transform.SetParent(packInfoObj.transform);
        RectTransform metaRect = packMetaObj.AddComponent<RectTransform>();
        metaRect.sizeDelta = new Vector2(0f, 20f);
        
        HorizontalLayoutGroup metaLayout = packMetaObj.AddComponent<HorizontalLayoutGroup>();
        metaLayout.spacing = 15f;
        metaLayout.childControlWidth = false;
        metaLayout.childControlHeight = false;
        metaLayout.childForceExpandWidth = false;
        metaLayout.childForceExpandHeight = false;
        
        // PackVersionText
        GameObject versionObj = new GameObject("PackVersionText");
        versionObj.transform.SetParent(packMetaObj.transform);
        RectTransform versionRect = versionObj.AddComponent<RectTransform>();
        versionRect.sizeDelta = new Vector2(100f, 20f);
        
        TextMeshProUGUI versionText = versionObj.AddComponent<TextMeshProUGUI>();
        versionText.text = "v1.0.0";
        versionText.fontSize = 14;
        versionText.fontStyle = FontStyles.Normal;
        versionText.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Gray
        versionText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // PackAuthorText
        GameObject authorObj = new GameObject("PackAuthorText");
        authorObj.transform.SetParent(packMetaObj.transform);
        RectTransform authorRect = authorObj.AddComponent<RectTransform>();
        authorRect.sizeDelta = new Vector2(200f, 20f);
        
        TextMeshProUGUI authorText = authorObj.AddComponent<TextMeshProUGUI>();
        authorText.text = "Author: Unknown";
        authorText.fontSize = 14;
        authorText.fontStyle = FontStyles.Normal;
        authorText.color = new Color(0.8f, 0.8f, 0.8f, 1f); // Gray
        authorText.alignment = TextAlignmentOptions.MidlineLeft;
        
        // DataPackItemUI script referanslarını bağla
        itemUI.packNameText = nameText;
        itemUI.packVersionText = versionText;
        itemUI.packAuthorText = authorText;
        itemUI.packLogoImage = logoImage;
        itemUI.itemButton = button;
        
        // Prefab'ı kaydet
        string prefabPath = "Assets/Prefabs/UI/DataPackItem.prefab";
        
        // Eğer zaten varsa uyar
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            if (EditorUtility.DisplayDialog("Prefab Zaten Var",
                "DataPackItem.prefab zaten mevcut. Üzerine yazmak istiyor musunuz?",
                "Evet", "Hayır"))
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                DestroyImmediate(prefabRoot);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Başarılı", "DataPackItem prefab'ı güncellendi!", "Tamam");
            }
            else
            {
                DestroyImmediate(prefabRoot);
            }
        }
        else
        {
            GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            DestroyImmediate(prefabRoot);
            AssetDatabase.Refresh();
            
            // Prefab'ı seç
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
            
            EditorUtility.DisplayDialog("Başarılı", 
                $"DataPackItem prefab'ı oluşturuldu!\n\nKonum: {prefabPath}\n\nŞimdi DataPackMenu sahnesinde 'Pack Item Prefab' alanına bu prefab'ı atayabilirsiniz.", 
                "Tamam");
        }
    }
}






