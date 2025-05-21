using System.Linq;
using DG.Tweening;
using DG.Tweening.Core.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Set all game objects that need to persist across scenes as child of this object.
/// TODO: Refactor the load system to work with save file's dependency
/// </summary>
public class GameBoostrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Load()
    {
        Debug.Log("[BOOSTRAP] Attempting to load GameBootstrap...");
        var bootstrapPrefab = Resources.Load<GameObject>("GameBootstrap");
        
        DOTween.safeModeLogBehaviour = SafeModeLogBehaviour.None;
#if UNITY_EDITOR
        DOTween.safeModeLogBehaviour = SafeModeLogBehaviour.Error;
#endif
        

        // Add debug information
        if (bootstrapPrefab == null)
        {
            Debug.LogError("[BOOSTRAP] GameBootstrap prefab not found in Resources folder!");
            return;
        }

        Debug.Log("[BOOSTRAP] Successfully loaded GameBootstrap");
        GameObject bootstrap = Instantiate(bootstrapPrefab);
        DontDestroyOnLoad(bootstrap);
    }
}