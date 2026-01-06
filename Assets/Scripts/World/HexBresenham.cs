using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HexBresenham
{
    /// <summary>
    /// Bresenham's line algorithm adapted for hexagonal grids using cube coordinates.
    /// Returns all hex coordinates along the straight line between two hexes.
    /// 
    /// This algorithm steps through hexes one at a time by choosing the direction that
    /// minimizes distance to the ideal line. It uses only integer arithmetic, making it
    /// faster and more precise than interpolation-based approaches.
    /// </summary>
    public static List<Vector2Int> HexLineDraw(int x0, int y0, int x1, int y1)
    {
        List<Vector2Int> results = new List<Vector2Int>();
        
        // Convert offset coordinates to cube coordinates
        Vector3Int a = OffsetToCube(x0, y0);
        Vector3Int b = OffsetToCube(x1, y1);
        
        // Calculate the distance in hex space (number of steps)
        int distance = (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2;
        
        // Handle the trivial case of start == end
        if (distance == 0)
        {
            results.Add(new Vector2Int(x0, y0));
            return results;
        }
        
        // Current position in cube coordinates
        Vector3Int current = a;
        results.Add(new Vector2Int(x0, y0));
        
        // Calculate deltas for each cube axis
        int dx = b.x - a.x;
        int dy = b.y - a.y;
        int dz = b.z - a.z;
        
        // Step through the line, choosing the best direction at each step
        for (int step = 1; step <= distance; step++)
        {
            // Calculate which coordinate should change to stay closest to the ideal line
            // We use the ratio of how far we've traveled vs total distance
            float t = (float)step / distance;
            
            // Calculate ideal position
            float idealX = a.x + dx * t;
            float idealY = a.y + dy * t;
            float idealZ = a.z + dz * t;
            
            // Try each of the 6 hex directions and pick the one closest to ideal
            Vector3Int best = current;
            float bestDist = float.MaxValue;
            
            // The 6 cube directions for hexagons: (+1,-1,0), (+1,0,-1), (0,+1,-1), (-1,+1,0), (-1,0,+1), (0,-1,+1)
            Vector3Int[] directions = new Vector3Int[]
            {
                new Vector3Int(1, -1, 0),
                new Vector3Int(1, 0, -1),
                new Vector3Int(0, 1, -1),
                new Vector3Int(-1, 1, 0),
                new Vector3Int(-1, 0, 1),
                new Vector3Int(0, -1, 1)
            };
            
            foreach (Vector3Int dir in directions)
            {
                Vector3Int candidate = current + dir;
                float dist = Math.Abs(candidate.x - idealX) + Math.Abs(candidate.y - idealY) + Math.Abs(candidate.z - idealZ);
                
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidate;
                }
            }
            
            current = best;
            Vector2Int offsetCoord = CubeToOffset(current.x, current.y, current.z);
            results.Add(offsetCoord);
        }
        
        return results;
    }

    /// <summary>
    /// Convert even-r offset coordinates to cube coordinates (for flat-top hexagons).
    /// 
    /// Offset coordinates (col, row) represent a standard 2D grid where hexagons are stored.
    /// Cube coordinates (x, y, z) are a 3D representation where x + y + z = 0 always holds.
    /// 
    /// Cube coordinates make hex math much simpler:
    /// - Distance between hexes = (|dx| + |dy| + |dz|) / 2
    /// - Neighbors are just ±1 in any two coordinates
    /// - Line algorithms work naturally in this space
    /// 
    /// For flat-top hexagons with even-r offset:
    /// - x = col - (row - (row & 1)) / 2  (shifts every other row)
    /// - z = row                            (row maps directly to z)
    /// - y = -x - z                         (enforces the constraint x + y + z = 0)
    /// </summary>
    public static Vector3Int OffsetToCube(int col, int row)
    {
        int x = col - (row - (row & 1)) / 2;
        int z = row;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Convert cube coordinates to even-r offset coordinates (for flat-top hexagons)
    /// </summary>
    public static Vector2Int CubeToOffset(int x, int y, int z)
    {
        int col = x + (z - (z & 1)) / 2;
        int row = z;
        return new Vector2Int(col, row);
    }
}