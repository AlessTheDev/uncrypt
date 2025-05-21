using UnityEngine;
using UnityEngine.Pool;

namespace Bullets
{
    public class LightningPool : SceneSingleton<LightningPool>
    {
        [SerializeField] private Lightning lightningPrefab;
        private ObjectPool<Lightning> _bulletPool;
        
        private void Start()
        {
            _bulletPool = new ObjectPool<Lightning>(
                () =>
                {
                    Lightning l = Instantiate(lightningPrefab);
                    l.SetPool(_bulletPool);
                    
                    return l;
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

        public Lightning GetLightning() => _bulletPool.Get();
    }
}