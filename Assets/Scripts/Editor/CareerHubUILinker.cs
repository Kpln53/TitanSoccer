using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// CareerHubUI referanslarını otomatik bağlayan Editor scripti
/// </summary>
public class CareerHubUILinker : Editor
{
    [MenuItem("TitanSoccer/UI/Link CareerHub References")]
    public static void LinkCareerHubReferences()
    {
        // CareerHubUI'yi bul
        var careerHubUI = Object.FindFirstObjectByType<CareerHubUI>();
        if (careerHubUI == null)
        {
            Debug.LogError("[CareerHubUILinker] CareerHubUI bulunamadı! CareerHub sahnesinde olduğundan emin ol.");
            return;
        }

        SerializedObject serializedUI = new SerializedObject(careerHubUI);
        int linkedCount = 0;

        // SocialMediaPanel'i bul ve bağla
        GameObject socialMediaPanel = GameObject.Find("SocialMediaPanel");
        if (socialMediaPanel == null)
        {
            // ContentArea içinde ara
            GameObject contentArea = GameObject.Find("ContentArea");
            if (contentArea != null)
            {
                Transform panelTransform = contentArea.transform.Find("SocialMediaPanel");
                if (panelTransform != null)
                {
                    socialMediaPanel = panelTransform.gameObject;
                }
            }
        }

        if (socialMediaPanel != null)
        {
            var prop = serializedUI.FindProperty("socialMediaPanel");
            if (prop != null)
            {
                prop.objectReferenceValue = socialMediaPanel;
                linkedCount++;
                Debug.Log("[CareerHubUILinker] ✅ SocialMediaPanel bağlandı");
            }
        }
        else
        {
            Debug.LogWarning("[CareerHubUILinker] ⚠️ SocialMediaPanel bulunamadı! Önce 'TitanSoccer > UI > Create Social Media Panel' çalıştır.");
        }

        // Alt menüdeki sosyal medya butonunu bul
        // Önce BottomNavigation veya BottomMenu'yu ara
        Button socialMediaButton = null;
        
        // Farklı isim olasılıklarını dene
        string[] possibleButtonNames = new[]
        {
            "SocialMediaButton",
            "SocialButton",
            "SosyalMedyaButton",
            "SMButton",
            "TwitterButton"
        };

        foreach (string buttonName in possibleButtonNames)
        {
            GameObject btnObj = GameObject.Find(buttonName);
            if (btnObj != null)
            {
                socialMediaButton = btnObj.GetComponent<Button>();
                if (socialMediaButton != null)
                {
                    Debug.Log($"[CareerHubUILinker] Buton bulundu: {buttonName}");
                    break;
                }
            }
        }

        // Hala bulunamadıysa, tüm butonları tara
        if (socialMediaButton == null)
        {
            // Alt panellerde ara
            GameObject bottomNav = GameObject.Find("BottomNavigation");
            if (bottomNav == null) bottomNav = GameObject.Find("BottomMenu");
            if (bottomNav == null) bottomNav = GameObject.Find("BottomPanel");
            if (bottomNav == null) bottomNav = GameObject.Find("NavigationBar");
            if (bottomNav == null) bottomNav = GameObject.Find("NavBar");

            if (bottomNav != null)
            {
                // Child'larda Button ara
                var buttons = bottomNav.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    string btnName = btn.gameObject.name.ToLower();
                    if (btnName.Contains("social") || btnName.Contains("sosyal") || btnName.Contains("twitter") || btnName.Contains("sm"))
                    {
                        socialMediaButton = btn;
                        Debug.Log($"[CareerHubUILinker] Alt menüde buton bulundu: {btn.gameObject.name}");
                        break;
                    }
                }
            }
        }

        if (socialMediaButton != null)
        {
            var prop = serializedUI.FindProperty("socialMediaButton");
            if (prop != null)
            {
                prop.objectReferenceValue = socialMediaButton;
                linkedCount++;
                Debug.Log("[CareerHubUILinker] ✅ SocialMediaButton bağlandı");
            }
        }
        else
        {
            Debug.LogWarning("[CareerHubUILinker] ⚠️ SocialMediaButton bulunamadı! Alt menüde 'SocialMediaButton' adında bir buton oluşturmalısın.");
        }

