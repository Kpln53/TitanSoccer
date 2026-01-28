using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CareerHistoryItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private TextMeshProUGUI teamText;
    [SerializeField] private TextMeshProUGUI matchesText;
    [SerializeField] private TextMeshProUGUI goalsText;
    [SerializeField] private TextMeshProUGUI assistsText;
    [SerializeField] private TextMeshProUGUI ratingText;

    public void Setup(CareerHistoryEntry entry)
    {
        if (seasonText) seasonText.text = entry.season;
        if (teamText) teamText.text = entry.teamName;
        if (matchesText) matchesText.text = entry.matches.ToString();
        if (goalsText) goalsText.text = entry.goals.ToString();
        if (assistsText) assistsText.text = entry.assists.ToString();
        if (ratingText) ratingText.text = entry.averageRating.ToString("F1");
    }
}
