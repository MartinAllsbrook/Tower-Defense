using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public enum StructureType
{
    Base = 0,
    Wall = 10,
    Turret = 100,
}

public class StructurePlacer : MonoBehaviour
{
    enum Mode
    {
        None,
        Placing,
        Removing
    }

    [SerializeField] World world;
    [SerializeField] StructureData[] structures;
    [SerializeField] RuleTile removeIconTile;

    Tilemap previewTilemap;
    StructureData currentStructure;
    Mode mode = Mode.None;
    bool basePlaced = false; // To ensure only one base is placed

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
        {
            // Get mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Vector3Int mouseGridPos = previewTilemap.WorldToCell(mouseWorldPos);

            // Update preview tilemap
            previewTilemap.ClearAllTiles();
            previewTilemap.SetTile(mouseGridPos, currentStructure.tile);
        }
        else if (mode == Mode.Removing)
        {
            // Get mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Vector3Int mouseGridPos = previewTilemap.WorldToCell(mouseWorldPos);

            // Update preview tilemap
            previewTilemap.ClearAllTiles();
            previewTilemap.SetTile(mouseGridPos, removeIconTile);
        }
    }

    // This method can be called from a Unity UI Button event
    public void EnterPlaceMode(int structureID)
    {
        currentStructure = GetStructureByType((StructureType)structureID);
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

    public void PlaceStructure(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool mapChanged = false;
            if (mode == Mode.Placing && currentStructure != null)
            {
                // Special case for placing the base
                if (currentStructure.objectType == StructureType.Base)
                {
                    if (basePlaced)
                        return;

                    mapChanged = world.SetTileAt(GetMouseCell(), currentStructure.tile);

                    if (mapChanged)
                    {    
                        FindFirstObjectByType<GameController>().PlaceBase();
                        basePlaced = true;
                        ExitMode();
                    }
                }
                else
                {
                    mapChanged = world.SetTileAt(GetMouseCell(), currentStructure.tile);
                }
            }
            else if (mode == Mode.Removing)
            {
                mapChanged = world.SetTileAt(GetMouseCell(), null);
            }

            if (mapChanged)
                world.UpdateTilemap();
        } 
    }

    StructureData GetStructureByType(StructureType type)
    {
        foreach (var structure in structures)
        {
            if (structure.objectType == type)
            {
                return structure;
            }
        }
        return null;
    }

    #region Helper Methods
    
    Vector3Int GetMouseCell()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        return world.WorldToCell(mouseWorldPos);
    }
    
    #endregion
}
