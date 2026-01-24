using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    void OnEnable()
    {
        Player.OnMoneyChanged += UpdateMoneyUI;
    }

    void OnDisable()
    {
        Player.OnMoneyChanged -= UpdateMoneyUI;
    }

    void UpdateMoneyUI(int amount)
    {
        SetMoney(amount);
    }

    public void SetMoney(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {amount}";
        }
    }
}
