using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject defensePrefab;
    [SerializeField] private GameObject defensePreviewPrefab;
    [SerializeField] private Tilemap hexTilemap;
    [SerializeField] private GameObject hexagonHighlightPrefab;

    private bool placingDefense = false;
    private GameObject defensePreviewInstance;
    private GameObject hexHighlightInstance;
    private Vector3Int lastHighlightedCell = new Vector3Int(int.MinValue, int.MinValue, 0);

    public void CreateDefense()
    {
        placingDefense = true;
        
        // Create preview at cursor position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        defensePreviewInstance = Instantiate(defensePreviewPrefab, mouseWorldPos, Quaternion.identity);
    }

    public void OnCancelPlacement()
    {
        placingDefense = false;
        
        // Clean up preview
        if (defensePreviewInstance != null)
        {
            Destroy(defensePreviewInstance);
        }
    }

    private void Update()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        
        // Update defense preview position to follow cursor
        if (placingDefense && defensePreviewInstance != null)
        {
            defensePreviewInstance.transform.position = mouseWorldPos;
        }

        // Highlight the hexagonal tile under the cursor
        if (hexTilemap != null && hexagonHighlightPrefab != null)
        {
            // Convert world position to cell coordinates
            Vector3Int cellPosition = hexTilemap.WorldToCell(mouseWorldPos);
            
            // Only update if we're over a different cell
            if (cellPosition != lastHighlightedCell)
            {
                lastHighlightedCell = cellPosition;
                
                // Get the center of the cell in world space
                Vector3 cellCenterWorld = hexTilemap.GetCellCenterWorld(cellPosition);
                
                // Create highlight GameObject if it doesn't exist
                if (hexHighlightInstance == null)
                {
                    hexHighlightInstance = Instantiate(hexagonHighlightPrefab);                    
                }
                
                // Position the highlight at the center of the hexagonal tile
                hexHighlightInstance.transform.position = cellCenterWorld;
            }
        }
    }

    // Called by the new Input System when the Click action is triggered
    public void OnClick(InputAction.CallbackContext context)
    {
        // Only respond when the button is pressed (not released)
        if (context.performed && placingDefense)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Instantiate(defensePrefab, mouseWorldPos, Quaternion.identity);
            
            // Clean up preview
            if (defensePreviewInstance != null)
            {
                Destroy(defensePreviewInstance);
            }
            
            placingDefense = false;
        }
    }

    // Called by the new Input System when the Cancel action is triggered
    public void OnCancel(InputAction.CallbackContext context)
    {
        // Only respond when the button is pressed (not released)
        if (context.performed && placingDefense)
        {
            // Clean up preview
            if (defensePreviewInstance != null)
            {
                Destroy(defensePreviewInstance);
            }
            
            placingDefense = false;
        }
    }
}