        serializedUI.ApplyModifiedProperties();

        // Sahneyi dirty olarak işaretle
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        // TopPanelUI referanslarını da bağla
        if (careerHubUI.topPanel != null)
        {
            LinkTopPanelReferences(careerHubUI.topPanel);
        }

        if (linkedCount > 0)
        {
            Debug.Log($"[CareerHubUILinker] ✅ {linkedCount} referans başarıyla bağlandı! Sahneyi kaydetmeyi unutma (Ctrl+S)");
        }
        else
        {
            Debug.LogWarning("[CareerHubUILinker] ⚠️ Hiçbir referans bağlanamadı. Manuel olarak bağlaman gerekebilir.");
        }

        Selection.activeGameObject = careerHubUI.gameObject;
    }

    private static void LinkTopPanelReferences(TopPanelUI topPanel)
    {
        SerializedObject serializedTopPanel = new SerializedObject(topPanel);
        bool changed = false;

        // Avatar Image'ı bul (KüçükResim)
        // Hiyerarşi: TopPanel -> SolTaraf -> KüçükResim
        Transform solTaraf = topPanel.transform.Find("SolTaraf");
        if (solTaraf != null)
        {
            Transform kucukResim = solTaraf.Find("KüçükResim");
            if (kucukResim != null)
            {
                Image avatarImage = kucukResim.GetComponent<Image>();
                if (avatarImage != null)
                {
                    var prop = serializedTopPanel.FindProperty("playerAvatarImage");
                    if (prop != null && prop.objectReferenceValue == null)
                    {
                        prop.objectReferenceValue = avatarImage;
                        changed = true;
                        Debug.Log("[CareerHubUILinker] ✅ TopPanel Avatar Image bağlandı");
                    }
                }
            }
        }

        if (changed)
        {
            serializedTopPanel.ApplyModifiedProperties();
            EditorUtility.SetDirty(topPanel);
        }
    }

    [MenuItem("TitanSoccer/UI/Create Social Media Button in BottomNav")]
    public static void CreateSocialMediaButton()
    {
        // BottomNavigation'ı bul
        GameObject bottomNav = GameObject.Find("BottomNavigation");
        if (bottomNav == null) bottomNav = GameObject.Find("BottomMenu");
        if (bottomNav == null) bottomNav = GameObject.Find("BottomPanel");
        if (bottomNav == null) bottomNav = GameObject.Find("NavigationBar");

        if (bottomNav == null)
        {
            Debug.LogError("[CareerHubUILinker] Alt menü bulunamadı! 'BottomNavigation', 'BottomMenu' veya 'BottomPanel' adında bir GameObject olmalı.");
            return;
        }

        // Mevcut buton varsa sil
        Transform existing = bottomNav.transform.Find("SocialMediaButton");
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }

        // Yeni buton oluştur
        GameObject buttonGO = new GameObject("SocialMediaButton");
        buttonGO.transform.SetParent(bottomNav.transform, false);

        // RectTransform
        var rect = buttonGO.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(80, 80);

        // Image
        var image = buttonGO.AddComponent<Image>();
        image.color = new Color(0.2f, 0.5f, 0.9f); // Mavi

        // Button
        var button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;

        // Text (ikon olarak emoji)
        GameObject textGO = new GameObject("Icon");
        textGO.transform.SetParent(buttonGO.transform, false);

        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var tmp = textGO.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text = "📱";
        tmp.fontSize = 36;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color = Color.white;

        Debug.Log("[CareerHubUILinker] ✅ SocialMediaButton oluşturuldu! Şimdi 'Link CareerHub References' çalıştır.");

        // Sahneyi dirty olarak işaretle
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Selection.activeGameObject = buttonGO;
    }
}
