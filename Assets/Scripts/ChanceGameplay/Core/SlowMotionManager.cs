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
        }

        private void Update()
        {
            // Smooth geçiş
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
        }

        /// <summary>
        /// Slow-motion'ı aktifleştir
        /// </summary>
        public void EnableSlowMotion()
        {
            if (isSlowMotion) return;

            isSlowMotion = true;
            targetTimeScale = slowMotionScale;
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


