using System.Collections.Generic;
using System.IO;
using UnityEngine;

class MoveAction : EnemyAction
{
    Queue<Vector2> path;

    public MoveAction(EnemyBrain brain, Queue<Vector2> path) : base(brain)
    {
        this.path = path;
    }

    public void SetPath(Queue<Vector2> newPath)
    {
        path = newPath;
    }

    public override void Execute()
    {
        EnemyMovement movement = brain.GetComponent<EnemyMovement>();
        movement.SetPath(path);
        movement.StartMoving(() => Complete());
    }

    public Queue<Vector2> GetPath()
    {
        return path;
    }
}