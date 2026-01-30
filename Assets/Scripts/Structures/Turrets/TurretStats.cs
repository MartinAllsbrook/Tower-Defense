using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds and manages a number of stats in key-value pairs. The keys are represented as integers (typically from an enum), and the values are floats.
/// Provides methods to get stat values and apply upgrades that modify these stats.
/// </summary>
public class TurretStats
{   
    float[] statValues;

    public TurretStats(int[] keys, float[] values)
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

    public void ApplyUpgrade(TurretUpgrade upgrade)
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