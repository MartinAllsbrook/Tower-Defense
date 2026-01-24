using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

class Pathfinder : MonoBehaviour
{
    [SerializeField] bool usePathfindingManager = true; // Toggle to use centralized manager

    ThetaStar cachedThetaStar;
    bool gridDirty = true;

    #region Lifecycle
    void OnEnable()
    {
        World.OnWorldUpdate += MarkGridDirty;
    }

    void OnDisable()
    {
        World.OnWorldUpdate -= MarkGridDirty;
    }
    #endregion

    public void MarkGridDirty()
    {
        gridDirty = true;
        if (usePathfindingManager && PathfindingManager.Instance != null)
        {
            PathfindingManager.Instance.MarkGridDirty(); // TODO: This might be redundant
        }
    }

    public async Awaitable<Path> GetPathToCell(Vector2Int cellPosition)
    {
        Vector3Int start = World.Instance.WorldToCell(transform.position);
        Vector2Int startCell = new Vector2Int(start.x, start.y);
        
        // Use PathfindingManager if available and enabled
        if (usePathfindingManager && PathfindingManager.Instance != null)
        {
            List<Vector2Int> pathList = await PathfindingManager.Instance.RequestPath(startCell, cellPosition);
            Path path = new Path(pathList);
            return path;
        }
        
        // Fallback to local pathfinding
        // Only rebuild ThetaStar if grid changed
        if (gridDirty || cachedThetaStar == null)
        {
            GridCell[,] gridCells = World.Instance.GetGrid();
            BoundsInt bounds = World.Instance.GetBounds();
            Vector2Int offset = new Vector2Int(bounds.xMin, bounds.yMin);
            
            await Awaitable.BackgroundThreadAsync();
            cachedThetaStar = new ThetaStar(gridCells, offset);
            await Awaitable.MainThreadAsync();
            
            gridDirty = false;
        }

        return await CreatePath(startCell, cellPosition);
    }

    async Awaitable<Path> CreatePath(Vector2Int startCell, Vector2Int targetCell)
    {
        // Switch to background thread for pathfinding computation
        await Awaitable.BackgroundThreadAsync();

        Stopwatch stopwatch = Stopwatch.StartNew();
        // Do pathfinding with cached ThetaStar
        List<Vector2Int> pathList = cachedThetaStar.CreatePath(startCell, targetCell);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Pathfinding took {stopwatch.ElapsedMilliseconds} ms.");

        // Switch back to main thread before returning
        await Awaitable.MainThreadAsync();

        Path path = new Path(pathList);

        return path;
    }
}