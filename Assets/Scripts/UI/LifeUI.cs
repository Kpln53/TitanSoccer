using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Yaşam UI - Oyuncunun özel yaşamı ve ilişkiler paneli
/// </summary>
public class LifeUI : MonoBehaviour
{
    [Header("İlişkiler")]
    public TextMeshProUGUI coachRelationText;
    public TextMeshProUGUI managementRelationText;
    public TextMeshProUGUI managerRelationText;
    public TextMeshProUGUI familyRelationText;
    public TextMeshProUGUI girlfriendRelationText;

    [Header("Takım Arkadaşları")]
    public Transform teammatesListParent;
    public GameObject teammateItemPrefab;

    private void OnEnable()
    {
        RefreshData();
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    private void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[LifeUI] No current save!");
            return;
        }

        RelationsData relations = GameManager.Instance.CurrentSave.relationsData;
        if (relations == null) return;

        // İlişkileri göster
        if (coachRelationText != null)
            coachRelationText.text = $"Teknik Direktör: {relations.coachRelation}";

        if (managementRelationText != null)
            managementRelationText.text = $"Yönetim: {relations.managementRelation}";

        if (managerRelationText != null)
            managerRelationText.text = $"Menajer: {relations.managerRelation}";

        if (familyRelationText != null)
            familyRelationText.text = $"Aile: {relations.familyRelation}";

        if (girlfriendRelationText != null)
        {
            string girlfriendText = relations.HasGirlfriend() 
                ? $"Sevgili: {relations.girlfriendRelation}" 
                : "Sevgili: Yok";
            girlfriendRelationText.text = girlfriendText;
        }

        // Takım arkadaşlarını göster
        DisplayTeammates(relations);
    }

    /// <summary>
    /// Takım arkadaşlarını göster
    /// </summary>
    private void DisplayTeammates(RelationsData relations)
    {
        if (teammatesListParent == null || relations.teammateRelations == null) return;

        // Mevcut item'ları temizle
        foreach (Transform child in teammatesListParent)
        {
            Destroy(child.gameObject);
        }

        // Her takım arkadaşı için item oluştur
        foreach (var teammate in relations.teammateRelations)
        {
            CreateTeammateItem(teammate);
        }
    }

    /// <summary>
    /// Takım arkadaşı item'ı oluştur
    /// </summary>
    private void CreateTeammateItem(TeammateRelation teammate)
    {
        GameObject itemObj;

        if (teammateItemPrefab != null)
        {
            itemObj = Instantiate(teammateItemPrefab, teammatesListParent);
        }
        else
        {
            itemObj = new GameObject($"TeammateItem_{teammate.teammateName}");
            itemObj.transform.SetParent(teammatesListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 50);

            Image bg = itemObj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

            // Takım arkadaşı adı ve ilişki seviyesi
            GameObject nameObj = new GameObject("TeammateName");
            nameObj.transform.SetParent(itemObj.transform);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(10, 5);
            nameRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            string relationEmoji = teammate.relationLevel > 50 ? "😊" : teammate.relationLevel > 0 ? "🙂" : teammate.relationLevel > -50 ? "😐" : "😠";
            nameText.text = $"{teammate.teammateName}: {relationEmoji} {teammate.relationLevel}";
            nameText.fontSize = 16;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.MidlineLeft;
        }

        // Prefab içinde TextMeshProUGUI varsa güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            string relationEmoji = teammate.relationLevel > 50 ? "😊" : teammate.relationLevel > 0 ? "🙂" : teammate.relationLevel > -50 ? "😐" : "😠";
            texts[0].text = $"{teammate.teammateName}: {relationEmoji} {teammate.relationLevel}";
        }
    }
}

