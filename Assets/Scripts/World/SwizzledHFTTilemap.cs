using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A swizzled wrapper class for Unity Hexagonal Flat Top Tilemaps, because they mix their x and y axes.
/// </summary>
[RequireComponent(typeof(Tilemap))]
public class SwizzledHFTTilemap : MonoBehaviour
{
    Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    #region Tilemap Methods
    public void SetOrigin(Vector3Int origin)
    {
        tilemap.origin = Swizzle3Int(origin);
    }

    public void SetSize(Vector3Int center)
    {
        tilemap.size = Swizzle3Int(center);
    }

    public T GetTile<T>(Vector3Int position) where T : TileBase
    {
        Vector3Int swizzledPos = Swizzle3Int(position);
        return tilemap.GetTile<T>(swizzledPos);
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        Vector3Int swizzledPos = Swizzle3Int(position);
        tilemap.SetTile(swizzledPos, tile);
    }

    public BoundsInt GetBounds()
    {
        return SwizzleBounds(tilemap.cellBounds);
    }

    public GameObject GetInstantiatedObject(Vector3Int position)
    {
        Vector3Int swizzledPos = Swizzle3Int(position);
        return tilemap.GetInstantiatedObject(swizzledPos);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        Vector3Int swizzledPos = Swizzle3Int(cellPosition);
        return tilemap.CellToWorld(swizzledPos);
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPosition);
        return Swizzle3Int(cellPos);
    }
    #endregion

    #region Helper Methods
    Vector3Int Swizzle3Int(Vector3Int original)
    {
        return new Vector3Int(original.y, original.x, original.z);
    }

    static BoundsInt SwizzleBounds(BoundsInt bounds)
    {
        return new BoundsInt(
            new Vector3Int(bounds.yMin, bounds.xMin, bounds.zMin),
            new Vector3Int(bounds.size.y, bounds.size.x, bounds.size.z)
        );
    }
    #endregion
}
