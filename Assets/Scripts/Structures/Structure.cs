using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] protected StructureTile tile;
    public StructureTile Tile => tile;

    protected float health;
    public bool IsDestroyed => health <= 0f;

    void Awake()
    {
        health = tile.MaxHealth;
    }

    public virtual void NeighborChanged()
    {
        
    }

    public bool DealDamage(float damage)
    {
        health -= damage;
        healthBar.SetFill(health / tile.MaxHealth);
        if (health <= 0)
        {
            DestroyStructure();
            return true;
        }
        return false;
    }

    virtual protected void DestroyStructure()
    {
        World world = FindFirstObjectByType<World>();
        
        // Remove the tile from the tilemap
        Vector3Int cellPosition = world.WorldToCell(transform.position);
        world.SetTileAt(cellPosition, null);
    }
}
