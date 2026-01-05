using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] private World world;
    [SerializeField] private Transform target;
    
    private float angle = 0f;
    public float radius = 2f;
    public float speed = 1f;
    private ThetaStar thetaStar;
    private Coroutine moveCoroutine;
    

    void Start()
    {
        thetaStar = new ThetaStar();

        world.OnGridUpdated += (updatedGrid) => {
            Debug.Log("World grid updated, recalculating path.");
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                MoveToTarget();
            }
        };
    }

    public void MoveToTarget()
    {
        Vector3Int targetPos = world.WorldToCell(target.position);
        MoveToLocation(new Vector2Int(targetPos.x, targetPos.y));
    }


    public void MoveToLocation(Vector2Int end)
    {
        Vector3Int start = world.WorldToCell(transform.position);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        List<Node> path = thetaStar.CreatePath(world.grid, world.bounds, new Vector2Int(start.x, start.y), end);
        stopwatch.Stop();
        Debug.Log($"Path creation took {stopwatch.ElapsedMilliseconds} ms.");

        if (path != null && path.Count > 0)
        {
            // Stop any existing movement
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            // Start moving along the path
            moveCoroutine = StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.Log("No valid path found.");
        }
    }

    private IEnumerator MoveAlongPath(List<Node> path)
    {
        foreach (var node in path)
        {
            // Convert grid coordinates to world position (flat-top hex grid)
            Vector3 targetWorldPos = world.CellToWorld(new Vector3Int(node.X, node.Y, 0));
            
            // Move towards the target position
            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetWorldPos, 
                    speed * Time.deltaTime
                );
                yield return null;
            }
            
            // Ensure we're exactly at the target position
            transform.position = targetWorldPos;
        }
        
        Debug.Log("Enemy reached destination.");
    }
}
