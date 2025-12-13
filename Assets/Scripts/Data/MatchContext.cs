using UnityEngine;

public enum ChanceOutcome
{
    None,
    Goal,
    Assist,
    ShotMiss,
    Turnover
}

public class MatchContext : MonoBehaviour
{
    public static MatchContext I { get; private set; }

    [Header("Match State")]
    public float minute;
    public int homeScore;
    public int awayScore;
    public float homePossession;
    public float energy01 = 1f;
    public float morale01 = 1f;
    public float rating = 6.0f;

    [Header("Chance Result")]
    public bool hasPendingResult;
    public ChanceOutcome lastOutcome = ChanceOutcome.None;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetOutcome(ChanceOutcome outcome)
    {
        lastOutcome = outcome;
        hasPendingResult = true;
    }

    public void ClearOutcome()
    {
        lastOutcome = ChanceOutcome.None;
        hasPendingResult = false;
    }
}
