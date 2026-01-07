using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class ThetaStar
{
    Node[,] nodeGrid;

    BoundsInt bounds;
    int columns;
    int rows;
    
    // Performance debugging
    public bool enablePerformanceDebugging = true;
    private int nodesExplored;
    private int losChecks;
    private int losChecksPassed;
    private int openSetPeakSize;
    private int closedSetSize;
    
    /// <summary>
    /// Initializes or updates the navigation grid based on the provided GridCell array and bounds
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="bounds"></param>
    public void UpdateNavGrid(GridCell[,] grid, BoundsInt bounds)
    {
        this.bounds = bounds;
        columns = bounds.size.x;
        rows = bounds.size.y;

        nodeGrid = new Node[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GridCell cell = grid[i, j];
                nodeGrid[i, j] = new Node(cell.Position.x, cell.Position.y, i, j, cell.Cost, cell.Traversable);
            }
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                nodeGrid[i, j].AddNeighbors(nodeGrid, bounds);
            }
        }
    }

    public List<Node> CreatePath(Vector2Int start, Vector2Int end)
    {
        // Reset performance counters
        nodesExplored = 0;
        losChecks = 0;
        losChecksPassed = 0;
        openSetPeakSize = 0;
        closedSetSize = 0;
        
        // Start performance timer
        Stopwatch stopwatch = enablePerformanceDebugging ? Stopwatch.StartNew() : null;
        
        // Reset nodes instead of copying
        ResetNodeGrid();
        
        // Initialize nodes
        Node End = null;
        Node Start = null;

        // Identify start/end nodes
        Vector2Int startGrid = TilemapToGrid(start);
        Vector2Int endGrid = TilemapToGrid(end);
        Start = nodeGrid[startGrid.x, startGrid.y];
        End = nodeGrid[endGrid.x, endGrid.y];

        // Validate start and end nodes+
        if (!IsValidPath(Start, End))
        {
            if (enablePerformanceDebugging && stopwatch != null)
            {
                stopwatch.Stop();
                UnityEngine.Debug.LogWarning($"[Theta*] Invalid path - Time: {stopwatch.ElapsedMilliseconds}ms");
            }
            return null;
        }

        // Theta* Algorithm
        MinHeap<Node> OpenSet = new MinHeap<Node>((a, b) => a.F.CompareTo(b.F));
        HashSet<Node> ClosedSet = new HashSet<Node>();

        OpenSet.Add(Start);
        Start.InOpenSet = true;
        Start.InOpenSet = true;

        while (OpenSet.Count > 0)
        {
            // Track peak open set size
            if (enablePerformanceDebugging && OpenSet.Count > openSetPeakSize)
                openSetPeakSize = OpenSet.Count;
            
            // The node in openSet with the lowest F value (MinHeap automatically maintains minimum at root)
            Node current = OpenSet.RemoveMin();
            
            // Track nodes explored
            if (enablePerformanceDebugging)
                nodesExplored++;

            // If we reached the end node, reconstruct and return the path
            if (End != null && current == End)
            {
                List<Node> path = RetracePath(Start, End);
                
                // Log performance metrics
                if (enablePerformanceDebugging && stopwatch != null)
                {
                    stopwatch.Stop();
                    UnityEngine.Debug.Log($"[Theta*] Path found!\n" +
                        $"  Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)\n" +
                        $"  Time / Length: {(path.Count > 0 ? (stopwatch.ElapsedTicks / (float)path.Count) : 0):F2} ticks/length\n" +
                        $"  Path Length: {path.Count} nodes\n" +
                        $"  Path Cost: {current.G:F2}\n" +
                        $"  Nodes Explored: {nodesExplored}\n" +
                        $"  Closed Set Size: {closedSetSize}\n" +
                        $"  Open Set Peak: {openSetPeakSize}\n" +
                        $"  LOS Checks: {losChecks} ({losChecksPassed} passed, {(losChecks > 0 ? (losChecksPassed * 100f / losChecks) : 0):F1}% success)\n" +
                        $"  Efficiency: {(nodesExplored > 0 ? (path.Count * 100f / nodesExplored) : 0):F1}% (path/explored)");
                }
                
                return path;
            }

            // If not at the end, continue searching
            current.InOpenSet = false;
            current.InClosedSet = true;
            ClosedSet.Add(current);
            
            // Track closed set size
            if (enablePerformanceDebugging)
                closedSetSize++;

            // Examine each neighbor of the current node (Theta* algorithm)
            List<Node> neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Node neighbor = neighbors[i];
                // Skip if neighbor is already evaluated (in ClosedSet) or is not traversable
                if (!neighbor.InClosedSet && neighbor.T)
                {
                    // Calculate cost via current node (standard A* path)
                    float distance = 1; // Approximate distance between adjacent hexes
                    float costViaCurrent = current.G + neighbor.C + distance;
                    
                    // The cost to reach this neighbor
                    float G;
                    // The would-be previous if we take this path
                    Node pathParent;
                    
                    // Check if there is line of sight from current's previous to this neighbor
                    Node previous = current.previous;
                    bool hasLOS = previous != null && LineOfSight(nodeGrid, previous, neighbor);
                    if (hasLOS)
                    {
                        // Calculate cost via line of sight path (Theta* optimization)
                        float costViaLOS = previous.G + CalculatePathCost(nodeGrid, previous, neighbor);
                        
                        // Choose the path with lower cost
                        if (costViaLOS <= costViaCurrent)
                        {
                            G = costViaLOS;
                            pathParent = previous;
                        }
                        else
                        {
                            G = costViaCurrent;
                            pathParent = current;
                        }
                    }
                    else
                    {
                        // No line of sight, must go through current
                        G = costViaCurrent;
                        pathParent = current;
                    }

                    bool newPath = false;
                    // If the neighbor is already discovered
                    if (neighbor.InOpenSet) 
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
                        neighbor.InOpenSet = true;
                    }
                    
                    if (newPath) // If we found a better or new path to this neighbor
                    {
                        neighbor.H = Heuristic(neighbor, End); // Calculate heuristic (estimated cost to goal)
                        neighbor.F = neighbor.G + neighbor.H; // Total estimated cost (actual cost + heuristic)
                        neighbor.previous = pathParent; // Track the path (may skip current node if LOS exists)
                        
                        // If we updated an existing node in the heap, reheapify to maintain heap property
                        if (neighbor.InOpenSet)
                            OpenSet.UpdatePriority(neighbor);
                    }
                }
            }

        }
        
        // No path found - log performance metrics
        if (enablePerformanceDebugging && stopwatch != null)
        {
            stopwatch.Stop();
            UnityEngine.Debug.LogWarning($"[Theta*] No path found!\n" +
                $"  Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks)\n" +
                $"  Nodes Explored: {nodesExplored}\n" +
                $"  Closed Set Size: {closedSetSize}\n" +
                $"  Open Set Peak: {openSetPeakSize}\n" +
                $"  LOS Checks: {losChecks} ({losChecksPassed} passed)");
        }
        
        return null;
    }

    #region General A* / Theta* Helpers

    // Reset nodes instead of copying
    private void ResetNodeGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Node node = nodeGrid[i, j];
                node.ResetPathfindingData();
            }
        }
    }

    private int Heuristic(Node a, Node b)
    {
        // For hex grids with node traversal costs, use hex distance
        // This provides an admissible heuristic (never overestimates)
        // Convert to cube coordinates and calculate hex distance
        Vector3Int cubeA = HexBresenham.OffsetToCube(a.CellX, a.CellY);
        Vector3Int cubeB = HexBresenham.OffsetToCube(b.CellX, b.CellY);
        return (Math.Abs(cubeA.x - cubeB.x) + Math.Abs(cubeA.y - cubeB.y) + Math.Abs(cubeA.z - cubeB.z)) / 2;
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
    private bool LineOfSight(Node[,] nodeGrid, Node a, Node b)
    {
        // Track LOS check count
        if (enablePerformanceDebugging)
            losChecks++;
        
        if (a == null || b == null) return false;
        if (a == b) return true;
        
        // Get all hexes along the line between a and b
        List<Vector2Int> hexLine = HexBresenham.HexLineDraw(a.CellX, a.CellY, b.CellX, b.CellY);
        
        // Check if all hexes along the line are traversable
        foreach (var hex in hexLine)
        {
            // Find the node with these coordinates
            Node node = FindNodeByCoordinates(nodeGrid, hex.x, hex.y);
            if (node == null || !node.T)
                return false;
        }
        
        // Track successful LOS checks
        if (enablePerformanceDebugging)
            losChecksPassed++;
        
        return true;
    }

    /// <summary>
    /// Calculates the total cost of moving from node a to node b,
    /// by summing the traversal cost of all nodes along the path
    /// </summary>
    private float CalculatePathCost(Node[,] nodeGrid, Node a, Node b)
    {
        if (a == null || b == null) return int.MaxValue;
        if (a == b) return 0;
        
        // Get all hexes along the line between a and b
        List<Vector2Int> hexLine = HexBresenham.HexLineDraw(a.CellX, a.CellY, b.CellX, b.CellY);

        // Sum up the traversal cost of nodes along the path
        // Skip index 0 (node 'a') because its cost is already in a.G
        // We only pay the cost for nodes we're entering, not the node we're leaving from
        float traversalCost = 0;
        for (int i = 1; i < hexLine.Count; i++)
        {
            var hex = hexLine[i];
            Node node = FindNodeByCoordinates(nodeGrid, hex.x, hex.y);
            if (node != null)
            {
                traversalCost += node.C;
            }
        }

        // Add true Euclidean distance as movement cost
        // Convert hex offset coordinates to world coordinates for flat-top hexagons
        float hexWidth = 1.5f; // Horizontal distance between hex centers (normalized)
        float hexHeight = Mathf.Sqrt(3); // Vertical distance (normalized)
        
        float x1 = a.CellX * hexWidth;
        float y1 = a.CellY * hexHeight + (a.CellX % 2) * (hexHeight * 0.5f);
        float x2 = b.CellX * hexWidth;
        float y2 = b.CellY * hexHeight + (b.CellX % 2) * (hexHeight * 0.5f);
        
        float distance = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        traversalCost += distance;


        return traversalCost;
    }

    #region Grid Conversion Helpers

    /// <summary>
    /// Finds a node in the grid by its actual X,Y coordinates
    /// </summary>
    private Node FindNodeByCoordinates(Node[,] nodeGrid, int x, int y)
    {
        Vector2Int gridPos = TilemapToGrid(new Vector2Int(x, y));
        if (gridPos.x >= 0 && gridPos.x < nodeGrid.GetLength(0) &&
            gridPos.y >= 0 && gridPos.y < nodeGrid.GetLength(1))
        {
            return nodeGrid[gridPos.x, gridPos.y];
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