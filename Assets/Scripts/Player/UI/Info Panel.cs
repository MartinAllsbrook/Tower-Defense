using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : TutorialPanel
{
    [Header("Info Panel Content")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button continueButton;

    protected override void Awake()
    {
        base.Awake();
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinueClicked);
        }
    }

    public void SetContent(string title, string description)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
    }

    private void OnContinueClicked()
    {
        Hide();
    }
}
