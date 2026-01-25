using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct TurretUpgrade<StatKeys> where StatKeys : Enum
{
    public string Name;
    public TurretStat<StatKeys> [] StatChanges;

    public TurretUpgrade(string name, TurretStat<StatKeys> [] statChanges)
    {
        Name = name;
        StatChanges = statChanges;
    }
}

[Serializable]
public struct TurretStat<StatKeys> where StatKeys : Enum
{
    public StatKeys Key;
    public float Value;

    public TurretStat(StatKeys key, float value)
    {
        Key = key;
        Value = value;
    }
}

public class TurretTile<StatKeys> : StructureTile where StatKeys : Enum
{

    [SerializeField] protected TurretStat<StatKeys>[] baseStats;
    [SerializeField] protected TurretUpgrade<StatKeys>[] upgradeOptions;

    public TurretStat<StatKeys>[] BaseStats { get { return baseStats; } }
    public TurretUpgrade<StatKeys>[] UpgradeOptions { get { return upgradeOptions; } }
}