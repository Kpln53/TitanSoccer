using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixCareerHubMenu : MonoBehaviour
{
    [MenuItem("TitanSoccer/UI/🔧 Fix CareerHub Menu")]
    public static void FixMenu()
    {
        GameObject canvas = GameObject.Find("CareerHubCanvas");
        if (!canvas) return;

        Transform altMenu = canvas.transform.Find("MainPanel/AltMenü");
        if (!altMenu) 
        {
            // Belki direkt root'tadır
            altMenu = canvas.transform.Find("AltMenü");
        }

        if (!altMenu)
        {
            Debug.LogError("AltMenü bulunamadı!");
            return;
        }

        // 1. Butonların Raycast Target alanlarını küçült (Image componentleri üzerinden)
        // Genelde butonların Image componentleri vardır. Eğer ikonlar ayrıysa, ana buton objesinin Image'ı şeffaf ve büyük olabilir.
        // Burada butonların RectTransform boyutlarını küçültüp, ikonları scale ile büyütebiliriz veya tam tersi.
        // En temiz yöntem: Butonların boyutunu (Width) küçültmek.

        Button[] buttons = altMenu.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            RectTransform rt = btn.GetComponent<RectTransform>();
            // Genişliği 120 civarına çek (ikonlar sığsın ama yanındakine taşmasın)
            rt.sizeDelta = new Vector2(120, rt.sizeDelta.y);
        }

        // 2. Parlama Efektini Bağla
        CareerHubUI ui = canvas.GetComponent<CareerHubUI>();
        if (!ui) ui = FindFirstObjectByType<CareerHubUI>();

        if (ui)
        {
            Transform glow = altMenu.Find("ParlamaEfekti/Üst");
            if (glow)
            {
                ui.glowEffect = glow.GetComponent<RectTransform>();
                // Glow'un parent'ı (ParlamaEfekti) butonlarla aynı hiyerarşide olmalı ki pozisyonlar tutsun.
                // Eğer ParlamaEfekti ayrı bir obje ise, onun içindeki "Üst" objesini hareket ettirmek yerine
                // ParlamaEfekti objesini butonların olduğu container'a taşıyıp oradan yönetmek daha kolay olabilir.
                // Şimdilik sadece referansı bağlayalım.
            }
            
            // Buton referanslarını da güncelle (isimlere göre)
            ui.homeButton = FindButton(altMenu, "MenüSimge");
            ui.newsButton = FindButton(altMenu, "HaberSimge");
            ui.marketButton = FindButton(altMenu, "MarketSimge");
            ui.trainingButton = FindButton(altMenu, "AntrenmanSimge");
            ui.lifeButton = FindButton(altMenu, "İlişkiSimge");
            ui.playerStatsButton = FindButton(altMenu, "GelişimSimge");
            ui.socialMediaButton = FindButton(altMenu, "KadroSimge"); // Sosyal medya için ikon yoksa geçici

            EditorUtility.SetDirty(ui);
        }

        Debug.Log("✅ CareerHub Menu Fixed!");
    }

    private static Button FindButton(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        if (t) return t.GetComponent<Button>();
        return null;
    }
}
