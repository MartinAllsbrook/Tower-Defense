using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "Scriptable Objects/Structure")]
public class Structure : ScriptableObject
{
    [SerializeField] public StructureType objectType;
    [SerializeField] public string structureName;
    [SerializeField] public WorldTile tile;
}
