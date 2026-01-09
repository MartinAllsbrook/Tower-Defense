using System;
using UnityEngine;

class EnemyPathfinding : MonoBehaviour
{
    World world;

    void Awake()
    {
        world = FindFirstObjectByType<World>();
    }

    public async Awaitable<Path> GetPathToCell(Vector2Int cellPosition)
    {
        Vector3Int start = world.WorldToCell(transform.position);
        GridCell[,] gridCells = world.GetGrid();
        BoundsInt bounds = world.GetBounds();
        Vector2Int offset = new Vector2Int(bounds.xMin, bounds.yMin);

        return await CreatePath(gridCells, offset, new Vector2Int(start.x, start.y), cellPosition);
    }

    async Awaitable<Path> CreatePath(GridCell[,] grid, Vector2Int offset, Vector2Int startCell, Vector2Int targetCell)
    {
        // try
        // {
            // Switch to background thread for pathfinding computation
            await Awaitable.BackgroundThreadAsync();

            // Do pathfinding
            ThetaStarNew thetaStar = new ThetaStarNew(grid, offset);
            Path path = thetaStar.CreatePath(startCell, targetCell);

            // Switch back to main thread before returning
            await Awaitable.MainThreadAsync();
            return path;

        // }
        // catch (System.Exception ex)
        // {
        //     Debug.LogError($"Pathfinding failed: {ex.Message}");
        //     await Awaitable.MainThreadAsync();
        //     return null;
        // }
    }
}