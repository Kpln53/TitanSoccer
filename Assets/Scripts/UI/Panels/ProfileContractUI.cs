using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ProfileContractUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private TextMeshProUGUI wageText;
    [SerializeField] private TextMeshProUGUI bonusesText;
    [SerializeField] private TextMeshProUGUI releaseClauseText;

    public void Setup(ContractData contract, string clubName)
    {
        if (contract == null) return;

        teamText.text = $"Takım: {clubName}";
        
        // Calculate remaining time
        float yearsRemaining = contract.GetDaysRemaining() / 365f;
        durationText.text = $"Süre: {contract.endDate.Year} Sezonu Sonu (Kalan: {yearsRemaining:F1} Yıl)";
        
        wageText.text = $"Maaş: €{contract.salary:N0} / Hafta";
        
        string bonuses = "Bonuslar: ";
        if (contract.bonuses != null && contract.bonuses.Count > 0)
        {
            List<string> bonusParts = new List<string>();
            foreach (var bonus in contract.bonuses)
            {
                string typeName = GetBonusTypeName(bonus.type);
                bonusParts.Add($"{typeName} €{bonus.amount:N0}");
            }
            bonuses += string.Join(", ", bonusParts);
        }
        else
        {
            bonuses += "Yok";
        }
        bonusesText.text = bonuses;

        // Release clause (mock or from data if added later)
        releaseClauseText.text = "Serbest Kalma Bedeli: €50.000.000"; 
    }

    private string GetBonusTypeName(BonusType type)
    {
        switch (type)
        {
            case BonusType.MatchFee: return "Maç Başı";
            case BonusType.GoalBonus: return "Gol";
            case BonusType.AssistBonus: return "Asist";
            case BonusType.CleanSheetBonus: return "Gol Yememe";
            case BonusType.WinBonus: return "Galibiyet";
            case BonusType.ChampionshipBonus: return "Şampiyonluk";
            default: return type.ToString();
        }
    }
}
