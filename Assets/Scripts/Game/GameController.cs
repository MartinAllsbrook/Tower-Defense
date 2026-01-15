using System;
using UnityEngine;

public class GameController: MonoBehaviour
{
    public event Action<int> OnBasePlaced;
    public event Action<int> OnRoundStart;
    public event Action<int> OnRoundEnd;
    public event Action OnGameOver;

    bool inRound = false;
    int currentRound = 0;
    int spawnersActive = 0;

    public void PlaceBase()
    {
        OnBasePlaced?.Invoke(0);
    }

    public void StartRound()
    {
        Debug.Log("Starting Round " + (currentRound + 1));
        currentRound++;
        inRound = true;
        OnRoundStart?.Invoke(currentRound);
    }

    public void EndRound()
    {
        inRound = false;
        OnRoundEnd?.Invoke(currentRound);
    }

    public void EndGame()
    {
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
    }

    #region Spawner Management

    public void RegisterSpawner()
    {
        spawnersActive++;
    }

    public void UnregisterSpawner()
    {
        spawnersActive = Math.Max(0, spawnersActive - 1);
        if (spawnersActive == 0)
        {
            EndRound();
        }
    }

    #endregion
}