using System;
using System.Collections;
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
    [SerializeField] VariedAudioClip placeSound;
    [SerializeField] LayerMask structureLayer;
    [SerializeField] UpgradeMenu upgradeMenu;

    public StructureTile[] Structures => structures;    

    Player player;
    SwizzledHFTTilemap previewTilemap;
    StructureTile currentStructure;
    Mode mode = Mode.None;
    bool basePlaced = false; // To ensure only one base is placed
    bool mouseDown = false;

    void Awake()
    {
        player = GetComponent<Player>();
        previewTilemap = GameObject.FindWithTag("Preview Tilemap").GetComponent<SwizzledHFTTilemap>();
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

    void OnEnable()
    {
        if (InputReader.Instance != null)
        {
            InputReader.Instance.OnCancel += ExitMode;
            InputReader.Instance.OnClick += Click;
            InputReader.Instance.OnRelease += Release;
            return;
        }

        StartCoroutine(SubscribeWhenReady());
    }

    IEnumerator SubscribeWhenReady()
    {
        while (InputReader.Instance == null)
            yield return null;

        InputReader.Instance.OnCancel += ExitMode;
        InputReader.Instance.OnClick += Click;
        InputReader.Instance.OnRelease += Release;
    }

    void OnDisable()
    {
        InputReader.Instance.OnCancel -= ExitMode;
        InputReader.Instance.OnClick -= Click;
        InputReader.Instance.OnRelease -= Release;
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
            Target baseTarget = world.GetStructureAt(new Vector2Int(cellPosition.x, cellPosition.y)) as Target;
            Player.Instance.SetTarget(baseTarget);

            if (basePlaced)
            {    
                AudioManager.PlayAudioAt(placeSound, transform.position); // TODO: Play at position of structure?
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
            GameController.PlaceStructure();

            AudioManager.PlayAudioAt(placeSound, transform.position); // TODO: Play at position of structure?
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
        Structure structure= world.GetStructureAt(new Vector2Int(cellPosition.x, cellPosition.y));
        if (structure == null || structure.Tile.ID == StructureType.Base)
            return false;
            
        return world.SetTileAt(cellPosition, null);
    }

    #endregion

    #region Controls & State

    public void ExitMode()
    {
        // If we have nothing to cancel, just ignore and let the InputReader know
        if (currentStructure == null && !upgradeMenu.gameObject.activeSelf)
        {
            InputReader.Instance.SkipCancel();
            return;
        }

        // Prevent exiting place mode if base not yet placed
        if (currentStructure != null && currentStructure.ID == StructureType.Base && !basePlaced)
            return;

        upgradeMenu.Close();
        previewTilemap.ClearAllTiles();
        currentStructure = null;
        mode = Mode.None;
    }

    void Click()
    {
        mouseDown = true;

        // Raycast from camera to mouse position, check for hit on structure layer
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 rayOrigin = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        float rayDistance = 100f;
        // Replace 'structureLayer' with the correct LayerMask for structures
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, rayDistance, structureLayer);
        if (hit.collider != null)
        {
            // Check if the hit object is a Defense (Turret) instance
            Turret turret = hit.collider.GetComponent<Turret>();
            if (turret != null)
            {
                upgradeMenu.Open(turret.GetComponent<TurretStats>().GetAvailableUpgrades(), turret.GetComponent<TurretStats>());
            }
        }
    }

    void Release ()
    {
        mouseDown = false;
    }

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
