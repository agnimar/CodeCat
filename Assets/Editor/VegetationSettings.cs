using System;
using System.Collections.Generic;
using UnityEngine;

namespace VegetationPainter
{
    /// <summary>Different painting modes the brush supports.</summary>
    public enum BrushMode
    {
        Paint,
        Stamp,
        Scatter,
        Eraser
    }

    /// <summary>Transform-related settings (scaling, rotation, etc.).</summary>
    [Serializable]
    public class TransformSettings
    {
        public bool useAdvancedRotation = false;
        public Vector3 rotationRangePerAxis = new Vector3(0f, 360f, 0f);
        public bool useNonUniformScale = false;
        public Vector3 minScale = Vector3.one * 0.8f;
        public Vector3 maxScale = Vector3.one * 1.2f;
        public float maxTiltAngle = 15f;
        public Vector3 positionJitter = new Vector3(0.1f, 0f, 0.1f);
    }

    /// <summary>Density-related settings (min distance, prevent overlap, etc.).</summary>
    [Serializable]
    public class DensitySettings
    {
        public bool checkDensity = true;
        public float minDistance = 1f;
        public float largeObjectRadius = 2f;
        public bool preventOverlap = true;
    }

    /// <summary>Core brush settings that define how vegetation is placed.</summary>
    [Serializable]
    public class BrushSettings
    {
        // Basic brush fields
        public BrushMode brushMode = BrushMode.Paint;
        public float radius = 5f;
        public bool alignToNormal = true;
        public float maxSlopeAngle = 45f;

        // Per-mode specifics
        public float spacing = 2f;    // For Paint mode
        public float density = 0.1f;    // For Scatter mode

        // Prefab references (plus optional weighting)
        public List<GameObject> prefabs = new List<GameObject>();
        public List<float> weights = new List<float>();

        // Extra settings
        public TransformSettings transformSettings = new TransformSettings();
        public DensitySettings densitySettings = new DensitySettings();
    }
}
