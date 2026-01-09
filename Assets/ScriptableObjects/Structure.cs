using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "Scriptable Objects/Structure")]
public class Structure : ScriptableObject
{
    [SerializeField] public StructureType objectType;
    [SerializeField] public string structureName;
    [Tooltip("Lower number means higher priority")]
    [SerializeField] public int priority; 
    [SerializeField] public WorldTile tile;

}
