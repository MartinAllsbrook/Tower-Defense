using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] protected StructureData structureData;
    protected float health;

    void Awake()
    {
        health = structureData.maxHealth;
    }
}
