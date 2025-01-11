using UnityEngine;
using UnityEditor;
using System;

namespace VegetationPainter
{
    public class VegetationPainterWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "Vegetation Painter";

        // Core references
        private BrushSettings brushSettings = new BrushSettings();
        private VegetationBrush brush;
        private GameObject vegetationRoot;
        private VegetationManager vegetationManager;

        // Scene painting
        private bool isPainting;

        // Visualization Toggles
        private bool showScatterPreview = true;
        private bool showDensityVisualization = true;
        private bool showProjectedGrid = true;
        private bool showSlopeHeatmap = true;

        // Scatter Preview randomization
        private int scatterSeed = 12345;
        private System.Random scatterRng;

        [MenuItem("Window/Vegetation Painter")]
        public static void ShowWindow() => GetWindow<VegetationPainterWindow>(WINDOW_TITLE);

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            scatterRng = new System.Random(scatterSeed);
            Initialize();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            vegetationManager?.Cleanup();
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Vegetation Painter", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Core Brush Settings Box
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // Core settings moved into the box
                brushSettings.brushMode = (BrushMode)EditorGUILayout.EnumPopup("Brush Mode", brushSettings.brushMode);
                brushSettings.radius = EditorGUILayout.Slider("Radius", brushSettings.radius, 1f, 50f);
                brushSettings.alignToNormal = EditorGUILayout.Toggle("Align to Normal", brushSettings.alignToNormal);
                brushSettings.maxSlopeAngle = EditorGUILayout.Slider("Max Slope Angle", brushSettings.maxSlopeAngle, 0f, 90f);

