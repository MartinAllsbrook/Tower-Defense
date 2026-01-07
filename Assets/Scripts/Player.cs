using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [SerializeField] private Tilemap hexTilemap;
    [SerializeField] private GameObject hexagonHighlightPrefab;

    private GameObject hexHighlightInstance;
    private Vector3Int lastHighlightedCell = new Vector3Int(int.MinValue, int.MinValue, 0);

    private void Update()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

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
}
