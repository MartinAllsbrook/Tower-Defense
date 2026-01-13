using TMPro;
using UnityEngine;

public class GeneralMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    public void SetMoney(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"Money: {amount}";
        }
    }
}
