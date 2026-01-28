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
        // Ensure any previous subscriptions are cleared
        Close();

        gameObject.SetActive(true);
        this.turret = turret;
        SetUpgrades(turret.GetUpgradeOptions());
    }

    public void Close()
    {
        gameObject.SetActive(false);

        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            upgradeSlotUIs[i].OnClicked -= UpgradeTurret;
        }
    }

    public void SetUpgrades(TurretUpgrade[] upgrades)
    {
        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            if (i >= upgrades.Length)
                continue;

            var upgrade = upgrades[i];
            upgradeSlotUIs[i].Set(upgrade);

            upgradeSlotUIs[i].OnClicked += UpgradeTurret;
        }
    }

    void UpgradeTurret(TurretUpgrade upgrade)
    {
        turret.ApplyUpgrade(upgrade);

        // Refresh the UI
        Close();
        Open(turret);
    }
}
