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
        for (int i = 0; i < upgrade.StatChanges.Length; i++)
        {
            ImproveStat(upgrade.StatChanges[i]);
        }
    }

    void ImproveStat(TurretStat<StatKeys> statChange)
    {
        if (statValues.ContainsKey(statChange.Key))
        {
            statValues[statChange.Key] *= (1f + statChange.Value / 100f);
        }
        else
        {
            Debug.LogWarning($"StatKeys {statChange.Key} not found in TurretStats. Cannot improve.");
        }
    }
}