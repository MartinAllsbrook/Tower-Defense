using System;
using UnityEngine;

public class GameController: MonoBehaviour
{
    public event Action OnBasePlaced;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnGameOver;

    public void PlaceBase()
    {
        // Logic to place the base
        OnBasePlaced?.Invoke();
    }

    public void StartRound()
    {
        // Logic to start the round
        OnRoundStart?.Invoke();
    }

    public void EndRound()
    {
        // Logic to end the round
        OnRoundEnd?.Invoke();
    }

    public void EndGame()
    {
        // Logic to handle end of game
        OnGameOver?.Invoke();
        Debug.Log("Game Over!");
    }
}