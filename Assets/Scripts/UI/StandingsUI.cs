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

    [Header("Fikstür")]
    public Transform fixtureListParent;
    public GameObject fixtureItemPrefab;
    public TextMeshProUGUI currentWeekText;
    public Button prevWeekButton;
    public Button nextWeekButton;

    [Header("Geri Butonu")]
    public Button backButton;

    private int displayWeek = 1;

    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            displayWeek = GameManager.Instance.CurrentSave.seasonData.currentWeek;
        }
        
        // Butonları otomatik bul (eğer bağlı değilse)
        FindButtonsIfNeeded();
        
        SetupButtons();
        RefreshData();
    }

    /// <summary>
    /// Butonları otomatik bul (eğer Inspector'da bağlanmamışsa)
    /// </summary>
    private void FindButtonsIfNeeded()
    {
        // FixtureHeader altındaki butonları bul
        Transform fixtureHeader = transform.Find("FixtureArea/FixtureHeader");
        if (fixtureHeader != null)
        {
            Button[] buttons = fixtureHeader.GetComponentsInChildren<Button>();
            
            if (buttons.Length >= 2)
            {
                // Sol button (x pozisyonu küçük olan) = Prev
                // Sağ button (x pozisyonu büyük olan) = Next
                if (buttons[0].transform.localPosition.x < buttons[1].transform.localPosition.x)
                {
                    if (prevWeekButton == null) prevWeekButton = buttons[0];
                    if (nextWeekButton == null) nextWeekButton = buttons[1];
                }
                else
                {
                    if (prevWeekButton == null) prevWeekButton = buttons[1];
                    if (nextWeekButton == null) nextWeekButton = buttons[0];
                }
                
                Debug.Log($"[StandingsUI] Buttons auto-found: Prev={prevWeekButton?.name}, Next={nextWeekButton?.name}");
            }
            else
            {
                Debug.LogWarning($"[StandingsUI] Found {buttons.Length} buttons in FixtureHeader, expected 2!");
            }
        }
        else
        {
            Debug.LogWarning("[StandingsUI] FixtureHeader not found!");
        }
        
        // currentWeekText'i de bul
        if (currentWeekText == null && fixtureHeader != null)
        {
            currentWeekText = fixtureHeader.GetComponentInChildren<TextMeshProUGUI>();
            if (currentWeekText != null)
            {
                Debug.Log("[StandingsUI] currentWeekText auto-found!");
            }
        }
    }

    private void OnDisable()
    {
        RemoveButtonListeners();
    }

    private void SetupButtons()
    {
        // Önce mevcut listener'ları temizle
        RemoveButtonListeners();
        
        // Yeni listener'ları ekle
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
            Debug.Log($"[StandingsUI] Back button setup. Interactable: {backButton.interactable}");
        }
        
        if (prevWeekButton != null)
        {
            prevWeekButton.onClick.AddListener(OnPrevWeek);
            prevWeekButton.interactable = true; // Aktif yap
            Debug.Log($"[StandingsUI] Prev week button setup. Interactable: {prevWeekButton.interactable}");
        }
        
        if (nextWeekButton != null)
        {
            nextWeekButton.onClick.AddListener(OnNextWeek);
            nextWeekButton.interactable = true; // Aktif yap
            Debug.Log($"[StandingsUI] Next week button setup. Interactable: {nextWeekButton.interactable}");
        }
    }

    private void RemoveButtonListeners()
    {
        if (backButton != null) backButton.onClick.RemoveListener(OnBackButton);
        if (prevWeekButton != null) prevWeekButton.onClick.RemoveListener(OnPrevWeek);
        if (nextWeekButton != null) nextWeekButton.onClick.RemoveListener(OnNextWeek);
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

        // Fikstürü göster
        DisplayFixture();
    }

    private void OnPrevWeek()
    {
        Debug.Log($"[StandingsUI] OnPrevWeek called. Current week: {displayWeek}");
        if (displayWeek > 1)
        {
            displayWeek--;
            Debug.Log($"[StandingsUI] Week changed to: {displayWeek}");
            DisplayFixture();
        }
        else
        {
            Debug.Log("[StandingsUI] Already at week 1, cannot go back.");
        }
    }

    private void OnNextWeek()
    {
        Debug.Log($"[StandingsUI] OnNextWeek called. Current week: {displayWeek}");
        // Maksimum hafta kontrolü (Örn: 34)
        // DataPack'ten lig takım sayısını alıp hafta sayısını hesaplayabiliriz
        // Şimdilik 38 hafta varsayalım (20 takım)
        if (displayWeek < 38) 
        {
            displayWeek++;
            Debug.Log($"[StandingsUI] Week changed to: {displayWeek}");
            DisplayFixture();
        }
        else
        {
            Debug.Log("[StandingsUI] Already at week 38, cannot go forward.");
        }
    }

    private void DisplayFixture()
    {
        if (fixtureListParent == null)
        {
            Debug.LogWarning("[StandingsUI] fixtureListParent is null!");
            return;
        }
        
        if (currentWeekText != null) 
            currentWeekText.text = $"{displayWeek}. HAFTA";

        // Temizle
        foreach (Transform child in fixtureListParent) Destroy(child.gameObject);

        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            var fixtures = GameManager.Instance.CurrentSave.seasonData.fixtures;
            
            if (fixtures == null || fixtures.Count == 0)
            {
                Debug.LogWarning("[StandingsUI] No fixtures found in seasonData!");
                CreateNoFixtureMessage();
                return;
            }
            
            var weekMatches = fixtures.Where(m => m.weekNumber == displayWeek).ToList();
            
            Debug.Log($"[StandingsUI] Displaying week {displayWeek}: Found {weekMatches.Count} matches.");

            if (weekMatches.Count == 0)
            {
                CreateNoFixtureMessage();
            }
            else
            {
                foreach (var match in weekMatches)
                {
                    CreateFixtureItem(match);
                }
            }
        }
        else
        {
            Debug.LogWarning("[StandingsUI] GameManager or CurrentSave is null!");
        }
    }

    private void CreateNoFixtureMessage()
    {
        GameObject msgObj = new GameObject("NoFixtureMessage");
        msgObj.transform.SetParent(fixtureListParent);
        
        RectTransform rt = msgObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 60);
        
        TextMeshProUGUI txt = msgObj.AddComponent<TextMeshProUGUI>();
        txt.text = "Bu hafta için fikstür bulunamadı.";
        txt.fontSize = 16;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.gray;
    }

    private void CreateFixtureItem(MatchData match)
    {
        GameObject itemObj;
        if (fixtureItemPrefab != null)
        {
            itemObj = Instantiate(fixtureItemPrefab, fixtureListParent);
        }
        else
        {
            // Fallback UI
            itemObj = new GameObject("FixtureItem");
            itemObj.transform.SetParent(fixtureListParent);
            
            RectTransform rt = itemObj.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 40);
            
            TextMeshProUGUI txt = itemObj.AddComponent<TextMeshProUGUI>();
            txt.fontSize = 18;
            txt.alignment = TextAlignmentOptions.Center;
            
            string score = match.isPlayed ? $"{match.homeScore} - {match.awayScore}" : "v";
            txt.text = $"{match.homeTeamName}  {score}  {match.awayTeamName}";
            
            if (match.isPlayed) txt.color = Color.gray;
            else txt.color = Color.white;
        }
        
        // Prefab varsa
        var texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            string score = match.isPlayed ? $"{match.homeScore} - {match.awayScore}" : "-";
            texts[0].text = $"{match.homeTeamName}  <color=#FFD700>{score}</color>  {match.awayTeamName}";
        }
    }

    /// <summary>
    /// Puan durumunu göster
    /// </summary>
    private void DisplayStandings(string leagueName)
    {
        if (standingsListParent == null)
        {
            Debug.LogWarning("[StandingsUI] Cannot display standings - missing standingsListParent");
            return;
        }

        // Mevcut item'ları temizle - Memory optimized
        for (int i = standingsListParent.childCount - 1; i >= 0; i--)
        {
            Transform child = standingsListParent.GetChild(i);
            if (child != null)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        // Önce SeasonData'dan puan durumunu dene
        if (GameManager.Instance != null && GameManager.Instance.HasCurrentSave())
        {
            SeasonData seasonData = GameManager.Instance.CurrentSave.seasonData;
            if (seasonData != null && seasonData.standings != null && seasonData.standings.Count > 0)
            {
                // Puan durumu var - sıralı göster
                var sortedStandings = seasonData.GetSortedStandings();
                int position = 1;
                foreach (var standing in sortedStandings)
                {
                    CreateStandingsItemFromData(standing, position);
                    position++;
                }
                Debug.Log($"[StandingsUI] Displayed {sortedStandings.Count} teams from standings.");
                return;
            }
        }

        // Puan durumu yoksa DataPack'ten takımları göster (fallback)
        if (DataPackManager.Instance != null && DataPackManager.Instance.activeDataPack != null)
        {
            LeagueData league = DataPackManager.Instance.GetLeague(leagueName);
            if (league != null && league.teams != null)
            {
                // Takımları güce göre sırala (sezon başında puan yok)
                List<TeamData> sortedTeams = league.teams.OrderByDescending(t => t.GetTeamPower()).ToList();

                int position = 1;
                foreach (var team in sortedTeams)
                {
                    CreateStandingsItemFromTeam(team, position);
                    position++;
                }
                Debug.Log($"[StandingsUI] Displayed {sortedTeams.Count} teams from DataPack (no standings yet).");
            }
            else
            {
                Debug.LogWarning($"[StandingsUI] League not found: {leagueName}");
            }
        }
    }

    /// <summary>
    /// Puan durumu verisinden item oluştur
    /// </summary>
    private void CreateStandingsItemFromData(TeamStandingData standing, int position)
    {
        GameObject itemObj;
        string playerClub = GameManager.Instance?.CurrentSave?.clubData?.clubName ?? "";
        bool isPlayerTeam = standing.teamName == playerClub;

        if (standingsItemPrefab != null)
        {
            itemObj = Instantiate(standingsItemPrefab, standingsListParent);
        }
        else
        {
            itemObj = new GameObject($"StandingsItem_{standing.teamName}");
            itemObj.transform.SetParent(standingsListParent);

            RectTransform rect = itemObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 50);

            Image bg = itemObj.AddComponent<Image>();
            // Üst 3: Yeşil, Alt 3: Kırmızı, Oyuncunun takımı: Altın
            if (isPlayerTeam)
                bg.color = new Color(0.4f, 0.35f, 0.1f, 0.9f); // Altın
            else if (position <= 3)
                bg.color = new Color(0.1f, 0.3f, 0.1f, 0.8f); // Yeşil
            else if (position >= 16)
                bg.color = new Color(0.3f, 0.1f, 0.1f, 0.8f); // Kırmızı
            else
                bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f); // Normal

            // Tablo formatı: Sıra | Takım | O | G | B | M | A | Y | Av | P
            GameObject textObj = new GameObject("StandingsText");
            textObj.transform.SetParent(itemObj.transform);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = FormatStandingRow(position, standing);
            text.fontSize = 14;
            text.color = isPlayerTeam ? new Color(1f, 0.9f, 0.3f) : Color.white;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            if (position <= 3 || isPlayerTeam)
                text.fontStyle = FontStyles.Bold;
        }

        // Prefab içinde TextMeshProUGUI varsa güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = FormatStandingRow(position, standing);
            if (position <= 3 || isPlayerTeam)
                texts[0].fontStyle = FontStyles.Bold;
            if (isPlayerTeam)
                texts[0].color = new Color(1f, 0.9f, 0.3f);
        }
    }

    /// <summary>
    /// Takım verisinden item oluştur (sezon başında puan yok)
    /// </summary>
    private void CreateStandingsItemFromTeam(TeamData team, int position)
    {
        GameObject itemObj;
        string playerClub = GameManager.Instance?.CurrentSave?.clubData?.clubName ?? "";
        bool isPlayerTeam = team.teamName == playerClub;

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
            if (isPlayerTeam)
                bg.color = new Color(0.4f, 0.35f, 0.1f, 0.9f); // Altın
            else
                bg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);

            GameObject textObj = new GameObject("StandingsText");
            textObj.transform.SetParent(itemObj.transform);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = $"{position,2}. {team.teamName,-20}  0  0  0  0   0   0   0   0";
            text.fontSize = 14;
            text.color = isPlayerTeam ? new Color(1f, 0.9f, 0.3f) : Color.white;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            if (isPlayerTeam)
                text.fontStyle = FontStyles.Bold;
        }

        // Prefab içinde TextMeshProUGUI varsa güncelle
        TextMeshProUGUI[] texts = itemObj.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            texts[0].text = $"{position,2}. {team.teamName,-20}  0  0  0  0   0   0   0   0";
            if (isPlayerTeam)
            {
                texts[0].fontStyle = FontStyles.Bold;
                texts[0].color = new Color(1f, 0.9f, 0.3f);
            }
        }
    }

    /// <summary>
    /// Puan durumu satırını formatla
    /// </summary>
    private string FormatStandingRow(int position, TeamStandingData s)
    {
        // Format: Sıra | Takım | O | G | B | M | A | Y | Av | P
        return $"{position,2}. {s.teamName,-20} {s.played,2} {s.wins,2} {s.draws,2} {s.losses,2}  {s.goalsFor,2}  {s.goalsAgainst,2}  {s.GoalDifference,3}  {s.points,2}";
    }

    private void OnBackButton()
    {
        gameObject.SetActive(false);
    }
}
