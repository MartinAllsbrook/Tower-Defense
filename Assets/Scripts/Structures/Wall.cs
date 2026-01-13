using UnityEngine;

public class Wall : Structure
{
    [Tooltip("Array of wall connection GameObjects in the order: N, NE, SE, S, SW, NW")]
    [SerializeField] GameObject[] wallConnections;

    void Start()
    {
        UpdateWallConnections();
    }

    public override void NeighborChanged()
    {
        UpdateWallConnections();
    }

    void UpdateWallConnections()
    {
        World world = FindFirstObjectByType<World>();
        Vector2Int cellPosition = world.WorldToCell2(transform.position);

        Vector2Int[] neighbors = World.GetNeighbors(cellPosition);

        for (int i = 0; i < 6; i++)
        {
            bool neighboringWall  = world.HasStructureAt(neighbors[i], StructureType.Wall);
            wallConnections[i].SetActive(neighboringWall);
        }
    }
}
