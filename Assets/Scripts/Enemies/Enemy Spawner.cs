using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 10;

    GameController gameController;
    int numActiveEnemies = 0;
    bool isSpawning = false;

    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();
        gameController.OnRoundStart += StartRound;
    }

    void StartRound(int roundNumber)
    {
        StartCoroutine(RoundCoroutine());
    }

    IEnumerator RoundCoroutine()
    {
        isSpawning = true;
        gameController.RegisterSpawner();
        int enemiesToSpawn = 5; // Example value, could be based on round number
        yield return StartCoroutine(SpawnEnemiesRoutine(enemiesToSpawn));
        isSpawning = false;
    }

    IEnumerator SpawnEnemiesRoutine(int count)
    {
        SpawnEnemy();
        count--;
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity).GetComponent<Enemy>();
        numActiveEnemies++;
        enemy.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    void OnEnemyDestroyed()
    {
        numActiveEnemies--;
        if (numActiveEnemies <= 0 && !isSpawning)
        {
            gameController.UnregisterSpawner();
        }
    }
}
