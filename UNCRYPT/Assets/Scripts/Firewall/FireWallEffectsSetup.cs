#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Firewall
{
    public class FireWallEffectsSetup : MonoBehaviour
    {
        [SerializeField] private GameObject fireWallPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private int spacing = 2;
        [SerializeField] private float wallHeightThreshold = 1f;
        [SerializeField] private float wallDist = 0.2f;
        [SerializeField] private float slope = 13f;
        [SerializeField] private Terrain terrain;
        [SerializeField] private LayerMask terrainLayer;

        private void Clear()
        {
            while(container.childCount > 0)
            {
                DestroyImmediate(container.GetChild(0).gameObject);
            }
        }
        private void SpawnFireWallsAlongTerrain()
        {
            TerrainData terrainData = terrain.terrainData;
            float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

            for (int z = 0; z < terrainData.heightmapResolution - spacing; z += spacing)
            {
                for (int x = 0; x < terrainData.heightmapResolution - spacing; x += spacing)
                {
                    if (IsWallLocation(heights, x, z, 5))
                    {
                        Vector3 fireWallPosition = terrain.transform.position;
                        fireWallPosition.x += x * terrainData.size.x / (terrainData.heightmapResolution - 1);
                        fireWallPosition.z += z * terrainData.size.z / (terrainData.heightmapResolution - 1);
                        fireWallPosition.y = terrain.SampleHeight(fireWallPosition) + 3f;
                        Transform fire = Instantiate(fireWallPrefab, fireWallPosition, Quaternion.identity).transform;
                        fire.SetParent(container);
                        AlignFireAlongWall(fire);
                    }
                }
            }
        }

        private void AlignFireAlongWall(Transform fireWall)
        {
            Vector3[] directionsToCheck = { fireWall.forward, fireWall.right, -fireWall.forward, -fireWall.right };

            float shorterDistance = Mathf.Infinity;
            RaycastHit closestHit = default;
            Vector3 wallDirection = Vector3.zero;

            for (int i = 0; i < directionsToCheck.Length; i++)
            {
                Physics.Raycast(fireWall.position, directionsToCheck[i], out RaycastHit hit, Mathf.Infinity, terrainLayer);
                float dist = hit.distance;
                if (dist < shorterDistance)
                {
                    shorterDistance = dist;
                    closestHit = hit;
                    wallDirection = directionsToCheck[i];
                }
            }

            fireWall.position = closestHit.point;
    
            fireWall.rotation = Quaternion.LookRotation(wallDirection, Vector3.up);
            fireWall.position += fireWall.forward * -wallDist;
            fireWall.Rotate(Vector3.right * slope);
        }

        private bool IsWallLocation(float[,] heights, int x, int z, int sampleSize)
        {
            float currentHeight = heights[z, x];
            float maxNeighborHeight = 0f;

            for (int i = -sampleSize; i <= sampleSize; i++)
            {
                for (int j = -sampleSize; j <= sampleSize; j++)
                {
                    if (z + i >= 0 && z + i < heights.GetLength(0) && x + j >= 0 && x + j < heights.GetLength(1))
                    {
                        maxNeighborHeight = Mathf.Max(maxNeighborHeight, heights[z + i, x + j]);
                    }
                }
            }

            if (currentHeight > wallHeightThreshold)
            {
                return false;
            }

            return maxNeighborHeight - currentHeight > wallHeightThreshold;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(FireWallEffectsSetup))] 
        public class FireWallEffectsSetupEditor : Editor {
	
            public override void  OnInspectorGUI () {
                FireWallEffectsSetup script = (FireWallEffectsSetup)target;
                GUILayout.Label("WARNING: This will edit the scene!");
                if(GUILayout.Button("Spawn Fire Along Walls")) {
                    script.SpawnFireWallsAlongTerrain();
                }
                if(GUILayout.Button("Clear")) {
                    script.Clear();
                }
                DrawDefaultInspector();
            }
        }
#endif
    }
}