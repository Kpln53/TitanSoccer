using UnityEngine;

/// <summary>
/// Pass Input Controller - Tap-to-pass + Click-to-move (birleşik)
/// Teammate varsa pas, yoksa click-to-move
/// </summary>
public class PassInputController : MonoBehaviour
{
    [Header("Settings")]
    public float clickRange = 1.5f;      // Teammate tıklama menzili
    public float maxPassRange = 8f;      // Maksimum pas mesafesi
    public float moveSpeed = 5f;         // Click-to-move hızı

    private PossessionManager possessionManager;
    private BallController ballController;
    private PlayPhaseManager phaseManager;
    private GameObject controlledPlayer;

    // Click-to-move
    private Vector3 moveTarget;
    private bool isMoving;

    private void Start()
    {
        possessionManager = PossessionManager.Instance;
        ballController = FindObjectOfType<BallController>();
        phaseManager = PlayPhaseManager.Instance;
        controlledPlayer = FindControlledPlayer();
    }

    private void Update()
    {
        // Sadece Attack phase ve top bizdeyken
        if (phaseManager == null || !phaseManager.IsPhaseActive) return;
        if (phaseManager.CurrentPhase != PlayPhaseManager.PhaseType.AttackChance) return;
        if (possessionManager == null || !possessionManager.IsTeamInPossession(PossessionManager.Team.Home)) return;
        if (possessionManager.CurrentBallState != PossessionManager.BallState.Controlled) return;

        // Controlled player kontrolü
        if (controlledPlayer == null)
        {
            controlledPlayer = FindControlledPlayer();
            if (controlledPlayer == null) return;
        }

        // Mouse input
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;

            // Önce teammate ara (pas öncelikli)
            GameObject teammate = FindNearestTeammate(mouseWorld);

            if (teammate != null)
            {
                // PAS YAP
                PassToTeammate(teammate);
                isMoving = false;
            }
            else
            {
                // CLICK-TO-MOVE
                moveTarget = mouseWorld;
                isMoving = true;
                Debug.Log($"[PassInput] Move to: {moveTarget}");
            }
        }

        // Click-to-move hareketi
        if (isMoving && controlledPlayer != null)
        {
            Vector3 direction = moveTarget - controlledPlayer.transform.position;
            float distance = direction.magnitude;

            if (distance > 0.1f)
            {
                Rigidbody2D rb = controlledPlayer.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction.normalized * moveSpeed;
                }
                else
                {
                    controlledPlayer.transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                }
            }
            else
            {
                isMoving = false;
                Rigidbody2D rb = controlledPlayer.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private GameObject FindControlledPlayer()
    {
        PlayerAgent[] players = FindObjectsOfType<PlayerAgent>();
        foreach (var player in players)
        {
            if (player.IsHomeTeam && player.gameObject.name.Contains("Controlled"))
            {
                return player.gameObject;
            }
        }

        // Fallback: ilk home player
        foreach (var player in players)
        {
            if (player.IsHomeTeam) return player.gameObject;
        }

        return null;
    }

    private GameObject FindNearestTeammate(Vector3 mousePos)
    {
        PlayerAgent[] players = FindObjectsOfType<PlayerAgent>();
        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var player in players)
        {
            if (!player.IsHomeTeam) continue; // Sadece takım arkadaşları
            if (player.gameObject == controlledPlayer) continue; // Kendisi değil

            float dist = Vector3.Distance(mousePos, player.transform.position);

            // Click range içinde VE pas range içinde
            if (dist <= clickRange)
            {
                float passDist = Vector3.Distance(controlledPlayer.transform.position, player.transform.position);
                if (passDist <= maxPassRange && dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = player.gameObject;
                }
            }
        }

        return nearest;
    }

    private void PassToTeammate(GameObject teammate)
    {
        if (teammate == null || ballController == null) return;

        GameObject passer = possessionManager.OwnerPlayer ?? controlledPlayer;
        if (passer == null) return;

        PlayerAgent passerAgent = passer.GetComponent<PlayerAgent>();
        if (passerAgent == null) return;

        // Pas başarı şansı
        float distance = Vector3.Distance(passer.transform.position, teammate.transform.position);
        float distFactor = 1f - (distance / maxPassRange) * 0.5f;
        float passSuccess = (passerAgent.GetEffectiveStat(passerAgent.passAccuracy) / 100f) * distFactor;

        // Intercept kontrolü (basit)
        bool intercepted = CheckInterception(passer.transform.position, teammate.transform.position);

        if (intercepted || Random.Range(0f, 1f) > passSuccess)
        {
            // Pas başarısız -> rakip yakalar
            GameObject interceptor = FindNearestOpponent((passer.transform.position + teammate.transform.position) / 2f);
            if (interceptor != null && possessionManager != null)
            {
                possessionManager.SetControlled(PossessionManager.Team.Away, interceptor);
            }
        }
        else
        {
            // Pas başarılı
            float passSpeed = 5f + (passerAgent.GetEffectiveStat(passerAgent.passAccuracy) / 100f) * 3f;
            ballController.PassTo(teammate, PossessionManager.Team.Home, passSpeed);
            Debug.Log($"[PassInput] Pass to {teammate.name}");
        }
    }

    private bool CheckInterception(Vector3 from, Vector3 to)
    {
        PlayerAgent[] players = FindObjectsOfType<PlayerAgent>();
        Vector3 passLine = (to - from).normalized;
        float passDistance = Vector3.Distance(from, to);

        foreach (var player in players)
        {
            if (player.IsHomeTeam) continue; // Sadece rakipler

            Vector3 toPlayer = player.transform.position - from;
            float dot = Vector3.Dot(toPlayer.normalized, passLine);
            float distToLine = Vector3.Distance(player.transform.position, from + passLine * Vector3.Dot(toPlayer, passLine));

            if (dot > 0 && dot < passDistance && distToLine < 1.5f)
            {
                float interceptChance = player.GetEffectiveStat(player.reaction) / 200f;
                if (Random.Range(0f, 1f) < interceptChance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private GameObject FindNearestOpponent(Vector3 pos)
    {
        PlayerAgent[] players = FindObjectsOfType<PlayerAgent>();
        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var player in players)
        {
            if (player.IsHomeTeam) continue;

            float dist = Vector3.Distance(pos, player.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = player.gameObject;
            }
        }

        return nearest;
    }
}

