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

    Tilemap previewTilemap;
    StructureTile currentStructure;
    Mode mode = Mode.None;
    bool basePlaced = false; // To ensure only one base is placed
    bool mouseDown = false;

    void Awake()
    {
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
                FindFirstObjectByType<GameController>().PlaceBase();
                ExitMode();
            }

            return basePlaced;
        }
        
        // For other structures, just place normally
        return world.SetTileAt(cellPosition, currentStructure);
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

}
