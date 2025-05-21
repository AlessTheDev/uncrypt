using System.Collections;
using Companion;
using Companion.States;
using DG.Tweening;
using DialogueSystem;
using NPCs;
using Player;
using TMPro;
using UI.FileSystemPuzzle;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FileSystemLevel
{
    // Change these serializable classes to use fields instead of properties
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public Enemy.Enemy Enemy;
        public int Number;
    }
    
    [System.Serializable]
    public class EnemyWave
    {
        public EnemySpawnInfo[] Spawns;
    }
    
    public class ArenaController : MonoBehaviour
    {
        [SerializeField] private EnemyWave[] enemyWaves;
        [SerializeField] private BoxCollider area;
        [SerializeField] private AdministratorPermissionsUI adminPermsUI;
        [SerializeField] private FileSystemPuzzleUI puzzleUI;
        [SerializeField] private ArenaGuard[] guards;
        [SerializeField] private CompanionController companion;
        [SerializeField] private Canvas arenaCanvas;
        [SerializeField] private TextMeshProUGUI enemiesLeftText;
        [SerializeField] private FileSystemController fileSystemController;

        private int _currentWave;
        
        private bool _isArenaActive;

        private int _currentlySpawnedEnemies;
        private void Awake()
        {
            _isArenaActive = false;
            arenaCanvas.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isArenaActive) return;
            
            if (other.gameObject.layer == PlayerController.Layer)
            {
                _isArenaActive = true;
                companion.StateMachine.TransitionToState<FollowPlayerState>();
                
                StartCoroutine(StartBattles());
            }
        }

        private IEnumerator StartBattles()
        {
            StartCoroutine(fileSystemController.SwitchToBattleTheme());
            
            arenaCanvas.enabled = true;
            CanvasGroup canvasGroup = arenaCanvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1, 1).SetEase(Ease.InQuad);
            
            foreach (var arenaGuard in guards)
            {
                arenaGuard.BlockPassage();
            }
            
            yield return new WaitForSeconds(5);
            
            _currentWave = 0;
            GameManager.Instance.Player.DisableMovement();
            yield return adminPermsUI.ShowCoroutine();
            GameManager.Instance.Player.EnableMovement();

            SpawnEnemies(_currentWave);
        }

        private void SpawnEnemies(int wave)
        {
            float y = GameManager.Instance.PlayerTransform.position.y;
            
            EnemySpawnInfo[] enemiesToSpawn = enemyWaves[wave].Spawns;

            foreach (var enemy in enemiesToSpawn)
            {
                for (int i = 0; i < enemy.Number; i++)
                {
                    Vector3 randomPos;
                    Vector3 playerPos = GameManager.Instance.PlayerTransform.position;

                    do
                    {
                        randomPos = new Vector3(
                            Random.Range(area.bounds.min.x, area.bounds.max.x),
                            y,
                            Random.Range(area.bounds.min.z, area.bounds.max.z)
                        );
                    } while (Utils.Utilities.RadialCheck(randomPos, playerPos, 4));
                    
                    Enemy.Enemy e = Instantiate(enemy.Enemy, randomPos, Quaternion.identity);
                    _currentlySpawnedEnemies++;
                    
                    e.OnDeath.AddListener((_) =>
                    {
                        _currentlySpawnedEnemies--;
                        UpdateEnemiesText();
                        
                        if (_currentlySpawnedEnemies == 0)
                        {
                            StartCoroutine(OnWaveFinished());
                        }
                    });
                }
            }
            UpdateEnemiesText();
        }

        private void UpdateEnemiesText()
        {
            enemiesLeftText.text = "Enemies Left: " + _currentlySpawnedEnemies;
        }

        private IEnumerator OnWaveFinished()
        {
            _currentWave++;
            if (_currentWave >= enemyWaves.Length)
            {
                GameManager.Instance.Player.DisableMovement();
                
                yield return adminPermsUI.ShowCoroutine(true);
                
                yield return new WaitForSeconds(3f);

                StartCoroutine(fileSystemController.AudioFadeOut(1.3f));
                
                yield return DialogueManager.Instance.ParseDialogue("arena_completed_1");
                
                puzzleUI.gameObject.SetActive(true);
                
                fileSystemController.StartPuzzleTheme();
                
                yield break;
            }
            
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.Player.DisableMovement();
            yield return adminPermsUI.ShowCoroutine();
            GameManager.Instance.Player.EnableMovement();

            yield return new WaitForSeconds(3);

            SpawnEnemies(_currentWave);
        }
    }
    
#if UNITY_EDITOR
[CustomEditor(typeof(ArenaController))]
public class ArenaControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // Draw the default inspector except for the waves
        DrawPropertiesExcluding(serializedObject, "enemyWaves");
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Enemy Waves", EditorStyles.boldLabel);
        
        // Get the enemy waves property
        SerializedProperty wavesProperty = serializedObject.FindProperty("enemyWaves");
        
        // Add wave button
        if (GUILayout.Button("Add Wave"))
        {
            wavesProperty.arraySize++;
            
            // Initialize the new wave's Spawns array
            var newWave = wavesProperty.GetArrayElementAtIndex(wavesProperty.arraySize - 1);
            var spawnsArray = newWave.FindPropertyRelative("Spawns");
            spawnsArray.arraySize = 0;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        // Draw each wave
        for (int waveIndex = 0; waveIndex < wavesProperty.arraySize; waveIndex++)
        {
            SerializedProperty waveProp = wavesProperty.GetArrayElementAtIndex(waveIndex);
            SerializedProperty spawnsProp = waveProp.FindPropertyRelative("Spawns");
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Wave header with remove button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Wave {waveIndex + 1}", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Remove Wave", GUILayout.Width(100)))
            {
                wavesProperty.DeleteArrayElementAtIndex(waveIndex);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            // Add enemy type button
            if (GUILayout.Button("Add Enemy Type"))
            {
                spawnsProp.arraySize++;
                serializedObject.ApplyModifiedProperties();
            }
            
            // Draw each enemy spawn
            for (int spawnIndex = 0; spawnIndex < spawnsProp.arraySize; spawnIndex++)
            {
                SerializedProperty spawnProp = spawnsProp.GetArrayElementAtIndex(spawnIndex);
                SerializedProperty enemyProp = spawnProp.FindPropertyRelative("Enemy");
                SerializedProperty numberProp = spawnProp.FindPropertyRelative("Number");
                
                EditorGUILayout.BeginHorizontal();
                
                // Enemy field
                EditorGUILayout.PropertyField(enemyProp, GUIContent.none, GUILayout.MinWidth(200));
                
                // Add a label and use IntField for the quantity display
                GUILayout.Label("Quantity:", GUILayout.Width(60));
                numberProp.intValue = EditorGUILayout.IntField(numberProp.intValue, GUILayout.Width(50));
                
                // Set minimum value to 1
                if (numberProp.intValue < 1)
                    numberProp.intValue = 1;
                
                // Remove button
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    spawnsProp.DeleteArrayElementAtIndex(spawnIndex);
                    serializedObject.ApplyModifiedProperties();
                    break; // Use break instead of return to avoid exiting the whole method
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
}