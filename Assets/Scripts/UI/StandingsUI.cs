using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Puan durumu UI - Lig puan durumu ekranı
/// </summary>
public class StandingsUI : MonoBehaviour
{
    [Header("Lig Bilgileri")]
    public TextMeshProUGUI leagueNameText;

    [Header("Puan Durumu Listesi")]
    public Transform standingsListParent;
    public GameObject standingsItemPrefab;

    [Header("Geri Butonu")]
    public Button backButton;

    private void OnEnable()
    {
        RefreshData();
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    /// <summary>
    /// Verileri yenile
    /// </summary>
    private void RefreshData()
    {
        if (GameManager.Instance == null || !GameManager.Instance.HasCurrentSave())
        {
            Debug.LogWarning("[StandingsUI] No current save!");
            return;
        }

        ClubData club = GameManager.Instance.CurrentSave.clubData;
        if (club == null) return;

        // Lig adını göster
        if (leagueNameText != null)
            leagueNameText.text = club.leagueName;

        // Puan durumunu göster
        DisplayStandings(club.leagueName);
    }

    /// <summary>
    /// Puan durumunu göster
    /// </summary>
    private void DisplayStandings(string leagueName)
    {
        if (standingsListParent == null || DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
        {
            Debug.LogWarning("[StandingsUI] Cannot display standings - missing references");
            return;
        }

        LeagueData league = DataPackManager.Instance != null ? DataPackManager.Instance.GetLeague(leagueName) : null;
        if (league == null || league.teams == null)
        {
            Debug.LogWarning($"[StandingsUI] League not found: {leagueName}");
            return;
        }

        // Mevcut item'ları temizle
        foreach (Transform child in standingsListParent)
        {
            Destroy(child.gameObject);
        }

        // Takımları sırala (örnek: takım gücüne göre)
        List<TeamData> sortedTeams = league.teams.OrderByDescending(t => t.GetTeamPower()).ToList();

        // Her takım için item oluştur
        int position = 1;
        foreach (var team in sortedTeams)
        {
            CreateStandingsItem(team, position);
            position++;
        }
    }

    /// <summary>
    /// Puan durumu item'ı oluştur
    /// </summary>
    private void CreateStandingsItem(TeamData team, int position)
    {
        GameObject itemObj;

        if (standingsItemPrefab != null)
        {
            itemObj = Instantiate(standingsItemPrefab, standingsListParent);
        }
        else
        {
            itemObj = new GameObject($"StandingsItem_{team.teamName}");
            itemObj.transform.SetParent(standingsListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 50);

            Image bg = itemObj.AddComponent<Image>();
            bg.color = position <= 3 ? new Color(0.2f, 0.3f, 0.1f, 0.8f) : new Color(0.1f, 0.1f, 0.15f, 0.8f);

            // Sıra, takım adı ve güç
            GameObject textObj = new GameObject("StandingsText");
            textObj.transform.SetParent(itemObj.transform);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = $"{position}. {team.teamName} - Güç: {team.GetTeamPower()}";
            text.fontSize = 16;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            if (position <= 3)
                text.fontStyle = FontStyles.Bold;
        }

        // Prefab içinde TextMeshProUGUI varsa güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = $"{position}. {team.teamName} - Güç: {team.GetTeamPower()}";
            if (position <= 3)
                texts[0].fontStyle = FontStyles.Bold;
        }
    }

    private void OnBackButton()
    {
        gameObject.SetActive(false);
    }
}

