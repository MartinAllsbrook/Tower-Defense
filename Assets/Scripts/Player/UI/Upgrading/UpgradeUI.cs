using System;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] UpgradeSlotUI[] upgradeSlotUIs;
    [SerializeField] StatDisplayUI[] statDisplayUIs;

    Turret turret;
    Stat[] savedStats;


    void Awake()
    {
        if (upgradeSlotUIs.Length != 3)
            Debug.LogError("UpgradeUI requires exactly 3 UpgradeSlotUIs assigned.");

        if (statDisplayUIs.Length != 6)
            Debug.LogError("StatsPreviewUI requires exactly 6 StatDisplayUIs assigned.");

        Close();
    }

    public void Open(Turret turret)
    {
        // Ensure any previous subscriptions are cleared
        Close();

        gameObject.SetActive(true);
        this.turret = turret;

        SetUpgrades(turret.GetUpgradeOptions());
        SetStats(turret.GetStats());
    }

    public void Close()
    {
        gameObject.SetActive(false);

        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            upgradeSlotUIs[i].OnClicked -= UpgradeTurret;
        }
    }

    public void SetUpgrades(Upgrade[] upgrades)
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

    public void SetStats(Stat[] stats)
    {
        savedStats = stats;

        for (int i = 0; i < statDisplayUIs.Length; i++)
        {
            if (i >= stats.Length)
            {
                statDisplayUIs[i].gameObject.SetActive(false);
                continue;   
            }

            statDisplayUIs[i].gameObject.SetActive(true);
            var stat = stats[i];
            statDisplayUIs[i].Set(stat.Name, stat.Value, stat.MinValue, stat.MaxValue);
        }
    }

    public void PreviewUpgrade(Upgrade upgrade)
    {
        for (int i = 0; i < upgrade.Keys.Length; i++)
        {
            int key = upgrade.Keys[i];
            float newValue = upgrade.Values[i];
            string name = upgrade.Stats[i];
            var statDisplayUI = statDisplayUIs[key];
            statDisplayUI.Set(name, savedStats[key].Value, newValue, savedStats[key].MinValue, savedStats[key].MaxValue); 
        }
    }

    public void ClearPreview()
    {
        SetStats(savedStats);
    }

    void UpgradeTurret(Upgrade upgrade)
    {
        turret.ApplyUpgrade(upgrade);

        // Refresh the UI
        Close();
        Open(turret);
    }
}
