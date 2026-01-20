using UnityEngine;

public enum BiomeID
{
    grass,
    sand,
    stone,
    snow,
    bug1 = 100,
    bug2 = 101,
    bug3 = 102,
    bug4 = 103,
}

[CreateAssetMenu(fileName = "New Biome Tile", menuName = "Tiles/Biome Tile")]
public class BiomeTile : HexagonalRuleTile
{    
    public BiomeID tag;
}
