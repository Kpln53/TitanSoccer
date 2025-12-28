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

    [Header("Slot Index (Manuel Ayar)")]
    [Tooltip("Eğer GameObject adından otomatik bulunamazsa, buraya manuel olarak slot index'i girin (0, 1, 2, ...)")]
    public int manualSlotIndex = -1; // Inspector'dan manuel ayarlanabilir

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

        // Önce manuel slot index'i kontrol et
        if (manualSlotIndex >= 0)
        {
            slotIndex = manualSlotIndex;
            Debug.Log($"[SaveSlotUI] Using manual slot index: {slotIndex}");
        }
        else
        {
            // Slot index'ini GameObject adından al (örn: "SaveSlot0" -> 0)
            string name = gameObject.name;
            Debug.Log($"[SaveSlotUI] Initializing slot with GameObject name: '{name}'");
            
            if (name.Contains("Slot"))
            {
                // Farklı formatları dene
                string indexStr = name.Replace("SaveSlot", "").Replace("Slot", "").Trim();
                
                // Eğer hala boşsa, sadece sayıları al
                if (string.IsNullOrEmpty(indexStr))
                {
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\d+");
                    var match = regex.Match(name);
                    if (match.Success)
                    {
                        indexStr = match.Value;
                    }
                }
                
                Debug.Log($"[SaveSlotUI] Extracted index string: '{indexStr}'");
                
                if (int.TryParse(indexStr, out int index))
                {
                    slotIndex = index;
                    Debug.Log($"[SaveSlotUI] Slot index set to: {slotIndex}");
                }
                else
                {
                    Debug.LogWarning($"[SaveSlotUI] Failed to parse slot index from name: '{name}'. Please set manualSlotIndex in Inspector!");
                }
            }
            else
            {
                Debug.LogWarning($"[SaveSlotUI] GameObject name '{name}' does not contain 'Slot'. Please set manualSlotIndex in Inspector!");
            }
        }

        Refresh();
    }

    /// <summary>
    /// Slot'u yenile (kayıt durumuna göre)
    /// </summary>
    public void Refresh()
    {
        Debug.Log($"[SaveSlotUI] Refresh called for slot index: {slotIndex}");
        
        hasSave = SaveSystem.HasSave(slotIndex);
        Debug.Log($"[SaveSlotUI] HasSave result: {hasSave}");

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
        Debug.Log($"[SaveSlotUI] DisplaySaveData called for slot {slotIndex}");
        
        if (saveData == null)
        {
            Debug.LogWarning($"[SaveSlotUI] SaveData is null for slot {slotIndex}!");
            DisplayEmpty();
            return;
        }
        
        Debug.Log($"[SaveSlotUI] Player: {saveData.playerProfile?.playerName ?? "NULL"}, Club: {saveData.clubData?.clubName ?? "NULL"}");
        
        if (slotNumberText != null)
            slotNumberText.text = $"Slot {slotIndex + 1}";

        if (playerNameText != null)
        {
            string playerName = saveData.playerProfile != null ? saveData.playerProfile.playerName : "Unknown Player";
            playerNameText.text = playerName;
            Debug.Log($"[SaveSlotUI] Player name set to: {playerName}");
        }

        if (clubNameText != null)
        {
            string clubName = saveData.clubData != null ? saveData.clubData.clubName : "No Club";
            clubNameText.text = clubName;
            Debug.Log($"[SaveSlotUI] Club name set to: {clubName}");
        }

        if (seasonText != null)
        {
            string seasonStr = saveData.seasonData != null ? $"Sezon {saveData.seasonData.seasonNumber}" : "Season 1";
            seasonText.text = seasonStr;
            Debug.Log($"[SaveSlotUI] Season set to: {seasonStr}");
        }

        if (dateText != null)
        {
            DateTime saveDate = SaveSystem.GetSaveDate(slotIndex);
            if (saveDate != DateTime.MinValue)
            {
                dateText.text = saveDate.ToString("dd.MM.yyyy HH:mm");
                Debug.Log($"[SaveSlotUI] Date set to: {saveDate}");
            }
            else
            {
                dateText.text = "Date Unknown";
                Debug.LogWarning($"[SaveSlotUI] Save date is MinValue for slot {slotIndex}");
            }
        }
        
        Debug.Log($"[SaveSlotUI] Save data displayed successfully for slot {slotIndex}");
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

