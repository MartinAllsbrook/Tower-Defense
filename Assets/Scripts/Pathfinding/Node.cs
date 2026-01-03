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

    public void AddNeighbors(Node[,] grid, int x, int y, bool allowDiagonal)
    {
        // X
        if (x < grid.GetUpperBound(0))
            Neighbors.Add(grid[x + 1, y]);
        if (x > 0)
            Neighbors.Add(grid[x - 1, y]);

        // Y
        if (y < grid.GetUpperBound(1))
            Neighbors.Add(grid[x, y + 1]);
        if (y > 0)
            Neighbors.Add(grid[x, y - 1]);

        if (!allowDiagonal)
            return;
     
        // Diagonals
        if (x > 0 && y > 0)
            Neighbors.Add(grid[x - 1, y - 1]);
        if (x < grid.GetUpperBound(0) && y > 0)
            Neighbors.Add(grid[x + 1, y - 1]);
        if (x > 0 && y < grid.GetUpperBound(1))
            Neighbors.Add(grid[x - 1, y + 1]);
        if (x < grid.GetUpperBound(0) && y < grid.GetUpperBound(1))
            Neighbors.Add(grid[x + 1, y + 1]);
    }
}