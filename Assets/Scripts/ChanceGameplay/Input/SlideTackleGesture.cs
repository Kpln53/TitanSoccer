using UnityEngine;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Kayarak Müdahale Gesture Sistemi
    /// Oyuncu üzerinde basılı tut + hızlı kaydır = kayarak müdahale
    /// </summary>
    public class SlideTackleGesture : MonoBehaviour
    {
        public static SlideTackleGesture Instance { get; private set; }

        [Header("Gesture Ayarları")]
        [SerializeField] private float minSwipeDistance = 0.5f;      // Minimum kaydırma mesafesi
        [SerializeField] private float maxSwipeTime = 0.5f;          // Maximum kaydırma süresi
        [SerializeField] private float playerTouchRadius = 1.2f;     // Oyuncuya dokunma yarıçapı
        [SerializeField] private float swipeSpeedThreshold = 3f;     // Minimum kaydırma hızı

        [Header("Durum")]
        [SerializeField] private bool isHolding = false;
        [SerializeField] private bool isSwipping = false;

        // Gesture verileri
        private Vector2 holdStartPos;
        private Vector2 swipeStartPos;
        private float holdStartTime;
        private float swipeStartTime;

        // Events
        public event Action<Vector2> OnSlideTackle;  // Yön parametresi ile

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Sadece savunma pozisyonunda aktif
            if (ChanceController.Instance == null) return;
            if (ChanceController.Instance.CurrentChanceType != ChanceType.Defense) return;

            // Çizgi çiziliyorken gesture algılama
            if (LineDrawer.Instance != null && LineDrawer.Instance.IsDrawing)
            {
                ResetGesture();
                return;
            }

            HandleInput();
        }

        private void HandleInput()
        {
            Vector2 currentPos = GetWorldMousePosition();

            if (Input.GetMouseButtonDown(0))
            {
                TryStartHold(currentPos);
            }
            else if (Input.GetMouseButton(0) && isHolding)
            {
                CheckSwipe(currentPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isSwipping)
                {
                    CompleteSwipe(currentPos);
                }
                ResetGesture();
            }
        }

        /// <summary>
        /// Basılı tutmayı başlat
        /// </summary>
        private void TryStartHold(Vector2 position)
        {
            if (ChanceController.Instance?.Player == null) return;

            Vector2 playerPos = ChanceController.Instance.Player.transform.position;
            float distance = Vector2.Distance(position, playerPos);

            if (distance <= playerTouchRadius)
            {
                isHolding = true;
                holdStartPos = position;
                holdStartTime = Time.unscaledTime;
                swipeStartPos = position;
                swipeStartTime = Time.unscaledTime;

                Debug.Log("[SlideTackle] Hold started");
            }
        }

        /// <summary>
        /// Kaydırma kontrolü
        /// </summary>
        private void CheckSwipe(Vector2 currentPos)
        {
            float swipeDistance = Vector2.Distance(swipeStartPos, currentPos);
            float swipeTime = Time.unscaledTime - swipeStartTime;

            // Hızlı kaydırma tespiti
            if (swipeTime > 0 && swipeDistance > minSwipeDistance)
            {
                float swipeSpeed = swipeDistance / swipeTime;

                if (swipeSpeed >= swipeSpeedThreshold && swipeTime <= maxSwipeTime)
                {
                    isSwipping = true;
                }
            }

            // Swipe başlangıcını güncelle (süre aşıldıysa)
            if (swipeTime > maxSwipeTime && !isSwipping)
            {
                swipeStartPos = currentPos;
                swipeStartTime = Time.unscaledTime;
            }
        }

        /// <summary>
        /// Kaydırmayı tamamla
        /// </summary>
        private void CompleteSwipe(Vector2 endPos)
        {
            Vector2 swipeDir = (endPos - swipeStartPos).normalized;
            float swipeDistance = Vector2.Distance(swipeStartPos, endPos);

            if (swipeDistance >= minSwipeDistance)
            {
                Debug.Log($"[SlideTackle] Slide tackle triggered! Direction: {swipeDir}");
                OnSlideTackle?.Invoke(swipeDir);
            }
        }

        /// <summary>
        /// Gesture'ı sıfırla
        /// </summary>
        private void ResetGesture()
        {
            isHolding = false;
            isSwipping = false;
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
