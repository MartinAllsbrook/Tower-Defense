using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class TurretUpgradeSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] RectTransform statChangesContainer;
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

    public void Set(string name, Enum[] stats, float[] changes)
    {
        nameText.text = name;
        for (int i = 0; i < stats.Length; i++)
        {
            string sign = changes[i] >= 0 ? "+" : "-";
            statChangeTexts[i].text = $"{stats[i]}: {sign}{Mathf.Abs(changes[i])}";

            statChangeTexts[i].color = changes[i] >= 0 ? green : red;
        }
    }
}
