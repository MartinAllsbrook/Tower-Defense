using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

public class EnemySpawnerManager : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    int spawnersActive = 0;

    void Awake()
    {
        GameController gameController = FindFirstObjectByType<GameController>();
        gameController.OnRoundEnd += (roundNumber) =>
        {
            UpgradeRandomSpawner();
        };
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        spawners.Add(spawner);
    }

    public void TallyActiveSpawner(EnemySpawner spawner)
    {
        spawnersActive++;
    }

    public void UntallyActiveSpawner(EnemySpawner spawner)
    {
        spawnersActive = Mathf.Max(0, spawnersActive - 1);
        if (spawnersActive == 0)
        {
            GameController gameController = FindFirstObjectByType<GameController>();
            gameController.EndRound();
        }
    }

    void UpgradeRandomSpawner()
    {
        if (spawners.Count == 0) return;

        int randomIndex = Random.Range(0, spawners.Count);
        for (int i = 0; i < spawners.Count; i++)
        {
            EnemySpawner spawner = spawners[(randomIndex + i) % spawners.Count];
            if (spawner.UpgradeSpawner())
            {
                Debug.Log("Upgraded spawner at " + spawner.transform.position);
                return;
            }
        }
    }

    // Console command for testing
    [Command] 
    public static void UpgradeSpawner()
    {
        EnemySpawnerManager manager = FindFirstObjectByType<EnemySpawnerManager>();
        if (manager == null)
        {
            Debug.LogWarning("No EnemySpawnerManager found in the scene.");
            return;
        }
        manager.UpgradeRandomSpawner();
    }
}