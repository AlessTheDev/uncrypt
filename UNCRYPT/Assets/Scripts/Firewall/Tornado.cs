using System;
using Player;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Firewall
{
    public class Tornado : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float damage;
        [SerializeField] private float damageCooldown = 2;
        [SerializeField] private LayerMask terrainLayer;

        private float _lastDamageTime;

        private Vector3 _direction;
        
        private bool _isActive;

        private void Start()
        {
            GameManager.Instance.OnPlayerEntersSafeZone.AddListener(DeActivate);
            GameManager.Instance.OnPlayerExitsSafeZone.AddListener(Activate);
        }

        private void Activate()
        {
            _isActive = true;
        }
        
        private void DeActivate()
        {
            _isActive = false;
        }

        private void OnEnable()
        {
            _direction = transform.forward;
            _isActive = true;
        }

        private void FixedUpdate()
        {
            if(_isActive)
            {
                transform.position += _direction * (speed * Time.fixedDeltaTime);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (((1 << other.gameObject.layer) & terrainLayer) != 0)
            {
                _direction = Vector3.Reflect(_direction, other.contacts[0].normal);
                _direction += new Vector3(1, 0, 1) * UnityEngine.Random.Range(-.5f, .5f);

                _direction = new Vector3(_direction.x, 0, _direction.z).normalized;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                if (Time.time - _lastDamageTime > damageCooldown)
                {
                    _lastDamageTime = Time.time;
                    GameManager.Instance.Player.Damage(damage);
                }
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnPlayerEntersSafeZone.RemoveListener(DeActivate);
            GameManager.Instance.OnPlayerExitsSafeZone.RemoveListener(Activate);
        }
    }
}