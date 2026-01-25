using System;
using UnityEngine;

[Serializable]
public struct TurretUpgrade<StatKeys> where StatKeys : Enum
{
    public string Name;
    public StatKeys[] Keys;
    public float[] Changes;

    public TurretUpgrade(string name, StatKeys[] keys, float[] changes)
    {
        Name = name;
        Keys = keys;
        Changes = changes;
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

public abstract class TurretTile<StatKeys> : StructureTile where StatKeys : Enum
{
    [SerializeField] TurretStat<StatKeys>[] baseStats;

    public TurretStat<StatKeys>[] BaseStats { get { return baseStats; } }

    abstract public TurretUpgrade<StatKeys>[] GetUpgradeOptions();
}