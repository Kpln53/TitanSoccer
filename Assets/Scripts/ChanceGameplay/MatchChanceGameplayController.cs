using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Match Chance Gameplay Controller - Ana bootstrap (11v11, 2D, daire oyuncular)
/// </summary>
public class MatchChanceGameplayController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ballPrefab;

    [Header("Spawn Settings")]
    public float fieldWidth = 20f;
    public float fieldHeight = 15f;

    [Header("Player Colors")]
    public Color homeColor = Color.blue;
    public Color awayColor = Color.red;
    public Color controlledColor = Color.yellow;

    private PossessionManager possessionManager;
    private PlayPhaseManager phaseManager;
    private SpawnSystem spawnSystem;
    private GameObject ballObject;
    private GameObject controlledPlayer;
    private List<GameObject> allPlayers = new List<GameObject>();

    private void Start()
    {
        SetupSystems();
        CreateBall();
        SpawnPlayers();

        // Input controllers ekle
        gameObject.AddComponent<PassInputController>();
        gameObject.AddComponent<SlideTackleController>();

        // Phase başlat
        if (MatchContext.Instance != null && phaseManager != null)
        {
            phaseManager.StartPhase(MatchContext.Instance.currentChance);
        }
    }

    private void SetupSystems()
    {
        // PossessionManager
        GameObject pmObj = GameObject.Find("PossessionManager");
        if (pmObj == null)
        {
            pmObj = new GameObject("PossessionManager");
            possessionManager = pmObj.AddComponent<PossessionManager>();
        }
        else
        {
            possessionManager = pmObj.GetComponent<PossessionManager>();
        }

        // PlayPhaseManager
        GameObject ppmObj = GameObject.Find("PlayPhaseManager");
        if (ppmObj == null)
        {
            ppmObj = new GameObject("PlayPhaseManager");
            phaseManager = ppmObj.AddComponent<PlayPhaseManager>();
        }
        else
        {
            phaseManager = ppmObj.GetComponent<PlayPhaseManager>();
        }

        // SpawnSystem
        GameObject ssObj = GameObject.Find("SpawnSystem");
        if (ssObj == null)
        {
            ssObj = new GameObject("SpawnSystem");
            spawnSystem = ssObj.AddComponent<SpawnSystem>();
            spawnSystem.fieldWidth = fieldWidth;
            spawnSystem.fieldHeight = fieldHeight;
        }
        else
        {
            spawnSystem = ssObj.GetComponent<SpawnSystem>();
        }
    }

    private void CreateBall()
    {
        if (ballPrefab == null)
        {
            // Basit 2D top oluştur
            ballObject = new GameObject("Ball");
            SpriteRenderer sr = ballObject.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite(16, Color.white);
            sr.sortingOrder = 2;
            ballObject.transform.localScale = Vector3.one * 0.3f;
        }
        else
        {
            ballObject = Instantiate(ballPrefab);
            ballObject.name = "Ball";
        }

        ballObject.AddComponent<BallController>();
    }

    private void SpawnPlayers()
    {
        if (spawnSystem == null || MatchContext.Instance == null) return;

        MatchContext ctx = MatchContext.Instance;
        bool isAttackChance = ctx.currentChance.chanceType == MatchContext.ChanceData.ChanceType.Attack;

        // Formasyonlar
        List<Vector3> homePositions = spawnSystem.GetFormationPositions(true);
        List<Vector3> awayPositions = spawnSystem.GetFormationPositions(false);

        // Home team (mavi/sarı)
        for (int i = 0; i < 11 && i < ctx.homeSquad.Count; i++)
        {
            if (!ctx.homeSquad[i].isStartingXI) continue;

            Vector3 pos = homePositions[i];
            GameObject player = CreateCirclePlayer(pos, true);
            player.name = $"Home Player {i + 1}";

            PlayerAgent agent = player.AddComponent<PlayerAgent>();
            agent.IsHomeTeam = true;
            agent.playerName = ctx.homeSquad[i].playerName;
            agent.overall = ctx.homeSquad[i].overall;
            agent.position = ctx.homeSquad[i].position;

            TeamAIController teamAI = player.AddComponent<TeamAIController>();
            teamAI.CalculateTeamStrengthFactor(ctx.homeTeamPower);
            agent.teamStrengthFactor = teamAI.teamStrengthFactor;

            // Controlled player (ilk veya context'te belirtilen)
            if (i == 0 || ctx.playerName == ctx.homeSquad[i].playerName)
            {
                controlledPlayer = player;
                player.name = "Controlled Player";
                SetPlayerColor(player, controlledColor); // Sarı
            }

            allPlayers.Add(player);
        }

        // Away team (kırmızı)
        for (int i = 0; i < 11 && i < ctx.awaySquad.Count; i++)
        {
            if (!ctx.awaySquad[i].isStartingXI) continue;

            Vector3 pos = awayPositions[i];
            GameObject player = CreateCirclePlayer(pos, false);
            player.name = $"Away Player {i + 1}";

            PlayerAgent agent = player.AddComponent<PlayerAgent>();
            agent.IsHomeTeam = false;
            agent.playerName = ctx.awaySquad[i].playerName;
            agent.overall = ctx.awaySquad[i].overall;
            agent.position = ctx.awaySquad[i].position;

            TeamAIController teamAI = player.AddComponent<TeamAIController>();
            teamAI.CalculateTeamStrengthFactor(ctx.awayTeamPower);
            agent.teamStrengthFactor = teamAI.teamStrengthFactor;

            allPlayers.Add(player);
        }

        // Topu kontrollü oyuncuya ver
        if (controlledPlayer != null && ballObject != null && possessionManager != null)
        {
            BallController ballCtrl = ballObject.GetComponent<BallController>();
            if (ballCtrl != null)
            {
                ballCtrl.transform.position = controlledPlayer.transform.position;
            }

            PossessionManager.Team initialTeam = isAttackChance
                ? PossessionManager.Team.Home
                : PossessionManager.Team.Away;

            GameObject initialOwner = isAttackChance ? controlledPlayer : null; // Defense'da rakip başlar
            if (initialOwner != null)
            {
                possessionManager.SetControlled(initialTeam, initialOwner);
            }

            // Camera follow
            CameraFollowController cam = Camera.main?.GetComponent<CameraFollowController>();
            if (cam == null && Camera.main != null)
            {
                cam = Camera.main.gameObject.AddComponent<CameraFollowController>();
            }
            if (cam != null)
            {
                cam.SetTarget(controlledPlayer);
            }
        }
    }

    private GameObject CreateCirclePlayer(Vector3 position, bool isHomeTeam)
    {
        GameObject player = new GameObject(isHomeTeam ? "Home Player" : "Away Player");
        player.transform.position = position;

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        Color color = isHomeTeam ? homeColor : awayColor;
        sr.sprite = CreateCircleSprite(32, color);
        sr.sortingOrder = 1;

        return player;
    }

    private void SetPlayerColor(GameObject player, Color color)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    /// <summary>
    /// Runtime daire sprite oluştur
    /// </summary>
    private Sprite CreateCircleSprite(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size);
        int radius = size / 2;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= radius ? color : Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

