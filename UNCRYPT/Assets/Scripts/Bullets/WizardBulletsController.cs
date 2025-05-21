using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Bullets
{
    public class WizardBulletsController : MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private int bulletsCount;
        [SerializeField] private float radius;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float cooldown;
        [SerializeField] private float reloadTime;
        
        private ObjectPool<Bullet> _bulletPool;

        private Bullet[] _bullets;
        private float _time;

        private bool _enabled;

        private float _lastBulletTime;

        private int _bulletCount;
        private bool _isReloading;

        private void Start()
        {
            _enabled = false;
            _bullets = new Bullet[bulletsCount];
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
                    bullet.transform.SetParent(transform);
                },
                bullet => bullet.gameObject.SetActive(false),
                shape => Destroy(shape.gameObject),
                false,
                bulletsCount + 5
            );
            StartCoroutine(SpawnBullets());
        }

        private IEnumerator SpawnBullets()
        {
            _time = 0;

            _isReloading = true;
            float waitTime = reloadTime / (bulletsCount + 1);
            for (int i = 0; i < bulletsCount; i++)
            {
                yield return new WaitForSeconds(waitTime);
                _bullets[i] = _bulletPool.Get();
                _bulletCount++;
            }
            yield return new WaitForSeconds(waitTime);
            _isReloading = false;
        }

        private void Update()
        {
            float distance = Mathf.PI * 2 / bulletsCount;
            float phaseShift = _time;
            for (int i = 0; i < bulletsCount; i++)
            {
                Bullet bullet = _bullets[i];
                if (!bullet) continue;

                float phase = (distance * i) + phaseShift;
                bullet.transform.localPosition = new Vector3(Mathf.Sin(phase), 0, Mathf.Cos(phase)) * radius;
                bullet.transform.rotation = Quaternion.Euler(0, phase * Mathf.Rad2Deg, 0);
            }

            _time += Time.deltaTime * rotationSpeed;

            if (_enabled && !_isReloading && Time.time - _lastBulletTime >= cooldown)
            {
                for (int i = 0; i < bulletsCount; i++)
                {
                    Bullet bullet = _bullets[i];
                    if (!bullet) continue;

                    if (bullet.RayIntersectsPlayer)
                    {
                        bullet.Launch();
                        _bullets[i] = null;
                        _bulletCount--;
                        _lastBulletTime = Time.time;

                        if (_bulletCount == 0)
                        {
                            StartCoroutine(SpawnBullets());
                        }
                    }
                }
            }
        }

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        // ReSharper disable once ParameterHidesMember
        // ReSharper disable once InconsistentNaming
        public void SetEnabled(bool _enabled)
        {
            this._enabled = _enabled;
        }
    }
}