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

        return await CreatePath(new Vector2Int(start.x, start.y), cellPosition);
    }

    async Awaitable<Path> CreatePath(Vector2Int startCell, Vector2Int targetCell)
    {
        // try
        // {
            await Awaitable.BackgroundThreadAsync();
            Path path = world.GetThetaStar().CreatePath(startCell, targetCell);
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