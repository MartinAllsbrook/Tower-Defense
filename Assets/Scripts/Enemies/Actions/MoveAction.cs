using System.Collections.Generic;
using System.IO;
using UnityEngine;

class MoveAction : EnemyAction
{
    Queue<Vector2> path;

    public void SetPath(Queue<Vector2> newPath)
    {
        path = newPath;
    }

    public Queue<Vector2> GetPath()
    {
        return path;
    }
}