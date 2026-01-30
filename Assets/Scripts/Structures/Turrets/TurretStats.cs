using System;
using System.Collections.Generic;
using UnityEngine;

public struct Upgrade
{
    public string Name;
    public int Level;
    public string[] Stats;
    public int[] Keys;
    public float[] Values;

    public Upgrade(string name, int level, string[] stats, int[] keys, float[] values)
    {
        Name = name;
        Level = level;
        Stats = stats;
        Keys = keys;
        Values = values;
    }
}

[Serializable]
public struct Upgrade<StatKeys> where StatKeys : Enum
{
    public string Name;
    public Stat<StatKeys> [] StatChanges;

    public Upgrade(string name, Stat<StatKeys> [] statChanges)
    {
        Name = name;
        StatChanges = statChanges;
    }
}

public struct Stat
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

    public Stat(string name, int key, float value, float minValue, float maxValue)
    {
        Name = name;
        Key = key;
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}

[Serializable]
public struct Stat<StatKeys> where StatKeys : Enum
{
    public StatKeys Key;
    public float Value;

    public Stat(StatKeys key, float value)
    {
        Key = key;
        Value = value;
    }
}

/// <summary>
/// Holds and manages a number of stats in key-value pairs. The keys are represented as integers (typically from an enum), and the values are floats.
/// Provides methods to get stat values and apply upgrades that modify these stats.
/// </summary>
public class Stats
{   
    float[] statValues;

    public Stats(int[] keys, float[] values)
    {
        if (keys.Length != values.Length)
        {
            Debug.LogError("StatKeys and values arrays must have the same length.");
            return;
        }

        statValues = new float[keys.Length];
        for (int i = 0; i < keys.Length; i++)
        {
            statValues[keys[i]] = values[i];
        }
    }

    public float GetStat(int key)
    {
        if (key < 0 || key >= statValues.Length)
        {
            Debug.LogError($"Stat key {key} is out of bounds.");
            return 0f;
        }

        return statValues[key];
    }

    public float GetStat(Enum stat)
    {
        int key = Convert.ToInt32(stat);
        return GetStat(key);
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        for (int i = 0; i < upgrade.Keys.Length; i++)
        {
            if (upgrade.Keys[i] < 0 || upgrade.Keys[i] >= statValues.Length)
            {
                Debug.LogError($"Stat key {upgrade.Keys[i]} is out of bounds.");
                continue;
            }

            statValues[upgrade.Keys[i]] *= (1f + upgrade.Values[i] / 100f);
            Debug.Log($"Applied upgrade: Stat {upgrade.Keys[i]} changed by {upgrade.Values[i]}% to {statValues[upgrade.Keys[i]]}");
        }
    }
}