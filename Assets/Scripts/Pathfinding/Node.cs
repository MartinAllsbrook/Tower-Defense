using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>   
/// Represents a node in the pathfinding grid.
/// </summary>
public class Node
{
    public int CellX;
    public int CellY;
    public int GridX;
    public int GridY;
    public float F; // F = G + H
    public float G; // Cost from start to current node
    public float H; // Heuristic cost estimate to end node
    public float C; // Cost to traverse this node
    public bool T; // Whether the node is traversable at all
    public List<Node> Neighbors;
    public Node previous = null;

    public Node(int cellX, int cellY, int gridX, int gridY, float cost, bool traversable)
    {
        CellX = cellX;
        CellY = cellY;
        GridX = gridX;
        GridY = gridY;

        F = 0;
        G = 0;
        H = 0;
        C = cost;
        T = traversable;

        Neighbors = new List<Node>();
    }

    public void AddNeighbors(Node[,] grid, BoundsInt bounds)
    {
        // Debug.Log($"Adding neighbors for node at ({X}, {Y})");

        // Flat top hex grid neighbor offsets
        // Even-q layout (even COLUMNS are reference, odd columns shifted down)
        // When column (X coordinate) is even
        int[][] evenColOffsets = new int[][] {
            new int[] {+1,  0}, // N
            new int[] { 0, +1}, // NE
            new int[] {-1, +1}, // SE
            new int[] {-1,  0}, // S
            new int[] {-1, -1}, // SW
            new int[] { 0, -1}  // NW
        };
        // When column (X coordinate) is odd (shifted down, so upper diagonals have Y-1)
        int[][] oddColOffsets = new int[][] {
            new int[] {+1,  0}, // N
            new int[] {+1, +1}, // NE
            new int[] { 0, +1}, // SE
            new int[] {-1,  0}, // S
            new int[] { 0, -1}, // SW
            new int[] {+1, -1}  // NW
        };
        
        // Use the actual Y coordinate of THIS node, not the array index
        int[][] offsets = (CellY % 2 == 0) ? evenColOffsets : oddColOffsets;

        foreach (var offset in offsets)
        {
            // Calculate neighbor's tilemap coordinates
            // TODO: We could just use GridX and GridY here and skip the offset calculation
            Vector2Int neighborPos = new Vector2Int(CellX + offset[0], CellY + offset[1]);

            // Correct bounds check: ensure neighborPos is within bounds
            if (neighborPos.x >= bounds.xMin && neighborPos.x < bounds.xMax &&
                neighborPos.y >= bounds.yMin && neighborPos.y < bounds.yMax)
            {
                // Get for the node with matching actual coordinates
                Node neighbor = FindNodeInGrid(grid, bounds, neighborPos);

                if (neighbor != null)
                {
                    Neighbors.Add(neighbor);
                }
            }
        }
    }

    private Node FindNodeInGrid(Node[,] grid, BoundsInt bounds, Vector2Int target)
    {
        // Debug.Log($"Target at ({target.x}, {target.y})");
        int x = target.x - bounds.xMin;
        int y = target.y - bounds.yMin;

        // Debug.Assert(x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1), $"Target position ({x}, {y}) is out of grid bounds. Grid bounds: xMin={grid.GetLowerBound(0)}, xMax={grid.GetUpperBound(0)}, yMin={grid.GetLowerBound(1)}, yMax={grid.GetUpperBound(1)}");
        Node node = grid[x, y];

        if (node.CellX == target.x && node.CellY == target.y)
        {
            return grid[x, y];
        }

        // Debug.LogWarning($"Node at ({target.x}, {target.y}) not found in grid.");
        return null;
    }


}