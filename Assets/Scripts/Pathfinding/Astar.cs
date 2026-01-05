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

            // Examine each neighbor of the current node (Theta* algorithm)
            var neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                var n = neighbors[i];
                // Skip if neighbor is already evaluated (in ClosedSet) or is not traversable
                if (!ClosedSet.Contains(n) && n.T)
                {
                    // THETA* KEY DIFFERENCE: Check line of sight from parent
                    // If there's line of sight from current's parent to this neighbor,
                    // we can skip the current node and create a more direct path
                    Node parent = current.previous;
                    int tempG;
                    Node pathParent;
                    
                    if (parent != null && LineOfSight(parent, n))
                    {
                        // Path 2: Direct line from parent to neighbor (skip current node)
                        tempG = parent.G + Heuristic(parent, n, allowDiagonalMovement);
                        pathParent = parent;
                    }
                    else
                    {
                        // Path 1: Traditional A* path through current node
                        tempG = current.G + current.C;
                        pathParent = current;
                    }

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
                        n.previous = pathParent; // Track the path (may skip current node if LOS exists)
                    }
                }
            }

        }
        return null;
    }

    private int Heuristic(Node a, Node b, bool allowDiagonal)
    {
        // Use Euclidean distance between node a and node b
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return (int)Mathf.RoundToInt(Mathf.Sqrt((float)(dx * dx + dy * dy)));
    }

    /// <summary>
    /// Checks if there is a clear line of sight between two nodes on a hexagonal grid
    /// </summary>
    private bool LineOfSight(Node a, Node b)
    {
        if (a == null || b == null) return false;
        if (a == b) return true;
        
        // Get all hexes along the line between a and b
        List<Vector2Int> hexLine = HexLineDraw(a.X, a.Y, b.X, b.Y);
        
        // Check if all hexes along the line are traversable
        foreach (var hex in hexLine)
        {
            // Find the node with these coordinates
            Node node = FindNodeByCoordinates(hex.x, hex.y);
            if (node == null || !node.T)
                return false;
        }
        
        return true;
    }

    /// <summary>
    /// Finds a node in the grid by its actual X,Y coordinates
    /// </summary>
    private Node FindNodeByCoordinates(int x, int y)
    {
        int columns = NodeGrid.GetUpperBound(0) + 1;
        int rows = NodeGrid.GetUpperBound(1) + 1;
        
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (NodeGrid[i, j].X == x && NodeGrid[i, j].Y == y)
                    return NodeGrid[i, j];
            }
        }
        return null;
    }

    /// <summary>
    /// Draws a line between two hexes using linear interpolation in cube coordinates
    /// Returns all hex coordinates along the line.
    /// Uses supersampling (multiple samples per hex) to ensure no hexes are missed.
    /// </summary>
    private List<Vector2Int> HexLineDraw(int x0, int y0, int x1, int y1)
    {
        List<Vector2Int> results = new List<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>(); // Track unique hexes
        
        // Convert offset coordinates to cube coordinates
        Vector3 a = OffsetToCube(x0, y0);
        Vector3 b = OffsetToCube(x1, y1);
        
        // Calculate distance in hex space
        int distance = (int)((Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z)) / 2);
        
        // Supersample: Use 3 samples per hex distance unit to catch hexes near boundaries
        // This ensures the line doesn't miss hexes it passes through
        int samples = Math.Max(distance * 5, 1);
        
        // Interpolate along the line with higher sampling rate
        for (int i = 0; i <= samples; i++)
        {
            float t = samples == 0 ? 0f : (float)i / samples;
            Vector3 cubeCoord = CubeLerp(a, b, t);
            Vector3 rounded = CubeRound(cubeCoord);
            Vector2Int offsetCoord = CubeToOffset((int)rounded.x, (int)rounded.y, (int)rounded.z);
            
            // Only add if we haven't seen this hex before
            if (visited.Add(offsetCoord))
            {
                results.Add(offsetCoord);
            }
        }
        
        return results;
    }

    /// <summary>
    /// Convert even-q offset coordinates to cube coordinates
    /// </summary>
    private Vector3 OffsetToCube(int col, int row)
    {
        int x = col;
        int z = row - (col - (col & 1)) / 2;
        int y = -x - z;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Convert cube coordinates to even-q offset coordinates
    /// </summary>
    private Vector2Int CubeToOffset(int x, int y, int z)
    {
        int col = x;
        int row = z + (x - (x & 1)) / 2;
        return new Vector2Int(col, row);
    }

    /// <summary>
    /// Linear interpolation between two cube coordinates
    /// </summary>
    private Vector3 CubeLerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t,
            a.z + (b.z - a.z) * t
        );
    }

    /// <summary>
    /// Round fractional cube coordinates to nearest hex
    /// </summary>
    private Vector3 CubeRound(Vector3 cube)
    {
        int rx = Mathf.RoundToInt(cube.x);
        int ry = Mathf.RoundToInt(cube.y);
        int rz = Mathf.RoundToInt(cube.z);
        
        float xDiff = Math.Abs(rx - cube.x);
        float yDiff = Math.Abs(ry - cube.y);
        float zDiff = Math.Abs(rz - cube.z);
        
        // Re-calculate the component with the largest rounding difference
        // to maintain the constraint that x + y + z = 0
        if (xDiff > yDiff && xDiff > zDiff)
            rx = -ry - rz;
        else if (yDiff > zDiff)
            ry = -rx - rz;
        else
            rz = -rx - ry;
        
        return new Vector3(rx, ry, rz);
    }
}