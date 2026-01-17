using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class MoveAction : EnemyAction
{
    Vector2[] path;

    public MoveAction(Enemy enemy, Vector2[] path) : base(enemy)
    {
        this.path = path;
    }

    public void SetPath(Vector2[] newPath)
    {
        path = newPath;
    }

    public override IEnumerator ExecuteRoutine()
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        yield return movement.FollowPath(path);

        // Finish
        Complete();
        yield return null;
    }

    public override void StopExecution()
    {
        base.StopExecution();

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.StopMovement();
    }

    public Vector2[] GetPath()
    {
        return path;
    }
}