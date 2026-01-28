using System;
using UnityEngine;

public class TurretUpgradeUI : MonoBehaviour
{
    [SerializeField] TurretUpgradeSlotUI[] upgradeSlotUIs;

    Turret turret;

    void Awake()
    {
        if (upgradeSlotUIs.Length != 3)
            Debug.LogError("TurretUpgradeUI requires exactly 3 TurretUpgradeSlotUIs assigned.");

        Close();
    }

    public void Open(Turret turret)
    {
        gameObject.SetActive(true);
        this.turret = turret;
        TurretTile turretTile = turret.Tile as TurretTile;
        SetUpgrades(turretTile.GetUpgradeOptions());
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetUpgrades(TurretUpgrade[] upgrades)
    {
        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            if (i < upgrades.Length)
            {
                var upgrade = upgrades[i];
                upgradeSlotUIs[i].Set(upgrade.Name, upgrade.Stats, upgrade.Keys, upgrade.Values);
    
                upgradeSlotUIs[i].OnClicked += UpgradeTurret;
            }
        }
    }

    void UpgradeTurret(TurretStat[] statChanges)
    {
        turret.Stats.ApplyUpgrade(statChanges);
    }
}
