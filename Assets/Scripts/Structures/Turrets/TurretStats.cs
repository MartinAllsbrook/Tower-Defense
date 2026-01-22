using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Range,
    FireRate,
    Damage,
}

class TurretStats : MonoBehaviour
{
    List<UpgradeType> upgrades = new List<UpgradeType>();

    Dictionary<UpgradeType, int> upgradeCosts = new Dictionary<UpgradeType, int>()
    {
        { UpgradeType.Range, 0 },
        { UpgradeType.FireRate, 0 },
        { UpgradeType.Damage, 0 },
    };
    
    [SerializeField] float baseRange = 5f;
    [SerializeField] float baseFireRate = 1f;
    [SerializeField] float baseDamage = 10f;
    [SerializeField] float baseProjectileSpeed = 15f;

    float range;
    float fireRate;
    float damage;
    float projectileSpeed;

    public float Range => range;
    public float FireRate => fireRate;
    public float Damage => damage;
    public float ProjectileSpeed => projectileSpeed;

    public int GetUpgradeCost(UpgradeType upgrade)
    {
        return upgradeCosts[upgrade];
    }

    public int GetUpgradeLevel(UpgradeType upgrade)
    {
        int level = 0;
        foreach (var up in upgrades)
        {
            if (up == upgrade)
                level++;
        }
        return level;
    }   

    void Start()
    {
        range = baseRange;
        fireRate = baseFireRate;
        damage = baseDamage;
        projectileSpeed = baseProjectileSpeed;
        upgrades = new List<UpgradeType>();
    }

    public void ApplyUpgrade(UpgradeType upgrade)
    {
        upgrades.Add(upgrade);
        switch (upgrade)
        {
            case UpgradeType.Range:
                range *= 1.2f;
                break;
            case UpgradeType.FireRate:
                fireRate *= 1.2f;
                break;
            case UpgradeType.Damage:
                damage *= 1.2f;
                break;
        }

        Debug.Log($"Applied upgrade: {upgrade}. New level: {GetUpgradeLevel(upgrade)}. Stats - Range: {range}, FireRate: {fireRate}, Damage: {damage}");
    }

    public UpgradeType[] GetAvailableUpgrades()
    {
        return new UpgradeType[] { UpgradeType.Range, UpgradeType.FireRate, UpgradeType.Damage };
    }
}