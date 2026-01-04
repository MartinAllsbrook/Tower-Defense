using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>   
/// Represents a node in the pathfinding grid.
/// </summary>
public class Node
{
    public int X;
    public int Y;
    public int F; // F = G + H
    public int G; // Cost from start to current node
    public int H; // Heuristic cost estimate to end node
    public int C; // Cost to traverse this node
    public bool T; // Whether the node is traversable at all
    public List<Node> Neighbors;
    public Node previous = null;

    public Node(int x, int y, int cost, bool traversable)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        C = cost;
        T = traversable;

        Neighbors = new List<Node>();
    }

    public void AddNeighbors(Node[,] grid, int arrayX, int arrayY)
    {
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

        int maxArrayX = grid.GetUpperBound(0);
        int minArrayX = grid.GetLowerBound(0);
        int maxArrayY = grid.GetUpperBound(1);
        int minArrayY = grid.GetLowerBound(1);
        
        // Use the actual Y coordinate of THIS node, not the array index
        int[][] offsets = (this.Y % 2 == 0) ? evenColOffsets : oddColOffsets;

        foreach (var offset in offsets)
        {
            // Calculate neighbor's actual coordinates
            int neighborX = this.X + offset[0];
            int neighborY = this.Y + offset[1];
            
            // Search for the node with matching actual coordinates
            Node neighbor = FindNodeInGrid(grid, neighborX, neighborY, minArrayX, maxArrayX, minArrayY, maxArrayY);
            if (neighbor != null)
            {
                Neighbors.Add(neighbor);
            }
        }
    }

    private Node FindNodeInGrid(Node[,] grid, int targetX, int targetY, int minX, int maxX, int minY, int maxY)
    {
        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (grid[i, j].X == targetX && grid[i, j].Y == targetY)
                {
                    return grid[i, j];
                }
            }
        }
        return null;
    }
}