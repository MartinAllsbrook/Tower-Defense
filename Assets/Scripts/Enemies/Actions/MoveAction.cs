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

    public override IEnumerator Execute()
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.SetPath(path);
        yield return movement.FollowPath();

        // Finish
        Complete();
        yield return null;
    }

    public Vector2[] GetPath()
    {
        return path;
    }
}