using UnityEngine;
using UnityEngine.Pool;

namespace Bullets
{
    public class WinrarBombsPool : SceneSingleton<WinrarBombsPool>
    {
        [SerializeField] private Rigidbody bulletPrefab;
        private ObjectPool<Rigidbody> _bulletPool;
        
        private void Start()
        {
            _bulletPool = new ObjectPool<Rigidbody>(
                () =>
                {
                    Rigidbody b = Instantiate(bulletPrefab, transform);
                    b.GetComponent<WinrarBomb>().SetPool(_bulletPool);
                    b.linearVelocity = Vector3.zero;
                    b.angularVelocity = Vector3.zero;
                    
                    return b;
                },
                bullet =>
                {
                    bullet.transform.rotation = Quaternion.identity;
                    bullet.gameObject.SetActive(true);
                },
                bullet => bullet.gameObject.SetActive(false),
                bullet => Destroy(bullet.gameObject),
                true,
                10
            );
        }

        public Rigidbody GetBomb() => _bulletPool.Get();
    }
}
