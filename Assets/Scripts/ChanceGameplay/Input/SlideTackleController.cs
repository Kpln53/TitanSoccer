using UnityEngine;

/// <summary>
/// Slide Tackle Controller - Drag input ile slide tackle (savunma)
/// </summary>
public class SlideTackleController : MonoBehaviour
{
    [Header("Settings")]
    public float clickRange = 1.0f;
    public float minDragDistance = 0.5f;
    public float slideSpeed = 8f;
    public float slideDuration = 0.5f;

    private PossessionManager possessionManager;
    private PlayPhaseManager phaseManager;

    private bool isDragging;
    private Vector3 dragStartPos;
    private GameObject draggedPlayer;

    private void Start()
    {
        possessionManager = PossessionManager.Instance;
        phaseManager = PlayPhaseManager.Instance;
    }

    private void Update()
    {
        // Sadece Defense phase'de
        if (phaseManager == null || !phaseManager.IsPhaseActive) return;
        if (phaseManager.CurrentPhase != PlayPhaseManager.PhaseType.DefenseChance) return;
        if (possessionManager == null || !possessionManager.IsTeamInPossession(PossessionManager.Team.Away)) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            // Bizim oyuncularımızdan birine tıkladık mı?
            GameObject player = FindPlayerAtPosition(mouseWorld, true);
            if (player != null)
            {
                isDragging = true;
                dragStartPos = mouseWorld;
                draggedPlayer = player;
            }
        }
        else if (Input.GetMouseButton(0) && isDragging && draggedPlayer != null)
        {
            // Drag sırasında slide direction gösterimi (opsiyonel)
            Vector3 slideDir = mouseWorld - dragStartPos;
            if (slideDir.magnitude > minDragDistance)
            {
                // Visual feedback eklenebilir
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDragging && draggedPlayer != null)
            {
                Vector3 slideDir = mouseWorld - dragStartPos;
                if (slideDir.magnitude > minDragDistance)
                {
                    AttemptSlideTackle(draggedPlayer, slideDir);
                }

                isDragging = false;
                draggedPlayer = null;
            }
        }
    }

    private GameObject FindPlayerAtPosition(Vector3 pos, bool isHomeTeam)
    {
        PlayerAgent[] players = FindObjectsOfType<PlayerAgent>();
        foreach (var player in players)
        {
            if (player.IsHomeTeam == isHomeTeam)
            {
                float dist = Vector3.Distance(pos, player.transform.position);
                if (dist <= clickRange)
                {
                    return player.gameObject;
                }
            }
        }
        return null;
    }

    private void AttemptSlideTackle(GameObject tackler, Vector3 direction)
    {
        if (tackler == null || possessionManager.OwnerPlayer == null) return;

        PlayerAgent tacklerAgent = tackler.GetComponent<PlayerAgent>();
        PlayerAgent ballOwnerAgent = possessionManager.OwnerPlayer.GetComponent<PlayerAgent>();

        if (tacklerAgent == null || ballOwnerAgent == null) return;

        // Tackle başarı şansı
        float dist = Vector3.Distance(tackler.transform.position, possessionManager.OwnerPlayer.transform.position);
        float distFactor = 1f - (dist / 5f) * 0.5f;
        distFactor = Mathf.Clamp01(distFactor);

        float tackleSuccess = tacklerAgent.GetEffectiveStat(tacklerAgent.tackleSkill) / 100f;
        tackleSuccess *= distFactor;

        float dribbleDefense = ballOwnerAgent.GetEffectiveStat(ballOwnerAgent.control) / 200f;
        tackleSuccess -= dribbleDefense;
        tackleSuccess = Mathf.Clamp01(tackleSuccess);

        if (Random.Range(0f, 1f) < tackleSuccess)
        {
            // Tackle başarılı -> top bize geçer
            possessionManager.SetControlled(PossessionManager.Team.Home, tackler);
            Debug.Log($"[SlideTackle] Success by {tackler.name}");
        }
        else
        {
            Debug.Log($"[SlideTackle] Failed by {tackler.name}");
        }
    }
}

