using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kritik olay pop-up sistemi
/// </summary>
public class CriticalEventSystem : MonoBehaviour
{
    public static CriticalEventSystem Instance;

    private Queue<CriticalEvent> eventQueue = new Queue<CriticalEvent>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Kritik olay ekle
    /// </summary>
    public void AddCriticalEvent(CriticalEvent criticalEvent)
    {
        eventQueue.Enqueue(criticalEvent);
        
        // Eğer GameStateManager varsa, kritik olay durumuna geç
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ShowCriticalEvent();
        }
    }

    /// <summary>
    /// Sıradaki kritik olayı al
    /// </summary>
    public CriticalEvent GetNextEvent()
    {
        if (eventQueue.Count > 0)
        {
            return eventQueue.Dequeue();
        }
        return null;
    }

    /// <summary>
    /// Bekleyen olay var mı?
    /// </summary>
    public bool HasPendingEvents()
    {
        return eventQueue.Count > 0;
    }

    /// <summary>
    /// Transfer teklifi olayı oluştur
    /// </summary>
    public void CreateTransferOfferEvent(TransferOffer offer)
    {
        CriticalEvent evt = new CriticalEvent
        {
            eventType = CriticalEventType.TransferOffer,
            title = "Transfer Teklifi",
            description = $"{offer.teamName} size transfer teklifi yaptı!",
            transferOffer = offer
        };
        
        AddCriticalEvent(evt);
    }

    /// <summary>
    /// Sakatlık olayı oluştur
    /// </summary>
    public void CreateInjuryEvent(InjuryResult injury)
    {
        CriticalEvent evt = new CriticalEvent
        {
            eventType = CriticalEventType.Injury,
            title = "Sakatlık!",
            description = $"Sakatlandınız! {injury.injuryType} - {injury.durationDays} gün sürecek.",
            injuryResult = injury
        };
        
        AddCriticalEvent(evt);
    }

    /// <summary>
    /// Önemli haber olayı oluştur
    /// </summary>
    public void CreateImportantNewsEvent(NewsItem news)
    {
        CriticalEvent evt = new CriticalEvent
        {
            eventType = CriticalEventType.ImportantNews,
            title = news.title,
            description = news.content,
            newsItem = news
        };
        
        AddCriticalEvent(evt);
    }
}

/// <summary>
/// Kritik olay
/// </summary>
[System.Serializable]
public class CriticalEvent
{
    public CriticalEventType eventType;
    public string title;
    public string description;
    
    // Olay tipine göre ek veriler
    public TransferOffer transferOffer;
    public InjuryResult injuryResult;
    public NewsItem newsItem;
}

public enum CriticalEventType
{
    TransferOffer,
    Injury,
    ImportantNews,
    BribeOffer,
    RelationshipEvent,
    Other
}

