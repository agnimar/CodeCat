using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VegetationPainter
{
    public class VegetationBrush
    {
        private readonly BrushSettings settings;
        private readonly GameObject vegetationRoot;
        private readonly VegetationGrid spatialGrid;

        // Track painting strokes for Undo
        private int currentUndoGroup = -1;
        private bool isPainting;
        private Vector3 lastPaintPosition;

        public VegetationBrush(BrushSettings settings, GameObject root)
        {
            this.settings = settings;
            this.vegetationRoot = root;
            this.spatialGrid = new VegetationGrid(5f);
            
            // Register existing vegetation
            if (root != null)
            {
                foreach (Transform child in root.transform)
                {
                    if (child != null)
                        spatialGrid.RegisterObject(child.gameObject);
                }
            }
        }
private void PaintMode(RaycastHit hit)
{
    
    // If we've moved MORE than the minimum spacing, allow placement
    if (lastPaintPosition != Vector3.zero)
    {
        float dist = Vector3.Distance(hit.point, lastPaintPosition);
        Debug.Log($"Distance from last paint: {dist}, Minimum spacing: {settings.spacing}");
        if (dist >= settings.spacing)  // Changed from < to >= 
        {
            Debug.Log("Distance sufficient, placing vegetation");
            if (!TryPlaceVegetation(hit)) return;
            lastPaintPosition = hit.point;
            Debug.Log($"Successfully placed vegetation at {hit.point}");
        }
    }
    else  // First plant - always place
    {
        Debug.Log("First placement");
        if (!TryPlaceVegetation(hit)) return;
        lastPaintPosition = hit.point;
        Debug.Log($"Successfully placed first vegetation at {hit.point}");
    }
}

private bool TryPlaceVegetation(RaycastHit hit)
{
    // Check density settings
    if (settings.densitySettings.checkDensity)
    {
        float checkRadius = settings.densitySettings.preventOverlap ? 
            settings.densitySettings.largeObjectRadius : 
            settings.densitySettings.minDistance;

        Debug.Log($"Checking overlap with radius: {checkRadius}");
        if (!spatialGrid.CheckOverlap(hit.point, checkRadius))
        {
            Debug.Log("Overlap check failed");
            return false;
        }
    }

    Debug.Log("All checks passed, placing vegetation");
    PlaceVegetation(hit);
    return true;
}



        public void BeginStroke()
        {
            if (isPainting) return;
            isPainting = true;
            lastPaintPosition = Vector3.zero;

            currentUndoGroup = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Paint Vegetation Stroke");
        }

        public void EndStroke()
        {
            if (!isPainting) return;
            isPainting = false;

            if (currentUndoGroup != -1)
            {
                Undo.CollapseUndoOperations(currentUndoGroup);
                currentUndoGroup = -1;
            }
        }

        public void Paint(RaycastHit hit)
        {
            if (!IsValidPlacement(hit)) return;

            switch (settings.brushMode)
            {
                case BrushMode.Paint:
                    PaintMode(hit);
                    break;
                case BrushMode.Stamp:
                    StampMode(hit);
                    break;
                case BrushMode.Scatter:
                    ScatterMode(hit);
                    break;
                case BrushMode.Eraser:
                    EraserMode(hit);
                    break;
            }
        }

        private bool IsValidPlacement(RaycastHit hit)
        {
            if (!isPainting) return false;
            if (settings.prefabs.Count == 0) return false;
            return Vector3.Angle(Vector3.up, hit.normal) <= settings.maxSlopeAngle;
        }

        private void StampMode(RaycastHit hit)
        {
            PlaceVegetation(hit);
        }

private void ScatterMode(RaycastHit hit)
{
    float area = Mathf.PI * settings.radius * settings.radius;
    // Make density more granular at lower values (0-1) but still allow higher values
    float adjustedDensity = settings.density <= 1f ? 
        settings.density * 0.05f : // Fine control for low values
        settings.density * 0.1f;   // Normal scaling for higher values
    
    int count = Mathf.RoundToInt(adjustedDensity * area);
    // Ensure at least 1 object for very low densities if area is reasonable
    if (count == 0 && settings.density > 0 && settings.radius > 1f)
        count = 1;
    
    for (int i = 0; i < count; i++)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * settings.radius;
        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * distance,
            30f,
            Mathf.Sin(angle) * distance
        );

        Vector3 checkPoint = hit.point + offset;
        if (Physics.Raycast(checkPoint, Vector3.down, out RaycastHit scatterHit, 60f))
        {
            if (Vector3.Angle(Vector3.up, scatterHit.normal) <= settings.maxSlopeAngle)
            {
                TryPlaceVegetation(scatterHit);
            }
        }
    }
}

        private void EraserMode(RaycastHit hit)
        {
            var objectsInRadius = spatialGrid.GetObjectsInRadius(hit.point, settings.radius);
            foreach (var obj in objectsInRadius)
            {
                if (obj != null)
                {
                    Undo.DestroyObjectImmediate(obj);
                    spatialGrid.UnregisterObject(obj);
                }
            }
        }

