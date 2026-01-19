using System.Collections.Generic;
using UnityEngine;

class EnemyBrain : MonoBehaviour
{
    Queue<EnemyAction> actionQueue;
    World world;
    EnemyPathfinding pathfinding;
    EnemyAction currentAction;
    Enemy enemy => GetComponent<Enemy>();
    bool isEvaluating = false;

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
        StartCoroutine(PeriodicStrategyRevaluation());
    }

    System.Collections.IEnumerator PeriodicStrategyRevaluation()
    {
        while (true)
        {
            // Wait 3-5 seconds (random)
            float waitTime = Random.Range(3f, 5f);
            yield return new WaitForSeconds(waitTime);

            // Don't interrupt if already evaluating
            if (isEvaluating)
                continue;

            // Trigger async evaluation
            EvaluateAndUpdateStrategy();

            // Wait until evaluation completes
            yield return new WaitUntil(() => !isEvaluating);
        }
    }

    async void EvaluateAndUpdateStrategy()
    {
        isEvaluating = true;
        Queue<EnemyAction> newQueue = await EvaluateStrategy();
        
        // Replace the action queue with the new strategy
        // Note: You may want to add logic here to preserve current action if needed
        actionQueue = newQueue;
        
        isEvaluating = false;
        
        if (currentAction != null)
        {
            currentAction.StopExecution();
        }

        OnActionComplete(); // Start executing from the new queue
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Structure") && !isEvaluating)
        {
            EvaluateAndUpdateStrategy();
        }
    }

    void OnActionComplete()
    {
        if (actionQueue.Count > 0)
        {
            
            if (currentAction != null)
            {
                currentAction.onComplete -= OnActionComplete;
                currentAction.StopExecution();
            }
            currentAction = actionQueue.Dequeue();
            currentAction.onComplete += OnActionComplete;
            currentAction.Execute();
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
        Path path = await pathfinding.GetPathToCell(new Vector2Int(targetCell.x, targetCell.y));
        
        List<int> structureIndices = new List<int>();
        List<Structure> structuresOnPath = new List<Structure>();
        for (int i = 0; i < path.tilePath.Length; i++)
        {
            Vector2Int tile = path.tilePath[i];
            Structure structure = world.GetStructureAt(tile);
            if (structure != null)
            {
                structureIndices.Add(i);
                structuresOnPath.Add(structure);
            }
        }

        Path[] subPaths = path.SplitAtIndices(structureIndices);
        for (int i = 0; i < subPaths.Length; i++)
        {
            Path subPath = subPaths[i];

            // Create move action to next structure or target
            MoveAction moveAction = new MoveAction(enemy, subPath.GetMinusOne());
            newActionQueue.Enqueue(moveAction);

            // If there's a structure at the end of this subpath, add an attack action
            if (i < structuresOnPath.Count)
            {
                Structure targetStructure = structuresOnPath[i];
                AttackAction attackAction = new AttackAction(enemy, targetStructure);
                newActionQueue.Enqueue(attackAction);
            }
        }

        newActionQueue.Enqueue(new AttackAction(enemy, target));

        return newActionQueue;
    }

    /*
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
    */

    /*
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
    */
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