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
        OnActionComplete();
    }

    void OnActionComplete()
    {
        if (actionQueue.Count > 0)
        {
            EnemyAction nextAction = actionQueue.Dequeue();
            nextAction.onComplete += OnActionComplete;
            nextAction.Execute();
        } 
        else
        {
            Debug.Log("Enemy has completed all actions in its queue.");
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
        Debug.Log($"Path: {path.nodes}");
        float priority = CalculatePriority(path);

        Queue<Vector2> worldPath = new Queue<Vector2>();
        foreach (Node node in path.nodes)
        {
            BoundsInt bounds = world.GetBounds();
            Vector3Int cellPos = new Vector3Int(node.X + bounds.xMin, node.Y + bounds.yMin, 0);
            Vector3 worldPos = world.CellToWorld(cellPos);
            worldPath.Enqueue(new Vector2(worldPos.x, worldPos.y));
        }
        // Remove the last item from worldPath if it exists
        if (worldPath.Count > 0)
        {
            // Convert to list, remove last, and rebuild queue
            var tempList = new List<Vector2>(worldPath);
            tempList.RemoveAt(tempList.Count - 1);
            worldPath = new Queue<Vector2>(tempList);
        }

        // Create move action
        MoveAction moveAction = new MoveAction(this, worldPath);

        // Add actions to the queue
        newActionQueue.Enqueue(moveAction);

        return newActionQueue;
    }

    Vector2Int FindObstacleOnPath(Path path)
    {
        foreach (Node node in path.nodes)
        {
            // MAJOR TODO: We should just know if the node is a structure and then get the first one
            BoundsInt bounds = world.GetBounds();
            StructureData structure = world.GetStructureAtCell(new Vector2Int(node.X + bounds.xMin, node.Y + bounds.yMin));
            if (structure != null)
            {
                return new Vector2Int(node.X + bounds.xMin, node.Y + bounds.yMin);
            }
        }
        return new Vector2Int(-1, -1); // No obstacle found
    }

    float CalculatePriority(Path path)
    {
        float cost = 0f;

        cost += path.cost;

        Node lastNode = path.nodes[path.nodes.Count - 1];
        BoundsInt bounds = world.GetBounds();
        StructureData target = world.GetStructureAtCell(new Vector2Int(lastNode.X + bounds.xMin, lastNode.Y + bounds.yMin));
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