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
        // Her slota bu menüyü tanıt
        if (slotUIs != null)
        {
            foreach (var slot in slotUIs)
            {
                if (slot != null)
                    slot.Initialize(this);
            }
        }

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    private void OnEnable()
    {
        RefreshAllSlots();
    }

    /// <summary>
    /// Tüm slotları yenile
    /// </summary>
    public void RefreshAllSlots()
    {
        if (slotUIs != null)
        {
            foreach (var slot in slotUIs)
            {
                if (slot != null)
                    slot.Refresh();
            }
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentSaveSlotIndex = slotIndex;
            GameManager.Instance.CurrentSave = null; // yeni kariyer
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

