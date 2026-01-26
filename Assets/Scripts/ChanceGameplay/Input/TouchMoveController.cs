using UnityEngine;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Dokunarak Hareket Sistemi
    /// Sahada dokunulan noktaya hareket et
    /// </summary>
    public class TouchMoveController : MonoBehaviour
    {
        public static TouchMoveController Instance { get; private set; }

        [Header("Hareket Ayarları")]
        [SerializeField] private float arriveDistance = 0.3f;       // Hedefe varış mesafesi
        [SerializeField] private float playerTouchRadius = 1.2f;    // Oyuncuya dokunma yarıçapı (çizgi için)

        [Header("Görsel")]
        [SerializeField] private bool showTargetIndicator = true;
        [SerializeField] private Color targetColor = new Color(1f, 1f, 1f, 0.5f);

        [Header("Durum")]
        [SerializeField] private bool isMoving = false;
        [SerializeField] private Vector2 targetPosition;

        // Target indicator
        private GameObject targetIndicator;
        private SpriteRenderer targetRenderer;

        // Events
        public event Action<Vector2> OnMoveStarted;
        public event Action OnMoveCompleted;
        public event Action OnMoveCancelled;

        // Properties
        public bool IsMoving => isMoving;
        public Vector2 TargetPosition => targetPosition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateTargetIndicator();
        }

        private void CreateTargetIndicator()
        {
            if (!showTargetIndicator) return;

            targetIndicator = new GameObject("MoveTargetIndicator");
            targetIndicator.transform.SetParent(transform);

            targetRenderer = targetIndicator.AddComponent<SpriteRenderer>();
            targetRenderer.sprite = CreateCircleSprite(32);
            targetRenderer.color = targetColor;
            targetRenderer.sortingOrder = 0;
            targetIndicator.transform.localScale = Vector3.one * 0.5f;
            targetIndicator.SetActive(false);
        }

        private void Update()
        {
            // Input handling - gerekli ama optimize edilmiş
            if (enabled && gameObject.activeInHierarchy)
            {
                HandleInput();
                UpdateMovement();
                UpdateTargetIndicator();
            }
        }

        private void HandleInput()
        {
            // Çizgi çiziliyorken hareket input'u alma
            if (LineDrawer.Instance != null && LineDrawer.Instance.IsDrawing)
            {
                return;
            }

            // Touch veya Mouse input
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 touchPos = GetWorldTouchPosition();
                
                // Oyuncunun üzerinde mi kontrol et (çizgi için ayrılmış)
                if (ChanceController.Instance?.Player != null)
                {
                    Vector2 playerPos = ChanceController.Instance.Player.transform.position;
                    float distToPlayer = Vector2.Distance(touchPos, playerPos);

                    if (distToPlayer <= playerTouchRadius)
                    {
                        // Oyuncunun üzerinde - çizgi sistemi devralacak
                        return;
                    }
                }

                // Saha içinde mi kontrol et
                if (IsValidMoveTarget(touchPos))
                {
                    StartMove(touchPos);
                }
            }
        }

        /// <summary>
        /// Geçerli hareket hedefi mi?
        /// </summary>
        private bool IsValidMoveTarget(Vector2 position)
        {
            if (ChanceController.Instance == null) return false;

            // FieldSettings varsa onu kullan
            if (ChanceController.Instance.Field != null)
            {
                var f = ChanceController.Instance.Field;
                float halfWidth = f.width / 2f;
                float halfHeight = f.length / 2f;

                return position.x >= -halfWidth && position.x <= halfWidth &&
                       position.y >= -halfHeight && position.y <= halfHeight;
            }

            // Yoksa eski fieldSize'ı kullan
            Vector2 fieldSize = ChanceController.Instance.FieldSize;
            float hw = fieldSize.x / 2f;
            float hh = fieldSize.y / 2f;

            return position.x >= -hw && position.x <= hw &&
                   position.y >= -hh && position.y <= hh;
        }

        /// <summary>
        /// Hareketi başlat
        /// </summary>
        private void StartMove(Vector2 target)
        {
            targetPosition = target;
            isMoving = true;

            // Slow-motion'dan çık
            if (ChanceController.Instance?.FlowState == GameFlowState.WaitingForInput)
            {
                ChanceController.Instance.StartAction();
            }

            OnMoveStarted?.Invoke(target);
            Debug.Log($"[TouchMove] Moving to {target}");
        }

        /// <summary>
        /// Hareketi güncelle
        /// </summary>
        private void UpdateMovement()
        {
            if (!isMoving) return;
            if (ChanceController.Instance?.Player == null) return;

            PlayerController player = ChanceController.Instance.Player;
            Vector2 playerPos = player.transform.position;
            float distToTarget = Vector2.Distance(playerPos, targetPosition);

            if (distToTarget <= arriveDistance)
            {
                // Hedefe ulaştı
                CompleteMove();
            }
        }

        /// <summary>
        /// Hareketi tamamla
        /// </summary>
        private void CompleteMove()
        {
            isMoving = false;

            // Slow-motion'a gir (hamle bekle)
            if (ChanceController.Instance != null && 
                ChanceController.Instance.FlowState == GameFlowState.Executing)
            {
                ChanceController.Instance.ActionCompleted();
            }

            OnMoveCompleted?.Invoke();
            Debug.Log("[TouchMove] Move completed");
        }

        /// <summary>
        /// Hareketi iptal et
        /// </summary>
        public void CancelMove()
        {
            if (!isMoving) return;

            isMoving = false;
            OnMoveCancelled?.Invoke();
            Debug.Log("[TouchMove] Move cancelled");
        }

        /// <summary>
        /// Hedef göstergesini güncelle
        /// </summary>
        private void UpdateTargetIndicator()
        {
            if (targetIndicator == null) return;

            if (isMoving && showTargetIndicator)
            {
                targetIndicator.SetActive(true);
                targetIndicator.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0f);

                // Pulse efekti
                float pulse = 0.4f + Mathf.Sin(Time.time * 5f) * 0.1f;
                targetIndicator.transform.localScale = Vector3.one * pulse;
            }
            else
            {
                targetIndicator.SetActive(false);
            }
        }

        /// <summary>
        /// Hareket yönü ve mesafesini al
        /// </summary>
        public Vector2 GetMoveDirection()
        {
            if (!isMoving || ChanceController.Instance?.Player == null)
                return Vector2.zero;

            Vector2 playerPos = ChanceController.Instance.Player.transform.position;
            return (targetPosition - playerPos).normalized;
        }

        /// <summary>
        /// Hedefe olan mesafeyi al
        /// </summary>
        public float GetDistanceToTarget()
        {
            if (!isMoving || ChanceController.Instance?.Player == null)
                return 0f;

            Vector2 playerPos = ChanceController.Instance.Player.transform.position;
            return Vector2.Distance(playerPos, targetPosition);
        }

        /// <summary>
        /// Touch pozisyonunu world space'e çevir
        /// </summary>
        private Vector2 GetWorldTouchPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// Daire sprite oluştur
        /// </summary>
        private Sprite CreateCircleSprite(int size)
        {
            Texture2D tex = new Texture2D(size, size);
            float radius = size / 2f;
            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    
                    // Halka şeklinde (içi boş)
                    bool isRing = dist >= radius - 3f && dist <= radius;
                    tex.SetPixel(x, y, isRing ? Color.white : Color.clear);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
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



