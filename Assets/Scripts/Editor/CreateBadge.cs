using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateBadge : MonoBehaviour
{
    public static void CreateNewsBadge()
    {
        GameObject newsButton = GameObject.Find("NewsButton");
        if (newsButton == null)
        {
            Debug.LogError("NewsButton not found!");
            return;
        }
        
        GameObject badge = new GameObject("NotificationBadge");
        badge.transform.SetParent(newsButton.transform, false);
        
        RectTransform rect = badge.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(10, 10);
        rect.sizeDelta = new Vector2(24, 24);
        
        Image img = badge.AddComponent<Image>();
        img.color = Color.red;
        
        GameObject textObj = new GameObject("CountText");
        textObj.transform.SetParent(badge.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "1";
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;
        
        Debug.Log("Badge created!");
    }
}
