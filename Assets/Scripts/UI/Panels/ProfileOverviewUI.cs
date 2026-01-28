using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileOverviewUI : MonoBehaviour
{
    [Header("Left Side Info")]
    [SerializeField] private Image playerPhoto;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private Image flagImage;
    [SerializeField] private TextMeshProUGUI nationalityText;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI overallText;

    [Header("Right Side Stats")]
    [SerializeField] private Transform statsContainer;
    [SerializeField] private GameObject statItemPrefab; // Prefab with Icon + Text + Value

    public void Setup(PlayerProfile profile)
    {
        if (profile == null) return;

        nameText.text = profile.playerName.ToUpper();
        ageText.text = profile.age.ToString();
        nationalityText.text = profile.nationality;
        positionText.text = $"{profile.position} ({GetPositionFullName(profile.position)})";
        overallText.text = $"{profile.overall} (Genel)";

        // Setup Stats
        UpdateStats(profile);
    }

    private void UpdateStats(PlayerProfile profile)
    {
        // Clear existing
        foreach (Transform child in statsContainer)
        {
            Destroy(child.gameObject);
        }

        // Create stat items
        CreateStatItem("Hız", profile.speed);
        CreateStatItem("Şut", profile.shootingSkill);
        CreateStatItem("Pas", profile.passingSkill);
        CreateStatItem("Dayanıklılık", profile.stamina);
        CreateStatItem("Teknik", profile.dribblingSkill); // Using dribbling as technique
        CreateStatItem("Zeka", profile.falsoSkill); // Using falso as intelligence/flair for now
    }

    private void CreateStatItem(string label, int value)
    {
        if (statItemPrefab == null) return;

        GameObject item = Instantiate(statItemPrefab, statsContainer);
        TextMeshProUGUI text = item.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = $"{label}: {value}";
        }
    }

    private string GetPositionFullName(PlayerPosition pos)
    {
        switch (pos)
        {
            case PlayerPosition.KL: return "Kaleci";
            case PlayerPosition.STP: return "Stoper";
            case PlayerPosition.SĞB: return "Sağ Bek";
            case PlayerPosition.SLB: return "Sol Bek";
            case PlayerPosition.MDO: return "Ön Libero";
            case PlayerPosition.MOO: return "Ofansif Orta Saha";
            case PlayerPosition.SĞK: return "Sağ Kanat";
            case PlayerPosition.SLK: return "Sol Kanat";
            case PlayerPosition.SF: return "Santrafor";
            default: return pos.ToString();
        }
    }
}
