using UnityEngine;

/// <summary>
/// AI durumları - State Machine pattern
/// </summary>
public enum AIState
{
    Idle,                  // Beklemede
    ChasingBall,           // Topu kovalıyor
    ReturningToPosition,   // Pozisyonuna dönüyor
    Defending,             // Savunma yapıyor
    Attacking,             // Hücum ediyor
    Supporting             // Destek veriyor
}

/// <summary>
/// AI durum verisi
/// </summary>
[System.Serializable]
public class AIStateData
{
    public AIState currentState = AIState.Idle;
    public float stateTimer = 0f;
    public Vector2 targetPosition;
    public Transform targetObject; // Top veya rakip oyuncu
}

