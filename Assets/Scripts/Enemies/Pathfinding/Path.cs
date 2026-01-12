using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Path
{
    public float cost;
    public Vector2Int[] tilePath;
    public Vector2[] worldPath;

    public Path(List<Vector2Int> path)
    {
        this.tilePath = path.ToArray();
        World world = Object.FindFirstObjectByType<World>();
        worldPath = tilePath.Select(tile => world.CellToWorld(new Vector3Int(tile.x, tile.y, 0))).ToArray();
    }

    /// <summary>
    /// Returns the world path without the last position.
    /// </summary>
    /// <returns></returns>
    public Vector2[] GetMinusOne()
    {
        if (worldPath.Length <= 1)
        {
            return new Vector2[0];
        }

        return worldPath.Take(worldPath.Length - 1).ToArray();
    }
}