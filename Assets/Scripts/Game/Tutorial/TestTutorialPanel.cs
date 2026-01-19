using UnityEngine;


class TestTutorialPanel : TutorialPanel
{
    void OnEnable()
    {
        GameController.OnBasePlaced += Open;
    }

    void OnDisable()
    {
        GameController.OnBasePlaced -= Open;   
    }
}