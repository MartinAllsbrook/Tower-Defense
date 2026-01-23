using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    void OnEnable()
    {
        Player.Instance.OnMoneyChanged += UpdateMoneyUI;
    }

    void OnDisable()
    {
        Player.Instance.OnMoneyChanged -= UpdateMoneyUI;
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
