using System;
using System.Diagnostics;
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
        // Switch to background thread for pathfinding computation
        await Awaitable.BackgroundThreadAsync();

        Stopwatch stopwatch = Stopwatch.StartNew();
        // Do pathfinding
        ThetaStar thetaStar = new ThetaStar(grid, offset);
        Path path = thetaStar.CreatePath(startCell, targetCell);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Pathfinding took {stopwatch.ElapsedMilliseconds} ms.");
        // Switch back to main thread before returning
        await Awaitable.MainThreadAsync();
        return path;
    }
}