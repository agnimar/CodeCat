using UnityEngine;
using System;
using System.Collections.Generic;

namespace VegetationPainter
{

    [Serializable]
    public class VegetationData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public string category;
    }

    [CreateAssetMenu(fileName = "VegetationPainterData", menuName = "Vegetation Painter/Data")]
    public class VegetationPainterData : ScriptableObject
    {
        public List<VegetationData> placements = new List<VegetationData>();
        
        public void AddPlacement(VegetationData data) => placements.Add(data);
        public void RemovePlacement(int index) 
        {
            if (index >= 0 && index < placements.Count)
                placements.RemoveAt(index);
        }
        public void Clear() => placements.Clear();
    }
}