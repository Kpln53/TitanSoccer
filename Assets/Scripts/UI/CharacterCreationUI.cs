using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Karakter oluşturma UI - Yeni kariyer için karakter oluşturma
/// </summary>
public class CharacterCreationUI : MonoBehaviour
{
    [Header("Oyuncu Bilgileri")]
    public TMP_InputField playerNameInput;
    public TMP_Dropdown positionDropdown;
    public Slider ageSlider;
    public TextMeshProUGUI ageText;
    public TMP_InputField nationalityInput;

    [Header("Yetenekler (Slider'lar)")]
    public Slider passingSlider;
    public Slider shootingSlider;
    public Slider dribblingSlider;
    public Slider falsoSlider;
    public Slider speedSlider;
    public Slider staminaSlider;
    public Slider defendingSlider;
    public Slider physicalSlider;

    [Header("Butonlar")]
    public Button createButton;
    public Button backButton;

    private PlayerProfile newPlayerProfile;

    private void Start()
    {
        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        // Yaş slider'ı
        if (ageSlider != null)
        {
            ageSlider.minValue = 16;
            ageSlider.maxValue = 35;
            ageSlider.value = 20;
            ageSlider.onValueChanged.AddListener(OnAgeChanged);
            OnAgeChanged(ageSlider.value);
        }

        // Yetenek slider'ları
        SetupSkillSlider(passingSlider);
        SetupSkillSlider(shootingSlider);
        SetupSkillSlider(dribblingSlider);
        SetupSkillSlider(falsoSlider);
        SetupSkillSlider(speedSlider);
        SetupSkillSlider(staminaSlider);
        SetupSkillSlider(defendingSlider);
        SetupSkillSlider(physicalSlider);

        // Pozisyon dropdown'ı
        if (positionDropdown != null)
        {
            positionDropdown.ClearOptions();
            positionDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Kaleci (KL)", "Stoper (STP)", "Sağ Bek (SĞB)", "Sol Bek (SLB)",
                "Merkez Orta Defans (MDO)", "Merkez Orta Ofans (MOO)",
                "Sağ Kanat (SĞK)", "Sol Kanat (SLK)", "Sağ Orta (SĞO)", "Sol Orta (SLO)",
                "Santrafor (SF)"
            });
        }
    }

    private void SetupSkillSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 50;
        }
    }

    private void SetupButtons()
    {
        if (createButton != null)
            createButton.onClick.AddListener(OnCreateButton);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    private void OnAgeChanged(float value)
    {
        if (ageText != null)
            ageText.text = $"Yaş: {(int)value}";
    }

    private void OnCreateButton()
    {
        if (playerNameInput == null || string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogWarning("[CharacterCreationUI] Player name is required!");
            return;
        }

        // Yeni oyuncu profili oluştur
        newPlayerProfile = new PlayerProfile
        {
            playerName = playerNameInput.text,
            age = (int)(ageSlider != null ? ageSlider.value : 20),
            nationality = nationalityInput != null ? nationalityInput.text : "Unknown"
        };

        // Pozisyon seç
        if (positionDropdown != null)
        {
            newPlayerProfile.position = ConvertPositionIndexToEnum(positionDropdown.value);
        }

        // Yetenekleri ayarla
        newPlayerProfile.passingSkill = (int)(passingSlider != null ? passingSlider.value : 50);
        newPlayerProfile.shootingSkill = (int)(shootingSlider != null ? shootingSlider.value : 50);
        newPlayerProfile.dribblingSkill = (int)(dribblingSlider != null ? dribblingSlider.value : 50);
        newPlayerProfile.falsoSkill = (int)(falsoSlider != null ? falsoSlider.value : 50);
        newPlayerProfile.speed = (int)(speedSlider != null ? speedSlider.value : 50);
        newPlayerProfile.stamina = (int)(staminaSlider != null ? staminaSlider.value : 50);
        newPlayerProfile.defendingSkill = (int)(defendingSlider != null ? defendingSlider.value : 50);
        newPlayerProfile.physicalStrength = (int)(physicalSlider != null ? physicalSlider.value : 50);

        // Overall hesapla
        newPlayerProfile.CalculateOverall();

        // Yeni SaveData oluştur
        SaveData newSave = new SaveData
        {
            playerProfile = newPlayerProfile,
            clubData = new ClubData(),
            seasonData = new SeasonData(),
            relationsData = new RelationsData(),
            economyData = new EconomyData(),
            mediaData = new MediaData()
        };

        // GameManager'a kaydet
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentSave(newSave, GameManager.Instance.CurrentSaveSlotIndex);
        }

        // TeamOffer sahnesine geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.TeamOffer);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TeamOffer");
        }
    }

    private PlayerPosition ConvertPositionIndexToEnum(int index)
    {
        return index switch
        {
            0 => PlayerPosition.KL,
            1 => PlayerPosition.STP,
            2 => PlayerPosition.SĞB,
            3 => PlayerPosition.SLB,
            4 => PlayerPosition.MDO,
            5 => PlayerPosition.MOO,
            6 => PlayerPosition.SĞK,
            7 => PlayerPosition.SLK,
            8 => PlayerPosition.SĞO,
            9 => PlayerPosition.SLO,
            10 => PlayerPosition.SF,
            _ => PlayerPosition.MOO
        };
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToPreviousState();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SaveSlots");
        }
    }
}

