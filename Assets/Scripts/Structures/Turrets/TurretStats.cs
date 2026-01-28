using System;
using System.Collections.Generic;
using UnityEngine;

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

    public float GetStat(int stat)
    {
        if (stat < 0 || stat >= statValues.Length)
        {
            Debug.LogError($"Stat key {stat} is out of bounds.");
            return 0f;
        }

        return statValues[stat];
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