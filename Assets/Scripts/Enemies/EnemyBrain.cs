using System.Collections.Generic;
using UnityEngine;

class EnemyBrain : MonoBehaviour
{
    Queue<EnemyAction> actionQueue;
    World world;

    void Awake()
    {
        world = FindFirstObjectByType<World>();
        actionQueue = new Queue<EnemyAction>();
    }

    void Start()
    {
        actionQueue = EvaluateStrategy();
        EnemyAction nextAction = actionQueue.Dequeue();
        if (nextAction is MoveAction moveAction)
        {
            Queue<Vector2> path = moveAction.GetPath();
            EnemyMovement movement = GetComponent<EnemyMovement>();
            movement.SetPath(path);
            movement.StartMoving();
        }
    }

    /// <summary>
    /// Incomplete method to evaluate and create a strategy for the enemy. Currently just generates a move action to the target.
    /// </summary>
    Queue<EnemyAction> EvaluateStrategy()
    {
        // Make a new action queue
        Queue<EnemyAction> newActionQueue = new Queue<EnemyAction>();

        // Create path to target
        Target target = FindFirstObjectByType<Target>();
        Vector3Int targetCell = world.WorldToCell(target.transform.position);
        Queue<Vector2> pathToTarget = GetPathToCell(new Vector2Int(targetCell.x, targetCell.y)); // Example target cell

        // Create move action
        MoveAction moveAction = new MoveAction();
        moveAction.SetPath(pathToTarget);

        // Add actions to the queue
        newActionQueue.Enqueue(moveAction);

        return newActionQueue;
    }

    Queue<Vector2> GetPathToCell(Vector2Int cellPosition)
    {
        Vector3Int start = world.WorldToCell(transform.position);
        List<Node> path = world.GetThetaStar().CreatePath(new Vector2Int(start.x, start.y), cellPosition);

        Queue<Vector2> worldPath = new Queue<Vector2>();
        foreach (Node node in path)
        {
            Vector3Int cellPos = new Vector3Int(node.CellX, node.CellY, 0);
            Vector3 worldPos = world.CellToWorld(cellPos);
            worldPath.Enqueue(new Vector2(worldPos.x, worldPos.y));
        }

        return worldPath;
    }
}

// void EvaluateStrategy()
// {
//     var mainObjective = FindMainObjective();
//     var mainPath = CalculatePath(mainObjective);
//     var mainScore = CalculatePriority(mainObjective, mainPath);
    
//     // Find obstacles on path to main objective
//     var obstacles = DetectObstaclesOnPath(mainPath, detectionRadius);
    
//     float bestSubScore = 0;
//     GameObject bestSubTarget = null;
    
//     foreach (var obstacle in obstacles)
//     {
//         float score = CalculatePriority(obstacle, CalculatePath(obstacle));
//         if (score > bestSubScore)
//         {
//             bestSubScore = score;
//             bestSubTarget = obstacle;
//         }
//     }
    
//     // Decision logic
//     if (bestSubScore > mainScore * switchThreshold) // e.g., 1.2x
//     {
//         CreateActionQueue(bestSubTarget, mainObjective);
//     }
//     else
//     {
//         CreateActionQueue(mainObjective);
//     }
// }