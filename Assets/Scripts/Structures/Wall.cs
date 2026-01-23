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
        Vector2Int cellPosition = World.Instance.WorldToCell2(transform.position);

        Vector2Int[] neighbors = World.GetNeighbors(cellPosition);

        for (int i = 0; i < 6; i++)
        {
            bool neighboringWall = World.Instance.HasStructureAt(neighbors[i], StructureType.Wall);
            bool neighboringTerrain = World.Instance.HasTileAt(neighbors[i], TileTag.Terrain);
            bool connect = neighboringWall || neighboringTerrain;
            wallConnections[i].SetActive(connect);
        }
    }
}
