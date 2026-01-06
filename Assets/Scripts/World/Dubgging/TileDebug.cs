using TMPro;
using UnityEngine;

public class TileDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coordinatesText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private SpriteRenderer borderRenderer;

    public void SetColor(Color color)
    {
        borderRenderer.color = color;
        coordinatesText.color = color;
        costText.color = color;
    }

    public void SetCoordinates(Vector2Int coordinates)
    {
        coordinatesText.text = $"Pos: ({coordinates.x}, {coordinates.y})";
    }

    public void SetCost(int cost)
    {
        costText.text = $"C: {cost}";
    }

}
