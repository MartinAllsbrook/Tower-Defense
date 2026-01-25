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
    
}