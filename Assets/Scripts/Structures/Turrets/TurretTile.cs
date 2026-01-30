using System;
using Unity.VisualScripting;
using UnityEngine;



// Non-generic base class for polymorphic access
public abstract class TurretTile : StructureTile
{
}

public class TurretTile<StatKeys> : TurretTile where StatKeys : Enum
{

    [SerializeField] protected Stat<StatKeys>[] baseStats;
    [SerializeField] protected Upgrade<StatKeys>[] upgradeOptions;

    public Stat<StatKeys>[] BaseStats { get { return baseStats; } }
    public Upgrade<StatKeys>[] UpgradeOptions { get { return upgradeOptions; } }

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