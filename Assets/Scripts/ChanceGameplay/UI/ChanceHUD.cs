using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Pozisyon HUD - Oynanış sırasında gösterilen UI
    /// </summary>
    public class ChanceHUD : MonoBehaviour
    {
        [Header("Üst Panel")]
        [SerializeField] private TextMeshProUGUI minuteText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI chanceTypeText;

        [Header("Alt Panel")]
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Image slowMotionIndicator;

        [Header("Kamera Butonu")]
        [SerializeField] private Button cameraSwitchButton;
        [SerializeField] private TextMeshProUGUI cameraButtonText;

        [Header("Renkler")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color slowMotionColor = Color.cyan;
        [SerializeField] private Color attackColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color defenseColor = new Color(0.8f, 0.2f, 0.2f);

        private ChanceCamera chanceCamera;

        private void Start()
        {
            SetupUI();
            SetupButtons();

            // Slow-motion event'lerini dinle
            if (SlowMotionManager.Instance != null)
            {
                SlowMotionManager.Instance.OnSlowMotionStart += OnSlowMotionStart;
                SlowMotionManager.Instance.OnSlowMotionEnd += OnSlowMotionEnd;
            }

            chanceCamera = FindObjectOfType<ChanceCamera>();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void SetupUI()
        {
            // Başlangıç metinleri
            if (instructionText != null)
            {
                instructionText.text = "Joystick ile hareket et\nÇizgi çizerek pas/şut yap";
            }
        }

        private void SetupButtons()
        {
            if (cameraSwitchButton != null)
            {
                cameraSwitchButton.onClick.AddListener(OnCameraSwitchClicked);
            }
        }

        private void UpdateUI()
        {
            if (ChanceController.Instance == null) return;

            // Dakika
            if (minuteText != null && MatchContext.Instance != null)
            {
                minuteText.text = $"{MatchContext.Instance.currentMinute}'";
            }

            // Skor
            if (scoreText != null && MatchContext.Instance != null)
            {
                scoreText.text = $"{MatchContext.Instance.homeScore} - {MatchContext.Instance.awayScore}";
            }

            // Pozisyon tipi
            if (chanceTypeText != null)
            {
                bool isAttack = ChanceController.Instance.CurrentChanceType == ChanceType.Attack;
                chanceTypeText.text = isAttack ? "ATAK" : "SAVUNMA";
                chanceTypeText.color = isAttack ? attackColor : defenseColor;
            }

            // Talimat metni
            UpdateInstructionText();
        }

        private void UpdateInstructionText()
        {
            if (instructionText == null || ChanceController.Instance == null) return;

            switch (ChanceController.Instance.FlowState)
            {
                case GameFlowState.WaitingForInput:
                    if (ChanceController.Instance.CurrentChanceType == ChanceType.Attack)
                    {
                        instructionText.text = "🕹️ Hareket et veya ✏️ Çizgi çiz";
                    }
                    else
                    {
                        instructionText.text = "🛡️ Kayarak müdahale için kaydır";
                    }
                    break;

                case GameFlowState.Executing:
                    instructionText.text = "...";
                    break;

                case GameFlowState.BallInFlight:
                    instructionText.text = "⚽ Top havada...";
                    break;

                case GameFlowState.AIPlaying:
                    instructionText.text = "🤖 Takım arkadaşı oynuyor";
                    break;

                case GameFlowState.Ended:
                    var outcome = ChanceController.Instance.Outcome;
                    switch (outcome)
                    {
                        case ChanceOutcome.Goal:
                            instructionText.text = "⚽ GOL!";
                            instructionText.color = Color.green;
                            break;
                        case ChanceOutcome.Saved:
                            instructionText.text = "🧤 Kurtarış!";
                            instructionText.color = Color.yellow;
                            break;
                        case ChanceOutcome.Missed:
                            instructionText.text = "❌ Kaçtı!";
                            instructionText.color = Color.red;
                            break;
                        case ChanceOutcome.Tackled:
                        case ChanceOutcome.Cleared:
                            instructionText.text = "✅ Temizlendi!";
                            instructionText.color = Color.cyan;
                            break;
                        default:
                            instructionText.text = "Pozisyon bitti";
                            break;
                    }
                    break;
            }
        }

        private void OnSlowMotionStart()
        {
            if (slowMotionIndicator != null)
            {
                slowMotionIndicator.color = slowMotionColor;
            }
        }

        private void OnSlowMotionEnd()
        {
            if (slowMotionIndicator != null)
            {
                slowMotionIndicator.color = normalColor;
            }
        }

        private void OnCameraSwitchClicked()
        {
            if (chanceCamera != null)
            {
                chanceCamera.ToggleMode();
                UpdateCameraButtonText();
            }
        }

        private void UpdateCameraButtonText()
        {
            if (cameraButtonText == null || chanceCamera == null) return;

            // Kamera modu göster (basit)
            cameraButtonText.text = "📷";
        }

        private void OnDestroy()
        {
            if (SlowMotionManager.Instance != null)
            {
                SlowMotionManager.Instance.OnSlowMotionStart -= OnSlowMotionStart;
                SlowMotionManager.Instance.OnSlowMotionEnd -= OnSlowMotionEnd;
            }
        }
    }
}

