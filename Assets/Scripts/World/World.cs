using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    // Event that passes the updated grid to subscribers
    public event Action<GridCell[,]> OnGridUpdated;

    public GridCell[,] grid;

    public BoundsInt bounds;
    public Tilemap tilemap; // PUBLIC FOR DEBUGGING

    void Start()
    {
        UpdateTilemap();

        // Tilemap.tilemapTileChanged += (tilemap, position) => {
        //     UpdateTilemap();
        // };
    }

    public void UpdateTilemap()
    {
        tilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();
        tilemap.CompressBounds(); // Optional: compress bounds to fit tiles
        bounds = tilemap.cellBounds;
        grid = CreateGrid(tilemap);

        // Notify all subscribers with the updated grid
        OnGridUpdated?.Invoke(grid);
    }

    GridCell[,] CreateGrid(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        GridCell[,] grid = new GridCell[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < bounds.size.x; x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < bounds.size.y; y++, j++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile is TaggedTile taggedTile)
                {
                    grid[i, j] = new GridCell
                    {
                        Position = new Vector2Int(x, y),
                        Cost = taggedTile.tag == TileTag.Structure ? 15 : 1,
                        Traversable = taggedTile.tag != TileTag.Terrain,
                    };
                }
                else
                {
                    grid[i, j] = new GridCell
                    {
                        Position = new Vector2Int(x, y),
                        Cost = 1,
                        Traversable = true,
                    };
                }
            }
        }

        return grid;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return tilemap.CellToWorld(cellPosition);
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
