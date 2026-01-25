using System;
using UnityEngine;

public class TurretUpgradeUI : MonoBehaviour
{
    [SerializeField] TurretUpgradeSlotUI[] upgradeSlotUIs;

    void Awake()
    {
        if (upgradeSlotUIs.Length != 3)
            Debug.LogError("TurretUpgradeUI requires exactly 3 TurretUpgradeSlotUIs assigned.");
    }

    public void Open(TurretTile<Enum> turret)
    {
        gameObject.SetActive(true);
        SetUpgrades(turret.GetUpgradeOptions());
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetUpgrades(TurretUpgrade<Enum>[] upgrades)
    {
        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            if (i < upgrades.Length)
            {
                var upgrade = upgrades[i];
                upgradeSlotUIs[i].Set(upgrade.Name, upgrade.Keys, upgrade.Changes);
            }
            else
            {
                upgradeSlotUIs[i].gameObject.SetActive(false);
            }
        }
    }
}
