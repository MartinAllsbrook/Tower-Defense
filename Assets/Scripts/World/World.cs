using System;
using System.Collections.Generic;
using QFSW.QC;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{

    [SerializeField] SwizzledHFTTilemap floorTilemap;
    [SerializeField] SwizzledHFTTilemap worldTilemap; 
    [SerializeField] SwizzledHFTTilemap borderTilemap;
    
    [Tooltip("Must be odd number to have a center tile")]
    [SerializeField] Vector2Int worldSize = new Vector2Int(15, 63);
    [SerializeField] int numSpawners = 8;
    [SerializeField] BiomeTile[] biomeTiles;
    [SerializeField] WorldTile mountainTile;
    [SerializeField] WorldTile enemySpawnerTile;

    // Event that passes the updated grid to subscribers
    public event Action OnWorldUpdate;

    // ThetaStar thetaStar;
    GridCell[,] grid;
    BiomeID[,] biomeMap;
    BoundsInt bounds;

    public Vector2Int halfWorldSize => new Vector2Int(worldSize.x / 2, worldSize.y / 2);

    void Awake()
    {
        worldTilemap.SetOrigin(new Vector3Int(-halfWorldSize.x, -halfWorldSize.y, 0));
        worldTilemap.SetSize(new Vector3Int(worldSize.x, worldSize.y, 1));
        worldTilemap.ResizeBounds();

        floorTilemap.SetOrigin(new Vector3Int(-halfWorldSize.x, -halfWorldSize.y, 0));
        floorTilemap.SetSize(new Vector3Int(worldSize.x, worldSize.y, 1));
        floorTilemap.ResizeBounds();
        UpdateTilemap();
    }

    void Start()
    {
        GenerateWorld();
        UpdateTilemap();
    }

    #region World Generation

    private void GenerateWorld()
    {
        GenerateBiomeTiles();
        GenerateWalls();
        GenerateTerrain();
        PlacePOI();
    }

    void GenerateBiomeTiles()
    {
        FastNoiseLite noise = new FastNoiseLite(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.05f);

        biomeMap = new BiomeID[worldSize.x, worldSize.y];
        
        for (int x = -halfWorldSize.x, i = 0; i < worldSize.x; i++, x++)
        {
            for (int y = -halfWorldSize.y, j = 0; j < worldSize.y; j++, y++)
            {
                float value = (noise.GetNoise(i, j) + 1) / 2; // Normalize to 0-1

                BiomeTile tile = biomeTiles[(int)(value * 4)]; // We are just using first 4 biomes for now

                floorTilemap.SetTile(new Vector3Int(x, y, 0), tile);

                biomeMap[i, j] = tile.tag;
            }
        }
    }

    void GenerateWalls() 
    {
        int halfSizeX = halfWorldSize.x + 1;
        int halfSizeY = halfWorldSize.y + 1;
        for (int x = -halfSizeX - 2; x <= halfSizeX + 2; x++)
        {
            borderTilemap.SetTile(new Vector3Int(x, -halfSizeY - 0, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(x, -halfSizeY - 1, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(x, -halfSizeY - 2, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(x,  halfSizeY + 0, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(x,  halfSizeY + 1, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(x,  halfSizeY + 2, 0), mountainTile);
        }
        for (int y = -halfSizeY - 2; y <= halfSizeY + 2; y++)
        {
            borderTilemap.SetTile(new Vector3Int(-halfSizeX - 0, y, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(-halfSizeX - 1, y, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int(-halfSizeX - 2, y, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int( halfSizeX + 0, y, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int( halfSizeX + 1, y, 0), mountainTile);
            borderTilemap.SetTile(new Vector3Int( halfSizeX + 2, y, 0), mountainTile);
        }
    }

    void GenerateTerrain()
    {
        FastNoiseLite noise = new FastNoiseLite(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.1f);
        
        for (int x = -halfWorldSize.x, i = 0; i < worldSize.x; i++, x++)
        {
            for (int y = -halfWorldSize.y, j = 0; j < worldSize.y; j++, y++)
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

        List<Vector2Int> points = FindPointsInBiome(numSpawners, 5);
        foreach (var point in points)
        {
            SetTileAt(new Vector3Int(point.x, point.y, 0), enemySpawnerTile); // TODO: Should this use internal methods?
            
            // if (!worldTilemap.HasTile(new Vector3Int(point.x, point.y, 0)))
            // {
            //     Vector3Int cellPos = new Vector3Int(point.x, point.y, 0);
            //     worldTilemap.SetTile(cellPos, enemySpawnerTile);
            // }
        }
    }

    List<Vector2Int> FindPointsInBiome(int count, int minSpacing, int maxAttempts = 1000)
    {
        System.Random rand = new System.Random();
        List<Vector2Int> points = new List<Vector2Int>();
        while (points.Count < count && maxAttempts > 0)
        {
            maxAttempts--;
            int x = rand.Next(0, worldSize.x);
            int y = rand.Next(0, worldSize.y);

            // Check biome
            // if (biomeMap[x, y] != biome)
            //     continue;

            // Convert to tilemap coordinates
            Vector2Int candidate = new Vector2Int(x - halfWorldSize.x, y - halfWorldSize.y);
            bool tooClose = false;

            // Check if tile is already occupied
            if (worldTilemap.HasTile(new Vector3Int(candidate.x, candidate.y, 0)))
            {   
                continue;
            }

            // Check spacing
            foreach (var point in points)
            {
                if (Vector2Int.Distance(point, candidate) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose)
                continue;
            
            points.Add(candidate);
        }

        return points;
    }

    #endregion

    #region Biome Modification

    public void SetBiomeAt(Vector3Int cellPosition, BiomeID newBiome)
    {
        if (!IsWithinBounds(cellPosition))
            return;

        BiomeTile biomeTile = null;
        foreach (var tile in biomeTiles)
        {
            if (tile.tag == newBiome)
            {
                biomeTile = tile;
                break;
            }
        }

        if (biomeTile != null)
        {
            floorTilemap.SetTile(cellPosition, biomeTile);
            biomeMap[cellPosition.x + halfWorldSize.x, cellPosition.y + halfWorldSize.y] = newBiome;
        }
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
        return cellPosition.x >= -halfWorldSize.x && cellPosition.x <= halfWorldSize.x &&
               cellPosition.y >= -halfWorldSize.y && cellPosition.y <= halfWorldSize.y;
    }

    public void UpdateTilemap()
    {
        // // TODO: Do we need to get the tilemap each time?
        // worldTilemap = GameObject.FindWithTag("World Tilemap").GetComponent<Tilemap>();        
        
        bounds = worldTilemap.GetBounds();
        grid = CreateGrid(worldTilemap); // TODO: Major Swizzle

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

    GridCell[,] CreateGrid(SwizzledHFTTilemap tilemap)
    {
        BoundsInt bounds = tilemap.GetBounds(); // TODO: Maybe make a get swizzled bounds method
        GridCell[,] grid = new GridCell[bounds.size.x, bounds.size.y];
        for (int x = bounds.xMin, i = 0; i < bounds.size.x; x++, i++)
        {
            for (int y = bounds.yMin, j = 0; j < bounds.size.y; y++, j++)
            {
                TileBase tile = tilemap.GetTile<TileBase>(new Vector3Int(x, y, 0)); // TODO: Use proper type of tile brah
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
        Vector3Int vector3Int = WorldToCell(worldPosition); // This already swizzles
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
        TileBase tile = worldTilemap.GetTile<TileBase>(cellPos); // TODO: Use proper type of tile brah
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
        TileBase tile = worldTilemap.GetTile<TileBase>(new Vector3Int(cellPosition.x, cellPosition.y, 0));
        return tile is WorldTile worldTile && worldTile.Tag == tag;
    }

    public bool HasStructureAt(Vector2Int cellPosition, StructureType structureID)
    {
        TileBase tile = worldTilemap.GetTile<TileBase>(new Vector3Int(cellPosition.x, cellPosition.y, 0));
        return tile is StructureTile structureTile && structureTile.ID == structureID;
    }

    #endregion

    #region Utility

    /// <summary>
    /// Returns the six neighboring cell positions for a given cell in an axial hex grid.
    /// </summary>
    /// <param name="cellPosition">The position of the cell in tilemap space</param>
    /// <returns>The positions of neighbors in tilemap space</returns>
    public static Vector2Int[] GetNeighbors(Vector2Int cellPosition)
    {
        // When column (X coordinate) is even
        Vector2Int[] evenColOffsets = new Vector2Int[] {
            new Vector2Int( 0, +1), // N
            new Vector2Int(+1,  0), // NE
            new Vector2Int(+1, -1), // SE
            new Vector2Int( 0, -1), // S
            new Vector2Int(-1, -1), // SW
            new Vector2Int(-1,  0)  // NW
        };
        // When column (X coordinate) is odd
        Vector2Int[] oddColOffsets = new Vector2Int[] {
            new Vector2Int( 0, +1), // N
            new Vector2Int(+1, +1), // NE
            new Vector2Int(+1,  0), // SE
            new Vector2Int( 0, -1), // S
            new Vector2Int(-1,  0), // SW
            new Vector2Int(-1, +1)  // NW
        };

        Vector2Int[] offsets = (cellPosition.x % 2 == 0) ? evenColOffsets : oddColOffsets;

        Vector2Int[] neighbors = new Vector2Int[6];
        for (int i = 0; i < offsets.Length; i++)
        {
            neighbors[i] = cellPosition + offsets[i];
        }

        return neighbors;
    }

    #endregion

    #region Debugging
    void TestMany()
    {
        // Debug.Log($"World Position: {new Vector3(0,0,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(0,0,0))}");
        // Debug.Log($"World Position: {new Vector3(1,1,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(1,1,0))}");
        // Debug.Log($"World Position: {new Vector3(-1,-1,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-1,-1,0))}");
        // Debug.Log($"World Position: {new Vector3(10,5,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(10,5,0))}");
        // Debug.Log($"World Position: {new Vector3(-10,-5,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-10,-5,0))}");
        // Debug.Log($"World Position: {new Vector3(0.5f,0.5f,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(0.5f,0.5f,0))}");
        // Debug.Log($"World Position: {new Vector3(-0.5f,-0.5f,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-0.5f,-0.5f,0))}");
        // Debug.Log($"World Position: {new Vector3(2,3,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(2,3,0))}");
        // Debug.Log($"World Position: {new Vector3(-2,-3,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-2,-3,0))}");
        // Debug.Log($"World Position: {new Vector3(5,-5,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(5,-5,0))}");
        // Debug.Log($"World Position: {new Vector3(-5,5,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-5,5,0))}");
        // Debug.Log($"World Position: {new Vector3(3.7f,-2.4f,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(3.7f,-2.4f,0))}");
        // Debug.Log($"World Position: {new Vector3(-3.7f,2.4f,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-3.7f,2.4f,0))}");
        // Debug.Log($"World Position: {new Vector3(15,15,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(15,15,0))}");
        // Debug.Log($"World Position: {new Vector3(-15,-15,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-15,-15,0))}");
        // Debug.Log($"World Position: {new Vector3(20,-20,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(20,-20,0))}");
        // Debug.Log($"World Position: {new Vector3(-20,20,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-20,20,0))}");
        // Debug.Log($"World Position: {new Vector3(0,10,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(0,10,0))}");
        // Debug.Log($"World Position: {new Vector3(0,-10,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(0,-10,0))}");
        // Debug.Log($"World Position: {new Vector3(10,0,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(10,0,0))}");
        // Debug.Log($"World Position: {new Vector3(-10,0,0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-10,0,0))}");
        // Debug.Log($"World Position: {new Vector3(7.25f, -8.75f, 0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(7.25f, -8.75f, 0))}");
        // Debug.Log($"World Position: {new Vector3(-7.25f, 8.75f, 0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-7.25f, 8.75f, 0))}");
        // Debug.Log($"World Position: {new Vector3(12.5f, 12.5f, 0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(12.5f, 12.5f, 0))}");
        // Debug.Log($"World Position: {new Vector3(-12.5f, -12.5f, 0)}, Cell Position: {worldTilemap.WorldToCell(new Vector3(-12.5f, -12.5f, 0))}");
    }

    [Command]
    static public void TestManyPoints()
    {
        World world = FindFirstObjectByType<World>();
        world.TestMany();
    } 
    #endregion
}
