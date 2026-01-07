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

            // 5. Saha görselini oluştur
            CreateFieldVisuals();

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
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
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

        /// <summary>
        /// Saha görsellerini oluştur
        /// </summary>
        private void CreateFieldVisuals()
        {
            // Saha zemin
            GameObject field = new GameObject("Field");
            SpriteRenderer fieldRenderer = field.AddComponent<SpriteRenderer>();
            fieldRenderer.sprite = CreateRectSprite(30, 22, new Color(0.2f, 0.5f, 0.2f));
            fieldRenderer.sortingOrder = -10;
            field.transform.position = new Vector3(0f, 0f, 0f);

            // Orta çizgi
            GameObject midLine = new GameObject("MidLine");
            SpriteRenderer midRenderer = midLine.AddComponent<SpriteRenderer>();
            midRenderer.sprite = CreateRectSprite(30, 0.1f, Color.white);
            midRenderer.sortingOrder = -9;
            midLine.transform.position = new Vector3(0f, 0f, 0f);

            // Orta daire
            GameObject midCircle = new GameObject("MidCircle");
            LineRenderer circleRenderer = midCircle.AddComponent<LineRenderer>();
            DrawCircle(circleRenderer, Vector3.zero, 3f, Color.white);

            // Üst ceza sahası
            GameObject topPenalty = new GameObject("TopPenaltyArea");
            LineRenderer topRenderer = topPenalty.AddComponent<LineRenderer>();
            DrawRect(topRenderer, new Vector3(0f, 9f, 0f), 12f, 4f, Color.white);

            // Alt ceza sahası
            GameObject bottomPenalty = new GameObject("BottomPenaltyArea");
            LineRenderer bottomRenderer = bottomPenalty.AddComponent<LineRenderer>();
            DrawRect(bottomRenderer, new Vector3(0f, -9f, 0f), 12f, 4f, Color.white);

            // Üst kale
            GameObject topGoal = new GameObject("TopGoal");
            LineRenderer topGoalRenderer = topGoal.AddComponent<LineRenderer>();
            DrawGoal(topGoalRenderer, new Vector3(0f, 11f, 0f), 5f, true);

            // Alt kale
            GameObject bottomGoal = new GameObject("BottomGoal");
            LineRenderer bottomGoalRenderer = bottomGoal.AddComponent<LineRenderer>();
            DrawGoal(bottomGoalRenderer, new Vector3(0f, -11f, 0f), 5f, false);
        }

        /// <summary>
        /// Dikdörtgen sprite oluştur
        /// </summary>
        private Sprite CreateRectSprite(float width, float height, Color color)
        {
            int w = Mathf.Max(1, (int)(width * 10));
            int h = Mathf.Max(1, (int)(height * 10));
            Texture2D tex = new Texture2D(w, h);
            
            Color[] colors = new Color[w * h];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }
            tex.SetPixels(colors);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 10f);
        }

        /// <summary>
        /// Daire çiz
        /// </summary>
        private void DrawCircle(LineRenderer lr, Vector3 center, float radius, Color color)
        {
            int segments = 32;
            lr.positionCount = segments + 1;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.useWorldSpace = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = color;
            lr.sortingOrder = -8;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2f * Mathf.PI / segments;
                float x = center.x + Mathf.Cos(angle) * radius;
                float y = center.y + Mathf.Sin(angle) * radius;
                lr.SetPosition(i, new Vector3(x, y, 0f));
            }
        }

        /// <summary>
        /// Dikdörtgen çiz
        /// </summary>
        private void DrawRect(LineRenderer lr, Vector3 center, float width, float height, Color color)
        {
            lr.positionCount = 5;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.useWorldSpace = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = color;
            lr.sortingOrder = -8;

            float halfW = width / 2f;
            float halfH = height / 2f;

            lr.SetPosition(0, center + new Vector3(-halfW, -halfH, 0f));
            lr.SetPosition(1, center + new Vector3(-halfW, halfH, 0f));
            lr.SetPosition(2, center + new Vector3(halfW, halfH, 0f));
            lr.SetPosition(3, center + new Vector3(halfW, -halfH, 0f));
            lr.SetPosition(4, center + new Vector3(-halfW, -halfH, 0f));
        }

        /// <summary>
        /// Kale çiz
        /// </summary>
        private void DrawGoal(LineRenderer lr, Vector3 center, float width, bool isTop)
        {
            lr.positionCount = 4;
            lr.startWidth = 0.15f;
            lr.endWidth = 0.15f;
            lr.useWorldSpace = true;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.sortingOrder = -7;

            float halfW = width / 2f;
            float depth = isTop ? 1f : -1f;

            lr.SetPosition(0, center + new Vector3(-halfW, 0f, 0f));
            lr.SetPosition(1, center + new Vector3(-halfW, depth, 0f));
            lr.SetPosition(2, center + new Vector3(halfW, depth, 0f));
            lr.SetPosition(3, center + new Vector3(halfW, 0f, 0f));
        }

        /// <summary>
        /// Sahayı çiz (debug görsel)
        /// </summary>
        private void OnDrawGizmos()
        {
            // Saha sınırları
            Gizmos.color = Color.white;
            Vector3 fieldCenter = Vector3.zero;
            Vector3 fieldSize = new Vector3(30f, 22f, 0f);
            Gizmos.DrawWireCube(fieldCenter, fieldSize);

            // Kale alanları
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3(0f, 9f, 0f), new Vector3(12f, 4f, 0f));
            Gizmos.DrawWireCube(new Vector3(0f, -9f, 0f), new Vector3(12f, 4f, 0f));

            // Kaleler
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(-2.5f, 11f, 0f), new Vector3(2.5f, 11f, 0f));
            Gizmos.DrawLine(new Vector3(-2.5f, -11f, 0f), new Vector3(2.5f, -11f, 0f));
        }
    }
}
