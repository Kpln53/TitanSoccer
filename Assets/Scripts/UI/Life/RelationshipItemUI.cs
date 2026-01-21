using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TitanSoccer.Life;
using System;

public class RelationshipItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;
    public Slider progressBar;
    public Image fillImage; // Bar rengi için
    public Button actionButton;

    private RelationshipData currentData;
    private Action<RelationshipData> onActionClick;

    public void Setup(RelationshipData data, Action<RelationshipData> callback)
    {
        currentData = data;
        onActionClick = callback;

        if (nameText) nameText.text = $"{GetTitle(data.type)} ({data.name})";
        
        if (progressBar) progressBar.value = data.value / 100f;
        
        if (statusText) 
        {
            statusText.text = $"{data.GetStatusText()}: %{data.value}";
            statusText.color = data.GetStatusColor();
        }

        if (fillImage) fillImage.color = data.GetStatusColor();

        if (actionButton)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => onActionClick?.Invoke(currentData));
        }
    }

    private string GetTitle(RelationshipType type)
    {
        switch (type)
        {
            case RelationshipType.Girlfriend: return "KIZ ARKADAŞ";
            case RelationshipType.Family: return "AİLE";
            case RelationshipType.Team: return "TAKIM";
            case RelationshipType.Coach: return "TEKNİK DİREKTÖR";
            case RelationshipType.Fans: return "TARAFTARLAR";
            case RelationshipType.Sponsor: return "SPONSORLAR";
            case RelationshipType.Manager: return "MENAJER";
            default: return "";
        }
    }
}
