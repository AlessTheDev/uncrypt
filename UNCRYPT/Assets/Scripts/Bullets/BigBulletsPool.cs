using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Bullets
{
    public class BigBulletsPool : SceneSingleton<BigBulletsPool>
    {
        [SerializeField] private Bullet bulletPrefab;
        private ObjectPool<Bullet> _bulletPool;
        
        private void Start()
        {
            _bulletPool = new ObjectPool<Bullet>(
                () =>
                {
                    Bullet b = Instantiate(bulletPrefab, transform);
                    b.SetPool(_bulletPool);
                    return b;
                },
                bullet =>
                {
                    bullet.gameObject.SetActive(true);
                },
                bullet => bullet.gameObject.SetActive(false),
                bullet => Destroy(bullet.gameObject),
                true,
                10
            );
        }

        public Bullet GetBullet() => _bulletPool.Get();
    }
}