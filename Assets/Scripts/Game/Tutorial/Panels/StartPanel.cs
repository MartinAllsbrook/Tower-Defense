class StartPanel : TutorialPanel
{
    void OnEnable()
    {
        GameController.OnFirstStructurePlaced += Open;
        GameController.OnRoundStart += Close;
    }

    void OnDisable()
    {
        GameController.OnFirstStructurePlaced -= Open;
        GameController.OnRoundStart -= Close;
    }


}