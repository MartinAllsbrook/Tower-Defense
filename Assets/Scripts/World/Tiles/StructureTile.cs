using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Tile", menuName = "Tiles/Structure Tile")]
public class StructureTile : WorldTile
{
    [SerializeField] private Structure structure;

    public Structure Structure
    {
        get { return structure; }
    }
} 