using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceStructureButton : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI costText;

    StructureTile structureTile;
    StructurePlacer structurePlacer;

    public void Initialize(StructureTile structureTile, StructurePlacer structurePlacer)
    {
        this.structureTile = structureTile;
        this.structurePlacer = structurePlacer;

        if (icon != null)
            icon.sprite = structureTile.Icon;
        if (nameText != null)
            nameText.text = structureTile.Name;
        if (costText != null)
            costText.text = $"Cost: {structureTile.Cost}";
    }

    public void OnButtonClicked()
    {
        if (structurePlacer != null && structureTile != null)
        {
            structurePlacer.EnterPlaceMode(structureTile.ID);
        }
    }
}
