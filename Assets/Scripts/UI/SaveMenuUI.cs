using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SaveMenuUI : MonoBehaviour
{
    [Header("References")]
    public Transform slotsContainer;
    public List<SaveSlotUI> saveSlots;
    
    private void OnEnable()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        // Slotları bul (eğer liste boşsa)
        if (saveSlots == null || saveSlots.Count == 0)
        {
            saveSlots = new List<SaveSlotUI>();
            if (slotsContainer != null)
            {
                foreach (Transform child in slotsContainer)
                {
                    SaveSlotUI slot = child.GetComponent<SaveSlotUI>();
                    if (slot != null) saveSlots.Add(slot);
                }
            }
        }

        for (int i = 0; i < saveSlots.Count; i++)
        {
            int slotIndex = i;
            SaveSlotUI slotUI = saveSlots[i];
            
            // Slot verisini kontrol et
            bool hasSave = SaveSystem.HasSave(slotIndex);
            SaveData data = hasSave ? SaveSystem.LoadGame(slotIndex) : null;

            // UI Güncelle
            slotUI.Setup(hasSave, data);

            // Tıklama olayı
            slotUI.slotButton.onClick.RemoveAllListeners();
            slotUI.slotButton.onClick.AddListener(() => OnSlotClicked(slotIndex, hasSave));

            // Silme butonu olayı
            if (slotUI.deleteButton != null)
            {
                slotUI.deleteButton.onClick.RemoveAllListeners();
                slotUI.deleteButton.onClick.AddListener(() => OnDeleteClicked(slotIndex));
            }
        }
    }

    private void OnDeleteClicked(int slotIndex)
    {
        // Kaydı sil
        SaveSystem.DeleteSave(slotIndex);
        
        // UI'ı yenile
        RefreshSlots();
        
        Debug.Log($"[SaveMenuUI] Slot {slotIndex} deleted.");
    }

    private void OnSlotClicked(int slotIndex, bool hasSave)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is null! Cannot proceed.");
            return;
        }

        if (hasSave)
        {
            // Kayıtlı oyunu yükle
            SaveData data = SaveSystem.LoadGame(slotIndex);
            GameManager.Instance.SetCurrentSave(data, slotIndex);
            SceneFlow.LoadCareerHub();
        }
        else
        {
            // Yeni oyun başlat -> Karakter Oluşturma
            GameManager.Instance.SetSaveSlotIndex(slotIndex);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
        }
    }
}
