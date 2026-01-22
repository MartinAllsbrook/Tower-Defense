using UnityEngine;

class UpgradeMenu : MonoBehaviour
{
    [SerializeField] UpgradeButton upgradeButtonPrefab;

    TurretStats currentTurret;

    public void Open(UpgradeType[] offers, TurretStats turret)
    {
        if (currentTurret == turret)
            return;
        else if (currentTurret != null)
            Close();

        gameObject.SetActive(true);

        currentTurret = turret;

        foreach (var offer in offers)
        {
            var button = Instantiate(upgradeButtonPrefab, transform);
            button.Setup(offer, turret.GetUpgradeCost(offer), turret.GetUpgradeLevel(offer));

            button.OnClicked += (upgrade) => {
                UpgradeSelected(upgrade, turret);
                RefreshOffers();
            };
        }
    }

    void RefreshOffers()
    {
        TurretStats turret = currentTurret;
        Close();

        Open(turret.GetAvailableUpgrades(), turret);
    }

    void UpgradeSelected(UpgradeType upgrade, TurretStats turret)
    {
        turret.ApplyUpgrade(upgrade);
    }

    public void Close()
    {
        currentTurret = null;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }
}