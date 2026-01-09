using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;

class ThetaStarNew
{
    NodeNew[,] grid;
    int width;
    int height;
    /// <summary>
    /// The lower-left coordinate of the tilemap, or 0,0 in grid space.
    /// Used to offset world positions to grid indices, and vice versa.
    /// </summary>
    Vector2Int gridOffset;

    public ThetaStarNew(GridCell[,] worldGrid, Vector2Int offset)
    {
        gridOffset = offset;

        width = worldGrid.GetLength(0);
        height = worldGrid.GetLength(1);
        grid = new NodeNew[width, height];
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

    public Path CreatePath(Vector2Int startPos, Vector2Int endPos)
    {
        NodeNew startNode = grid[startPos.x - gridOffset.x, startPos.y - gridOffset.y];
        NodeNew endNode = grid[endPos.x - gridOffset.x, endPos.y - gridOffset.y];

        // TODO: Validation?

        MinHeap<NodeNew> openSet = new MinHeap<NodeNew>((a, b) => a.F.CompareTo(b.F));
        HashSet<NodeNew> closedSet = new HashSet<NodeNew>();

        startNode.G = 0;
        startNode.H = Heuristic(startNode, endNode);
        startNode.F = startNode.G + startNode.H;
        openSet.Add(startNode);
        startNode.InOpenSet = true;

        while (openSet.Count > 0)
        {
            NodeNew current = openSet.RemoveMin();
            current.InOpenSet = false;
            
            if (current.InClosedSet)
                continue; // Already processed this node
            
            closedSet.Add(current);
            current.InClosedSet = true;

            // If we reached the end, reconstruct path
            if (current.X == endNode.X && current.Y == endNode.Y)
            {
                List<NodeNew> path = new List<NodeNew>();
                NodeNew temp = current;
                while (temp.previous.x != -1 && temp.previous.y != -1)
                {
                    path.Add(temp);
                    temp = grid[temp.previous.x, temp.previous.y];
                }
                path.Add(startNode);
                path.Reverse();
                Debug.Log($"Path {path} found with {path.Count} nodes.");

                return new Path
                {
                    valid = true,
                    cost = current.G,
                    nodes = path
                };
            }

            NodeNew[] neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Length; i++)
            {
                NodeNew neighbor = neighbors[i];
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
        return new Path
        {
            valid = false,
            cost = float.MaxValue,
            nodes = null
        };
    }

    float Heuristic(NodeNew a, NodeNew b)
    {
        // Using Euclidean distance as heuristic
        return Vector2Int.Distance(new Vector2Int(a.X, a.Y), new Vector2Int(b.X, b.Y));
    }

    NodeNew InitializeNode(int x, int y, float cost, bool traversable)
    {
        return new NodeNew
        {
            X = x,
            Y = y,
            T = traversable,
            C = cost,
            F = 0f,
            G = 0f,
            H = 0f,
            Neighbors = new NodeNew[0],
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

        int tilemapX = position.x + gridOffset.x;
        int[][] offsets = (tilemapX % 2 == 0) ? evenColOffsets : oddColOffsets;

        List<NodeNew> neighbors = new List<NodeNew>();
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