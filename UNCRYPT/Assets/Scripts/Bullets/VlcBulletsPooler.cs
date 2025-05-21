using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Bullets
{
    public class VlcBulletsPooler : SceneSingleton<VlcBulletsPooler>
    {
        [SerializeField] private Bullet bulletPrefab;
        private ObjectPool<Bullet> _bulletsPool;

        private void Start()
        {
            _bulletsPool = new ObjectPool<Bullet>(
                () =>
                {
                    Bullet bullet = Instantiate(bulletPrefab, transform);
                    bullet.SetPool(_bulletsPool);
                    return bullet;
                },
                bullet =>
                {
                    bullet.gameObject.SetActive(true);
                    bullet.transform.SetParent(transform);
                },
                bullet => bullet.gameObject.SetActive(false),
                bullet => Destroy(bullet.gameObject),
                false,
                10
            );
        }

        public Bullet GetBullet() => _bulletsPool.Get();
    }
}