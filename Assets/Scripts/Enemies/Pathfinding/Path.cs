using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Path
{
    World world;
    public float cost;
    public Vector2Int[] tilePath;
    public Vector2[] worldPath;

    public Path(List<Vector2Int> path)
    {
        tilePath = path.ToArray();
        worldPath = tilePath.Select(tile => World.Instance.CellToWorld(new Vector3Int(tile.x, tile.y, 0))).ToArray();
    }

    /// <summary>
    /// Returns the world path without the last position.
    /// </summary>
    public Vector2[] GetMinusOne()
    {
        if (worldPath.Length <= 1)
        {
            return new Vector2[0];
        }

        return worldPath.Take(worldPath.Length - 1).ToArray();
    }

    public Path[] SplitAtIndices(List<int> indices)
    {
        List<Path> paths = new List<Path>();
        int start = 0;

        foreach (int index in indices)
        {
            List<Vector2Int> subPath = tilePath.Skip(start).Take(index - start + 1).ToList();
            paths.Add(new Path(subPath));
            start = index + 1;
        }

        // Add remaining path if any
        if (start < tilePath.Length)
        {
            List<Vector2Int> subPath = tilePath.Skip(start).ToList();
            paths.Add(new Path(subPath));
        }

        return paths.ToArray();
    }
}