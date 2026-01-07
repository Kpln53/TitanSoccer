using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Transfer sistemi - Transfer teklifi kabul/red ve transfer işlemleri (Singleton)
/// </summary>
public class TransferSystem : MonoBehaviour
{
    public static TransferSystem Instance { get; private set; }

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

        Debug.Log("[TransferSystem] TransferSystem initialized.");
    }

    /// <summary>
    /// Transfer teklifini kabul et
    /// </summary>
    public bool AcceptOffer(SaveData saveData, TransferOffer offer)
    {
        if (saveData == null || offer == null)
        {
            Debug.LogWarning("[TransferSystem] SaveData or TransferOffer is null! Cannot accept offer.");
            return false;
        }

        if (offer.isAccepted || offer.isRejected || offer.isExpired)
        {
            Debug.LogWarning("[TransferSystem] Offer is already processed or expired!");
            return false;
        }

        // Kulüp bilgilerini güncelle
        if (saveData.clubData == null)
        {
            saveData.clubData = new ClubData();
        }

        saveData.clubData.clubName = offer.clubName;
        saveData.clubData.leagueName = offer.leagueName;

        // Sözleşmeyi güncelle
        if (saveData.clubData.contract == null)
        {
            saveData.clubData.contract = new ContractData();
        }

        saveData.clubData.contract.salary = offer.salary;
        saveData.clubData.contract.contractDuration = offer.contractDuration;
        saveData.clubData.contract.playingTime = offer.playingTime;
        
        // PlayingTime'ı ContractRole'a çevir
        saveData.clubData.contract.role = ConvertPlayingTimeToRole(offer.playingTime);
        
        saveData.clubData.contract.clauses = new System.Collections.Generic.List<ClauseType>(offer.clauses);
        saveData.clubData.contract.startDate = System.DateTime.Now;
        saveData.clubData.contract.startDateString = System.DateTime.Now.ToString("yyyy-MM-dd");
        saveData.clubData.contract.endDate = saveData.clubData.contract.startDate.AddYears(offer.contractDuration);
        saveData.clubData.contract.endDateString = saveData.clubData.contract.endDate.ToString("yyyy-MM-dd");
        saveData.clubData.contract.signingBonus = offer.signingBonus;
        saveData.clubData.contract.clubName = offer.clubName;

        // Fikstür ve Puan Durumu Oluşturma
        if (DataPackManager.Instance != null && !string.IsNullOrEmpty(saveData.clubData.leagueName))
        {
            LeagueData league = DataPackManager.Instance.GetLeague(saveData.clubData.leagueName);
            if (league != null && league.teams != null && league.teams.Count > 0)
            {
                // Sezon başlangıç tarihi (Sözleşme başlangıcı veya şimdiki zaman)
                DateTime seasonStart = saveData.clubData.contract.startDate;
                
                // Fikstürü oluştur
                List<MatchData> fixtures = FixtureGenerator.GenerateSeasonFixtures(league.teams, seasonStart);
                
                // Sezon verilerine kaydet
                if (saveData.seasonData == null) saveData.seasonData = new SeasonData();
                
                saveData.seasonData.fixtures = fixtures;
                saveData.seasonData.seasonStartDate = seasonStart;
                saveData.seasonData.seasonStartDateString = seasonStart.ToString("yyyy-MM-dd");
                
                // Puan durumunu başlat
                saveData.seasonData.InitializeStandings(league.teams);
                
                // Sezon bitişini son maç tarihine göre güncelle
                if (fixtures.Count > 0)
                {
                    DateTime lastMatchDate = fixtures[fixtures.Count - 1].matchDate;
                    saveData.seasonData.seasonEndDate = lastMatchDate;
                    saveData.seasonData.seasonEndDateString = lastMatchDate.ToString("yyyy-MM-dd");
                }
                
                Debug.Log($"[TransferSystem] Season fixtures generated: {fixtures.Count} matches, {league.teams.Count} teams in standings.");
            }
            else
            {
                Debug.LogWarning($"[TransferSystem] Could not find league '{saveData.clubData.leagueName}' or teams to generate fixtures!");
            }
        }

        // Teklifi işaretle
        offer.isAccepted = true;

        Debug.Log($"[TransferSystem] Transfer offer accepted: {offer.clubName}");
        return true;
    }

    /// <summary>
    /// Transfer teklifini reddet
    /// </summary>
    public void RejectOffer(TransferOffer offer)
    {
        if (offer == null)
            return;

        if (offer.isAccepted || offer.isRejected)
        {
            Debug.LogWarning("[TransferSystem] Offer is already processed!");
            return;
        }

        offer.isRejected = true;
        Debug.Log($"[TransferSystem] Transfer offer rejected: {offer.clubName}");
    }

    /// <summary>
    /// PlayingTime'ı ContractRole'a çevir
    /// </summary>
    private ContractRole ConvertPlayingTimeToRole(PlayingTime playingTime)
    {
        return playingTime switch
        {
            PlayingTime.Starter => ContractRole.Starter,
            PlayingTime.Rotation => ContractRole.Rotation,
            PlayingTime.Substitute => ContractRole.Substitute,
            _ => ContractRole.Rotation
        };
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}







