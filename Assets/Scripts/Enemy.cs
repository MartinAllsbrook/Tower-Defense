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

    void Start()
    {
        Tilemap walkableTilemap = GameObject.FindWithTag("Walkable Tilemap").GetComponent<Tilemap>();

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
        List<Node> path = aStar.CreatePath(grid, new Vector2Int(0, 0), end);

        if (path != null)
        {
            foreach (var node in path)
            {
                Debug.Log($"Path Node: ({node.X}, {node.Y})");
            }
        }
        else
        {
            Debug.Log("No valid path found.");
        }
    }

    void Update()
    {

    }
}
