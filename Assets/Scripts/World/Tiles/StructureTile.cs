using System;
using UnityEngine;


public enum StructureType
{
    Base = 0,
    Wall = 10,
    Turret = 100,
}

[CreateAssetMenu(fileName = "New Structure Tile", menuName = "Tiles/Structure Tile")]
public class StructureTile : WorldTile
{
    [SerializeField] StructureType id;

    [Tooltip("Lower number means higher priority")]
    [SerializeField] int priority; 

    [SerializeField] float maxHealth;

    // Data accessors
    public StructureType ID => id;
    public int Priority => priority;
    public float MaxHealth => maxHealth;
}

