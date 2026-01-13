using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] protected StructureData structureData;
    protected float health;
    public bool IsDestroyed => health <= 0f;
    public StructureData StructureData => structureData;

    void Awake()
    {
        health = structureData.maxHealth;
    }

    public bool DealDamage(float damage)
    {
        health -= damage;
        healthBar.SetFill(health / structureData.maxHealth);
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
        world.RemoveTileAt(cellPosition);
    }
}
