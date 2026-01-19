using UnityEngine;

public class Target : Structure
{
    override protected void DestroyStructure()
    {
        GameController.EndGame();

        base.DestroyStructure();
    }
}
