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

    void Awake()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            structurePlacer = player.GetComponent<StructurePlacer>();
        }
        player.OnMoneyChanged += UpdateAffordability;
    }

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

        UpdateAffordability();
    }

    void UpdateAffordability()
    {
        if (structureTile != null && structurePlacer != null)
            {
                if (structurePlacer.CanAffordStructure(structureTile))
                {
                    GetComponent<Button>().interactable = true;
                    costText.color = Color.white;
                }
                else
                {
                    GetComponent<Button>().interactable = false;
                    costText.color = Color.red;
                }
            }
    }

    public void OnButtonClicked()
    {
        if (structurePlacer != null && structureTile != null)
        {
            structurePlacer.EnterPlaceMode(structureTile.ID);
        }
    }
}
