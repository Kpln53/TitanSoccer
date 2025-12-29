using UnityEngine;

/// <summary>
/// Pas sistemi - Takım arkadaşına 1 tık = pas
/// </summary>
public class PassSystem : MonoBehaviour
{
    [Header("Pas Ayarları")]
    [SerializeField] private float passSpeed = 12f;

    private PlayerController player;
    private BallController ball;

    /// <summary>
    /// Pas at (takım arkadaşına)
    /// </summary>
    public void PassToTeammate(PlayerController teammate)
    {
        if (player == null || !player.HasBall || ball == null || teammate == null)
            return;

        if (teammate == player)
        {
            Debug.LogWarning("[PassSystem] Kendine pas atılamaz!");
            return;
        }

        // Pas yönü ve hedef
        Vector2 passDirection = (teammate.Position - player.Position).normalized;
        Vector2 passStartPos = player.Position + passDirection * 0.5f;

        // Topu fırlat
        ball.SetPosition(passStartPos);
        ball.Launch(passDirection, passSpeed);

        // Zamanı normale döndür
        if (TimeFlowManager.Instance != null)
        {
            TimeFlowManager.Instance.NormalizeTime();
        }

        Debug.Log($"[PassSystem] Pas atıldı: {teammate.name}");

        // Event tetikle (pas animasyonu bitince sonuç belirlenecek)
        OnPassExecuted?.Invoke(teammate);
    }

    /// <summary>
    /// Pas animasyonu bitti mi? (top durdu mu?)
    /// </summary>
    public bool IsPassAnimationComplete()
    {
        return ball != null && ball.IsStopped();
    }

    /// <summary>
    /// Referansları ayarla
    /// </summary>
    public void SetReferences(PlayerController playerController, BallController ballController)
    {
        player = playerController;
        ball = ballController;
    }

    // Event
    public System.Action<PlayerController> OnPassExecuted; // teammate
}
