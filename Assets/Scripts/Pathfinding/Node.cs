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

    public void AddNeighbors(Node[,] grid, int x, int y)
    {
        // Flat top hex grid neighbor offsets
        // Even-q layout (even columns shifted down)
        int[][] evenOffsets = new int[][] {
            new int[] {+1,  0}, new int[] { 0, -1}, new int[] {-1, -1},
            new int[] {-1,  0}, new int[] {-1, +1}, new int[] { 0, +1}
        };
        int[][] oddOffsets = new int[][] {
            new int[] {+1,  0}, new int[] {+1, -1}, new int[] { 0, -1},
            new int[] {-1,  0}, new int[] { 0, +1}, new int[] {+1, +1}
        };

        int maxX = grid.GetUpperBound(0);
        int maxY = grid.GetUpperBound(1);
        int[][] offsets = (x % 2 == 0) ? evenOffsets : oddOffsets;

        foreach (var offset in offsets)
        {
            int nx = x + offset[0];
            int ny = y + offset[1];
            if (nx >= 0 && nx <= maxX && ny >= 0 && ny <= maxY)
            {
                Neighbors.Add(grid[nx, ny]);
            }
        }
    }
}