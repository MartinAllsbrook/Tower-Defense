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

// Non-generic base class for polymorphic access
public abstract class TurretTile : StructureTile
{
    public abstract TurretUpgrade<Enum>[] GetUpgradeOptions();
}

public class TurretTile<StatKeys> : TurretTile where StatKeys : Enum
{

    [SerializeField] protected TurretStat<StatKeys>[] baseStats;
    [SerializeField] protected TurretUpgrade<StatKeys>[] upgradeOptions;

    public TurretStat<StatKeys>[] BaseStats { get { return baseStats; } }
    public TurretUpgrade<StatKeys>[] UpgradeOptions { get { return upgradeOptions; } }

    public override TurretUpgrade<Enum>[] GetUpgradeOptions()
    {
        // Convert the strongly-typed upgrades to the base Enum type
        TurretUpgrade<Enum>[] result = new TurretUpgrade<Enum>[upgradeOptions.Length];
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            var upgrade = upgradeOptions[i];
            TurretStat<Enum>[] stats = new TurretStat<Enum>[upgrade.StatChanges.Length];
            for (int j = 0; j < upgrade.StatChanges.Length; j++)
            {
                stats[j] = new TurretStat<Enum>(upgrade.StatChanges[j].Key, upgrade.StatChanges[j].Value);
            }
            result[i] = new TurretUpgrade<Enum>(upgrade.Name, stats);
        }
        return result;
    }
}