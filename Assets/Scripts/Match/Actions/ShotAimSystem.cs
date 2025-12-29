using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Score Hero tarzı şut çizgi sistemi
/// Oyuncuya basılı tut → Çizgi çiz → Bırak → Şut
/// </summary>
public class ShotAimSystem : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private PlayerController player;
    [SerializeField] private BallController ball;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private LineRenderer trajectoryLine;

    [Header("Şut Ayarları")]
    [SerializeField] private float minShotPower = 8f;
    [SerializeField] private float maxShotPower = 30f;
    [SerializeField] private float maxLineLength = 10f;
    [SerializeField] private float shotPowerMultiplier = 10f;

    [Header("Görsel Ayarlar")]
    [SerializeField] private Color aimLineColor = Color.white;
    [SerializeField] private Color trajectoryColor = Color.yellow;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private int trajectoryPoints = 30;

    // State
    private bool isAiming = false;
    private Vector2 aimStartPos; // Oyuncu pozisyonu
    private Vector2 currentAimPos; // Çizilen yön

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        SetupLines();
    }

    /// <summary>
    /// Çizgi renderer'ları ayarla
    /// </summary>
    void SetupLines()
    {
        // Aim çizgisi (ana çizgi)
        if (aimLine == null)
        {
            GameObject lineObj = new GameObject("AimLine");
            lineObj.transform.SetParent(transform);
            aimLine = lineObj.AddComponent<LineRenderer>();
        }

        aimLine.positionCount = 2;
        aimLine.startWidth = lineWidth;
        aimLine.endWidth = lineWidth * 0.5f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.sortingOrder = 100;
        aimLine.enabled = false;

        // Trajectory çizgisi (yörünge)
        if (trajectoryLine == null)
        {
            GameObject trajObj = new GameObject("TrajectoryLine");
            trajObj.transform.SetParent(transform);
            trajectoryLine = trajObj.AddComponent<LineRenderer>();
        }

        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.startWidth = lineWidth * 0.4f;
        trajectoryLine.endWidth = lineWidth * 0.1f;
        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        trajectoryLine.sortingOrder = 99;
        trajectoryLine.enabled = false;
    }

    /// <summary>
    /// Şut modunu başlat (oyuncuya basılı tutulduğunda)
    /// </summary>
    public void StartAiming(PlayerController targetPlayer)
    {
        if (!targetPlayer.HasBall) 
        {
            Debug.Log("[ShotAimSystem] Oyuncunun topu yok, şut modu başlatılamadı.");
            return;
        }
        
        // Ball referansını bul
        if (ball == null)
        {
            ball = FindObjectOfType<BallController>();
        }

        player = targetPlayer;
        aimStartPos = player.Position;
        isAiming = true;
        
        // Oyuncu hareketini durdur
        player.Stop();

        aimLine.enabled = true;
        trajectoryLine.enabled = true;

        Debug.Log("[ShotAimSystem] Şut modu başladı! Oyuncu hareketi durduruldu.");
    }

    /// <summary>
    /// Şut çizgisini güncelle (parmak sürüklenirken)
    /// </summary>
    public void UpdateAiming(Vector2 screenPos)
    {
        if (!isAiming || player == null) return;

        // Screen to world dönüşümü (Z değeri ile)
        float zDistance = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 screenPos3D = new Vector3(screenPos.x, screenPos.y, zDistance);
        Vector3 worldPos3D = mainCamera.ScreenToWorldPoint(screenPos3D);
        Vector2 worldPos = new Vector2(worldPos3D.x, worldPos3D.y);
        
        // Oyuncu pozisyonunu güncelle (hareket edebilir)
        aimStartPos = player.Position;
        currentAimPos = worldPos;

        // Çizgi: Oyuncudan → Parmak pozisyonuna
        Vector2 direction = (currentAimPos - aimStartPos);
        float lineLength = Mathf.Min(direction.magnitude, maxLineLength);
        
        // Yönü normalize et (sıfır değilse)
        if (direction.magnitude > 0.01f)
        {
            direction = direction.normalized;
        }
        else
        {
            direction = Vector2.right; // Varsayılan yön
            lineLength = 0f;
        }

        // Aim çizgisi - Oyuncudan parmağa
        aimLine.SetPosition(0, aimStartPos);
        aimLine.SetPosition(1, aimStartPos + direction * lineLength);
        aimLine.startColor = aimLineColor;
        aimLine.endColor = new Color(aimLineColor.r, aimLineColor.g, aimLineColor.b, 0.5f);

        // Trajectory çizgisi (yörünge)
        if (lineLength > 0.1f)
        {
            UpdateTrajectory(aimStartPos, direction, lineLength);
        }
        else
        {
            trajectoryLine.enabled = false;
        }
    }

    /// <summary>
    /// Yörünge çizgisini güncelle
    /// </summary>
    void UpdateTrajectory(Vector2 startPos, Vector2 direction, float lineLength)
    {
        float powerRatio = lineLength / maxLineLength;
        float shotPower = (powerRatio * maxShotPower * shotPowerMultiplier);

        // Fizik simülasyonu
        float friction = 0.98f;
        float timeStep = 0.1f;
        Vector2 velocity = direction * shotPower;
        Vector2 position = startPos;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            trajectoryLine.SetPosition(i, position);

            velocity *= friction;
            position += velocity * timeStep;

            if (velocity.magnitude < 0.5f) break;
        }

        trajectoryLine.startColor = trajectoryColor;
        trajectoryLine.endColor = new Color(trajectoryColor.r, trajectoryColor.g, trajectoryColor.b, 0.2f);
    }

    /// <summary>
    /// Şut at (parmak bırakıldığında)
    /// </summary>
    public void ExecuteShot()
    {
        if (!isAiming || player == null || ball == null) return;

        // Oyuncunun son pozisyonunu al
        aimStartPos = player.Position;
        
        Vector2 direction = (currentAimPos - aimStartPos);
        float lineLength = Mathf.Min(direction.magnitude, maxLineLength);
        
        // Top kontrolü - Top oyuncunun yakınında olmalı
        Vector2 ballPos = ball.transform.position;
        float distanceToBall = Vector2.Distance(aimStartPos, ballPos);
        
        if (distanceToBall > 1.5f)
        {
            Debug.Log($"[ShotAimSystem] Top çok uzakta ({distanceToBall:F2}), şut iptal edildi.");
            CancelAiming();
            return;
        }
        
        if (direction.magnitude < 0.1f)
        {
            direction = Vector2.right;
            lineLength = 0f;
        }
        else
        {
            direction = direction.normalized;
        }
        
        float powerRatio = lineLength / maxLineLength;
        float shotPower = Mathf.Lerp(minShotPower, maxShotPower, powerRatio) * shotPowerMultiplier;

        // Minimum güç kontrolü
        if (shotPower < minShotPower)
        {
            Debug.Log("[ShotAimSystem] Şut çok zayıf, iptal edildi.");
            CancelAiming();
            return;
        }

        // Topu oyuncunun önüne yerleştir ve fırlat
        Vector2 shotStartPos = aimStartPos + direction * 0.5f;
        ball.SetPosition(shotStartPos);
        ball.Launch(direction, shotPower);

        // Zamanı normale döndür
        if (TimeFlowManager.Instance != null)
        {
            TimeFlowManager.Instance.NormalizeTime();
        }

        // Çizgileri kapat
        aimLine.enabled = false;
        trajectoryLine.enabled = false;
        isAiming = false;

        Debug.Log($"[ShotAimSystem] Şut atıldı! Güç: {shotPower:F1}, Yön: {direction}");

        // Event tetikle (MatchChanceSceneManager dinleyecek)
        OnShotExecuted?.Invoke(direction, shotPower);
    }

    /// <summary>
    /// Şut modunu iptal et
    /// </summary>
    public void CancelAiming()
    {
        isAiming = false;
        aimLine.enabled = false;
        trajectoryLine.enabled = false;
    }

    // Event
    public System.Action<Vector2, float> OnShotExecuted; // direction, power

    // Getters
    public bool IsAiming => isAiming;
}
