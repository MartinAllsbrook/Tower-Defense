using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public enum StructureType
{
    Wall = 0,
    Turret = 100,
}

public class StructurePlacer : MonoBehaviour
{
    [SerializeField] Structure[] structures;

    Tilemap previewTilemap;
    Tilemap worldTilemap;

    private Structure currentStructure;
    private bool isPlacingStructure = false;

    void Start()
    {
        previewTilemap = GameObject.FindWithTag("Preview Tilemap").GetComponent<Tilemap>();
        worldTilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();
    }

    void Update()
    {
        if (isPlacingStructure && currentStructure != null)
        {
            // Get mouse position in world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Vector3Int mouseGridPos = previewTilemap.WorldToCell(mouseWorldPos);

            // Update preview tilemap
            previewTilemap.ClearAllTiles();
            previewTilemap.SetTile(mouseGridPos, currentStructure.tile);
        }
    }

    // This method can be called from a Unity UI Button event
    public void EnterPlaceMode(int structureID)
    {
        currentStructure = GetStructureByType((StructureType)structureID);
        isPlacingStructure = true;
        Debug.Log("Entering place mode for structure type: " + currentStructure.name);
    }

    public void ExitPlaceMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            previewTilemap.ClearAllTiles();
            currentStructure = default;
            isPlacingStructure = false;
            Debug.Log("Exiting place mode");   
        }
    }

    public void PlaceStructure(InputAction.CallbackContext context)
    {
        if (context.performed && isPlacingStructure)
        {
            Vector3Int cellPosition = GetMouseGridPosition(worldTilemap);
            worldTilemap.SetTile(cellPosition, currentStructure.tile);
        }
    }

    Structure GetStructureByType(StructureType type)
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

    void CreateStructure(StructureType type, Vector3 position)
    {
        
    }

    #region Helper Methods
    
    Vector3Int GetMouseGridPosition(Tilemap tilemap)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        return tilemap.WorldToCell(mouseWorldPos);
    }
    
    #endregion
}
