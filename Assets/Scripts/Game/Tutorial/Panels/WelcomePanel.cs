using UnityEngine;

public class WelcomePanel : TutorialPanel
{
    protected override void Awake()
    {
        base.Awake();
        Open();
    }

    void OnEnable()
    {
        GameController.OnBasePlaced += Close;
    }

    void OnDisable()
    {
        GameController.OnBasePlaced -= Close;
    }
}
