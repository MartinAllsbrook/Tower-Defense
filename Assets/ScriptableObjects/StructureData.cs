using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "Scriptable Objects/StructureData")]
public class StructureData : ScriptableObject
{
    [SerializeField] public StructureType objectType;

    [SerializeField] public string structureName;

    [Tooltip("Lower number means higher priority")]
    [SerializeField] public int priority; 

    [SerializeField] public WorldTile tile;

    [SerializeField] public float maxHealth;
}
