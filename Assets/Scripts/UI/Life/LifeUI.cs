using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using TitanSoccer.Life;

public class LifeUI : MonoBehaviour
{
    [Header("References")]
    public Transform listContent;
    public GameObject itemPrefab;

    [Header("Interaction Popup")]
    public GameObject popupPanel;
    public TextMeshProUGUI popupTitle;
    public Transform popupOptionsContainer;
    public GameObject popupOptionPrefab;
    public Button popupCloseButton;

    [Header("Bottom Bar")]
    public Button profileButton;
    public Button shopButton;
    public Button gambleButton;

    private void OnEnable()
    {
        // Sistem yoksa oluştur (Test için)
        if (RelationshipSystem.Instance == null)
        {
            GameObject sys = new GameObject("RelationshipSystem");
            sys.AddComponent<RelationshipSystem>();
        }

        if (popupPanel) popupPanel.SetActive(false);
        if (popupCloseButton) popupCloseButton.onClick.RemoveAllListeners();
        if (popupCloseButton) popupCloseButton.onClick.AddListener(() => popupPanel.SetActive(false));

        RefreshList();
    }

    public void RefreshList()
    {
        foreach (Transform child in listContent) Destroy(child.gameObject);

        var list = RelationshipSystem.Instance.Relationships;
        foreach (var rel in list)
        {
            GameObject obj = Instantiate(itemPrefab, listContent);
            RelationshipItemUI ui = obj.GetComponent<RelationshipItemUI>();
            if (ui)
            {
                ui.Setup(rel, OnItemClicked);
            }
        }
    }

    private void OnItemClicked(RelationshipData rel)
    {
        ShowInteractionPopup(rel);
    }

    private void ShowInteractionPopup(RelationshipData rel)
    {
        if (!popupPanel) return;

        popupPanel.SetActive(true);
        if (popupTitle) popupTitle.text = $"{rel.name} ile ne yapmak istersin?";

        // Seçenekleri temizle
        foreach (Transform child in popupOptionsContainer) Destroy(child.gameObject);

        // Seçenekleri al
        var options = RelationshipSystem.Instance.GetInteractions(rel.type);

        foreach (var opt in options)
        {
            GameObject btnObj = Instantiate(popupOptionPrefab, popupOptionsContainer);
            
            // Buton metni
            TextMeshProUGUI txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = opt.label;

            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => ExecuteInteraction(rel, opt));
        }
    }

    private void ExecuteInteraction(RelationshipData rel, InteractionOption opt)
    {
        string result = RelationshipSystem.Instance.ExecuteInteraction(rel, opt);
        Debug.Log(result); // İleride ekrana toast mesajı olarak basılabilir
        
        popupPanel.SetActive(false);
        RefreshList(); // Listeyi güncelle (puan değişti)
    }
}
