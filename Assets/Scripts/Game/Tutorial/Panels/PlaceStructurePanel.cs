class PlaceStructurePanel : TutorialPanel
{
    void OnEnable()
    {
        GameController.OnBasePlaced += Open;
        GameController.OnFirstStructurePlaced += Close;
    }

    void OnDisable()
    {
        GameController.OnBasePlaced -= Open;
        GameController.OnFirstStructurePlaced -= Close;
    }
}