using System;
using Unity.VisualScripting;
using UnityEngine;

public struct TurretUpgrade
{
    public string Name;
    public string[] Stats;
    public int[] Keys;
    public float[] Values;

    public TurretUpgrade(string name, string[] stats, int[] keys, float[] values)
    {
        Name = name;
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
    public int Key;
    public float Value;

    public TurretStat(int key, float value)
    {
        Key = key;
        Value = value;
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
    public abstract TurretUpgrade[] GetUpgradeOptions();
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

    public override TurretUpgrade[] GetUpgradeOptions()
    {
        // Convert the strongly-typed upgrades to the int type
        TurretUpgrade[] result = new TurretUpgrade[upgradeOptions.Length];
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            TurretUpgrade<StatKeys> upgrade = upgradeOptions[i];

            string[] stats = new string[upgrade.StatChanges.Length];
            int[] keys = new int[upgrade.StatChanges.Length];
            float[] values = new float[upgrade.StatChanges.Length];

            for (int j = 0; j < upgrade.StatChanges.Length; j++)
            {
                stats[j] = upgrade.StatChanges[j].Key.ToString();
                keys[j] = Convert.ToInt32(upgrade.StatChanges[j].Key);
                values[j] = upgrade.StatChanges[j].Value;
            }
            result[i] = new TurretUpgrade(upgrade.Name, stats, keys, values);
        }
        return result;
    }
}