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

    public void ApplyUpgrade(TurretStat[] statChanges)
    {
        for (int i = 0; i < statChanges.Length; i++)
        {
            if (statChanges[i].Key < 0 || statChanges[i].Key >= statValues.Length)
            {
                Debug.LogError($"Stat key {statChanges[i].Key} is out of bounds.");
                continue;
            }

            statValues[statChanges[i].Key] *= (1f + statChanges[i].Value / 100f);
            Debug.Log($"Applied upgrade: Stat {statChanges[i].Key} changed by {statChanges[i].Value}% to {statValues[statChanges[i].Key]}");
        }
    }
}