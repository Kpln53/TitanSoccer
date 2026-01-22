using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class AdvancedMenuFix : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🛡️ Advanced Menu Fix (Raycast Masking)")]
    public static void ApplyFix()
    {
        GameObject canvas = GameObject.Find("CareerHubCanvas");
        if (!canvas) return;

        Transform altMenu = canvas.transform.Find("MainPanel/AltMenü") ?? canvas.transform.Find("AltMenü");
        if (!altMenu) return;

        Button[] buttons = altMenu.GetComponentsInChildren<Button>();
        
        foreach (Button btn in buttons)
        {
            // 1. Butonun Image componentini bul (Tıklamayı algılayan kısım)
            Image hitImage = btn.GetComponent<Image>();
            
            // Eğer butonda image yoksa ekle (Raycast Target için)
            if (!hitImage)
            {
                hitImage = btn.gameObject.AddComponent<Image>();
                hitImage.color = new Color(0, 0, 0, 0); // Tamamen şeffaf
            }

            // 2. Alpha Hit Test Threshold Ayarı
            // Bu ayar, resmin şeffaf kısımlarına tıklanmasını engeller.
            // Ancak bunun çalışması için Sprite'ın Read/Write Enabled olması gerekir.
            // Eğer sprite yoksa veya kare ise işe yaramaz.
            
            // Alternatif: Butonun boyutunu küçültüp, görseli child objeye alıp büyütmek.
            // Önceki script bunu yaptı ama görsel bozuldu dedin.
            
            // ŞİMDİ: Görseli bozmadan sadece tıklama alanını (Hitbox) küçülteceğiz.
            
            // A. Mevcut görseli (varsa) child objeye taşı
            if (hitImage.sprite != null)
            {
                GameObject visualObj = new GameObject("Visual");
                visualObj.transform.SetParent(btn.transform, false);
                visualObj.transform.SetAsFirstSibling(); // En arkaya at
                
                RectTransform visualRT = visualObj.AddComponent<RectTransform>();
                visualRT.anchorMin = Vector2.zero;
                visualRT.anchorMax = Vector2.one;
                visualRT.sizeDelta = Vector2.zero; // Ebeveyni doldur
                
                Image visualImg = visualObj.AddComponent<Image>();
                visualImg.sprite = hitImage.sprite;
                visualImg.color = hitImage.color;
                visualImg.raycastTarget = false; // Görsel tıklamayı engellemesin

                // Ana butondaki görseli kaldır (sadece hitbox kalsın)
                hitImage.sprite = null;
                hitImage.color = new Color(0, 0, 0, 0); // Görünmez hitbox
            }

            // B. Hitbox'ı (Ana Buton) Daralt
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            float originalWidth = btnRect.rect.width;
            float newWidth = 110f; // İdeal tıklama genişliği
            
            btnRect.sizeDelta = new Vector2(newWidth, btnRect.sizeDelta.y);

            // C. Görseli (Child) Genişlet (Eski boyutuna döndür)
            // Child, parent'a göre stretch olduğu için parent küçülünce o da küçüldü.
            // Bunu tersine çevirmek için negatif margin vereceğiz.
            
            Transform visualTr = btn.transform.Find("Visual");
            if (visualTr)
            {
                RectTransform vRT = visualTr.GetComponent<RectTransform>();
                float diff = originalWidth - newWidth;
                vRT.offsetMin = new Vector2(-diff/2, 0);
                vRT.offsetMax = new Vector2(diff/2, 0);
            }
        }

        Debug.Log("✅ Advanced Fix Applied: Hitboxes shrunk, Visuals preserved!");
    }
}
