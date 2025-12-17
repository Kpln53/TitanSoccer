using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Kritik olay pop-up UI - Transfer teklifi, sakatlık, önemli haberler
/// </summary>
public class CriticalEventPopUpUI : MonoBehaviour
{
    [Header("UI Referansları")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Button closeButton;
    
    [Header("Transfer Offer UI")]
    [SerializeField] private GameObject transferOfferPanel;
    [SerializeField] private TextMeshProUGUI transferTeamNameText;
    [SerializeField] private TextMeshProUGUI transferSalaryText;
    [SerializeField] private TextMeshProUGUI transferDurationText;
    [SerializeField] private TextMeshProUGUI transferPlayingTimeText;
    
    [Header("Injury UI")]
    [SerializeField] private GameObject injuryPanel;
    [SerializeField] private TextMeshProUGUI injuryTypeText;
    [SerializeField] private TextMeshProUGUI injuryDurationText;

    private CriticalEvent currentEvent;

    private void Start()
    {
        SetupButtons();
        
        if (popupPanel != null)
            popupPanel.SetActive(false);
        
        if (transferOfferPanel != null)
            transferOfferPanel.SetActive(false);
        
        if (injuryPanel != null)
            injuryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Event geldiğinde göster
        CheckForEvents();
    }

    private void SetupButtons()
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(OnAcceptButton);
        }
        
        if (rejectButton != null)
        {
            rejectButton.onClick.AddListener(OnRejectButton);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButton);
        }
    }

    /// <summary>
    /// Bekleyen olayları kontrol et
    /// </summary>
    private void CheckForEvents()
    {
        if (CriticalEventSystem.Instance == null)
            return;
        
        if (CriticalEventSystem.Instance.HasPendingEvents())
        {
            CriticalEvent evt = CriticalEventSystem.Instance.GetNextEvent();
            if (evt != null)
            {
                ShowEvent(evt);
            }
        }
    }

    /// <summary>
    /// Olayı göster
    /// </summary>
    public void ShowEvent(CriticalEvent evt)
    {
        currentEvent = evt;
        
        if (popupPanel == null)
            return;
        
        popupPanel.SetActive(true);
        
        if (titleText != null)
            titleText.text = evt.title;
        
        if (descriptionText != null)
            descriptionText.text = evt.description;
        
        // Olay tipine göre özel UI göster
        switch (evt.eventType)
        {
            case CriticalEventType.TransferOffer:
                ShowTransferOfferUI(evt);
                break;
            case CriticalEventType.Injury:
                ShowInjuryUI(evt);
                break;
            case CriticalEventType.ImportantNews:
                ShowNewsUI(evt);
                break;
        }
        
        // Butonları ayarla
        SetupEventButtons(evt);
    }

    /// <summary>
    /// Transfer teklifi UI'sini göster
    /// </summary>
    private void ShowTransferOfferUI(CriticalEvent evt)
    {
        if (transferOfferPanel == null || evt.transferOffer == null)
            return;
        
        transferOfferPanel.SetActive(true);
        
        var offer = evt.transferOffer;
        
        if (transferTeamNameText != null)
            transferTeamNameText.text = offer.teamName;
        
        if (transferSalaryText != null)
            transferSalaryText.text = $"Maaş: {offer.monthlySalary:N0} €/ay";
        
        if (transferDurationText != null)
            transferDurationText.text = $"Süre: {offer.contractDuration} ay";
        
        if (transferPlayingTimeText != null)
        {
            string playingTimeStr = "";
            switch (offer.playingTime)
            {
                case PlayingTime.Starter:
                    playingTimeStr = "İlk 11";
                    break;
                case PlayingTime.Rotation:
                    playingTimeStr = "Rotasyon";
                    break;
                case PlayingTime.Substitute:
                    playingTimeStr = "Yedek";
                    break;
            }
            transferPlayingTimeText.text = $"Oynama Zamanı: {playingTimeStr}";
        }
    }

    /// <summary>
    /// Sakatlık UI'sini göster
    /// </summary>
    private void ShowInjuryUI(CriticalEvent evt)
    {
        if (injuryPanel == null || evt.injuryResult == null)
            return;
        
        injuryPanel.SetActive(true);
        
        var injury = evt.injuryResult;
        
        if (injuryTypeText != null)
            injuryTypeText.text = $"Sakatlık: {injury.injuryType}";
        
        if (injuryDurationText != null)
            injuryDurationText.text = $"Süre: {injury.durationDays} gün";
    }

    /// <summary>
    /// Haber UI'sini göster
    /// </summary>
    private void ShowNewsUI(CriticalEvent evt)
    {
        // Haber için özel UI yoksa genel pop-up kullanılır
    }

    /// <summary>
    /// Olay butonlarını ayarla
    /// </summary>
    private void SetupEventButtons(CriticalEvent evt)
    {
        // Transfer teklifi için Accept/Reject butonları
        if (evt.eventType == CriticalEventType.TransferOffer)
        {
            if (acceptButton != null)
            {
                acceptButton.gameObject.SetActive(true);
            }
            
            if (rejectButton != null)
            {
                rejectButton.gameObject.SetActive(true);
            }
        }
        else
        {
            // Diğer olaylar için sadece Close butonu
            if (acceptButton != null)
                acceptButton.gameObject.SetActive(false);
            
            if (rejectButton != null)
                rejectButton.gameObject.SetActive(false);
        }
    }

    private void OnAcceptButton()
    {
        if (currentEvent == null)
            return;
        
        if (currentEvent.eventType == CriticalEventType.TransferOffer && currentEvent.transferOffer != null)
        {
            // Transfer teklifini kabul et
            if (TransferSystem.Instance != null && GameManager.Instance != null && GameManager.Instance.CurrentSave != null)
            {
                TransferSystem.Instance.AcceptOffer(currentEvent.transferOffer, GameManager.Instance.CurrentSave);
            }
        }
        
        ClosePopup();
    }

    private void OnRejectButton()
    {
        if (currentEvent == null)
            return;
        
        if (currentEvent.eventType == CriticalEventType.TransferOffer && currentEvent.transferOffer != null)
        {
            // Transfer teklifini reddet
            if (TransferSystem.Instance != null)
            {
                TransferSystem.Instance.RejectOffer(currentEvent.transferOffer);
            }
        }
        
        ClosePopup();
    }

    private void OnCloseButton()
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        
        if (transferOfferPanel != null)
            transferOfferPanel.SetActive(false);
        
        if (injuryPanel != null)
            injuryPanel.SetActive(false);
        
        // Kritik olay kapatıldı
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.CloseCriticalEvent();
        }
        
        // Bir sonraki olayı kontrol et
        CheckForEvents();
    }
}


