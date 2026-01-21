using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Maç simülasyon sistemi - Maçları simüle eder ve sonuçları hesaplar (Singleton)
/// </summary>
public class MatchSimulationSystem : MonoBehaviour
{
    public static MatchSimulationSystem Instance { get; private set; }

    [Header("Simülasyon Ayarları")]
    [Range(0.1f, 5f)]
    public float simulationSpeed = 1f; // Simülasyon hızı (1x, 2x, 3x)
    
    private MatchData currentMatch;
    private bool isSimulating = false;
    private bool isPaused = false;
    private Coroutine simulationCoroutine;
    private MatchChanceData pendingChance; // Bekleyen pozisyon (2D sahne sonuç bekliyor)

    // Event callbacks
    public System.Action<int, int> OnScoreChanged; // (homeScore, awayScore)
    public System.Action<int> OnMinuteChanged; // (minute)
    public System.Action<float> OnPlayerRatingChanged; // (rating)
    public System.Action<int> OnBallPossessionChanged; // (homePossessionPercentage)
    public System.Action<string> OnCommentaryChanged; // (commentary)
    public System.Action<MatchEvent> OnMatchEvent; // (event)
    public System.Action OnMatchFinished; // Maç bitti
    public System.Action<MatchChanceData> OnPlayerChance; // Oyuncuya pozisyon geldi

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
        Debug.Log("[MatchSimulationSystem] MatchSimulationSystem initialized.");
    }

    /// <summary>
    /// Maçı başlat (simülasyon)
    /// </summary>
    public void StartMatch(MatchData match)
    {
        if (match == null)
        {
            Debug.LogError("[MatchSimulationSystem] Cannot start match: match data is null!");
            return;
        }

        if (isSimulating)
        {
            Debug.LogWarning("[MatchSimulationSystem] Match simulation already in progress!");
            return;
        }

        currentMatch = match;
        currentMatch.matchEvents.Clear();
        currentMatch.homeScore = 0;
        currentMatch.awayScore = 0;
        currentMatch.playerRating = 6.5f; // Başlangıç rating'i
        currentMatch.playerMinutesPlayed = 0;
        currentMatch.playerGoals = 0;
        currentMatch.playerAssists = 0;
        currentMatch.isPlayed = false;

        Debug.Log($"[MatchSimulationSystem] Starting match: {match.homeTeam} vs {match.awayTeam}");

        // Maç başlangıç olayı
        AddMatchEvent(0, MatchEventType.MatchStart, match.homeTeam, "", "Maç başladı!");
        
        // İlk yorum
        if (CommentatorSystem.Instance != null)
        {
            string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.MatchStart);
            OnCommentaryChanged?.Invoke(commentary);
        }

        // Simülasyonu başlat
        isSimulating = true;
        isPaused = false;
        Time.timeScale = simulationSpeed; // Başlangıç hızını ayarla
        simulationCoroutine = StartCoroutine(SimulateMatch());
    }

    /// <summary>
    /// Maç simülasyonu coroutine'i
    /// </summary>
    private IEnumerator SimulateMatch()
    {
        SaveData saveData = GameManager.Instance?.CurrentSave;
        if (saveData == null)
        {
            Debug.LogError("[MatchSimulationSystem] No save data available!");
            yield break;
        }

        string playerTeamName = saveData.clubData.clubName;
        PlayerProfile playerProfile = saveData.playerProfile;
        bool isPlayerHome = currentMatch.homeTeam == playerTeamName;

        // Takım güçlerini al
        int homeTeamPower = GetTeamPower(currentMatch.homeTeam);
        int awayTeamPower = GetTeamPower(currentMatch.awayTeam);

        // Başlangıç pozisyonlarını hesapla (güç farkına göre)
        int baseHomePossession = 50 + (homeTeamPower - awayTeamPower) / 2;
        baseHomePossession = Mathf.Clamp(baseHomePossession, 30, 70);
        int currentHomePossession = baseHomePossession;

        // Skorlar
        int homeScore = 0;
        int awayScore = 0;

        // Oyuncu rating'i (başlangıç)
        float playerRating = 6.5f;
        int playerGoals = 0;
        int playerAssists = 0;

        // Maç dakikası
        int minute = 0;

        // İlk yarı (0-45)
        while (minute < 45 && isSimulating)
        {
            minute++;
            yield return new WaitForSeconds(1f / simulationSpeed);

            // Pozisyon değişimi (rastgele)
            if (Random.Range(0, 100) < 10) // %10 şans
            {
                currentHomePossession = baseHomePossession + Random.Range(-10, 11);
                currentHomePossession = Mathf.Clamp(currentHomePossession, 30, 70);
                OnBallPossessionChanged?.Invoke(currentHomePossession);
            }

            // Gol fırsatı kontrolü (her dakika)
            if (Random.Range(0, 100) < 15) // %15 şans her dakika (artırıldı)
            {
                bool isHomeAttack = Random.Range(0, 100) < currentHomePossession;
                bool isPlayerTeamAttack = (isHomeAttack && isPlayerHome) || (!isHomeAttack && !isPlayerHome);

                // Eğer oyuncunun takımı atak yapıyorsa, pozisyon oyunu başlat
                if (isPlayerTeamAttack)
                {
                    // Pozisyon oluştur
                    MatchChanceData chance = CreatePlayerChance(minute, isHomeAttack, homeTeamPower, awayTeamPower);
                    pendingChance = chance;
                    
                    // Simülasyonu duraklat ve 2D pozisyon sahnesine geç
                    isPaused = true;
                    Time.timeScale = 0f;
                    OnPlayerChance?.Invoke(chance);
                    
                    // Pozisyon sonucu gelene kadar bekle (2D sahne sonuç dönecek)
                    yield return StartCoroutine(WaitForChanceDecision(chance));
                    
                    // Pozisyon sonucuna göre işlem yap
                    ProcessChanceResult(chance, ref homeScore, ref awayScore, ref playerGoals, ref playerRating, 
                        isHomeAttack, currentMatch, OnScoreChanged, OnCommentaryChanged);
                    
                    // Simülasyona devam et
                    isPaused = false;
                    pendingChance = null;
                    Time.timeScale = simulationSpeed;
                }
                else
                {
                    // Rakip takım atak yapıyorsa otomatik simüle et
                    int attackPower = isHomeAttack ? homeTeamPower : awayTeamPower;
                    int defensePower = isHomeAttack ? awayTeamPower : homeTeamPower;
                    int powerDifference = attackPower - defensePower;
                    int goalChance = Mathf.Clamp(40 + powerDifference + Random.Range(-15, 26), 0, 100);

                    if (goalChance > 35) // Gol!
                    {
                        if (isHomeAttack)
                            homeScore++;
                        else
                            awayScore++;

                        AddMatchEvent(minute, MatchEventType.Goal, 
                            isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam, 
                            GetRandomPlayerName(isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam), 
                            $"{(isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam)} gol attı!");

                        OnScoreChanged?.Invoke(homeScore, awayScore);

                        if (CommentatorSystem.Instance != null)
                        {
                            string teamName = isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam;
                            string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.Goal, "", teamName);
                            OnCommentaryChanged?.Invoke(commentary);
                        }
                    }
                    else
                    {
                        AddMatchEvent(minute, MatchEventType.ChanceMissed, 
                            isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam, 
                            "", "Fırsat kaçtı!");
                    }
                }
            }

            // Oyuncu rating güncellemesi (yavaşça artar/azalır)
            if (Random.Range(0, 100) < 5) // %5 şans
            {
                float ratingChange = Random.Range(-0.1f, 0.1f);
                playerRating = Mathf.Clamp(playerRating + ratingChange, 4.0f, 10.0f);
                OnPlayerRatingChanged?.Invoke(playerRating);
            }

            // Rastgele yorumlar (normal maç akışı)
            if (Random.Range(0, 100) < 3) // %3 şans her dakika
            {
                if (CommentatorSystem.Instance != null)
                {
                    string[] generalComments = {
                        "Maç dengeli devam ediyor.",
                        "Her iki takım da iyi oynuyor.",
                        "Top orta sahadaki mücadelede.",
                        "Takımlar pozisyon arayışında.",
                        "Maç tempolu geçiyor."
                    };
                    string commentary = generalComments[Random.Range(0, generalComments.Length)];
                    OnCommentaryChanged?.Invoke(commentary);
                }
            }

            // UI güncellemesi
            OnMinuteChanged?.Invoke(minute);
        }

        // Devre arası
        if (isSimulating)
        {
            AddMatchEvent(45, MatchEventType.HalfTime, "", "", "İlk yarı sona erdi.");
            if (CommentatorSystem.Instance != null)
            {
                string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.HalfTime);
                OnCommentaryChanged?.Invoke(commentary);
            }
            yield return new WaitForSeconds(2f / simulationSpeed);
        }

        // İkinci yarı başlangıcı
        if (isSimulating && CommentatorSystem.Instance != null)
        {
            string secondHalfCommentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.SecondHalfStart);
            OnCommentaryChanged?.Invoke(secondHalfCommentary);
        }

        // İkinci yarı (45-90)
        while (minute < 90 && isSimulating)
        {
            minute++;
            yield return new WaitForSeconds(1f / simulationSpeed);

            // Pozisyon değişimi
            if (Random.Range(0, 100) < 10)
            {
                currentHomePossession = baseHomePossession + Random.Range(-10, 11);
                currentHomePossession = Mathf.Clamp(currentHomePossession, 30, 70);
                OnBallPossessionChanged?.Invoke(currentHomePossession);
            }

            // Gol fırsatı
            if (Random.Range(0, 100) < 15) // %15 şans her dakika (artırıldı)
            {
                bool isHomeAttack = Random.Range(0, 100) < currentHomePossession;
                int attackPower = isHomeAttack ? homeTeamPower : awayTeamPower;
                int defensePower = isHomeAttack ? awayTeamPower : homeTeamPower;
                int powerDifference = attackPower - defensePower;
                int goalChance = Mathf.Clamp(40 + powerDifference + Random.Range(-15, 26), 0, 100); // Eşik düşürüldü

                if (goalChance > 35) // Gol! (eşik 35'e düşürüldü)
                {
                    if (isHomeAttack)
                    {
                        homeScore++;
                        AddMatchEvent(minute, MatchEventType.Goal, currentMatch.homeTeam, 
                            GetRandomPlayerName(currentMatch.homeTeam), 
                            $"{currentMatch.homeTeam} gol attı!");

                        if (isPlayerHome && Random.Range(0, 100) < 30)
                        {
                            playerGoals++;
                            playerRating += 0.3f;
                        }
                    }
                    else
                    {
                        awayScore++;
                        AddMatchEvent(minute, MatchEventType.Goal, currentMatch.awayTeam, 
                            GetRandomPlayerName(currentMatch.awayTeam), 
                            $"{currentMatch.awayTeam} gol attı!");

                        if (!isPlayerHome && Random.Range(0, 100) < 30)
                        {
                            playerGoals++;
                            playerRating += 0.3f;
                        }
                    }

                    OnScoreChanged?.Invoke(homeScore, awayScore);

                    if (CommentatorSystem.Instance != null)
                    {
                        string teamName = isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam;
                        string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.Goal, "", teamName);
                        OnCommentaryChanged?.Invoke(commentary);
                    }
                }
                else
                {
                    AddMatchEvent(minute, MatchEventType.ChanceMissed, 
                        isHomeAttack ? currentMatch.homeTeam : currentMatch.awayTeam, 
                        "", "Fırsat kaçtı!");
                }
            }

            // Rating güncellemesi
            if (Random.Range(0, 100) < 5)
            {
                float ratingChange = Random.Range(-0.1f, 0.1f);
                playerRating = Mathf.Clamp(playerRating + ratingChange, 4.0f, 10.0f);
                OnPlayerRatingChanged?.Invoke(playerRating);
            }

            // Rastgele yorumlar (normal maç akışı)
            if (Random.Range(0, 100) < 3) // %3 şans her dakika
            {
                if (CommentatorSystem.Instance != null)
                {
                    string[] generalComments = {
                        "Maç dengeli devam ediyor.",
                        "Her iki takım da iyi oynuyor.",
                        "Top orta sahadaki mücadelede.",
                        "Takımlar pozisyon arayışında.",
                        "Maç tempolu geçiyor."
                    };
                    string commentary = generalComments[Random.Range(0, generalComments.Length)];
                    OnCommentaryChanged?.Invoke(commentary);
                }
            }

            OnMinuteChanged?.Invoke(minute);
        }

        // Maç sonu
        if (isSimulating)
        {
            currentMatch.homeScore = homeScore;
            currentMatch.awayScore = awayScore;
            currentMatch.playerRating = playerRating;
            currentMatch.playerMinutesPlayed = 90; // Tam maç oynadı varsayımı
            currentMatch.playerGoals = playerGoals;
            currentMatch.isPlayed = true;
            currentMatch.homePossession = currentHomePossession;
            currentMatch.awayPossession = 100 - currentHomePossession;

            AddMatchEvent(90, MatchEventType.MatchEnd, "", "", "Maç sona erdi.");

            if (CommentatorSystem.Instance != null)
            {
                string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.MatchEnd);
                OnCommentaryChanged?.Invoke(commentary);
            }

            // Oyuncu istatistiklerini güncelle
            if (playerProfile != null)
            {
                playerProfile.careerMatches++;
                playerProfile.careerGoals += playerGoals;
                playerProfile.careerAssists += playerAssists;
                // Ortalama rating güncellemesi
                float totalRating = playerProfile.careerAverageRating * (playerProfile.careerMatches - 1) + playerRating;
                playerProfile.careerAverageRating = totalRating / playerProfile.careerMatches;
            }

            // Fixture listesindeki maçı güncelle
            if (saveData.seasonData != null && saveData.seasonData.fixtures != null)
            {
                foreach (var fixture in saveData.seasonData.fixtures)
                {
                    // Maçı tarih ve takım isimlerine göre bul
                    if (fixture.homeTeam == currentMatch.homeTeam && 
                        fixture.awayTeam == currentMatch.awayTeam &&
                        fixture.matchDate.Date == currentMatch.matchDate.Date)
                    {
                        // Maç sonuçlarını kopyala
                        fixture.homeScore = currentMatch.homeScore;
                        fixture.awayScore = currentMatch.awayScore;
                        fixture.playerRating = currentMatch.playerRating;
                        fixture.playerMinutesPlayed = currentMatch.playerMinutesPlayed;
                        fixture.playerGoals = currentMatch.playerGoals;
                        fixture.playerAssists = currentMatch.playerAssists;
                        fixture.isPlayed = true;
                        fixture.homePossession = currentMatch.homePossession;
                        fixture.awayPossession = currentMatch.awayPossession;
                        fixture.matchEvents = new List<MatchEvent>(currentMatch.matchEvents);
                        break;
                    }
                }
            }

            // Kaydı güncelle
            saveData.UpdateSaveDate();
            SaveSystem.SaveGame(saveData, GameManager.Instance.CurrentSaveSlotIndex);

            isSimulating = false;
            OnMatchFinished?.Invoke();
            Debug.Log($"[MatchSimulationSystem] Match finished: {homeScore}-{awayScore}");
        }
    }

    /// <summary>
    /// Maçı duraklat/devam ettir
    /// </summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : simulationSpeed;
        Debug.Log($"[MatchSimulationSystem] Match paused: {paused}");
    }

    /// <summary>
    /// Simülasyon hızını ayarla
    /// </summary>
    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = Mathf.Clamp(speed, 0.1f, 5f);
        if (!isPaused)
        {
            Time.timeScale = simulationSpeed;
        }
        Debug.Log($"[MatchSimulationSystem] Simulation speed set to {simulationSpeed}x");
    }

    /// <summary>
    /// Maçı simüle et (hızlı, UI olmadan)
    /// </summary>
    public void SimulateMatchInstant(MatchData match)
    {
        if (match == null)
        {
            Debug.LogError("[MatchSimulationSystem] Cannot simulate match: match data is null!");
            return;
        }

        SaveData saveData = GameManager.Instance?.CurrentSave;
        if (saveData == null) return;

        string playerTeamName = saveData.clubData.clubName;
        bool isPlayerHome = match.homeTeam == playerTeamName;

        // Takım güçleri
        int homeTeamPower = GetTeamPower(match.homeTeam);
        int awayTeamPower = GetTeamPower(match.awayTeam);

        // Skorları hesapla (basit algoritma)
        int homeScore = 0;
        int awayScore = 0;

        // İlk yarı
        for (int i = 0; i < 45; i++)
        {
            if (Random.Range(0, 100) < 15) // %15 şans gol fırsatı (artırıldı)
            {
                bool isHomeAttack = Random.Range(0, 100) < (50 + (homeTeamPower - awayTeamPower) / 2);
                int powerDifference = isHomeAttack ? (homeTeamPower - awayTeamPower) : (awayTeamPower - homeTeamPower);
                int goalChance = Mathf.Clamp(40 + powerDifference + Random.Range(-15, 26), 0, 100); // Eşik düşürüldü

                if (goalChance > 35) // Gol! (eşik 35'e düşürüldü)
                {
                    if (isHomeAttack) homeScore++;
                    else awayScore++;
                }
            }
        }

        // İkinci yarı
        for (int i = 0; i < 45; i++)
        {
            if (Random.Range(0, 100) < 15) // %15 şans gol fırsatı (artırıldı)
            {
                bool isHomeAttack = Random.Range(0, 100) < (50 + (homeTeamPower - awayTeamPower) / 2);
                int powerDifference = isHomeAttack ? (homeTeamPower - awayTeamPower) : (awayTeamPower - homeTeamPower);
                int goalChance = Mathf.Clamp(40 + powerDifference + Random.Range(-15, 26), 0, 100); // Eşik düşürüldü

                if (goalChance > 35) // Gol! (eşik 35'e düşürüldü)
                {
                    if (isHomeAttack) homeScore++;
                    else awayScore++;
                }
            }
        }

        // Sonuçları kaydet
        match.homeScore = homeScore;
        match.awayScore = awayScore;
        match.isPlayed = true;
        match.playerRating = Random.Range(6.0f, 8.5f);
        match.playerMinutesPlayed = 90;
        match.homePossession = 50 + (homeTeamPower - awayTeamPower) / 2;
        match.awayPossession = 100 - match.homePossession;

        // Fixture listesindeki maçı güncelle (saveData zaten yukarıda tanımlı)
        if (saveData != null && saveData.seasonData != null && saveData.seasonData.fixtures != null)
        {
            foreach (var fixture in saveData.seasonData.fixtures)
            {
                if (fixture.homeTeam == match.homeTeam && 
                    fixture.awayTeam == match.awayTeam &&
                    fixture.matchDate.Date == match.matchDate.Date)
                {
                    fixture.homeScore = match.homeScore;
                    fixture.awayScore = match.awayScore;
                    fixture.playerRating = match.playerRating;
                    fixture.playerMinutesPlayed = match.playerMinutesPlayed;
                    fixture.isPlayed = true;
                    fixture.homePossession = match.homePossession;
                    fixture.awayPossession = match.awayPossession;
                    break;
                }
            }
            
            // Kaydı güncelle
            saveData.UpdateSaveDate();
            SaveSystem.SaveGame(saveData, GameManager.Instance.CurrentSaveSlotIndex);
        }
    }

    /// <summary>
    /// Maç olayı ekle
    /// </summary>
    private void AddMatchEvent(int minute, MatchEventType eventType, string teamName, string playerName, string description)
    {
        MatchEvent matchEvent = new MatchEvent
        {
            minute = minute,
            eventType = eventType,
            teamName = teamName,
            playerName = playerName,
            description = description
        };

        currentMatch.matchEvents.Add(matchEvent);
        OnMatchEvent?.Invoke(matchEvent);
    }

    /// <summary>
    /// Takım gücünü al
    /// </summary>
    private int GetTeamPower(string teamName)
    {
        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
            return 50; // Varsayılan güç

        TeamData team = DataPackManager.Instance.activeDataPack.GetTeamByName(teamName);
        if (team != null)
        {
            team.CalculateTeamPower();
            return team.GetTeamPower();
        }

        return 50; // Varsayılan
    }

    /// <summary>
    /// Rastgele oyuncu adı al (takımdan)
    /// </summary>
    private string GetRandomPlayerName(string teamName)
    {
        if (DataPackManager.Instance == null || DataPackManager.Instance.activeDataPack == null)
            return "Oyuncu";

        TeamData team = DataPackManager.Instance.activeDataPack.GetTeamByName(teamName);
        if (team != null && team.players != null && team.players.Count > 0)
        {
            int randomIndex = Random.Range(0, team.players.Count);
            return team.players[randomIndex].playerName;
        }

        return "Oyuncu";
    }

    /// <summary>
    /// Pozisyon oluştur (oyuncu için)
    /// </summary>
    private MatchChanceData CreatePlayerChance(int minute, bool isHomeAttack, int homeTeamPower, int awayTeamPower)
    {
        MatchChanceData chance = new MatchChanceData
        {
            minute = minute,
            chanceType = GetRandomChanceType(),
            description = GetChanceDescription(GetRandomChanceType()),
            successChance = CalculateSuccessChance(homeTeamPower, awayTeamPower, isHomeAttack),
            result = MatchChanceResult.Pending
        };

        return chance;
    }

    /// <summary>
    /// Rastgele pozisyon türü seç
    /// </summary>
    private MatchChanceType GetRandomChanceType()
    {
        MatchChanceType[] types = {
            MatchChanceType.Shot,
            MatchChanceType.Pass,
            MatchChanceType.Dribble,
            MatchChanceType.Cross,
            MatchChanceType.OneOnOne
        };
        return types[Random.Range(0, types.Length)];
    }

    /// <summary>
    /// Pozisyon açıklaması al
    /// </summary>
    private string GetChanceDescription(MatchChanceType type)
    {
        switch (type)
        {
            case MatchChanceType.Shot:
                return "Gol şansınız var! Şut atabilirsiniz.";
            case MatchChanceType.Pass:
                return "Takım arkadaşınıza pas verebilirsiniz.";
            case MatchChanceType.Dribble:
                return "Rakibi çalımlayarak ilerleyebilirsiniz.";
            case MatchChanceType.Cross:
                return "Orta açma şansınız var.";
            case MatchChanceType.OneOnOne:
                return "Kaleciyle karşı karşıyasınız!";
            default:
                return "Pozisyon oluştu!";
        }
    }

    /// <summary>
    /// Başarı ihtimalini hesapla
    /// </summary>
    private float CalculateSuccessChance(int homeTeamPower, int awayTeamPower, bool isHomeAttack)
    {
        int attackPower = isHomeAttack ? homeTeamPower : awayTeamPower;
        int defensePower = isHomeAttack ? awayTeamPower : homeTeamPower;
        int powerDifference = attackPower - defensePower;
        
        // 0.3 (30%) ile 0.8 (80%) arası başarı şansı
        float baseChance = 0.3f + (powerDifference / 100f);
        return Mathf.Clamp(baseChance, 0.3f, 0.8f);
    }

    /// <summary>
    /// Pozisyon kararı gelene kadar bekle
    /// </summary>
    private IEnumerator WaitForChanceDecision(MatchChanceData chance)
    {
        while (chance.result == MatchChanceResult.Pending)
        {
            yield return null; // Bir frame bekle
        }
    }

    /// <summary>
    /// Pozisyon sonucunu al (2D sahne tamamlandıktan sonra çağrılır)
    /// </summary>
    public void OnChanceSceneFinished(MatchChanceData chance, MatchChanceResult result)
    {
        if (pendingChance == chance && chance.result == MatchChanceResult.Pending)
        {
            chance.result = result;
            Debug.Log($"[MatchSimulationSystem] Chance scene finished with result: {result}");
        }
    }


    /// <summary>
    /// Pozisyon sonucunu işle ve skor/istatistikleri güncelle
    /// </summary>
    private void ProcessChanceResult(MatchChanceData chance, ref int homeScore, ref int awayScore, 
        ref int playerGoals, ref float playerRating, bool isHomeAttack, MatchData match,
        System.Action<int, int> OnScoreChanged, System.Action<string> OnCommentaryChanged)
    {
        SaveData saveData = GameManager.Instance?.CurrentSave;
        string playerTeamName = saveData?.clubData?.clubName ?? "";
        bool isPlayerHome = match.homeTeam == playerTeamName;

        switch (chance.result)
        {
            case MatchChanceResult.Goal:
                if (isHomeAttack)
                    homeScore++;
                else
                    awayScore++;

                // Oyuncu gol attı
                playerGoals++;
                playerRating += 0.5f; // Gol bonusu

                AddMatchEvent(chance.minute, MatchEventType.Goal, 
                    isHomeAttack ? match.homeTeam : match.awayTeam, 
                    saveData?.playerProfile?.playerName ?? "Oyuncu", 
                    $"{(isHomeAttack ? match.homeTeam : match.awayTeam)} gol attı!");

                OnScoreChanged?.Invoke(homeScore, awayScore);

                if (CommentatorSystem.Instance != null)
                {
                    string teamName = isHomeAttack ? match.homeTeam : match.awayTeam;
                    string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.Goal, 
                        saveData?.playerProfile?.playerName ?? "", teamName);
                    OnCommentaryChanged?.Invoke(commentary);
                }
                break;

            case MatchChanceResult.Assist:
                playerRating += 0.2f; // Asist bonusu
                // Asist olayı eklenebilir
                break;

            case MatchChanceResult.Save:
                if (CommentatorSystem.Instance != null)
                {
                    string commentary = CommentatorSystem.Instance.GenerateCommentary(CommentTrigger.Save);
                    OnCommentaryChanged?.Invoke(commentary);
                }
                break;

            case MatchChanceResult.Miss:
            case MatchChanceResult.Blocked:
                AddMatchEvent(chance.minute, MatchEventType.ChanceMissed, 
                    isHomeAttack ? match.homeTeam : match.awayTeam, 
                    "", "Fırsat kaçtı!");
                break;
        }
    }

    /// <summary>
    /// Maçı durdur
    /// </summary>
    public void StopMatch()
    {
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
            simulationCoroutine = null;
        }
        isSimulating = false;
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("[MatchSimulationSystem] Match stopped");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
