using UnityEngine;

[CreateAssetMenu(fileName = "New StructureData Tile", menuName = "Tiles/StructureData Tile")]
public class StructureTile : WorldTile
{
    [SerializeField] private StructureData structure;

    public StructureData StructureData
    {
        get { return structure; }
    }
} 