using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    private float angle = 0f;
    public float radius = 2f;
    public float speed = 1f;
    private Vector3Int[,] grid;
    private Astar aStar;
    private Tilemap walkableTilemap;
    private Coroutine moveCoroutine;

    void Start()
    {
        walkableTilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();

        walkableTilemap.CompressBounds(); // Optional: compress bounds to fit tiles
        grid = CreateGrid(walkableTilemap);


        aStar = new Astar(grid, grid.GetUpperBound(0) + 1, grid.GetUpperBound(1) + 1);
    }

    Vector3Int[,] CreateGrid(Tilemap walkableTilemap)
    {
        BoundsInt bounds = walkableTilemap.cellBounds;
        Vector3Int[,] grid = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (walkableTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    grid[i, j] = new Vector3Int(x, y, 0);
                }
                else
                {
                    grid[i, j] = new Vector3Int(x, y, 1);
                }
            }
        }

        return grid;
    }

    public void MoveToLocation(Vector2Int end)
    {
        Vector3Int gridPos = walkableTilemap.WorldToCell(transform.position);
        List<Node> path = aStar.CreatePath(grid, new Vector2Int(gridPos.x, gridPos.y), end);

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
            Vector3 targetWorldPos = GridToWorldPosition(node.X, node.Y);
            
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

    private Vector3 GridToWorldPosition(int gridX, int gridY)
    {
        // Convert grid coordinates to tilemap cell position
        Vector3Int cellPosition = new Vector3Int(gridX, gridY, 0);
        
        // Get the world position from the tilemap
        // Tilemap.CellToWorld gives us the world position of the cell
        Vector3 worldPosition = walkableTilemap.CellToWorld(cellPosition);
        
        // // Center the position within the cell
        // worldPosition += walkableTilemap.cellSize / 2f;
        
        return worldPosition;
    }

    void Update()
    {

    }
}
