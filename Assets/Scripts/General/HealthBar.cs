using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform healthFill;

    private float fill = 1f;

    public void SetFill(float value)
    {
        fill = Mathf.Clamp01(value);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.localScale = new Vector3(fill, 1f, 1f);
        }
    }
}
