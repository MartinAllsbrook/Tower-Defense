using System;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] UpgradeSlotUI[] upgradeSlotUIs;
    [SerializeField] StatDisplayUI[] statDisplayUIs;

    Turret turret;
    Dictionary<int, StatInfo> savedStats = new Dictionary<int, StatInfo>();

    bool isPreviewing = false;
    Upgrade previewingUpgrade;

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
        SetStats(turret.GetStatsAsInfo());

        if (isPreviewing)
        {
            PreviewUpgrade(previewingUpgrade);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        isPreviewing = false;

        for (int i = 0; i < upgradeSlotUIs.Length; i++)
        {
            upgradeSlotUIs[i].OnClicked -= UpgradeTurret;
            upgradeSlotUIs[i].OnHoverEnter -= PreviewUpgrade;
            upgradeSlotUIs[i].OnHoverExit -= ClearPreview;
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
            upgradeSlotUIs[i].OnHoverEnter += PreviewUpgrade;
            upgradeSlotUIs[i].OnHoverExit += ClearPreview;
        }
    }

    public void SetStats(StatInfo[] stats)
    {
        savedStats.Clear();
        for (int i = 0; i < stats.Length; i++)
        {
            savedStats[Convert.ToInt32(stats[i].Key)] = stats[i];
        }

        for (int i = 0; i < statDisplayUIs.Length; i++)
        {
            if (i >= stats.Length)
            {
                statDisplayUIs[i].gameObject.SetActive(false);
                continue;   
            }

            statDisplayUIs[i].gameObject.SetActive(true);
            var stat = stats[i];
            statDisplayUIs[i].Set(stat.Name, stat.Value, stat.EstimatedMin, stat.EstimatedMax);
        }
    }

    public void SetStats(Dictionary<int, StatInfo> stats)
    {
        SetStats(new List<StatInfo>(stats.Values).ToArray());
    }

    public void PreviewUpgrade(Upgrade upgrade)
    {
        isPreviewing = true;
        previewingUpgrade = upgrade;

        for (int i = 0; i < upgrade.Keys.Length; i++)
        {
            int key = upgrade.Keys[i];
            float currentValue = savedStats[key].Value;
            float newValue = ((upgrade.Values[i] / 100f) + 1) * currentValue;

            var statDisplayUI = statDisplayUIs[key]; // This is expecting keys to be at proper index
            statDisplayUI.Set(savedStats[key].Name, currentValue, newValue, savedStats[key].EstimatedMin, savedStats[key].EstimatedMax); 
        }
    }

    public void ClearPreview()
    {
        isPreviewing = false;

        SetStats(savedStats);
    }

    void UpgradeTurret(Upgrade upgrade)
    {
        turret.ApplyUpgrade(upgrade);

        // Refresh the UI
        Close();
        isPreviewing = true;
        Open(turret);
    }
}
