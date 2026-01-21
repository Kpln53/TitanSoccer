using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transfer teklifi - Transfer sistemi için
/// </summary>
[System.Serializable]
public class TransferOffer
{
    [Header("Kulüp Bilgileri")]
    public string clubName;        // Teklif eden kulüp
    public string leagueName;      // Kulüp ligi
    
    [Header("Sözleşme Detayları")]
    public int salary;             // Maaş
    public int contractDuration;   // Sözleşme süresi (yıl)
    public PlayingTime playingTime; // Oynama süresi garantisi
    public ContractRole role;      // Rol
    
    [Header("Primler ve Maddeler")]
    public int signingBonus;       // İmza parası
    public List<ClauseType> clauses; // Sözleşme maddeleri
    
    [Header("Teklif Bilgileri")]
    public TransferOfferType offerType; // Teklif türü (Permanent/Loan)
    public DateTime offerDate;     // Teklif tarihi
    public string offerDateString; // Tarih (string)
    
    [Header("Durum")]
    public bool isAccepted;        // Kabul edildi mi?
    public bool isRejected;        // Reddedildi mi?
    public bool isExpired;         // Süresi doldu mu?
    
    public TransferOffer()
    {
        clauses = new List<ClauseType>();
        offerType = TransferOfferType.Permanent;
        offerDate = DateTime.Now;
        offerDateString = offerDate.ToString("yyyy-MM-dd");
        isAccepted = false;
        isRejected = false;
        isExpired = false;
    }
}

