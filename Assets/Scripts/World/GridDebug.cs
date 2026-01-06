using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

public class GridDebug : MonoBehaviour
{
    [SerializeField] private bool showGrid = false;
    [SerializeField] private World world;
    [SerializeField] private TileDebug tileDebugPrefab;

    private List<GameObject> labels = new List<GameObject>();

    void Awake()
    {
        world.OnGridUpdated += HandleGridUpdate;
    }

    void HandleGridUpdate(GridCell[,] grid)
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
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                Vector3 worldPos = world.tilemap.CellToWorld(cellPos);
                Vector2Int gridPos = new Vector2Int(x - bounds.xMin, y - bounds.yMin);

                TileDebug tileLabel = Instantiate(tileDebugPrefab, worldPos, Quaternion.identity, this.transform);
                tileLabel.name = $"TileLabel_{x}_{y}";

                GridCell cell = grid[gridPos.x, gridPos.y];

                tileLabel.SetCoordinates(cell.Position);
                tileLabel.SetCost(cell.Cost); // Placeholder cost
                if (!cell.Traversable)
                {
                    tileLabel.SetColor(Color.red);
                }
                // if (line.Contains(cell.Position))
                // {
                //     tileLabel.SetColor(Color.green);
                // }
                labels.Add(tileLabel.gameObject);
            }
        }
    }

    List<Vector2Int> HexBresenhamTest()
    {
        List<Vector2Int> line = HexBresenham.HexLineDraw(-5, 4, 3, -7);
        foreach (var hex in line)
        {
            Debug.Log($"Hex: {hex}");
        }
        return line;
    }
}
