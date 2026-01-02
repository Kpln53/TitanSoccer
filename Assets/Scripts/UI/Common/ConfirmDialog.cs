using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Confirm Dialog - Onay dialog'u
/// </summary>
public class ConfirmDialog : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    private System.Action onConfirm;
    private System.Action onCancel;

    private void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancel);
    }

    /// <summary>
    /// Dialog'u göster
    /// </summary>
    public void Show(string message, System.Action onConfirm, System.Action onCancel = null)
    {
        if (messageText != null)
            messageText.text = message;

        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);
    }

    /// <summary>
    /// Dialog'u gizle
    /// </summary>
    public void Hide()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        onConfirm = null;
        onCancel = null;
    }

    private void OnConfirm()
    {
        onConfirm?.Invoke();
        Hide();
    }

    private void OnCancel()
    {
        onCancel?.Invoke();
        Hide();
    }
}

