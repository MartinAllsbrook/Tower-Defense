using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] protected StructureTile tile; // Set by StructureTile when placed, for some reason this only works as a SerializeField
    public StructureTile Tile => tile;

    protected float health;
    public bool IsDestroyed => health <= 0f;
    protected bool isVisualPreview = false;

    void Awake()
    {
        health = tile.MaxHealth;
    }

    protected virtual void Update()
    {
        if (isVisualPreview) return;
        
        // Update logic goes here
    }

    public void SetAsVisualPreview(bool isPlaceable)
    {
        isVisualPreview = true;
        
        // Disable collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // Get all sprite renderers including children
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color color = sr.color;
            color.a = 0.5f; // Semi-transparent
            if (!isPlaceable)
                color = Color.Lerp(Color.white, Color.red, 0.4f);   // Less saturated red
            sr.color = color;
        }
    }

    // Called by StructureTile.GetTileData to automatically set the tile reference
    public void SetTile(StructureTile structureTile)
    {
        tile = structureTile;
    }

    public virtual void NeighborChanged() {}

    public bool DealDamage(float damage)
    {
        health -= damage;
        GameController.StructureTakenDamage();
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
