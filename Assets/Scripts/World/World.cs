using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;



public class World : MonoBehaviour
{
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] public Tilemap tilemap; // PUBLIC FOR DEBUGGING

    public enum WorldSizeOption { Size15 = 15, Size31 = 31, Size63 = 63, Size127 = 127, Size255 = 255 }
    [SerializeField] private WorldSizeOption worldSize = WorldSizeOption.Size63;
    [SerializeField] private BiomeTile[] backgroundTiles;
    [SerializeField] private TaggedTile enemySpawnerTile;

    // Event that passes the updated grid to subscribers
    public event Action<GridCell[,]> OnGridUpdated;

    public GridCell[,] grid;

    public BoundsInt bounds;

    private BiomeID[,] biomeMap;


    void Start()
    {
        GenerateWorld();
        UpdateTilemap();
    }

    private void GenerateWorld()
    {
        GenerateBiomeTiles();
        PlacePOI();
    }

    void GenerateBiomeTiles()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.05f);
        int halfSize = (int)worldSize / 2;

        biomeMap = new BiomeID[(int)worldSize, (int)worldSize];
        
        for (int x = -halfSize, i = 0; i < (int)worldSize; i++, x++)
        {
            for (int y = -halfSize, j = 0; j < (int)worldSize; j++, y++)
            {
                float value = (noise.GetNoise(i, j) + 1) / 2; // Normalize to 0-1
                Debug.Log(value);

                BiomeTile tile = backgroundTiles[(int)(value * backgroundTiles.Length)];
                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), tile);    

                biomeMap[i, j] = tile.tag;
            }
        }
    }

    // Currently just places enemy spawners
    void PlacePOI()
    {
        int numPoints;
        switch (worldSize)
        {
            case WorldSizeOption.Size15:
                numPoints = 1;
                break;
            case WorldSizeOption.Size31:
                numPoints = 4;
                break;
            case WorldSizeOption.Size63:
                numPoints = 16;
                break;
            case WorldSizeOption.Size127:
                numPoints = 64;
                break;
            case WorldSizeOption.Size255:
                numPoints = 256;
                break;
            default:
                numPoints = 2;
                break;
        }

        List<Vector2Int> points = FindPointsInBiome(BiomeID.cursed, numPoints, 5);
        foreach (var point in points)
        {
            Vector3Int cellPos = new Vector3Int(point.x, point.y, 0);
            tilemap.SetTile(cellPos, enemySpawnerTile);
        }
    }

    List<Vector2Int> FindPointsInBiome(BiomeID biome, int count, int minSpacing, int maxAttempts = 100)
    {
        int halfSize = (int)worldSize / 2;
        System.Random rand = new System.Random();
        List<Vector2Int> points = new List<Vector2Int>();

        while (points.Count < count && maxAttempts > 0)
        {
            maxAttempts--;
            int x = rand.Next(0, (int)worldSize);
            int y = rand.Next(0, (int)worldSize);

            if (biomeMap[x, y] != biome)
                continue;

            Vector2Int candidate = new Vector2Int(x - halfSize, y - halfSize);
            bool tooClose = false;

            foreach (var point in points)
            {
                if (Vector2Int.Distance(point, candidate) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                points.Add(candidate);
                Debug.Log($"Found point in biome {biome} at {candidate}");
            }
        }

        return points;
    }

    public void UpdateTilemap()
    {
        tilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();
        tilemap.CompressBounds(); // Optional: compress bounds to fit tiles
        bounds = tilemap.cellBounds;
        grid = CreateGrid(tilemap);

        // Notify all subscribers with the updated grid
        OnGridUpdated?.Invoke(grid);
    }

    GridCell[,] CreateGrid(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        GridCell[,] grid = new GridCell[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < bounds.size.x; x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < bounds.size.y; y++, j++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile is TaggedTile taggedTile)
                {
                    grid[i, j] = new GridCell
                    {
                        Position = new Vector2Int(x, y),
                        Cost = taggedTile.tag == TileTag.Structure ? 15 : 1,
                        Traversable = taggedTile.tag != TileTag.Terrain,
                    };
                }
                else
                {
                    grid[i, j] = new GridCell
                    {
                        Position = new Vector2Int(x, y),
                        Cost = 1,
                        Traversable = true,
                    };
                }
            }
        }

        return grid;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return tilemap.CellToWorld(cellPosition);
    }
}
