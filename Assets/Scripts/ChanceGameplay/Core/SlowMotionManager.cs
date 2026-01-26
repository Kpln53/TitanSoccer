using UnityEngine;
using System;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Slow-Motion yöneticisi
    /// Joystick bırakıldığında oyun yavaşlar
    /// </summary>
    public class SlowMotionManager : MonoBehaviour
    {
        public static SlowMotionManager Instance { get; private set; }

        [Header("Slow Motion Ayarları")]
        [SerializeField] private float slowMotionScale = 0.2f;      // Yavaşlama oranı (0.2 = %20 hız)
        [SerializeField] private float transitionSpeed = 5f;         // Geçiş hızı
        [SerializeField] private float normalTimeScale = 1f;         // Normal hız

        [Header("Durum")]
        [SerializeField] private bool isSlowMotion = false;
        [SerializeField] private float currentTimeScale = 1f;
        [SerializeField] private float targetTimeScale = 1f;

        // Visual Effect
        private UnityEngine.UI.Image overlayImage;
        private float targetOverlayAlpha = 0f;
        private float currentOverlayAlpha = 0f;

        // Events
        public event Action OnSlowMotionStart;
        public event Action OnSlowMotionEnd;

        public bool IsSlowMotion => isSlowMotion;
        public float CurrentTimeScale => currentTimeScale;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateVisualEffect();
        }

        private void CreateVisualEffect()
        {
            // Canvas oluştur
            GameObject canvasObj = new GameObject("SlowMotionCanvas");
            canvasObj.transform.SetParent(transform);
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 90; // HUD'un altında (HUD 100)

            // Image oluştur
            GameObject imgObj = new GameObject("Overlay");
            imgObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform rect = imgObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            overlayImage = imgObj.AddComponent<UnityEngine.UI.Image>();
            overlayImage.color = new Color(0.1f, 0.1f, 0.1f, 0f); // Siyah/Gri, başlangıçta görünmez
            overlayImage.raycastTarget = false;
        }

        private void Update()
        {
            // Smooth geçiş (Time Scale)
            if (!Mathf.Approximately(currentTimeScale, targetTimeScale))
            {
                currentTimeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, Time.unscaledDeltaTime * transitionSpeed);
                
                // Çok yakınsa snap yap
                if (Mathf.Abs(currentTimeScale - targetTimeScale) < 0.01f)
                {
                    currentTimeScale = targetTimeScale;
                }

                Time.timeScale = currentTimeScale;
                Time.fixedDeltaTime = 0.02f * currentTimeScale; // Physics için
            }

            // Visual Effect Update
            if (overlayImage != null)
            {
                if (!Mathf.Approximately(currentOverlayAlpha, targetOverlayAlpha))
                {
                    currentOverlayAlpha = Mathf.Lerp(currentOverlayAlpha, targetOverlayAlpha, Time.unscaledDeltaTime * transitionSpeed * 2f);
                    Color c = overlayImage.color;
                    c.a = currentOverlayAlpha;
                    overlayImage.color = c;
                }
            }
        }

        /// <summary>
        /// Slow-motion'ı aktifleştir
        /// </summary>
        public void EnableSlowMotion()
        {
            if (isSlowMotion) return;

            isSlowMotion = true;
            targetTimeScale = slowMotionScale;
            targetOverlayAlpha = 0.6f; // %60 grileşme
            OnSlowMotionStart?.Invoke();

            Debug.Log("[SlowMotion] Enabled");
        }

        /// <summary>
        /// Slow-motion'ı devre dışı bırak (normal hıza dön)
        /// </summary>
        public void DisableSlowMotion()
        {
            if (!isSlowMotion) return;

            isSlowMotion = false;
            targetTimeScale = normalTimeScale;
            targetOverlayAlpha = 0f; // Normale dön
            OnSlowMotionEnd?.Invoke();

            Debug.Log("[SlowMotion] Disabled");
        }

        /// <summary>
        /// Anında normal hıza dön (geçiş olmadan)
        /// </summary>
        public void ResetToNormal()
        {
            isSlowMotion = false;
            currentTimeScale = normalTimeScale;
            targetTimeScale = normalTimeScale;
            Time.timeScale = normalTimeScale;
            Time.fixedDeltaTime = 0.02f;
            
            targetOverlayAlpha = 0f;
            currentOverlayAlpha = 0f;
            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = 0f;
                overlayImage.color = c;
            }
        }

        /// <summary>
        /// Slow-motion toggle
        /// </summary>
        public void ToggleSlowMotion()
        {
            if (isSlowMotion)
                DisableSlowMotion();
            else
                EnableSlowMotion();
        }

        /// <summary>
        /// Slow-motion oranını ayarla
        /// </summary>
        public void SetSlowMotionScale(float scale)
        {
            slowMotionScale = Mathf.Clamp(scale, 0.05f, 0.5f);
            if (isSlowMotion)
            {
                targetTimeScale = slowMotionScale;
            }
        }

        private void OnDestroy()
        {
            // Sahne değiştiğinde normal hıza dön
            ResetToNormal();

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}



