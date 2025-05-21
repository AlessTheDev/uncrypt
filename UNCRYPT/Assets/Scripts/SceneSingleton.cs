using System;
using UnityEngine;

/// <summary>
/// This singleton can be different for every scene, so if a scene changes the Instance is updated
/// </summary>
/// <typeparam name="T">The subclass</typeparam>
public class SceneSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log($"[SINGLETON] Assigning new instance of type {typeof(T).Name}");
        }
        Instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake(){}

    private void OnDestroy()
    {
        Instance = null;
    }
    
    protected virtual void Clean(){}
}