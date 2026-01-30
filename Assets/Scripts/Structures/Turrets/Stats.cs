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
    public StatKeyValue<StatKeys> [] StatChanges;

    public Upgrade(string name, StatKeyValue<StatKeys> [] statChanges)
    {
        Name = name;
        StatChanges = statChanges;
    }
}

[Serializable]
public struct StatKeyValue<StatKeys> where StatKeys : Enum
{
    public StatKeys Key;
    public float Value;

    public StatKeyValue(StatKeys key, float value)
    {
        Key = key;
        Value = value;
    }
}

public struct StatInfo
{
    public string Name;
    public int Key;
    public float Value;
    public float EstimatedMin;
    public float EstimatedMax;

    public StatInfo(string name, int key, float value, float estimatedMin, float estimatedMax)
    {
        Name = name;
        Key = key;
        Value = value;
        EstimatedMin = estimatedMin;
        EstimatedMax = estimatedMax;
    }
}

[Serializable]
public struct StatInfo<StatKeys> where StatKeys : Enum
{
    public string Name;
    public StatKeys Key;
    public float Value;
    public float EstimatedMin;
    public float EstimatedMax;
}

/// <summary>
/// Holds and manages a number of stats in key-value pairs. The keys are represented as integers (typically from an enum), and the values are floats.
/// Provides methods to get stat values and apply upgrades that modify these stats.
/// </summary>
public class Stats
{   
    Dictionary<int, float> statValues;

    public Stats(int[] keys, float[] values)
    {
        if (keys.Length != values.Length)
        {
            Debug.LogError("StatKeys and values arrays must have the same length.");
            return;
        }

        statValues = new Dictionary<int, float>();
        for (int i = 0; i < keys.Length; i++)
        {
            statValues[keys[i]] = values[i];
        }
    }

    public float GetStat(int key)
    {
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
            statValues[upgrade.Keys[i]] *= (1f + upgrade.Values[i] / 100f);
            Debug.Log($"Applied upgrade: Stat {upgrade.Keys[i]} changed by {upgrade.Values[i]}% to {statValues[upgrade.Keys[i]]}");
        }
    }
}