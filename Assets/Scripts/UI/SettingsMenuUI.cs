using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public TextMeshProUGUI volumeValueText;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Game Settings")]
    public TMP_Dropdown languageDropdown;
    public Toggle notificationsToggle; // Öneri: Bildirimler

    [Header("Cloud Settings")]
    public Toggle cloudSaveToggle;
    public TextMeshProUGUI cloudStatusText;

    private void Start()
    {
        LoadSettings();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (musicToggle) musicToggle.onValueChanged.AddListener(OnMusicToggled);
        if (sfxToggle) sfxToggle.onValueChanged.AddListener(OnSfxToggled);
        if (languageDropdown) languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        if (notificationsToggle) notificationsToggle.onValueChanged.AddListener(OnNotificationsToggled);
        if (cloudSaveToggle) cloudSaveToggle.onValueChanged.AddListener(OnCloudSaveToggled);
    }

    private void LoadSettings()
    {
        // Varsayılan değerler veya kayıtlı değerler
        if (masterVolumeSlider) 
        {
            float vol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.value = vol;
            UpdateVolumeText(vol);
        }

        if (musicToggle) musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        if (sfxToggle) sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        if (languageDropdown) languageDropdown.value = PlayerPrefs.GetInt("Language", 0); // 0: TR, 1: EN
        if (notificationsToggle) notificationsToggle.isOn = PlayerPrefs.GetInt("Notifications", 1) == 1;
        
        // Cloud Save (Visual)
        if (cloudSaveToggle) 
        {
            bool isCloudOn = PlayerPrefs.GetInt("CloudSave", 0) == 1;
            cloudSaveToggle.isOn = isCloudOn;
            UpdateCloudText(isCloudOn);
        }
    }

    // --- CALLBACKS ---

    private void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        UpdateVolumeText(value);
        // AudioListener.volume = value; // Gerçek ses kontrolü
    }

    private void UpdateVolumeText(float value)
    {
        if (volumeValueText) volumeValueText.text = $"%{Mathf.RoundToInt(value * 100)}";
    }

    private void OnMusicToggled(bool isOn)
    {
        PlayerPrefs.SetInt("MusicEnabled", isOn ? 1 : 0);
    }

    private void OnSfxToggled(bool isOn)
    {
        PlayerPrefs.SetInt("SFXEnabled", isOn ? 1 : 0);
    }

    private void OnLanguageChanged(int index)
    {
        PlayerPrefs.SetInt("Language", index);
        Debug.Log($"Dil değiştirildi: {(index == 0 ? "Türkçe" : "English")}");
    }

    private void OnNotificationsToggled(bool isOn)
    {
        PlayerPrefs.SetInt("Notifications", isOn ? 1 : 0);
    }

    private void OnCloudSaveToggled(bool isOn)
    {
        PlayerPrefs.SetInt("CloudSave", isOn ? 1 : 0);
        UpdateCloudText(isOn);
    }

    private void UpdateCloudText(bool isOn)
    {
        if (cloudStatusText)
        {
            cloudStatusText.text = isOn ? "Durum: <color=green>AKTİF</color>" : "Durum: <color=red>PASİF</color>";
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
