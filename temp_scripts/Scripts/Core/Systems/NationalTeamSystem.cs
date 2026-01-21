using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Milli takım sistemi - Seçilme kriterleri ve takvim entegrasyonu (Singleton)
/// </summary>
public class NationalTeamSystem : MonoBehaviour
{
    public static NationalTeamSystem Instance { get; private set; }

    [Header("Milli Takım Ayarları")]
    public int minimumOverallForSelection = 70; // Seçilme için minimum overall

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

        Debug.Log("[NationalTeamSystem] NationalTeamSystem initialized.");
    }

    /// <summary>
    /// Oyuncunun milli takıma seçilme kriterlerini kontrol et
    /// </summary>
    public bool CheckSelectionCriteria(PlayerProfile player)
    {
        if (player == null)
            return false;

        // Overall kontrolü
        if (player.overall < minimumOverallForSelection)
        {
            return false;
        }

        // Form kontrolü (form düşükse seçilmeyebilir)
        if (player.form < 40f)
        {
            return false;
        }

        // Sakatlık kontrolü
        if (player.IsInjured())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Milli takıma seçilme şansını hesapla (0-100)
    /// </summary>
    public float CalculateSelectionChance(PlayerProfile player)
    {
        if (player == null)
            return 0f;

        if (!CheckSelectionCriteria(player))
            return 0f;

        float chance = 50f; // Base chance

        // Overall'a göre artış
        chance += (player.overall - minimumOverallForSelection) * 2f;

        // Form'a göre artış/azalış
        chance += (player.form - 50f) * 0.5f;

        // Sezon performansına göre (rating)
        if (player.careerAverageRating > 7.0f)
        {
            chance += 20f;
        }
        else if (player.careerAverageRating > 6.5f)
        {
            chance += 10f;
        }

        return Mathf.Clamp(chance, 0f, 100f);
    }

    /// <summary>
    /// Milli takım çağrısı oluştur
    /// </summary>
    public NationalTeamCall CreateNationalTeamCall(PlayerProfile player, string teamName, string competition)
    {
        if (player == null || !CheckSelectionCriteria(player))
        {
            return null;
        }

        NationalTeamCall call = new NationalTeamCall
        {
            teamName = teamName,
            competition = competition,
            callDate = System.DateTime.Now,
            callDateString = System.DateTime.Now.ToString("yyyy-MM-dd")
        };

        Debug.Log($"[NationalTeamSystem] National team call created: {teamName} - {competition}");
        return call;
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
/// Milli takım çağrısı
/// </summary>
[System.Serializable]
public class NationalTeamCall
{
    public string teamName;
    public string competition; // "World Cup", "Euro", "Friendly", vb.
    public System.DateTime callDate;
    public string callDateString;
    public bool isAccepted;
    public bool isRejected;

    public NationalTeamCall()
    {
        isAccepted = false;
        isRejected = false;
    }
}


