using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField] StructureData structureData;
    float health;

    void Awake()
    {
        health = structureData.maxHealth;
    }
}
