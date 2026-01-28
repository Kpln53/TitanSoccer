using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileUI : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private Button overviewButton;
    [SerializeField] private Button careerButton;
    [SerializeField] private Button contractButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject overviewPanel;
    [SerializeField] private GameObject careerPanel;
    [SerializeField] private GameObject contractPanel;

    [Header("Sub-Controllers")]
    [SerializeField] private ProfileOverviewUI overviewUI;
    [SerializeField] private ProfileCareerUI careerUI;
    [SerializeField] private ProfileContractUI contractUI;

    [Header("Styling")]
    [SerializeField] private Color activeTabColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color inactiveTabColor = new Color(0.1f, 0.3f, 0.1f, 1f);

    private void Start()
    {
        // Add listeners
        overviewButton.onClick.AddListener(ShowOverview);
        careerButton.onClick.AddListener(ShowCareer);
        contractButton.onClick.AddListener(ShowContract);

        // Initial state
        ShowOverview();
        
        // Load Data
        RefreshData();
    }

    public void RefreshData()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null) return;

        var save = GameManager.Instance.CurrentSave;
        var profile = save.playerProfile;
        
        // Get contract (assuming it's stored somewhere, or mock for now if not in SaveData directly accessible)
        // For now, creating a dummy contract if not found, or using ClubData if available
        ContractData contract = new ContractData(); 
        contract.salary = 150000;
        contract.bonuses.Add(new ContractBonus(BonusType.WinBonus, 10000));
        contract.bonuses.Add(new ContractBonus(BonusType.GoalBonus, 5000));
        contract.bonuses.Add(new ContractBonus(BonusType.AssistBonus, 2500));

        // Setup sub-panels
        if (overviewUI) overviewUI.Setup(profile);
        if (careerUI) careerUI.Setup(null); // Pass history list here
        if (contractUI) contractUI.Setup(contract, profile.currentClubName);
    }

    private void ShowOverview()
    {
        SetActiveTab(overviewButton, overviewPanel);
    }

    private void ShowCareer()
    {
        SetActiveTab(careerButton, careerPanel);
    }

    private void ShowContract()
    {
        SetActiveTab(contractButton, contractPanel);
    }

    private void SetActiveTab(Button activeBtn, GameObject activePanel)
    {
        // Reset all
        overviewPanel.SetActive(false);
        careerPanel.SetActive(false);
        contractPanel.SetActive(false);

        SetButtonColor(overviewButton, inactiveTabColor);
        SetButtonColor(careerButton, inactiveTabColor);
        SetButtonColor(contractButton, inactiveTabColor);

        // Set active
        activePanel.SetActive(true);
        SetButtonColor(activeBtn, activeTabColor);
    }

    private void SetButtonColor(Button btn, Color color)
    {
        var img = btn.GetComponent<Image>();
        if (img) img.color = color;
    }
}
