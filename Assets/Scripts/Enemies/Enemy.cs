using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private GameObject spriteObject;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float maxHealth = 100f;

    private float health = 100f;
    private World world;
    private Target target;
    private float angle = 0f;
    private ThetaStar thetaStar;
    private Coroutine moveCoroutine;

    void Start()
    {
        target = GameObject.FindWithTag("Target").GetComponent<Target>();
        world = GameObject.FindWithTag("World").GetComponent<World>();

        // Debug.Log("Moving to target at position: " + target.transform.position);
        
        thetaStar = new ThetaStar();

        world.OnGridUpdated += OnUpdateGrid;
        MoveToTarget();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(10f);
        }
    }

    void DecreaseHealth(float amount)
    {
        health -= amount;
        healthBar.SetFill(health / maxHealth);
        if (health <= 0)
        {
            world.OnGridUpdated -= OnUpdateGrid;
            Destroy(gameObject);
        }
    }

    void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget < radius)
        {
            target.DealDamage(50f);
            world.OnGridUpdated -= OnUpdateGrid;
            Destroy(gameObject);
        }
    }

    private void OnUpdateGrid(GridCell[,] updatedGrid)
    {
        Debug.Log("World grid updated, recalculating path.");
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            MoveToTarget();
        }
    }

    public void MoveToTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set for Enemy.");
            return;
        }

        Vector3Int targetPos = world.WorldToCell(target.transform.position);
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
                Vector3 direction = (targetWorldPos - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }
            
            // Ensure we're exactly at the target position
            transform.position = targetWorldPos;
        }
        
        Debug.Log("Enemy reached destination.");
    }
}
