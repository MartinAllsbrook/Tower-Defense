using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GridDebug : MonoBehaviour
{
    [SerializeField] private Tilemap walkableTilemap;

    void Start()
    {
        // Tilemap walkableTilemap = GameObject.FindWithTag("Walkable Tilemap").GetComponent<Tilemap>();
        walkableTilemap.CompressBounds(); // Optional: compress bounds to fit tiles
        BoundsInt bounds = walkableTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = walkableTilemap.CellToWorld(cellPosition);

                // Create text to show coordinates
                GameObject textObj = new GameObject($"GridDebug_{x}_{y}");
                textObj.transform.position = worldPosition;
                textObj.transform.SetParent(transform);

                TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
                textMesh.text = $"{x}, {y}";
                textMesh.fontSize = 3;
                textMesh.alignment = TextAlignmentOptions.Center;

                // Red if there is a tile, green if not
                if (walkableTilemap.HasTile(cellPosition))
                {
                    textMesh.color = Color.red;
                }
                else
                {
                    textMesh.color = Color.green;
                }
            }
        }
    }
}
