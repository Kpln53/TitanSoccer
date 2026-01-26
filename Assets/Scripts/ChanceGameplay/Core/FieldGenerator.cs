using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TitanSoccer.ChanceGameplay
{
    [RequireComponent(typeof(FieldSettings))]
    public class FieldGenerator : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private bool autoUpdate = true;
        [SerializeField] private float lineWidth = 0.15f;
        [SerializeField] private Color lineColor = Color.white;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private int circleSegments = 64;

        [Header("References")]
        [SerializeField] private Transform linesContainer;
        [SerializeField] private Transform collidersContainer;

        private FieldSettings settings;

        private void Awake()
        {
            settings = GetComponent<FieldSettings>();
        }

        private void Start()
        {
            GenerateField();
        }

        private void OnValidate()
        {
            if (autoUpdate && !Application.isPlaying)
            {
                // Delay call to avoid errors during serialization
                #if UNITY_EDITOR
                EditorApplication.delayCall += () => {
                    if (this != null) GenerateField();
                };
                #endif
            }
        }

        public void GenerateField()
        {
            if (settings == null) settings = GetComponent<FieldSettings>();

            // Create containers if they don't exist
            if (linesContainer == null)
            {
                var container = new GameObject("FieldLines");
                container.transform.SetParent(transform, false);
                linesContainer = container.transform;
            }

            if (collidersContainer == null)
            {
                var container = new GameObject("FieldColliders");
                container.transform.SetParent(transform, false);
                collidersContainer = container.transform;
            }

            // Clear existing
            foreach (Transform child in linesContainer)
            {
                DestroyImmediate(child.gameObject);
            }
            foreach (Transform child in collidersContainer)
            {
                DestroyImmediate(child.gameObject);
            }

            GenerateLines();
            GenerateColliders();
        }

        private void GenerateLines()
        {
            // 1. Outer Boundary
            CreateLine("OuterBoundary", new Vector3[] {
                new Vector3(-settings.width/2, -settings.length/2),
                new Vector3(-settings.width/2, settings.length/2),
                new Vector3(settings.width/2, settings.length/2),
                new Vector3(settings.width/2, -settings.length/2)
            }, true);

            // 2. Center Line
            CreateLine("CenterLine", new Vector3[] {
                new Vector3(-settings.width/2, 0),
                new Vector3(settings.width/2, 0)
            }, false);

            // 3. Center Circle
            CreateCircle("CenterCircle", Vector3.zero, 3f); // Standard radius 9.15m -> scaled down for game feel? keeping 3f as per gizmos

            // 4. Penalty Areas
            // Top
            float topY = settings.length / 2;
            CreateLine("TopPenaltyArea", new Vector3[] {
                new Vector3(-settings.penaltyAreaWidth/2, topY),
                new Vector3(-settings.penaltyAreaWidth/2, topY - settings.penaltyAreaLength),
                new Vector3(settings.penaltyAreaWidth/2, topY - settings.penaltyAreaLength),
                new Vector3(settings.penaltyAreaWidth/2, topY)
            }, false);

            // Bottom
            float bottomY = -settings.length / 2;
            CreateLine("BottomPenaltyArea", new Vector3[] {
                new Vector3(-settings.penaltyAreaWidth/2, bottomY),
                new Vector3(-settings.penaltyAreaWidth/2, bottomY + settings.penaltyAreaLength),
                new Vector3(settings.penaltyAreaWidth/2, bottomY + settings.penaltyAreaLength),
                new Vector3(settings.penaltyAreaWidth/2, bottomY)
            }, false);

            // 5. Goals (Visual box)
            // Top Goal
            CreateLine("TopGoal", new Vector3[] {
                new Vector3(-settings.goalWidth/2, topY),
                new Vector3(-settings.goalWidth/2, topY + settings.goalDepth),
                new Vector3(settings.goalWidth/2, topY + settings.goalDepth),
                new Vector3(settings.goalWidth/2, topY)
            }, false);

            // Bottom Goal
            CreateLine("BottomGoal", new Vector3[] {
                new Vector3(-settings.goalWidth/2, bottomY),
                new Vector3(-settings.goalWidth/2, bottomY - settings.goalDepth),
                new Vector3(settings.goalWidth/2, bottomY - settings.goalDepth),
                new Vector3(settings.goalWidth/2, bottomY)
            }, false);
        }

        private void GenerateColliders()
        {
            // Create edge colliders for boundaries to keep ball in play
            // But maybe we want the ball to go out? 
            // For now, let's add colliders for the goal nets.

            float topY = settings.length / 2;
            float bottomY = -settings.length / 2;

            // Top Goal Net
            CreateBoxCollider("TopGoalBack", new Vector2(0, topY + settings.goalDepth + 0.1f), new Vector2(settings.goalWidth, 0.2f));
            CreateBoxCollider("TopGoalLeft", new Vector2(-settings.goalWidth/2 - 0.1f, topY + settings.goalDepth/2), new Vector2(0.2f, settings.goalDepth));
            CreateBoxCollider("TopGoalRight", new Vector2(settings.goalWidth/2 + 0.1f, topY + settings.goalDepth/2), new Vector2(0.2f, settings.goalDepth));

            // Bottom Goal Net
            CreateBoxCollider("BottomGoalBack", new Vector2(0, bottomY - settings.goalDepth - 0.1f), new Vector2(settings.goalWidth, 0.2f));
            CreateBoxCollider("BottomGoalLeft", new Vector2(-settings.goalWidth/2 - 0.1f, bottomY - settings.goalDepth/2), new Vector2(0.2f, settings.goalDepth));
            CreateBoxCollider("BottomGoalRight", new Vector2(settings.goalWidth/2 + 0.1f, bottomY - settings.goalDepth/2), new Vector2(0.2f, settings.goalDepth));
        }

        private void CreateLine(string name, Vector3[] points, bool loop)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(linesContainer, false);
            
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = points.Length;
            lr.SetPositions(points);
            lr.loop = loop;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.sortingOrder = 0; // Floor
        }

        private void CreateCircle(string name, Vector3 center, float radius)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(linesContainer, false);
            obj.transform.localPosition = center;

            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = circleSegments + 1;
            lr.loop = true;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lineColor;
            lr.endColor = lineColor;

            Vector3[] points = new Vector3[circleSegments + 1];
            float angleStep = 360f / circleSegments;

            for (int i = 0; i <= circleSegments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                points[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            }

            lr.SetPositions(points);
        }

        private void CreateBoxCollider(string name, Vector2 pos, Vector2 size)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(collidersContainer, false);
            obj.transform.localPosition = pos;

            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.size = size;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FieldGenerator))]
    public class FieldGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FieldGenerator generator = (FieldGenerator)target;
            if (GUILayout.Button("Generate Field"))
            {
                generator.GenerateField();
            }
        }
    }
#endif
}
