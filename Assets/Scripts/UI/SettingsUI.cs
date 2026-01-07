using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Ayarlar UI - Oyun ayarları ekranı
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;

    [Header("Grafik Ayarları")]
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    [Header("Butonlar")]
    public Button backButton;
    public Button saveButton;

    private void Start()
    {
        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        // Ses slider'ları
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            OnMasterVolumeChanged(masterVolumeSlider.value);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            OnMusicVolumeChanged(musicVolumeSlider.value);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            OnSFXVolumeChanged(sfxVolumeSlider.value);
        }

        // Kalite dropdown'ı
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Very Low", "Low", "Medium", "High", "Very High", "Ultra"
            });
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        // Fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);

        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButton);
    }

    private void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        if (masterVolumeText != null)
            masterVolumeText.text = $"Master: {(int)(value * 100)}%";
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (musicVolumeText != null)
            musicVolumeText.text = $"Music: {(int)(value * 100)}%";
        // TODO: Music volume ayarı (AudioManager varsa)
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"SFX: {(int)(value * 100)}%";
        // TODO: SFX volume ayarı (AudioManager varsa)
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    private void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }

    private void OnSaveButton()
    {
        // Ayarları kaydet
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);

        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

        if (sfxVolumeSlider != null)
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);

        PlayerPrefs.Save();
        Debug.Log("[SettingsUI] Settings saved!");
    }

    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToPreviousState();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}







