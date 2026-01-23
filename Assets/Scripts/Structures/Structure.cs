using UnityEngine;

// Require a Health component on the same GameObject
[RequireComponent(typeof(Health))]
public class Structure : MonoBehaviour
{
    [SerializeField] protected StructureTile tile; // Set by StructureTile when placed, for some reason this only works as a SerializeField
    public StructureTile Tile => tile;

    Health health;
    protected bool isVisualPreview = false;
    
    #region Lifecycle
    protected virtual void Update()
    {
        if (isVisualPreview) return;
        
        // Update logic goes here
    }

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        health.OnDeath += DestroyStructure; // Unsubscribe in DestroyStructure
    }

    virtual protected void DestroyStructure()
    {
        health.OnDeath -= DestroyStructure; // Unsubscribe to avoid potential issues
        
        // Remove the tile from the tilemap
        Vector3Int cellPosition = World.Instance.WorldToCell(transform.position);
        World.Instance.SetTileAt(cellPosition, null);
    }
    #endregion

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


}
