using UnityEngine;

/// <summary>
/// This singleton is initialized on GameLoad and persist through the whole game.
/// If you try to create a new instance it will be destroyed.
/// </summary>
/// <typeparam name="T">The subclass</typeparam>
public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"[SINGLETON] Attempted to add more than one instance of {typeof(T).Name}");
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
        Debug.Log($"[SINGLETON] Initialized instance of {typeof(T).Name}");
        OnAwake();
    }

    protected virtual void OnAwake(){}
}