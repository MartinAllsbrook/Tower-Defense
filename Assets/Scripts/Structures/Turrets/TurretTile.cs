using System;
using Unity.VisualScripting;
using UnityEngine;

public struct TurretUpgrade
{
    public string Name;
    public int Level;
    public string[] Stats;
    public int[] Keys;
    public float[] Values;

    public TurretUpgrade(string name, int level, string[] stats, int[] keys, float[] values)
    {
        Name = name;
        Level = level;
        Stats = stats;
        Keys = keys;
        Values = values;
    }
}

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

public struct TurretStat
{
    public string Name;
    public int Key;
    public float Value;
    /// <summary>
    /// Estimated min value for the stat
    /// </summary>
    public float MinValue;
    /// <summary>
    /// Estimated max value for the stat
    /// </summary>
    public float MaxValue;

    public TurretStat(string name, int key, float value, float minValue, float maxValue)
    {
        Name = name;
        Key = key;
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
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
}

public class TurretTile<StatKeys> : TurretTile where StatKeys : Enum
{

    [SerializeField] protected TurretStat<StatKeys>[] baseStats;
    [SerializeField] protected TurretUpgrade<StatKeys>[] upgradeOptions;

    public TurretStat<StatKeys>[] BaseStats { get { return baseStats; } }
    public TurretUpgrade<StatKeys>[] UpgradeOptions { get { return upgradeOptions; } }

    public bool VerifyAllBaseStatsExist()
    {
        Array enumValues = Enum.GetValues(typeof(StatKeys));
        foreach (StatKeys key in enumValues)
        {
            bool found = false;
            foreach (var stat in baseStats)
            {
                if (stat.Key.Equals(key))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError($"Base stat for key {key} is missing in TurretTile.");
                return false;
            }
        }
        return true;
    }

    public StatKeys[] GetKeys()
    {
        StatKeys[] keys = new StatKeys[baseStats.Length];
        for (int i = 0; i < baseStats.Length; i++)
        {
            keys[i] = baseStats[i].Key;
        }
        return keys;
    }

    public int[] GetKeysInt()
    {
        int[] keys = new int[baseStats.Length];
        for (int i = 0; i < baseStats.Length; i++)
        {
            keys[i] = Convert.ToInt32(baseStats[i].Key);
        }
        return keys;
    }

    public float[] GetBaseValues()
    {
        float[] values = new float[baseStats.Length];
        for (int i = 0; i < baseStats.Length; i++)
        {
            values[i] = baseStats[i].Value;
        }
        return values;
    }
}