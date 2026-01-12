using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;

class ThetaStar
{
    Node[,] grid;
    int width;
    int height;
    /// <summary>
    /// The lower-left coordinate of the tilemap, or 0,0 in grid space.
    /// Used to offset world positions to grid indices, and vice versa.
    /// </summary>
    Vector2Int gridOffset;

    public ThetaStar(GridCell[,] worldGrid, Vector2Int offset)
    {
        gridOffset = offset;

        width = worldGrid.GetLength(0);
        height = worldGrid.GetLength(1);
        grid = new Node[width, height];
        for (int x = 0; x < worldGrid.GetLength(0); x++)
        {
            for (int y = 0; y < worldGrid.GetLength(1); y++)
            {
                GridCell cell = worldGrid[x, y];
                grid[x, y] = InitializeNode(x, y, cell.Cost, cell.Traversable);
            }
        }

        // After all nodes are initialized, add neighbors
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                AddNeighbors(new Vector2Int(x, y));
            }
        }
    }

    /// <summary>
    /// Creates a path from startPos to endPos using the Theta* algorithm.
    /// </summary>
    /// <param name="startPos">The start point of the path in tilemap space</param>
    /// <param name="endPos">The end point of the path in tilemap space</param>
    /// <returns>A list of nodes, representing the path. Node coordinates are in grid space (need to be converted)</returns>
    public List<Vector2Int> CreatePath(Vector2Int startPos, Vector2Int endPos)
    {
        Node startNode = grid[startPos.x - gridOffset.x, startPos.y - gridOffset.y];
        Node endNode = grid[endPos.x - gridOffset.x, endPos.y - gridOffset.y];

        // TODO: Validation?

        MinHeap<Node> openSet = new MinHeap<Node>((a, b) => a.F.CompareTo(b.F));
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.G = 0;
        startNode.H = Heuristic(startNode, endNode);
        startNode.F = startNode.G + startNode.H;
        openSet.Add(startNode);
        startNode.InOpenSet = true;

        while (openSet.Count > 0)
        {
            Node current = openSet.RemoveMin();
            current.InOpenSet = false;
            
            if (current.InClosedSet)
                continue; // Already processed this node
            
            closedSet.Add(current);
            current.InClosedSet = true;

            // If we reached the end, reconstruct path
            if (current.X == endNode.X && current.Y == endNode.Y)
            {
                List<Node> path = new List<Node>();
                Node temp = current;
                while (temp.previous.x != -1 && temp.previous.y != -1)
                {
                    path.Add(temp);
                    temp = grid[temp.previous.x, temp.previous.y];
                }
                path.Add(startNode);
                path.Reverse();
                Debug.Log($"Path {path} found with {path.Count} nodes.");

                List<Vector2Int> tilemapPath = new List<Vector2Int>();
                foreach (Node node in path)
                {
                    tilemapPath.Add(new Vector2Int(node.X + gridOffset.x, node.Y + gridOffset.y));
                }

                return tilemapPath;
            }

            Node[] neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Length; i++)
            {
                Node neighbor = neighbors[i];
                if (neighbor.InClosedSet || !neighbor.T)
                {
                    continue; // Skip if already evaluated or not traversable
                }

                float tentativeG = current.G + neighbor.C;

                if (!neighbor.InOpenSet)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor, endNode);
                    neighbor.F = neighbor.G + neighbor.H;
                    neighbor.previous = new Vector2Int(current.X, current.Y);
                    
                    openSet.Add(neighbor);
                    neighbor.InOpenSet = true;
                }
                else if (tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor, endNode);
                    neighbor.F = neighbor.G + neighbor.H;
                    neighbor.previous = new Vector2Int(current.X, current.Y);
                    
                    openSet.UpdatePriority(neighbor);
                }
            }
        }

        // No path found
        Debug.LogWarning($"No path found from {startPos} to {endPos}.");
        return null;
    }

    float Heuristic(Node a, Node b)
    {
        // Using Euclidean distance as heuristic
        return Vector2Int.Distance(new Vector2Int(a.X, a.Y), new Vector2Int(b.X, b.Y));
    }

    Node InitializeNode(int x, int y, float cost, bool traversable)
    {
        return new Node
        {
            X = x,
            Y = y,
            T = traversable,
            C = cost,
            F = 0f,
            G = 0f,
            H = 0f,
            Neighbors = new Node[0],
            previous = new Vector2Int(-1, -1),
            InOpenSet = false,
            InClosedSet = false
        };
    }

    void AddNeighbors(Vector2Int position)
    {
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

        int tilemapY = position.y + gridOffset.y;
        int[][] offsets = (tilemapY % 2 == 0) ? evenColOffsets : oddColOffsets;

        List<Node> neighbors = new List<Node>();
        foreach (int[] offset in offsets)
        {
            int neighborX = position.x + offset[0];
            int neighborY = position.y + offset[1];

            // Correct bounds check
            if (neighborX >= 0 && neighborX < width &&
                neighborY >= 0 && neighborY < height)
            {
                // Add neighbor
                neighbors.Add(grid[neighborX, neighborY]);
            }
        }

        grid[position.x, position.y].Neighbors = neighbors.ToArray();
    }
}