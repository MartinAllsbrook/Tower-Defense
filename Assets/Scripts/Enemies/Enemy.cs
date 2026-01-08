using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [SerializeField] float radius = 2f;
    [SerializeField] GameObject spriteObject;
    [SerializeField] HealthBar healthBar;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] GameObject[] legObjects;

    float health = 100f;
    World world;
    Target target;
    Coroutine moveCoroutine;
    bool gameOver = false;

    void Awake()
    {
        target = FindFirstObjectByType<Target>();
        world = FindFirstObjectByType<World>();

        GameController gameController = FindFirstObjectByType<GameController>();
        gameController.OnGameOver += OnGameEnd;
        world.OnWorldUpdate += OnUpdateGrid;
    }

    void Start()
    {   
        AnimateLegs();
    }

    void AnimateLegs()
    {
        // Randomize leg animation starting points
        foreach (GameObject leg in legObjects)
        {
            Animation anim = leg.GetComponent<Animation>();
            if (anim != null && anim.clip != null)
            {
                anim[anim.clip.name].time = Random.Range(0f, anim.clip.length);
                anim.Play();
            }
        }
    }

    void Update()
    {
        if (target == null || gameOver) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget < radius)
        {
            target.DealDamage(50f);
            world.OnWorldUpdate -= OnUpdateGrid;
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DecreaseHealth(10f);
        }
    }

    void OnGameEnd()
    {
        gameOver = true;
    }

    void DecreaseHealth(float amount)
    {
        health -= amount;
        healthBar.SetFill(health / maxHealth);
        if (health <= 0)
        {
            world.OnWorldUpdate -= OnUpdateGrid;
            Destroy(gameObject);
        }
    }

    private void OnUpdateGrid()
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

        List<Node> path = world.GetThetaStar().CreatePath(new Vector2Int(start.x, start.y), end);

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
            Vector3 targetWorldPos = world.CellToWorld(new Vector3Int(node.CellX, node.CellY, 0));
            
            // Move towards the target position
            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetWorldPos, 
                    Time.deltaTime
                );
                Vector3 direction = (targetWorldPos - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                spriteObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }
            
            // Ensure we're exactly at the target position
            transform.position = targetWorldPos;
        }
        
        // Debug.Log("Enemy reached destination.");
    }
}
