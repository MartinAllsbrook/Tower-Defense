using System;
using UnityEngine;

public class GameController: MonoBehaviour
{
    public event Action OnBasePlaced;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;

    [SerializeField] private Player player;

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
}