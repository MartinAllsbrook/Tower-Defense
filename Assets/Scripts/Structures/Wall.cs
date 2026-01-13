using UnityEngine;

public class Wall : Structure
{
    [Tooltip("Array of wall connection GameObjects in the order: N, NE, SE, S, SW, NW")]
    [SerializeField] GameObject[] wallConnections;

    void Start()
    {
        UpdateWallConnections(notifyNeighbors: true);
    }

    void UpdateWallConnections(bool notifyNeighbors = false)
    {
        World world = FindFirstObjectByType<World>();
        Vector2Int cellPosition = world.WorldToCell2(transform.position);
        
        Vector2Int[] offsets = World.GetNeighbors(cellPosition);

        // Check adjacent tiles for walls
        bool N  = world.HasStructureAt(cellPosition + offsets[0], StructureType.Wall);
        bool NE = world.HasStructureAt(cellPosition + offsets[1], StructureType.Wall);
        bool SE = world.HasStructureAt(cellPosition + offsets[2], StructureType.Wall);
        bool S  = world.HasStructureAt(cellPosition + offsets[3], StructureType.Wall);
        bool SW = world.HasStructureAt(cellPosition + offsets[4], StructureType.Wall);
        bool NW = world.HasStructureAt(cellPosition + offsets[5], StructureType.Wall);

        // Update wall connector based on adjacent walls
        wallConnections[0].SetActive(N);
        wallConnections[1].SetActive(NE);
        wallConnections[2].SetActive(SE);
        wallConnections[3].SetActive(S);
        wallConnections[4].SetActive(SW);
        wallConnections[5].SetActive(NW);

        // Notify neighboring walls to update their connections (but don't let them notify back)
        if (notifyNeighbors)
        {
            NotifyNeighborWall(world, cellPosition + offsets[0]);
            NotifyNeighborWall(world, cellPosition + offsets[1]);
            NotifyNeighborWall(world, cellPosition + offsets[2]);
            NotifyNeighborWall(world, cellPosition + offsets[3]);
            NotifyNeighborWall(world, cellPosition + offsets[4]);
            NotifyNeighborWall(world, cellPosition + offsets[5]);
        }
    }

    void NotifyNeighborWall(World world, Vector2Int neighborCell)
    {
        if (world.HasStructureAt(neighborCell, StructureType.Wall))
        {
            Structure structure = world.GetStructureAt(neighborCell);
            if (structure is Wall neighborWall)
            {
                neighborWall.UpdateWallConnections(notifyNeighbors: false);
            }
        }
    }
}
