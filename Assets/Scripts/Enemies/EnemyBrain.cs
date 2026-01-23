using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
class EnemyBrain : MonoBehaviour
{
    Queue<EnemyAction> actionQueue;
    Pathfinder pathfinder;
    EnemyAction currentAction;
    Enemy enemy => GetComponent<Enemy>();
    bool isEvaluating = false;

    void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        actionQueue = new Queue<EnemyAction>();
    }

    // This currently takes 3.5-4.5 ms on average to compute
    async void Start()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        actionQueue = await EvaluateStrategy();
        stopwatch.Stop();
        Debug.Log($"EvaluateStrategy took {stopwatch.ElapsedTicks} ticks, {stopwatch.ElapsedMilliseconds} ms");
        OnActionComplete();
        StartCoroutine(PeriodicStrategyRevaluation());   
    }

    /// <summary>
    /// Incomplete method to evaluate and create a strategy for the enemy. Currently just generates a move action to the target.
    /// </summary>
    async Awaitable<Queue<EnemyAction>> EvaluateStrategy()
    {
        // Make a new action queue
        Queue<EnemyAction> newActionQueue = new Queue<EnemyAction>();

        // Create path to target
        Target target = Player.Instance.GetTarget();
        Vector3Int targetCell = World.Instance.WorldToCell(target.transform.position);
        Path path = await pathfinder.GetPathToCell(new Vector2Int(targetCell.x, targetCell.y));
        
        List<int> structureIndices = new List<int>();
        List<Structure> structuresOnPath = new List<Structure>();
        for (int i = 0; i < path.tilePath.Length; i++)
        {
            Vector2Int tile = path.tilePath[i];
            Structure structure = World.Instance.GetStructureAt(tile);
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

    IEnumerator PeriodicStrategyRevaluation()
    {
        // Stagger initial evaluation by random delay to avoid all enemies evaluating at once
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        
        while (true)
        {
            // Wait 3-5 seconds (random)
            float waitTime = Random.Range(3f, 5f);
            yield return new WaitForSeconds(waitTime);

            // Don't interrupt if already evaluating
            if (isEvaluating)
                continue;

            // Trigger async evaluation (fire and forget - let it run asynchronously)
            _ = EvaluateAndUpdateStrategyAsync();
        }
    }

    async Awaitable EvaluateAndUpdateStrategyAsync()
    {
        if (isEvaluating)
            return;
            
        isEvaluating = true;
        
        try
        {
            Queue<EnemyAction> newQueue = await EvaluateStrategy();
            
            // Replace the action queue with the new strategy
            actionQueue = newQueue;
            
            if (currentAction != null)
            {
                currentAction.StopExecution();
            }

            OnActionComplete(); // Start executing from the new queue
        }
        finally
        {
            isEvaluating = false;
        }
    }

    void EvaluateAndUpdateStrategy()
    {
        // Wrapper for collision events
        _ = EvaluateAndUpdateStrategyAsync();
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

    // TODO: Remove all this dead code...

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