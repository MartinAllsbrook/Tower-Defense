using System;
using UnityEngine;

public enum GunTurretStat
{
    Range,
    FireRate,
    Damage,
    ProjectileSpeed,
    Penetration,
    Accuracy,
}

[CreateAssetMenu(fileName = "New Gun Turret Tile", menuName = "Tiles/Gun Turret")]
public class GunTurretTile : TurretTile<GunTurretStat>
{
    TurretStat<GunTurretStat>[] baseStats = new TurretStat<GunTurretStat>[]
    {
        new TurretStat<GunTurretStat>(GunTurretStat.Range, 5f),
        new TurretStat<GunTurretStat>(GunTurretStat.FireRate, 1f),
        new TurretStat<GunTurretStat>(GunTurretStat.Damage, 10f),
        new TurretStat<GunTurretStat>(GunTurretStat.ProjectileSpeed, 10f),
        new TurretStat<GunTurretStat>(GunTurretStat.Penetration, 1f),
        new TurretStat<GunTurretStat>(GunTurretStat.Accuracy, 0f),
    };
    
    TurretUpgrade<GunTurretStat>[] upgradeOptions = new TurretUpgrade<GunTurretStat>[3]
    {
        new TurretUpgrade<GunTurretStat>(
            "Mechanisms",
            new GunTurretStat[] { GunTurretStat.FireRate, GunTurretStat.Accuracy },
            new float[] { 5f, -5f }
        ),
        new TurretUpgrade<GunTurretStat>(
            "Barrel",
            new GunTurretStat[] { GunTurretStat.Range, GunTurretStat.Accuracy, GunTurretStat.Penetration, GunTurretStat.ProjectileSpeed },
            new float[] { 10f, 5f, 2f, 5f }
        ),
        new TurretUpgrade<GunTurretStat>(
            "Caliber",
            new GunTurretStat[] { GunTurretStat.Damage, GunTurretStat.Penetration, GunTurretStat.ProjectileSpeed },
            new float[] { 10f, 3f, -5f }
        ),
    };

    public override TurretStat<GunTurretStat>[] BaseStats => baseStats;

    public override TurretUpgrade<GunTurretStat>[] UpgradeOptions => upgradeOptions;
}