using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Utils;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemySpawnRate
{
    [Range(0, 100)] public int spawnRate;
    public Enemy.Enemy enemy;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnRate[] enemies;
    [SerializeField] private int maxEnemies = 3;

    [SerializeField] private int playerCheckRadius;
    [SerializeField] private int spawnRadius;
    [SerializeField] private float spawnInterval = 0.5f;

    private int _currentEnemies;
    private float _lastSpawnTime;
    private float _lastTimeInsideSpawnArea;

    private Transform _player;

    private void Start()
    {
        _player = GameManager.Instance.PlayerTransform;

        int rateSum = enemies.Sum(enemy => enemy.spawnRate);

        if (rateSum != 100)
        {
            Debug.LogError(
                $"The spawn rate sum of each enemy in {gameObject.name} must be 100, but it's currently {rateSum}");
        }
    }

    private void FixedUpdate()
    {
        SpawnCheck();
    }

    private void SpawnCheck()
    {
        if (_currentEnemies >= maxEnemies) return;

        float diffX = _player.position.x - transform.position.x;
        float diffY = _player.position.x - transform.position.x;

        float diffXSquared = diffX * diffX;
        float diffYSquared = diffY * diffY;

        float distSquared = diffXSquared + diffYSquared;

        float playerCheckRadiusSquared = playerCheckRadius * playerCheckRadius;
        if (distSquared < playerCheckRadiusSquared) // If it's inside the check radius
        {
            float spawnRadiusSquared = spawnRadius * spawnRadius;
            if (distSquared > spawnRadiusSquared) // If it's outside the spawn radius
            {
                if (Time.time - _lastTimeInsideSpawnArea >= 7 && Time.time - _lastSpawnTime >= spawnInterval) // If it meets spawn interval condition
                {
                    SpawnRandomEnemy();
                }
            }
            else // If it's inside the spawn radius
            {
                _lastTimeInsideSpawnArea = Time.time;
            }
        }
    }

    private void SpawnRandomEnemy()
    {
        Enemy.Enemy e = Instantiate(ChooseEnemy(), FindValidSpawnPoint(40), Quaternion.identity);
        e.OnDeath.AddListener(OnEnemyDeath);
        e.transform.position = new Vector3(e.transform.position.x, 0, e.transform.position.z);
        _lastSpawnTime = Time.time;
        _currentEnemies++;
    }

    private void OnEnemyDeath(Enemy.Enemy enemy)
    {
        _currentEnemies--;
        enemy.OnDeath.RemoveListener(OnEnemyDeath);
    }

    private Enemy.Enemy ChooseEnemy()
    {
        int random = Random.Range(0, 100);

        int currentPercent = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            currentPercent += enemies[i].spawnRate;
            if (random <= currentPercent)
            {
                return enemies[i].enemy;
            }
        }

        return null; // This should never happen
    }

    private Vector3 FindValidSpawnPoint(int maxIterations)
    {
        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 randomSpawnPoint = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0,
                Random.Range(-spawnRadius, spawnRadius));

            if (NavMesh.SamplePosition(randomSpawnPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                if (hit.position.y < 5f)
                {
                    return hit.position;
                }
            }
        }

        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Utilities.DrawCylinder(transform.position, playerCheckRadius, 15f, new Color(247, 76, 67));
        Gizmos.DrawCube(transform.position + new Vector3(0, 7.5f, 0),
            new Vector3(spawnRadius * 2, 15, spawnRadius * 2));
    }
}