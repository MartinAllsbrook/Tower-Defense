using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class TurretUpgradeSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI[] statChangeTexts;
    [SerializeField] Color green;
    [SerializeField] Color red;

    public event Action OnClicked;
    public event Action OnHoverEnter;
    public event Action OnHoverExit;

    void Awake()
    {
        if (statChangeTexts.Length < 6)
            Debug.LogError("TurretUpgradeSlotUI requires at least 6 stat change TextMeshProUGUI elements assigned.");
    }

    public void Set(string name, TurretStat<Enum>[] statChanges)
    {
        nameText.text = name;
        for (int i = 0; i < statChanges.Length; i++)
        {
            Enum key = statChanges[i].Key;
            float value = statChanges[i].Value;

            string sign = value >= 0 ? "+" : "-";
            statChangeTexts[i].text = $"{key}: {sign}{Mathf.Abs(value)}";

            statChangeTexts[i].color = value >= 0 ? green : red;
        }
    }
}
