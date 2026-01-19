using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialUI : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepId;
        public TutorialPanel panel;
        public UnityEvent onStepComplete;
    }

    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private bool autoStartTutorial = true;
    [SerializeField] private float delayBetweenSteps = 0.5f;

    private int currentStepIndex = -1;
    private TutorialPanel currentActivePanel;
    private bool tutorialActive = false;

    private void Start()
    {
        // Initialize all panels as hidden
        foreach (var step in tutorialSteps)
        {
            if (step.panel != null)
            {
                step.panel.gameObject.SetActive(false);
                step.panel.OnPanelClosed += HandlePanelClosed;
            }
        }

        if (autoStartTutorial)
        {
            StartTutorial();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        foreach (var step in tutorialSteps)
        {
            if (step.panel != null)
            {
                step.panel.OnPanelClosed -= HandlePanelClosed;
            }
        }
    }

    public void StartTutorial()
    {
        if (tutorialActive) return;
        
        tutorialActive = true;
        currentStepIndex = -1;
        ShowNextStep();
    }

    public void ShowNextStep()
    {
        if (!tutorialActive) return;

        currentStepIndex++;
        
        if (currentStepIndex >= tutorialSteps.Count)
        {
            CompleteTutorial();
            return;
        }

        ShowStep(currentStepIndex);
    }

    public void ShowStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= tutorialSteps.Count) return;

        // Hide current panel if any
        HideCurrentPanel();

        var step = tutorialSteps[stepIndex];
        if (step.panel != null)
        {
            currentActivePanel = step.panel;
            step.panel.Show();
        }
    }

    public void ShowStepById(string stepId)
    {
        var step = tutorialSteps.FirstOrDefault(s => s.stepId == stepId);
        if (step != null)
        {
            int index = tutorialSteps.IndexOf(step);
            ShowStep(index);
        }
    }

    private void HideCurrentPanel()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.Hide();
            currentActivePanel = null;
        }
    }

    private void HandlePanelClosed(TutorialPanel panel)
    {
        // Find the step that was closed and invoke its completion event
        var step = tutorialSteps.FirstOrDefault(s => s.panel == panel);
        if (step != null)
        {
            step.onStepComplete?.Invoke();
        }

        // Auto-advance to next step after a delay
        if (tutorialActive && delayBetweenSteps > 0)
        {
            Invoke(nameof(ShowNextStep), delayBetweenSteps);
        }
        else if (tutorialActive)
        {
            ShowNextStep();
        }
    }

    private void CompleteTutorial()
    {
        tutorialActive = false;
        HideCurrentPanel();
        Debug.Log("Tutorial completed!");
    }

    public void SkipTutorial()
    {
        tutorialActive = false;
        HideCurrentPanel();
    }

    // Public methods to control tutorial flow
    public bool IsTutorialActive() => tutorialActive;
    public int GetCurrentStepIndex() => currentStepIndex;
    public TutorialPanel GetCurrentPanel() => currentActivePanel;
}
