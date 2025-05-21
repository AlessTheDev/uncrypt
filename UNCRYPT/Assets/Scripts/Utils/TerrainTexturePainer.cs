#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
namespace Utils
{
    [ExecuteInEditMode]
    public class TerrainTexturePainter : MonoBehaviour
    {
        public Terrain terrain;
        public float heightThreshold = 10f;

        public int belowThresholdTexture = 0;
        public int navMeshTexture = 1;
        public int offNavMeshTexture = 2;

        public bool debugPainting = false;
        public float navMeshCheckRadius = 0.5f;

        public void PaintTerrain()
        {
            if (terrain == null)
            {
                Debug.LogError("No terrain assigned!");
                return;
            }
            
            TerrainData terrainData = terrain.terrainData;
            int width = terrainData.alphamapWidth;
            int height = terrainData.alphamapHeight;
            int layers = terrainData.alphamapLayers;
            
            if (belowThresholdTexture >= layers || navMeshTexture >= layers || offNavMeshTexture >= layers)
            {
                Debug.LogError("Texture index out of range! Please check that your terrain has enough textures assigned.");
                return;
            }

            float[,,] splatmapData = new float[width, height, layers];
            
            // Calculate terrain scale factors
            float terrainWidth = terrainData.size.x;
            float terrainLength = terrainData.size.z;
            float terrainHeight = terrainData.size.y;
            
            Vector3 terrainPosition = terrain.transform.position;
            
            int belowThresholdCount = 0;
            int navMeshCount = 0;
            int offNavMeshCount = 0;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Convert alpha map coordinates to world position
                    float worldX = terrainPosition.x + ((float)x / width) * terrainWidth;
                    float worldZ = terrainPosition.z + ((float)y / height) * terrainLength;
                    
                    // Sample height at this position
                    float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPosition.y;
                    Vector3 worldPos = new Vector3(worldX, worldY, worldZ);
                    
                    // Clear the weights array
                    for (int i = 0; i < layers; i++)
                    {
                        splatmapData[y, x, i] = 0;
                    }
                    
                    // Determine which texture to use
                    if (worldY < heightThreshold + terrainPosition.y)
                    {
                        splatmapData[y, x, belowThresholdTexture] = 1f;
                        belowThresholdCount++;
                    }
                    else
                    {
                        // Check if on NavMesh
                        bool isOnNavMesh = NavMesh.SamplePosition(worldPos, out NavMeshHit hit, navMeshCheckRadius, NavMesh.AllAreas);
                        
                        if (isOnNavMesh)
                        {
                            splatmapData[y, x, navMeshTexture] = 1f;
                            navMeshCount++;
                        }
                        else
                        {
                            splatmapData[y, x, offNavMeshTexture] = 1f;
                            offNavMeshCount++;
                        }
                    }
                }
            }
            
            // Apply the splatmap to the terrain
            terrainData.SetAlphamaps(0, 0, splatmapData);
            
            if (debugPainting)
            {
                Debug.Log($"Terrain painting complete:\n" +
                          $"Below threshold: {belowThresholdCount} pixels\n" +
                          $"On NavMesh: {navMeshCount} pixels\n" +
                          $"Off NavMesh: {offNavMeshCount} pixels");
            }
        }
    }

    [CustomEditor(typeof(TerrainTexturePainter))] 
    public class TerrainTexturePainterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainTexturePainter script = (TerrainTexturePainter)target;
            
            EditorGUILayout.LabelField("Terrain Texture Painter", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Paint Terrain"))
            {
                Undo.RecordObject(script.terrain.terrainData, "Paint Terrain Textures");
                script.PaintTerrain();
            }
            
            DrawDefaultInspector();
        }
    }
}
#endif
