using System;
using System.Collections;
using DialogueSystem;
using Player;
using Terminal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class GameManager : SceneSingleton<GameManager>
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject slashAttack;
    [SerializeField] private GameObject attackHitVfx;

    public PlayerController Player => player;
    public Transform PlayerTransform => player.transform;
    public Rigidbody PlayerRb { get; private set; }

    private ObjectPool<PoolableObject> _attacksPool;
    private ObjectPool<ParticleSystem> _attackParticlesPool;

    public UnityEvent OnPlayerEntersSafeZone;
    public UnityEvent OnPlayerExitsSafeZone;

    protected override void OnAwake()
    {
        PlayerRb = player.GetComponent<Rigidbody>();

        _attacksPool = new ObjectPool<PoolableObject>(
            () =>
            {
                PoolableObject b = Instantiate(slashAttack, transform.position, Quaternion.identity)
                    .GetComponent<PoolableObject>();
                b.SetPool(_attacksPool);
                return b;
            },
            slash => slash.gameObject.SetActive(true),
            slash => slash.gameObject.SetActive(false),
            slash => Destroy(slash.gameObject),
            true,
            4
        );

        _attackParticlesPool = new ObjectPool<ParticleSystem>(
            () =>
            {
                ParticleSystem b = Instantiate(attackHitVfx, transform.position, Quaternion.identity)
                    .GetComponent<ParticleSystem>();
                return b;
            },
            slash => slash.gameObject.SetActive(true),
            slash => slash.gameObject.SetActive(false),
            slash => Destroy(slash.gameObject),
            true,
            10
        );

        InputManager.Instance.InputActions.Enable();
    }

    public void SpawnAttackParticles(Vector3 position)
    {
        ParticleSystem effect = _attackParticlesPool.Get();
        effect.transform.position = position;
        effect.Play();
        StartCoroutine(ReleaseAfterDuration(effect));
    }

    private IEnumerator ReleaseAfterDuration(ParticleSystem effect)
    {
        yield return new WaitForSeconds(effect.main.duration);
        effect.Stop();
        _attackParticlesPool.Release(effect);
    }

    public void HitStop(float duration, float timeScale = 0, float delay = 0)
    {
        StartCoroutine(HitStopCoroutine(duration, timeScale, delay));
    }

    private IEnumerator HitStopCoroutine(float duration, float timeScale, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    public GameObject SlashAttack => _attacksPool.Get().gameObject;

    protected override void Clean()
    {
        OnPlayerEntersSafeZone.RemoveAllListeners();
        OnPlayerExitsSafeZone.RemoveAllListeners();
    }
}