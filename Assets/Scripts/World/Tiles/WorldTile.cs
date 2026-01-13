using UnityEngine;

public enum TileTag
{
    Ground,
    Structure,
    Terrain,
}

[CreateAssetMenu(fileName = "New World Tile", menuName = "Tiles/World Tile")]
public class WorldTile : HexagonalRuleTile
{    
    [SerializeField] protected TileTag tag;

    public TileTag Tag
    {
        get { return tag; }
    }
}
