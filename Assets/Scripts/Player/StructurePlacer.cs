using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class StructurePlacer : MonoBehaviour
{
    enum Mode
    {
        None,
        Placing,
        Removing
    }

    [SerializeField] World world;
    [SerializeField] StructureTile[] structures;
    [SerializeField] RuleTile removeIconTile;

    public StructureTile[] Structures => structures;
    
    Player player;
    Tilemap previewTilemap;
    StructureTile currentStructure;
    Mode mode = Mode.None;
    bool basePlaced = false; // To ensure only one base is placed
    bool mouseDown = false;

    void Awake()
    {
        player = GetComponent<Player>();
        previewTilemap = GameObject.FindWithTag("Preview Tilemap").GetComponent<Tilemap>();
    }

    void Start()
    {
        EnterPlaceMode((int)StructureType.Base);
    }

    void Update()
    {
        if (mode == Mode.Placing && currentStructure != null)
            PlaceUpdate();
        else if (mode == Mode.Removing)
            RemoveUpdate();
    }

    #region Place & Remove

    void PlaceUpdate()
    {
        Vector3Int mouseGridPos = GetMouseCell();

        // Update preview tilemap
        previewTilemap.ClearAllTiles();
        if (world.IsWithinBounds(mouseGridPos))
        {
            previewTilemap.SetTile(mouseGridPos, currentStructure);
            Structure structure = previewTilemap.GetInstantiatedObject(mouseGridPos).GetComponent<Structure>();
            
            if (structure != null)
            {
                structure.SetAsVisualPreview(CanAffordStructure(currentStructure));
            }

            if (mouseDown)
            {
                PlaceStructureAt(mouseGridPos, currentStructure);
            }
        }
    }

    bool PlaceStructureAt(Vector3Int cellPosition, StructureTile structureData)
    {
        // Special case for placing the base
        if (currentStructure.ID == StructureType.Base)
        {
            if (basePlaced)
                return false;

            basePlaced = world.SetTileAt(cellPosition, currentStructure);

            if (basePlaced)
            {    
                GameController.PlaceBase();
                ExitMode();
            }

            return basePlaced;
        }
        
        // For other structures, just place normally
        if (!CanAffordStructure(structureData)) // Just a little safety check even though next if should handle it
            return false;

        if (world.SetTileAt(cellPosition, structureData))
        {
            player.SpendMoney(structureData.Cost);
            return true;
        }

        return false;
    }

    void RemoveUpdate()
    {
        Vector3Int mouseGridPos = GetMouseCell();

        // Update preview tilemap
        previewTilemap.ClearAllTiles();
        if (world.IsWithinBounds(mouseGridPos))
        {
            previewTilemap.SetTile(mouseGridPos, removeIconTile);
            if (mouseDown)
            {
                RemoveStructureAt(mouseGridPos);
            }
        }
    }

    bool RemoveStructureAt(Vector3Int cellPosition)
    {
        return world.SetTileAt(cellPosition, null);
    }

    #endregion

    #region State (Mode) Management

    public void EnterPlaceMode(StructureType structureID)
    {
        currentStructure = GetStructureByType(structureID);
        mode = Mode.Placing;
    }

    public void EnterRemoveMode()
    {
        currentStructure = null;
        mode = Mode.Removing;
    }

    public void ExitMode(InputAction.CallbackContext context)
    {
        if (context.performed)
            ExitMode();
    }

    public void ExitMode()
    {
        previewTilemap.ClearAllTiles();
        currentStructure = null;
        mode = Mode.None;
    }

    #endregion

    #region Input Handling
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            mouseDown = true;
        }
        else if (context.canceled)
        {
            mouseDown = false;
        }
    }
    #endregion

    #region Utility Methods

    Vector3Int GetMouseCell()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        return world.WorldToCell(mouseWorldPos);
    }

    StructureTile GetStructureByType(StructureType type)
    {
        foreach (var structure in structures)
        {
            if (structure.ID == type)
            {
                return structure;
            }
        }
        return null;
    }

    public bool CanAffordStructure(StructureType type)
    {
        StructureTile tile = GetStructureByType(type);
        if (player != null && tile != null)
        {
            return player.GetMoney() >= tile.Cost;
        }
        return false;
    }

    public bool CanAffordStructure(StructureTile tile)
    {
        if (player != null && tile != null)
        {
            return player.GetMoney() >= tile.Cost;
        }
        return false;
    }

    #endregion

}
