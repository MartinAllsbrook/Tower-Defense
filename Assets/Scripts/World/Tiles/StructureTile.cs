using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Tile", menuName = "Tiles/Structure Tile")]
public class StructureTile : WorldTile
{
    [SerializeField] private StructureData structureData;

    public StructureData StructureData
    {
        get { return structureData; }
    }
} 