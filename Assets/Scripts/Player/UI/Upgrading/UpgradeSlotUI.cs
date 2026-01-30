using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;


public class UpgradeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI[] statChangeTexts;
    [SerializeField] Color green;
    [SerializeField] Color red;

    public event Action<Upgrade> OnClicked = delegate { };
    public event Action<Upgrade> OnHoverEnter = delegate { };
    public event Action<Upgrade> OnHoverExit = delegate { };

    Upgrade upgrade;
    int maxStatChanges = 6;

    void Awake()
    {
        if (statChangeTexts.Length != maxStatChanges)
            Debug.LogError($"UpgradeSlotUI requires exactly {maxStatChanges} elements assigned.");
    }

    public void Set(Upgrade upgrade)
    {
        this.upgrade = upgrade;

        nameText.text = $"{upgrade.Name} (Lv. {upgrade.Level})";

        for (int i = 0; i < maxStatChanges; i++)
        {
            if (i >= upgrade.Keys.Length)
            {
                statChangeTexts[i].text = "";
                continue;
            }
            
            string statName = upgrade.Stats[i];
            float percent = upgrade.Values[i];

            string sign = percent >= 0 ? "+" : "-";
            statChangeTexts[i].text = $"{statName}: {sign}{Mathf.Abs(percent)}%";

            statChangeTexts[i].color = percent >= 0 ? green : red;
        }
    }

    public void OnClick()
    {
        OnClicked.Invoke(upgrade);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter.Invoke(upgrade);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit.Invoke(upgrade);
    }
}
