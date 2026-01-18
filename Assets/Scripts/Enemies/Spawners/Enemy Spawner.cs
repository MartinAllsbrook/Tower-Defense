using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 10;

    GameController gameController;
    EnemySpawnerManager spawnerManager;
    World world;
    int numActiveEnemies = 0;
    bool isSpawning = false;
    int spawnerLevel = 1;

    void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();
        gameController.OnRoundStart += StartRound;

        spawnerManager = FindFirstObjectByType<EnemySpawnerManager>();
        spawnerManager.RegisterSpawner(this);

        world = FindFirstObjectByType<World>();
    }

    void Start()
    {
        World world = FindFirstObjectByType<World>();
        Vector3Int tilePos = world.WorldToCell(transform.position);
        
            world.SetBiomeAt(tilePos, BiomeID.bug1);
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

    public bool UpgradeSpawner()
    {
        if (spawnerLevel >= 4)
        {
            Debug.Log("Spawner at " + transform.position + " is already at max level.");
            return false;
        }

        spawnerLevel++;

        Debug.Log("Upgrading spawner at " + transform.position);
        World world = FindFirstObjectByType<World>();
        Vector2Int tilePos = world.WorldToCell2(transform.position);
        
        // Set the center tile (spawner position) to the current spawner level's bug type
        BiomeID centerBiome = BiomeID.bug1 + (spawnerLevel - 1);
        Debug.Log("Setting center tile at " + tilePos + " to " + centerBiome);
        world.SetBiomeAt(new Vector3Int(tilePos.x, tilePos.y, 0), centerBiome);
        
        // Track which ring each tile belongs to (starting from ring 1, since center is ring 0)
        Dictionary<Vector2Int, int> tileRingLevel = new Dictionary<Vector2Int, int>();
        Vector2Int[] lastNeighbors = { tilePos };
        
        for (int ring = 1; ring < spawnerLevel; ring++)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            foreach (var pos in lastNeighbors)
            {
                Vector2Int[] nbs = World.GetNeighbors(pos);
                foreach (var nb in nbs)
                {
                    if (!tileRingLevel.ContainsKey(nb) && nb != tilePos)
                    {
                        neighbors.Add(nb);
                        tileRingLevel[nb] = ring;
                    }
                }
            }

            lastNeighbors = neighbors.ToArray();
        }

        // Now set the biomes based on ring level
        // Center is spawnerLevel bug type, each ring outward decreases by one
        foreach (var kvp in tileRingLevel)
        {
            Vector2Int tilePosition = kvp.Key;
            int ringLevel = kvp.Value;
            
            // Calculate biome: center has highest, each ring out decreases
            BiomeID biome = BiomeID.bug1 + (spawnerLevel - ringLevel - 1);
            
            Debug.Log("Setting biome at " + tilePosition + " (ring " + ringLevel + ") to " + biome);
            world.SetBiomeAt(new Vector3Int(tilePosition.x, tilePosition.y, 0), biome);
        }

        return true;
    }
}
