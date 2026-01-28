using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;


public class TurretUpgradeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI[] statChangeTexts;
    [SerializeField] Color green;
    [SerializeField] Color red;

    public event Action<TurretStat[]> OnClicked = delegate { };
    public event Action<TurretStat[]> OnHoverEnter = delegate { };
    public event Action<TurretStat[]> OnHoverExit = delegate { };

    TurretStat[] statChanges;
    int maxStatChanges = 6;

    void Awake()
    {
        if (statChangeTexts.Length != maxStatChanges)
            Debug.LogError($"TurretUpgradeSlotUI requires exactly {maxStatChanges} elements assigned.");
    }

    public void Set(string name, string[] statNames, int[] statKeys, float[] statPercents)
    {
        nameText.text = name;
        statChanges = new TurretStat[statKeys.Length];

        for (int i = 0; i < maxStatChanges; i++)
        {
            if (i >= statKeys.Length)
            {
                statChangeTexts[i].text = "";
                continue;
            }
            
            string statName = statNames[i];
            float percent = statPercents[i];

            string sign = percent >= 0 ? "+" : "-";
            statChangeTexts[i].text = $"{statName}: {sign}{Mathf.Abs(percent)}%";

            statChangeTexts[i].color = percent >= 0 ? green : red;
        
            statChanges[i] = new TurretStat(statKeys[i], percent);
        }
    }

    public void OnClick()
    {
        OnClicked?.Invoke(statChanges);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Called when mouse enters the button
        Debug.Log("Mouse entered button");
        OnHoverEnter.Invoke(statChanges);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Called when mouse exits the button
        Debug.Log("Mouse exited button");
        OnHoverExit.Invoke(statChanges);
    }
}
