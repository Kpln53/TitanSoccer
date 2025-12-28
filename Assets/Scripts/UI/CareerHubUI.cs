using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CareerHubUI : MonoBehaviour
{
    [Header("Profil UI")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI overallText;

    [Header("Ma� Bilgisi UI")]
    public TextMeshProUGUI nextMatchTitleText;
    public TextMeshProUGUI nextMatchTeamsText;
    public TextMeshProUGUI nextMatchTypeText;

    private void Start()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentSave == null)
        {
            Debug.LogError("CareerHub: CurrentSave bulunamad�!");
            if (playerNameText != null)
                playerNameText.text = "Kay�t bulunamad�";
            return;
        }

        var data = GameManager.Instance.CurrentSave;

        // Profil bilgileri
        if (playerNameText != null)
            playerNameText.text = $"{data.playerName} ({data.position})";

        if (clubNameText != null)
            clubNameText.text = data.clubName;

        if (seasonText != null)
            seasonText.text = $"Sezon {data.season}";

        if (overallText != null)
            overallText.text = $"OVR {data.overall}";

        // Ma� bilgisi (�imdilik basit dummy veri)
        if (nextMatchTitleText != null)
            nextMatchTitleText.text = "S�radaki Ma�";

        if (nextMatchTeamsText != null)
            nextMatchTeamsText.text = $"{data.clubName} vs Rakip FC";

        if (nextMatchTypeText != null)
            nextMatchTypeText.text = "Lig Ma��";
    }

    public void OnGoToMatchButton()
    {
        Debug.Log("Ma�a Git butonuna bas�ld�! (Buradan Match sahnesine ge�ece�iz)");
        SceneManager.LoadScene("MatchScene");
    }
}
