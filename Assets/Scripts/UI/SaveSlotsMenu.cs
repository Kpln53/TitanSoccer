using UnityEngine;

/// <summary>
/// Kayıt slotları menüsü - Kayıt slotlarını gösterir
/// </summary>
public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Slot Referansları")]
    public SaveSlotUI[] slotUIs;

    [Header("Geri Butonu")]
    public UnityEngine.UI.Button backButton;

    private void Awake()
    {
        Debug.Log($"[SaveSlotsMenu] Awake called. Found {slotUIs?.Length ?? 0} slot UI(s).");
        
        // Her slota bu menüyü tanıt
        if (slotUIs != null)
        {
            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null)
                {
                    Debug.Log($"[SaveSlotsMenu] Initializing slot UI {i}: {slotUIs[i].gameObject.name}");
                    slotUIs[i].Initialize(this);
                }
                else
                {
                    Debug.LogWarning($"[SaveSlotsMenu] Slot UI {i} is NULL!");
                }
            }
        }
        else
        {
            Debug.LogError("[SaveSlotsMenu] slotUIs array is NULL! Please assign slot UIs in Inspector.");
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
        else
            Debug.LogWarning("[SaveSlotsMenu] Back button is NULL!");
    }

    private void OnEnable()
    {
        Debug.Log("[SaveSlotsMenu] OnEnable called. Refreshing all slots...");
        RefreshAllSlots();
    }

    /// <summary>
    /// Tüm slotları yenile
    /// </summary>
    public void RefreshAllSlots()
    {
        Debug.Log($"[SaveSlotsMenu] RefreshAllSlots called. Found {slotUIs?.Length ?? 0} slot UI(s).");
        
        if (slotUIs != null)
        {
            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null)
                {
                    Debug.Log($"[SaveSlotsMenu] Refreshing slot UI {i}: {slotUIs[i].gameObject.name}");
                    slotUIs[i].Refresh();
                }
            }
        }
        else
        {
            Debug.LogError("[SaveSlotsMenu] slotUIs array is NULL! Cannot refresh slots.");
        }
    }

    /// <summary>
    /// Slot doluysa "Devam Et" buradan gelir
    /// </summary>
    public void OnSlotContinue(SaveData data, int slotIndex)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentSave(data, slotIndex);
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CareerHub);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CareerHub");
        }
    }

    /// <summary>
    /// Slot boşsa "Yeni Oyun" buradan gelir
    /// </summary>
    public void OnSlotNewGame(int slotIndex)
    {
        // Yeni oyun için slot index'i ayarla
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSaveSlotIndex(slotIndex);
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.CharacterCreation);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterCreation");
        }
    }

    /// <summary>
    /// Geri butonu
    /// </summary>
    private void OnBackButton()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ReturnToMainMenu();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}
