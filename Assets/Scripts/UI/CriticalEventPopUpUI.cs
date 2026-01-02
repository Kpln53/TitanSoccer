using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Kritik olay popup UI - Önemli olaylar için popup ekranı
/// </summary>
public class CriticalEventPopUpUI : MonoBehaviour
{
    [Header("Popup Elemanları")]
    public GameObject popupPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Button confirmButton;
    public Button cancelButton;

    private System.Action onConfirm;
    private System.Action onCancel;

    private void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButton);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButton);
    }

    /// <summary>
    /// Popup göster
    /// </summary>
    public void ShowPopup(string title, string content, System.Action onConfirmAction = null, System.Action onCancelAction = null)
    {
        if (popupPanel == null) return;

        popupPanel.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        if (contentText != null)
            contentText.text = content;

        onConfirm = onConfirmAction;
        onCancel = onCancelAction;

        // Cancel butonu sadece onCancel varsa göster
        if (cancelButton != null)
            cancelButton.gameObject.SetActive(onCancel != null);
    }

    /// <summary>
    /// Popup'ı gizle
    /// </summary>
    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        onConfirm = null;
        onCancel = null;
    }

    private void OnConfirmButton()
    {
        onConfirm?.Invoke();
        HidePopup();
    }

    private void OnCancelButton()
    {
        onCancel?.Invoke();
        HidePopup();
    }
}





