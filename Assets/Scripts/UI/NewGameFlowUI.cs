using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class NewGameFlowUI : MonoBehaviour
{
    [Header("UI Referansları")]
    public TMP_InputField playerNameInput;
    public TMP_Dropdown positionDropdown;
    public TMP_Dropdown leagueDropdown; // Lig seçimi (clubDropdown yerine)

    private void Start()
    {
        // Hangi referanslar boş, başta bir kere loglayalım
        if (playerNameInput == null) Debug.LogError("playerNameInput Inspector'da atanmadı!");
        if (positionDropdown == null) Debug.LogError("positionDropdown Inspector'da atanmadı!");
        if (leagueDropdown == null) Debug.LogError("leagueDropdown Inspector'da atanmadı!");

        // Pozisyon listesi
        if (positionDropdown != null)
        {
            positionDropdown.ClearOptions();
            positionDropdown.AddOptions(new List<string>
            {
                "SF", "SĞO", "SLO", "MOO", "MDO", "SĞK", "SLK", "STP", "SĞB", "SLB", "KL"
            });
        }

        // Lig listesi (DataPackManager'dan)
        SetupLeagueDropdown();
    }
    
    /// <summary>
    /// Lig dropdown'ını ayarla
    /// </summary>
    void SetupLeagueDropdown()
    {
        if (leagueDropdown == null) return;
        
        leagueDropdown.ClearOptions();
        
        DataPackManager dataPackManager = DataPackManager.Instance;
        if (dataPackManager == null)
        {
            Debug.LogError("[NewGameFlowUI] DataPackManager bulunamadı!");
            return;
        }
        
        DataPack activePack = dataPackManager.GetActiveDataPack();
        if (activePack == null)
        {
            Debug.LogError("[NewGameFlowUI] Aktif Data Pack bulunamadı!");
            return;
        }
        
        List<string> leagueNames = new List<string>();
        foreach (var league in activePack.leagues)
        {
            if (league != null && league.teams != null && league.teams.Count > 0)
            {
                leagueNames.Add(league.leagueName);
            }
        }
        
        if (leagueNames.Count == 0)
        {
            Debug.LogWarning("[NewGameFlowUI] Hiç lig bulunamadı!");
            return;
        }
        
        leagueDropdown.AddOptions(leagueNames);
    }

    public void OnStartCareerButton()
    {
        Debug.Log("OnStartCareerButton çağrıldı.");

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager yok, akış bozuk!");
            return;
        }

        // --- EN ÖNEMLİ KISIM: NULL KONTROL ---
        if (playerNameInput == null)
        {
            Debug.LogError("playerNameInput NULL! Inspector'da bağlaman lazım.");
            return;
        }
        if (positionDropdown == null)
        {
            Debug.LogError("positionDropdown NULL! Inspector'da bağlaman lazım.");
            return;
        }
        if (leagueDropdown == null)
        {
            Debug.LogError("leagueDropdown NULL! Inspector'da bağlaman lazım.");
            return;
        }
        // ---------------------------------------

        string playerName = playerNameInput.text;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("İsim boş olamaz.");
            return;
        }

        string pos = positionDropdown.options[positionDropdown.value].text;
        string leagueName = leagueDropdown.options[leagueDropdown.value].text;

        // Yeni SaveData oluştur
        SaveData data = new SaveData();
        
        // PlayerProfile oluştur
        data.playerProfile = new PlayerProfile();
        data.playerProfile.playerName = playerName;
        data.playerProfile.surname = ""; // Karakter oluşturma ekranında eklenecek
        data.playerProfile.position = ConvertPositionStringToEnum(pos);
        data.playerProfile.age = 18;
        data.playerProfile.overall = 60;
        
        // Başlangıç statları
        data.playerProfile.pace = 60;
        data.playerProfile.shooting = 60;
        data.playerProfile.passing = 60;
        data.playerProfile.dribbling = 60;
        data.playerProfile.defense = 60;
        data.playerProfile.stamina = 60;
        
        // ClubData
        data.clubData = new ClubData();
        data.clubData.leagueName = leagueName;
        data.clubData.clubName = ""; // Takım seçim ekranında seçilecek
        
        // SeasonData
        data.seasonData = new SeasonData();
        data.seasonData.seasonNumber = 1;
        data.seasonData.currentWeek = 1;
        
        // Eski uyumluluk için (deprecated)
        data.playerName = playerName;
        data.position = pos;
        data.leagueName = leagueName;
        data.clubName = "";
        data.season = 1;
        data.overall = 60;

        int slotIndex = GameManager.Instance.CurrentSaveSlotIndex;
        if (slotIndex < 0)
        {
            Debug.LogError("Geçerli slot index yok!");
            return;
        }

        // Önce SaveData'yı kaydet (takım seçimi için)
        SaveSystem.SaveGame(data, slotIndex);
        GameManager.Instance.SetCurrentSave(data, slotIndex);

        // Takım seçim ekranına geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.NewCareerFlow);
        }
        else
        {
            SceneManager.LoadScene("TeamSelection");
        }
    }
    
    /// <summary>
    /// Position string'ini enum'a çevir
    /// </summary>
    private PlayerPosition ConvertPositionStringToEnum(string position)
    {
        switch (position)
        {
            case "SF": return PlayerPosition.SF;
            case "SĞO": return PlayerPosition.SĞO;
            case "SLO": return PlayerPosition.SLO;
            case "MOO": return PlayerPosition.MOO;
            case "MDO": return PlayerPosition.MDO;
            case "SĞK": return PlayerPosition.SĞK;
            case "SLK": return PlayerPosition.SLK;
            case "STP": return PlayerPosition.STP;
            case "SĞB": return PlayerPosition.SĞB;
            case "SLB": return PlayerPosition.SLB;
            case "KL": return PlayerPosition.KL;
            default: return PlayerPosition.MOO;
        }
    }
}
