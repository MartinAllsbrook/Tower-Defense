using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

public class GridDebug : MonoBehaviour
{
    [SerializeField] private bool showGrid = false;
    [SerializeField] private World world;

    private List<GameObject> labels = new List<GameObject>();

    void Awake()
    {
        world.OnGridUpdated += HandleGridUpdate;
    }

    void HandleGridUpdate(Vector3Int[,] grid)
    {
        // Clear existing labels
        foreach (var label in labels)
        {
            Destroy(label);
        }

        if (!showGrid)
            return;

        BoundsInt bounds = world.tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 worldPosition = world.tilemap.CellToWorld(cellPosition);

                CreateLabel(worldPosition, world.tilemap.HasTile(cellPosition), x, y);
            }
        }
    }

    // void Start()
    // {
    //     if (!showGrid)
    //         return;

    //     // Tilemap walkableTilemap = GameObject.FindWithTag("Walkable Tilemap").GetComponent<Tilemap>();
    //     walkableTilemap.CompressBounds(); // Optional: compress bounds to fit tiles
    //     BoundsInt bounds = walkableTilemap.cellBounds;

    //     for (int x = bounds.xMin; x < bounds.xMax; x++)
    //     {
    //         for (int y = bounds.yMin; y < bounds.yMax; y++)
    //         {
    //             Vector3Int cellPosition = new Vector3Int(x, y, 0);
    //             Vector3 worldPosition = walkableTilemap.CellToWorld(cellPosition);

    //             CreateLabel(worldPosition, !walkableTilemap.HasTile(cellPosition), x, y);
    //         }
    //     }
    // }

    private void CreateLabel(Vector3 worldPosition, bool isBlocked, int x, int y)
    {
        GameObject textObj = new GameObject($"GridDebug_{x}_{y}");
        textObj.transform.position = worldPosition;
        textObj.transform.SetParent(transform);

        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.text = $"{x}, {y}";
        textMesh.fontSize = 3;
        textMesh.alignment = TextAlignmentOptions.Center;

        textMesh.color = isBlocked ? Color.red : Color.green;

        labels.Add(textObj);
    }
}
