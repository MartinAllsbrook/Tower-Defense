using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    List<EnemySpawner> spawners = new List<EnemySpawner>();

    int spawnersActive = 0;

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
        spawners[randomIndex].UpgradeSpawner();
    }
}