using QFSW.QC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Improved version of GridDebug focused on debugging the world class.
/// </summary>
public class WorldDebug : MonoBehaviour
{
    [SerializeField] private TileBase debugTile;
    [SerializeField] private SwizzledHFTTilemap debugTilemap;
    [SerializeField] private bool createOnStart = false;

    void Start()
    {
        if (createOnStart)
        {
            CreateGrid();
        }
    }

    void CreateGrid()
    {
        World world = GetComponent<World>();
        BoundsInt bounds = world.GetBounds();

        Debug.Log("Placing debug tiles for world bounds: " + bounds);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                PlaceDebugTileAtCell(cellPosition);
            }
        }
    }

    void PlaceDebugTileAtCell(Vector3Int vector3Int)
    {
        debugTilemap.SetTile(vector3Int, debugTile);
        GameObject placedTileObject = debugTilemap.GetInstantiatedObject(vector3Int);
        TileDebug tileDebug = placedTileObject.GetComponent<TileDebug>();

        tileDebug.SetCoordinates(new Vector2Int(vector3Int.x, vector3Int.y));        
    }

    [Command("ShowWorldGrid")]
    static void ShowGridCommand()
    {
        WorldDebug worldDebug = FindFirstObjectByType<WorldDebug>();
        worldDebug.CreateGrid();
    }
}
