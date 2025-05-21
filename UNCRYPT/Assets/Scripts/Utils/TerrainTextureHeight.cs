#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Utils
{
    [ExecuteInEditMode]
    public class TerrainTextureHeight : MonoBehaviour
    {
        [System.Serializable]
        public class HeightTerrainLayer
        {
            public float minHeight;
            public float maxHeight;
            public float noiseScale = 0.1f;
            public bool isGrass = false;
        }

        public Terrain terrain;
        public HeightTerrainLayer[] heightLayers;
        public float blendStrength = 0.1f;

        public void PaintTerrain()
        {
            if (terrain == null) return;
        
            TerrainData terrainData = terrain.terrainData;
            float[,,] splatmapData = new float[terrainData.alphamapWidth, 
                terrainData.alphamapHeight, 
                terrainData.alphamapLayers];

            // Get heights 
            float[,] heights = terrainData.GetHeights(0, 0, 
                terrainData.heightmapResolution,
                terrainData.heightmapResolution);

            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrainData.alphamapWidth; x++)
                {
                    // Convert alphamap coordinates to heightmap coordinates
                    int heightmapY = Mathf.RoundToInt((float)y * terrainData.heightmapResolution / terrainData.alphamapHeight);
                    int heightmapX = Mathf.RoundToInt((float)x * terrainData.heightmapResolution / terrainData.alphamapWidth);
                
                    // Get height in world units
                    float height = heights[heightmapY, heightmapX] * terrainData.heightmapScale.y;

                    float[] weights = new float[heightLayers.Length];
                    float weightSum = 0;
                
                    Vector3 worldPos = new Vector3(
                        x * terrainData.size.x / terrainData.alphamapWidth + terrain.transform.position.x,
                        height,
                        y * terrainData.size.z / terrainData.alphamapHeight + terrain.transform.position.z
                    );

                    bool isOnNavMesh = NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas);

                    for (int i = 0; i < heightLayers.Length; i++)
                    {
                        bool layerCondition = height >= heightLayers[i].minHeight &&
                                              height <= heightLayers[i].maxHeight;
                        bool grassCondition = height <= 0.1f && heightLayers[i].isGrass && isOnNavMesh; // Navmesh condition
                        if ((layerCondition && !grassCondition) || grassCondition)
                        {
                            float noise = Mathf.PerlinNoise(
                                x * heightLayers[i].noiseScale, 
                                y * heightLayers[i].noiseScale
                            );
                    
                            weights[i] = 1 + (noise * blendStrength);
                            weightSum += weights[i];
                        }
                    }

                    for (int i = 0; i < heightLayers.Length; i++)
                    {
                        splatmapData[y, x, i] = weightSum > 0 ? weights[i] / weightSum : 0;
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    
        [CustomEditor(typeof(TerrainTextureHeight))] 
        public class TerrainTextureHeightEditor : Editor {
	
            public override void  OnInspectorGUI () {
                TerrainTextureHeight script = (TerrainTextureHeight)target;
                if(GUILayout.Button("Paint Terrain")) {
                    script.PaintTerrain();
                }
                DrawDefaultInspector();
            }
        }
    }
}
#endif