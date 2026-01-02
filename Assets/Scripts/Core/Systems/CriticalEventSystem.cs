using UnityEngine;
using System;

/// <summary>
/// Kritik olaylar sistemi - Popup olaylar (transfer, sakatlık, vb.) (Singleton)
/// </summary>
public class CriticalEventSystem : MonoBehaviour
{
    public static CriticalEventSystem Instance { get; private set; }

    // Event callback'leri
    public event Action<CriticalEvent> OnEventTriggered;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[CriticalEventSystem] CriticalEventSystem initialized.");
    }

    /// <summary>
    /// Kritik olay tetikle
    /// </summary>
    public void TriggerEvent(CriticalEventType eventType, object eventData = null)
    {
        CriticalEvent criticalEvent = new CriticalEvent
        {
            eventType = eventType,
            eventData = eventData,
            triggerTime = DateTime.Now,
            triggerTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            isHandled = false
        };

        Debug.Log($"[CriticalEventSystem] Critical event triggered: {eventType}");

        // Event'i tetikle (UI dinleyecek)
        OnEventTriggered?.Invoke(criticalEvent);
    }

    /// <summary>
    /// Transfer teklifi olayı oluştur
    /// </summary>
    public void CreateTransferOfferEvent(TransferOffer offer)
    {
        if (offer == null)
            return;

        TriggerEvent(CriticalEventType.TransferOffer, offer);
    }

    /// <summary>
    /// Sakatlık olayı oluştur
    /// </summary>
    public void CreateInjuryEvent(InjuryRecord injury)
    {
        if (injury == null)
            return;

        TriggerEvent(CriticalEventType.Injury, injury);
    }

    /// <summary>
    /// Sözleşme yenileme olayı oluştur
    /// </summary>
    public void CreateContractRenewalEvent()
    {
        TriggerEvent(CriticalEventType.ContractRenewal);
    }

    /// <summary>
    /// Milli takım çağrısı olayı oluştur
    /// </summary>
    public void CreateNationalTeamCallEvent(NationalTeamCall call)
    {
        if (call == null)
            return;

        TriggerEvent(CriticalEventType.NationalTeamCall, call);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

/// <summary>
/// Kritik olay
/// </summary>
[Serializable]
public class CriticalEvent
{
    public CriticalEventType eventType;  // Olay türü
    public object eventData;            // Olay verisi (TransferOffer, InjuryRecord, vb.)
    public DateTime triggerTime;        // Tetiklenme zamanı
    public string triggerTimeString;    // Tarih (string)
    public bool isHandled;             // İşlendi mi?
}





