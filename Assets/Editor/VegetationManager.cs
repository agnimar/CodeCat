using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace VegetationPainter
{
    public class VegetationManager
    {
        private const float DEFAULT_GRID_SIZE = 5f;
        
        private readonly GameObject vegetationRoot;
        private readonly VegetationGrid spatialGrid;
        private VegetationPainterData currentData;
        private string lastLoadedPath;

        public VegetationManager(GameObject root, VegetationPainterData data)
        {
            vegetationRoot = root;
            currentData = data;
            spatialGrid = new VegetationGrid(DEFAULT_GRID_SIZE);

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

        public void SaveToAsset(string assetPath = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                assetPath = EditorUtility.SaveFilePanelInProject(
                    "Save Vegetation Data",
                    "VegetationData",
                    "asset",
                    "Choose location to save vegetation data"
                );

                if (string.IsNullOrEmpty(assetPath))
                    return;
            }

            VegetationPainterData data = AssetDatabase.LoadAssetAtPath<VegetationPainterData>(assetPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<VegetationPainterData>();
                AssetDatabase.CreateAsset(data, assetPath);
            }

            SaveVegetationToData(data);
            lastLoadedPath = assetPath;
            currentData = data;
        }

        private void SaveVegetationToData(VegetationPainterData data)
        {
            if (data == null || vegetationRoot == null) return;

            data.placements.Clear();
            foreach (Transform child in vegetationRoot.transform)
            {
                if (child == null) continue;

                VegetationData placement = new VegetationData
                {
                    prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject),
                    position = child.position,
                    rotation = child.rotation,
                    scale = child.localScale,
                    category = child.gameObject.name
                };
                
                data.placements.Add(placement);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }

        public void LoadFromAsset(string assetPath = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                string path = EditorUtility.OpenFilePanel(
                    "Load Vegetation Data",
                    "Assets",
                    "asset"
                );
                
                if (string.IsNullOrEmpty(path)) return;

                // Convert absolute path to project-relative path
                if (path.StartsWith(Application.dataPath))
                {
                    assetPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    Debug.LogError("Please select a file inside the Assets folder");
                    return;
                }
            }

            VegetationPainterData data = AssetDatabase.LoadAssetAtPath<VegetationPainterData>(assetPath);
            if (data == null)
            {
                Debug.LogError($"Failed to load vegetation data from: {assetPath}");
                return;
            }

            LoadVegetationFromData(data);
        }

        private void LoadVegetationFromData(VegetationPainterData data)
        {
            if (data == null || vegetationRoot == null) return;

            Undo.RegisterFullObjectHierarchyUndo(vegetationRoot, "Load Vegetation Data");
            Clear();

            foreach (VegetationData placement in data.placements)
            {
                if (placement.prefab == null) continue;

                GameObject instance = PrefabUtility.InstantiatePrefab(placement.prefab) as GameObject;
                if (instance == null) continue;

                instance.transform.SetParent(vegetationRoot.transform);
                instance.transform.position = placement.position;
                instance.transform.rotation = placement.rotation;
                instance.transform.localScale = placement.scale;
                    
                spatialGrid.RegisterObject(instance);
            }

            lastLoadedPath = data.name;
            currentData = data;
        }

        public void SaveToCurrent()
        {
            if (!string.IsNullOrEmpty(lastLoadedPath))
                SaveToAsset(lastLoadedPath);
            else
                SaveToAsset();
        }

        public void Clear()
        {
            if (vegetationRoot == null) return;

            Undo.RegisterFullObjectHierarchyUndo(vegetationRoot, "Clear Vegetation");
            spatialGrid.Clear();
            
            while (vegetationRoot.transform.childCount > 0)
            {
                Object.DestroyImmediate(vegetationRoot.transform.GetChild(0).gameObject);
            }

            if (currentData != null)
                EditorUtility.SetDirty(currentData);
        }

        public void Cleanup()
        {
            spatialGrid?.Cleanup();
        }
    }
}