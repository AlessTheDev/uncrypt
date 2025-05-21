using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Object = System.Object;

namespace Bullets
{
    public class WinrarBomb : MonoBehaviour
    {
        [SerializeField] private float damage = 20;
        [SerializeField] private ParticleSystem impactExplosion;
        [SerializeField] private GameObject visual;
        [SerializeField] private Collider col;
        [SerializeField] private AudioSource fallAudioSource;
        [SerializeField] private AudioSource explosionAudioSource;

        private ObjectPool<Rigidbody> _pool;

        private void OnEnable()
        {
            visual.SetActive(true);
            col.enabled = true;
            
            fallAudioSource.Play();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                GameManager.Instance.Player.Damage(damage);
            }
            else if (other.gameObject.layer != LayerMask.NameToLayer("Terrain"))
            {
                return;
            }

            CameraManager.Instance.SmallShake();
            visual.SetActive(false);
            col.enabled = false;
            StartCoroutine(PlayExplosionAndRelease());
        }

        private IEnumerator PlayExplosionAndRelease()
        {
            explosionAudioSource.Play();
            impactExplosion.Play();
            yield return new WaitForSeconds(impactExplosion.main.duration);
            _pool.Release(GetComponent<Rigidbody>());
        }

        public void SetPool(ObjectPool<Rigidbody> pool)
        {
            _pool = pool;
        }
    }
}