using System;
using TMPro;
using UnityEngine;

class UpgradeButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;

    UpgradeType upgradeType;

    public event Action<UpgradeType> OnClicked;

    public void Setup(UpgradeType upgradeType, int cost, int level)
    {
        this.upgradeType = upgradeType;
        nameText.text = upgradeType.ToString() + " Lv." + (level + 1);
        costText.text = "Cost: " + cost;
    }

    public void Clicked()
    {
        OnClicked?.Invoke(upgradeType);
    }
}