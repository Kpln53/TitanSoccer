using UnityEngine;
using System;

/// <summary>
/// Possession Manager - Top sahipliği yönetimi (Single source of truth)
/// </summary>
public class PossessionManager : MonoBehaviour
{
    public static PossessionManager Instance { get; private set; }

    public enum BallState
    {
        Controlled,    // Top bir oyuncuda
        InFlight,      // Top havada (pas/şut)
        Loose          // Top serbest
    }

    public enum Team
    {
        Home,
        Away
    }

    public BallState CurrentBallState { get; private set; }
    public Team OwnerTeam { get; private set; }
    public GameObject OwnerPlayer { get; private set; }

    public event Action<Team, GameObject> OnPossessionChanged;
    public event Action<BallState> OnBallStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentBallState = BallState.Loose;
        OwnerTeam = Team.Home;
        OwnerPlayer = null;
    }

    /// <summary>
    /// Possession ayarla
    /// </summary>
    public void SetPossession(Team team, GameObject player, BallState state = BallState.Controlled)
    {
        Team oldTeam = OwnerTeam;
        GameObject oldPlayer = OwnerPlayer;
        BallState oldState = CurrentBallState;

        OwnerTeam = team;
        OwnerPlayer = player;
        CurrentBallState = state;

        if (oldState != state)
        {
            OnBallStateChanged?.Invoke(state);
        }

        if (oldTeam != team || oldPlayer != player)
        {
            OnPossessionChanged?.Invoke(team, player);
        }

        Debug.Log($"[PossessionManager] Possession: {team}, Player: {(player != null ? player.name : "null")}, State: {state}");
    }

    /// <summary>
    /// Ball state ayarla
    /// </summary>
    public void SetBallState(BallState state)
    {
        if (CurrentBallState != state)
        {
            CurrentBallState = state;
            OnBallStateChanged?.Invoke(state);
        }
    }

    /// <summary>
    /// Topu serbest bırak
    /// </summary>
    public void ReleaseBall()
    {
        SetPossession(OwnerTeam, null, BallState.Loose);
    }

    /// <summary>
    /// Takım topa sahip mi?
    /// </summary>
    public bool IsTeamInPossession(Team team)
    {
        return CurrentBallState != BallState.Loose && OwnerTeam == team;
    }

    /// <summary>
    /// Controlled state'e ayarla (kısa versiyon)
    /// </summary>
    public void SetControlled(Team team, GameObject player)
    {
        SetPossession(team, player, BallState.Controlled);
    }

    /// <summary>
    /// InFlight state'e ayarla (pas sırasında)
    /// </summary>
    public void SetInFlight(Team team, GameObject from, GameObject to)
    {
        SetPossession(team, null, BallState.InFlight);
    }

    /// <summary>
    /// Loose state'e ayarla
    /// </summary>
    public void SetLoose()
    {
        SetPossession(OwnerTeam, null, BallState.Loose);
    }
}

