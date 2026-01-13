using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public enum WorldSizeOption { Size15 = 15, Size31 = 31, Size63 = 63, Size127 = 127, Size255 = 255 }

    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap worldTilemap; 
    [SerializeField] WorldSizeOption worldSize = WorldSizeOption.Size63;
    [SerializeField] BiomeTile[] backgroundTiles;
    [SerializeField] WorldTile mountainTile;
    [SerializeField] WorldTile enemySpawnerTile;

    // Event that passes the updated grid to subscribers
    public event Action OnWorldUpdate;
    public int WorldSize => (int)worldSize;

    // ThetaStar thetaStar;
    GridCell[,] grid;
    BiomeID[,] biomeMap;
    BoundsInt bounds;

    void Start()
    {
        worldTilemap.origin = new Vector3Int(-(WorldSize / 2), -(WorldSize / 2), 0);
        worldTilemap.size = new Vector3Int(WorldSize, WorldSize, 1);
        worldTilemap.ResizeBounds();

        floorTilemap.origin = new Vector3Int(-(WorldSize / 2), -(WorldSize / 2), 0);
        floorTilemap.size = new Vector3Int(WorldSize, WorldSize, 1);
        floorTilemap.ResizeBounds();

        GenerateWorld();
        UpdateTilemap();
    }

    #region World Generation

    private void GenerateWorld()
    {
        GenerateBiomeTiles();
        GenerateTerrain();
        PlacePOI();
    }

    void GenerateBiomeTiles()
    {
        FastNoiseLite noise = new FastNoiseLite(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.05f);
        int halfSize = WorldSize / 2;

        biomeMap = new BiomeID[WorldSize, WorldSize];
        
        for (int x = -halfSize, i = 0; i < WorldSize; i++, x++)
        {
            for (int y = -halfSize, j = 0; j < WorldSize; j++, y++)
            {
                float value = (noise.GetNoise(i, j) + 1) / 2; // Normalize to 0-1

                BiomeTile tile = backgroundTiles[(int)(value * backgroundTiles.Length)];
                floorTilemap.SetTile(new Vector3Int(x, y, 0), tile);    

                biomeMap[i, j] = tile.tag;
            }
        }
    }

    void GenerateTerrain()
    {
        FastNoiseLite noise = new FastNoiseLite(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.1f);
        int halfSize = WorldSize / 2;
        
        for (int x = -halfSize, i = 0; i < WorldSize; i++, x++)
        {
            for (int y = -halfSize, j = 0; j < WorldSize; j++, y++)
            {
                float value = (noise.GetNoise(i, j) + 1) / 2; // Normalize to 0-1

                if (value < 0.2f)
                {
                    worldTilemap.SetTile(new Vector3Int(x, y, 0), mountainTile);
                }  
            }
        }
    }

    // Currently just places enemy spawners
    void PlacePOI()
    {
        Debug.Log("Placing Points of Interest...");
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
            if (!worldTilemap.HasTile(new Vector3Int(point.x, point.y, 0)))
            {
                Vector3Int cellPos = new Vector3Int(point.x, point.y, 0);
                worldTilemap.SetTile(cellPos, enemySpawnerTile);
            }
        }
    }

    List<Vector2Int> FindPointsInBiome(BiomeID biome, int count, int minSpacing, int maxAttempts = 1000)
    {
        int halfSize = WorldSize / 2;
        System.Random rand = new System.Random();
        List<Vector2Int> points = new List<Vector2Int>();
        while (points.Count < count && maxAttempts > 0)
        {
            maxAttempts--;
            int x = rand.Next(0, WorldSize);
            int y = rand.Next(0, WorldSize);

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

    #endregion

    #region World Modification

    public bool SetTileAt(Vector3Int cellPosition, WorldTile newTile)
    {
        if (!IsWithinBounds(cellPosition))
            return false;

        WorldTile existingTile = worldTilemap.GetTile<WorldTile>(cellPosition);

        // Only allow placing on a empty tile or removing an existing tile
        if (existingTile == null || newTile == null)
        {
            worldTilemap.SetTile(cellPosition, newTile);
            UpdateTilemap();
            UpdateNeighbors(cellPosition);
            return true;
        }

        return false;
    }

    void UpdateNeighbors(Vector3Int cellPosition)
    {
        Vector2Int[] offsets = GetNeighbors(new Vector2Int(cellPosition.x, cellPosition.y));

        foreach (var offset in offsets)
        {
            Vector3Int neighborPos = new Vector3Int(offset.x, offset.y, 0);
            Structure structure = GetStructureAt(new Vector2Int(neighborPos.x, neighborPos.y));
            if (structure != null)
            {
                structure.NeighborChanged();
            }
        }
    }

    public bool IsWithinBounds(Vector3Int cellPosition)
    {
        int halfSize = WorldSize / 2;
        return cellPosition.x >= -halfSize && cellPosition.x <= halfSize &&
               cellPosition.y >= -halfSize && cellPosition.y <= halfSize;
    }

    public void UpdateTilemap()
    {
        // TODO: Do we need to get the tilemap each time?
        worldTilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();        
        
        bounds = worldTilemap.cellBounds;
        grid = CreateGrid(worldTilemap);

        // Debugging Grid
        GridDebug gridDebug = GetComponent<GridDebug>();
        if (gridDebug != null)
        {
            gridDebug.HandleGridUpdate(grid, bounds);
        }

        // Notify all subscribers with the updated grid
        OnWorldUpdate?.Invoke();
    }

    #endregion

    #region Pathfinding and Grid Management

    GridCell[,] CreateGrid(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        GridCell[,] grid = new GridCell[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < bounds.size.x; x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < bounds.size.y; y++, j++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile is WorldTile taggedTile)
                {
                    grid[i, j] = new GridCell
                    {
                        Position = new Vector2Int(x, y),
                        Cost = taggedTile.Tag == TileTag.Structure ? 15 : 1,
                        Traversable = taggedTile.Tag != TileTag.Terrain,
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

    #endregion

    #region Utility Methods

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return worldTilemap.WorldToCell(worldPosition);
    }

    public Vector2Int WorldToCell2(Vector3 worldPosition)
    {
        Vector3Int vector3Int = worldTilemap.WorldToCell(worldPosition);
        return new Vector2Int(vector3Int.x, vector3Int.y);
    }

    public Vector2 CellToWorld(Vector3Int cellPosition)
    {
        return worldTilemap.CellToWorld(cellPosition);
    }

    public GridCell[,] GetGrid()
    {
        return grid;
    }

    public BoundsInt GetBounds()
    {
        return bounds;
    }

    /// <summary>
    /// Returns the StructureData ScriptableObject at the given cell position, or null if none exists.
    /// </summary>
    public Structure GetStructureAt(Vector2Int cellPosition)
    {
        Vector3Int cellPos = new Vector3Int(cellPosition.x, cellPosition.y, 0);
        TileBase tile = worldTilemap.GetTile(cellPos);
        if (tile is WorldTile worldTile && worldTile.Tag == TileTag.Structure)
        {
            GameObject structureObj = worldTilemap.GetInstantiatedObject(cellPos);
            if (structureObj != null)
            {
                Structure structure = structureObj.GetComponent<Structure>();
                return structure;
            }
        }

        // if (tile is StructureTile structureTile)
        // {
        //     Structure structure = structureTile.GameObject().GetComponent<Structure>();
        //     return structure;
        // }
        return null;
    }

    public bool HasTileAt(Vector2Int cellPosition, TileTag tag)
    {
        TileBase tile = worldTilemap.GetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0));
        return (tile is WorldTile worldTile && worldTile.Tag == tag);
    }

    public bool HasStructureAt(Vector2Int cellPosition, StructureType structureID)
    {
        TileBase tile = worldTilemap.GetTile(new Vector3Int(cellPosition.x, cellPosition.y, 0));
        return (tile is StructureTile structureTile && structureTile.ID == structureID);
    }

    #endregion

    #region Static Utility

    /// <summary>
    /// Returns the six neighboring cell positions for a given cell in an axial hex grid.
    /// </summary>
    /// <param name="cellPosition">The position of the cell in tilemap space</param>
    /// <returns>The positions of neighbors in tilemap space</returns>
    public static Vector2Int[] GetNeighbors(Vector2Int cellPosition)
    {
        // When column (Y coordinate) is even
        Vector2Int[] evenColOffsets = new Vector2Int[] {
            new Vector2Int(+1,  0), // N
            new Vector2Int( 0, +1), // NE
            new Vector2Int(-1, +1), // SE
            new Vector2Int(-1,  0), // S
            new Vector2Int(-1, -1), // SW
            new Vector2Int( 0, -1)  // NW
        };
        // When column (Y coordinate) is odd
        Vector2Int[] oddColOffsets = new Vector2Int[] {
            new Vector2Int(+1,  0), // N
            new Vector2Int(+1, +1), // NE
            new Vector2Int( 0, +1), // SE
            new Vector2Int(-1,  0), // S
            new Vector2Int( 0, -1), // SW
            new Vector2Int(+1, -1)  // NW
        };

        Vector2Int[] offsets = (cellPosition.y % 2 == 0) ? evenColOffsets : oddColOffsets;

        Vector2Int[] neighbors = new Vector2Int[6];
        for (int i = 0; i < offsets.Length; i++)
        {
            neighbors[i] = cellPosition + offsets[i];
        }

        return neighbors;
    }

    #endregion
}
