using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    Player player;
    void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    void OnEnable()
    {
        player.OnMoneyChanged += UpdateMoneyUI;
    }

    void OnDisable()
    {
        player.OnMoneyChanged -= UpdateMoneyUI;
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
