using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("States")]
    public GameObject emptyStateObject; // "Yeni Kariyer" görünümü
    public GameObject filledStateObject; // "Kayıtlı Slot" görünümü

    [Header("Filled State References")]
    public TextMeshProUGUI filledTitleText;
    public TextMeshProUGUI filledSubtitleText;
    public Image playerIcon;

    [Header("Empty State References")]
    public TextMeshProUGUI emptyTitleText;
    public TextMeshProUGUI emptySubtitleText;

    [Header("Components")]
    public Button slotButton;
    public Outline outline;

    public void Setup(bool hasSave, SaveData data)
    {
        if (hasSave && data != null)
        {
            // Dolu Slot Görünümü
            if (emptyStateObject) emptyStateObject.SetActive(false);
            if (filledStateObject) filledStateObject.SetActive(true);

            if (filledTitleText) filledTitleText.text = "KAYITLI SLOT";
            if (filledSubtitleText) 
            {
                int ovr = CalculateOverall(data);
                filledSubtitleText.text = $"{data.playerProfile.playerName} | OVR {ovr} | {data.seasonData.seasonName}";
            }
            
            if (outline) outline.effectColor = new Color(1f, 1f, 1f, 0.3f); // Beyazımsı
        }
        else
        {
            // Boş Slot Görünümü
            if (emptyStateObject) emptyStateObject.SetActive(true);
            if (filledStateObject) filledStateObject.SetActive(false);

            if (emptyTitleText) emptyTitleText.text = "YENİ KARİYER";
            if (emptySubtitleText) emptySubtitleText.text = "Boş Slot";

            if (outline) outline.effectColor = new Color(1f, 0.8f, 0.2f, 0.6f); // Altın rengi
        }
    }

    private int CalculateOverall(SaveData data)
    {
        if (data.playerProfile == null) return 70;
        var p = data.playerProfile;
        return (p.shootingSkill + p.passingSkill + p.dribblingSkill + p.speed + p.physicalStrength + p.defendingSkill) / 6;
    }
}
