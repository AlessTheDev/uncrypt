#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using Random = UnityEngine.Random;

namespace FileSystemLevel
{
    public class PlaceFloatingObjects : MonoBehaviour
    {
        [SerializeField] private int maxIterations = 100;
        [SerializeField] private int spawnNumber = 100;
        [SerializeField] private FloatingObject floatingObject;
        [SerializeField] private Renderer waterContainer;
        [SerializeField] private LayerMask referenceTerrainLayer; // Terrain layer for collision checking
        
        private FloatingObject[] _floatingObjects;
        [SerializeField] private Transform[] objectVariants;

        private void Awake()
        {
            _floatingObjects = GetComponentsInChildren<FloatingObject>();
        }

        private void Spawn()
        {
            Bounds bounds = waterContainer.bounds;

            float centerX = bounds.center.x;
            float centerZ = bounds.center.z;
            float halfSizeX = (bounds.max.x - bounds.min.x) / 2f;
            float halfSizeZ = (bounds.max.z - bounds.min.z) / 2f;

            int placedObjects = 0;
            int iterations = 0;

            while (placedObjects < spawnNumber && iterations < maxIterations * spawnNumber)
            {
                iterations++;

                // Quadratic distribution biasing towards the center
                float randomX = centerX + (Random.value < 0.5f ? -1 : 1) * Mathf.Pow(Random.value, 2) * halfSizeX;
                float randomZ = centerZ + (Random.value < 0.5f ? -1 : 1) * Mathf.Pow(Random.value, 2) * halfSizeZ;

                Vector3 randomPos = new Vector3(
                    randomX,
                    transform.position.y,
                    randomZ
                );

                if (Physics.CheckSphere(randomPos, 1f, referenceTerrainLayer))
                {
                    continue; // Skip this position and try again
                }

                Transform newObject = Instantiate(floatingObject, randomPos, Quaternion.identity).transform;
                newObject.SetParent(transform, true);

                Transform variantObject = Instantiate(objectVariants[Random.Range(0, objectVariants.Length)], newObject);
                variantObject.localPosition = Vector3.zero;
                variantObject.localRotation = Quaternion.identity;



                placedObjects++;
            }

            if (placedObjects < spawnNumber)
            {
                Debug.LogWarning($"Spawned {placedObjects}/{spawnNumber} objects. Max iterations reached.");
            }
        }

        private void Clear()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void FixedUpdate()
        {
            foreach (FloatingObject obj in _floatingObjects)
            {
                obj.UpdateObject();
            }
        }

#if  UNITY_EDITOR
        [CustomEditor(typeof(PlaceFloatingObjects))]
        public class PlaceFloatingObjectsEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                PlaceFloatingObjects script = (PlaceFloatingObjects)target;

                GUILayout.Label("WARNING: This will edit the scene!", EditorStyles.boldLabel);

                if (GUILayout.Button("Spawn Floating Objects"))
                {
                    script.Spawn();
                }

                if (GUILayout.Button("Clear"))
                {
                    script.Clear();
                }

                DrawDefaultInspector();
            }
        }
#endif
    }
}
