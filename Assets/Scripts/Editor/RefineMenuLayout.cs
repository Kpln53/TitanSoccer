using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RefineMenuLayout : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🎨 Refine Menu Layout (Fix Visuals)")]
    public static void Refine()
    {
        GameObject canvas = GameObject.Find("CareerHubCanvas");
        if (!canvas) return;

        Transform altMenu = canvas.transform.Find("MainPanel/AltMenü") ?? canvas.transform.Find("AltMenü");
        if (!altMenu) return;

        RectTransform menuRect = altMenu.GetComponent<RectTransform>();
        float totalWidth = menuRect.rect.width;
        
        // Butonları bul
        Button[] buttons = altMenu.GetComponentsInChildren<Button>();
        int count = buttons.Length;
        if (count == 0) return;

        // Her butona düşen maksimum genişlik (Overlap olmaması için)
        float buttonWidth = totalWidth / count;
        // Biraz boşluk bırakalım
        float targetClickWidth = buttonWidth * 0.9f; 

        foreach (Button btn in buttons)
        {
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            
            // 1. Tıklama Alanını Ayarla (Daralt)
            // Yüksekliği koru, genişliği ayarla
            btnRect.sizeDelta = new Vector2(targetClickWidth, btnRect.sizeDelta.y);

            // 2. Görseli (İkonu) Koru
            // İkonun butonun boyutuna göre ezilmesini engellemek için
            // Anchor'ları merkeze alıp sabit boyut vereceğiz.
            
            Image iconImg = btn.GetComponent<Image>();
            // Eğer butonda image yoksa child'da olabilir
            if (!iconImg && btn.transform.childCount > 0)
                iconImg = btn.transform.GetChild(0).GetComponent<Image>();

            if (iconImg)
            {
                RectTransform iconRect = iconImg.rectTransform;
                
                // Anchor'ları merkeze çek (Stretch olmasın)
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                
                // Sabit boyut ver (Örn: 80x80 veya mevcut yüksekliğe göre)
                // Mevcut yüksekliğin %80'i kadar kare yapalım
                float size = btnRect.rect.height * 0.8f;
                iconRect.sizeDelta = new Vector2(size, size);
                
                // Pozisyonu sıfırla
                iconRect.anchoredPosition = Vector2.zero;
            }
        }

        Debug.Log($"✅ Menu Refined! Button Width: {targetClickWidth}px, Icons Centered.");
    }
}
