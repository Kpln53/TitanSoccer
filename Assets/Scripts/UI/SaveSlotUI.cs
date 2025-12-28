using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Kayıt slotu UI - Tek bir kayıt slotunu gösterir
/// </summary>
public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI slotNumberText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI clubNameText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI dateText;
    public Button continueButton;
    public Button newGameButton;
    public Button deleteButton;

    private SaveSlotsMenu parentMenu;
    private int slotIndex = -1;
    private bool hasSave = false;

    /// <summary>
    /// Slot'u başlat
    /// </summary>
    public void Initialize(SaveSlotsMenu menu)
    {
        parentMenu = menu;

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameButton);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDeleteButton);

        // Slot index'ini GameObject adından al (örn: "SaveSlot0" -> 0)
        string name = gameObject.name;
        if (name.Contains("Slot"))
        {
            string indexStr = name.Replace("SaveSlot", "").Replace("Slot", "");
            if (int.TryParse(indexStr, out int index))
            {
                slotIndex = index;
            }
        }

        Refresh();
    }

    /// <summary>
    /// Slot'u yenile (kayıt durumuna göre)
    /// </summary>
    public void Refresh()
    {
        hasSave = SaveSystem.HasSave(slotIndex);

        if (hasSave)
        {
            // Kayıt yükle ve göster
            SaveData saveData = SaveSystem.LoadGame(slotIndex);
            if (saveData != null)
            {
                DisplaySaveData(saveData);
            }
            else
            {
                DisplayEmpty();
            }
        }
        else
        {
            // Boş slot göster
            DisplayEmpty();
        }

        // Butonları ayarla
        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSave);

        if (newGameButton != null)
            newGameButton.gameObject.SetActive(!hasSave);

        if (deleteButton != null)
            deleteButton.gameObject.SetActive(hasSave);
    }

    /// <summary>
    /// Kayıt verilerini göster
    /// </summary>
    private void DisplaySaveData(SaveData saveData)
    {
        if (slotNumberText != null)
            slotNumberText.text = $"Slot {slotIndex + 1}";

        if (playerNameText != null)
            playerNameText.text = saveData.playerProfile != null ? saveData.playerProfile.playerName : "Unknown Player";

        if (clubNameText != null)
            clubNameText.text = saveData.clubData != null ? saveData.clubData.clubName : "No Club";

        if (seasonText != null)
            seasonText.text = saveData.seasonData != null ? $"Sezon {saveData.seasonData.seasonNumber}" : "Season 1";

        if (dateText != null)
        {
            DateTime saveDate = SaveSystem.GetSaveDate(slotIndex);
            if (saveDate != DateTime.MinValue)
            {
                dateText.text = saveDate.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                dateText.text = "Date Unknown";
            }
        }
    }

    /// <summary>
    /// Boş slot göster
    /// </summary>
    private void DisplayEmpty()
    {
        if (slotNumberText != null)
            slotNumberText.text = $"Slot {slotIndex + 1}";

        if (playerNameText != null)
            playerNameText.text = "Empty Slot";

        if (clubNameText != null)
            clubNameText.text = "";

        if (seasonText != null)
            seasonText.text = "";

        if (dateText != null)
            dateText.text = "";
    }

    private void OnContinueButton()
    {
        if (hasSave && parentMenu != null)
        {
            SaveData saveData = SaveSystem.LoadGame(slotIndex);
            if (saveData != null)
            {
                parentMenu.OnSlotContinue(saveData, slotIndex);
            }
        }
    }

    private void OnNewGameButton()
    {
        if (!hasSave && parentMenu != null)
        {
            parentMenu.OnSlotNewGame(slotIndex);
        }
    }

    private void OnDeleteButton()
    {
        if (hasSave)
        {
            SaveSystem.DeleteSave(slotIndex);
            Refresh();
            
            if (parentMenu != null)
            {
                parentMenu.RefreshAllSlots();
            }
        }
    }
}

