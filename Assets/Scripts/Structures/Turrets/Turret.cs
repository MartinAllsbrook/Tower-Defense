using System;
using UnityEngine;

public abstract class Turret : Structure 
{
    public abstract Upgrade[] GetUpgradeOptions();
    public abstract StatInfo[] GetStatsAsInfo();
    public abstract void ApplyUpgrade(Upgrade upgrade);
}

public class Turret<StatKey> : Turret where StatKey : Enum // This class does not NEED to be abstract
{
    [Header("Layers")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstacleLayers;
    [SerializeField] TurretTile<StatKey> turretTile;

    // Protected
    protected Stats stats;
    protected TurretSwivel swivel;
    
    // Private
    Target target;
    int[] upgradeLevels;

    protected override void Awake()
    {
        base.Awake();

        if (turretTile.VerifyAllBaseStatsExist())
        {
            stats = new Stats(turretTile.GetKeysInt(), turretTile.GetBaseValues());
        }

        upgradeLevels = new int[turretTile.UpgradeOptions.Length];

        swivel = GetComponentInChildren<TurretSwivel>();
    }

    void Start()
    {
        target = FindFirstObjectByType<Target>();
    }

    protected Collider2D FindClosestEnemyWithLineOfSight(float range)
    {
        // Perform a 2D circle cast to find enemies in range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (enemiesInRange.Length == 0) return null;

        Collider2D closest = null;
        float minDist = float.MaxValue;

        for (int i = 0; i < enemiesInRange.Length; i++)
        {
            Vector2 direction = enemiesInRange[i].transform.position - transform.position;
            float dist = direction.magnitude;

            LayerMask combinedMask = enemyLayer | obstacleLayers;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, dist, combinedMask);

            // Check if raycast hit the enemy (not blocked by obstacles)
            if (hit.collider == enemiesInRange[i])
            {
                // Calculate distance to both turret and target, use the minimum
                float distToTurret = dist;
                float distToTarget = target != null ? 
                    Vector2.Distance(enemiesInRange[i].transform.position, target.transform.position) : 
                    float.MaxValue;
                float minDistToEither = Mathf.Min(distToTurret, distToTarget);

                if (minDistToEither < minDist)
                {
                    closest = enemiesInRange[i];
                    minDist = minDistToEither;
                }
            }
        }

        return closest;
    }

    #region Upgrading

    public override Upgrade[] GetUpgradeOptions()
    {
        // Convert the strongly-typed upgrades to the int type
        Upgrade<StatKey>[] upgradeOptions = turretTile.UpgradeOptions;

        Upgrade[] result = new Upgrade[upgradeOptions.Length];
        for (int i = 0; i < upgradeOptions.Length; i++)
        {
            Upgrade<StatKey> upgrade = upgradeOptions[i];

            string[] stats = new string[upgrade.StatChanges.Length];
            int[] keys = new int[upgrade.StatChanges.Length];
            float[] values = new float[upgrade.StatChanges.Length];

            for (int j = 0; j < upgrade.StatChanges.Length; j++)
            {
                stats[j] = upgrade.StatChanges[j].Key.ToString();
                keys[j] = Convert.ToInt32(upgrade.StatChanges[j].Key);
                values[j] = upgrade.StatChanges[j].Value;
            }
            result[i] = new Upgrade(upgrade.Name, upgradeLevels[i] + 1, stats, keys, values);
        }
        return result;
    }

    public override StatInfo[] GetStatsAsInfo()
    {
        StatInfo<StatKey>[] baseStats = turretTile.BaseStats;
        StatInfo[] result = new StatInfo[baseStats.Length];

        for (int i = 0; i < baseStats.Length; i++)
        {
            StatInfo<StatKey> baseStat = baseStats[i];
            StatKey key = baseStat.Key;
            result[i] = new StatInfo(baseStat.Name, Convert.ToInt32(key), stats.GetStat(key), baseStat.EstimatedMin, baseStat.EstimatedMax);
        }

        return result;
    }

    public override void ApplyUpgrade(Upgrade upgrade)
    {
        stats.ApplyUpgrade(upgrade);
        
        int index = 0;
        for (int i = 0; i < turretTile.UpgradeOptions.Length; i++)
        {
            if (turretTile.UpgradeOptions[i].Name == upgrade.Name)
            {
                index = i;
                break;
            }
        }

        if (index >= 0 && index < upgradeLevels.Length)
        {
            upgradeLevels[index]++;
        }
        else
        {
            Debug.LogError("Attempted to apply an upgrade that does not belong to this turret.");
        }
    }

    #endregion
}
