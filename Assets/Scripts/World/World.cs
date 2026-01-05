using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    // Event that passes the updated grid to subscribers
    public event Action<Vector3Int[,]> OnGridUpdated;

    private Vector3Int[,] grid;

    public Tilemap tilemap; // PUBLIC FOR DEBUGGING

    void Start()
    {
        UpdateTilemap();
    }

    public void UpdateTilemap()
    {
        tilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();
        tilemap.CompressBounds(); // Optional: compress bounds to fit tiles
        grid = CreateGrid(tilemap);

        // Notify all subscribers with the updated grid
        OnGridUpdated?.Invoke(grid);
    }

    Vector3Int[,] CreateGrid(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int[,] grid = new Vector3Int[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
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
}

// Example subscriber class:
// public class GridListener : MonoBehaviour
// {
//     private void Start()
//     {
//         World world = FindFirstObjectByType<World>();
//         world.OnGridUpdated += HandleGridUpdate;
//     }
//
//     private void HandleGridUpdate(Vector3Int[,] updatedGrid)
//     {
//         Debug.Log($"Grid updated! Size: {updatedGrid.GetLength(0)}x{updatedGrid.GetLength(1)}");
//         // Use the updated grid data here
//     }
//
//     private void OnDestroy()
//     {
//         World world = FindFirstObjectByType<World>();
//         if (world != null)
//         {
//             world.OnGridUpdated -= HandleGridUpdate;
//         }
//     }
// }
