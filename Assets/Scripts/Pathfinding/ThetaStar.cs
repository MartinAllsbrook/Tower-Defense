using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThetaStar
{
    public Node[,] NodeGrid;
    BoundsInt bounds;

    public ThetaStar()
    {
        
    }
    
    private bool IsValidPath(Node start, Node end)
    {
        if (end == null)
            return false;
        if (start == null)
            return false;
        if (!end.T)
            return false;
        return true;
    }

    public List<Node> CreatePath(GridCell[,] grid, BoundsInt bounds, Vector2Int start, Vector2Int end)
    {
        // Initialize nodes
        Node End = null;
        Node Start = null;
        this.bounds = bounds;
        int columns = bounds.size.x;
        int rows = bounds.size.y;
        NodeGrid = new Node[columns, rows];

        // Create nodes based on grid data
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GridCell cell = grid[i, j];
                NodeGrid[i, j] = new Node(cell.Position.x, cell.Position.y, cell.Cost, cell.Traversable);
            }
        }

        // Add neighbors 
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                NodeGrid[i, j].AddNeighbors(NodeGrid, bounds);
            }
        }

        // Identify start/end nodes
        Vector2Int startGrid = TilemapToGrid(start);
        Vector2Int endGrid = TilemapToGrid(end);
        Start = NodeGrid[startGrid.x, startGrid.y];
        End = NodeGrid[endGrid.x, endGrid.y];

        // Validate start and end nodes+
        if (!IsValidPath(Start, End))
            return null;

        // Theta* Algorithm
        List<Node> OpenSet = new List<Node>();
        List<Node> ClosedSet = new List<Node>();

        OpenSet.Add(Start);

        while (OpenSet.Count > 0)
        {
            // Find smallest F within the open set
            int winnerIndex = 0;
            for (int i = 0; i < OpenSet.Count; i++)
                if (OpenSet[i].F < OpenSet[winnerIndex].F)
                    winnerIndex = i;
                else if (OpenSet[i].F == OpenSet[winnerIndex].F) // TODO: Make this tie breaking for faster routing better
                    if (OpenSet[i].H < OpenSet[winnerIndex].H)
                        winnerIndex = i;

            // The node in openSet with the lowest F value
            Node current = OpenSet[winnerIndex]; 

            // If we reached the end node, reconstruct and return the path
            if (End != null && current == End)
            {
                Debug.Log("Path found, cost: " + current.G);
                return RetracePath(Start, End);
            }

            // If not at the end, continue searching
            OpenSet.Remove(current);
            ClosedSet.Add(current);

            // Examine each neighbor of the current node (Theta* algorithm)
            List<Node> neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Node neighbor = neighbors[i];
                // Skip if neighbor is already evaluated (in ClosedSet) or is not traversable
                if (!ClosedSet.Contains(neighbor) && neighbor.T)
                {
                    Node parent = current.previous;
                    // The cost to reach this neighbor
                    float G;
                    // The would-be parent if we take this path
                    Node pathParent;
                    
                    // if (parent != null && LineOfSight(parent, neighbor))
                    // {
                    //     // Path 2: Direct line from parent to neighbor (skip current node)
                    //     // Cost = parent's accumulated cost + sum of traversal costs along the line
                    //     G = parent.G + CalculatePathCost(parent, neighbor);
                    //     pathParent = parent;
                    // }
                    else
                    {
                        float distance = 0.866f; // Approximate distance between adjacent hexes
                        G = current.G + neighbor.C + distance; // Cost from start to neighbor via current
                        pathParent = current;
                    }

                    bool newPath = false;
                    // If the neighbor is already discovered
                    if (OpenSet.Contains(neighbor)) 
                    {
                        // Check if this path to the neighbor is better than the previously found path
                        if (G < neighbor.G)
                        {
                            // Update to the better (shorter) path cost
                            neighbor.G = G; 
                            newPath = true;
                        }
                    }
                    else // Neighbor has not been discovered yet
                    {
                        neighbor.G = G; // Set initial path cost
                        newPath = true;
                        OpenSet.Add(neighbor); // Add to nodes to be evaluated
                    }
                    
                    if (newPath) // If we found a better or new path to this neighbor
                    {
                        neighbor.H = Heuristic(neighbor, End); // Calculate heuristic (estimated cost to goal)
                        neighbor.F = neighbor.G + neighbor.H; // Total estimated cost (actual cost + heuristic)
                        neighbor.previous = pathParent; // Track the path (may skip current node if LOS exists)
                    }
                }
            }

        }
        return null;
    }

    private int Heuristic(Node a, Node b)
    {
        // For hex grids with node traversal costs, use hex distance
        // This provides an admissible heuristic (never overestimates)
        // Convert to cube coordinates and calculate hex distance
        Vector3 cubeA = OffsetToCube(a.X, a.Y);
        Vector3 cubeB = OffsetToCube(b.X, b.Y);
        return (int)((Math.Abs(cubeA.x - cubeB.x) + Math.Abs(cubeA.y - cubeB.y) + Math.Abs(cubeA.z - cubeB.z)) / 2);
    }


    #region General A* / Theta* Helpers

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }
        path.Reverse();
        return path;
    }

    #endregion 

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
    /// Calculates the total cost of moving from node a to node b,
    /// by summing the traversal cost of all nodes along the path
    /// </summary>
    private float CalculatePathCost(Node a, Node b)
    {
        if (a == null || b == null) return int.MaxValue;
        if (a == b) return 0;
        
        // Get all hexes along the line between a and b
        List<Vector2Int> hexLine = HexLineDraw(a.X, a.Y, b.X, b.Y);
        
        // Sum up the traversal cost of nodes along the path
        // Skip index 0 (node 'a') because its cost is already in a.G
        // We only pay the cost for nodes we're entering, not the node we're leaving from
        float traversalCost = 0;
        for (int i = 1; i < hexLine.Count; i++)
        {
            var hex = hexLine[i];
            Node node = FindNodeByCoordinates(hex.x, hex.y);
            if (node != null)
            {
                traversalCost += node.C;
            }
        }
        
        return traversalCost;
    }

    #region Hex Line Drawing Helpers

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

    #endregion

    #region Grid Conversion Helpers

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

    private Vector2Int GridToTilemap(Vector2Int gridPos)
    {
        int tilemapX = gridPos.x + bounds.xMin;
        int tilemapY = gridPos.y + bounds.yMin;
        return new Vector2Int(tilemapX, tilemapY);
    }

    private Vector2Int TilemapToGrid(Vector2Int tilemapPos)
    {
        int gridX = tilemapPos.x - bounds.xMin;
        int gridY = tilemapPos.y - bounds.yMin;
        return new Vector2Int(gridX, gridY);
    }

    #endregion
}