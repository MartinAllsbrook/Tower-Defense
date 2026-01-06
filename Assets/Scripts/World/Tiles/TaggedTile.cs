using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileTag
{
    Ground,
    Structure,
    Terrain,
}

[CreateAssetMenu(fileName = "New Tagged Tile", menuName = "Tiles/Tagged Tile")]
public class TaggedTile : HexagonalRuleTile
{    
    public TileTag tag;
}
