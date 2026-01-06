using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Pozisyon Kamerası
    /// Oyuncuyu veya topu takip eder
    /// </summary>
    public class ChanceCamera : MonoBehaviour
    {
        [Header("Takip Ayarları")]
        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

        [Header("Zoom Ayarları")]
        [SerializeField] private float defaultZoom = 8f;
        [SerializeField] private float closeZoom = 5f;
        [SerializeField] private float wideZoom = 12f;

        [Header("Sınırlar")]
        [SerializeField] private float minX = -12f;
        [SerializeField] private float maxX = 12f;
        [SerializeField] private float minY = -10f;
        [SerializeField] private float maxY = 12f;

        [Header("Durum")]
        [SerializeField] private CameraMode currentMode = CameraMode.FollowPlayer;
        [SerializeField] private Transform currentTarget;

        private Camera cam;
        private float targetZoom;

        public enum CameraMode
        {
            FollowPlayer,   // Oyuncuyu takip et
            FollowBall,     // Topu takip et
            FollowBoth,     // İkisinin ortasını takip et
            Fixed           // Sabit
        }

        private void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                cam = Camera.main;
            }

            targetZoom = defaultZoom;
        }

        private bool initialized = false;

        private void Start()
        {
            TryInitialize();
        }

        private void TryInitialize()
        {
            if (initialized) return;
            
            // ChanceController hazır mı?
            if (ChanceController.Instance != null && ChanceController.Instance.Player != null)
            {
                SetTarget(ChanceController.Instance.Player.transform, CameraMode.FollowPlayer);
                initialized = true;
                Debug.Log("[ChanceCamera] Initialized - following player");
            }
        }

        private void LateUpdate()
        {
            // Geç başlatma için
            if (!initialized)
            {
                TryInitialize();
            }
            
            UpdatePosition();
            UpdateZoom();
        }

        /// <summary>
        /// Kamera pozisyonunu güncelle
        /// </summary>
        private void UpdatePosition()
        {
            Vector3 targetPos = GetTargetPosition();

            if (targetPos == Vector3.zero) return;

            // Hedef pozisyon + offset
            Vector3 desiredPos = targetPos + offset;

            // Sınırları uygula
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);

            // Yumuşak takip
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
        }

        /// <summary>
        /// Hedef pozisyonu al
        /// </summary>
        private Vector3 GetTargetPosition()
        {
            switch (currentMode)
            {
                case CameraMode.FollowPlayer:
                    if (currentTarget != null)
                        return currentTarget.position;
                    break;

                case CameraMode.FollowBall:
                    if (ChanceController.Instance?.Ball != null)
                        return ChanceController.Instance.Ball.transform.position;
                    break;

                case CameraMode.FollowBoth:
                    if (currentTarget != null && ChanceController.Instance?.Ball != null)
                    {
                        Vector3 playerPos = currentTarget.position;
                        Vector3 ballPos = ChanceController.Instance.Ball.transform.position;
                        return (playerPos + ballPos) / 2f;
                    }
                    break;

                case CameraMode.Fixed:
                    return transform.position - offset;
            }

            // Fallback
            if (currentTarget != null)
                return currentTarget.position;

            return Vector3.zero;
        }

        /// <summary>
        /// Zoom güncelle
        /// </summary>
        private void UpdateZoom()
        {
            if (cam == null) return;

            // Moda göre zoom
            switch (currentMode)
            {
                case CameraMode.FollowPlayer:
                    targetZoom = defaultZoom;
                    break;

                case CameraMode.FollowBall:
                    // Şut sırasında biraz yakınlaş
                    targetZoom = closeZoom;
                    break;

                case CameraMode.FollowBoth:
                    // İkisini de göster
                    if (currentTarget != null && ChanceController.Instance?.Ball != null)
                    {
                        float dist = Vector3.Distance(currentTarget.position, 
                            ChanceController.Instance.Ball.transform.position);
                        targetZoom = Mathf.Lerp(closeZoom, wideZoom, dist / 15f);
                    }
                    break;

                case CameraMode.Fixed:
                    targetZoom = wideZoom;
                    break;
            }

            // Yumuşak zoom
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }

        /// <summary>
        /// Hedef ve mod ayarla
        /// </summary>
        public void SetTarget(Transform target, CameraMode mode)
        {
            currentTarget = target;
            currentMode = mode;

            Debug.Log($"[ChanceCamera] Target: {target?.name}, Mode: {mode}");
        }

        /// <summary>
        /// Topu takip et
        /// </summary>
        public void FollowBall()
        {
            currentMode = CameraMode.FollowBall;
        }

        /// <summary>
        /// Oyuncuyu takip et
        /// </summary>
        public void FollowPlayer()
        {
            currentMode = CameraMode.FollowPlayer;
        }

        /// <summary>
        /// İkisini de takip et
        /// </summary>
        public void FollowBoth()
        {
            currentMode = CameraMode.FollowBoth;
        }

        /// <summary>
        /// Kamera modunu değiştir (toggle)
        /// </summary>
        public void ToggleMode()
        {
            switch (currentMode)
            {
                case CameraMode.FollowPlayer:
                    currentMode = CameraMode.FollowBall;
                    break;
                case CameraMode.FollowBall:
                    currentMode = CameraMode.FollowBoth;
                    break;
                case CameraMode.FollowBoth:
                    currentMode = CameraMode.FollowPlayer;
                    break;
            }

            Debug.Log($"[ChanceCamera] Mode toggled to: {currentMode}");
        }

        /// <summary>
        /// Anlık pozisyona sıçra (geçiş olmadan)
        /// </summary>
        public void SnapToTarget()
        {
            Vector3 targetPos = GetTargetPosition();
            if (targetPos != Vector3.zero)
            {
                transform.position = targetPos + offset;
            }
        }
    }
}

