using UnityEngine;

public class Target : Structure
{
    override protected void DestroyStructure()
    {
        GameController gameController = FindFirstObjectByType<GameController>();
        gameController.EndGame();

        base.DestroyStructure();
    }
}
