using System;
using UnityEngine;

public enum GunTurretStatKeys
{
    Range,
    FireRate,
    Damage,
    ProjectileSpeed,
    Penetration,
    Accuracy,
}

[CreateAssetMenu(fileName = "New Ballistic Turret Data", menuName = "Turrets/Ballistic Turret Data")]
public class GunTurretTile : TurretTile<GunTurretStatKeys>
{

    public override TurretUpgrade<GunTurretStatKeys>[] GetUpgradeOptions()
    {
        return new TurretUpgrade<GunTurretStatKeys>[3]
        {
            new TurretUpgrade<GunTurretStatKeys>(
                "Mechanisms",
                new GunTurretStatKeys[] { GunTurretStatKeys.FireRate, GunTurretStatKeys.Accuracy },
                new float[] { 5f, -5f }
            ),
            new TurretUpgrade<GunTurretStatKeys>(
                "Barrel",
                new GunTurretStatKeys[] { GunTurretStatKeys.Range, GunTurretStatKeys.Accuracy, GunTurretStatKeys.Penetration, GunTurretStatKeys.ProjectileSpeed },
                new float[] { 10f, 5f, 2f, 5f }
            ),
            new TurretUpgrade<GunTurretStatKeys>(
                "Caliber",
                new GunTurretStatKeys[] { GunTurretStatKeys.Damage, GunTurretStatKeys.Penetration, GunTurretStatKeys.ProjectileSpeed },
                new float[] { 10f, 3f, -5f }
            ),
        };
    }
}