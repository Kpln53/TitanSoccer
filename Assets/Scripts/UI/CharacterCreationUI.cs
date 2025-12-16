using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Karakter oluşturma UI - Saç, ten rengi, forma, aksesuar seçimi
/// </summary>
public class CharacterCreationUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TMP_InputField playerSurnameInput;
    [SerializeField] private TMP_Dropdown nationalityDropdown;
    [SerializeField] private TMP_Dropdown positionDropdown;
    
    [Header("Karakter Özelleştirme")]
    [SerializeField] private TMP_Dropdown hairStyleDropdown;
    [SerializeField] private TMP_Dropdown skinColorDropdown;
    [SerializeField] private TMP_Dropdown jerseyLengthDropdown;
    [SerializeField] private Toggle glovesToggle;
    [SerializeField] private Toggle maskToggle;
    
    [Header("Butonlar")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backButton;
    
    [Header("Karakter Görseli (Preview)")]
    [SerializeField] private Image characterPreview;
    
    private CharacterAppearance characterAppearance = new CharacterAppearance();

    private void Start()
    {
        SetupDropdowns();
        SetupButtons();
    }

    private void SetupDropdowns()
    {
        // Pozisyon dropdown'ını doldur (Kaleci hariç)
        if (positionDropdown != null)
        {
            positionDropdown.ClearOptions();
            positionDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Stoper", "Sağ Bek", "Sol Bek", "Merkez Orta Defans",
                "Merkez Orta Ofans", "Sağ Kanat", "Sol Kanat", "Sağ Orta", "Sol Orta", "Santrafor"
            });
        }
        
        // Saç stili
        if (hairStyleDropdown != null)
        {
            hairStyleDropdown.ClearOptions();
            hairStyleDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Kısa", "Orta", "Uzun", "Düz", "Kıvırcık", "Kel"
            });
        }
        
        // Ten rengi
        if (skinColorDropdown != null)
        {
            skinColorDropdown.ClearOptions();
            skinColorDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Açık", "Orta", "Koyu"
            });
        }
        
        // Forma uzunluğu
        if (jerseyLengthDropdown != null)
        {
            jerseyLengthDropdown.ClearOptions();
            jerseyLengthDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Kısa Kollu", "Uzun Kollu"
            });
        }
        
        // Uyruk dropdown'ını doldur (basit liste)
        if (nationalityDropdown != null)
        {
            nationalityDropdown.ClearOptions();
            nationalityDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Türkiye", "Almanya", "Fransa", "İspanya", "İtalya", "İngiltere",
                "Brezilya", "Arjantin", "Portekiz", "Hollanda", "Diğer"
            });
        }
    }

    private void SetupButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButton);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
        }
    }

    private void OnContinueButton()
    {
        // Validasyon
        if (string.IsNullOrWhiteSpace(playerNameInput.text))
        {
            Debug.LogWarning("Oyuncu adı boş olamaz!");
            return;
        }
        
        // Karakter görünümünü kaydet
        SaveCharacterAppearance();
        
        // PlayerProfile oluştur
        PlayerProfile profile = CreatePlayerProfile();
        
        // SaveData'ya kaydet
        if (GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
        {
            GameManager.Instance.CurrentSave.playerProfile = profile;
            GameManager.Instance.CurrentSave.playerProfile.characterAppearance = characterAppearance;
        }
        
        // Sonraki ekrana geç (Lig seçimi veya takım teklifleri)
        if (GameStateManager.Instance != null)
        {
            // NewGameFlow'dan devam et
            GameStateManager.Instance.ChangeState(GameState.NewCareerFlow);
        }
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.SaveSlots);
        }
    }

    private void SaveCharacterAppearance()
    {
        if (hairStyleDropdown != null)
            characterAppearance.hairStyle = hairStyleDropdown.value;
        
        if (skinColorDropdown != null)
            characterAppearance.skinColor = skinColorDropdown.value;
        
        if (jerseyLengthDropdown != null)
            characterAppearance.jerseyLength = jerseyLengthDropdown.value;
        
        if (glovesToggle != null)
            characterAppearance.hasGloves = glovesToggle.isOn;
        
        if (maskToggle != null)
            characterAppearance.hasMask = maskToggle.isOn;
    }

    private PlayerProfile CreatePlayerProfile()
    {
        PlayerProfile profile = new PlayerProfile();
        
        profile.playerName = playerNameInput.text;
        profile.surname = playerSurnameInput.text;
        
        if (nationalityDropdown != null)
            profile.nationality = nationalityDropdown.options[nationalityDropdown.value].text;
        
        if (positionDropdown != null)
        {
            string posText = positionDropdown.options[positionDropdown.value].text;
            profile.position = ConvertPositionStringToEnum(posText);
        }
        
        profile.age = 18; // Başlangıç yaşı
        profile.overall = 60; // Başlangıç OVR
        
        // Başlangıç statları
        profile.pace = 60;
        profile.shooting = 60;
        profile.passing = 60;
        profile.dribbling = 60;
        profile.defense = 60;
        profile.stamina = 60;
        
        // Gizli değerler
        profile.injuryProne = 50;
        profile.morality = 75;
        profile.discipline = 75;
        
        // Form/Moral/Energy
        profile.form = 0.7f;
        profile.morale = 0.7f;
        profile.energy = 1f;
        
        return profile;
    }

    private PlayerPosition ConvertPositionStringToEnum(string position)
    {
        switch (position)
        {
            case "Stoper": return PlayerPosition.STP;
            case "Sağ Bek": return PlayerPosition.SĞB;
            case "Sol Bek": return PlayerPosition.SLB;
            case "Merkez Orta Defans": return PlayerPosition.MDO;
            case "Merkez Orta Ofans": return PlayerPosition.MOO;
            case "Sağ Kanat": return PlayerPosition.SĞK;
            case "Sol Kanat": return PlayerPosition.SLK;
            case "Sağ Orta": return PlayerPosition.SĞO;
            case "Sol Orta": return PlayerPosition.SLO;
            case "Santrafor": return PlayerPosition.SF;
            default: return PlayerPosition.MOO;
        }
    }
}

/// <summary>
/// Karakter görünüm verileri
/// </summary>
[System.Serializable]
public class CharacterAppearance
{
    public int hairStyle = 0;
    public int skinColor = 0;
    public int jerseyLength = 0; // 0 = kısa kollu, 1 = uzun kollu
    public bool hasGloves = false;
    public bool hasMask = false;
}

