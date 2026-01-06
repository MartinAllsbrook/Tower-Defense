using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BiomeID
{
    grass,
    sand,
    stone,
    cursed
}

[CreateAssetMenu(fileName = "New Biome Tile", menuName = "Tiles/Biome Tile")]
public class BiomeTile : HexagonalRuleTile
{    
    public BiomeID tag;
}
