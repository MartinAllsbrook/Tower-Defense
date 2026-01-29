using QFSW.QC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statNameText;
    [SerializeField] TextMeshProUGUI statValueText;
    [SerializeField] Slider fillSlider;
    [SerializeField] Slider positiveSlider;
    [SerializeField] Slider negativeSlider;
    [SerializeField] Color defaultColor;
    [SerializeField] Color negativeColor;
    [SerializeField] Color positiveColor;

    public void Set(string name, float value, float min, float max)
    {
        statNameText.text = name;
        statValueText.text = value.ToString("F1");
        fillSlider.minValue = min;
        fillSlider.maxValue = max;
        fillSlider.value = value;

        positiveSlider.gameObject.SetActive(false);
        negativeSlider.gameObject.SetActive(false);
    
        statValueText.color = defaultColor;
    }

    public void Set(string name, float baseValue, float previewValue, float min, float max)
    {
        statNameText.text = name;
        fillSlider.minValue = min;
        fillSlider.maxValue = max;

        if (previewValue > baseValue)
        {
            positiveSlider.gameObject.SetActive(true);
            negativeSlider.gameObject.SetActive(false);

            positiveSlider.minValue = min;
            positiveSlider.maxValue = max;

            positiveSlider.value = previewValue;
            fillSlider.value = baseValue;

            statValueText.text = previewValue.ToString("F1");
            statValueText.color = positiveColor;
        }
        else if (previewValue < baseValue)
        {
            positiveSlider.gameObject.SetActive(false);
            negativeSlider.gameObject.SetActive(true);

            negativeSlider.minValue = min;
            negativeSlider.maxValue = max;

            negativeSlider.value = baseValue;
            fillSlider.value = previewValue;

            statValueText.text = previewValue.ToString("F1");
            statValueText.color = negativeColor;
        }
        else
        {
            positiveSlider.gameObject.SetActive(false);
            negativeSlider.gameObject.SetActive(false);
        }
    }

    [Command("SetFill")]
    public static void SetFillCommand(string name, float baseValue, float previewValue, float min, float max)
    {
        StatDisplayUI ui = GameObject.FindFirstObjectByType<StatDisplayUI>();
        ui.Set(name, baseValue, previewValue, min, max);
    }
}