private bool  PlaceVegetation(RaycastHit hit)
{

        if (settings.densitySettings.checkDensity)
    {
        float checkRadius = settings.densitySettings.preventOverlap ? 
            settings.densitySettings.largeObjectRadius : 
            settings.densitySettings.minDistance;

        if (!spatialGrid.CheckOverlap(hit.point, checkRadius))
            return false;
    }

    GameObject prefab = SelectPrefab();
    if (!prefab) return false;

    GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
    Undo.RegisterCreatedObjectUndo(instance, "Paint Vegetation");

    instance.transform.SetParent(vegetationRoot.transform);
    instance.transform.position = hit.point;
    ApplyTransformSettings(instance, hit);
    
    // Register BEFORE applying position jitter to ensure accurate spatial tracking
    spatialGrid.RegisterObject(instance);
    return true;
}

        

        private void ApplyTransformSettings(GameObject instance, RaycastHit hit)
        {
            var ts = settings.transformSettings;

            // Position jitter
            if (ts.positionJitter != Vector3.zero)
            {
                Vector3 jitter = new Vector3(
                    Random.Range(-ts.positionJitter.x, ts.positionJitter.x),
                    Random.Range(-ts.positionJitter.y, ts.positionJitter.y),
                    Random.Range(-ts.positionJitter.z, ts.positionJitter.z)
                );
                instance.transform.position += jitter;
            }

            // Base rotation
            Quaternion rotation = Quaternion.identity;
            if (settings.alignToNormal)
            {
                rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                
                // Add random tilt
                if (ts.maxTiltAngle > 0f)
                {
                    Vector3 randomTilt = Random.insideUnitSphere * ts.maxTiltAngle;
                    rotation *= Quaternion.Euler(randomTilt);
                }
            }

            // Advanced rotation
            if (ts.useAdvancedRotation)
            {
                Vector3 randomRotation = new Vector3(
                    Random.Range(-ts.rotationRangePerAxis.x, ts.rotationRangePerAxis.x),
                    Random.Range(-ts.rotationRangePerAxis.y, ts.rotationRangePerAxis.y),
                    Random.Range(-ts.rotationRangePerAxis.z, ts.rotationRangePerAxis.z)
                );
                rotation *= Quaternion.Euler(randomRotation);
            }
            else
            {
                rotation *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            instance.transform.rotation = rotation;

            // Scale
            if (ts.useNonUniformScale)
            {
                instance.transform.localScale = new Vector3(
                    Random.Range(ts.minScale.x, ts.maxScale.x),
                    Random.Range(ts.minScale.y, ts.maxScale.y),
                    Random.Range(ts.minScale.z, ts.maxScale.z)
                );
            }
            else
            {
                float uniformScale = Random.Range(ts.minScale.x, ts.maxScale.x);
                instance.transform.localScale = Vector3.one * uniformScale;
            }
        }

        private GameObject SelectPrefab()
        {
            if (settings.weights.Count == settings.prefabs.Count)
            {
                float totalWeight = 0f;
                foreach (float w in settings.weights) totalWeight += w;
                float random = Random.Range(0f, totalWeight);
                
                float cumulative = 0f;
                for (int i = 0; i < settings.weights.Count; i++)
                {
                    cumulative += settings.weights[i];
                    if (random <= cumulative)
                        return settings.prefabs[i];
                }
            }
            
            return settings.prefabs[Random.Range(0, settings.prefabs.Count)];
        }

        public bool CheckDensity(Vector3 position, float checkRadius)
        {
            return spatialGrid.CheckOverlap(position, checkRadius);
        }

        public void DrawPreview(RaycastHit hit)
        {
            float r = settings.radius;
            bool validSlope = Vector3.Angle(Vector3.up, hit.normal) <= settings.maxSlopeAngle;

            // Draw brush circle
            Color brushColor = settings.brushMode == BrushMode.Eraser
                ? new Color(0.8f, 0.2f, 0.2f, 0.25f)  // Red for eraser
                : (validSlope ? new Color(0.2f, 0.8f, 0.2f, 0.25f) : new Color(0.8f, 0.2f, 0.2f, 0.25f));

            Handles.color = brushColor;
            Handles.DrawSolidDisc(hit.point, hit.normal, r);

            Handles.color = Color.white;
            Handles.DrawWireDisc(hit.point, hit.normal, r);

            // Draw normal indicator
            if (settings.alignToNormal)
            {
                Handles.color = Color.yellow;
                float length = r * 0.3f;
                Vector3 end = hit.point + hit.normal * length;
                Handles.DrawAAPolyLine(3f, hit.point, end);
            }

            // Draw minimum spacing preview for paint mode
            if (settings.brushMode == BrushMode.Paint)
            {
                Handles.color = new Color(1f, 1f, 0f, 0.15f);
                Handles.DrawWireDisc(hit.point, hit.normal, settings.spacing);
            }
        }
    }
}