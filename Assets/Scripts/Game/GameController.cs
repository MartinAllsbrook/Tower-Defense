using System;
using UnityEngine;

/// <summary>
/// Event Manager
/// </summary>
public static class GameController
{
    // Tutorial / Info Events
    public static event Action OnGameStart; // This is kind of a placeholder idk. literally just going to be like a Start call
    public static event Action OnFirstStructurePlaced;
    public static event Action StructurePlaced;

    // General Events
    public static event Action<int> OnBasePlaced;
    public static event Action<int> OnRoundStart;
    public static event Action<int> OnRoundEnd;
    public static event Action OnGameOver;

    static bool firstStructurePlaced = false;
    static bool inRound = false;
    static int currentRound = 0;
    static int spawnersActive = 0;
    static float lastStructureDamageTime = 0f;

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
        PlayerPrefs.SetInt("RoundsSurvived", currentRound);
        OnRoundEnd?.Invoke(currentRound);
    }

    public static void EndGame()
    {
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Over");
    }

    public static void PlaceStructure()
    {
        if (!firstStructurePlaced)
        {
            firstStructurePlaced = true;
            OnFirstStructurePlaced?.Invoke();
        }
        StructurePlaced?.Invoke();
    }

    // TODO: Move this somewhere else and make different sound for different structure types
    public static void StructureTakenDamage()
    {
        float timeBetweenAlerts = 3f;
        if (Time.time - lastStructureDamageTime > timeBetweenAlerts)
        {
            AudioManager audioManager = GameObject.FindFirstObjectByType<AudioManager>();
            audioManager.PlayStructureUnderAttackSound();
            lastStructureDamageTime = Time.time;
        }
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