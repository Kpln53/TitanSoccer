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
        Debug.Log("[SaveMenuUI] RefreshSlots called");
        
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
                Debug.Log($"[SaveMenuUI] Found {saveSlots.Count} save slots");
            }
            else
            {
                Debug.LogWarning("[SaveMenuUI] slotsContainer is NULL!");
            }
        }

        for (int i = 0; i < saveSlots.Count; i++)
        {
            int slotIndex = i;
            SaveSlotUI slotUI = saveSlots[i];
            
            if (slotUI == null)
            {
                Debug.LogWarning($"[SaveMenuUI] Slot {i} is NULL!");
                continue;
            }
            
            // Slot verisini kontrol et
            bool hasSave = SaveSystem.HasSave(slotIndex);
            SaveData data = hasSave ? SaveSystem.LoadGame(slotIndex) : null;

            Debug.Log($"[SaveMenuUI] Slot {i}: hasSave={hasSave}");

            // UI Güncelle
            slotUI.Setup(hasSave, data);

            // Tıklama olayları - Hem empty hem filled button'a listener ekle
            
            // Empty button (yeni kariyer)
            if (slotUI.emptyButton != null)
            {
                slotUI.emptyButton.onClick.RemoveAllListeners();
                slotUI.emptyButton.onClick.AddListener(() => OnSlotClicked(slotIndex, false));
                Debug.Log($"[SaveMenuUI] Empty button listener added to slot {slotIndex}. Interactable: {slotUI.emptyButton.interactable}");
            }
            else
            {
                Debug.LogWarning($"[SaveMenuUI] Slot {slotIndex} emptyButton is NULL!");
            }
            
            // Filled button (kayıtlı oyun)
            if (slotUI.filledButton != null)
            {
                slotUI.filledButton.onClick.RemoveAllListeners();
                slotUI.filledButton.onClick.AddListener(() => OnSlotClicked(slotIndex, true));
                Debug.Log($"[SaveMenuUI] Filled button listener added to slot {slotIndex}. Interactable: {slotUI.filledButton.interactable}");
            }
            else
            {
                Debug.LogWarning($"[SaveMenuUI] Slot {slotIndex} filledButton is NULL!");
            }
            
            // Fallback: Eski slotButton sistemi (eğer yeni butonlar yoksa)
            if (slotUI.slotButton != null && slotUI.emptyButton == null && slotUI.filledButton == null)
            {
                slotUI.slotButton.onClick.RemoveAllListeners();
                slotUI.slotButton.onClick.AddListener(() => OnSlotClicked(slotIndex, hasSave));
                Debug.Log($"[SaveMenuUI] Fallback slotButton listener added to slot {slotIndex}. Interactable: {slotUI.slotButton.interactable}");
            }

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
        Debug.Log($"[SaveMenuUI] Slot {slotIndex} clicked. HasSave: {hasSave}");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("[SaveMenuUI] GameManager instance is null! Cannot proceed.");
            return;
        }

        if (hasSave)
        {
            // Kayıtlı oyunu yükle
            Debug.Log($"[SaveMenuUI] Loading saved game from slot {slotIndex}...");
            SaveData data = SaveSystem.LoadGame(slotIndex);
            GameManager.Instance.SetCurrentSave(data, slotIndex);
            SceneFlow.LoadCareerHub();
        }
        else
        {
            // Yeni oyun başlat -> Karakter Oluşturma
            Debug.Log($"[SaveMenuUI] Starting new career in slot {slotIndex}...");
            GameManager.Instance.SetSaveSlotIndex(slotIndex);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
        }
    }
}
