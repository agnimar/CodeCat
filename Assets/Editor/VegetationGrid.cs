using UnityEngine;
using System.Collections.Generic;

namespace VegetationPainter
{
    public class VegetationGrid
    {
        private const float DEFAULT_CELL_SIZE = 5f;
        private readonly float cellSize;
        private readonly Dictionary<Vector2Int, HashSet<GameObject>> grid;
        private readonly Dictionary<GameObject, Vector2Int> objectToCell;

        public VegetationGrid(float cellSize = DEFAULT_CELL_SIZE)
        {
            this.cellSize = cellSize;
            grid = new Dictionary<Vector2Int, HashSet<GameObject>>();
            objectToCell = new Dictionary<GameObject, Vector2Int>();
        }

        private Vector2Int WorldToCell(Vector3 worldPos) => new Vector2Int(
            Mathf.FloorToInt(worldPos.x / cellSize),
            Mathf.FloorToInt(worldPos.z / cellSize)
        );

        public void RegisterObject(GameObject obj)
        {
            if (obj == null) return;

            Vector2Int cell = WorldToCell(obj.transform.position);
            
            if (!grid.TryGetValue(cell, out HashSet<GameObject> objects))
            {
                objects = new HashSet<GameObject>();
                grid[cell] = objects;
            }
            
            grid[cell].Add(obj);
            objectToCell[obj] = cell;
        }

        public void UnregisterObject(GameObject obj)
        {
            if (obj == null) return;

            if (objectToCell.TryGetValue(obj, out Vector2Int cell))
            {
                if (grid.TryGetValue(cell, out HashSet<GameObject> objects))
                {
                    objects.Remove(obj);
                    if (objects.Count == 0)
                    {
                        grid.Remove(cell);
                    }
                }
                objectToCell.Remove(obj);
            }
        }

        /// <summary>
        /// Checks if there is any object within <paramref name="overlapRadius"/> of <paramref name="position"/>.
        /// If an overlap is found, returns false; otherwise true.
        /// </summary>
        public bool CheckOverlap(Vector3 position, float overlapRadius)
        {
            Vector2Int centerCell = WorldToCell(position);
            int cellRadius = Mathf.CeilToInt(overlapRadius / cellSize);
            float overlapSqr = overlapRadius * overlapRadius;

            // Check neighboring cells within the overlap radius
            for (int x = -cellRadius; x <= cellRadius; x++)
            {
                for (int z = -cellRadius; z <= cellRadius; z++)
                {
                    Vector2Int checkCell = centerCell + new Vector2Int(x, z);
                    if (!grid.TryGetValue(checkCell, out HashSet<GameObject> objects)) 
                        continue;

                    foreach (GameObject obj in objects)
                    {
                        if (obj != null)
                        {
                            float distSqr = (obj.transform.position - position).sqrMagnitude;
                            if (distSqr < overlapSqr)
                            {
                                // There's an object within the overlapRadius
                                return false;
                            }
                        }
                    }
                }
            }
            // No overlap found
            return true;
        }

        public List<GameObject> GetObjectsInRadius(Vector3 position, float radius)
        {
            var result = new List<GameObject>();
            Vector2Int centerCell = WorldToCell(position);
            int cellRadius = Mathf.CeilToInt(radius / cellSize);
            float radiusSqr = radius * radius;

            for (int x = -cellRadius; x <= cellRadius; x++)
            {
                for (int z = -cellRadius; z <= cellRadius; z++)
                {
                    Vector2Int checkCell = centerCell + new Vector2Int(x, z);
                    if (!grid.TryGetValue(checkCell, out HashSet<GameObject> objects)) 
                        continue;

                    foreach (GameObject obj in objects)
                    {
                        if (obj != null &&
                            (obj.transform.position - position).sqrMagnitude <= radiusSqr)
                        {
                            result.Add(obj);
                        }
                    }
                }
            }
            return result;
        }

        public void Clear()
        {
            grid.Clear();
            objectToCell.Clear();
        }

        public void Cleanup()
        {
            var nullObjects = new List<GameObject>();
            foreach (var kvp in objectToCell)
            {
                if (kvp.Key == null)
                    nullObjects.Add(kvp.Key);
            }

            foreach (var obj in nullObjects)
                UnregisterObject(obj);
        }
    }
}
