
using UnityEngine;

public struct GridCell
{
    // The world (Tilemap) position of this cell
    public Vector2Int Position;
    
    // The cost to traverse this cell
    public int Cost;
    
    // Whether this cell can be traversed
    public bool Traversable;
}