                // Mode-specific settings
                switch (brushSettings.brushMode)
                {
                    case BrushMode.Paint:
                        brushSettings.spacing = EditorGUILayout.Slider("Paint Spacing", brushSettings.spacing, 0.1f, 10f);
                        DrawDensitySettings();
                        break;
                    case BrushMode.Scatter:
                        brushSettings.density = EditorGUILayout.Slider("Scatter Density", brushSettings.density, 0.1f, 1f);
                        DrawDensitySettings();
                        break;
                    case BrushMode.Stamp:
                        DrawDensitySettings();
                        EditorGUILayout.HelpBox("Click to place single objects.", MessageType.Info);
                        break;
                    case BrushMode.Eraser:
                        EditorGUILayout.HelpBox("Hold and drag to erase vegetation.", MessageType.Info);
                        break;
                }
            }
            EditorGUILayout.EndVertical();

            // Transform Settings
            GUILayout.Space(5);
            DrawTransformSettings();

            // Visualization Toggles
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Visualization Options", EditorStyles.boldLabel);
            showScatterPreview = EditorGUILayout.Toggle("Show Scatter Preview", showScatterPreview);
            showDensityVisualization = EditorGUILayout.Toggle("Show Density Visualization", showDensityVisualization);
            showProjectedGrid = EditorGUILayout.Toggle("Show Projected Grid", showProjectedGrid);
            showSlopeHeatmap = EditorGUILayout.Toggle("Show Slope Heatmap", showSlopeHeatmap);

            if (GUILayout.Button("Randomize Scatter Preview"))
            {
                scatterSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                scatterRng = new System.Random(scatterSeed);
            }

            GUILayout.Space(5);
            DrawPrefabSection();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                vegetationManager?.SaveToAsset();
            if (GUILayout.Button("Load", EditorStyles.toolbarButton))
                vegetationManager?.LoadFromAsset();
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("Clear Vegetation", 
                    "Are you sure you want to clear all vegetation?", "Yes", "No"))
                {
                    vegetationManager?.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDensitySettings()
        {
            var ds = brushSettings.densitySettings;
            ds.checkDensity = EditorGUILayout.Toggle("Check Density", ds.checkDensity);
            if (!ds.checkDensity) return;

            using (new EditorGUI.IndentLevelScope())
            {
                ds.preventOverlap = EditorGUILayout.Toggle("Prevent Overlap", ds.preventOverlap);
                if (ds.preventOverlap)
                {
                    ds.largeObjectRadius = EditorGUILayout.FloatField("Object Radius", ds.largeObjectRadius);
                }
                else
                {
                    ds.minDistance = EditorGUILayout.FloatField("Min Distance", ds.minDistance);
                }
            }
        }

        private void DrawTransformSettings()
        {
            var ts = brushSettings.transformSettings;
            EditorGUILayout.LabelField("Transform Settings", EditorStyles.boldLabel);

            ts.useAdvancedRotation = EditorGUILayout.Toggle("Advanced Rotation", ts.useAdvancedRotation);
            if (ts.useAdvancedRotation)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    ts.rotationRangePerAxis = EditorGUILayout.Vector3Field("Rotation Range", ts.rotationRangePerAxis);
                }
            }

            ts.useNonUniformScale = EditorGUILayout.Toggle("Non-Uniform Scale", ts.useNonUniformScale);
            using (new EditorGUI.IndentLevelScope())
            {
                if (ts.useNonUniformScale)
                {
                    ts.minScale = EditorGUILayout.Vector3Field("Min Scale", ts.minScale);
                    ts.maxScale = EditorGUILayout.Vector3Field("Max Scale", ts.maxScale);
                }
                else
                {
                    float minUniform = ts.minScale.x;
                    float maxUniform = ts.maxScale.x;
                    minUniform = EditorGUILayout.FloatField("Min Scale", minUniform);
                    maxUniform = EditorGUILayout.FloatField("Max Scale", maxUniform);
                    ts.minScale = Vector3.one * minUniform;
                    ts.maxScale = Vector3.one * maxUniform;
                }
            }

            if (brushSettings.alignToNormal)
            {
                ts.maxTiltAngle = EditorGUILayout.Slider("Max Tilt Angle", ts.maxTiltAngle, 0f, 45f);
            }
            ts.positionJitter = EditorGUILayout.Vector3Field("Position Jitter", ts.positionJitter);
        }

        private void DrawPrefabSection()
        {
            EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);

            for (int i = 0; i < brushSettings.prefabs.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();

                // Show a small preview
                Texture2D preview = AssetPreview.GetAssetPreview(brushSettings.prefabs[i]);
                if (preview)
                {
                    GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                }
                else
                {
                    GUILayout.Box("No\nPreview", GUILayout.Width(64), GUILayout.Height(64));
                }

                EditorGUILayout.BeginVertical();
                brushSettings.prefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                    "Prefab",
                    brushSettings.prefabs[i],
                    typeof(GameObject),
                    false
                );

                // Show optional weight
                if (i < brushSettings.weights.Count)
                {
                    brushSettings.weights[i] = EditorGUILayout.Slider("Weight", brushSettings.weights[i], 0f, 10f);
                }

                EditorGUILayout.EndVertical();

                // Remove button
                if (GUILayout.Button("Remove", GUILayout.Width(60), GUILayout.Height(30)))
                {
                    brushSettings.prefabs.RemoveAt(i);
                    if (i < brushSettings.weights.Count)
                        brushSettings.weights.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Prefab"))
            {
                brushSettings.prefabs.Add(null);
                brushSettings.weights.Add(1f);
            }
        }

        private void Initialize()
        {
            if (vegetationRoot == null)
            {
                vegetationRoot = GameObject.Find("VegetationRoot");
                if (vegetationRoot == null)
                {
                    vegetationRoot = new GameObject("VegetationRoot");
                }
            }

            if (brush == null)
            {
                brush = new VegetationBrush(brushSettings, vegetationRoot);
            }

            if (vegetationManager == null)
            {
                var data = CreateInstance<VegetationPainterData>();
                vegetationManager = new VegetationManager(vegetationRoot, data);
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                brush.DrawPreview(hit);

                if (showSlopeHeatmap)
                    DrawSlopeHeatmap(hit.point, brushSettings.radius, brushSettings.maxSlopeAngle);

                if (showProjectedGrid)
                    DrawProjectedGrid(hit.point);

                if (showDensityVisualization)
                    DrawDensityVisualization(hit.point);

                if (showScatterPreview && brushSettings.brushMode == BrushMode.Scatter)
                    DrawScatterPreview(hit.point);

                HandlePaintingEvents(e, hit);
                sceneView.Repaint();
            }
        }

        private void HandlePaintingEvents(Event e, RaycastHit hit)
        {
            switch (e.type)
            {
                case EventType.MouseDown when e.button == 0:
                    brush.BeginStroke();
                    brush.Paint(hit);
                    e.Use();
                    break;

                case EventType.MouseDrag when e.button == 0:
                    if (brushSettings.brushMode == BrushMode.Paint || 
                        brushSettings.brushMode == BrushMode.Eraser)
                    {
                        brush.Paint(hit);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp when e.button == 0:
                    brush.EndStroke();
                    e.Use();
                    break;
            }
        }

        #region Visualization Methods

        private void DrawScatterPreview(Vector3 center)
        {
            int previewCount = Mathf.RoundToInt(brushSettings.density * Mathf.PI * brushSettings.radius * brushSettings.radius);
            previewCount = Mathf.Min(previewCount, 50); // Cap preview points

            for (int i = 0; i < previewCount; i++)
            {
                float angle = (float)(scatterRng.NextDouble() * Mathf.PI * 2);
                float distance = Mathf.Sqrt((float)scatterRng.NextDouble()) * brushSettings.radius;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * distance,
                    30f,
                    Mathf.Sin(angle) * distance
                );

                Vector3 pos = center + offset;
                if (Physics.Raycast(pos, Vector3.down, out RaycastHit sampleHit, 60f))
                {
                    bool validSlope = Vector3.Angle(Vector3.up, sampleHit.normal) <= brushSettings.maxSlopeAngle;
                    bool validDensity = true;
                    
                    if (brushSettings.densitySettings.checkDensity)
                    {
                        float checkRadius = brushSettings.densitySettings.preventOverlap ? 
                            brushSettings.densitySettings.largeObjectRadius : 
                            brushSettings.densitySettings.minDistance;
                            
                        validDensity = brush.CheckDensity(sampleHit.point, checkRadius);
                    }

                    Color previewColor = validSlope && validDensity ? 
                        new Color(0f, 1f, 0f, 0.35f) : 
                        new Color(1f, 0f, 0f, 0.35f);

                    Handles.color = previewColor;
                    Handles.SphereHandleCap(0, sampleHit.point, Quaternion.identity, 0.3f, EventType.Repaint);
                }
            }
            
            // Reset RNG for consistent preview
            scatterRng = new System.Random(scatterSeed);
        }

        private void DrawDensityVisualization(Vector3 center)
        {
            if (!brushSettings.densitySettings.checkDensity) return;

            float checkRadius = brushSettings.densitySettings.preventOverlap
                ? brushSettings.densitySettings.largeObjectRadius
                : brushSettings.densitySettings.minDistance;

            Handles.color = new Color(1f, 1f, 0f, 0.25f);
            Handles.DrawSolidDisc(center, Vector3.up, checkRadius);

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(center, Vector3.up, checkRadius);
        }

        private void DrawProjectedGrid(Vector3 center)
        {
            Handles.color = new Color(1f, 1f, 1f, 0.15f);
            float cellSize = 1f;
            int gridCount = 5;

            for (int x = -gridCount; x <= gridCount; x++)
            {
                for (int z = -gridCount; z <= gridCount; z++)
                {
                    Vector3 gridPos = center + new Vector3(x * cellSize, 30f, z * cellSize);
                    if (Physics.Raycast(gridPos, Vector3.down, out RaycastHit hit))
                    {
                        Handles.DrawAAPolyLine(2f,
                            hit.point + Vector3.left * 0.05f,
                            hit.point + Vector3.right * 0.05f
                        );
                        Handles.DrawAAPolyLine(2f,
                            hit.point + Vector3.forward * 0.05f,
                            hit.point + Vector3.back * 0.05f
                        );
                    }
                }
            }
        }

        private void DrawSlopeHeatmap(Vector3 center, float radius, float maxSlope)
        {
            int segments = 36;
            float step = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angleA = step * i;
                float angleB = step * (i + 1);

                Quaternion rotA = Quaternion.Euler(0f, angleA, 0f);
                Quaternion rotB = Quaternion.Euler(0f, angleB, 0f);

                Vector3 edgeA = center + rotA * Vector3.forward * radius + Vector3.up * 30f;
                Vector3 edgeB = center + rotB * Vector3.forward * radius + Vector3.up * 30f;

                if (Physics.Raycast(edgeA, Vector3.down, out RaycastHit hitA))
                {
                    float slopeA = Vector3.Angle(Vector3.up, hitA.normal);
                    Color colorA = GetSlopeColor(slopeA, maxSlope);

                    if (Physics.Raycast(edgeB, Vector3.down, out RaycastHit hitB))
                    {
                        float slopeB = Vector3.Angle(Vector3.up, hitB.normal);
                        Color colorB = GetSlopeColor(slopeB, maxSlope);

                        Handles.color = Color.Lerp(colorA, colorB, 0.5f);
                        Handles.DrawAAPolyLine(3f, hitA.point, hitB.point);
                    }
                }
            }
        }

        private Color GetSlopeColor(float slopeAngle, float maxSlope)
        {
            float t = Mathf.Clamp01(slopeAngle / maxSlope);
            return Color.Lerp(Color.green, Color.red, t);
        }

        #endregion
    }
}