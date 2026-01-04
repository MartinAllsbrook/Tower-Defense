using System;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public Node[,] NodeGrid;
    public Astar(Vector3Int[,] grid, int columns, int rows)
    {
        NodeGrid = new Node[columns, rows];
    }
    private bool IsValidPath(Vector3Int[,] grid, Node start, Node end)
    {
        if (end == null)
            return false;
        if (start == null)
            return false;
        if (!end.T)
            return false;
        return true;
    }
    public List<Node> CreatePath(Vector3Int[,] grid, Vector2Int start, Vector2Int end, bool allowDiagonalMovement = false)
    {
        Debug.Log(grid);

        // Initialize nodes
        Node End = null;
        Node Start = null;
        var columns = NodeGrid.GetUpperBound(0) + 1;
        var rows = NodeGrid.GetUpperBound(1) + 1;
        NodeGrid = new Node[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                NodeGrid[i, j] = new Node(grid[i, j].x, grid[i, j].y, 1, grid[i, j].z >= 1);
            }
        }

        // Add neighbors and identify start/end nodes
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                NodeGrid[i, j].AddNeighbors(NodeGrid, i, j);
                if (NodeGrid[i, j].X == start.x && NodeGrid[i, j].Y == start.y)
                    Start = NodeGrid[i, j];
                else if (NodeGrid[i, j].X == end.x && NodeGrid[i, j].Y == end.y)
                    End = NodeGrid[i, j];
            }
        }

        // Validate start and end nodes
        if (!IsValidPath(grid, Start, End))
            return null;

        // A* Algorithm
        List<Node> OpenSet = new List<Node>();
        List<Node> ClosedSet = new List<Node>();

        OpenSet.Add(Start);

        while (OpenSet.Count > 0)
        {
            //Find shortest step distance in the direction of your goal within the open set
            int winner = 0;
            for (int i = 0; i < OpenSet.Count; i++)
                if (OpenSet[i].F < OpenSet[winner].F)
                    winner = i;
                else if (OpenSet[i].F == OpenSet[winner].F) // TODO: Make this tie breaking for faster routing better
                    if (OpenSet[i].H < OpenSet[winner].H)
                        winner = i;

            var current = OpenSet[winner]; // The node in openSet with the lowest F value

            //Found the path; create path (from end to start) and return in correct order
            if (End != null && current == End)
            {
                List<Node> Path = new List<Node>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != null)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                Path.Reverse();
                return Path;
            }

            OpenSet.Remove(current);
            ClosedSet.Add(current);

            // Examine each neighbor of the current node
            var neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                var n = neighbors[i];
                // Skip if neighbor is already evaluated (in ClosedSet) or is not traversable
                if (!ClosedSet.Contains(n) && n.T)
                {
                    // Calculate tentative G score (cost from start to this neighbor through current node)
                    var tempG = current.G + current.C;

                    bool newPath = false;
                    if (OpenSet.Contains(n)) // Neighbor is already discovered
                    {
                        // Check if this path to the neighbor is better than the previously found path
                        if (tempG < n.G)
                        {
                            n.G = tempG; // Update to the better (shorter) path cost
                            newPath = true;
                        }
                    }
                    else // Neighbor has not been discovered yet
                    {
                        n.G = tempG; // Set initial path cost
                        newPath = true;
                        OpenSet.Add(n); // Add to nodes to be evaluated
                    }
                    
                    if (newPath) // If we found a better or new path to this neighbor
                    {
                        n.H = Heuristic(n, End, allowDiagonalMovement); // Calculate heuristic (estimated cost to goal)
                        n.F = n.G + n.H; // Total estimated cost (actual cost + heuristic)
                        n.previous = current; // Track the path by setting parent node
                    }
                }
            }

        }
        return null;
    }

    private int Heuristic(Node a, Node b, bool allowDiagonal)
    {
        // Hexagonal distance for even-q offset coordinates (flat-top hexagons)
        // Convert from offset coordinates to axial coordinates for distance calculation
        int aq = a.X;
        int ar = a.Y - (a.X - (a.X & 1)) / 2;
        int bq = b.X;
        int br = b.Y - (b.X - (b.X & 1)) / 2;
        
        // Calculate hex distance in axial coordinates (cube distance / 2)
        int distance = (Math.Abs(aq - bq) + Math.Abs(aq + ar - bq - br) + Math.Abs(ar - br)) / 2;
        
        return distance;
    }
}