using UnityEngine;
using System;

public class TutorialPanel : MonoBehaviour
{
    private CanvasGroup tutorialPanel;

    public event Action OnCompleted;

    protected virtual void Awake()
    {
        tutorialPanel = GetComponent<CanvasGroup>();
        Hide();
    }

    /// <summary>
    /// Call this to add the panel to the tutorial queue for opening.
    /// </summary>
    public void Open()
    {
        TutorialController.EnqueuePanel(this);
    }

    /// <summary>
    /// Called by TutorialController when this panel should actually open.
    /// </summary>
    public void DoOpen()
    {
        Show();
    }

    /// <summary>
    /// Call this to close the panel and trigger the next in the queue.
    /// </summary>
    public void Close()
    {
        Hide();
        OnCompleted?.Invoke();
        TutorialController.OnPanelClosed(this);
    }

    void Hide()
    {
        tutorialPanel.alpha = 0f;
        tutorialPanel.interactable = false;
        tutorialPanel.blocksRaycasts = false;
    }

    void Show()
    {
        tutorialPanel.alpha = 1f;
        tutorialPanel.interactable = true;
        tutorialPanel.blocksRaycasts = true;
    }

    protected void Open(int _)
    {
        Open();
    }

    protected void Close(int _)
    {
        Close();
    }
}
