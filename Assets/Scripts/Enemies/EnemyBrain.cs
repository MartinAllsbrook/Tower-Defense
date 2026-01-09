using System.Collections.Generic;
using UnityEngine;

class EnemyBrain : MonoBehaviour
{
    Queue<EnemyAction> actionQueue;
    World world;
    EnemyPathfinding pathfinding;

    void Awake()
    {
        world = FindFirstObjectByType<World>();
        pathfinding = GetComponent<EnemyPathfinding>();
        actionQueue = new Queue<EnemyAction>();
    }

    async void Start()
    {
        actionQueue = await EvaluateStrategy();
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
    async Awaitable<Queue<EnemyAction>> EvaluateStrategy()
    {
        // Make a new action queue
        Queue<EnemyAction> newActionQueue = new Queue<EnemyAction>();

        // Create path to target
        Target target = FindFirstObjectByType<Target>();
        Vector3Int targetCell = world.WorldToCell(target.transform.position);
        Path path = await pathfinding.GetPathToCell(new Vector2Int(targetCell.x, targetCell.y)); // Example target cell


        // TODO: This is unused right now but eventually we can compare it to other potential targets
        float priority = CalculatePriority(path);

        Queue<Vector2> worldPath = new Queue<Vector2>();
        foreach (Node node in path.nodes)
        {
            Vector3Int cellPos = new Vector3Int(node.CellX, node.CellY, 0);
            Vector3 worldPos = world.CellToWorld(cellPos);
            worldPath.Enqueue(new Vector2(worldPos.x, worldPos.y));
        }

        // Create move action
        MoveAction moveAction = new MoveAction();
        moveAction.SetPath(worldPath);

        // Add actions to the queue
        newActionQueue.Enqueue(moveAction);

        return newActionQueue;
    }

    Vector2Int FindObstacleOnPath(Path path)
    {
        foreach (Node node in path.nodes)
        {
            // MAJOR TODO: We should just know if the node is a structure and then get the first one
            Structure structure = world.GetStructureAtCell(new Vector2Int(node.CellX, node.CellY));
            if (structure != null)
            {
                return new Vector2Int(node.CellX, node.CellY);
            }
        }
        return new Vector2Int(-1, -1); // No obstacle found
    }

    float CalculatePriority(Path path)
    {
        float cost = 0f;

        cost += path.cost;

        Node lastNode = path.nodes[path.nodes.Count - 1];
        Structure target = world.GetStructureAtCell(new Vector2Int(lastNode.CellX, lastNode.CellY));
        if (target != null)
        {
            cost += target.priority;
        }

        return cost;
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