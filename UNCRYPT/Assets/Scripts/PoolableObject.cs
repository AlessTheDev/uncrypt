using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolableObject : MonoBehaviour
{
    private ObjectPool<PoolableObject> _pool;
    public void SetPool(ObjectPool<PoolableObject> pool)
    {
        _pool = pool;
    }

    private void OnDisable()
    {
        _pool.Release(this);
    }
}
