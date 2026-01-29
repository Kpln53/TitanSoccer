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
    public Button filledButton; // Filled state için buton

    [Header("Empty State References")]
    public TextMeshProUGUI emptyTitleText;
    public TextMeshProUGUI emptySubtitleText;
    public Button emptyButton; // Empty state için buton

    [Header("Components")]
    public Button slotButton; // Fallback (eski sistem için)
    public Button deleteButton; // Silme butonu
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
            if (deleteButton) deleteButton.gameObject.SetActive(true); // Silme butonu aktif
            
            // Filled button'u aktif et, empty button'u pasif et
            if (filledButton) filledButton.gameObject.SetActive(true);
            if (emptyButton) emptyButton.gameObject.SetActive(false);
        }
        else
        {
            // Boş Slot Görünümü
            if (emptyStateObject) emptyStateObject.SetActive(true);
            if (filledStateObject) filledStateObject.SetActive(false);

            if (emptyTitleText) emptyTitleText.text = "YENİ KARİYER";
            if (emptySubtitleText) emptySubtitleText.text = "Boş Slot";

            if (outline) outline.effectColor = new Color(1f, 0.8f, 0.2f, 0.6f); // Altın rengi
            if (deleteButton) deleteButton.gameObject.SetActive(false); // Silme butonu pasif
            
            // Empty button'u aktif et, filled button'u pasif et
            if (emptyButton) emptyButton.gameObject.SetActive(true);
            if (filledButton) filledButton.gameObject.SetActive(false);
        }
    }

    private int CalculateOverall(SaveData data)
    {
        if (data.playerProfile == null) return 70;
        var p = data.playerProfile;
        return (p.shootingSkill + p.passingSkill + p.dribblingSkill + p.speed + p.physicalStrength + p.defendingSkill) / 6;
    }
}
