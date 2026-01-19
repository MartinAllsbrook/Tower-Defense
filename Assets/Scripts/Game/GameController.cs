using System;
using UnityEngine;

public static class GameController
{
    public static event Action<int> OnBasePlaced;
    public static event Action<int> OnRoundStart;
    public static event Action<int> OnRoundEnd;
    public static event Action OnGameOver;

    static bool inRound = false;
    static int currentRound = 0;
    static int spawnersActive = 0;

    public static void PlaceBase()
    {
        OnBasePlaced?.Invoke(0);
    }

    public static void StartRound()
    {
        Debug.Log("Starting Round " + (currentRound + 1));
        currentRound++;
        inRound = true;
        OnRoundStart?.Invoke(currentRound);
    }

    public static void EndRound()
    {
        inRound = false;
        OnRoundEnd?.Invoke(currentRound);
    }

    public static void EndGame()
    {
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
    }

    #region Spawner Management

    public static void RegisterSpawner()
    {
        spawnersActive++;
    }

    public static void UnregisterSpawner()
    {
        spawnersActive = Math.Max(0, spawnersActive - 1);
        if (spawnersActive == 0)
        {
            EndRound();
        }
    }

    #endregion
}