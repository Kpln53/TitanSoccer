using UnityEngine;

namespace TitanSoccer.ChanceGameplay
{
    /// <summary>
    /// Pozisyon Sahnesi Bootstrap
    /// Sahne yüklendiğinde tüm sistemleri kurar
    /// </summary>
    public class ChanceSceneBootstrap : MonoBehaviour
    {
        [Header("Otomatik Kurulum")]
        [SerializeField] private bool autoSetup = true;

        [Header("Prefabs (Opsiyonel)")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private GameObject teammatePrefab;
        [SerializeField] private GameObject opponentPrefab;
        [SerializeField] private GameObject goalkeeperPrefab;

        private void Awake()
        {
            if (autoSetup)
            {
                SetupScene();
            }
        }

        /// <summary>
        /// Sahneyi kur
        /// </summary>
        public void SetupScene()
        {
            Debug.Log("[ChanceBootstrap] Setting up Chance scene...");

            // 1. Managers oluştur
            CreateManagers();

            // 2. Kamera kur
            SetupCamera();

            // 3. Input sistemlerini kur
            SetupInput();

            // 4. ChanceController oluştur (oyuncuları spawn eder)
            CreateChanceController();

            // 5. Saha görselini oluştur - ARTIK ChanceController ve FieldGenerator YAPIYOR
            // CreateFieldVisuals();

            // 6. HUD oluştur
            CreateHUD();

            Debug.Log("[ChanceBootstrap] Scene setup complete!");
        }

        /// <summary>
        /// Manager'ları oluştur
        /// </summary>
        private void CreateManagers()
        {
            // SlowMotionManager
            if (SlowMotionManager.Instance == null)
            {
                GameObject smObj = new GameObject("SlowMotionManager");
                smObj.AddComponent<SlowMotionManager>();
            }
        }

        /// <summary>
        /// Kamerayı kur
        /// </summary>
        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            // Ortografik kamera
            cam.orthographic = true;
            cam.orthographicSize = 10f;
            cam.transform.position = new Vector3(0f, 2f, -10f);
            cam.backgroundColor = new Color(0.15f, 0.4f, 0.15f); // Yeşil saha rengi

            // ChanceCamera ekle
            if (cam.GetComponent<ChanceCamera>() == null)
            {
                cam.gameObject.AddComponent<ChanceCamera>();
            }
        }

        /// <summary>
        /// Input sistemlerini kur
        /// </summary>
        private void SetupInput()
        {
            // TouchMoveController (Joystick yerine dokunarak hareket)
            if (TouchMoveController.Instance == null)
            {
                GameObject tmObj = new GameObject("TouchMoveController");
                tmObj.AddComponent<TouchMoveController>();
            }

            // LineDrawer
            if (LineDrawer.Instance == null)
            {
                GameObject ldObj = new GameObject("LineDrawer");
                ldObj.AddComponent<LineDrawer>();
            }

            // SlideTackleGesture
            if (SlideTackleGesture.Instance == null)
            {
                GameObject stObj = new GameObject("SlideTackleGesture");
                stObj.AddComponent<SlideTackleGesture>();
            }
        }

        /// <summary>
        /// ChanceController oluştur
        /// </summary>
        private void CreateChanceController()
        {
            if (ChanceController.Instance == null)
            {
                GameObject ccObj = new GameObject("ChanceController");
                ChanceController cc = ccObj.AddComponent<ChanceController>();
            }
        }

        /// <summary>
        /// HUD oluştur
        /// </summary>
        private void CreateHUD()
        {
            // EventSystem kontrolü (UI için gerekli)
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("[ChanceBootstrap] EventSystem created");
            }

            // Canvas
            GameObject canvasObj = new GameObject("ChanceHUDCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            UnityEngine.UI.CanvasScaler scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Skip Button (Pozisyonu Atla)
            GameObject skipBtnObj = new GameObject("SkipChanceButton");
            skipBtnObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform skipRect = skipBtnObj.AddComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(1f, 0f);
            skipRect.anchorMax = new Vector2(1f, 0f);
            skipRect.pivot = new Vector2(1f, 0f);
            skipRect.anchoredPosition = new Vector2(-20f, 20f);
            skipRect.sizeDelta = new Vector2(160f, 60f);
            
            UnityEngine.UI.Image skipBg = skipBtnObj.AddComponent<UnityEngine.UI.Image>();
            skipBg.color = new Color(0.7f, 0.15f, 0.15f, 0.95f);
            skipBg.raycastTarget = true;
            
            UnityEngine.UI.Button skipBtn = skipBtnObj.AddComponent<UnityEngine.UI.Button>();
            skipBtn.targetGraphic = skipBg;
            skipBtn.interactable = true;
            
            // Buton direkt olarak SkipChance'i çağırsın
            skipBtn.onClick.AddListener(() => {
                Debug.Log("[ChanceHUD] Skip button clicked!");
                if (ChanceController.Instance != null)
                {
                    ChanceController.Instance.SkipChance();
                }
            });
            
            // Skip Button Text
            GameObject skipTextObj = new GameObject("Text");
            skipTextObj.transform.SetParent(skipBtnObj.transform, false);
            
            RectTransform skipTextRect = skipTextObj.AddComponent<RectTransform>();
            skipTextRect.anchorMin = Vector2.zero;
            skipTextRect.anchorMax = Vector2.one;
            skipTextRect.offsetMin = Vector2.zero;
            skipTextRect.offsetMax = Vector2.zero;
            
            TMPro.TextMeshProUGUI skipText = skipTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            skipText.text = "⏩ ATLA";
            skipText.fontSize = 22;
            skipText.fontStyle = TMPro.FontStyles.Bold;
            skipText.alignment = TMPro.TextAlignmentOptions.Center;
            skipText.color = Color.white;
            skipText.raycastTarget = false; // Text'in raycast'ı engellememesi için
            
            // ChanceHUD component ekle (opsiyonel - diğer UI elementleri için)
            ChanceHUD hud = canvasObj.AddComponent<ChanceHUD>();
            hud.skipChanceButton = skipBtn;
            hud.skipButtonText = skipText;
            
            Debug.Log("[ChanceBootstrap] HUD with Skip button created and event listener attached");
        }

        // Eski saha oluşturma kodları kaldırıldı.
        // Artık FieldSettings ve FieldGenerator kullanılıyor.
    }
}
