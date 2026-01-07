using UnityEngine;
using System.Collections.Generic;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Çizgi Çizme Sistemi
    /// Oyuncu üzerinde basılı tutup çizgi çizerek pas/şut yapılır
    /// </summary>
    public class LineDrawer : MonoBehaviour
    {
        public static LineDrawer Instance { get; private set; }

        [Header("Çizgi Ayarları")]
        [SerializeField] private float minDrawDistance = 0.5f;       // Minimum çizgi uzunluğu
        [SerializeField] private float maxDrawDistance = 15f;        // Maximum çizgi uzunluğu
        [SerializeField] private float pointInterval = 0.1f;         // Nokta ekleme aralığı
        [SerializeField] private float playerTouchRadius = 1.2f;     // Oyuncuya dokunma yarıçapı

        [Header("Görsel")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Color lineColor = Color.white;
        [SerializeField] private Color passLineColor = Color.cyan;
        [SerializeField] private Color shootLineColor = Color.red;
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private Material lineMaterial;

        [Header("Durum")]
        [SerializeField] private bool isDrawing = false;
        [SerializeField] private bool isValidStart = false;

        // Çizgi verileri
        private List<Vector2> currentPoints = new List<Vector2>();
        private Vector2 startPoint;
        private float drawStartTime;
        private GameObject targetPlayer;

        // Events
        public event Action<LineData> OnLineCompleted;
        public event Action OnDrawStarted;
        public event Action OnDrawCancelled;

        // Properties
        public bool IsDrawing => isDrawing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupLineRenderer();
        }

        private void SetupLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth * 0.5f; // Biraz incelen
            lineRenderer.positionCount = 0;
            lineRenderer.useWorldSpace = true;

            if (lineMaterial != null)
            {
                lineRenderer.material = lineMaterial;
            }
            else
            {
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.sortingOrder = 10;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Touch veya Mouse input
            if (Input.GetMouseButtonDown(0))
            {
                TryStartDraw(GetWorldMousePosition());
            }
            else if (Input.GetMouseButton(0) && isDrawing)
            {
                ContinueDraw(GetWorldMousePosition());
            }
            else if (Input.GetMouseButtonUp(0) && isDrawing)
            {
                EndDraw(GetWorldMousePosition());
            }
        }

        /// <summary>
        /// Çizgi çizmeye başla
        /// </summary>
        private void TryStartDraw(Vector2 position)
        {
            // Oyuncuya dokunuldu mu kontrol et
            if (ChanceController.Instance?.Player == null) return;
            
            // Top bizde değilse çizgi çizme!
            if (!ChanceController.Instance.Player.HasBall)
            {
                Debug.Log("[LineDrawer] Cannot draw - player doesn't have the ball!");
                return;
            }

            Vector2 playerPos = ChanceController.Instance.Player.transform.position;
            float distance = Vector2.Distance(position, playerPos);

            if (distance <= playerTouchRadius)
            {
                // Oyuncu üzerinde başladı - çizgi çizmeye başla
                isDrawing = true;
                isValidStart = true;
                startPoint = playerPos;
                drawStartTime = Time.unscaledTime;
                targetPlayer = null;
                
                currentPoints.Clear();
                currentPoints.Add(playerPos);

                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, new Vector3(playerPos.x, playerPos.y, 0));
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;

                OnDrawStarted?.Invoke();
                Debug.Log("[LineDrawer] Draw started on player");
            }
        }

        /// <summary>
        /// Çizgi çizmeye devam et
        /// </summary>
        private void ContinueDraw(Vector2 position)
        {
            if (!isDrawing || !isValidStart) return;

            // Son noktadan yeterli uzaklıkta mı?
            Vector2 lastPoint = currentPoints[currentPoints.Count - 1];
            float distance = Vector2.Distance(position, lastPoint);

            if (distance >= pointInterval)
            {
                // Maximum uzunluk kontrolü
                float totalLength = GetTotalLength() + distance;
                if (totalLength > maxDrawDistance)
                {
                    // Max'a ulaştı - otomatik bitir
                    EndDraw(position);
                    return;
                }

                currentPoints.Add(position);
                
                lineRenderer.positionCount = currentPoints.Count;
                lineRenderer.SetPosition(currentPoints.Count - 1, new Vector3(position.x, position.y, 0));

                // Hedef kontrolü ve renk güncelleme
                UpdateLineTarget(position);
            }
        }

        /// <summary>
        /// Çizgi hedefini ve rengini güncelle
        /// </summary>
        private void UpdateLineTarget(Vector2 position)
        {
            if (ChanceController.Instance == null) return;

            // Takım arkadaşı kontrolü (pas için)
            targetPlayer = null;
            foreach (var teammate in ChanceController.Instance.Teammates)
            {
                if (teammate == null) continue;

                float dist = Vector2.Distance(position, teammate.transform.position);
                if (dist < 1.5f) // Takım arkadaşına yakın
                {
                    targetPlayer = teammate;
                    lineRenderer.startColor = passLineColor;
                    lineRenderer.endColor = passLineColor;
                    return;
                }
            }

            // Kale bölgesi kontrolü (şut için)
            Vector2 goalPos = ChanceController.Instance.GoalPosition;
            if (ChanceController.Instance.IsInGoalArea(position))
            {
                lineRenderer.startColor = shootLineColor;
                lineRenderer.endColor = shootLineColor;
                return;
            }

            // Normal çizgi rengi
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }

        /// <summary>
        /// Çizgi çizmeyi bitir
        /// </summary>
        private void EndDraw(Vector2 position)
        {
            if (!isDrawing) return;

            isDrawing = false;

            // Son noktayı ekle
            if (currentPoints.Count > 0)
            {
                Vector2 lastPoint = currentPoints[currentPoints.Count - 1];
                if (Vector2.Distance(position, lastPoint) > 0.05f)
                {
                    currentPoints.Add(position);
                }
            }

            // Minimum uzunluk kontrolü
            float totalLength = GetTotalLength();
            if (totalLength < minDrawDistance || currentPoints.Count < 2)
            {
                CancelDraw();
                return;
            }

            // Son hedef kontrolü
            UpdateLineTarget(position);

            // Çizgi verisini oluştur
            LineData lineData = AnalyzeLine();

            // LineRenderer'ı temizle
            ClearLine();

            // Event gönder
            OnLineCompleted?.Invoke(lineData);

            Debug.Log($"[LineDrawer] Line completed: {lineData.detectedAction}, Length: {totalLength:F2}, Curve: {lineData.curvature:F2}");
        }

        /// <summary>
        /// Çizgi çizmeyi iptal et
        /// </summary>
        public void CancelDraw()
        {
            if (!isDrawing && currentPoints.Count == 0) return;

            isDrawing = false;
            isValidStart = false;
            currentPoints.Clear();
            targetPlayer = null;
            ClearLine();

            OnDrawCancelled?.Invoke();
            Debug.Log("[LineDrawer] Draw cancelled");
        }

        /// <summary>
        /// Çizgiyi analiz et
        /// </summary>
        private LineData AnalyzeLine()
        {
            LineData data = new LineData
            {
                startPoint = currentPoints[0],
                endPoint = currentPoints[currentPoints.Count - 1],
                points = new System.Collections.Generic.List<UnityEngine.Vector2>(currentPoints),
                length = GetTotalLength(),
                targetPlayer = targetPlayer,
                targetPosition = currentPoints[currentPoints.Count - 1]
            };

            data.drawSpeed = data.length / Mathf.Max(0.1f, Time.unscaledTime - drawStartTime);

            // Eğrilik hesapla
            data.curvature = CalculateCurvature();

            // Eylem tipi belirle
            data.detectedAction = DetectAction(data);

            return data;
        }

        /// <summary>
        /// Eğrilik hesapla (-1: sol, +1: sağ, 0: düz)
        /// </summary>
        private float CalculateCurvature()
        {
            if (currentPoints.Count < 3) return 0f;

            // Başlangıç ve bitiş arasındaki düz çizgi
            Vector2 start = currentPoints[0];
            Vector2 end = currentPoints[currentPoints.Count - 1];
            Vector2 straightDir = (end - start).normalized;

            if (straightDir == Vector2.zero) return 0f;

            // Her noktanın düz çizgiden sapmasını hesapla
            float totalDeviation = 0f;
            int count = 0;

            for (int i = 1; i < currentPoints.Count - 1; i++)
            {
                Vector2 point = currentPoints[i];
                Vector2 toPoint = point - start;
                
                // Cross product ile sağa/sola sapma
                float cross = straightDir.x * toPoint.y - straightDir.y * toPoint.x;
                totalDeviation += cross;
                count++;
            }

            if (count == 0) return 0f;

            // Normalize et (-1 ile +1 arası)
            float avgDeviation = totalDeviation / count;
            float normalizedCurve = Mathf.Clamp(avgDeviation / 2f, -1f, 1f);

            return normalizedCurve;
        }

        /// <summary>
        /// Eylem tipi belirle
        /// </summary>
        private ActionType DetectAction(LineData data)
        {
            // 1. Hedef takım arkadaşı varsa → PAS
            if (data.targetPlayer != null)
            {
                return ActionType.Pass;
            }

            // 2. Kale bölgesine doğruysa → ŞUT
            if (ChanceController.Instance != null)
            {
                Vector2 goalPos = ChanceController.Instance.GoalPosition;
                Vector2 direction = (data.endPoint - data.startPoint).normalized;
                Vector2 toGoal = (goalPos - data.startPoint).normalized;

                float dotToGoal = Vector2.Dot(direction, toGoal);
                
                // Kaleye doğru mu ve yeterince uzun mu?
                if (dotToGoal > 0.3f && data.length > 2f)
                {
                    // Kale bölgesinde mi bitiyor?
                    if (ChanceController.Instance.IsInGoalArea(data.endPoint))
                    {
                        return ActionType.Shoot;
                    }

                    // Kaleye doğru uzun çizgi de şut sayılsın
                    if (dotToGoal > 0.6f && data.length > 4f)
                    {
                        return ActionType.Shoot;
                    }
                }
            }

            // 3. Aksi halde → HAREKET (dribble)
            return ActionType.Dribble;
        }

        /// <summary>
        /// Toplam çizgi uzunluğu
        /// </summary>
        private float GetTotalLength()
        {
            if (currentPoints.Count < 2) return 0f;

            float length = 0f;
            for (int i = 1; i < currentPoints.Count; i++)
            {
                length += Vector2.Distance(currentPoints[i - 1], currentPoints[i]);
            }
            return length;
        }

        /// <summary>
        /// LineRenderer'ı temizle
        /// </summary>
        private void ClearLine()
        {
            lineRenderer.positionCount = 0;
            currentPoints.Clear();
            targetPlayer = null;
            
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
        }

        /// <summary>
        /// Mouse pozisyonunu world space'e çevir
        /// </summary>
        private Vector2 GetWorldMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
