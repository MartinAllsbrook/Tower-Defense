using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum StructureType
{
    Base = 0,
    Wall = 10,
    Turret = 100,
}

[CreateAssetMenu(fileName = "New Structure Tile", menuName = "Tiles/Structure Tile")]
public class StructureTile : WorldTile
{
    [SerializeField] StructureType id;

    [Tooltip("Lower number means higher priority")]
    [SerializeField] int priority; 
    [SerializeField] float maxHealth;
    [SerializeField] Sprite icon;
    [SerializeField] string structureName;
    [SerializeField] int cost;

    // Data accessors
    public StructureType ID => id;
    public int Priority => priority;
    public float MaxHealth => maxHealth;
    public Sprite Icon => icon;
    public string Name => structureName;
    public int Cost => cost;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        
        // When the tile is placed, the GameObject gets instantiated by the Tilemap
        // We hook into this to automatically set the Structure's tile field
        if (tileData.gameObject != null)
        {
            Structure structure = tileData.gameObject.GetComponent<Structure>();
            if (structure != null)
            {
                structure.SetTile(this);
            }
        }
    }
}

