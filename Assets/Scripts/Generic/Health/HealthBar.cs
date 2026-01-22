using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] RectTransform healthFillTransform;
    [SerializeField] Image healthFillImage;
    [SerializeField] GameObject healthBarContainer;
    [SerializeField] Gradient healthGradient;

    float fill = 1f;

    void Start()
    {
        healthBarContainer.SetActive(false);
    }

    public void SetFill(float value)
    {
        fill = Mathf.Clamp01(value);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (fill < 1f && !healthBarContainer.activeSelf)
            healthBarContainer.SetActive(true);
        
        healthFillTransform.localScale = new Vector3(fill, 1f, 1f);
        healthFillImage.color = healthGradient.Evaluate(fill);
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
