using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretStats<StatKeys> where StatKeys : Enum
{   
    Dictionary<StatKeys, float> statValues = new Dictionary<StatKeys, float>();

    public TurretStats(TurretTile<StatKeys> turretTile)
    {
        foreach (var stat in turretTile.BaseStats)
        {
            statValues[stat.Key] = stat.Value;
        }
    }

    public float GetStat(StatKeys stat)
    {
        if (statValues.TryGetValue(stat, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"StatKeys {stat} not found in TurretStats.");
            return 0f;
        }
    }

    public void ApplyUpgrade(TurretUpgrade<StatKeys> upgrade)
    {
        for (int i = 0; i < upgrade.Keys.Length; i++)
        {
            ImproveStat(upgrade.Keys[i], upgrade.Changes[i]);
        }
    }

    void ImproveStat(StatKeys stat, float percentIncrease)
    {
        if (statValues.ContainsKey(stat))
        {
            statValues[stat] *= (1f + percentIncrease);
        }
        else
        {
            Debug.LogWarning($"StatKeys {stat} not found in TurretStats. Cannot improve.");
        }
    }
}